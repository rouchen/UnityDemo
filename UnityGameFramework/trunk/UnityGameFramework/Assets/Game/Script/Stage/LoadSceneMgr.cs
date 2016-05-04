using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.SceneManagement;

//! 場景路徑資料.
public class ScenePathData
{
    public string name;
    public string path;
}

//! 場景資料.
public class LoadSceneData
{
    public ScenePathData parentScene;
    public List<ScenePathData> childSceneList;
}

public class LoadSceneMgr : MonoBehaviour
{
    //! 場景List.
    List<LoadSceneData> loadSceneDataList = new List<LoadSceneData>();

    //! 異步載入的 op 清單.
    List<AsyncOperation> asyncOP = new List<AsyncOperation>();

    //! 異步載入後要被清除的 scene.
    List<Scene> toBeUnloadScene = new List<Scene>();
    bool isStartAsyncAutoActive = false;
    float asyncAutoActiveTime = 0;
    float currentTime = 0;
    
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
                instance.LoadScenesFile("Data/LoadScene");
            }
            return instance;
        }
    }

    /// <summary>
    /// 載入場景檔案.
    /// </summary>
    /// <param name="path">路徑</param>
    void LoadScenesFile(string path)
    {
        Object resObject = Resources.Load(path);
        
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(resObject.ToString());

        ParseScenesData(xmlDoc.FirstChild);

        xmlDoc = null;
        resObject = null;
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
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
            loadSceneData.parentScene.path = parentNode.ChildNodes[i].Attributes["Path"].InnerText;

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
            scenePathData.path = parentNode.ChildNodes[i].Attributes["Path"].InnerText;
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
        LoadSceneData sceneData = FindSceneData(sceneName);
        string parentSceneFullName = sceneData.parentScene.path + sceneData.parentScene.name;

        //LoadSceneData emptySceneData = FindSceneData("Empty");
        //string emptySceneFullName = emptySceneData.parentScene.path + emptySceneData.parentScene.name;


        if (async)
        {
            //清空.
            asyncOP.Clear();
            toBeUnloadScene.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                toBeUnloadScene.Add(SceneManager.GetSceneAt(i));
            }

            //! Parent Scene.

            //SceneManager.LoadSceneAsync(parentSceneFullName);
            //先載一個空埸景(清空之前的), 再把其它的 async load 進來. 
            //SceneManager.LoadSceneAsync(emptySceneFullName);
            // single 會 block 其它的 additive.
            //StartCoroutine(LoadSceneAsync(emptySceneFullName, LoadSceneMode.Single));


            //! Child Scene.

            LoadChildScene(sceneData.childSceneList, async, false);


            //parent scene 最後 load 才抓得到 child scene 的東西.
            StartCoroutine(LoadSceneAsync(parentSceneFullName, LoadSceneMode.Additive));

            if (asyncSceneAutoActive)
            {
                LoadSceneAsyncStartAutoActive(0);
            }
        }
        else
        {
            //! Parent Scene.
            SceneManager.LoadScene(parentSceneFullName);
            //! Child Scene.
            LoadChildScene(sceneData.childSceneList, async, false);
        }

    }

    /// <summary>
    /// Load Child Scene.
    /// </summary>
    /// <param name="childSceneList"></param>
    /// <param name="async">非同步</param>
    /// <param name="checkLoad">檢查是否load過</param>
    void LoadChildScene(List<ScenePathData> childSceneList, bool async, bool checkLoad)
    {
        for (int i = 0; i < childSceneList.Count; i++)
        {
            string childSceneFullName = childSceneList[i].path + childSceneList[i].name;

            //! 檢查是否load過.
            if (checkLoad)
            {
                Scene scene = SceneManager.GetSceneByName(childSceneFullName);
                if (scene.IsValid())
                {
                    continue;
                }
            }

            //! Load場景.
            if (async)
            {
                //SceneManager.LoadSceneAsync(childSceneFullName, LoadSceneMode.Additive);
				StartCoroutine(LoadSceneAsync(childSceneFullName, LoadSceneMode.Additive));
				
            }
            else
            {
                SceneManager.LoadScene(childSceneFullName, LoadSceneMode.Additive);
            }
        }
    }

    /// <summary>
    /// 載入近度是否將近完成.
    /// </summary>
    /// <returns></returns>
    public bool IsLoadSceneAsyncApproximatelyDone()
    {
        bool alldone = true;
        for (int i = 0; i < asyncOP.Count; i++)
        {           
            alldone &= Mathf.Approximately(asyncOP[i].progress, 0.9f);
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
    void LoadSceneAsyncActiveLoadedScene()
    {
                
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
                    SceneManager.UnloadScene(toBeUnloadScene[i].buildIndex);
                }
                 break;
            }
            yield return null;
        }

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
        for (int i = 0; i < loadSceneDataList.Count; i++)
        {
            ScenePathData parentScene = loadSceneDataList[i].parentScene;
            List<ScenePathData> childScenes = loadSceneDataList[i].childSceneList;
            if (parentScene.name == sceneName)
            {
                LoadChildScene(childScenes, false, true);
            }
        }
    }

    
    
    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        
    }

	/// <summary>
	/// 
	/// </summary>
	void Start () 
    {
	
	}

    /// <summary>
    /// 多久後自動 active 異步載入的場景.
    /// </summary>
    /// <param name="activetime"></param>
    public void LoadSceneAsyncStartAutoActive(float activetime)
    {
        isStartAsyncAutoActive = true;
        asyncAutoActiveTime = activetime;
        currentTime = 0;

    }

	// Update is called once per frame
	void Update ()
    {
        if (isStartAsyncAutoActive)
        {
            currentTime += Time.deltaTime;

            if (IsLoadSceneAsyncApproximatelyDone() && currentTime >= asyncAutoActiveTime)
            {
                isStartAsyncAutoActive = false;
                LoadSceneAsyncActiveLoadedScene();
            }
            
        }
	}
}
