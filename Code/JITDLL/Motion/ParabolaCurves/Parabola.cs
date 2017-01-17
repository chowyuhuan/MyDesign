using UnityEngine;
using System.Collections;

/// <summary>
/// 抛物线 
/// </summary>
public struct Parabola
{
    // 系数 
    float _a;
    float _b;
    float _c;

    public Parabola(float x1, float y1, float x2, float y2, float topY)
        : this()
    {
        Set(x1, y1, x2, y2, topY);
    }

    public void Set(float x1, float y1, float x2, float y2, float topY, bool isConvex = true)
    {
        float k = (y1 - y2) / (x1 - x2);
        float pa = 4 * x1 * x1 - 4 * x1 * (x1 + x2) + (x1 + x2) * (x1 + x2);
        float pb = 4 * k * x1 + 4 * topY - 2 * k * (x1 + x2) - 4 * y1;
        float pc = k * k;

        float pd = pb * pb - 4 * pa * pc;
        //float a1 = (-pb - Mathf.Sqrt(pd)) / (2 * pa);
        //float a1 = (-pb + Mathf.Sqrt(pd)) / (2 * pa);
        float a1 = isConvex ? (-pb - Mathf.Sqrt(pd)) / (2 * pa) : (-pb + Mathf.Sqrt(pd)) / (2 * pa);

        _a = a1;
        _b = k - _a * (x1 + x2);
        _c = topY + _b * _b / (4 * _a);
    }

    /// <summary>
    /// 根据x求y 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public float GetY(float x)
    {
        return _a * x * x + _b * x + _c;
    }

    /// <summary>
    /// 根据x求斜率 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public float GetSlope(float x)
    {
        return 2 * _a * x + _b;
    }
}
