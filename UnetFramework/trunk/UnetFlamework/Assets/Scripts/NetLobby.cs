using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class NetLobby : NetworkManager
{    
    public GameObject building;
    public GameObject mobRed;
    public GameObject mobBlue;
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Vector3 pos = Vector3.zero;        
        GameObject thePlayer = (GameObject)Instantiate(base.playerPrefab, pos, Quaternion.identity);                      
        NetworkServer.AddPlayerForConnection(conn, thePlayer, playerControllerId);

    }
        
    void Start()
    {
        // because, Registered Spawnable Prefabs always empty list, maybe bug ,maybe not.
        this.spawnPrefabs.Add(building);
        this.spawnPrefabs.Add(mobRed);
        this.spawnPrefabs.Add(mobBlue);
    }



    
  
}
