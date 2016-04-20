using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class WarmTest : MonoBehaviour {
    
    public float updateInterval = 0.5F;
    int frames = 0;
    float timeleft;
    float lastTimeSinceStartUp;
    Text warmTxt;
    TimeSpan timeSpan;
    TimeSpan tempTimeSpan;   
   
	// Use this for initialization
	void Start () {
        tempTimeSpan = new TimeSpan();
        warmTxt = GetComponent<Text>();
        if (warmTxt == null)
        {
            Debug.LogError("Text Componenet Not Found !!");
        }
        lastTimeSinceStartUp = Time.realtimeSinceStartup;
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        ++frames;
        timeleft = Time.realtimeSinceStartup - lastTimeSinceStartUp;

        if (timeleft >= updateInterval)
        {
            timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
            if (timeSpan.Hours == tempTimeSpan.Hours && timeSpan.Minutes == tempTimeSpan.Minutes && timeSpan.Seconds == tempTimeSpan.Seconds && timeSpan.Days == tempTimeSpan.Days)
            {
                return;
            }
            warmTxt.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes, timeSpan.Seconds);
            tempTimeSpan = timeSpan;
        }
	}
}

