using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public class SoundSceneResource : MonoBehaviour {

    public bool adjustPanel = false;
    //bool hideMenu = false;

    [Tooltip("Filename was default name, you can setting alias name, if you want.")]
    public List<soundAudioInfo> soundAudioInfos = new List<soundAudioInfo>();
    
    // audio mixer
    public AudioMixer gameAudioMixer;
    
    static public SoundSceneResource instance;

    /// <summary>
    /// 
    /// </summary>
    void Start ()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        if (instance != null)
        {
            SoundMgr.Singleton.RegisterAudioSourceByRootScene(soundAudioInfos);
            return;
        }
        instance = this;
        SoundMgr.Singleton.InitAudioMixer(gameAudioMixer);
        SoundMgr.Singleton.RegisterAudioSource(soundAudioInfos);

    }
    /// <summary>
    /// 
    /// </summary>
    void Update ()
    {
	    
	}


    //void OnGUI()
    //{
    //    if (adjustPanel == false)
    //    {
    //        return;
    //    }

    //    if (GUI.Button(new Rect((Screen.width - 100), 50, 100, 33), "Menu"))
    //    {
    //        hideMenu = !hideMenu;
    //    }

    //    if (hideMenu == true)
    //    {
    //        return;
    //    }

    //    if (GUI.Button(new Rect((Screen.width / 2), 50, 100, 33), "All Stop"))
    //    {

    //        for (int i = 0; i < soundAudioInfos.Count; i++)
    //        //foreach (KeyValuePair<string, soundAudioInfo> kvp in audioDic)
    //        {
    //            if (soundAudioInfos[i].audioClip != null)
    //            {

    //                if (soundAudioInfos[i].group == "BGM")
    //                {
    //                    SoundMgr.Singleton.StopBGM(soundAudioInfos[i].name);
    //                }
    //                else 
    //                {
    //                    SoundMgr.Singleton.StopSound(soundAudioInfos[i].name);
    //                }
    //            }
    //        }


    //    }

    //    int posY = Screen.height / 4;
    //    int posX = 10;
    //    int idx = 0;
    //    int width = 100;

    //    for (int i = 0; i < soundAudioInfos.Count;i++)
    //    //foreach (KeyValuePair<string, soundAudioInfo> kvp in audioDic)
    //    {
    //        if ((posX + width) > Screen.width)
    //        {
    //            posY += 45;
    //            posX = 10;
    //        }

    //        if (GUI.Button(new Rect(posX, posY, width, 33), soundAudioInfos[i].name))
    //        {

    //            if (soundAudioInfos[i].audioClip != null)
    //            {

    //                if (soundAudioInfos[i].group == "BGM")
    //                {
    //                    SoundMgr.Singleton.PlayBGM(soundAudioInfos[i].name);
    //                }
    //                else 
    //                {
    //                    SoundMgr.Singleton.PlaySound(soundAudioInfos[i].name);
    //                }
    //            }

             
    //        }
    //        posX += width;
    //        idx++;
    //    }

    //}

    /// <summary>
    /// deconstruction.
    /// </summary>
    void OnDestroy()
    {
        // 場景A非同步場景B, A可能在B awake 後才 ondestory, soundMgr 
        // SoundMgr.Singleton.UnRegisterAudioSource();
    }
}


[Serializable]
public class soundAudioInfo
{
    public string name;
    public AudioClip audioClip;
    public string group;

    [NonSerialized]
    public bool fadeCheck;
    [NonSerialized]
    public bool inOrOut;
    [NonSerialized]
    public float fadeTime;
    [NonSerialized]
    public float initailTime; 
    [NonSerialized]
    public float audioVolume;

}