using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GoGoSound))]
public class GoGoSoundEditor : GoGoComponentEditor
{
    GoGoSound gogoSound;

    void Awake()
    {
        gogoSound = (GoGoSound)target;
        gogoSound.audioSource = gogoSound.GetComponent<AudioSource>();
        RegisterKeyFrameList(gogoSound.soundKeyFrameList.Cast<KeyFrameBase>());
        EditorApplication.update += UpdateInEditorMode;
    }

    void OnDestroy()
    {
        EditorApplication.update -= UpdateInEditorMode;
    }

    protected override void UpdateInEditorMode()
    {
        if (EditorApplication.isPlaying ||
           gogoSound.audioSource == null ||
           gogoSound.audioSource.clip == null)
        {
            return;
        }

        base.UpdateInEditorMode();

		if (gogoSound.audioSource.isPlaying) 
		{
			Repaint();
		}
    }

    public override void OnInspectorGUI()
    {
        if (EditorApplication.isPlaying || 
            gogoSound.audioSource == null || 
            gogoSound.audioSource.clip == null)
        {
            return;
        }

		EditorGUILayout.FloatField ("clip time", gogoSound.audioSource.time);
		EditorGUILayout.FloatField ("clip length", gogoSound.audioSource.clip.length);

       base.OnInspectorGUI();
    }

    protected override void KeyFrameSort()
    {
        gogoSound.soundKeyFrameList.Sort((x, y) => { return x.time.CompareTo(y.time); });
        base.KeyFrameSort();
    }

    protected override void AddKeyFrame()
    {
		UnSelectedAllKeyFrameList ();
        SoundKeyFrame keyFrame = new SoundKeyFrame();
        keyFrame.time = GetCurrTime();
        keyFrame.isSelected = true;

        gogoSound.soundKeyFrameList.Add(keyFrame);
		base.AddKeyFrame();
    }

    protected override void RemoveKeyFrame()
    {
        base.RemoveKeyFrame();
        List<SoundKeyFrame> keyFrameList = gogoSound.soundKeyFrameList;
        for (int i = 0; i < keyFrameList.Count; i++)
        {
            if (keyFrameList[i].isSelected)
            {
                keyFrameList.RemoveAt(i);
                i = i - 1;
            }
        }
    }

    protected override void CustomKeyFrameGUI(int idx)
    {
        List<SoundKeyFrame> keyFrameList = gogoSound.soundKeyFrameList;

        keyFrameList[idx].playType = (SoundKeyFrame.PlayType)EditorGUILayout.EnumPopup("PlayType", keyFrameList[idx].playType);
        keyFrameList[idx].volume = EditorGUILayout.Slider("Volume", keyFrameList[idx].volume, 0.0f, 1.0f);
    }

}
