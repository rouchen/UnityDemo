using UnityEngine;
using System.Collections;

public class InfoMgr : MonoBehaviour 
{
    static InfoMgr instance;

    //! Singleton.
    public static InfoMgr Singleton
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
    public DesignDataInfo designDataInfo = new DesignDataInfo();

    //===========================================

    /// <summary>
    /// 每個Credit要重置的資料.
    /// </summary>
    public void ResetEveryCredit()
    {
        playerInfo.ResetEveryCredit();
        gameInfo.ResetEveryCredit();
    }

}
