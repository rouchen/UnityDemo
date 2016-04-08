using UnityEngine;
using System;


[Serializable]
public class FightReady : ProcBase
{
    // .........
    Fight fight;
    public float maxWaitTime = 5;
    public float curWaitTime = 0;

    override public void OnGUI()
    {
        if (GUILayout.Button("goto FightGame"))
        {
            fight.GotoNextProc(typeof(FightGame).ToString());

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mono"></param>
    public FightReady(MonoBehaviour mono, string procName)
        : base(mono, procName)
    {
        fight = mono as Fight;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Initialize()
    {
        base.Initialize();

        curWaitTime = 0;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Release()
    {
        base.Release();
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ProcStart()
    {
        base.ProcStart();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ProcUpdate()
    {
        curWaitTime += Time.deltaTime;
        if (curWaitTime >= maxWaitTime)
        {

        }

        base.ProcUpdate();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ProcInput()
    {
        base.ProcInput();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ProcEnd()
    {
        base.ProcEnd();
    }
}
