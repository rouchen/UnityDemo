using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoGoAnimator : GoGoComponent 
{
	public Animator animator;
    public List<AnimatorKeyFrame> animatorKeyFrameList = new List<AnimatorKeyFrame>();

	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public override void Refresh()
	{
		if (animator == null || animator.runtimeAnimatorController == null) 
		{
			return;
		}

		base.Refresh ();

		for (int i = animatorKeyFrameList.Count - 1; i >= 0; i--)
		{
			if (currTime >= animatorKeyFrameList[i].time)
			{
                CaculateAnimator(i, currTime);
				break;
			}
		}
	}

    void CaculateAnimator(int idx, float currTime)
    {
        float normalizeTime = 0.0f;
        
        if (animatorKeyFrameList[idx].animationStateLength != 0.0f &&
            animatorKeyFrameList[idx].speed != 0.0f)
        {
            float duringTime = currTime - animatorKeyFrameList[idx].time + animatorKeyFrameList[idx].animationStateStartPos;

			if (animatorKeyFrameList [idx].isLoop) 
			{
				if (animatorKeyFrameList [idx].animationStateEndPos >= animatorKeyFrameList [idx].animationStateStartPos) 
				{
					//! Loop時算出相對時間(超過結束時間，就扣掉結束時間再加上起始時間).
					while (duringTime >= animatorKeyFrameList [idx].animationStateEndPos) 
					{
						duringTime = duringTime - animatorKeyFrameList [idx].animationStateEndPos + animatorKeyFrameList [idx].animationStateStartPos;
					}
				}
			} 
			else 
			{
				//! 超過End時間，就到End就好.
				duringTime = (duringTime > animatorKeyFrameList [idx].animationStateEndPos) ? animatorKeyFrameList [idx].animationStateEndPos : duringTime;
			}


			duringTime *= animatorKeyFrameList[idx].speed;

            //! keyFrame間不內插，時間到才跳下一個KeyFrame.
            //! 美術製作時必須依照設定的SampleRate壓滿key.
            if (!animatorKeyFrameList[idx].isInterpolation)
            {
                float oneFrameTime = 1.0f / animatorKeyFrameList[idx].sampleRate;
                int fameNum = (int)(duringTime / oneFrameTime);
                duringTime = fameNum * oneFrameTime;
            }

            normalizeTime = duringTime / (animatorKeyFrameList[idx].animationStateLength * animatorKeyFrameList[idx].speed);
        }

		animator.Play(animatorKeyFrameList[idx].animationStateName, 0, normalizeTime);
        
        //! 雖然每個frame都設animation的位置，
        //! 但是speed還是要設定，因為這樣animation的Event觸發才會正確.
        animator.speed = animatorKeyFrameList[idx].speed;
        
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            animator.Update(0.0f);
        }
#endif
    }
}
