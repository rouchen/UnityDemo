using UnityEngine;
using System.Collections;
using System.Text;
using System.Reflection;
public class Fight : MonoBehaviour 
{
    enum FIGHT_PROC
    {
        STANDBY,
        WAITINPUT,
        CHANGE_SCENE,   // 一定要有換場景流程.
    }

    public StringBuilder strngBuild = new StringBuilder(256, 256);
    string tmpstr;

    //
    public ProcMgr procMgr = new ProcMgr();

    Object testObject;

    public FightUI ui;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        
    }

	/// <summary>
	/// 
	/// </summary>
	void Start () 
    {
        tmpstr = typeof(FightInrto).ToString();
        FightInrto introp = new FightInrto(this, tmpstr);
        procMgr.Add(tmpstr, introp);

        tmpstr = typeof(FightReady).ToString();
        FightReady readyp = new FightReady(this, tmpstr);
        procMgr.Add(tmpstr, readyp);

        tmpstr = typeof(FightGame).ToString();
        FightGame gamep = new FightGame(this, tmpstr);
        procMgr.Add(tmpstr, gamep);

        tmpstr = typeof(FightPowerUp).ToString();
        FightPowerUp powp = new FightPowerUp(this, tmpstr);
        procMgr.Add(tmpstr, powp);


        tmpstr = typeof(FightFinish).ToString();
        FightFinish fishp = new FightFinish(this, tmpstr);
        procMgr.Add(tmpstr, fishp);


        tmpstr = typeof(FightResult).ToString();
        FightResult resp = new FightResult(this, tmpstr);
        procMgr.Add(tmpstr, resp);

        tmpstr = typeof(FightWarning).ToString();
        FightWarning wrnp = new FightWarning(this, tmpstr);
        procMgr.Add(tmpstr, wrnp);

        procMgr.SetCurrProc("FightInrto");
        


    }

    /// <summary>
    /// debug 用.
    /// </summary>
#if UNITY_EDITOR
    void OnGUI()
    {
        if (procMgr.currProc != null)
        {
            procMgr.currProc.OnGUI();
        }
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    void Update () 
    {
        procMgr.Update();
	}

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        procMgr.LateUpdate();
    }

    /// <summary>
    /// 
    /// </summary>
    void FixUpdate()
    {
        procMgr.FixUpdate();
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDestroy()
    {
        procMgr.Destroy();
        procMgr = null;
        Destroy(testObject);

        Resources.UnloadUnusedAssets();
        //! GC ??
        System.GC.Collect();

    }

    public void GotoNextProc(string procname)
    {
        procMgr.SetNextProcName(procname);
    }
}
