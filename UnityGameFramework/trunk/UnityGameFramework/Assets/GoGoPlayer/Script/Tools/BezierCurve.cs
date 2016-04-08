using UnityEngine;
using System.Collections;

public static class BezierCurve 
{
    /// <summary>
    /// 取得p0到p2間Bezier曲線上的點.
    ///       p1    
    /// p0           p2
    /// </summary>
    /// <param name="p0">起始點</param>
    /// <param name="p1">第3點</param>
    /// <param name="p2">結束點</param>
    /// <param name="t">0~1 0為起始點 1為結束點</param>
    /// <returns></returns>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t )
    {
        t = Mathf.Clamp01(t);
        float oneMinust = 1.0f - t;
        Vector3 point = new Vector3();

        //! Bezier Curve公式.
        //! Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t)
        //! B(t) = (1-t)^2*p0 + 2*(1-t)*t*p1 + t^2*p2.
        point = oneMinust * oneMinust * p0 + 2.0f * oneMinust * t * p1 + t * t * p2;
        return point;
    }

    /// <summary>
    /// 取得p0到p4間Bezier曲線上的點.
    ///     p1      p3
    /// p0      p2
    /// </summary>
    /// <param name="p0">起始點</param>
    /// <param name="p1">第3個點</param>
    /// <param name="p2">第4個點</param>
    /// <param name="p3">結束點</param>
    /// <param name="t">0~1 0為起始點 1為結束點</param>
    /// <returns></returns>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinust = 1.0f - t;
        Vector3 point = new Vector3();

        //! Bezier Curve公式.
        //! B(t)= (1-t)^3*p0 + 3*(1-t)^2*t*p1 + 3*(1-t)*t^2*p2 + t^3*p3
        point = oneMinust * oneMinust * oneMinust * p0 +
                3.0f * oneMinust * oneMinust * t * p1 +
                3.0f * oneMinust * t * t * p2 +
                t * t * t * p3;

        return point;
    }

    /// <summary>
    /// 取得Bezier曲線上任一點的方向.
    /// </summary>
    /// <param name="p0">起始點</param>
    /// <param name="p1">第3點</param>
    /// <param name="p2">結束點</param>
    /// <param name="t">0~1 0為起始點 1為結束點</param>
    /// <returns></returns>
    public static Vector3 GetDirection(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        Vector3 dir = new Vector3();

        //! Bezier Curve上任一點方向公式
        //! B(t) = 2*(1-t)(p1-p0) + 2*t*(p2-p1)
        dir = 2.0f * (1.0f - t) * (p1 - p0) + 2.0f * t * (p2 - p1);
        dir = dir.normalized;
        return dir;
    }

    /// <summary>
    /// 取得Bezier曲線上任一點的方向.
    /// </summary>
    /// <param name="p0">起始點</param>
    /// <param name="p1">第3個點</param>
    /// <param name="p2">第4個點</param>
    /// <param name="p3">結束點</param>
    /// <param name="t">0~1 0為起始點 1為結束點</param>
    /// <returns></returns>
    public static Vector3 GetDirection(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinust = 1.0f - t;
        Vector3 dir = new Vector3();

        //! Bezier Curve上任一點方向公式
        //! B(t) = 3*(1-t)^2(p1-p0) + 6*(1-t)*t*(p2-p1) + 3*t^2*(p3-p2)
        dir = 3.0f * oneMinust * oneMinust * (p1 - p0) +
              6.0f * oneMinust * t * (p2 - p1) +
              3.0f * t * t * (p3 - p2);
        dir = dir.normalized;
        return dir;    
    }
}
