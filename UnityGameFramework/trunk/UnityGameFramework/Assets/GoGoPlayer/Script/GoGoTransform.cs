using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoGoTransform : GoGoComponent 
{
    public bool isLookForward;
    
   	public List<TransformKeyFrame> transformKeyFrameList = new List<TransformKeyFrame>();
    
    // Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Play ();
	}

	public override void Refresh ()
	{
		base.Refresh();

		for (int i = transformKeyFrameList.Count - 1; i >= 0; i--) 
		{
			if(currTime >= transformKeyFrameList[i].time)
			{               
				calculateTranform(i, currTime);
				break;
			}

		}
	}

	void calculateTranform(int idx, float currTime)
	{
		Vector3 pos = transformKeyFrameList[idx].position;
		Vector3 rot = transformKeyFrameList[idx].rotation;
		Vector3 scal = transformKeyFrameList[idx].scale;
        
		if (idx < transformKeyFrameList.Count - 1) 
		{
			float factor = (currTime - transformKeyFrameList[idx].time) / (transformKeyFrameList[idx+1].time - transformKeyFrameList[idx].time);

            //! 使用Bezier曲線.
            if (transformKeyFrameList[idx].isUseBezierCurve)
            {
                pos = BezierCurve.GetPoint(transformKeyFrameList[idx].position,
                                           transformKeyFrameList[idx].bezierTangent1,
                                           transformKeyFrameList[idx].bezierTangent2,
                                           transformKeyFrameList[idx + 1].position,
                                           factor);
 
            }
            else
            {
                pos = Vector3.Lerp(pos, transformKeyFrameList[idx + 1].position, factor);        
            }			
			rot = Vector3.Lerp(rot, transformKeyFrameList[idx+1].rotation, factor);
			scal = Vector3.Lerp(scal, transformKeyFrameList[idx+1].scale, factor);  
        }

        if (isLookForward)
        {
            if (idx < transformKeyFrameList.Count - 1)
            {
                if (transformKeyFrameList[idx].isUseBezierCurve)
                {
                    float factor = (currTime - transformKeyFrameList[idx].time) / (transformKeyFrameList[idx+1].time - transformKeyFrameList[idx].time);
                    Vector3 dir = BezierCurve.GetDirection(transformKeyFrameList[idx].position,
                                                           transformKeyFrameList[idx].bezierTangent1,
                                                           transformKeyFrameList[idx].bezierTangent2,
                                                           transformKeyFrameList[idx + 1].position,
                                                           factor);
                    Vector3 worldLookPoint = pos;
                    if (transform.parent != null)
                    {
                        worldLookPoint = transform.parent.TransformPoint(worldLookPoint);
                    }
                    //Debug.Log(worldLookPoint);
                    transform.LookAt(worldLookPoint + 100.0f*dir);

                }
                else
                {
                    Vector3 worldLookPoint = transformKeyFrameList[idx + 1].position;
                    if (transform.parent != null)
                    {
                        worldLookPoint = transform.parent.TransformPoint(worldLookPoint);
                    }
                    transform.LookAt(worldLookPoint);
                }
            }
        }
        else
        {
            transform.localEulerAngles = rot;
        }

        transform.localPosition = pos;
        transform.localScale = scal;
        gameObject.SetActive(transformKeyFrameList[idx].isActive);
	}   
}
