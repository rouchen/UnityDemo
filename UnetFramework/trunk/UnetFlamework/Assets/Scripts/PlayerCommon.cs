using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerCommon : NetworkBehaviour
{
    static public PlayerCommon singleton;

    //[SyncVar]

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
        singleton = this;
    }


	// Update is called once per frame
	void Update () {
        
	}


}
