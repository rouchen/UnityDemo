using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour {
    public GameObject player;  
    private FSMMgr fsm;
    private SkillMgr skm;
    public SkillMgr getSkillMgr { get { return skm; } }
    public FSMMgr getFSMMgr { get { return fsm; } }

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }
	// Use this for initialization
	void Start () {
        MakeFSM();       
        player = gameObject;
        SetActorData();
        skm = new SkillMgr();   
	}
    public void SetActorData()
    {
        // set actor data.
    }


    public void FixedUpdate()
    {
        fsm.currentState.Update(player, gameObject);              
        skm.SkillMgrUpdate();
    }
	// Update is called once per frame
	void Update () {
	
	}

    private void MakeFSM()
    {

        ActorAttackState attackState = new ActorAttackState(2.0f);
        ActorMoveState moveState = new ActorMoveState(1.0f);
        
        ActorSkillState skillState = new ActorSkillState(0.1f);
        
        attackState.AddTransition(Transition.TS_FIND_TARGET, StateID.STS_MOVE);
        attackState.AddTransition(Transition.TS_SKILL, StateID.STS_SKILL);

        moveState.AddTransition(Transition.TS_ATTACK, StateID.STS_ATTACK);
        moveState.AddTransition(Transition.TS_SKILL, StateID.STS_SKILL);

        fsm = new FSMMgr();        
        fsm.AddState(moveState);
        fsm.AddState(attackState);
        fsm.AddState(skillState);
    }


    public void DoSkill()
    {
        Debug.Log("SKILL ON!!");
        SetTransition(Transition.TS_SKILL);
        Hashtable parm = new Hashtable();
        parm.Add("1","14");
        fsm.OnMessage(player, parm);
    }
}
