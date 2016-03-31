using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerCommon : NetworkBehaviour
{
    //static public PlayerCommon singleton;    


    [SyncVar]
    public int pid;
    [SyncVar]
    public int team = 0;
    
    // test info in inspetor.
    public bool isClientSingleton = false;
    public bool isServerSingleton = false;

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
      //  singleton = this;
    }

	// Update is called once per frame
	void Update () {
        
	}

    public void SetPlayerId(int id)
    {
        pid = id;        
    }

    public void SetTeam(int tid)
    {
        team = tid;
    }

    public int GetTeam()
    {
       return team ;
    }

    public int GetId()
    {
        return pid;

    }

}
