using System.Collections.Generic;
using UnityEngine;

public static class Yielders
{
    public static bool Enabled = true;

    public static int _internalCounter = 0;

    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    static UnityEngine.WaitForEndOfFrame _endOfFrame = new UnityEngine.WaitForEndOfFrame();
    public static UnityEngine.WaitForEndOfFrame EndOfFrame
    {
        get { _internalCounter++; return Enabled ? _endOfFrame : new UnityEngine.WaitForEndOfFrame(); }
    }

    static UnityEngine.WaitForFixedUpdate _fixedUpdate = new UnityEngine.WaitForFixedUpdate();
    public static UnityEngine.WaitForFixedUpdate FixedUpdate
    {
        get { _internalCounter++; return Enabled ? _fixedUpdate : new UnityEngine.WaitForFixedUpdate(); }
    }

    public static UnityEngine.WaitForSeconds GetWaitForSeconds(float seconds)
    {
        _internalCounter++;

        if (!Enabled)
            return new UnityEngine.WaitForSeconds(seconds);

        UnityEngine.WaitForSeconds wfs;

        if (!_waitForSecondsYielders.TryGetValue(seconds, out wfs))
            _waitForSecondsYielders.Add(seconds, wfs = new UnityEngine.WaitForSeconds(seconds));

        return wfs;
    }

    public static void ClearWaitForSeconds()
    {
        _waitForSecondsYielders.Clear();
    }

    public static UnityEngine.WaitForSecondsRealtime GetWaitForRtSeconds(float seconds)
    {
        _internalCounter++;

        if (!Enabled)
            return new UnityEngine.WaitForSecondsRealtime(seconds);

        UnityEngine.WaitForSecondsRealtime wfs;

        if (!_waitForRtSecondsYielders.TryGetValue(seconds, out wfs))
            _waitForRtSecondsYielders.Add(seconds, wfs = new UnityEngine.WaitForSecondsRealtime(seconds));

        return wfs;
    }

    public static void ClearWaitForRtSeconds()
    {
        _waitForRtSecondsYielders.Clear();
    }

    static Dictionary<float, UnityEngine.WaitForSeconds> _waitForSecondsYielders = new Dictionary<float, UnityEngine.WaitForSeconds>(100, new FloatComparer());
    static Dictionary<float, UnityEngine.WaitForSecondsRealtime> _waitForRtSecondsYielders = new Dictionary<float, UnityEngine.WaitForSecondsRealtime>(10, new FloatComparer());
}

/// <summary>
/// 警告：Unity的WaitForEndOfFrame已经被屏蔽，请使用Yielders
/// </summary>
public abstract class WaitForEndOfFrame { }
/// <summary>
/// 警告：Unity的WaitForFixedUpdate已经被屏蔽，请使用Yielders
/// </summary>
public abstract class WaitForFixedUpdate { }
/// <summary>
/// 警告：Unity的WaitForSeconds已经被屏蔽，请使用Yielders
/// </summary>
public abstract class WaitForSeconds { }
/// <summary>
/// 警告：Unity的WaitForSecondsRealtime已经被屏蔽，请使用Yielders
/// </summary>
public abstract class WaitForSecondsRealtime { }