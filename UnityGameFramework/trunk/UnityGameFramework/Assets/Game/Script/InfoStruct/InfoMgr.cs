using UnityEngine;
using System.Collections;

public class InfoMgr : MonoBehaviour 
{
    static InfoMgr instance;
    public InfoMgr Singleton
    {
        get 
        {
            if (instance == null)
            {
                GameObject go = new GameObject("InfoMgr");
                instance = go.AddComponent<InfoMgr>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public PlayerInfo playerInfo = new PlayerInfo();
    public GameInfo gameInfo = new GameInfo();
	
}
