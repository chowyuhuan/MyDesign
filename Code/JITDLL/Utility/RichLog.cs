/**************************************************************************************************
 This class is targeting at classify diffrent logs to make your test work easier.
 Easily to use:
 step 1:Add module name to class Modules:
    public sealed class Modules
    {
        public const string Test = "Test";
        public const string Default = "Default";
        //add your module here and add the corresponding logformat params to LogConfig
        public const string ExampleModule = "ExampleModule";
    }
 step 2:Add module format to class LogConfig:
     protected sealed class LogConfig
    {
        ...
        static Dictionary<string, LogFormat> _ModuleFormats = new Dictionary<string, LogFormat>(){
            {Modules.Test, new LogFormat(){_Format = ELogFormat.E_All, _Size = ELogFontSize.XL, _Color = LogColor.olive}},
            {Modules.Default, new LogFormat(){_Format = ELogFormat.E_All, _Size = ELogFontSize.S, _Color = LogColor.white}},
            //add your module format here,note that the format must be added into Mudules or the moudele's format might not work.
            {Modules.ExampleModule, new LogFormat(){_Format = ELogFormat.E_All, _Size = ELogFontSize.S, _Color = LogColor.white}},
        };
        ...
    }
 step 3: Use RichLog in your code:
     RichLog.RL_Debug.Log(RichLog.Modules.ExampleModule, "This is one example message !");
     RichLog.RL_Debug.LogWarning(RichLog.Modules.ExampleModule, "This is one example message !");
     RichLog.RL_Debug.LogError(RichLog.Modules.ExampleModule, "This is one example message !");
 * 
 * 
 Not satisfied with the config format??It's all right,just use your own custom format with RichLog.Modules.RawMessage:
    RichLog.RL_Debug.Log(RichLog.Modules.RawMessage, RichLog.LogFormat.FormatMsg("test:RichLog.ELogFontSize.XXL", RichLog.ELogFormat.E_All, (int)RichLog.ELogFontSize.XXL, RichLog.LogColor.orange));
 or like this:
    RichLog.RL_Release.Log(RichLog.Modules.RawMessage, "<color=green><size=10>myModuleName</size></color>", "<color=green><size=20>myMessage</size></color>"); 
****************************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RichLog
{
    public class LogColor
    {
        #region system color
        public const string cyan = "cyan";//#00ffffff
        public const string black = "black";//#000000ff
        public const string blue = "blue";//#0000ffff
        public const string brown = "brown";//#a52a2aff
        public const string darkblue = "darkblue";//#0000a0ff
        public const string fuchsia = "fuchsia";//#ff00ffff
        public const string green = "green";//#008000ff
        public const string grey = "grey";//#808080ff
        public const string lightblue = "lightblue";//#add8e6ff
        public const string lime = "lime";//#00ff00ff
        public const string maroon = "maroon";//#800000ff
        public const string navy = "navy";//#000080ff
        public const string olive = "olive";//#808000ff
        public const string orange = "orange";//#ffa500ff //used by dqb
        public const string purple = "purple";//#800080ff
        public const string red = "red";//#ff0000ff // used by system(error log color)
        public const string silver = "silver";//#c0c0c0ff
        public const string teal = "teal";//#008080ff
        public const string white = "white";//#ffffffff //used by system(default log color)
        public const string yellow = "yellow";//#ffff00ff //used by system(warning log color)
        #endregion
        /*
           Not found your color ? All right, just add it here like: 
         *      public const string mycolor = "#b2d235ff";
         * You can pick your color value from here:http://bbs.bianzhirensheng.com/color01.html.
         * And be aware that the color value you pick from the page are in RGB format, so just fix it by add "ff" suffix.
         * */
    }

    public enum ELogFormat
    {
        None = 0,
        E_Bold = 1,
        E_Italics = 2,
        E_Size = 4,
        E_Color = 8,
        E_All
    }

    public enum ELogFontSize
    {
        S = 14,
        L = 18,
        XL = 22,
        XXL = 26,
        Big = 34,
        Huge = 40,
    }

    public sealed class Modules
    {
        public const string RawMessage = "RawMessage";//do not add this module to LogConfig
        public const string Test = "Test";
        public const string Default = "Default";
        //add your module here and add the corresponding logformat params to LogConfig

    }

    protected sealed class LogConfig
    {
        public static bool _OpenRichLog = true;//若要屏蔽所有的格式，改为false
        public static bool _AllowUnAssignedModule = true;//默认允许不指定模块名
        static Dictionary<string, LogFormat> _ModuleFormats = new Dictionary<string, LogFormat>(){
            {Modules.Test, new LogFormat(){_Format = ELogFormat.E_All, _Size = ELogFontSize.L, _Color = LogColor.olive, _OpenModule = true}},
            {Modules.Default, new LogFormat(){_Format = ELogFormat.E_All, _Size = ELogFontSize.L, _Color = LogColor.white, _OpenModule = true}},
            //add your module format here,note that the format must be added into Mudules or the moudele's format might not work.

        };

        public static bool GetModuleFormat(string module, out LogFormat moduleFormat)
        {
            return _ModuleFormats.TryGetValue(module, out moduleFormat);
        }
    }

    protected sealed class FormatTag
    {
        const string _BoldTagString = "<b>{0}</b>";
        const string _ItalicsTagString = "<i>{0}</i>";
        const string _SizeTagString = "<size={0}>{1}</size>";
        const string _ColorTagString = "<color={0}>{1}</color>";
        public static string Bold(string message)
        {
            return string.Format(_BoldTagString, message);
        }

        public static string Italics(string message)
        {
            return string.Format(_ItalicsTagString, message);
        }

        public static string Size(string message, int fontSize)
        {
            return string.Format(_SizeTagString, fontSize.ToString(), message);
        }

        public static string Color(string message, string color)
        {
            return string.Format(_ColorTagString, color, message);
        }
    }

    public sealed class LogFormat
    {
        public ELogFormat _Format = ELogFormat.None;
        public ELogFontSize _Size = ELogFontSize.S;
        public string _Color = LogColor.white;
        public bool _OpenModule = true;

        public static string FormatMsg(string messag, LogFormat format)
        {
            return format != null ? FormatMsg(messag, format._Format, (int)format._Size, format._Color) : messag;
        }

        public static string FormatMsg(string messag, ELogFormat eFormats, int size = (int)ELogFontSize.S, string color = LogColor.white)
        {
            if (ELogFormat.E_Bold == (ELogFormat.E_Bold & eFormats) || ELogFormat.E_All == eFormats)
            {
                messag = FormatTag.Bold(messag);
            }
            if (ELogFormat.E_Italics == (ELogFormat.E_Italics & eFormats) || ELogFormat.E_All == eFormats)
            {
                messag = FormatTag.Italics(messag);
            }
            if (ELogFormat.E_Size == (ELogFormat.E_Size & eFormats) || ELogFormat.E_All == eFormats)
            {
                messag = FormatTag.Size(messag, size);
            }
            if (ELogFormat.E_Color == (ELogFormat.E_Color & eFormats) || ELogFormat.E_All == eFormats)
            {
                messag = FormatTag.Color(messag, color);
            }
            return messag;
        }
    }

    protected sealed class HeaderTag
    {
        const string _ModuleTagString = "【{0}】{1}";
        const string _SubModuleTagString = "【{0}】【{1}】{2}";
        const string _ReleaseWaringString = "<color=red>![ReleaseLogWaring]</color>{0}";
        public static string Module(string moduleName, string message)
        {
            LogFormat formatter = null;
            if (LogConfig.GetModuleFormat(moduleName, out formatter) && LogConfig._OpenRichLog)
            {
                moduleName = LogFormat.FormatMsg(moduleName, formatter);
                message = LogFormat.FormatMsg(message, formatter);
            }
            return string.Format(_ModuleTagString, moduleName, message);
        }

        public static string SubModule(string moduleName, string subModuleName, string message)
        {
            LogFormat formatter = null;
            if (LogConfig.GetModuleFormat(moduleName, out formatter) && LogConfig._OpenRichLog)
            {
                moduleName = LogFormat.FormatMsg(moduleName, formatter);
                subModuleName = LogFormat.FormatMsg(subModuleName, formatter);
                message = LogFormat.FormatMsg(message, formatter);
            }
            return string.Format(_SubModuleTagString, moduleName, subModuleName, message);
        }

        public static string ReleaseWarning(string message)
        {
            return string.Format(_ReleaseWaringString, message);
        }
    }

    protected class LogHandler
    {
        public virtual bool BLog(string moduleName)
        {
            return true;
        }
        public virtual bool BLogToUI()
        {
#if LOG_TO_UI
            return true;
#else
            return false;
#endif
        }

        public virtual string FormatLogMessage(string moduleName, string message)
        {
            return HeaderTag.Module(moduleName, message);
        }
        public virtual string FormatLogMessage(string moduleName, string subModuleName, string message)
        {
            return HeaderTag.SubModule(moduleName, subModuleName, message);
        }

        public void Log(string moduleName, string message)
        {
            if (BLog(moduleName))
            {
                if (BLogToUI())
                {
                    LogStringToUI(message);
                }
                else
                {
                    Debug.Log(message);
                }
            }
        }

        public void Log(string moduleName, string message, Object context)
        {
            if (BLog(moduleName))
            {
                if (BLogToUI())
                {
                    LogStringToUI(message);
                }
                else
                {
                    Debug.Log(message, context);
                }
            }
        }

        public void LogWarning(string moduleName, string message)
        {
            if (BLog(moduleName))
            {
                if (BLogToUI())
                {
                    LogStringToUI(message);
                }
                else
                {
                    Debug.LogWarning(message);
                }
            }
        }

        public void LogWarning(string moduleName, string message, Object context)
        {
            if (BLog(moduleName))
            {
                if (BLogToUI())
                {
                    LogStringToUI(message);
                }
                else
                {
                    Debug.LogWarning(message, context);
                }
            }
        }

        public void LogError(string moduleName, string message)
        {
            if (BLog(moduleName))
            {
                if (BLogToUI())
                {
                    LogStringToUI(message);
                }
                else
                {
                    Debug.LogError(message);
                }
            }
        }

        public void LogError(string moduleName, string message, Object context)
        {
            if (BLog(moduleName))
            {
                if (BLogToUI())
                {
                    LogStringToUI(message);
                }
                else
                {
                    Debug.LogError(message, context);
                }
            }
        }

        public virtual void LogStringToUI(string message)
        {
            //Todo:ui log
            if(null == RichLog_UILog.Instance)
            {
                RichLog_UILog.GetRichLog_UILog();
            }
            Debug.Log(message);
        }
    }

    protected sealed class DebugLogHander : LogHandler
    {
        public override bool BLog(string moduleName)
        {
#if (UNITY_EDITOR || UNITY_DEBUG) && !NO_LOG
            LogFormat formatter = null;
            if (LogConfig.GetModuleFormat(moduleName, out formatter))
            {
                return formatter._OpenModule;
            }
            return LogConfig._AllowUnAssignedModule;
#else
            return false;
#endif
        }
    }

    protected sealed class ReleaseLogHandler : LogHandler
    {
        public override bool BLog(string moduleName)
        {
#if !NO_LOG
            LogFormat formatter = null;
            if (LogConfig.GetModuleFormat(moduleName, out formatter))
            {
                return formatter._OpenModule;
            }
            return LogConfig._AllowUnAssignedModule;
#else
            return false;
#endif
        }
        public override string FormatLogMessage(string moduleName, string message)
        {
            return BLog(moduleName) ? HeaderTag.ReleaseWarning(HeaderTag.Module(moduleName, message)) : null;
        }

        public override string FormatLogMessage(string moduleName, string subModuleName, string message)
        {
            return BLog(moduleName) ? HeaderTag.ReleaseWarning(HeaderTag.SubModule(moduleName, subModuleName, message)) : null;
        }
    }

    public interface IRichLogger
    {
        void Log(string moduleName, string message);
        void Log(string moduleName, string subModuleName, string message);
        void Log(string moduleName, string message, Object context);
        void Log(string domuleName, string subModuleName, string message, Object context);
        void LogWarning(string moduleName, string message);
        void LogWarning(string moduleName, string subModuleName, string message);
        void LogWarning(string moduleName, string message, Object context);
        void LogWarning(string domuleName, string subModuleName, string message, Object context);
        void LogError(string moduleName, string message);
        void LogError(string moduleName, string subModuleName, string message);
        void LogError(string moduleName, string message, Object context);
        void LogError(string moduleName, string subModuleName, string message, Object context);
    }

    protected sealed class RichLogger : IRichLogger
    {
        LogHandler _LogHandler;
        public RichLogger(LogHandler logHandler)
        {
            _LogHandler = logHandler;
        }

        public void Log(string moduleName, string message)
        {
            _LogHandler.Log(moduleName, _LogHandler.FormatLogMessage(moduleName, message));
        }
        public void Log(string moduleName, string subModuleName, string message)
        {
            _LogHandler.Log(moduleName, _LogHandler.FormatLogMessage(moduleName, subModuleName, message));
        }
        public void Log(string moduleName, string message, Object context)
        {
            _LogHandler.Log(moduleName, _LogHandler.FormatLogMessage(moduleName, message), context);
        }
        public void Log(string moduleName, string subModuleName, string message, Object context)
        {
            _LogHandler.Log(moduleName, _LogHandler.FormatLogMessage(moduleName, subModuleName, message), context);
        }
        public void LogWarning(string moduleName, string message)
        {
            _LogHandler.LogWarning(moduleName, _LogHandler.FormatLogMessage(moduleName, message));
        }
        public void LogWarning(string moduleName, string subModuleName, string message)
        {
            _LogHandler.LogWarning(moduleName, _LogHandler.FormatLogMessage(moduleName, subModuleName, message));
        }
        public void LogWarning(string moduleName, string message, Object context)
        {
            _LogHandler.LogWarning(moduleName, _LogHandler.FormatLogMessage(moduleName, message), context);
        }
        public void LogWarning(string moduleName, string subModuleName, string message, Object context)
        {
            _LogHandler.LogWarning(moduleName, _LogHandler.FormatLogMessage(moduleName, subModuleName, message), context);
        }
        public void LogError(string moduleName, string message)
        {
            _LogHandler.LogError(moduleName, _LogHandler.FormatLogMessage(moduleName, message));
        }
        public void LogError(string moduleName, string subModuleName, string message)
        {
            _LogHandler.LogError(moduleName, _LogHandler.FormatLogMessage(moduleName, subModuleName, message));
        }
        public void LogError(string moduleName, string message, Object context)
        {
            _LogHandler.LogError(moduleName, _LogHandler.FormatLogMessage(moduleName, message), context);
        }
        public void LogError(string moduleName, string subModuleName, string message, Object context)
        {
            _LogHandler.LogError(moduleName, _LogHandler.FormatLogMessage(moduleName, subModuleName, message), context);
        }
    }

    public static IRichLogger RL_Debug = new RichLogger(new DebugLogHander());

    public static IRichLogger RL_Release = new RichLogger(new ReleaseLogHandler());
}
