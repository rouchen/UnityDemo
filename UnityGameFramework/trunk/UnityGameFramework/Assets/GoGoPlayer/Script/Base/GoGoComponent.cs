using UnityEngine;
using System.Collections;

public class GoGoComponent : MonoBehaviour 
{
	public GoGoPlayer gogoPlayer;
    
    public bool isUseLocalTime;
    public bool isLoop;
    public float startLocalTime;
    public float endLocalTime;
    public float currTime;
    public bool isLastEnableState;

	public void SetGoGoPlayer(GoGoPlayer player)
	{
		gogoPlayer = player;
	}

	public virtual void Refresh()
	{
        if (gogoPlayer == null)
        {
            return;
        }

        if (!isUseLocalTime)
        {
            currTime = gogoPlayer.currTime;
        }
	}

    public void RefreshLocalTime(float deltaTime)
    {
        if (isUseLocalTime)
        {
			currTime += deltaTime;

			if (isLoop) 
			{
				currTime = (currTime > endLocalTime) ? startLocalTime : currTime;
			} 
			else 
			{
				currTime = (currTime > endLocalTime) ? endLocalTime : currTime;
			}
        }
    }
}
