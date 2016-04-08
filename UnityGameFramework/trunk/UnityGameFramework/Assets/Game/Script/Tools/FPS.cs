using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class FPS : MonoBehaviour
{
    public float updateInterval = 0.5F;
    int frames = 0;
    float timeleft;
    float lastTimeSinceStartUp;
    Text fpsTxt;
    StringBuilder stringBuilder = new StringBuilder(128, 128);
    void Start()
    {
        fpsTxt = GetComponent<Text>();
        if (fpsTxt == null)
        {
            Debug.LogError("Text Componenet Not Found !!");
        }
        lastTimeSinceStartUp = Time.realtimeSinceStartup;
    }

    void Update()
    {  
        ++frames;
        timeleft = Time.realtimeSinceStartup - lastTimeSinceStartUp;
        
        if (timeleft >= updateInterval)
        {
            float fps = frames / timeleft;
            stringBuilder.Length = 0;
            stringBuilder.AppendFormat("{0:F2} FPS", fps);
            //string format = System.String.Format("{0:F2} FPS", fps);
            //fpsTxt.text = format;
            fpsTxt.text = stringBuilder.ToString();
            if (fps < 10)
            {
                fpsTxt.color = Color.red;
            }
            else if (fps < 30)
            {
                fpsTxt.color = Color.yellow;
            }
            else
            {
                fpsTxt.color = Color.green;
            }

            lastTimeSinceStartUp = Time.realtimeSinceStartup;
            frames = 0;
        }
    }
}
