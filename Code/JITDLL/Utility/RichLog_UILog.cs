using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RichLog_UILog : MonoBehaviour {
    public static RichLog_UILog Instance { get; protected set; }
    List<string> LogPageList = new List<string>();
    string LogStr { get; set; }
    string DisplayPage { get; set; }
    Vector2 ScrollPos;
    int CurrentPageIndex = -1;
    bool AutoFlipPage = true;
    const int MaxLogLength = 4096;
    const string LogFileName = "Rich_Log";

    public static void GetRichLog_UILog()
    {
        GameObject go = new GameObject("RichLog_UILog");
        RichLog_UILog uilog = go.AddComponent<RichLog_UILog>();
        uilog.ClearOld();
    }

    void Log(string logString, string stackTrace, LogType type)
    {
        CheckLogStrSize();
        LogStr = StringOperationUtil.OptimizedStringOperation.i + LogStr + logString + "\n";
    }

    void Awake()
    {
        if(null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void CheckLogStrSize()
    {
        if(LogStr.Length > MaxLogLength)
        {
            LogPageList.Add(LogStr);
            LogStr = StringOperationUtil.OptimizedStringOperation.i + "";
        }
    }

    void ClearOld()
    {
        LogStr = StringOperationUtil.OptimizedStringOperation.i + "";
        LogPageList.Clear();
        AutoFlipPage = true;
    }

    string TimeStamp(string timeFormat = "yyyy_MM_dd_HH_mm_ss")
    {
        return System.DateTime.Now.ToString(timeFormat);
    }

    void SaveToFile()
    {
        try
        {
            System.IO.StreamWriter Log_Writer = new System.IO.StreamWriter(GetLogFileName());
            Log_Writer.AutoFlush = true;
            for (int index = 0; index < LogPageList.Count; ++index )
            {
                Log_Writer.Write(LogPageList[index]);
            }
            Log_Writer.Write(LogStr);
            Log_Writer.Flush();
            Log_Writer.Close();
            Log_Writer.Dispose();
        }
        catch (System.IO.IOException e)
        {
            ShowException(e);
        }
    }

    void ShowException(System.Exception e)
    {
        if(null != e)
        {
            Log(e.Message, null, UnityEngine.LogType.Log);
            Log(e.StackTrace, null, UnityEngine.LogType.Log);
        }
    }

    string GetLogFileName()
    {
        string basePath = Application.persistentDataPath;
        string timeStamp = TimeStamp();
        return string.Format("{0}/{1}_{2}{3}", basePath, LogFileName, timeStamp, ".txt");
    }

    string GetLogPage(int index)
    {
        if(LogPageList.Count > 0)
        {
            return LogPageList[Mathf.Clamp(index, 0, LogPageList.Count - 1)];
        }
        return "";
    }

    void PauseLogPageFlip()
    {
        AutoFlipPage = false;
        CurrentPageIndex = LogPageList.Count;
        LogPageList.Add(LogStr);
        DisplayPage = GetLogPage(CurrentPageIndex);
        LogStr = StringOperationUtil.OptimizedStringOperation.i + "";
    }

    void ContinueLogPageFlip()
    {
        AutoFlipPage = true;
        CurrentPageIndex = LogPageList.Count;
        DisplayPage = LogStr;
    }

    void OnGUI()
    {
        using(var areascrope = new GUILayout.AreaScope(new Rect(100, 100, Screen.width - 100, Screen.height - 100)))
        {
            using (var scrollView = new GUILayout.ScrollViewScope(ScrollPos, GUILayout.Width(Screen.width - 150), GUILayout.Height(Screen.height - 280)))
            {
                ScrollPos = scrollView.scrollPosition;
                GUILayout.Label(AutoFlipPage ? LogStr : DisplayPage);
            }
            using(var herizontal = new GUILayout.HorizontalScope(GUILayout.Height(100)))
            {
                if (GUILayout.Button("Clear", GUILayout.Height(100), GUILayout.Width(150)))
                {
                    ClearOld();
                }
                if (GUILayout.Button("Save To File", GUILayout.Width(150)))
                {
                    SaveToFile();
                }
                if (GUILayout.Button("-", GUILayout.Height(100), GUILayout.Width(100)))
                {
                    if (AutoFlipPage)
                    {
                        PauseLogPageFlip();
                    }
                    else
                    {
                        DisplayPage = GetLogPage(--CurrentPageIndex);
                    }
                }
                if (GUILayout.Button(AutoFlipPage ? "Pause" : "Continue", GUILayout.Height(100), GUILayout.Width(100)))
                {
                    if (AutoFlipPage)
                    {
                        PauseLogPageFlip();
                    }
                    else
                    {
                        ContinueLogPageFlip();
                    }
                }
                GUILayout.Label(AutoFlipPage ? LogPageList.Count.ToString() : CurrentPageIndex.ToString(), GUILayout.Height(100), GUILayout.Width(100));
                if (GUILayout.Button("+", GUILayout.Height(100), GUILayout.Width(100)))
                {
                    if (AutoFlipPage)
                    {
                        PauseLogPageFlip();
                    }
                    else
                    {
                        DisplayPage = GetLogPage(++CurrentPageIndex);
                    }
                }
            }
        }

    }
}
