using UnityEngine;
using System.Collections;

public class BookkeepingMgr : MonoBehaviour 
{
    static BookkeepingMgr instance;
    public BookkeepingMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("BookkeepingMgr");
                instance = go.AddComponent<BookkeepingMgr>();
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
