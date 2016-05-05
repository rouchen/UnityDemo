using UnityEngine;
using System;

[Serializable]
public class ProcBase 
{
    MonoBehaviour monoBehavior;
    public string name = "";

    public ProcBase(MonoBehaviour mono, string procName)
    {
        name = procName;
        monoBehavior = mono;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual bool Initialize()
    {
        Debug.Log(ToString() + " Initialize" );
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual bool Release()
    {
        Debug.Log(ToString() + " Release");
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ProcStart()
    {
        Debug.Log(ToString() + " ProcStart");

    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ProcUpdate()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ProcInput()
    {
        
    }

    /// <summary>
    /// 換場景時，也會執行，所以不能在ProcEnd函式中作換場景的操作，不然場景會load兩次。
    /// </summary>
    public virtual void ProcEnd()
    {
        Debug.Log(ToString() + " ProcEnd");

    }

    /// <summary>
    /// 
    /// </summary>
    public void ProcLateUpdate()
    {
 
    }

    /// <summary>
    /// 
    /// </summary>
    public void ProcFixUpdate()
    {
 
    }

    /// <summary>
    /// Debug 用.
    /// </summary>
    public virtual void OnGUI()
    {
    }
}
