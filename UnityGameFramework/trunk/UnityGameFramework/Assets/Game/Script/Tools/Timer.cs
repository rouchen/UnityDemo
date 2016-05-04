using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    public class TimeCounter
    {
        //! 設定時間.
        public float timeSet = 0.0f;
        //! 計時.
        public float timeCount = 0.0f;
        //! 是否倒數.
        public bool isCountDown = false;
        //! 是否開始計時.
        public bool isCount = false;

        //!delegate.
        public delegate void TimeCallBack();
        public TimeCallBack timeUpCallBack;
        public TimeCallBack everySecondCallBack;

        //! TimeUp CallBack是否Call過.
        public bool isTimeUpCallBackCalled;
        //! 上一次時間(單位秒).
        public int lastTimeInSecond;
    }

    //! instance.
    static Timer instance = null;
    //! 計時器
    Dictionary<string, TimeCounter> timerCounterDic = new Dictionary<string, TimeCounter>();

    //! Singleton.
    public static Timer Singleton
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Timer");
                instance = go.AddComponent<Timer>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    //=====================================================

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
        foreach (KeyValuePair<string, TimeCounter> counter in timerCounterDic)
        {
            TimeCounter timeCounter = counter.Value;
            if (timeCounter.isCount)
            {
                if (timeCounter.isCountDown)
                {
                    timeCounter.timeCount -= Time.deltaTime;
                    
                    //! 時間到呼叫timeUpCallBack回呼函式.
                    if (timeCounter.timeCount <= 0.0f && 
                        timeCounter.timeUpCallBack != null &&
                        timeCounter.isTimeUpCallBackCalled == false)
                    {
                        timeCounter.timeUpCallBack();
                        timeCounter.isTimeUpCallBackCalled = true;
                    }

                }
                else
                {
                    timeCounter.timeCount += Time.deltaTime;
                    
                    //! 時間到呼叫timeUpCallBack回呼函式.
                    if (timeCounter.timeCount >= timeCounter.timeSet &&
                        timeCounter.timeUpCallBack != null &&
                        timeCounter.isTimeUpCallBackCalled == false)
                    {
                        timeCounter.timeUpCallBack();
                        timeCounter.isTimeUpCallBackCalled = true;
                    }
                }

                //! 秒數有變化時，呼叫everySecondCallBack回呼函式.
                int timeInSecond = (int)timeCounter.timeCount;
                if (timeInSecond != timeCounter.lastTimeInSecond &&
                    timeCounter.everySecondCallBack != null)
                {
                    timeCounter.everySecondCallBack();
                    timeCounter.lastTimeInSecond = timeInSecond;
                }

            }
        }
	}

    /// <summary>
    /// 註冊計時器.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <param name="timeSet">計時時間</param>
    /// <param name="isCountDown">是否倒數</param>
    /// <returns></returns>
    public bool RegisterCounter(string timeName, float timeSet, bool isCountDown)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            return false;
        }

        TimeCounter counter = new TimeCounter();
        counter.timeSet = timeSet;
        counter.isCountDown = isCountDown;
        if (isCountDown)
        {
            counter.timeCount = timeSet;
        }
        counter.lastTimeInSecond = (int)counter.timeCount;

        timerCounterDic.Add(timeName, counter);
        return true;
    }

    /// <summary>
    /// 反註冊計時器.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public bool UnRegisterCounter(string timeName)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            timerCounterDic[timeName].timeUpCallBack = null;
            timerCounterDic[timeName].everySecondCallBack = null;
            timerCounterDic[timeName] = null;
            timerCounterDic.Remove(timeName);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 反註冊全部計時器.
    /// </summary>
    public void UnRegisterAllCounter()
    {
        timerCounterDic.Clear();
    }

    /// <summary>
    /// 重置時間
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <param name="timeSet">計時時間</param>
    /// <returns></returns>
    public bool ResetTime(string timeName, float timeSet)
    {
        if (!timerCounterDic.ContainsKey(timeName))
        {
            return false;
        }

        TimeCounter counter = timerCounterDic[timeName];
        counter.timeSet = timeSet;
        counter.isTimeUpCallBackCalled = false;
        if (counter.isCountDown)
        {
            counter.timeCount = timeSet;
        }
        else
        {
            counter.timeCount = 0.0f;
        }
        counter.lastTimeInSecond = (int)counter.timeCount;

        return true;
    }

    /// <summary>
    /// 重置時間.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public bool ResetTime(string timeName)
    {
        if (!timerCounterDic.ContainsKey(timeName))
        {
            return false;
        }

        TimeCounter counter = timerCounterDic[timeName];
        counter.isTimeUpCallBackCalled = false;
        if (counter.isCountDown)
        {
            counter.timeCount = counter.timeSet;
        }
        else
        {
            counter.timeCount = 0.0f;
        }
        counter.lastTimeInSecond = (int)counter.timeCount;

        return true;
    }

    /// <summary>
    /// 重置時間.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <param name="timeSet">計時時間</param>
    /// <param name="isCoutDown">是否倒數</param>
    /// <returns></returns>
    public bool ResetTime(string timeName, float timeSet, bool isCoutDown)
    {
        if (!timerCounterDic.ContainsKey(timeName))
        {
            return false;
        }

        TimeCounter counter = timerCounterDic[timeName];
        counter.timeSet = timeSet;
        counter.isCountDown = isCoutDown;
        counter.isTimeUpCallBackCalled = false;
        if (counter.isCountDown)
        {
            counter.timeCount = timeSet;
        }
        else
        {
            counter.timeCount = 0.0f;
        }
        counter.lastTimeInSecond = (int)counter.timeCount;

        return true;
    }

    /// <summary>
    /// 開始計時
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public bool StartCount(string timeName)
    {
        if (!timerCounterDic.ContainsKey(timeName))
        {
            return false;
        }

        timerCounterDic[timeName].isCount = true;
        return true;
    }

    /// <summary>
    /// 停止計時.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public bool StopCount(string timeName)
    {
        if (!timerCounterDic.ContainsKey(timeName))
        {
            return false;
        }

        timerCounterDic[timeName].isCount = false;
        return true;
    }

    /// <summary>
    /// 取得時間.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public float GetTime(string timeName)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            float count = timerCounterDic[timeName].timeCount;
            return count;
        }

        return 0.0f;
    }

    /// <summary>
    /// 是否時間到.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public bool isTimeUp(string timeName)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            TimeCounter counter = timerCounterDic[timeName];
            if (counter.isCountDown)
            {
                if (counter.timeCount <= 0.0f)
                {
                    return true;
                }
            }
            else
            {
                if (counter.timeCount >= counter.timeSet)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 是否倒數計時.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public bool IsCountDown(string timeName)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            bool isCountDown = timerCounterDic[timeName].isCountDown;
            return isCountDown;
        }
        return false;
    }

    /// <summary>
    /// 取得設定的時間.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <returns></returns>
    public float GetTimeSet(string timeName)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            float timeSet = timerCounterDic[timeName].timeSet;
            return timeSet;
        }

        return 0.0f;
    }

    /// <summary>
    /// 加入時間到回呼函式
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <param name="callBack">回呼函式</param>
    public void AddTimeUpCallBack(string timeName, TimeCounter.TimeCallBack callBack)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            timerCounterDic[timeName].timeUpCallBack += callBack;
        }
    }

    /// <summary>
    /// 刪除時間到回呼函式.
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <param name="callBack">回呼函式</param>
    public void DeleteTimeUpCallBack(string timeName, TimeCounter.TimeCallBack callBack)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            timerCounterDic[timeName].timeUpCallBack -= callBack;
        }
    }

    /// <summary>
    /// 刪除時間到回呼函式
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    public void DeleteTimeUpCallBack(string timeName)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            timerCounterDic[timeName].timeUpCallBack = null;
        }
    }

    /// <summary>
    /// 加入每秒回呼函式
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <param name="callBack">回呼函式</param>
    public void AddEverySecondCallBack(string timeName, TimeCounter.TimeCallBack callBack)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            timerCounterDic[timeName].everySecondCallBack += callBack;
        }
    }

    /// <summary>
    /// 刪除每秒回呼函式
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    /// <param name="callBack">回呼函式</param>
    public void DeleteEverySecondCallBack(string timeName, TimeCounter.TimeCallBack callBack)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            timerCounterDic[timeName].everySecondCallBack -= callBack;
        }
    }

    /// <summary>
    /// 刪除每秒回呼函式
    /// </summary>
    /// <param name="timeName">計時器名稱</param>
    public void DeleteEverySecondCallBack(string timeName)
    {
        if (timerCounterDic.ContainsKey(timeName))
        {
            timerCounterDic[timeName].everySecondCallBack = null;
        }
    }
}
