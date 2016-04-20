using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SystemMessage : MonoBehaviour 
{
    static SystemMessage instance = null;
    Canvas canvas;
    CanvasScaler canvasScaler;
    Text fpsText;
    Text memoryText;
    Text creditText;
    Text copyrightText;
    Font font;
    int sysMsgOrder = 3;
    Vector2 screenResolution = new Vector2(1080.0f, 1920.0f);

    public static SystemMessage Singleton
    {
        get 
        {
            if(instance == null)
            {
                GameObject go = new GameObject("SystemMessage");
                instance = go.AddComponent<SystemMessage>();
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

        //! Fps.
        GameObject fpsGo = new GameObject("Fps");
        fpsGo.transform.parent = transform;
        fpsGo.AddComponent<CanvasRenderer>();
        fpsText = fpsGo.AddComponent<Text>();
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        fpsText.font = font;
        fpsText.fontSize = 46;
        fpsText.alignment = TextAnchor.MiddleRight;
        RectTransform rect = fpsGo.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(-250.0f, -40.0f, 0.0f);
        rect.sizeDelta = new Vector2( 457.0f, 135.0f);
        fpsGo.AddComponent<FPS>();

        // top and right.
        fpsGo.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
        fpsGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);


        //! memeory.
        GameObject memGo = new GameObject("Memory");
        memGo.transform.parent = transform;
        memGo.AddComponent<CanvasRenderer>();
        memoryText = memGo.AddComponent<Text>();
        memoryText.font = font;
        memoryText.fontSize = 46;
        memoryText.alignment = TextAnchor.MiddleRight;
        rect = memGo.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(-250.0f, -120.0f, 0.0f);
        rect.sizeDelta = new Vector2(457.0f, 135.0f);
        memGo.AddComponent<Memory>();

        // top and right.
        memGo.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
        memGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

    }

    // Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
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
