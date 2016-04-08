using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class RayCastMgr : MonoBehaviour 
{
    // declare delegate type.
    public delegate void RayCastCallback(RaycastHit[] rayCastHit); 
    // to store the function.        
    protected RayCastCallback cbFunction;
    
    // declare ray 
    Ray ray;
    //RaycastHit rayHit;  
    RaycastHit[] rayCastHit;    
    LayerMask mask;
    Dictionary<RayCastCallback, string[]> rayDic = new Dictionary<RayCastCallback, string[]>();
    
    // singleton.
    static RayCastMgr instance;

    public static RayCastMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("RayCastMgr");
                instance = go.AddComponent<RayCastMgr>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    private RayCastMgr()
    {

    }
	
    // Use this for initialization
	void Start () 
    {
	
	}

    /// <summary>
    /// 註冊RayCastMgr
    /// </summary>
    /// <param name="layers">layer name</param>
    /// <param name="rayCastCb">callback(RaycastHit[] rayCastHit)</param>
    /// <returns></returns>
    public bool Register(string[] layers, RayCastCallback rayCastCb)
	{        
        if (!rayDic.ContainsKey(rayCastCb))
        {
            cbFunction += rayCastCb;            
            rayDic.Add(rayCastCb, layers);
            ResetLayerMask();
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// 反註冊RayCastMgr
    /// </summary>
    /// <param name="rayCastCb">callback</param>
    /// <returns></returns>
    public bool UnRegister( RayCastCallback rayCastCb)
    {
        if (rayDic.ContainsKey(rayCastCb))
        {
            cbFunction -= rayCastCb;
            rayDic.Remove(rayCastCb);
            ResetLayerMask();
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 設定mask
    /// </summary>
    void ResetLayerMask()
    {
        string[] layers;
        LayerMask maskTpm;
        var enumerator =  rayDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            layers = enumerator.Current.Value;              
            for (int i = 0; i < layers.Length; i++) 
            {
                maskTpm = 1 << LayerMask.NameToLayer(layers[i]);
                mask = mask | maskTpm;
            }            
        }
    }

    /// <summary>
    /// RayCast interface
    /// </summary>
    /// <param name="hitPozs">position Vector3 list</param>
    public void OnRayCast(Vector3[] hitPozs, Camera cam , float dis = 1000f)
    {
        if (cam == null)
        {
            Debug.LogError("OnRayCast camera null");
            return;
        }
        
        for (int i = 0; i < hitPozs.Length; i++) 
        {
            //ray = Camera.main.ScreenPointToRay(hitPozs[i]);
            ray = cam.ScreenPointToRay(hitPozs[i]);
            rayCastHit = Physics.RaycastAll(ray, dis, mask.value);
            if (rayCastHit.Length > 0)
            { 
                cbFunction(rayCastHit);
            }
        } 
    }
	
    // Update is called once per frame
	/// <summary>
	/// 測試用.
	/// </summary>
    void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3[] v3List = new Vector3[] { Input.mousePosition ,};
            OnRayCast(v3List, Camera.main, 1000f);
        }
	}
}
