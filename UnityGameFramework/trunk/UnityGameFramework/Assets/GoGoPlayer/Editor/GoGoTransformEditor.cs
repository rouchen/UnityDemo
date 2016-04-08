using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GoGoTransform))]
public class GoGoTransformEditor : GoGoComponentEditor 
{
	GoGoTransform gogoTransform;
    int currBezierTangentIdx = -1;

	// Use this for initialization
	void Awake () 
	{
		gogoTransform = (GoGoTransform)target;
        RegisterKeyFrameList(gogoTransform.transformKeyFrameList.Cast<KeyFrameBase>());
		EditorApplication.update += UpdateInEditorMode;
	}

    void OnDestroy()
    {
        EditorApplication.update -= UpdateInEditorMode;
    }

    protected override void UpdateInEditorMode()
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }
        base.UpdateInEditorMode();
    }
	
	public override void OnInspectorGUI()
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}

        gogoTransform.isLookForward = EditorGUILayout.Toggle("Look Forward", gogoTransform.isLookForward);

		base.OnInspectorGUI ();
	}

    protected override void KeyFrameSort()
    {
        gogoTransform.transformKeyFrameList.Sort((x, y) => { return x.time.CompareTo(y.time); });
        base.KeyFrameSort();
    }

	protected override void AddKeyFrame()
	{
		UnSelectedAllKeyFrameList();
        
		TransformKeyFrame keyFrame = new TransformKeyFrame ();
		keyFrame.position = gogoTransform.transform.localPosition;
		keyFrame.rotation = gogoTransform.transform.localEulerAngles;
		keyFrame.scale = gogoTransform.transform.localScale;
		keyFrame.time = GetCurrTime();
		keyFrame.isSelected = true;

		gogoTransform.transformKeyFrameList.Add(keyFrame);

		base.AddKeyFrame();
	}
	protected override void RemoveKeyFrame()
	{   
		List<TransformKeyFrame> keyFrameList = gogoTransform.transformKeyFrameList;
		for (int i = 0; i < keyFrameList.Count; i++) 
		{
			if(keyFrameList[i].isSelected)
			{
				keyFrameList.RemoveAt(i);
				i = i - 1;
			}
		}
        base.RemoveKeyFrame();
	}

    protected override void CustomKeyFrameGUI(int idx)
	{
        List<TransformKeyFrame> keyFrameList = gogoTransform.transformKeyFrameList;

        keyFrameList[idx].isActive = EditorGUILayout.Toggle("Active", keyFrameList[idx].isActive);
        keyFrameList[idx].position = EditorGUILayout.Vector3Field("Position", keyFrameList[idx].position);
		if (gogoTransform.isLookForward == false) 
		{
			keyFrameList[idx].rotation = EditorGUILayout.Vector3Field("Rotation", keyFrameList[idx].rotation);
		}
        keyFrameList[idx].scale = EditorGUILayout.Vector3Field("Scale", keyFrameList[idx].scale);
        keyFrameList[idx].isUseBezierCurve = EditorGUILayout.Toggle("BezierCurve", keyFrameList[idx].isUseBezierCurve);
        if (keyFrameList[idx].isUseBezierCurve)
        {
            EditorGUI.indentLevel = 1;
            keyFrameList[idx].bezierTangent1 = EditorGUILayout.Vector3Field("Tangent1", keyFrameList[idx].bezierTangent1);
            keyFrameList[idx].bezierTangent2 = EditorGUILayout.Vector3Field("Tangent2", keyFrameList[idx].bezierTangent2);
            EditorGUI.indentLevel = 0;
        }
    }

	protected override bool AsignDataToKeyFrame()
	{
		List<TransformKeyFrame> keyFrameList = gogoTransform.transformKeyFrameList;
        keyFrameList[currSelectedKeyFrameIdx].isActive = gogoTransform.gameObject.activeInHierarchy;
        keyFrameList[currSelectedKeyFrameIdx].position = gogoTransform.transform.localPosition;

        if (gogoTransform.isLookForward == false)
        {
            keyFrameList[currSelectedKeyFrameIdx].rotation = gogoTransform.transform.localEulerAngles;
		}

        keyFrameList [currSelectedKeyFrameIdx].scale = gogoTransform.transform.localScale;

        return true;
	}

    void OnSceneGUI()
    {
        List<TransformKeyFrame> keyFrameList = gogoTransform.transformKeyFrameList;
        for (int i = 0; i < keyFrameList.Count; i++)
        {   
			//! 路徑畫線.
            if (i < keyFrameList.Count - 1)
            {
                if (keyFrameList[i].isUseBezierCurve)
                {
                    //! Bezier曲線.
                    DrawBezierCurve(i);
                }
                else
                {
                    //! 直線.
                    DrawLine(i);
                }
            }
			//! keyFrame文字.
            DrawKeyFrame(i);
        }
    }

    void DrawBezierCurve(int idx)
    {
        List<TransformKeyFrame> keyFrameList = gogoTransform.transformKeyFrameList;
        Transform handleTransform = gogoTransform.transform.parent;
        Vector3 p0 = keyFrameList[idx].position;
        Vector3 p1 = keyFrameList[idx + 1].position;

        if (handleTransform != null)
        {
            p0 = handleTransform.TransformPoint(keyFrameList[idx].position);
            p1 = handleTransform.TransformPoint(keyFrameList[idx + 1].position);
        }

        Vector3 p2;
        Vector3 p3;
        if (idx == currSelectedKeyFrameIdx)
        {
            p2 = GetBezierTangentPosition(0, ref keyFrameList[idx]._bezierTangent1, true);
            p3 = GetBezierTangentPosition(1, ref keyFrameList[idx]._bezierTangent2, true);

            Handles.color = Color.magenta;
            Handles.DrawLine(p0, p2);
            Handles.DrawLine(p3, p1);
        }
        else
        {
            p2 = GetBezierTangentPosition(0, ref keyFrameList[idx]._bezierTangent1, false);
            p3 = GetBezierTangentPosition(1, ref keyFrameList[idx]._bezierTangent2, false);
        }
        Handles.DrawBezier(p0, p1, p2, p3, Color.white, null, 2.0f);
        
    }

    Vector3 GetBezierTangentPosition(int idx, ref Vector3 pos, bool isShowHandle)
    {
        Quaternion handleRotation = Quaternion.identity;
        Vector3 tangent = pos;
        Transform handleTransform = gogoTransform.transform.parent;
        if (handleTransform != null)
        {
            handleRotation = handleTransform.transform.rotation;
            tangent = handleTransform.transform.TransformPoint(tangent);
        }

        if (isShowHandle)
        {
            float size = HandleUtility.GetHandleSize(tangent);
            Handles.color = Color.magenta;
            if (Handles.Button(tangent, handleRotation, size * 0.05f, size * 0.05f, Handles.DotCap))
            {
                currBezierTangentIdx = idx;
            }
            else if (currBezierTangentIdx == idx)
            {
                EditorGUI.BeginChangeCheck();
                tangent = Handles.DoPositionHandle(tangent, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    //! 設值.
                    if (handleTransform != null)
                    {
                        pos = handleTransform.transform.InverseTransformPoint(tangent);
                    }
                    else
                    {
                        pos = tangent;
                    }
                    EditorUtility.SetDirty(target);
                }
            }
        }
        return tangent;
    }

    void DrawLine(int idx)
    {
        List<TransformKeyFrame> keyFrameList = gogoTransform.transformKeyFrameList;
        Transform handleTransform = gogoTransform.transform.parent;
        Vector3 p0 = keyFrameList[idx].position;
        Vector3 p1 = keyFrameList[idx + 1].position;

        if (handleTransform != null)
        {
            p0 = handleTransform.TransformPoint(keyFrameList[idx].position);
            p1 = handleTransform.TransformPoint(keyFrameList[idx + 1].position);
        }

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
    }

    void DrawKeyFrame(int idx)
    {
        List<TransformKeyFrame> keyFrameList = gogoTransform.transformKeyFrameList;
        Transform handleTransform = gogoTransform.transform.parent;

        Vector3 labelPos = keyFrameList[idx].position;
        if (handleTransform != null)
        {
            labelPos = handleTransform.TransformPoint(labelPos);
        }

        //! 畫點.
        float size = HandleUtility.GetHandleSize(labelPos) * 0.05f;
        Handles.color = Color.white;
        //Handles.DotCap(0, labelPos, Quaternion.identity, size);
        if (Handles.Button(labelPos, Quaternion.identity, size, size, Handles.DotCap))
        {
            keyFrameList[idx].isSelected = true;
            //! 重設選取的KeyFrame.
            currSelectedKeyFrameIdx = -1;
            CheckToggleOnlyOneSelected(idx);
            Repaint();
        }

        //! 文字.
        //labelPos.y += 5.0f * size;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(labelPos, keyFrameName + idx.ToString(), style);
    }

}
