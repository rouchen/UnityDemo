using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NumberUI : MonoBehaviour
{
    //! 數字圖.
    [Tooltip("數字圖，順序從0~9")]
    public List<Sprite> numberSpriteList;
    //! 數字位數.
    [Tooltip("數字UI Image元件，順序：個、十、百、千、萬位")]
    public List<Image> numberImageList;
    [Tooltip("不足位數前面是否補零")]
    public bool isZeroFill;

    //! 是否要播換圖動畫.
    [Tooltip("是否要播換圖動畫")]
    public bool isPlayAnimation;
    //! 是否只更改變動部分.
    [Tooltip("只修改變動的數字(動態有時只有數字變動時才要演出)")]
    public bool isPlayOnNumberChange;
    //! 動畫名稱.
    [Tooltip("AnimationState名稱")]
    public string animationName;
    
    //! 紀錄目前每個元件的數字.
    List<int> currImageNumber = new List<int>();

    //=================================

    /// <summary>
    /// Awake.
    /// </summary>
    void Awake()
    {
        for (int i = 0; i < numberImageList.Count; i++)
        {
            currImageNumber.Add(0);
        }
    }

    /// <summary>
    /// Start.
    /// </summary>
    void Start ()
    {
	    
	}
	
    /// <summary>
    /// Update.
    /// </summary>
	void Update ()
    {
	
	}

    /// <summary>
    /// 設定數字.
    /// </summary>
    /// <param name="num">數字</param>
    public void SetNumber(int num)
    {
        int tmpNum = num;   
        for (int i = numberImageList.Count-1; i >= 0; i--)
        {
            int pow = GetTenPower(i);
            int r = tmpNum / pow;
            tmpNum = tmpNum % pow;
            
            if (r == 0 && isZeroFill == false && i > 0)
            {
                numberImageList[i].gameObject.SetActive(false);

                if (isPlayAnimation && (!isPlayOnNumberChange || r != currImageNumber[i]))
                {
                    Animator anim = numberImageList[i].GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.enabled = false;
                    }
                }                  
            }
            else
            {
                numberImageList[i].gameObject.SetActive(true);
                //! 加防呆.
                if (r < numberSpriteList.Count)
                {
                    numberImageList[i].sprite = numberSpriteList[r];
                }

                if (isPlayAnimation && (!isPlayOnNumberChange || r != currImageNumber[i]))
                {
                    Animator anim = numberImageList[i].GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.enabled = true;
                        anim.Play(animationName, 0, 0.0f);
                    }
                }
            }

            currImageNumber[i] = r;
        }
    }

    /// <summary>
    /// 取得10的n次方.
    /// </summary>
    /// <param name="pow"></param>
    /// <returns></returns>
    int GetTenPower(int pow)
    {
        int num = 1;
        for (int i = 0; i < pow; i++)
        {
            num *= 10;
        }

        return num;
    }
}
