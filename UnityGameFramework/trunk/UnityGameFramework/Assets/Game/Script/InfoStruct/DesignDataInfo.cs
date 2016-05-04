using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

public class DesignDataInfo
{
    Dictionary<int, SuitInfoJson> suitDic = new Dictionary<int, SuitInfoJson>();
    TextAsset loadTempText;
    
    /// <summary>
    /// init
    /// </summary>
    public DesignDataInfo()
    {
        LoadSuitDic();
    }

    /// <summary>
    /// set suitDic by json data.
    /// </summary>
    void LoadSuitDic()
    {
        string tmp;
        tmp = LoadJsonFile("Data/Suit");
        JsonData jd = JsonMapper.ToObject(tmp);
        int id = 0;
        for (int i = 0; i < jd.Count; i++)
        {
            SuitInfoJson sij = new SuitInfoJson();
            id = int.Parse(jd[i]["id"].ToString());
            sij.name = jd[i]["name"].ToString();
            // attr[0] ~ attr[5]
            sij.attr.Add(int.Parse(jd[i]["atk"].ToString()));
            sij.attr.Add(int.Parse(jd[i]["def"].ToString()));
            sij.attr.Add(int.Parse(jd[i]["agi"].ToString()));
            sij.attr.Add(int.Parse(jd[i]["sync"].ToString()));
            sij.attr.Add(int.Parse(jd[i]["insight"].ToString()));
            sij.attr.Add(int.Parse(jd[i]["rare"].ToString()));
            sij.context = jd[i]["context"].ToString();
            suitDic.Add(id, sij);
        }       
    }

    /// <summary>
    /// load json file to string.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string LoadJsonFile(string path)
    {
        loadTempText = Resources.Load(path) as TextAsset;
        if (loadTempText == null)
        {
            Debug.LogError(path + " load fail");
        }
        return loadTempText.text;
    }

    /// <summary>
    /// get suit name by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetSuitName(int id)
    {
        return suitDic[id].name;
    }
    
    /// <summary>
    /// get suit context by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetSuitContext(int id)
    {
        return suitDic[id].context;
    }

    public int GetSuitNumber()
    {
        return suitDic.Count;
    }

    public List<int> GetSuitAttrList(int id)
    {
        return suitDic[id].attr;
    }
}

/// <summary>
/// SuitInfo data structure.
/// </summary>
class SuitInfoJson
{
    public string id;    
    public string name;
    public List<int> attr = new List<int>();
    public string context;
}