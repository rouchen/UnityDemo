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
    public GameObject building;
    public GameObject mobBlue;
    public GameObject mobRed;
    PlayerCommon pCommmon; 

	// Use this for initialization
	void Start () 
    {
        if (isClient && isLocalPlayer)
        {
            singleton = this;
            pCommmon.isServerSingleton = true;
        }
        else if (isServer && isClient && isLocalPlayer && hasAuthority) 
        {
            singleton = this;
            pCommmon.isServerSingleton = true;
        }
	}

    //[Server]
    void Awake()
    {
        pCommmon = GetComponent<PlayerCommon>();
        DemoMgr.singleton.AddPlayerOnline();
        pCommmon.SetPlayerId(DemoMgr.GetPlayerOnline());
        pCommmon.SetTeam(((2 - (DemoMgr.GetPlayerOnline() %2) )) );
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
        Vector3 StartPos = Vector3.zero;
        Vector3 EndPos = Vector3.zero;
        GameObject colorGo = mobBlue;
        if (pCommmon.GetTeam() == 1)
        {
            StartPos = DemoMgr.singleton.targetPosBlue.position;
            EndPos = DemoMgr.singleton.targetPosRed.position;
            colorGo = mobBlue;
        }
        
        else if (pCommmon.GetTeam() == 2) 
        {
            StartPos = DemoMgr.singleton.targetPosRed.position;
            EndPos = DemoMgr.singleton.targetPosBlue.position;
            colorGo = mobRed;
        }

        GameObject mobGo = (GameObject)GameObject.Instantiate(colorGo, StartPos, Quaternion.identity);
        
        NavMeshAgent nma = mobGo.GetComponent<NavMeshAgent>();
        nma.destination = EndPos;
        GameObject.Destroy(mobGo, 10.0f);
        mobGo.GetComponent<MobServer>().InitData(pCommmon.GetTeam());
        NetworkServer.Spawn(mobGo);


    }


}
