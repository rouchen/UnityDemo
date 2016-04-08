using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GoGoManager))] 
public class GoGoManagerEditor : Editor 
{
	GoGoManager gogoManager;
	SerializedProperty gogoPlayerProperty;
	int currGoGoPlayerNum = -1;
	float lastRealTime;

	void Awake () 
	{
		gogoManager = (GoGoManager)target;
		EditorApplication.update += UpdateInEditorMode;
		ResetTime ();
	}

	public override void OnInspectorGUI()
	{
		if (EditorApplication.isPlaying) 
		{
			return;
		}

		gogoManager.startTime = EditorGUILayout.FloatField ("Start Time", gogoManager.startTime);
		gogoManager.endTime = EditorGUILayout.FloatField ("End Time", gogoManager.endTime);
		gogoManager.isLoop = EditorGUILayout.Toggle ("Loop", gogoManager.isLoop);
		gogoManager.speed = EditorGUILayout.FloatField ("speed", gogoManager.speed);

		string playName = (gogoManager.isPlay) ? "||" : ">>";
		if (GUILayout.Button(playName))
		{
            gogoManager.isPlay = !gogoManager.isPlay;
		}

		gogoManager.currTime = EditorGUILayout.Slider ("TimeLine", gogoManager.currTime, gogoManager.startTime, gogoManager.endTime);

		serializedObject.Update ();
		gogoPlayerProperty = serializedObject.FindProperty ("gogoPlayerList");
		EditorGUILayout.PropertyField (gogoPlayerProperty, true);
		serializedObject.ApplyModifiedProperties ();

		CheckGoGoPlayerInitialize ();
	}

	void UpdateInEditorMode()
	{
		if (EditorApplication.isPlaying) 
		{
			return;
		}

		RefreshTime ();
		Refresh ();
	}

	void OnDestroy()
	{
		EditorApplication.update -= UpdateInEditorMode;
	}

	void RefreshTime()
	{
		float deltaTime = Time.realtimeSinceStartup - lastRealTime;
		lastRealTime = Time.realtimeSinceStartup;

		gogoManager.RefreshTime(deltaTime);
        gogoManager.RefreshPlayerTime(deltaTime);
	}

	void Refresh()
	{
        gogoManager.Refresh();
	}

	void ResetTime()
	{
		lastRealTime = Time.realtimeSinceStartup;
	}
		
	void CheckGoGoPlayerInitialize()
	{
		List<GoGoPlayer> gogoPlayerList = gogoManager.gogoPlayerList;

		int gogoPlayerNum = 0;

		for (int i = 0; i < gogoPlayerList.Count; i++) 
		{
			if (gogoPlayerList [i] != null) 
			{
				gogoPlayerNum++;
			}
		}

		if (gogoPlayerNum != currGoGoPlayerNum) 
		{
			gogoManager.InitializeAllPlayer ();
			currGoGoPlayerNum = gogoPlayerNum;
		}
	}
}
