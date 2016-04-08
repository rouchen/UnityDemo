using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoGoSound :  GoGoComponent
{
    public AudioSource audioSource;
    public List<SoundKeyFrame> soundKeyFrameList = new List<SoundKeyFrame>();

    int currKeyFrameIdx = -1;
	SoundKeyFrame.PlayType currPlayType = SoundKeyFrame.PlayType.NONE;

	// Use this for initialization
	void Start () 
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

	public override void Refresh()
    {
        if (audioSource == null ||
            audioSource.clip == null)
        {
            return;
        }

		base.Refresh();

		bool isPlayKey = false;
        for (int i = soundKeyFrameList.Count - 1; i >= 0; i--)
        {
            if (currTime >= soundKeyFrameList[i].time)
            {
				if (currKeyFrameIdx != i || 
				    currPlayType != soundKeyFrameList[i].playType)
                {
                    currKeyFrameIdx = i;
					currPlayType = soundKeyFrameList[i].playType;
                    JudgePlayType(i);
                }

                calculateSound(i, currTime);
				isPlayKey = true;
                break;
            }
        }


		if (isPlayKey == false)
		{
			currKeyFrameIdx = -1;
			if(audioSource.isPlaying)
			{
				audioSource.Stop();
			}
		}
    }

    void calculateSound(int idx, float currTime)
    {
        float volume = soundKeyFrameList[idx].volume;

        if (idx < soundKeyFrameList.Count - 1)
        {
            float factor = (currTime - soundKeyFrameList[idx].time) / (soundKeyFrameList[idx + 1].time - soundKeyFrameList[idx].time);
            volume = Mathf.Lerp(volume, soundKeyFrameList[idx + 1].volume, factor);
        }

        audioSource.volume = volume;
    }

    void JudgePlayType(int idx)
    {
        switch (soundKeyFrameList[idx].playType)
        {
            case SoundKeyFrame.PlayType.NONE:
                break;
            case SoundKeyFrame.PlayType.PLAY:
                audioSource.Play();
                break;
            case SoundKeyFrame.PlayType.PAUSE:
                audioSource.Pause();
                break;
            case SoundKeyFrame.PlayType.RESUME:
                audioSource.Play();
                break;
            case SoundKeyFrame.PlayType.STOP:
                audioSource.Stop();
                break;
        }
    }
}
