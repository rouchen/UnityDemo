using UnityEngine;
using System.Collections;

[System.Serializable]
public class KeyFrameBase
{
	public float time;
    public float timeTmp;

	public bool isSelected;
	public bool isChange;
    public bool isFoldOut;

	public KeyFrameBase()
	{
		time = 0.0f;
		isSelected = false;
		isChange = false;
        isFoldOut = true;
	}
}
