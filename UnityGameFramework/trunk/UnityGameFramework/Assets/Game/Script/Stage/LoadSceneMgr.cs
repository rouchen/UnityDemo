using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.SceneManagement;

//! 場景路徑資料.
public class ScenePathData
{
    public string name;
    public string idType;
}

//! 場景資料.
public class LoadSceneData
{
    public ScenePathData parentScene;
    public List<ScenePathData> childSceneList;
}

//! 要Load的場景名稱.
public class LoadSceneRealData
{
    //! 主場景.
    public string parentSceneName;
    //! 子場景.
    public List<string> childSceneNameList = new List<string>();
    //! 非同步載入.
    public bool async;
    //! 非同步載入，是否載入完自動開啟.
    public bool asyncSceneAutoActive;
    //! 是否已在Loading.
    public bool isLoading;
}

public class LoadSceneMgr : MonoBehaviour
{
    //! 場景List.
    List<LoadSceneData> loadSceneDataList = new List<LoadSceneData>();

    //! 要Load的場景的實際資料.
    List<LoadSceneRealData> loadSceneRealDataList = new List<LoadSceneRealData>();

    //LoadSceneRealData loadSceneRealData;

    //! 異步載入的 op 清單.
    List<AsyncOperation> asyncOP = new List<AsyncOperation>();
    //! 異步載入後要被清除的 scene.
    List<Scene> toBeUnloadScene = new List<Scene>();

    bool isStartAsyncAutoActive = false;
    float asyncAutoActiveTime = 0;
    float currentTime = 0;
    //! 是否初始化.
    bool isInitialize;

    //! Singleton.
    static LoadSceneMgr instance = null;
    public static LoadSceneMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("LoadSceneMgr");
                instance = go.AddComponent<LoadSceneMgr>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    //===================================

    /// <summary>
    /// 初始化.
    /// </summary>
    void Initialize()
    {
        if (isInitialize)
        {
            return;
        }
        LoadScenesFile();

        isInitialize = true;
    }

    /// <summary>
    /// 載入場景檔案.
    /// </summary>
    void LoadScenesFile()
    {
        string assetbundelName = "LoadScene";
        string assetName = "LoadScene.xml";

        LoadAssetBundleMgr.Singleton.SetDownloadDirectory(FileDirectory.GetDataResourceAssetbundlePath());
        LoadAssetBundleMgr.Singleton.LoadAssetbundle(assetbundelName, assetName);
        
        TextAsset resObject = LoadAssetBundleMgr.Singleton.GetAsset(assetbundelName, assetName) as TextAsset;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(resObject.text);
        ParseScenesData(xmlDoc.FirstChild);
        xmlDoc = null;
        resObject = null;

        LoadAssetBundleMgr.Singleton.UnloadAssetbundleImmediately(assetbundelName, true);
    }

    /// <summary>
    /// 分析場景資料.
    /// </summary>
    /// <param name="parentNode">xml父節點</param>
    void ParseScenesData(XmlNode parentNode)
    {
        loadSceneDataList.Clear();

        for (int i = 0; i < parentNode.ChildNodes.Count; i++)
        {
            //! 忽略註解.
            if (parentNode.ChildNodes[i].NodeType == XmlNodeType.Comment)
            {
                continue;
            }

            LoadSceneData loadSceneData = new LoadSceneData();
            
            //! Parent.
            loadSceneData.parentScene = new ScenePathData();
            loadSceneData.parentScene.name = parentNode.ChildNodes[i].Attributes["Name"].InnerText;
            loadSceneData.parentScene.idType = parentNode.ChildNodes[i].Attributes["IdType"].InnerText;

            //! Child List.
            loadSceneData.childSceneList = new List<ScenePathData>();
            ParseChildScenesData(parentNode.ChildNodes[i], loadSceneData.childSceneList);

            loadSceneDataList.Add(loadSceneData);
        }    
    }

    /// <summary>
    /// 分析子場景資料.
    /// </summary>
    /// <param name="parentNode">xml父節點</param>
    /// <param name="sceneDataList">子場景List</param>
    void ParseChildScenesData(XmlNode parentNode, List<ScenePathData> sceneDataList)
    {
        for (int i = 0; i < parentNode.ChildNodes.Count; i++)
        {
            //! 忽略註解.
            if (parentNode.ChildNodes[i].NodeType == XmlNodeType.Comment)
            {
                continue;
            }

            ScenePathData scenePathData = new ScenePathData();
            scenePathData.name = parentNode.ChildNodes[i].Attributes["Name"].InnerText;
            scenePathData.idType = parentNode.ChildNodes[i].Attributes["IdType"].InnerText;

            sceneDataList.Add(scenePathData);
        }
    }

    /// <summary>
    /// 載入場景
    /// </summary>
    /// <param name="sceneName">場景名稱</param>
    /// <param name="async">是否非同步</param>
    /// <param name="asyncSceneAutoActive">非同步時, 是否自動 Active Scene, 否則手動檢查 IsLoadSceneAsyncApproximatelyDone() 再呼叫 LoadSceneAsyncStartAutoActive() </param>
    public void LoadScene(string sceneName, bool async, bool asyncSceneAutoActive = true)
    {
        Initialize();

        //! 取得要Load場景的全部真實名稱.
        LoadSceneRealData sceneRealData = GetLoadSceneRealData(sceneName, async, asyncSceneAutoActive);

        //! 不使用下面code

        ////! 同步載入則count++
        ////! 非同步前，則先檢查是否已有同步的載入
        ////! bug出現在，同個frame先同步再非同步載入，會發生兩個場景都是呈現同步載入狀態，同時出現兩個場景，可能是unity的SceneManager不允許同時使用同步和非同步。
        //if (sceneRealData.async)
        //{
        //    if (count != 0)
        //    {
        //        sceneRealData = null;
        //        return;
        //    }
        //}
        //else
        //{
        //    count++;
        //}
        loadSceneRealDataList.Add(sceneRealData);

#if ASSETBUNDLE
        DownloadAssetBundle(sceneRealData, async);
        //! 同步載入，直接開始Load.
        if(!async)
        {
            LoadSceneInternal(sceneRealData);
        }
#else //! ASSETBUNDLE.
        LoadSceneInternal(sceneRealData);
#endif //! ASSETBUNDLE.

    }

    /// <summary>
    /// 下載 AssetBundle.
    /// </summary>
    /// <param name="sceneRealData"></param>
    /// <param name="isAsync">是否非同步</param>
    void DownloadAssetBundle(LoadSceneRealData sceneRealData, bool isAsync)
    {
        //! 設定AssetBundle下載路徑.
        LoadAssetBundleMgr.Singleton.SetDownloadDirectory(FileDirectory.GetStageAssetBundlePath());

        if (isAsync)
        {
            //! 下載AssetBundle.
            LoadAssetBundleMgr.Singleton.LoadAssetbundleAsync(sceneRealData.parentSceneName);

            for (int i = 0; i < sceneRealData.childSceneNameList.Count; i++)
            {
                LoadAssetBundleMgr.Singleton.LoadAssetbundleAsync(sceneRealData.childSceneNameList[i]);
            }
        }
        else
        {
            //! 下載AssetBundle.
            LoadAssetBundleMgr.Singleton.LoadAssetbundle(sceneRealData.parentSceneName);

            for (int i = 0; i < sceneRealData.childSceneNameList.Count; i++)
            {
                LoadAssetBundleMgr.Singleton.LoadAssetbundle(sceneRealData.childSceneNameList[i]);
            }
        }
    }

    /// <summary>
    /// 載入場景(使用AssetBundle).
    /// </summary>
    /// <param name="sceneRealData"></param>
    void LoadSceneFromAssetBundleAsync(LoadSceneRealData sceneRealData)
    {
        //! 未載入場景或已載入中，不做處理.
        if (sceneRealData == null || sceneRealData.isLoading)
        {
            return;
        }

        //! 檢查是否下載完成.
        //! 檢查主場景.
        if (!LoadAssetBundleMgr.Singleton.IsAssetbundleLoaded(sceneRealData.parentSceneName))
        {
            return;
        }

        //! 檢查子場景.
        for (int i = 0; i < sceneRealData.childSceneNameList.Count; i++)
        {
            if (!LoadAssetBundleMgr.Singleton.IsAssetbundleLoaded(sceneRealData.childSceneNameList[i]))
            {
                return;
            }
        }

        //! 全部AssetBundle下載完成.
        //! 開始載入場景.
        LoadSceneInternal(sceneRealData);
        sceneRealData.isLoading = true;
    }
    /// <summary>
    /// 實際Load Scene.
    /// </summary>
    /// <param name="sceneRealData"></param>
    void LoadSceneInternal(LoadSceneRealData sceneRealData)
    {
        if (sceneRealData.async)
        {
            asyncOP.Clear();
            toBeUnloadScene.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                toBeUnloadScene.Add(SceneManager.GetSceneAt(i));
            }
            LoadChildScene(sceneRealData, false);


            //parent scene 最後 load 才抓得到 child scene 的東西.
            StartCoroutine(LoadSceneAsync(sceneRealData.parentSceneName, LoadSceneMode.Additive));

            if (sceneRealData.asyncSceneAutoActive)
            {
                LoadSceneAsyncStartAutoActive(0);
            }
        }
        else
        {
            //! Parent Scene.
            SceneManager.LoadScene(sceneRealData.parentSceneName);
            //! Child Scene.
            LoadChildScene(sceneRealData, false);

#if ASSETBUNDLE
            UnloadSceneAssetbundle(false, false);
#endif //! ASSETBUNDLE.
        }
    }

    /// <summary>
    /// Unload場景的Aseetbundle.
    /// </summary>
    /// <param name="unloadAll">Assetbundle unload 參數(連建出來的物件都刪掉)</param>
    /// <param name="forceUnload">就算被reference多次也強制刪除</param>
    void UnloadSceneAssetbundle(bool unloadAll = false, bool forceUnload = false)
    {
        //! 釋放場景的assetbundle.
        while (loadSceneRealDataList.Count > 0)
        {
            LoadSceneRealData sceneRealData = loadSceneRealDataList[0];

            //! 釋放場景AssetBundle.
            LoadAssetBundleMgr.Singleton.UnloadAssetbundle(sceneRealData.parentSceneName, unloadAll, forceUnload);

            for (int j = 0; j < sceneRealData.childSceneNameList.Count; j++)
            {
                LoadAssetBundleMgr.Singleton.UnloadAssetbundle(sceneRealData.childSceneNameList[j], unloadAll, forceUnload);
            }

            loadSceneRealDataList.RemoveAt(0);
        }

        Debug.LogWarning("loadSceneRealDataList Count: " + loadSceneRealDataList.Count.ToString());
    }

    /// <summary>
    /// 取得要Load場景的全部實際資料.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="async"></param>
    /// <param name="asyncSceneAutoActive"></param>
    /// <returns></returns>
    LoadSceneRealData GetLoadSceneRealData(string sceneName, bool async, bool asyncSceneAutoActive)
    {
        LoadSceneRealData sceneRealData = new LoadSceneRealData();
        sceneRealData.async = async;
        sceneRealData.asyncSceneAutoActive = asyncSceneAutoActive;

        LoadSceneData sceneData = FindSceneData(sceneName);

        //! 取得Parent SceneName.
        string parentName = sceneData.parentScene.name;
        //! 如果有idType，就取得遊戲中對應id.
        if (sceneData.parentScene.idType != "")
        {
            int id = GetIdByIdType(sceneData.parentScene.idType);
            parentName += id.ToString();
        }
        sceneRealData.parentSceneName = parentName;

        //! 取得Child SceneName.
        for (int i = 0; i < sceneData.childSceneList.Count; i++)
        {
            ScenePathData childPathData = sceneData.childSceneList[i];

            string childName = childPathData.name;
            //! 如果有idType，就取得遊戲中對應id.
            if (childPathData.idType != "")
            {
                int id = GetIdByIdType(childPathData.idType);
                childName += id.ToString();
            }
            sceneRealData.childSceneNameList.Add(childName);
        }

        return sceneRealData;
    }

    /// <summary>
    /// 檢查場景是否已在Loading列表中.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    bool IsLoadSceneRealDataExist(string sceneName)
    {
        for (int i = 0; i < loadSceneRealDataList.Count; i++)
        {
            if (loadSceneRealDataList[i].parentSceneName == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Load子場景.
    /// </summary>
    /// <param name="sceneRealData"></param>
    /// <param name="checkLoad"></param>
    void LoadChildScene(LoadSceneRealData sceneRealData, bool checkLoad)
    {
        List<string> childSceneList = sceneRealData.childSceneNameList;
        for (int i = 0; i < childSceneList.Count; i++)
        {
            //! 檢查是否load過.
            if (checkLoad)
            {
                Scene scene = SceneManager.GetSceneByName(childSceneList[i]);
                if (scene.IsValid())
                {
                    continue;
                }
            }

            //! Load場景.
            if (sceneRealData.async)
            {
                //SceneManager.LoadSceneAsync(childSceneFullName, LoadSceneMode.Additive);
                StartCoroutine(LoadSceneAsync(childSceneList[i], LoadSceneMode.Additive));

            }
            else
            {
                SceneManager.LoadScene(childSceneList[i], LoadSceneMode.Additive);
            }

        }
    }

    /// <summary>
    /// 載入近度是否將近完成.
    /// </summary>
    /// <returns></returns>
    public bool IsLoadSceneAsyncApproximatelyDone()
    {
        Initialize();

        if (asyncOP.Count == 0)
        {
            //! 避開若無緣無故使用LoadSceneAsyncStartAutoActive開啟isStartAsyncAutoActive = true;
            //! 會造成update跑起來沒意義，浪費效能
            //! 判斷沒有非同步載入時，將update的isStartAsyncAutoActive關閉
            isStartAsyncAutoActive = false;
            return false;
        }

        bool alldone = true;

        for (int i = 0; i < asyncOP.Count; i++)
        {
            //! 多個判斷progress是否為1.0f，可以使非同步的卸載其他場景
            //! 避開非同步載入變成同步載入bug，造成progress直接變成1.0f
            if (asyncOP[i].progress != 1.0f)
            {
                alldone &= Mathf.Approximately(asyncOP[i].progress, 0.9f);
            }
            else
            {
                alldone &=true;
            }
        }

        return alldone;
    }

    /// <summary>
    /// 載入近度是否完成.(不代表場景己載入).
    /// </summary>
    /// <returns></returns>
    bool IsLoadSceneAsyncDone()
    {
        bool alldone = true;
        for (int i = 0; i < asyncOP.Count; i++)
        {
            alldone &= asyncOP[i].isDone;
        }

        return alldone;
    }

    /// <summary>
    /// 場景是否載入完成.
    /// </summary>
    /// <returns></returns>
    bool IsLoadSceneAsyncLoaded()
    {
        bool alldone = true;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            alldone &= SceneManager.GetSceneAt(i).isLoaded;
        }

        return alldone;
    }

    /// <summary>
    /// active 己載入的場景.
    /// </summary>
    public void LoadSceneAsyncActiveLoadedScene()
    {
        Initialize();

        for (int i = 0; i < asyncOP.Count; i++)
        {
            asyncOP[i].allowSceneActivation = true; ;
        }

        //防止下個場抓錯鏡頭.
        Camera cm = Camera.main;
        if (cm != null)
        {
            cm.tag = "Untagged";
        }
        StartCoroutine(LoadSceneAsyncUnloadAndSetActiveScene());
    }
    
    /// <summary>
    /// 開始異步載入場景.
    /// </summary>
    /// <param name="scenename"></param>
    /// <param name="lsm"></param>
    /// <returns></returns>
    IEnumerator LoadSceneAsync(string scenename, LoadSceneMode lsm)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(scenename, lsm);
        ao.allowSceneActivation = false;
        asyncOP.Add(ao);
        yield return ao;
    }

    /// <summary>
    /// 關始 unload 之前的場景, 及設定目前 active 場景.
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneAsyncUnloadAndSetActiveScene()
    {
        while (true)
        {
            if (IsLoadSceneAsyncLoaded())
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
                for (int i = 0; i < toBeUnloadScene.Count; i++)
                {
                    SceneManager.UnloadScene(toBeUnloadScene[i].name);
                }

                //! 非同步場景卸載，不是用unity的LoadScene是自行卸載，
                //! 所以自己Call Resources.UnloadUnusedAssets().
                Resources.UnloadUnusedAssets();

                break;
            }
            yield return null;
        }

#if ASSETBUNDLE
        UnloadSceneAssetbundle(false, false);
#endif //! ASSETBUNDLE.
    }


    /// <summary>
    /// 場景資料.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    LoadSceneData FindSceneData(string sceneName)
    {
        for (int i = 0; i < loadSceneDataList.Count; i++)
        {
            if (sceneName == loadSceneDataList[i].parentScene.name)
            {
                return loadSceneDataList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 取得場景中root GameObject中的Component(只取第一個找到的Component).
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="sceneName">場景名稱</param>
    /// <returns></returns>
    public T GetSceneRootComponenet<T>(string sceneName)
    {
        Initialize();

        T t = default(T);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.IsValid())
        {
            GameObject[] goArray = scene.GetRootGameObjects();
            for (int i = 0; i < goArray.Length; i++)
            {
                t = goArray[i].GetComponent<T>();
                if (t != null)
                {
                    return t;
                }
            }
        }

        return t;
    }

    /// <summary>
    /// 取得場景中root下的同名稱GameObject.
    /// </summary>
    /// <param name="sceneName">場景名稱</param>
    /// <param name="goName">GameObject名稱</param>
    /// <returns></returns>
    public GameObject GetSceneGameObjectByName(string sceneName, string goName)
    {
        Initialize();

        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.IsValid())
        {
            GameObject[] goArray = scene.GetRootGameObjects();
            for (int i = 0; i < goArray.Length; i++)
            {
                if (goArray[i].name == goName)
                {
                    return goArray[i];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 子場景如果沒Load進來就load子場景
    /// (在流程中的Awake加這個檢查，就能在Editor模式下，開場景時，就把子場景一起load進來).
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadChildScenesIfNotLoad(string sceneName)
    {
        //! Editor模式直接從特定場景開始跑，才需要.
#if UNITY_EDITOR

        Initialize();

        if (IsLoadSceneRealDataExist(sceneName) == false)
        {
            LoadSceneRealData sceneRealData = GetLoadSceneRealData(sceneName, false, false);
#if ASSETBUNDLE
            //! 使用AssetBundle先載入AssetBundle.
            LoadAssetBundleMgr.Singleton.SetDownloadDirectory(FileDirectory.GetStageAssetBundlePath());
            List<string> loadSceneList = new List<string>();
            for (int i = 0; i < sceneRealData.childSceneNameList.Count; i++)
            {
                string childSceneName = sceneRealData.childSceneNameList[i];
                AssetbundleData assetbundleData = LoadAssetBundleMgr.Singleton.GetAssetbundleData(childSceneName);
                //! 沒Load過就Load.
                if (assetbundleData == null)
                {
                    LoadAssetBundleMgr.Singleton.LoadAssetbundle(childSceneName);
                    loadSceneList.Add(childSceneName);
                }
            }

            LoadChildScene(sceneRealData, true);

            //! 場景Load完就unload assetbundle.
            for (int i = 0; i < loadSceneList.Count; i++)
            {
                LoadAssetBundleMgr.Singleton.UnloadAssetbundle(loadSceneList[i]);
            }

#else
            LoadChildScene(sceneRealData, true);
#endif //! ASSETBUNDLE.

            sceneRealData = null;
        }
#endif //! UNITY_EDITOR.
        
    }

    /// <summary>
    /// 使用idType字串取得Id.
    /// </summary>
    /// <param name="idType"></param>
    /// <returns></returns>
    int GetIdByIdType(string idType)
    {
        int id = 0;
        if (idType == "suitId")
        {
            id = InfoMgr.Singleton.GetPlayerInfo().GetSuitId();
        }

        //! 之後有其他的寫在後面.

        return id;
    }
    

    /// <summary>
    /// 多久後自動 active 異步載入的場景.
    /// </summary>
    /// <param name="activetime"></param>
    public void LoadSceneAsyncStartAutoActive(float activetime)
    {
        Initialize();

        isStartAsyncAutoActive = true;
        asyncAutoActiveTime = activetime;
        currentTime = 0;

    }

	// Update is called once per frame
	void Update ()
    {
#if ASSETBUNDLE
        for (int i = 0; i < loadSceneRealDataList.Count; i++)
        {
            LoadSceneFromAssetBundleAsync(loadSceneRealDataList[i]);
        }
#endif //! ASSETBUNDLE.

        if (isStartAsyncAutoActive)
        {
            currentTime += Time.deltaTime;
            if (IsLoadSceneAsyncApproximatelyDone() && currentTime >= asyncAutoActiveTime)
            {
                isStartAsyncAutoActive = false;
                LoadSceneAsyncActiveLoadedScene();
            }
        }
        //! 不使用下面code

        ////! 每個frame重置count數，不讓同一個frame同時做同步與非同步的場景切換
        //if(count != 0)
        //{
        //    count = 0;
        //}
    }
}
