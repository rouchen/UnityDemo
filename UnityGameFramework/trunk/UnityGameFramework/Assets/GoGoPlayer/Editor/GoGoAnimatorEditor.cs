using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

[CustomEditor(typeof(GoGoAnimator))]
public class GoGoAnimatorEditor : GoGoComponentEditor 
{
	GoGoAnimator gogoAnimator;
    List<ChildAnimatorState> animationStateList = new List<ChildAnimatorState>();
    AnimationClip[] animationClipArray;
    List<string> animationStateNameList = new List<string>();

	// Use this for initialization
	void Awake () 
	{
		gogoAnimator = (GoGoAnimator)target;
		gogoAnimator.animator = gogoAnimator.GetComponent<Animator> ();
        RegisterKeyFrameList(gogoAnimator.animatorKeyFrameList.Cast<KeyFrameBase>());
        EditorApplication.update += UpdateInEditorMode;

        SetMecanimData();
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

        base.OnInspectorGUI();
    }

    protected override void KeyFrameSort()
    {
        gogoAnimator.animatorKeyFrameList.Sort((x, y) => { return x.time.CompareTo(y.time); });
        base.KeyFrameSort();
    }

    protected override void AddKeyFrame()
    {
		UnSelectedAllKeyFrameList ();
        AnimatorKeyFrame keyFrame = new AnimatorKeyFrame();
		keyFrame.animaionStateIdx = 0;
        keyFrame.animationStateName = GetAnimationStateNameByIndex(0);
        int hashCode = GetAnimationClipHashCodeByIndex(0);
        keyFrame.animationStateLength = GetAnimationClipLengthByHashCode(hashCode);
        keyFrame.animationStateStartPos = 0.0f;
        keyFrame.animationStateEndPos = keyFrame.animationStateLength;
        keyFrame.time = GetCurrTime();
        keyFrame.isSelected = true;

        gogoAnimator.animatorKeyFrameList.Add(keyFrame);
		base.AddKeyFrame();
    }

    protected override void RemoveKeyFrame()
    {
        base.RemoveKeyFrame();
        List<AnimatorKeyFrame> keyFrameList = gogoAnimator.animatorKeyFrameList;
        for (int i = 0; i < keyFrameList.Count; i++)
        {
            if (keyFrameList[i].isSelected)
            {
                keyFrameList.RemoveAt(i);
                i = i - 1;
            }
        }
    }

	protected override void ShowCustomTool()
	{
		if (GUILayout.Button ("Split", GUILayout.Width(100))) 
		{
			SplitAnimation ();
		}
	}

	void SplitAnimation()
	{
		List<AnimatorKeyFrame> keyFrameList = gogoAnimator.animatorKeyFrameList;

		int currIdx = -1;
		float currTime = GetCurrTime ();
		for (int i = keyFrameList.Count - 1; i >= 0; i--) 
		{
			if ( currTime >= keyFrameList [i].time) 
			{
				currIdx = i;
				break;
			}
		}

		if (currIdx != -1) 
		{
			float endPos = (currTime - keyFrameList [currIdx].time) + keyFrameList [currIdx].animationStateStartPos;
			endPos = Mathf.Min (endPos, keyFrameList[currIdx].animationStateEndPos);

			UnSelectedAllKeyFrameList ();
			AnimatorKeyFrame keyFrame = new AnimatorKeyFrame();
			keyFrame.animaionStateIdx = keyFrameList [currIdx].animaionStateIdx;
			keyFrame.animationStateName = GetAnimationStateNameByIndex(keyFrameList [currIdx].animaionStateIdx);
			keyFrame.animationStateLength = keyFrameList [currIdx].animationStateLength;
			keyFrame.animationStateStartPos = endPos;
			keyFrame.animationStateEndPos = keyFrameList [currIdx].animationStateEndPos;
			keyFrame.speed = keyFrameList [currIdx].speed;
			keyFrame.isInterpolation = keyFrameList [currIdx].isInterpolation;
			keyFrame.sampleRate = keyFrameList [currIdx].sampleRate;
			keyFrame.isLoop = keyFrameList [currIdx].isLoop;
			keyFrame.time = GetCurrTime();
			keyFrame.isSelected = true;

			keyFrameList [currIdx].animationStateEndPos = endPos;

			gogoAnimator.animatorKeyFrameList.Add(keyFrame);
			base.AddKeyFrame ();
		}
	}

    protected override void CustomKeyFrameGUI(int idx)
    {
        List<AnimatorKeyFrame> keyFrameList = gogoAnimator.animatorKeyFrameList;

        int currAnimationStateIdx = EditorGUILayout.Popup("AnimationState", keyFrameList[idx].animaionStateIdx, animationStateNameList.ToArray());

        //! 更換動作重新設定KeyFrame資料.
        if (currAnimationStateIdx != keyFrameList[idx].animaionStateIdx)
        {
            keyFrameList[idx].animaionStateIdx = currAnimationStateIdx;
            ModifyAnimationStateDataInKeyFame(idx);
        }

        EditorGUI.indentLevel = 1;
        EditorGUILayout.LabelField("Length:   " + keyFrameList[idx].animationStateLength.ToString());
        EditorGUILayout.LabelField("PlayerStartTime:   " + keyFrameList[idx].time.ToString());
        EditorGUILayout.LabelField("PlayerEndTime:   " + (keyFrameList[idx].time + (keyFrameList[idx].animationStateEndPos - keyFrameList[idx].animationStateStartPos)).ToString());
        EditorGUI.indentLevel = 0;

        keyFrameList[idx].animationStateStartPos = EditorGUILayout.FloatField("StartPos", keyFrameList[idx].animationStateStartPos);
        keyFrameList[idx].animationStateEndPos = EditorGUILayout.FloatField("EndPos", keyFrameList[idx].animationStateEndPos);

        float speed = EditorGUILayout.FloatField("Speed", keyFrameList[idx].speed);
        if (speed != keyFrameList[idx].speed)
        {
            keyFrameList[idx].speed = speed;
            ModifyAnimationStateDataInKeyFame(idx);
        }

        keyFrameList[idx].isInterpolation = EditorGUILayout.Toggle("Interplotion", keyFrameList[idx].isInterpolation);
        if (!keyFrameList[idx].isInterpolation)
        {
            EditorGUI.indentLevel = 1;
            keyFrameList[idx].sampleRate = EditorGUILayout.IntField("SampleRate", keyFrameList[idx].sampleRate);
            EditorGUI.indentLevel = 0;
        }

		keyFrameList [idx].isLoop = EditorGUILayout.Toggle ("Loop", keyFrameList [idx].isLoop);
    }

    void SetMecanimData()
    {
        if (gogoAnimator.animator == null)
        {
            return;
        }

        animationClipArray = AnimationUtility.GetAnimationClips(gogoAnimator.gameObject);

        animationStateList.Clear();
        animationStateNameList.Clear();

        AnimatorController ac = gogoAnimator.animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        if (ac != null)
        {
            for (int i = 0; i < ac.layers.Length; i++)
            {
                AnimatorControllerLayer layer = ac.layers[i];

                for (int j = 0; j < layer.stateMachine.states.Length; j++)
                {
                    animationStateList.Add(layer.stateMachine.states[j]);
                    animationStateNameList.Add(layer.stateMachine.states[j].state.name);
                }
            }   
        }
    }

    string GetAnimationStateNameByIndex(int idx)
    {
        string animationStateName = "";
        for (int i = 0; i < animationStateNameList.Count; i++)
        {
            if (i == idx)
            {
                animationStateName = animationStateNameList[i];
                break;
            }
        }

        return animationStateName;
    }

    int GetAnimationClipHashCodeByIndex(int idx)
    {
        int hashCode = -1;
        for (int i = 0; i < animationStateList.Count; i++)
        {
            if (i == idx)
            {
                hashCode = animationStateList[i].state.motion.GetHashCode();
                break;
            }
        }
        return hashCode;
    }

    float GetAnimationClipLengthByHashCode(int hashCode)
    {
        float clipLength = 0.0f;
        for (int i = 0; i < animationClipArray.Length; i++)
        {
            if (animationClipArray[i].GetHashCode() == hashCode)
            {
                clipLength = animationClipArray[i].length;
            }
        }

        return clipLength;
    }

    void ModifyAnimationStateDataInKeyFame(int idx)
    {
        List<AnimatorKeyFrame> keyFrameList = gogoAnimator.animatorKeyFrameList;

        keyFrameList[idx].animationStateName = GetAnimationStateNameByIndex(keyFrameList[idx].animaionStateIdx);
        
        int hashCode = GetAnimationClipHashCodeByIndex(keyFrameList[idx].animaionStateIdx);
        float length =  GetAnimationClipLengthByHashCode(hashCode);

        //! 算出結束時間點佔總長比例，調整速度時一起自動調整結束時間點.
        //! 操作起來比較方便.
        float endFactor = 0.0f;
        if (keyFrameList[idx].animationStateLength != 0.0f)
        {
            endFactor = keyFrameList[idx].animationStateEndPos / keyFrameList[idx].animationStateLength;
        }

        if (keyFrameList[idx].speed != 0.0f)
        {
            keyFrameList[idx].animationStateLength = length / keyFrameList[idx].speed;
            keyFrameList[idx].animationStateEndPos = keyFrameList[idx].animationStateLength * endFactor;
        }
        
        //! 起始時間和結束時間，卡上限.
        keyFrameList[idx].animationStateStartPos = Mathf.Max(keyFrameList[idx].animationStateStartPos, 0.0f);
        keyFrameList[idx].animationStateStartPos = Mathf.Min(keyFrameList[idx].animationStateStartPos, keyFrameList[idx].animationStateLength);
        keyFrameList[idx].animationStateEndPos = Mathf.Max(keyFrameList[idx].animationStateEndPos, 0.0f);
        keyFrameList[idx].animationStateEndPos = Mathf.Min(keyFrameList[idx].animationStateEndPos, keyFrameList[idx].animationStateLength);

    }
}