using UnityEngine;
using System.Collections;

public class CreditMgr : MonoBehaviour 
{
    static CreditMgr instance;
    public CreditMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CreditMgr");
                instance = go.AddComponent<CreditMgr>();
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
