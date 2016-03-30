using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerClient : NetworkBehaviour
{
    //[Client]
    //[ClientCallback]
    //[ClientRpc]
    static public PlayerClient singleton;
    bool login = false;
    // to do :: use player data structure.
    public int pid = 0;  
	// Use this for initialization
	void Start () {
        if (isLocalPlayer && isClient)
            singleton = this;
        else if (isServer)
            singleton = this;
	}
    
    
    void Awake()
    {

    }
    
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

    }

	// client update is called once per frame
    [ClientCallback]
	void Update ()     
    {
        if (!isLocalPlayer)
            return;

        if (singleton == null)
            return;

        if  (!login)
        { 
            PlayerServer.singleton.CmdLogin();
            login = true;
        }

	}

    [ClientRpc]
    public void RpcLogin(int playerId)
    {
   //     if (!isLocalPlayer)
   //       return;
    
        if (pid == 0)
            pid = playerId;

        if (pid == 2 && isLocalPlayer)
        {
            //Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, 0);
            DemoMgr.singleton.Set2pComps();

        }
    }

    [Client]
    public void MakeMob()
    {
        PlayerServer.singleton.CmdMakeMob();   
    }
}
