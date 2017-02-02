using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class SoundMgr : MonoBehaviour
{               
    Dictionary<string, soundAudioInfo> audioDic = new Dictionary<string, soundAudioInfo>();
    Dictionary<string, AudioMixerGroup> amgDic = new Dictionary<string, AudioMixerGroup>();
    // system sound use.
    Dictionary<string, soundAudioInfo> sysAudioDic = new Dictionary<string, soundAudioInfo>();
    AudioMixer mainAudioMixer;
    float mainAudioVol;
    
    // sound use.
    List<AudioSource> soundAudios = new List<AudioSource>();    
    int maxSoundAudio = 5;

    // bgm use.
    Dictionary<string, AudioSource> bgmAudios = new Dictionary<string, AudioSource>();
    int maxBGMAudio = 5;



    bool soundMute = false;
    bool systemLoad = false;
    public static SoundMgr instance = null;

    //! 淡入淡出source
    List<soundAudioInfo> fadeInOut = new List<soundAudioInfo>();
    //=======================================
    //! Singleton case.
    public static SoundMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SoundMgr");
                instance = go.AddComponent<SoundMgr>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    /// <summary>
    /// 初始化.
    /// </summary>
    public void Initialize()
    {

    }

    /// <summary>    
    /// interface :: play sound by name. 
    /// </summary>    
    /// <param name="name"></param>    
    public void PlaySound(string name)
    {
        PlaySound(name, false);
    }


    /// <summary>    
    /// interface :: play sound by name. 
    /// </summary>    
    /// <param name="name"></param>    
    public void PlayOneShotSound(string name)
    {
        PlaySound(name, true);
    }



    /// <summary>
    /// interface :: play sound by name and audioSource. 
    /// maybe you use 3d audio design. but not support resource pool.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="adoSourc"></param>
    public void PlaySound(string name, AudioSource adoSourc)
    {
        PlaySoundByRefSource(name, adoSourc, false);
    }

    /// <summary>
    /// interface :: play sound by name and audioSource. 
    /// maybe you use 3d audio design. but not support resource pool.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="adoSourc"></param>
    public void PlayOneShotSound(string name, AudioSource adoSourc)
    {
        PlaySoundByRefSource(name, adoSourc, true);
    }

    /// <summary>
    /// play sound by adoSource, maybe you use 3d audio design. but not support resource pool.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="adoSource"></param>
    /// <param name="isPlayOneShot"></param>
    void PlaySoundByRefSource(string name, AudioSource adoSource, bool isPlayOneShot)
    {
        if (adoSource == null)
        {
            Debug.LogWarning("AudioSource null!! name is " + name);
        }

        if (adoSource != null && audioDic[name] != null)
        {
            InitAdoSource(adoSource, audioDic[name].group);
            adoSource.clip = audioDic[name].audioClip;
            if (isPlayOneShot)
            {
                adoSource.PlayOneShot(adoSource.clip);
            }
            else
            {
                adoSource.Play();
            }            
        }
    }


    /// <summary>    
    /// play sound by name. name reference by AudioSource.name
    /// </summary>    
    /// <param name="name"></param>
    /// <param name="isPlayOneShot"></param>
    void PlaySound(string name , bool isPlayOneShot)
    {
        if (!audioDic.ContainsKey(name))
        {
            Debug.LogWarning("cant find audio by name => " + name);
            return;
        }

        AudioSource adoSource = null;
        // get audio source. use resource pool design.
        bool isPlay = true;
        int notPlayIdx = 0;        
        string logProc = "";
        // try get same adoSource
        for (int i = 0; i < soundAudios.Count; i++)
        {
           
            if (audioDic[name].audioClip.name == soundAudios[i].clip.name)
            {
               // Debug.LogError("right get " + audioDic[name].audioClip.name + " / " + soundAudios[i].clip.name);
                adoSource = soundAudios[i];                
                logProc += "same name/";                
                continue;
            }
            if (!soundAudios[i].isPlaying)
            {
                isPlay = false;
                notPlayIdx = i;
            }
        }

        // try get not playing adoSource
        if (adoSource == null && !isPlay)
        {
            adoSource = soundAudios[notPlayIdx];
            logProc += "get not playing adoSource/";
        }

        // can't find same or not playing adosource.
        if (adoSource == null)
        {
            if (soundAudios.Count > maxSoundAudio)
            {
                Debug.LogWarning("max sound audio , max  = " + maxSoundAudio.ToString());
                return;
            }
            else
            {
                adoSource = this.gameObject.AddComponent<AudioSource>();
                logProc += "create new adoSource/";
                InitAdoSource(adoSource , audioDic[name].group);
                soundAudios.Add(adoSource);
            }
        }

        if (adoSource != null)
        {
            adoSource.clip = audioDic[name].audioClip;
            SetAudioMixerGroup(adoSource, audioDic[name].group);
            
            if (audioDic[name].audioClip == adoSource.clip)
            {
                logProc += "same Clip/";
            }
            if (audioDic[name].audioClip.name != adoSource.clip.name)
            {
                logProc += "diferent name " + audioDic[name].audioClip.name + "|" + adoSource.clip.name + "/";
            }
                    
            if (isPlayOneShot)
            {
                adoSource.PlayOneShot(adoSource.clip);
                //Debug.LogWarning("adoSource PlayOneShot, name  = " + name);
            }
            else
            {
                adoSource.Play();
               // Debug.LogWarning("adoSource Play, name  = " + name);
               // Debug.LogError(logProc);
            }
        }
    }


    /// <summary>
    /// stop sound by name. name reference by AudioSource.name
    /// </summary>
    /// <param name="name"></param>
    public void StopSound(string name)
    {
        if (!audioDic.ContainsKey(name))
        {
            Debug.LogWarning("cant find audio by name => " + name);
            return;
        }

        for (int i = 0; i < soundAudios.Count; i++)
        {
            //if ((audioDic[name].audioClip == soundAudios[i].clip) && (soundAudios[i].isPlaying == true))
            if ((audioDic[name].audioClip.name == soundAudios[i].clip.name) && (soundAudios[i].isPlaying == true))
            {
                soundAudios[i].Stop();
            }
        }
    }



    


    /// <summary>
    /// initialztion adoSource.
    /// </summary>
    /// <param name="adoSource"></param>
    void InitAdoSource(AudioSource adoSource, string groupName, bool isloop = false)
    {
        adoSource.mute = soundMute;
        //adoSource.volume = baseVolume * soundVolumeScale;
        adoSource.playOnAwake = false;
        adoSource.loop = isloop;
        SetAudioMixerGroup(adoSource, groupName);
        //adoSource.outputAudioMixerGroup = soundAMG;
    }

    /// <summary>
    /// set audioMixer group
    /// </summary>
    /// <param name="adoSource"></param>
    /// <param name="groupName"></param>
    void SetAudioMixerGroup(AudioSource adoSource, string groupName)
    {
        if (amgDic.ContainsKey(groupName))
        {
            adoSource.outputAudioMixerGroup = amgDic[groupName];
        }
        else
        {
            Debug.LogWarning("Not has audioMixerGroup adoSource name is " + groupName);
        }

    }

    /// <summary>
    /// set sound pool size, the value need greater then 0.
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxSoundAudio(int value)
    {
        if (value > 0)
        {
            maxSoundAudio = value;
        }
    }

    /// <summary>
    /// set audioMixer volume,range -80 ~ 20f.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMainAudioVol(float volume)
    {
        if (mainAudioMixer == null)
        {
            return;
        }

        float vol;
        if (mainAudioMixer.GetFloat("Volume", out vol))
        {
            mainAudioMixer.SetFloat("Volume", volume);
            mainAudioVol = volume;
        }
        else
        {
            Debug.LogError("AudioMixer group Master need expose -> Volume"); ;
        }
    }

    /// <summary>
    /// get audioMixer volume
    /// </summary>
    /// <returns></returns>
    public float GetMainAudioVol()
    {
        if (mainAudioMixer == null)
        {
            return 0f;
        }

        float vol;
        if (mainAudioMixer.GetFloat("Volume", out vol))
        {
            return vol;
        }                   
        else
        {
            Debug.LogError("AudioMixer group Master need expose -> Volume");
            return 0f;
        }
    }


    /// <summary>
    /// Un- / Mutes the AudioSource. Mute sets the volume=0, Un-Mute restore the original volume.        
    /// </summary>
    /// <param name="value"></param>
    public void SetMute(bool value)
    {        
        if (value == true)
        {
            SetMainAudioVol(-80.0f);
        }
        else
        {
            SetMainAudioVol(mainAudioVol);            
        }
       
        soundMute = value;
    }


    /// <summary>
    /// get soundMute
    /// </summary>
    /// <returns></returns>
    public bool GetMute()
    {
        return soundMute;
    }

   


    void Start()
    {
        //InitAudioMixer();
        //InitBGM();        
        // log
        LogAudioSource();
    }


    /// <summary>
    /// regist system audio, dont clear, regist by onece.
    /// </summary>
    /// <param name="soundAudioInfos"></param>
    public void RegisterSystemAudioSource(List<soundAudioInfo> soundAudioInfos)
    {
        if (systemLoad)
        {
            return;
        }

        for (int i = 0; i < soundAudioInfos.Count; i++)
        {
            if ((soundAudioInfos[i].name == "") && (soundAudioInfos[i].audioClip != null))
            {
                soundAudioInfos[i].name = soundAudioInfos[i].audioClip.name;
            }
            //!存入Dictionary
            sysAudioDic[soundAudioInfos[i].name] = soundAudioInfos[i];
            audioDic[soundAudioInfos[i].name] = soundAudioInfos[i];
        }
        systemLoad = true;
    }



    /// <summary>
    /// regist by SoundSceneResource.
    /// </summary>
    /// <param name="soundAudioInfos"></param>
    public void RegisterAudioSource(List<soundAudioInfo> soundAudioInfos)
    {
        //! 關閉上個流程音效
        for (int i = 0; i < soundAudios.Count; i++)
        {
            if (soundAudios[i].isPlaying)
            {
                soundAudios[i].Stop();
            }
        }

        audioDic.Clear();


        // system sound.

        foreach (KeyValuePair<string, soundAudioInfo> sysAdo in sysAudioDic)
        {
            if ((sysAdo.Value.name == "") && (sysAdo.Value.audioClip != null))
            {
                sysAdo.Value.name = sysAdo.Value.audioClip.name;
            }
            audioDic[sysAdo.Value.name] = sysAdo.Value;
        }
        int soundCount = soundAudioInfos.Count;
        if (soundCount > 30)
        {
            Debug.LogWarning("Too much soundAudioInfo loading");
        }
        // normal sound.
        for (int i = 0; i < soundCount; i++)
        {
            if ((soundAudioInfos[i].name == "") && (soundAudioInfos[i].audioClip != null)) 
            {
                soundAudioInfos[i].name = soundAudioInfos[i].audioClip.name;
            }
            audioDic[soundAudioInfos[i].name] = soundAudioInfos[i];
        }
    }
    /// <summary>
    /// other Scene list audio regist
    /// </summary>
    /// <param name="soundAudioInfos"></param>
    public void RegisterAudioSourceByRootScene(List<soundAudioInfo> soundAudioInfos)
    {
        int soundCount = soundAudioInfos.Count;
        if(soundCount > 30)
        {
            Debug.LogWarning("Too much soundAudioInfo loading");
        }
        for (int i = 0; i < soundCount; i++)
        {
            if ((soundAudioInfos[i].name == "") && (soundAudioInfos[i].audioClip != null))
            {
                soundAudioInfos[i].name = soundAudioInfos[i].audioClip.name;
            }
            //! 防呆
            if (!audioDic.ContainsKey(soundAudioInfos[i].name))
            {
                audioDic[soundAudioInfos[i].name] = soundAudioInfos[i];
            }
            else
            {
                Debug.LogError("Same Key in dictionary !! : " + soundAudioInfos[i].name);
            }
        }
    }
    /// <summary>
    /// other Scene one audio regist
    /// </summary>
    /// <param name="soundAudioInfos"></param>
    public void RegisterAudioSourceByRootScene(soundAudioInfo soundAudioInfos)
    {
        if ((soundAudioInfos.name == "") && (soundAudioInfos.audioClip != null))
        {
            soundAudioInfos.name = soundAudioInfos.audioClip.name;
        }
        if (!audioDic.ContainsKey(soundAudioInfos.name))
        {
            audioDic[soundAudioInfos.name] = soundAudioInfos;
        }
        else
        {
            Debug.LogError("Same Key in dictionary !! : " + soundAudioInfos.name);
        }
    }
    /// <summary>
    /// 抽卡測試規格使用
    /// </summary>
    /// <param name="soundAudioInfos"></param>
    public void DeleteAudioSourceByRootScene(soundAudioInfo soundAudioInfos)
    {
        if (audioDic.ContainsKey(soundAudioInfos.name))
        {
            audioDic.Remove(soundAudioInfos.name);
        }
        else
        {
            Debug.LogError("No Key in the dictionary !! : " + soundAudioInfos.name);
        }
    }

    /// <summary>
    /// initialztion audio mixer by SoundSceneResource.
    /// </summary>
    public void InitAudioMixer(AudioMixer gameAudioMixer)
    {
        if (gameAudioMixer == null)
        {
            Debug.LogWarning("AudioMixer cant find!");
            return;
        }

        AudioMixerGroup[] amgs = gameAudioMixer.FindMatchingGroups("Master");
        for (int i = 0; i < amgs.Length; i++)
        {
            // ignore first group name "Master";
            if (amgs[i].name == "Master")
            {
                continue;
            }
            else 
            {
                amgDic[amgs[i].name] = amgs[i];                
            }
        }
        mainAudioMixer = gameAudioMixer;
        mainAudioVol = GetMainAudioVol();
        //mainAudioBaseVol = GetMainAudioVol();
        
    }



    /// <summary>
    /// Log audio source name.
    /// </summary>
    void LogAudioSource()
    {
        string logText;
        logText = "[Scene: " + SceneManager.GetActiveScene().name + " GameObject: " + this.gameObject.name + " ]";
        logText += "\n[alias name/file name]\n";
        string adoClipName = "null ref";
        foreach (KeyValuePair<string, soundAudioInfo> kvp in audioDic)
        {

            if (kvp.Value.audioClip == null)
            {
                adoClipName = "null ref";
            }
            else
            {
                adoClipName = kvp.Value.audioClip.name;
            }

            logText += "[" + kvp.Value.name + "/" + adoClipName + "]\n";            
        }
         
        Debug.Log(logText);
    }

    /// <summary>
    /// put in MonoBehaviour.Start();
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    public void PlayBGM(string name, bool isLoop = true)
    {
        if (!audioDic.ContainsKey(name))
        {
            return;
        }

        if (!bgmAudios.ContainsKey(name) )
        {
            if (bgmAudios.Count < maxBGMAudio)
            {
                AudioSource adoSource = null;
                adoSource = this.gameObject.AddComponent<AudioSource>();
                InitAdoSource(adoSource, audioDic[name].group, isLoop);
                bgmAudios[name] = adoSource;
                adoSource.clip = audioDic[name].audioClip;
                bgmAudios[name].Play();
            }
            else

            {
                Debug.LogWarning("too much bgm!!, maxBGMAudio = " + maxBGMAudio.ToString());
            }            
        }
        else
        {
            if (!bgmAudios[name].isPlaying)
            {
                bgmAudios[name].Play();
            }
        }                    
    }

    /// <summary>
    /// clear all bgm audioSource.
    /// </summary>
    /// <param name="name"></param>
    public void ClearBGM()
    {
        foreach (KeyValuePair<string, AudioSource> kvp in bgmAudios)
        {
            StopBGM(kvp.Key);
            for (int i = 0; i < fadeInOut.Count; ++i)
            {
                if (kvp.Key == fadeInOut[i].name)
                {
                    fadeInOut.Remove(fadeInOut[i]);
                    break;
                }
            }
            DestroyObject(kvp.Value);
        }

        bgmAudios.Clear();
    }

    /// <summary>
    /// pause bgm.    
    /// </summary>
    public void PauseBGM(string name)
    {
        if (bgmAudios.ContainsKey(name))
        {
            bgmAudios[name].Pause();
        }
    }

    /// <summary>
    /// stop bgm
    /// </summary>
    public void StopBGM(string name)
    {
        if (bgmAudios.ContainsKey(name))
        {
            bgmAudios[name].Stop();
        }        
    }
    /// <summary>
    /// Bgm sound is playing
    /// </summary>
    ///  <param name="name">背景音名子</param>
    public bool IsBgmPlaying(string name)
    {
        if (bgmAudios.ContainsKey(name))
        {
            if (bgmAudios[name].isPlaying)
            {
                return true;
            }
            else
                return false;
        }
        else
        {
            Debug.LogWarning("cant find Bgm audio by name => " + name + " Return False");
            return false;
        }
    }
    /// <summary>
    /// Set Bgm Volume 
    /// </summary>
    ///  <param name="name">背景音名子</param>
    public void SetBgmVolume(string name,float musicLevl)
    {
        //!不存在的話就return
        if (!audioDic.ContainsKey(name))
        {
            Debug.LogWarning("cant find audio by name => " + name );
            return;
        }
        //! 如果存在而且在bgmAudios
        if (bgmAudios.ContainsKey(name))
        {
            bgmAudios[name].volume = musicLevl;
        }
        //! 存在但不在bgmAudios裡面
        else
        {
            if (bgmAudios.Count < maxBGMAudio)
            {
                AudioSource adoSource = null;
                adoSource = this.gameObject.AddComponent<AudioSource>();
                InitAdoSource(adoSource, audioDic[name].group, true);
                bgmAudios[name] = adoSource;
                bgmAudios[name].clip = audioDic[name].audioClip;
                bgmAudios[name].volume = musicLevl;
            }
            //!太多放不進去
            else
            {
                Debug.LogWarning("too much bgm!!, maxBGMAudio = " + maxBGMAudio.ToString());
            }
        }
    }
    /// <summary>
    /// normal sound is playing
    /// </summary>
    ///  <param name="name">背景音名子</param>
    public bool IsSoundPlaying(string name)
    {
        if (audioDic.ContainsKey(name))
        {
            for(int i =0; i< soundAudios.Count; i++)
            {
                if(audioDic[name].audioClip.name == soundAudios[i].clip.name)
                {
                    if (soundAudios[i].isPlaying)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            return false;
        }
        else
        {
            Debug.LogWarning("cant find normal audio by name => " + name + " Return False");
            return false;
        }
    }
    /// <summary>
    /// 除了系統音全刪除
    /// </summary>
    public void ClearAll()
    {
        foreach (KeyValuePair<string, AudioSource> kvp in bgmAudios)
        {
            StopBGM(kvp.Key);
            DestroyObject(kvp.Value);
        }
        for(int i = 0; i< soundAudios.Count; i++)
        {
            if (soundAudios[i].isPlaying)
            {
                soundAudios[i].Stop();
            }
            soundAudios[i].clip = null;
        }
        //!刪除AudioSouce
        soundAudios.Clear();
        bgmAudios.Clear();
        //!Dictionary 清空
        audioDic.Clear();
        //!將audioMixer清空
        amgDic.Clear();
        //! 淡入淡出 清空
        fadeInOut.Clear();
    }
    /// <summary>
    /// 聲音淡出
    /// </summary>
    /// <param name="time">淡出時間</param>
    /// <param name="name">音效名子</param>
    /// <param name="valueVolume">起始音量，0.0f就是預設值</param>
    /// <param name="loop">背景音樂LOOP</param>
    public void FadeOut(float time, string name, float valueVolume = 0.0f,bool loop =true)
    {
        if (audioDic.ContainsKey(name))
        {
            soundAudioInfo tempAudioSource = audioDic[name];
            tempAudioSource.inOrOut = false;
            tempAudioSource.fadeCheck = true;
            //! 淡出時間
            tempAudioSource.fadeTime = time;
            //! 起始值
            tempAudioSource.initailTime = 0.0f;

            //! 播放音樂
            if (bgmAudios.ContainsKey(name))
            {
                if (!bgmAudios[name].isPlaying)
                {
                    PlayBGM(name,loop);
                }
                //! 起始音量
                tempAudioSource.audioVolume = bgmAudios[name].volume;
            }
            else if (!bgmAudios.ContainsKey(name))
            {
                PlayBGM(name, loop);
                //! 起始音量
                tempAudioSource.audioVolume = bgmAudios[name].volume;
            }
            else
            {
                bool check = false;
                for (int i = 0; i < soundAudios.Count; i++)
                {
                    if (audioDic[name].audioClip.name == soundAudios[i].clip.name)
                    {
                        if (!soundAudios[i].isPlaying)
                        {
                            PlaySound(audioDic[name].audioClip.name);
                        }
                        //! 起始音量
                        tempAudioSource.audioVolume = soundAudios[i].volume;
                        check = true;
                        break;
                    }
                }
                //! 無播放
                if (!check)
                {
                    PlaySound(audioDic[name].audioClip.name);
                    //! 起始音量
                    tempAudioSource.audioVolume = 1.0f;
                }
            }
            //! 不是預設值
            if(valueVolume != 0.0f)
            {
                tempAudioSource.audioVolume = valueVolume;
            }
            //! 加入淡入淡出
            fadeInOut.Add(audioDic[name]);
        }
        else
        {
            Debug.LogWarning("cant find normal audio by name => " + name);
        }
    }
    /// <summary>
    /// 聲音淡入
    /// </summary>
    /// <param name="time">淡入時間</param>
    /// <param name="name">音效名子</param>
    /// <param name="valueVolume">最終音量</param>
    /// <param name="loop">背景音樂LOOP</param>
    public void FadeIn(float time, string name, float valueVolume = 1.0f,bool loop = true)
    {
        //!如果在字典裏面
        if (audioDic.ContainsKey(name))
        {
            soundAudioInfo tempAudioSource = audioDic[name];
            tempAudioSource.inOrOut = true;
            tempAudioSource.fadeCheck = true;
            //! 結束音量
            tempAudioSource.audioVolume = valueVolume;
            //! 淡入時間
            tempAudioSource.fadeTime = time;
            //! 起始值
            tempAudioSource.initailTime = 0.0f;

            //! 播放音樂
            if (bgmAudios.ContainsKey(name))
            {
                if (!bgmAudios[name].isPlaying)
                {
                    PlayBGM(name, loop);
                }
            }
            else if(!bgmAudios.ContainsKey(name))
            {
                PlayBGM(name, loop);
            }
            else
            {
                bool check =false;
                for (int i = 0; i < soundAudios.Count; i++)
                {
                    if (audioDic[name].audioClip.name == soundAudios[i].clip.name)
                    {
                        if (!soundAudios[i].isPlaying)
                        {
                            PlaySound(audioDic[name].audioClip.name);
                        }
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    PlaySound(audioDic[name].audioClip.name);
                }
            }
            //! 加入淡入淡出
            fadeInOut.Add(audioDic[name]);
        }
        else
        {
            Debug.LogWarning("cant find  audio by name => " + name);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        
        for(int j =0;j< fadeInOut.Count;++j)
        {
            soundAudioInfo soundInfo = fadeInOut[j];
            if (soundInfo.fadeCheck)
            {
                soundInfo.initailTime += Time.deltaTime;

                AudioSource tempAudioSouce = null;

                //! 尋找audioSource
                if (bgmAudios.ContainsKey(soundInfo.name))
                {
                    tempAudioSouce = bgmAudios[soundInfo.name];
                }
                else
                {
                    for (int i = 0; i < soundAudios.Count; i++)
                    {
                        if (soundInfo.audioClip.name == soundAudios[i].clip.name)
                        {
                            soundAudios[i].volume = (soundInfo.initailTime / soundInfo.fadeTime) * soundInfo.audioVolume;
                            tempAudioSouce = soundAudios[i];
                            break;
                        }
                    }
                }
                //! 取失敗
                if(tempAudioSouce == null)
                {
                    Debug.LogError(" Fade Sound Fail");
                    return;
                }

                //! 淡入
                if (soundInfo.inOrOut)
                {
                    tempAudioSouce.volume = (soundInfo.initailTime / soundInfo.fadeTime) * soundInfo.audioVolume;
                }
                //! 淡出
                else
                {
                    tempAudioSouce.volume = soundInfo.audioVolume - (soundInfo.initailTime / soundInfo.fadeTime) * soundInfo.audioVolume;
                }

                //! 結束
                if (soundInfo.initailTime >= soundInfo.fadeTime)
                {
                    //! 淡出停止
                    if (!soundInfo.inOrOut)
                    {
                        tempAudioSouce.Stop();
                    }
                    soundInfo.fadeCheck = false;
                    fadeInOut.Remove(soundInfo);
                }
            }
        }
    }

    /// <summary>
    /// deconstruction.
    /// </summary>
    void OnDestroy()
    {
        audioDic.Clear();
        amgDic.Clear();
        fadeInOut.Clear();
        //soundAudioInfos.Clear();
    }

}

