using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoGoPlayer : MonoBehaviour 
{
	public float startTime;
	public float endTime;
	public float _currTime;

    public bool isLoop;
	public float speed = 1.0f;
    public bool isPlay;

	public float startTimeOnManager;

	public GoGoManager gogoManager;

    public List<GameObject> gogoGameObjectList = new List<GameObject>();
	public List<GoGoComponent> gogoComponentList = new List<GoGoComponent>();


	public float currTime
	{
		set
		{
			_currTime = value;
			if (gogoManager != null) 
			{
				//gogoManager.currTime = startTimeOnManager + (_currTime / speed);
			}
		}
		get
		{
			return _currTime;
		}
	}
   
	// Use this for initialization
	void Start () 
	{
        GetAllComponents();
		Play ();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (gogoManager != null)
        {
            RefreshTimeByManager(Time.deltaTime);
        }
        else
        {
            RefreshTime(Time.deltaTime);
        }
		
		Refresh();
	}

	public void RefreshTime(float deltaTime)
	{
		if (isPlay)
		{
            currTime += deltaTime * speed;
            RefreshLocalTime(deltaTime * speed);
		}

		if (isLoop) 
		{
			currTime = (currTime > endTime) ? (currTime - endTime) : currTime;
		} 
		else 
		{
			currTime = (currTime > endTime) ? endTime : currTime;
		}
	}

    public void RefreshTimeByManager(float deltaTime)
    {
        currTime = (gogoManager.currTime - startTimeOnManager) * speed;

        if (currTime > 0)
        {
            RefreshLocalTime(deltaTime * speed);
        }

        if (isLoop)
        {
            currTime = (currTime > endTime) ? (currTime - endTime) : currTime;
        }
        else
        {
            currTime = (currTime > endTime) ? endTime : currTime;
        }
    }

	public void Refresh()
	{
		for (int i = 0; i < gogoComponentList.Count; i++) 
		{
			if(gogoComponentList[i] != null)
			{   
				gogoComponentList[i].Refresh();
			}
		}
	}

    public void RefreshLocalTime(float deltaTime)
    {
        for (int i = 0; i < gogoComponentList.Count; i++)
        {
            if (gogoComponentList[i] != null)
            {
                gogoComponentList[i].RefreshLocalTime(deltaTime);
            }
        }
    }

	public void Play(float time)
	{
		isPlay = true;
		currTime = time;	
	}

	public void Play()
	{
		isPlay = true;
	}

	public void Pause()
	{
		isPlay = false;
	}

	public void Stop()
	{
		isPlay = false;
		currTime = startTime;
	}

	public void GetAllComponents()
	{
		gogoComponentList.Clear();
		for (int i = 0; i < gogoGameObjectList.Count; i++)
		{
			if (gogoGameObjectList[i] != null)
			{
				GoGoComponent[] components = gogoGameObjectList[i].GetComponents<GoGoComponent>();
				for (int j = 0; j < components.Length; j++)
				{
					components[j].SetGoGoPlayer(this);
					gogoComponentList.Add(components[j]);
				}
			}
		}
	}

	public void SetGoGoManager(GoGoManager manager)
	{
		gogoManager = manager;
	}

	public void RemoveGoGoManager()
	{
		gogoManager = null;
	}

	public void SetGoGoPlayerActive(bool isActive)
	{
		for (int i = 0; i < gogoGameObjectList.Count; i++) 
		{
			gogoGameObjectList [i].SetActive (isActive);
		}	
	}
}
