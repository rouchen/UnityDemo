using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CreditMgr : MonoBehaviour 
{

    // (N)Coin = 1 Credit
    int coinToCredit = 3;
    // (N)Credit = 1 Game
    int ceditToGame = 1;
    int creditCnt = 0;
    int coinCnt = 0;
    int serviceCnt;
    bool freePlay;
    
    // UI componet.
    Canvas canvas;
    CanvasScaler canvasScaler;
    Text creditText;
    Font font;
    int sysMsgOrder = 4;
    Vector2 screenResolution = new Vector2(1080.0f, 1920.0f);


    static CreditMgr instance;
    public static CreditMgr Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CreditMgr");
                instance = go.AddComponent<CreditMgr>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Awake()
    {
        // set UI.
        SetUIComponent();
    }

    /// <summary>
    /// get credit count numbers.
    /// </summary>
    /// <returns></returns>
    public int GetCredit()
    {
        return creditCnt;
    }

    /// <summary>
    /// add one coin.
    /// </summary>
    public void AddCoin()
    {
        coinCnt += 1;
        creditCnt += coinCnt / coinToCredit;
        coinCnt = coinCnt % coinToCredit;
        RefreshCreditText();
    }

    /// <summary>
    /// pay to playing game.
    /// </summary>
    /// <returns>play or not</returns>
    public bool PayToPlayGame()
    {
        bool result = false;
        if (freePlay == true)
        {
            result =  true;
            RefreshCreditText();
        }
        else if ((creditCnt - ceditToGame) >= 0)
        {
            creditCnt -= ceditToGame;
            result = true;
            RefreshCreditText();
        }

        return result;
    }

    public bool CheckToPlayGame()
    {
        bool result = false;
        if (freePlay == true)
        {
            result = true;            
        }
        else if ((creditCnt - ceditToGame) >= 0)
        {
            result = true;            
        }
        return result;
    }


        /// <summary>
        /// refresh credit text.
        /// </summary>
        void RefreshCreditText()
    {
        if (creditText != null)
        {
            if (freePlay == true)
            {
                creditText.text = "FREE PLAY";
            }
            else
            {
                String text = String.Format("CREDIT(S) {0} {1}/ {2}", creditCnt.ToString(), coinCnt.ToString(), coinToCredit.ToString());
                creditText.text = text;
            }
        }
    }

    /// <summary>
    /// Set UI Component.
    /// </summary>
    void SetUIComponent()
    {
        //! Canvas.
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sysMsgOrder;
        canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = screenResolution;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 1.0f;
        canvasScaler.referencePixelsPerUnit = 100.0f;
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        RectTransform rect;

        //! Credit.
        GameObject creditGO = new GameObject("Credit");
        creditGO.transform.parent = transform;
        creditGO.AddComponent<CanvasRenderer>();
        creditText = creditGO.AddComponent<Text>();
        creditText.font = font;
        creditText.fontSize = 38;
        creditText.alignment = TextAnchor.MiddleCenter;
        RefreshCreditText();        
        rect = creditGO.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0.0f, 70.0f, 0.0f);
        rect.sizeDelta = new Vector2(457.0f, 135.0f);

        // bottom and center.
        creditGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
        creditGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {

    }

    /// <summary>
    /// 顯示訊息.
    /// </summary>
    /// <param name="show"></param>
    public void Show(bool show)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            trans.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// 設定解析度.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetResolution(float width, float height)
    {
        screenResolution.x = width;
        screenResolution.y = height;

        //! 有 canvasScaler，直接更改解析度.
        if (canvasScaler)
        {
            canvasScaler.referenceResolution = screenResolution;
        }
    }
}