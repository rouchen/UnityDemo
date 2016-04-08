using UnityEngine;
using System.Collections;

public class IgsLibMgr : MonoBehaviour 
{
    static IgsLibMgr instance;
    public IgsLibMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("IgsLibMgr");
                instance = go.AddComponent<IgsLibMgr>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
