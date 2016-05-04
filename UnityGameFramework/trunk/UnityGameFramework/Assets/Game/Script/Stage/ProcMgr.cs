using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ProcMgr
{
    //
    Dictionary<string, ProcBase> procDic = new Dictionary<string, ProcBase>();
    //
    public string currProcName = "";
    //
    public string nextProcName = "";
    //
    public string lastProcName = "";
    //
    bool isEnterProc = true;
    //是否換 Proc.
    bool isChangeProc = false;

    public ProcBase currProc = null;
    /// <summary>
    /// 
    /// </summary>
    public ProcMgr()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="proc"></param>
    /// <returns></returns>
    public bool Add(string name, ProcBase proc)
    {
        if (!procDic.ContainsKey(name))
        {
            procDic.Add(name, proc);
            proc.Initialize();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 暴力直接換.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool SetCurrProc(string name)
    {
        if (procDic.ContainsKey(name))
        {
            lastProcName = currProcName;
            currProcName = name;
            currProc = procDic[name];
            //還是得做一次EnterProc.
            isEnterProc = true;

            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetCurrProcName()
    {
        return currProcName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public bool SetNextProcName(string name)
    {
        if (procDic.ContainsKey(name))
        {
            nextProcName = name;

            //名子不同才要換.
            isChangeProc = false;
            if (nextProcName != currProcName)
            {
                isChangeProc = true;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetLastProcName()
    {
        return lastProcName;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {

        if (currProc == null)
        {
            return;
        }

        if (isEnterProc)
        {
            currProc.ProcStart();
            isEnterProc = false;
        }

        currProc.ProcInput();
        currProc.ProcUpdate();

        //換 Proc.
        if (isChangeProc)
        {
            currProc.ProcEnd();
            SetCurrProc(nextProcName);
            isEnterProc = true;
            isChangeProc = false;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public void LateUpdate()
    {
        if (!procDic.ContainsKey(currProcName))
        {
            return;
        }

        procDic[currProcName].ProcLateUpdate();
    }

    /// <summary>
    /// 
    /// </summary>
    public void FixUpdate()
    {
        if (!procDic.ContainsKey(currProcName))
        {
            return;
        }

        procDic[currProcName].ProcFixUpdate();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Destroy()
    {
        foreach (KeyValuePair<string, ProcBase> proc in procDic)
        {
            proc.Value.Release();
        }
        procDic.Clear();
    }

    /// <summary>
    /// 通用的建立子流程.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mono"></param>
    public void CreateAddPorc<T>(MonoBehaviour mono) where T : ProcBase
    {
        string tmpstr = typeof(T).ToString();
        ProcBase newporc = (T)Activator.CreateInstance(typeof(T), mono, tmpstr);
        Add(tmpstr, newporc);
    }
}
