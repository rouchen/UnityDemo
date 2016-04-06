using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MobServer : NetworkBehaviour
{
    //[Server]
    //[ServerCallback]
    //[Command]
    MobCommon mCommmon;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    

    void Awake()
    {
        mCommmon = GetComponent<MobCommon>();        
    }

    [ServerCallback]
    void OnTriggerEnter(Collider collider)
    {
        MobServer ms = collider.gameObject.GetComponent<MobServer>();
        if (ms != null)
        {
            if (mCommmon.GetTeam() != ms.mCommmon.GetTeam())
            {
                mCommmon.AddHp(-1);
                if (mCommmon.GetHp() <= 0)
                {
                    GameObject.Destroy(this.gameObject);
                }

            }

        }

    }



    public void InitData(int teamId)
    {
        mCommmon.SetTeam(teamId);
    }
}
