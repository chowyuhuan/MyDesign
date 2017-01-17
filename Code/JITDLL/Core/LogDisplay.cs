using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogDisplay : MonoBehaviour
{
#if SCREEN_LOG
    [RuntimeInitializeOnLoadMethod]
    static void Setup()
    {
        GameObject go = new GameObject("LogDisplay");
        //go.hideFlags = HideFlags.HideAndDontSave;

        GameObject.DontDestroyOnLoad(go);

        go.AddComponent<LogDisplay>();
    }
#endif

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            _logs.Add(type.ToString() + ":" + logString + "\n" + stackTrace);
        }
        else
        {
            _logs.Add(type.ToString() + ":" + logString);
        }

        while(_logs.Count > _maxCount)
        {
            _logs.RemoveAt(0);
        }
    }

    Vector2 _scroll;
    List<string> _logs = new List<string>();

    int _maxCount = 50;

    void OnGUI()
    {
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;
        fontStyle.normal.textColor = new Color(1, 0, 0);
        fontStyle.fontSize = 22;

        _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Width(Screen.width));
        for (int i = 0; i < _logs.Count; ++i)
        {
            GUILayout.Label(_logs[i], fontStyle);
        }

        GUILayout.EndScrollView();
    }
}
