using UnityEngine;
using System;


[Serializable]
public class FightGame : ProcBase
{
    // .........
    Fight fight;
    public float maxWaitTime = 5;
    public float curWaitTime = 0;
    public float curWaitTime2 = 0;
    public float curWaitTime3 = 0;

    override public void OnGUI()
    {
        if (GUILayout.Button("goto FightPowerUp"))
        {
            fight.GotoNextProc(typeof(FightPowerUp).ToString());

        }
        if (GUILayout.Button("goto FightWarning"))
        {
            fight.GotoNextProc(typeof(FightWarning).ToString());

        }
        if (GUILayout.Button("goto FightFinish"))
        {
            fight.GotoNextProc(typeof(FightFinish).ToString());

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mono"></param>
    public FightGame(MonoBehaviour mono, string procName)
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
