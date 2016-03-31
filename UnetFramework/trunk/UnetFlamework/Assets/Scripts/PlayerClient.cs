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
    

    PlayerCommon pCommmon; 
	// Use this for initialization
	void Start () {
        if (isClient && isLocalPlayer)
        {
            singleton = this;
            pCommmon.isClientSingleton = true;
        }
        if (isServer && isClient && isLocalPlayer && hasAuthority)
        {
            singleton = this;
            pCommmon.isClientSingleton = true;
        }
        
	}
    
    
    void Awake()
    {
        pCommmon = GetComponent<PlayerCommon>();
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
            if (pCommmon.GetTeam() == 2)
            {                
                DemoMgr.singleton.Set2pComps();                
                login = true;
            }
            
        }

	}

    /// <summary>
    /// broadcast all players.
    /// </summary>
    [ClientRpc] 
    public void RpcDoSomething()
    {     
        
    }

    [Client]
    public void MakeMob()
    {
        PlayerServer.singleton.CmdMakeMob();   
    }

    public int GetPid()
    {
        return pCommmon.GetId();
    }


    public int GetTeam()
    {
        return pCommmon.GetTeam();
    }
}
