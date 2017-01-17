using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorLogTool {
    public static string InitLog4Track(string fileName, string logPath, bool log4track)
    {
        if(log4track)
        {
            return Log4Track.InitLog4Track(fileName, logPath);
        }
        return null;
    }

    public static void CloseLog4Track(bool log4track)
    {
        if(log4track)
        {
            Log4Track.Close();
        }
    }

    public static void Log(string message, bool log4track)
    {
        Debug.Log(message);
        if(log4track)
        {
            Log4Track.Log(message);
        }
    }

    public static void Log(string tag, string message, bool log4track)
    {
        Debug.Log(tag + "   " + message);
        if (log4track)
        {
            Log4Track.Log(tag, message);
        }
    }

    public static void LogError(string message, bool log4track)
    {
        message = "【错误】" + message;
        Debug.LogError(message);
        if (log4track)
        {
            Log4Track.Log(message);
        }
    }

    public static void LogError(string tag, string message, bool log4track)
    {
        message = "【错误】" + message;
        Debug.LogError(message);
        if (log4track)
        {
            Log4Track.Log(tag, message);
        }
    }

    public static void BeginLog4TrackBlock(string blockName, bool log4track)
    {
        if(log4track)
        {
            Log4Track.BeginBlock(blockName);
        }
    }

    public static void EndLog4TrackBlock(bool log4track)
    {
        if(log4track)
        {
            Log4Track.EndBlock();
        }
    }
}
