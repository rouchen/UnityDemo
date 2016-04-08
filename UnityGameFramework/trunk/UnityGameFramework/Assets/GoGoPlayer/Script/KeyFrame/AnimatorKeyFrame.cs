using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimatorKeyFrame : KeyFrameBase 
{
	public int animaionStateIdx;
    public string animationStateName;
    public float animationStateLength;
    
	public float _animationStateStartPos;
	public float animationStateStartPos
	{
		set 
		{
			if (_animationStateStartPos != value) 
			{
				isChange = true;
			}
			_animationStateStartPos = value;
		}
		get
		{
			return _animationStateStartPos;
		}
	}

	public float _animationStateEndPos;
	public float animationStateEndPos
	{
		set
		{
			if (_animationStateEndPos != value) 
			{
				isChange = true;
			}
			_animationStateEndPos = value;
		}
		get
		{
			return _animationStateEndPos;
		}
	}

    public float _speed;
	public float speed
	{
		set
		{
			if (_speed != value) 
			{
				isChange = true;
			}
			_speed = value;
		}
		get
		{
			return _speed;
		}
	}

    public bool _isInterpolation;
	public bool isInterpolation
	{
		set
		{
			if (_isInterpolation != value) 
			{
				isChange = true;
			}
			_isInterpolation = value;
		}
		get
		{
			return _isInterpolation;
		}
	}
		
    public int _sampleRate;
	public int sampleRate
	{
		set
		{
			if (_sampleRate != value) 
			{
				isChange = true;
			}
			_sampleRate = value;
		}
		get
		{
			return _sampleRate;
		}
	}
	public bool _isLoop;
	public bool isLoop
	{
		set
		{
			if (_isLoop != value) 
			{
				isChange = true;
			}
			_isLoop = value;
		}
		get
		{
			return _isLoop;
		}
	}

	public AnimatorKeyFrame() : base()
	{
        animaionStateIdx = 0;
        animationStateName = "";
        animationStateLength = 0.0f;
        animationStateStartPos = 0.0f;
        animationStateEndPos = 0.0f;
        speed = 1.0f;
        isInterpolation = true;
        sampleRate = 30;
		isLoop = false;
	}
}
