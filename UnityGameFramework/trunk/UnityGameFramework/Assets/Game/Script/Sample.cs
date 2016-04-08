using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 類別字首大寫, 單字開頭大寫.
public class Sample : MonoBehaviour 
{
    // member 字首小寫 (變數要說明.).
    // private 不用寫, static寫在public後面.
    public static bool testCode;

    // property 字首大寫, 單字開頭大寫..
    public bool IsTestCode
    {
        set { testCode = value; }
        get { return testCode; }
    }

    // Dictionary變數最後加Dic.
    // List變數最後加List. 
    Dictionary<string, ProcBase> procDic = new Dictionary<string, ProcBase>();

    // enum 全大小,單字用底線分隔.
    enum TEST_ENUM
    {
        // 項目 全大小,單字用底線分隔.
        TEST_A,
        TEST_B,
        TEST_C,
    };

	// Use this for initialization
	void Start () 
    {
       //TextAsset ta = Resources.Load("aaaa\dd") as TextAsset;

       //Application.persistentDataPath + "aaaa\dd.txt";
       //Application.persistentDataPath + "aaaa\dd.txt";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 函式字首大寫.
    /// </summary>
    /// <param name="t">同member</param>
    /// <param name="b">同member</param>
    /// <returns></returns>
    bool Testcode(int t, bool b)
    {
        return true;
    }
}
