using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    //
    static SceneMgr instance = null;
    public static SceneMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SceneMgr");
                instance = go.AddComponent<SceneMgr>();
                DontDestroyOnLoad(go);
                instance.LoadSceneFile("");
            }
            return instance;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public void LoadSceneFile(string path)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="async"></param>
    /// <param name="b"></param>
    public void LoadScene(string sceneName, bool async)
    {
        // 研究一下SceneManager用法.
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        ao.allowSceneActivation = false;

        // 查表 決定要load哪些Scene.
        SceneManager.LoadSceneAsync(sceneName + "tw");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName"></param>
    public void ChangeScene(string sceneName)
    {
 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public bool LoadSceneEnd(string sceneName)
    {
        return true;
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
