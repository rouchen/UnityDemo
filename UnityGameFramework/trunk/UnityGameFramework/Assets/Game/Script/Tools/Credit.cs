using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    static Credit instance = null;
    Canvas canvas;
    CanvasScaler canvasScaler;
    Text fpsText;
    Text memoryText;
    Text creditText;
    Text copyrightText;
    Font font;
    int sysMsgOrder = 4;
    Vector2 screenResolution = new Vector2(1080.0f, 1920.0f);

    public static Credit Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Credit");
                instance = go.AddComponent<Credit>();
                DontDestroyOnLoad(go);

            }
            return instance;
        }
    }

    void Awake()
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
        creditText.text = "CREDIT(S) X Y/ Y";
        rect = creditGO.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0.0f, 70.0f, 0.0f);
        rect.sizeDelta = new Vector2(457.0f, 135.0f);

        // bottom and center.
        creditGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
        creditGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);

        /*
        //! Copyright.
        GameObject copyrightGO = new GameObject("Copyright");
        copyrightGO.transform.parent = transform;
        copyrightGO.AddComponent<CanvasRenderer>();
        copyrightText = copyrightGO.AddComponent<Text>();
        copyrightText.font = font;
        copyrightText.fontSize = 38;
        copyrightText.alignment = TextAnchor.MiddleCenter;
        copyrightText.text = "© 2016 IGS ALL RIGHTS RESEAVED.";
        rect = copyrightGO.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0.0f, 25.0f, 0.0f);
        rect.sizeDelta = new Vector2(680.0f, 135.0f);

        // bottom and center.
        copyrightGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
        copyrightGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);*/
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
