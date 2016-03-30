using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Transition
{
    NullTransition = 0, // Use this transition to represent a non-existing transition in your system
    TS_BACK,
    TS_FIND_TARGET,
    TS_ATTACK,
    TS_SKILL,
}

public enum StateID
{
    STS_NONE = 0,
    STS_MOVE ,
    STS_IDLE ,
    STS_ATTACK ,
    STS_SKILL,
    STS_DEAD,

}
// to do:: 拆檔案. FSM define.FSMMgr,FSMState,StateA,StateB...

public abstract class FSMState
{
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
    protected StateID stateID;
    public StateID id { get { return stateID; } }

    protected float finalTime;
    protected float intervalTime;


    protected FSMState(float interTime = 0.1f)
    {
        this.intervalTime = interTime;
        ResetTimer();
    }
    
    public bool TimeUp()
    {
        if ((this.finalTime - Time.time) < 0)
        {        
            return true;
        }

        return false;
    }

    public void ResetTimer()
    {
        this.finalTime = Time.time + this.intervalTime;
    }

    public void AddTransition(Transition trans, StateID id)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
            return;
        }
        
        if (id == StateID.STS_NONE)
        {
            Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
            return;
        }

        if (map.ContainsKey(trans))
        {
            Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() + "Impossible to assign to another state");
            return;
        }

        map.Add(trans, id);
    }
    
    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        { 
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }

        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() +
                           " was not on the state's transition list");
    }

    public StateID GetOutputState(Transition trans)
    {
        // Check if the map has this transition
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.STS_NONE;
    }


    public virtual void DoBeforeEntering() { }
    public virtual void DoBeforeLeaving() { }
    public virtual void OnMessage(GameObject player, Hashtable param){}
    public void Update(GameObject player, GameObject npc)
    {
        if (!TimeUp())
            return;
                            
        Act( player,  npc);
        Reason(player, npc);

        ResetTimer();
    }
    public abstract void Reason(GameObject player, GameObject npc);
    public abstract void Act(GameObject player, GameObject npc);

}


public class FSMMgr  {
    private List<FSMState> states;
    private StateID currentStateID;    
    public StateID GetCurrentStateID { get { return currentStateID; } }
    public FSMState currentState;
    public FSMState GetCurrentState { get { return currentState; } }
    private StateID beforeStateID;
    public StateID GetBeforeStateID { get { return beforeStateID; } }
   
        

    public FSMMgr()
    {
        states = new List<FSMState>();
    }
    
    public void AddState(FSMState s)
    {
        // Check for Null reference before deleting
        if (s == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }
 
        // First State inserted is also the Initial state,
        //   the state the machine is in when the simulation begins
        if (states.Count == 0)
        {
            states.Add(s);
            currentState = s;
            currentStateID = s.id;            
            beforeStateID = currentStateID;
            return;
        }
 
        // Add the state to the List if it's not inside it
        foreach (FSMState state in states)
        {
            if (state.id == s.id)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + s.id.ToString() + 
                               " because state has already been added");
                return;
            }
        }
        states.Add(s);
    }

    public void OnMessage(GameObject player, Hashtable param)
    {
        // Check for Null reference before deleting
        if (currentState == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }
        currentState.OnMessage(player, param);
    }

    public void DeleteState(StateID id)
    {
        // Check for NullState before deleting
        if (id == StateID.STS_NONE)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
            return;
        }

        // Search the List and delete the state if it's inside it
        foreach (FSMState state in states)
        {
            if (state.id == id)
            {
                states.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                       ". It was not on the list of states");
    }


    public void PerformTransition(Transition trans)
    {

        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
            return;
        }
        
        
        StateID id = currentState.GetOutputState(trans);

        if (trans == Transition.TS_BACK) 
        {
            id = beforeStateID;
        }

        
        if (id == StateID.STS_NONE)
        {
            Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a target state " +
                           " for transition " + trans.ToString());
            return;
        }

        beforeStateID = currentStateID;
        currentStateID = id;
        foreach (FSMState state in states)
        {
            if (state.id == currentStateID)
            {
                currentState.DoBeforeLeaving();
                currentState = state;
                currentState.DoBeforeEntering();
                break;
            }
        }

    } 

}




// ...........State  demo


public class ActorAttackState : FSMState
{
    private int demoCount;

    public ActorAttackState(float timer = 0.5f):base(timer)
    {
        stateID = StateID.STS_ATTACK;
        demoCount = 0;

    }


//    public override void Reason(GameObject player, GameObject npc)
    public override void Reason(GameObject player, GameObject npc)
    {       
        // if target miss or too far
        //    to move .  SetTransition(Transition.LostPlayer);
        if (demoCount == 5) 
        {            
            Debug.Log("attack over!~");
            demoCount = 0;
            player.GetComponent<Actor>().SetTransition(Transition.TS_FIND_TARGET);
        }
            
    }

    public override void Act(GameObject player, GameObject npc)
    {
      // do attack -> call skill system 
        demoCount++;
        Debug.Log("attack!! ->" + demoCount.ToString());        
    }

}
public class ActorMoveState : FSMState
{
    private int demoCount;

    public ActorMoveState(float timer = 0.5f): base(timer)
    {
        stateID = StateID.STS_MOVE;
        demoCount = 0;

    }


    //    public override void Reason(GameObject player, GameObject npc)
    public override void Reason(GameObject player, GameObject npc)
    {


        if (demoCount == 5)
        {
            Debug.Log("Stop moveing!~");
            player.GetComponent<Actor>().SetTransition(Transition.TS_ATTACK);
            demoCount = 0;
        }
    }

    public override void Act(GameObject player, GameObject npc)
    {
        // call skill system 
        demoCount++;
        Debug.Log("moveing -> " + demoCount.ToString());


    }

}

public class ActorSkillState : FSMState
    {
    private int demoCount;

    public ActorSkillState(float timer = 0.5f): base(timer)
    {
        stateID = StateID.STS_SKILL;
        demoCount = 0;

    }


    
    public override void Reason(GameObject player, GameObject npc)
    {           
    
    }

    public override void Act(GameObject player, GameObject npc)
    {
    
    }
    public override void OnMessage(GameObject player, Hashtable param)
    {
    
        if (param.Contains("activity"))
        {
            // skill system callback.
            player.GetComponent<Actor>().SetTransition(Transition.TS_BACK);
          
        }
        else
        {
            // call skill system .
            demoCount++;    
            player.GetComponent<Actor>().getSkillMgr.AddSkill(player, demoCount);
        }
        

    }

}