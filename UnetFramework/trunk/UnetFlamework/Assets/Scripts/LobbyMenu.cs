using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LobbyMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {


        int posY = Screen.height / 2 - 100;

        if (GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "Single Player Game\n[Press S]") ||
            Input.GetKeyDown(KeyCode.S))
        {

            NetworkManager.singleton.StartHost();            
        }
        posY += 38;
        /*
        if (GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "Host Multiplayer Game\n[Press H]") ||
            Input.GetKeyDown(KeyCode.H))
        {
            //NetworkManager.singleton.StartMatchMaker();
            //SceneManager.LoadScene()
            //Application.LoadLevel("host");
        }
        posY += 38;

        if (GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "Join Multiplayer Game\n[Press J]") ||
            Input.GetKeyDown(KeyCode.J))
        {
            //NetworkManager.singleton.StartMatchMaker();
            //Application.LoadLevel("join");
        }
        posY += 38;
        */
        if (GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "Join Local Game\n[Press K]") ||
            Input.GetKeyDown(KeyCode.K))
        {
            NetworkManager.singleton.StartClient();         
        }
        posY += 44;

        if (GUI.Button(new Rect(Screen.width / 2 - 100, posY, 200, 33), "Start Server\n[Press H]") ||
          Input.GetKeyDown(KeyCode.H))
        {
            NetworkManager.singleton.StartServer();
            
        }
        posY += 44;
    }
}
