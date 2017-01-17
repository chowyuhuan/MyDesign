using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public static class Debug
{
    [Conditional("LOG")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }
    [Conditional("LOG")]
    public static void Log(object message, UnityEngine.Object obj)
    {
        UnityEngine.Debug.Log(message, obj);
    }

    [Conditional("LOG")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("LOG")]
    public static void LogWarning(object message, UnityEngine.Object obj)
    {
        UnityEngine.Debug.LogWarning(message, obj);
    }

    [Conditional("LOG")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [Conditional("LOG")]
    public static void LogError(object message, UnityEngine.Object obj)
    {
        UnityEngine.Debug.LogError(message, obj);
    }

    [Conditional("LOG")]
    public static void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    [Conditional("LOG")]
    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    [Conditional("LOG")]
    public static void LogErrorFormat(string message, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(message, args);
    }

    [Conditional("LOG")]
    public static void LogFormat(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    [Conditional("LOG")]
    public static void LogWarningFormat(string message, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(message, args);
    }

    [Conditional("LOG")]
    public static void Assert(bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }

    [Conditional("LOG")]
    public static void Assert(bool condition, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, context);
    }

    [Conditional("LOG")]
    public static void Assert(bool condition, object message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    [Conditional("LOG")]
    public static void Assert(bool condition, object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, message, context);
    }

    [Conditional("LOG")]
    public static void AssertFormat(bool condition, string format, params object[] args)
    {
        UnityEngine.Debug.AssertFormat(condition, format, args);
    }

    [Conditional("LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    [Conditional("LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }

    // Simulate release build.
#if LOG
    public static bool isDebugBuild = true;

#else
    public static bool isDebugBuild = false;
#endif
}
