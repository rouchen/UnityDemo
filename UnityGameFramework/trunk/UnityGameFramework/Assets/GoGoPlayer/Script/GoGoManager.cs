using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoGoManager : MonoBehaviour 
{
    public float startTime;
    public float endTime;
    public float currTime;

    public bool isLoop;
	public float speed = 1.0f;
    public bool isPlay;

    public List<GoGoPlayer> gogoPlayerList = new List<GoGoPlayer>();

	// Use this for initialization
	void Start () 
    {
		InitializeAllPlayer ();
		Play ();
	}
	
	// Update is called once per frame
	void Update () 
    {
		RefreshTime (Time.deltaTime);

		//! 時間跑到的GoGoPlayer才開啟.
		CheckGoGoPlayerActive ();
	}

	public void RefreshTime(float deltaTime)
	{
		if (isPlay) 
		{
			currTime += deltaTime * speed;
			if (isLoop) 
			{
				currTime = (currTime > endTime) ? (currTime - endTime) : currTime;
			}
			else
			{
				currTime = (currTime > endTime) ? endTime : currTime;
			}
		}
	}

    public void RefreshPlayerTime(float deltaTime)
    {
        for (int i = 0; i < gogoPlayerList.Count; i++)
        {
            if (gogoPlayerList[i] != null)
            {
                gogoPlayerList[i].RefreshTimeByManager(deltaTime);
            }
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < gogoPlayerList.Count; i++)
        {
            if (gogoPlayerList[i] != null)
            {
                gogoPlayerList[i].Refresh();
            }
        }
    }

	void CheckGoGoPlayerActive()
	{
		for (int i = 0; i < gogoPlayerList.Count; i++) 
		{
            if (gogoPlayerList[i] != null)
            {
                if (currTime < gogoPlayerList[i].startTimeOnManager)
                {
                    gogoPlayerList[i].SetGoGoPlayerActive(false);
                    gogoPlayerList[i].gameObject.SetActive(false);
                }
                else
                {
                    gogoPlayerList[i].SetGoGoPlayerActive(true);
                    gogoPlayerList[i].gameObject.SetActive(true);
                    
                    //! 開啟後要更新位置一次，因為下一個frame GoGoPlayer的Update才會跑到.
                    //! 才不會閃一下.
                    gogoPlayerList[i].RefreshTimeByManager(Time.deltaTime);
                    gogoPlayerList[i].Refresh();
                }
            }
		}
	}

	public void Play()
	{
		isPlay = true;

		for (int i = 0; i < gogoPlayerList.Count; i++) 
		{
			gogoPlayerList [i].Play ();
		}
	}

	public void Pause()
	{
		isPlay = false;

		for (int i = 0; i < gogoPlayerList.Count; i++) 
		{
			gogoPlayerList [i].Pause();
		}
	}

	public void InitializeAllPlayer()
	{
		GoGoPlayer[] allPlayerList = FindObjectsOfType<GoGoPlayer> ();

		for (int i = 0; i < allPlayerList.Length; i++) 
		{
			bool isFound = false;
			for (int j = 0; j < gogoPlayerList.Count; j++) 
			{
				if (allPlayerList [i] == gogoPlayerList [j]) 
				{
					allPlayerList [i].SetGoGoManager (this);
					isFound = true;
					break;
				}	
			}
			if (!isFound)
			{
				allPlayerList [i].SetGoGoManager (null);
			}
		}
	}
}
