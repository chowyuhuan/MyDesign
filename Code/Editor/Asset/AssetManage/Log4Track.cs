using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Log4Track
{
    const string Split_Space = "    ";
    const string Block_Title_Tag = "++++++++++++++++++++++";
    const string Block_Tail_Tag = "---------------------";
    const int Block_Split_Line = 2;

    static System.IO.StreamWriter Log_Writer = null;
    static bool First_Block = true;
    static Stack<string> _BlockList = new Stack<string>();
    static string _DefaultTag = "Track";
    static bool _InitDone = false;

    /// <summary>
    /// 默认生成打包bundle的log文件
    /// </summary>
    public static void InitLog4Track()
    {
        try
        {
            Log_Writer = new System.IO.StreamWriter(GetLogFileName());
            Log_Writer.AutoFlush = true;
            _InitDone = true;
        }
        catch (System.IO.IOException e)
        {
            ShowException(e);
        }
    }

    public static string InitLog4Track(string fileName, string logPath)
    {
        string logfile = null;
        try
        {
            logfile = GetLogFileName(fileName, logPath);
            Log_Writer = new System.IO.StreamWriter(logfile);
            Log_Writer.AutoFlush = true;
            _InitDone = true;
        }
        catch (System.IO.IOException e)
        {
            ShowException(e);
        }
        return logfile;
    }

    static void CheckPath(string path)
    {
        try
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
        catch (System.IO.IOException e)
        {
            ShowException(e);
        }
    }

    static string GetLogFileName(string fileName = "Build_Bundle_Track", string path = "")
    {
        string basePath = System.Environment.CurrentDirectory.Replace("\\", "/");
        path = basePath + "/Logs/" + path;//强制所有Log文件放在Logs目录下
        CheckPath(path);
        return string.Format("{0}{1}{2}{3}{4}{5}", path, "/", fileName, "_", TimeStamp("yyyy_MM_dd_HH_mm_ss"), ".txt");
    }

    static void CheckInit()
    {
        if (!_InitDone)
        {
            InitLog4Track();
        }
    }

    static public void Close()
    {
        CheckInit();
        try
        {
            Log_Writer.Flush();
            Log_Writer.Close();
            Log_Writer.Dispose();
            ResetData();
        }
        catch (System.IO.IOException e)
        {
            ShowException(e);
        }
    }

    static void ResetData()
    {
        Log_Writer = null;
        _InitDone = false;
        First_Block = true;
        _BlockList.Clear();
        _DefaultTag = "Track";
    }

    static public void Log(string tag, string msg)
    {
        CheckInit();
        LogFormatLine(TimeStamp(), Split_Space, tag, Split_Space, msg);
    }

    static public void Log(string msg)
    {
        CheckInit();
        Log(_DefaultTag, msg);
    }

    static public void SetDefaultTag(string tag)
    {
        CheckInit();
        _DefaultTag = tag;
    }

    static public void BeginBlock(string blockName)
    {
        CheckInit();
        if (!First_Block)
        {
            BlockSplitSpace();
        }
        else
        {
            First_Block = false;
        }
        _BlockList.Push(blockName);
        Title(blockName);
    }

    static public void EndBlock()
    {
        CheckInit();
        if (_BlockList.Count > 0)
        {
            Tail(_BlockList.Peek());
            _BlockList.Pop();
        }
    }

    static void Title(string title)
    {
        LogFormatLine(Block_Title_Tag, Split_Space, title + " Begin", Split_Space, Block_Title_Tag);
    }

    static void Tail(string tail)
    {
        LogFormatLine(Block_Tail_Tag, Split_Space, tail + " End", Split_Space, Block_Tail_Tag);
    }

    static void LogFormatLine(string para1, string para2, string para3, string para4, string para5)
    {
        try
        {
            Log_Writer.WriteLine(string.Format("{0}{1}{2}{3}{4}", para1, para2, para3, para4, para5));
        }
        catch (System.Exception e)
        {
            ShowException(e);
        }
    }

    static void BlockSplitSpace()
    {
        for (int index = 0; index < Block_Split_Line; index++)
        {
            Log_Writer.WriteLine("");
        }
    }

    static string TimeStamp(string timeFormat = "yyyy-MM-dd HH:mm:ss.ffff")
    {
        return System.DateTime.Now.ToString(timeFormat);
    }

    static void ShowException(System.Exception e)
    {
        if (null != e)
        {
            Debug.LogError(e.Message);
            Debug.Log(e.StackTrace);
        }
    }
}
