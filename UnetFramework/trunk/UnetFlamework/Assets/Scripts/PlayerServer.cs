using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerServer : NetworkBehaviour
{
    //[Server]
    //[ServerCallback]
    //[Command]
    //[SyncEvent]

    static public PlayerServer singleton;

    bool readyToGo = false;
    public GameObject building;
    public GameObject mob01;        

	// Use this for initialization
	void Start () {
        if (isServer)
            singleton = this;
        else if (isLocalPlayer)
            singleton = this;
	}

    //[Server]
    void Awake()
    {

    }

	// Server update is called once per frame

    void Update () 
    {
	    
	}
    
    [Command]
    public void CmdMakeCube()
    { 
        Vector3 pos = Vector3.zero;
        pos.x = -20.4f;
        pos.y = 0.38f;
        pos.z = 3.79f;
        GameObject b = (GameObject)GameObject.Instantiate(building, pos, Quaternion.identity);        
        NetworkServer.Spawn(b);

    
    }

    [Command]
    public void CmdMakeMob()
    {
        Vector3 pos = Vector3.zero;
        pos.x = -20f;
        pos.y = 0f;
        pos.z = 3f;
        GameObject b = (GameObject)GameObject.Instantiate(mob01, pos, Quaternion.identity);
        NavMeshAgent nma = b.GetComponent<NavMeshAgent>();
        nma.destination = DemoMgr.singleton.targetPos.position;
        GameObject.Destroy(b, 10.0f);
        NetworkServer.Spawn(b);

    }


    [Command]
    public void CmdLogin()
    {
        DemoMgr.singleton.AddPlayerOnline();
        PlayerClient.singleton.RpcLogin(DemoMgr.GetPlayerOnline());  // set player id.
    }

    public void SetPid()
    {
       //emoMgr.singleton.AddPlayerOnline();
    }

}
