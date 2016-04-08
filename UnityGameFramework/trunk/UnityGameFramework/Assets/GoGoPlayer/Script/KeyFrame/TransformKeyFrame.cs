using UnityEngine;
using System.Collections;

[System.Serializable]
public class TransformKeyFrame : KeyFrameBase
{
	public Vector3 _position;
    public Vector3 position
    {
        set
        {
            if (_position != value)
            {
                isChange = true;
            }
            _position = value;
        }
        get
        {
            return _position;
        }
    }

    public Vector3 _rotation;
    public Vector3 rotation
    {
        set
        {
            if (_rotation != value)
            {
                isChange = true;
            }
            _rotation = value;
        }
        get
        {
            return _rotation;
        }
    }

    public Vector3 _scale;
    public Vector3 scale
    {
        set
        {
            if (_scale != value)
            {
                isChange = true;
            }
            _scale = value;
        }
        get
        {
            return _scale;
        }
    }

    public bool _isActive;
    public bool isActive
    {
        set
        {
            if (_isActive != value)
            {
                isChange = true;
            }
            _isActive = value;
        }
        get
        {
            return _isActive;
        }
    }

    public bool _isUseBezierCurve;
    public bool isUseBezierCurve
    {
        set
        {
            if (_isUseBezierCurve != value)
            {
                isChange = true;
            }
            _isUseBezierCurve = value;
        }
        get 
        {
            return _isUseBezierCurve;
        }
    }

    public Vector3 _bezierTangent1;
    public Vector3 bezierTangent1
    {
        set
        {
            if (_bezierTangent1 != value)
            {
                isChange = true;
            }
            _bezierTangent1 = value;
        }
        get
        {
            return _bezierTangent1;
        }
    }

    public Vector3 _bezierTangent2;
    public Vector3 bezierTangent2
    {
        set
        {
            if (_bezierTangent2 != value)
            {
                isChange = true;
            }
            _bezierTangent2 = value;
        }
        get
        {
            return _bezierTangent2;
        }
    }

	public TransformKeyFrame() : base()
	{
        position = Vector3.zero;
        rotation = Vector3.zero;
        scale = Vector3.one;
        isActive = true;
        _isUseBezierCurve = false;
        _bezierTangent1 = Vector3.zero;
        _bezierTangent2 = Vector3.zero;
	}
}
