using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DemoMgr : NetworkBehaviour{

    static public DemoMgr singleton;
    public GameObject building;
    public GameObject ground;
    public Transform targetPosRed;
    public Transform targetPosBlue;
    
    // server sync to client.
    [SyncVar]
    int playerOnline = 0;    
   
    void Awake()
    {
        singleton = this;
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {        
        
	}

    void OnGUI()
    {
        int posY = Screen.height / 2 - 100;

        if (GUI.Button(new Rect(5, 5, 40, 25), "ESC") || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.client.Disconnect();
                SceneManager.LoadScene("NetLobby");

            }
        }


   
        


        if (isServer && !isClient)
        {
            posY -= 38;
            GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "SERVER ONLY");
            posY += 38;
            GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "player count : " + playerOnline);
            
            return;
        }



        if (GUI.Button(new Rect(50, 5, 80, 25), "Pid : " + PlayerClient.singleton.GetPid()))
        {
        }

        if (GUI.Button(new Rect(150, 5, 80, 25), "Team : " + PlayerClient.singleton.GetTeam()))
        {
        }   
       

        if (GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "make!!") ||
            Input.GetKeyDown(KeyCode.S))
        {
            PlayerServer.singleton.CmdMakeCube();
        }
        posY += 38;
        
        if (GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "make mob!!") ||Input.GetKeyDown(KeyCode.T))
        {
         //   PlayerServer.singleton.CmdMakeMob();      
            PlayerClient.singleton.MakeMob();
        }
        posY += 38;



    }    
    
    public void Set2pComps()
    {
        Camera.main.transform.rotation = Quaternion.Euler(90, 360, 0);
        ground.transform.rotation = Quaternion.Euler(0, 180, 0);
        Vector3 pos = Vector3.zero;
        pos.x = -20.44f;
        pos.y = 15.96f;
        pos.z = -2.42f;


        Camera.main.transform.position = pos;

    }






    public static int GetPlayerOnline()
    {
        return singleton.playerOnline;
    }


    public int AddPlayerOnline()
    {
        playerOnline += 1;
        return playerOnline;
    }

}
