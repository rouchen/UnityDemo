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
    public void LoadScene(string sceneName, bool async)
    {
        LoadSceneData sceneData = FindSceneData(sceneName);
        string parentSceneFullName = sceneData.parentScene.path + sceneData.parentScene.name;

        if (async)
        {
            //! Parent Scene.
            SceneManager.LoadSceneAsync(parentSceneFullName);
                        
            //! Child Scene.
            for (int i = 0; i < sceneData.childSceneList.Count; i++)
            {
                ScenePathData scenePathData = sceneData.childSceneList[i];
                string childSceneFullName = scenePathData.path + scenePathData.name;

                SceneManager.LoadSceneAsync(childSceneFullName, LoadSceneMode.Additive);
            }
        }
        else
        {
            //! Parent Scene.
            SceneManager.LoadScene(parentSceneFullName);
            
            //! Child Scene.
            for (int i = 0; i < sceneData.childSceneList.Count; i++)
            {
                ScenePathData scenePathData = sceneData.childSceneList[i];
                string childSceneFullName = scenePathData.path + scenePathData.name;

                SceneManager.LoadScene(childSceneFullName, LoadSceneMode.Additive);
            }
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
	
	// Update is called once per frame
	void Update () {
	
	}
}
