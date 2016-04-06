using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MobCommon : NetworkBehaviour
{
    [SyncVar]
    public int hp;
    
    [SyncVar]
    public int team;
	
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void SetTeam(int tid)
    {
        team = tid;
    }

    public int GetTeam()
    {
        return team;
    }

    public void SetHp(int value)
    {
        hp = value;
    }

    public void AddHp(int value)
    {
        hp += value;
    }

    public int GetHp()
    {
        return hp;
    }
}
