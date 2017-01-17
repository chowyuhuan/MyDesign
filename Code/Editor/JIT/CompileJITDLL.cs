using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Diagnostics;

public class CompileJITDLL : MonoBehaviour
{
    static string[][] _scriptsToMove = null;

    [MenuItem("工具/脚本/热更新/生成DLL", false, 100)]
    public static void CompileForApp()
    {
        TryToCompileScripts("..\\..\\..\\StreamingAssets\\jit");
    }

    [MenuItem("工具/脚本/热更新/生成更新DLL", false, 100)]
    public static void CompileForABUpdate()
    {
        TryToCompileScripts("..\\..\\..\\..\\AssetBundles\\jit");
    }

    /// <summary>
    /// 剥离热更新脚本（打首包时，一定要执行此接口）
    /// </summary>
    [MenuItem("工具/脚本/热更新/剥离JIT脚本", false, 100)]
    public static void StripJITScripts()
    {
        MoveScripts(true);
    }

    /// <summary>
    /// 恢复热更新脚本
    /// </summary>
    [MenuItem("工具/脚本/热更新/恢复JIT脚本", false, 100)]
    public static void RecoverJITScripts()
    {
        MoveScripts(false);
    }

    static void Notify(string msg, bool error)
    {
        if (error)
        {
            UnityEngine.Debug.LogError(LogTag.JIT + msg);
        }
        else
        {
            UnityEngine.Debug.Log(LogTag.JIT + msg);
        }
    }

    public static bool TryToCompileScripts(string targetPath)
    {
        string tool = System.IO.Path.Combine(Application.dataPath, "Scripts/Editor/JIT/Compile.bat");

        Notify("正在编译...", false);
        Process p = new Process();
        p.StartInfo.FileName = tool;                //确定程序名
        p.StartInfo.Arguments = "\"" + EditorApplication.applicationContentsPath + "\" \"" + PlayerSettings.GetScriptingDefineSymbolsForGroup(GetBuildTargetGroup()) + "\" " + targetPath;
        p.StartInfo.UseShellExecute = false;        //Shell的使用
        p.StartInfo.RedirectStandardInput = false;  //重定向输入
        p.StartInfo.RedirectStandardOutput = false; //重定向输出
        p.StartInfo.RedirectStandardError = false;  //重定向输出错误
        p.StartInfo.CreateNoWindow = true;          //设置置不显示示窗口
        p.Start();
        p.WaitForExit();
        if (p.ExitCode == 0)
        {
            Notify("生成DLL成功", false);
            AssetDatabase.Refresh();
            return true;
        }
        else
        {
            Notify("生成DLL失败，请查看编译日志(Logs/CompileDLL)", true);
            return false;
        }
    }

    static BuildTargetGroup GetBuildTargetGroup()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    return BuildTargetGroup.Android;
                }
            case BuildTarget.iOS:
                {
                    return BuildTargetGroup.iOS;
                }
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                {
                    return BuildTargetGroup.Standalone;
                }
        }
        return BuildTargetGroup.Unknown;
    }

    static bool ExistScriptsToMove()
    {
        if (_scriptsToMove == null)
        {
            _scriptsToMove = new string[3][];
            _scriptsToMove[0] = new string[] { "Assets/Scripts/JITDLL", "Assets/WebplayerTemplates/JITDLL" };
            _scriptsToMove[1] = new string[] { "Assets/Scripts/Editor/Skill", "Assets/WebplayerTemplates/Editor/Skill" };
            _scriptsToMove[2] = new string[] { "Assets/Scripts/Editor/Asset/CreateActorPrefab.cs", "Assets/WebplayerTemplates/Editor/Asset/CreateActorPrefab.cs" };
        }
        return _scriptsToMove != null;
    }

    static string MoveScripts(bool forward)
    {
        int src = forward ? 0 : 1;
        int dis = forward ? 1 : 0;
        string result = string.Empty;
        if (ExistScriptsToMove())
        {
            for (int i = 0; i < _scriptsToMove.Length; ++i)
            {
                result = AssetDatabase.ValidateMoveAsset(_scriptsToMove[i][src], _scriptsToMove[i][dis]);
                if (!string.IsNullOrEmpty(result))
                {
                    Notify("移动脚本失败（F:" + _scriptsToMove[i][src] + " T:" + _scriptsToMove[i][dis] + " R:" + result + ")", true);
                    return result;
                }
                else
                {
                    Notify("移动脚本成功（F:" + _scriptsToMove[i][src] + " T:" + _scriptsToMove[i][dis] + ")", false);
                }
            }

            for (int i = 0; i < _scriptsToMove.Length; ++i)
            {
                result = AssetDatabase.MoveAsset(_scriptsToMove[i][src], _scriptsToMove[i][dis]);
                if (!string.IsNullOrEmpty(result))
                {
                    Notify("移动脚本失败（F:" + _scriptsToMove[i][src] + " T:" + _scriptsToMove[i][dis] + " R:" + result + ")", true);
                    return result;
                }
                else
                {
                    Notify("移动脚本成功（F:" + _scriptsToMove[i][src] + " T:" + _scriptsToMove[i][dis] + ")", false);
                }
            }
        }
        return result;
    }
}
