using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillBase
{
    protected int skID;
    protected GameObject skPlayer;
    public int id { get { return skID; } }
    public GameObject player { get { return skPlayer; } }
    protected bool skEnd;
    public bool IsEnd { get { return skEnd; } }
    private int demoCount;
    private int skID1;
    
    public SkillBase(GameObject player, int skID)
    {
        this.skID = skID;
        this.skPlayer = player;
        demoCount = 0;
    }
  
    public void Update()
    {
        if (IsEnd)
            return;

        if (DoSkill())
            if (DoFx())
                if (End())
                {
                    skEnd = true;
                }        
        demoCount++;
        Debug.Log(this.skID.ToString() + " skilling ~" + demoCount.ToString());
    }


    private bool DoSkill()
    {
        if (demoCount > 10)
        {
            return true;
        }
        else
        {
           // Debug.Log("DoSkill +");
            return false;
        }
        // do somethings.
    }

    private bool DoFx()
    {
        if (demoCount > 20)
        {
            return true;
        }
        else
        {
           // Debug.Log("DoFx +");
            return false;
        }
        // do FX somethings.
    }

    private bool End()
    {
        if (demoCount > 30)
        {
            return true;
        }
        else
        {
          //  Debug.Log("Skill End +");
            return false;
        }
        // is end.
    }


}


public class SkillMgr  {
    private List<SkillBase> skills;

    public SkillMgr()
    {    
        skills = new List<SkillBase>();
    }
	
	public void SkillMgrUpdate () {

        for (int idx = 0; idx < skills.Count; idx++)
        { 
            if (skills[idx].IsEnd)
            {
                //Hashtable parm = new Hashtable();
                //parm.Add("activity","0");
                //skills[idx].player.GetComponent<Actor>().getFSMMgr.OnMessage(skills[idx].player, parm);
                skills.Remove(skills[idx]);
                continue;
            }
            else
            {
                skills[idx].Update();
            }

        }
	}

    public void AddSkill(GameObject player, int skID)
    {

        if (skID == 0)
        {
            Debug.LogError("SkillMgr ERROR: Null reference is not allowed");
        }
        SkillBase skInstance ;
        skInstance = new SkillBase(player, skID);
        skills.Add(skInstance);

        Hashtable parm = new Hashtable();
        parm.Add("activity","0");
        player.GetComponent<Actor>().getFSMMgr.OnMessage(player, parm);

    }


}
