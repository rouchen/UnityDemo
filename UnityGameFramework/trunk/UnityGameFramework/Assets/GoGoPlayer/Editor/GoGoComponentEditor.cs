using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GoGoComponent))]
public class GoGoComponentEditor : Editor 
{
	protected const string keyFramefocusName = "KeyFrameFocus";
	protected const string keyFrameName = "KeyFrame";

    IEnumerable<KeyFrameBase> keyFrameBaseList = new List<KeyFrameBase>();
	protected int currSelectedKeyFrameIdx = -1;
	float lastTime = -1;
	Vector2 scrollPos = new Vector2 ();

    protected void RegisterKeyFrameList(IEnumerable<KeyFrameBase> keyFrameList)
    {
        keyFrameBaseList = keyFrameList;
    }

	public override void OnInspectorGUI()
	{
		GoGoComponent gogoComponent = (GoGoComponent)target;
		GoGoPlayer gogoPlayer = gogoComponent.gogoPlayer;

        if (gogoPlayer == null)
        {
            return;
        }

        ShowTimeLineGUI();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add",GUILayout.Width(100))) 
		{
			AddKeyFrame();
		}
        if (GUILayout.Button("Remove", GUILayout.Width(100))) 
		{
			RemoveKeyFrame();
		}
		ShowCustomTool ();
		EditorGUILayout.EndHorizontal();

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos, false, true);
        ShowKeyFrameGUI();
		EditorGUILayout.EndScrollView ();

        SceneView.RepaintAll();
	}

	protected virtual void ShowCustomTool()
	{
		
	}

    void ShowTimeLineGUI()
    {
        GoGoComponent gogoComponent = (GoGoComponent)target;
        GoGoPlayer gogoPlayer = gogoComponent.gogoPlayer;

        //! 使用物件本地時間.
        gogoComponent.isUseLocalTime = EditorGUILayout.Toggle("UseLocalTime", gogoComponent.isUseLocalTime);

        if (gogoComponent.isUseLocalTime)
        {
            EditorGUI.indentLevel = 1;
            gogoComponent.isLoop = EditorGUILayout.Toggle("Loop", gogoComponent.isLoop);
            gogoComponent.startLocalTime = EditorGUILayout.FloatField("StartTime", gogoComponent.startLocalTime);
            gogoComponent.endLocalTime = EditorGUILayout.FloatField("EndTime", gogoComponent.endLocalTime);
            gogoComponent.currTime = EditorGUILayout.Slider("TimeLine",
                                                        gogoComponent.currTime,
                                                        gogoComponent.startLocalTime,
                                                        gogoComponent.endLocalTime);
            EditorGUI.indentLevel = 0;
        }
        else
        {
            gogoPlayer.currTime = EditorGUILayout.Slider("TimeLine",
                                                     gogoPlayer.currTime,
                                                     gogoPlayer.startTime,
                                                     gogoPlayer.endTime);
        }
        
    }

    protected void ShowKeyFrameGUI()
    {
        serializedObject.Update();

        List<KeyFrameBase> keyFrameList = keyFrameBaseList.ToList<KeyFrameBase>();

        bool isTimeChange = false;
        
        for (int i = 0; i < keyFrameList.Count; i++)
        {
            Color selColor = Color.white;

            if ((i == keyFrameList.Count - 1 && GetCurrTime() >= keyFrameList[i].time ) ||
                (GetCurrTime() >= keyFrameList[i].time && GetCurrTime() < keyFrameList[i+1].time))
            {
                selColor = Color.yellow;
            }

            GUI.color = selColor;
			EditorGUI.indentLevel = 0;
			keyFrameList[i].isSelected = EditorGUILayout.BeginToggleGroup(keyFrameName+i.ToString(), keyFrameList[i].isSelected);

            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
			EditorGUI.indentLevel = 1;
            keyFrameList[i].isFoldOut = EditorGUILayout.Foldout(keyFrameList[i].isFoldOut, "Time");

            string keyFrameFocus = keyFramefocusName + i.ToString();
            GUI.SetNextControlName(keyFrameFocus);
            keyFrameList[i].timeTmp = EditorGUILayout.FloatField("", keyFrameList[i].timeTmp, GUILayout.Width(100));
            
            //! 調整時間要按apply才有作用.
            string nameFocus = GUI.GetNameOfFocusedControl();
            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                isTimeChange = true;
                keyFrameList[i].time = keyFrameList[i].timeTmp;
                CheckKeyFrameTimeUnique(i);
            }

            //! 若失去focus時間就設回原值.
            if (nameFocus != keyFrameFocus && Event.current.type == EventType.Repaint)
            {
                keyFrameList[i].timeTmp = keyFrameList[i].time;
            }   

            EditorGUILayout.EndHorizontal();

            if (keyFrameList[i].isFoldOut)
            {
                CustomKeyFrameGUI(i);
            }

            EditorGUILayout.EndToggleGroup();
            CheckToggleOnlyOneSelected(i);
            GUI.color = Color.white;
        }
		serializedObject.ApplyModifiedProperties();

        if (isTimeChange)
        {
            KeyFrameSort();
        }
        
    }

	void CheckKeyFrameTimeUnique(int idx)
	{
		KeyFrameBase keyFrame = keyFrameBaseList.ElementAt<KeyFrameBase> (idx);
		for (int j = 0; j < keyFrameBaseList.Count<KeyFrameBase> (); j++) 
		{
			if (idx == j) 
			{
				continue;
			}
			KeyFrameBase compareKeyFrame = keyFrameBaseList.ElementAt<KeyFrameBase> (j);
			if (keyFrame.time == compareKeyFrame.time) 
			{
				keyFrame.time += 0.0333f;
				j = -1;
			}
		}
	}

    protected void CheckToggleOnlyOneSelected(int idx)
    {
        KeyFrameBase keyFrame = keyFrameBaseList.ElementAt<KeyFrameBase>(idx);

        if (keyFrame.isSelected)
        {
            if (currSelectedKeyFrameIdx != idx)
            {
                SetCurrTime(keyFrame.time);
                Refresh();
                //SceneView.RepaintAll();

                for (int i = 0; i < keyFrameBaseList.Count<KeyFrameBase>(); i++)
                {
                    if (i != idx)
                    {
                        keyFrameBaseList.ElementAt<KeyFrameBase>(i).isSelected = false;
                    }
                }
                currSelectedKeyFrameIdx = idx;
            }
        }
        else
        {  
            if (currSelectedKeyFrameIdx == idx)
            {
                currSelectedKeyFrameIdx = -1;
            }
        }
    }

    void ModifyComponentEnable()
    {
        GoGoComponent gogoComponent = (GoGoComponent)target;

        if (gogoComponent.enabled != gogoComponent.isLastEnableState)
        {
            gogoComponent.isLastEnableState = gogoComponent.enabled;

            if (gogoComponent.enabled)
            {
                GoGoComponent[] gogoComponenet = gogoComponent.GetComponents<GoGoComponent>();
                for (int i = 0; i < gogoComponenet.Length; i++)
                {
                    if (gogoComponenet[i] != gogoComponent)
                    {
                        gogoComponenet[i].enabled = false;
                        gogoComponenet[i].isLastEnableState = false;
                    }
                }
            }
        }
    }

    protected virtual void CustomKeyFrameGUI(int idx)
    {

    }

    protected virtual void KeyFrameSort()
    {
        //keyFrameBaseList = keyFrameBaseList.OrderBy(x => x.time);

        for (int i = 0; i < keyFrameBaseList.Count<KeyFrameBase>(); i++)
        {
            KeyFrameBase keyFrame = keyFrameBaseList.ElementAt<KeyFrameBase>(i);

            if (keyFrame.isSelected)
            {
                currSelectedKeyFrameIdx = i;
                SetCurrTime(keyFrame.time);
				GUI.FocusControl (keyFramefocusName + i.ToString());
                break;
            }
        }
    }
    
	protected virtual void AddKeyFrame()
	{
        CheckKeyFrameTimeUnique(keyFrameBaseList.Count<KeyFrameBase>() -1);
		KeyFrameSort();
		//SceneView.RepaintAll ();
	}

	protected virtual void RemoveKeyFrame()
	{
        currSelectedKeyFrameIdx = -1;
	}

	protected virtual void UpdateInEditorMode()
	{
        GoGoComponent gogoComponent = (GoGoComponent)target;
        GoGoPlayer gogoPlayer = gogoComponent.gogoPlayer;

		if (EditorApplication.isPlaying || gogoPlayer == null || 
			gogoComponent == null || !gogoComponent.enabled)
        {
            return;
        }

        //! 同一個gameobject只能有一個GoGoComponent Enable.
        //! 不然SceneView視窗拖拉物件會卡住.
        ModifyComponentEnable();

        if (IsOnSelectedKeyFrame())
        {
            //! 當KeyFrame數值有變時，重新設定數值.
            KeyFrameBase keyFrame = keyFrameBaseList.ElementAt<KeyFrameBase>(currSelectedKeyFrameIdx);
            if (keyFrame.isChange)
            {
				Refresh();
                keyFrame.isChange = false;
            }
			else if(GetCurrTime() != lastTime)
			{
				Refresh();
			}
			else
			{
				AsignDataToKeyFrame();
			}
        }
        else
        {
            Refresh();
        }

		lastTime = GetCurrTime();
       
	}

	bool IsOnSelectedKeyFrame()
	{
		if (currSelectedKeyFrameIdx == -1) 
		{
			return false;
		}

        KeyFrameBase keyFrame = keyFrameBaseList.ElementAt<KeyFrameBase>(currSelectedKeyFrameIdx);

        if (keyFrame.isSelected &&
            keyFrame.time == GetCurrTime())
        {
            return true;
        }
		
		return false;
	}

	protected virtual bool AsignDataToKeyFrame()
	{
        return false;
	}

    public void SetCurrTime(float currTime)
    {
        GoGoComponent gogoComponent = (GoGoComponent)target;
        GoGoPlayer gogoPlayer = gogoComponent.gogoPlayer;

        if (gogoComponent.isUseLocalTime)
        {
            gogoComponent.currTime = currTime;
        }
        else
        {
            gogoPlayer.currTime = currTime;
        }
    }

    public float GetCurrTime()
    {
        GoGoComponent gogoComponent = (GoGoComponent)target;
        GoGoPlayer gogoPlayer = gogoComponent.gogoPlayer;

        if (gogoComponent.isUseLocalTime)
        {
            return gogoComponent.currTime;
        }
        else
        {
            return gogoPlayer.currTime;
        }
    }

    protected void UnSelectedAllKeyFrameList()
    {
        for (int i = 0; i < keyFrameBaseList.Count<KeyFrameBase>(); i++)
        {
            keyFrameBaseList.ElementAt<KeyFrameBase>(i).isSelected = false;
        }
    }

    void Refresh()
    {
        GoGoComponent gogoComponent = (GoGoComponent)target;
        GoGoPlayer gogoPlayer = gogoComponent.gogoPlayer;

        if (gogoPlayer.gogoManager != null)
        {
            gogoPlayer.gogoManager.currTime = gogoPlayer.startTimeOnManager + (gogoPlayer.currTime / gogoPlayer.speed);
            gogoPlayer.gogoManager.RefreshPlayerTime(0.0f);
            gogoPlayer.gogoManager.Refresh();
        }
        else
        {
            gogoPlayer.Refresh();
        }

    }
}
