using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GoGoPlayer))] 
public class GoGoPlayerEditor : Editor 
{
	GoGoPlayer gogoPlayer;
	SerializedProperty gogoComponentProperty;
    int currGoGoGameObjectNum = -1;
    float lastRealTime;

	void Awake()
	{
		gogoPlayer = (GoGoPlayer)target;
		EditorApplication.update += UpdateInEditorMode;
        
        ResetTime();
	}

	public override void OnInspectorGUI()
	{
		if (EditorApplication.isPlaying) 
		{
			return;
		}

		gogoPlayer.startTimeOnManager = EditorGUILayout.FloatField ("Time On Manager:", gogoPlayer.startTimeOnManager);

		EditorGUILayout.Space ();

		gogoPlayer.startTime = EditorGUILayout.FloatField ("Start Time:", gogoPlayer.startTime);
		gogoPlayer.endTime = EditorGUILayout.FloatField ("End Time:", gogoPlayer.endTime);
		gogoPlayer.isLoop = EditorGUILayout.Toggle("Loop", gogoPlayer.isLoop);
		gogoPlayer.speed = EditorGUILayout.FloatField ("speed", gogoPlayer.speed);

        string playName = (gogoPlayer.isPlay) ? "||" : ">>";
        if (GUILayout.Button(playName))
        {
            gogoPlayer.isPlay = !gogoPlayer.isPlay;
        }

		gogoPlayer.currTime = EditorGUILayout.Slider("TimeLine", gogoPlayer.currTime, gogoPlayer.startTime, gogoPlayer.endTime);
	
		serializedObject.Update();
        gogoComponentProperty = serializedObject.FindProperty("gogoGameObjectList");
		EditorGUILayout.PropertyField(gogoComponentProperty, true);
		serializedObject.ApplyModifiedProperties();
		CheckGoGoComponentInitialize();
	}

	void UpdateInEditorMode()
	{
		if (EditorApplication.isPlaying) 
		{
			return;
		}

        RefreshTime();
        Refresh();
	}

	void CheckGoGoComponentInitialize()
	{
        List<GameObject> gameObjectList = gogoPlayer.gogoGameObjectList;

		int gameObjectNum = 0;

        for (int i = 0; i < gameObjectList.Count; i++) 
		{
            if (gameObjectList[i] != null)
			{
                gameObjectNum++;
			}
		}

        if (currGoGoGameObjectNum != gameObjectNum) 
		{
            gogoPlayer.GetAllComponents();
            currGoGoGameObjectNum = gameObjectNum;
		}
	}

	void OnDestroy()
	{
		EditorApplication.update -= UpdateInEditorMode;
	}

    void RefreshTime()
    {
        float deltaTime = Time.realtimeSinceStartup - lastRealTime;
        lastRealTime = Time.realtimeSinceStartup;

        gogoPlayer.RefreshTime(deltaTime);
        if (gogoPlayer.gogoManager != null)
        {
            
            RefreshManagerTime(deltaTime);
        }
    }

    void RefreshManagerTime(float deltaTime)
    {
        gogoPlayer.gogoManager.currTime = gogoPlayer.startTimeOnManager + (gogoPlayer.currTime / gogoPlayer.speed);
        gogoPlayer.gogoManager.RefreshPlayerTime(deltaTime);
    }

    void Refresh()
    {
        if (gogoPlayer.gogoManager == null)
        {
            gogoPlayer.Refresh();
        }
        else
        {
            gogoPlayer.gogoManager.Refresh();
        }
    }

    void ResetTime()
    {
        lastRealTime = Time.realtimeSinceStartup;
    }

}
