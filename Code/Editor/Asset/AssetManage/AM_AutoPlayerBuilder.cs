using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using System;
using System.Collections;
using System.Collections.Generic;

public class AM_AutoPlayerBuilder
{

    public static class AndroidSDKFolder
    {
        public static string Path
        {
            get { return EditorPrefs.GetString("AndroidSdkRoot"); }
            set { EditorPrefs.SetString("AndroidSdkRoot", value); }
        }
    }

    static class PlayerBuildParam
    {
        public class Remote
        {
            public const string ReleaseBuild = "ReleaseBuild";//是否是发布版本
            public const string AndroidSDKPath = "AndroidSDKPath";
            public const string Log4Track = "Log4Track";//开启自定义生成Log
            public const string TargetVersion = "TargetVersion";//目标显示版本号
            public const string BundleVersionCode = "BundleVersionCode";//发布程序迭代版本号
            public const string BuildName = "BuildName";//要创建的文件名，会在程序处理之后添加后缀
            public const string ScriptDefineSymbol = "ScriptDefineSymbol";//预处理命令
            public const string BuildScenes = "BuildScenes";//要打包的场景，没有配置时使用编辑器配置文件中的场景
            public const string DevelopmentBuild = "DevelopmentBuild";
            public const string CompileAndSplitScripts = "CompileAndSplitScripts";
            public const string EditorLogFileLocation = "EditorLogFileLocation";//编辑器Log位置，相对于工程目录（这个位置是在Unity外部的命令行参数中指定的）
            public const string BuildSVNRevision = "BuildSVNRevision";//生成时的svn版本号
            public const string JenkinsBuildUrl = "JenkinsBuildUrl";//本次生成的Jenkins记录信息，可以通过此链接直接跳转到Jenkins的生成信息页面
        }

        public class Internal
        {
            public const string BuildAchievePath = "BuildAchieve";
            public const string ReleaseBuildPath = "Public";
            public const string TestBuildPath = "Test";
            public const string ReleaseBuildSuffix = "public";
            public const string TestBuildSuffix = "test";

            public const string LastBuildAchieveLocation = "BuildPlayer_Last_Build_Achieve_Location";
            public const string LastLog4TrackFileLocation = "BuildPlayer_Last_Log4Track_File_Location";
            public const string LastEditorLogFileLocation = "BuildPlayer_Last_Editor_Log_File_Location";
        }

        public class BuildOpetion
        {
            public const string Development = "Development";
        }
    }

    static Dictionary<string, string> _BuildArgsDic;
    static string _TargetVersion;
    static bool _ReleaseBuild;
    static bool _CompileAndStripScript;
    static string _BuildPath;
    static string _SVNRevision;

    [MenuItem("工具/资源/测试/自动打包/收集安装包生成Log")]
    public static void GatherLastBuildAPKAchieve()
    {
        bool log4track = true;//这个函数的log信息应该永远都记录到文件
        EditorLogTool.InitLog4Track("GatherLastBuildAPKAchieve", "GatherLastBuildAPKAchieve", log4track);
        string lastBuildAchieveFile = EditorPrefs.GetString(PlayerBuildParam.Internal.LastBuildAchieveLocation, null);
        if (string.IsNullOrEmpty(lastBuildAchieveFile))
        {
            EditorLogTool.LogError("【没有找到APK生成信息】", log4track);
        }
        else if (!System.IO.File.Exists(lastBuildAchieveFile))
        {
            EditorLogTool.LogError("【未找到生成的APK文件】", "【" + lastBuildAchieveFile + "】", log4track);
        }
        else
        {
            string targetFile;
            string gatherBasePath = System.IO.Path.GetDirectoryName(lastBuildAchieveFile);
            string lastLog4TrackFile = EditorPrefs.GetString(PlayerBuildParam.Internal.LastLog4TrackFileLocation, null);
            if (!System.IO.File.Exists(lastLog4TrackFile))
            {
                EditorLogTool.LogError("【未找到生成的Log4Track文件】", "【" + lastLog4TrackFile + "】", log4track);
            }
            else
            {
                targetFile = System.IO.Path.Combine(gatherBasePath, System.IO.Path.GetFileName(lastLog4TrackFile));
                System.IO.File.Move(lastLog4TrackFile, targetFile);
                EditorLogTool.Log("【拷贝Log文件】", "【" + lastLog4TrackFile + "---->" + targetFile + "】", log4track);
            }

            string lastEditorLogFile = EditorPrefs.GetString(PlayerBuildParam.Internal.LastEditorLogFileLocation, null);
            if (!System.IO.File.Exists(lastEditorLogFile))
            {
                EditorLogTool.LogError("【未找到生成的UnityEditorLog文件】", "【" + lastEditorLogFile + "】", log4track);
            }
            else
            {
                targetFile = System.IO.Path.Combine(gatherBasePath, System.IO.Path.GetFileName(lastEditorLogFile));
                System.IO.File.Move(lastEditorLogFile, targetFile);
                EditorLogTool.Log("【拷贝Log文件】", "【" + lastEditorLogFile + "---->" + targetFile + "】", log4track);
            }
        }
        EditorLogTool.EndLog4TrackBlock(log4track);
        EditorLogTool.CloseLog4Track(log4track);
    }

    [MenuItem("工具/资源/测试/自动打包/生成安装包")]
    public static void TestAutoBuildPlayer()
    {
        BuildPlayer(true, false);
    }

    public static void AutoBuildPlayer()
    {
        BuildPlayer(true, true);
    }
    static void BuildPlayer(bool log4track, bool quietly)
    {
        string log4trackfile = EditorLogTool.InitLog4Track("AutoBuildAPK", "AutoBuildAPK", log4track);
        EditorPrefs.SetString(PlayerBuildParam.Internal.LastLog4TrackFileLocation, log4trackfile);
        _BuildArgsDic = AM_EditorTool.ParseCmdLineParams(System.Environment.GetCommandLineArgs(), log4track);
        if (_BuildArgsDic.Count > 0)
        {
            LogRemoteParams(log4track);
        }


        EditorLogTool.BeginLog4TrackBlock("【生成信息】", log4track);
        _SVNRevision = GetSVNRevision(log4track);
        _ReleaseBuild = GetBuildIntent(log4track);
        _BuildPath = GetBuildPath(_ReleaseBuild, log4track);
        _TargetVersion = GetBundleVersion(log4track);
        string targetPath = GetBuildAchieveFullPath(log4track);
        _CompileAndStripScript = CheckScriptCompileAndStrip(log4track);
        CheckBundleVersionCode(log4track);
        CheckScriptingDefineSymbolsForGroup(log4track);
        EditorLogTool.EndLog4TrackBlock(log4track);

        EditorLogTool.BeginLog4TrackBlock("【记录输出信息】", log4track);
        RecordLog4TrackLocation(log4trackfile, log4track);
        RecordEditorLogLocation(log4track);
        RecordBuildAchieveLocation(targetPath, log4track);
        EditorLogTool.EndLog4TrackBlock(log4track);

        if(_CompileAndStripScript)
        {
            CompileJITDLL.CompileForApp();
            CompileJITDLL.StripJITScripts();
        }

        AM_HashFileToVersion.GenAppVersion(_TargetVersion, log4track);

        try
        {
            AM_AssetToABPathMapper.GenerateEmtpyMapper(log4track);
            AM_ForceLoadFromAppEditor.GenerateEmtpyMapper(log4track);
            CheckEnvireonmentVariables(log4track);
            ExportUIDependence(quietly, log4track);
            Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, !quietly, Packer.Execution.Normal);
            string[] levels = GetBuildScenes(log4track);
            BuildOptions buildOps = GetBuildOpetions(log4track);
            BuildPlayer(levels, targetPath, buildOps, log4track);
        }
        catch (System.Exception e)
        {
            EditorLogTool.LogError("【生成失败】", targetPath, log4track);
            EditorLogTool.LogError(e.Message, log4track);
            EditorLogTool.LogError(e.StackTrace, log4track);
        }
        finally
        {
            EditorLogTool.CloseLog4Track(log4track);
            if(_CompileAndStripScript)
            {
                CompileJITDLL.RecoverJITScripts();
            }
        }
    }
    static void RecordLog4TrackLocation(string logfile, bool log4track)
    {
        EditorPrefs.SetString(PlayerBuildParam.Internal.LastLog4TrackFileLocation, logfile);
        EditorLogTool.Log("【记录跟踪Log文件位置】", "【" + logfile + "】", log4track);
    }

    static void RecordBuildAchieveLocation(string buildAchieve, bool log4track)
    {
        EditorPrefs.SetString(PlayerBuildParam.Internal.LastBuildAchieveLocation, buildAchieve);
        EditorLogTool.Log("【记录生成文件位置】", "【" + buildAchieve + "】", log4track);
    }

    static void RecordEditorLogLocation(bool log4track)
    {
        string editorLogLocation = null;
        if (_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.EditorLogFileLocation, out editorLogLocation))
        {
            Debug.LogError(editorLogLocation);
            string logLocation = System.IO.Path.Combine(System.Environment.CurrentDirectory, editorLogLocation).Replace("\\", "/");
            EditorPrefs.SetString(PlayerBuildParam.Internal.LastEditorLogFileLocation, logLocation);
            EditorLogTool.Log("【记录Unity编辑器Log文件位置】", "【" + logLocation + "】", log4track);
        }
        else
        {
            EditorPrefs.SetString(PlayerBuildParam.Internal.LastEditorLogFileLocation, null);
            EditorLogTool.LogError("【没有配置Unity编辑器Log文件位置信息】", log4track);
        }
    }

    static bool CheckScriptCompileAndStrip(bool log4track)
    {
        string compileAndSplit;
        bool cas = false;
        if(_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.CompileAndSplitScripts, out compileAndSplit))
        {
            if(bool.TryParse(compileAndSplit.ToLower(), out cas))
            {
                EditorLogTool.Log("【编译并剥离脚本】" + compileAndSplit, log4track);
            }
            else
            {
                EditorLogTool.Log("【解析脚本编译信息错误，请检查Jenkins的构建命令】", log4track);
            }
        }
        else
        {
            EditorLogTool.Log("【没有配置脚本剥离信息】", log4track);
        }
        return cas;
    }

    static void LogRemoteParams(bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("【获得配置参数】", log4track);
        foreach (string param in _BuildArgsDic.Keys)
        {
            EditorLogTool.Log("【" + param + "】", "【" + _BuildArgsDic[param] + "】", log4track);
        }
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    static string GetSVNRevision(bool log4track)
    {
        string svnRevision;
        if (_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.BuildSVNRevision, out svnRevision)
            && !string.IsNullOrEmpty(svnRevision))
        {
            EditorLogTool.Log("【SVN Revision】", "【" + svnRevision + "】", log4track);
        }
        else
        {
            EditorLogTool.LogError("【没有 SVN Revision 信息，请检查Jenkins的构建命令】", log4track);
        }
        return svnRevision;
    }

    static void CheckScriptingDefineSymbolsForGroup(bool log4track)
    {
        string scriptSymbol;
        BuildTargetGroup btg = GetBuildTargetGroup();
        if (_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.ScriptDefineSymbol, out scriptSymbol)
            && !string.IsNullOrEmpty(scriptSymbol))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, scriptSymbol);
            EditorLogTool.Log("【使用配置预编译符号】", "【" + btg.ToString() + "】" + scriptSymbol, log4track);
        }
        else
        {
            EditorLogTool.Log("【使用编辑器预编译符号】", "【" + btg.ToString() + "】" + PlayerSettings.GetScriptingDefineSymbolsForGroup(btg), log4track);
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

    static int CheckBundleVersionCode(bool log4track)
    {
        int vc;
        string versionCode;
        if (_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.BundleVersionCode, out versionCode)
            && int.TryParse(versionCode, out vc))
        {
            SetBundleVersionCode(vc);
            EditorLogTool.Log("【使用配置BundleVersionCode】", versionCode, log4track);
        }
        else
        {
            vc = GetEditorBundleVersionCode();
            EditorLogTool.Log("【使用编辑器设定BundleVersionCode】", vc.ToString(), log4track);
        }
        return vc;
    }

    static int GetEditorBundleVersionCode()//编辑器中的设置值
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    return PlayerSettings.Android.bundleVersionCode;
                }
            case BuildTarget.iOS:
                {
                    return int.Parse(PlayerSettings.iOS.buildNumber);
                }
        }
        return 1;
    }

    static void SetBundleVersionCode(int versionCode)
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    PlayerSettings.Android.bundleVersionCode = versionCode;
                    break;
                }
            case BuildTarget.iOS:
                {
                    PlayerSettings.iOS.buildNumber = versionCode.ToString();
                    break;
                }
        }
    }

    static string GetBundleVersion(bool log4track)
    {
        string version;
        if (_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.TargetVersion, out version) && !string.IsNullOrEmpty(version))//没有配置版本
        {
            PlayerSettings.bundleVersion = version;
            EditorLogTool.Log("【使用配置版本号】", version, log4track);
        }
        else
        {
            version = PlayerSettings.bundleVersion;
            EditorLogTool.Log("【使用编辑器版本号】", PlayerSettings.bundleVersion, log4track);
        }
        return version;
    }

    static string GetBuildAchieveFullPath(bool log4track)
    {
        string achieveName = GetAchieveName(_ReleaseBuild, _TargetVersion, _SVNRevision, log4track);
        string timeStamp = AM_EditorTool.TimeStampStr("yyyy_MM_dd_HH_mm_ss");
        string timeStampPath = string.Format("{0}{1}{2}{3}{4}", _BuildPath, "/", System.IO.Path.GetFileNameWithoutExtension(achieveName), "_", timeStamp);
        System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(timeStampPath);
        if (!dirInfo.Exists)
        {
            EditorLogTool.Log("【创建输出路径】", timeStampPath, log4track);
            dirInfo.Create();
        }
        string buildAchieveFullPath = string.Format("{0}{1}{2}", timeStampPath, "/", achieveName);
        EditorLogTool.Log("【生成目标全路径】", buildAchieveFullPath, log4track);
        return buildAchieveFullPath;
    }

    static string GetBuildPath(bool releaseBuild, bool log4track)
    {
        string releasePath;
        if (releaseBuild)
        {
            releasePath = PlayerBuildParam.Internal.ReleaseBuildPath;
        }
        else
        {
            releasePath = PlayerBuildParam.Internal.TestBuildPath;
        }
        string buildPath = string.Format("{0}{1}{2}{3}{4}{5}{6}",
            System.Environment.CurrentDirectory.Replace("\\", "/"),
            "/",
            PlayerBuildParam.Internal.BuildAchievePath,
            "/",
            releasePath,
            "/",
            AM_PathHelper_Editor.GetAssetBundlePlatFormFolder()
            );
        EditorLogTool.Log("【目标文件输出路径】", buildPath, log4track);
        System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(buildPath);
        if (!dirInfo.Exists)
        {
            EditorLogTool.Log("【输出路径不存在】", buildPath, log4track);
            EditorLogTool.Log("【创建输出路径】", buildPath, log4track);
            dirInfo.Create();
        }
        return buildPath;
    }

    static string GetAchieveName(bool releaseBuild, string version, string svnRevision, bool log4track)
    {
        string configName;
        if (!_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.BuildName, out configName)
            || string.IsNullOrEmpty(configName))
        {
            configName = PlayerSettings.productName;
        }
        string releaseSuffix;
        if (releaseBuild)
        {
            releaseSuffix = PlayerBuildParam.Internal.ReleaseBuildSuffix;
        }
        else
        {
            releaseSuffix = PlayerBuildParam.Internal.TestBuildSuffix;
        }
        string buildAchieveName = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
            configName,
            "_",
            version,
            "_",
            releaseSuffix,
            "_svnrevision",
            svnRevision,
            AM_PathHelper_Editor.GetPlatformBuildAchieveSuffix()
            );
        EditorLogTool.Log("【目标文件名】", buildAchieveName, log4track);
        return buildAchieveName;
    }

    static bool GetBuildIntent(bool log4track)
    {
        string releaseBuildParam;
        if (_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.ReleaseBuild, out releaseBuildParam))
        {
            bool releaseBuild;
            if (bool.TryParse(releaseBuildParam, out releaseBuild) && releaseBuild)
            {
                EditorLogTool.Log("【生成用途】", "【正式发布版】", log4track);
                return releaseBuild;
            }
        }
        EditorLogTool.Log("【生成用途】", "【内部测试版】", log4track);
        return false;
    }

    static string[] GetBuildScenes(bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("配置场景", log4track);
        string[] levels;
        string buildScenes;
        if (GetRemoteConfigBuildScenes(out levels, out buildScenes))
        {
            EditorLogTool.Log("【使用配置场景】", buildScenes, log4track);
        }
        else if (GetLocalConfigBuildScenes(out levels, out buildScenes))
        {
            EditorLogTool.Log("【使用本地配置场景】", buildScenes, log4track);
        }
        else if (GetEditorBuildScenes(out levels, out buildScenes))
        {
            EditorLogTool.Log("【使用编辑器配置场景】", buildScenes, log4track);
        }

        if (null != levels && levels.Length > 0)
        {
            for (int index = 0; index < levels.Length; ++index)
            {
                EditorLogTool.Log("【场景】", levels[index], log4track);
            }
        }
        else
        {
            EditorLogTool.LogError("【没有配置打包的场景】", log4track);
        }
        EditorLogTool.EndLog4TrackBlock(log4track);
        return levels;
    }

    static bool GetEditorBuildScenes(out string[] levels, out string buildScenes)
    {
        if (EditorBuildSettings.scenes.Length > 0)
        {
            List<string> levelList = new List<string>();
            foreach (EditorBuildSettingsScene ebse in EditorBuildSettings.scenes)
            {
                if (ebse.enabled)
                {
                    levelList.Add(ebse.path);
                }
            }
            if (levelList.Count > 0)
            {
                levels = levelList.ToArray();
                buildScenes = String.Join(";", levels);
                return true;
            }
        }
        levels = null;
        buildScenes = null;
        return false;
    }

    static bool GetRemoteConfigBuildScenes(out string[] levels, out string buildScenes)
    {
        if (_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.BuildScenes, out buildScenes) && !string.IsNullOrEmpty(buildScenes))
        {
            char[] spliter = { ';' };
            levels = buildScenes.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
            return levels.Length > 0;
        }
        levels = null;
        return false;
    }

    static bool GetLocalConfigBuildScenes(out string[] levels, out string buildScenes)
    {
        AM_PathConfig scenePathConfig = AssetDatabase.LoadAssetAtPath<AM_PathConfig>("Assets/Scripts/Editor/Asset/AssetManage/ScenePathConfig.asset");
        levels = scenePathConfig.GetConfigPathList().ToArray();
        buildScenes = String.Join(";", levels);
        return levels.Length > 0;
    }

    static void CheckEnvireonmentVariables(bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("检查配置环境", log4track);
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    CheckAndroidSDKPath(log4track);
                    break;
                }
        }
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    static void CheckAndroidSDKPath(bool log4track)
    {
        EditorLogTool.Log("【当前AndroidSDK路径】", AndroidSDKFolder.Path, log4track);
        if (string.IsNullOrEmpty(AndroidSDKFolder.Path))
        {
            string androidSDKPath;
            if (!_BuildArgsDic.TryGetValue(PlayerBuildParam.Remote.AndroidSDKPath, out androidSDKPath))
            {
                androidSDKPath = "D:/Android/android-sdk";
            }
            EditorLogTool.Log("【设置AndroidSDK路径】", androidSDKPath, log4track);
            AndroidSDKFolder.Path = androidSDKPath;
        }
        EditorLogTool.Log("【使用AndroidSDK路径】", AndroidSDKFolder.Path, log4track);
    }

    static BuildOptions GetBuildOpetions(bool log4track)
    {
        BuildOptions buildOpetions = BuildOptions.None;
        BuildOptions configOp;
        if (GetBuildOpetion(PlayerBuildParam.BuildOpetion.Development, out configOp))
        {
            EditorLogTool.Log("【使用生成选项】", configOp.ToString(), log4track);
            buildOpetions |= configOp;
        }
        return buildOpetions;
    }

    static bool GetBuildOpetion(string opetionKey, out BuildOptions op)
    {
        bool useOp = false;
        string opetionString = null;
        if (!string.IsNullOrEmpty(opetionKey))
        {
            if (_BuildArgsDic.TryGetValue(opetionKey, out opetionString))
            {
                if (bool.TryParse(opetionString, out useOp)
                    && GetBuildOpetionFromString(opetionString, out op))
                {
                    return true;
                }
            }
        }
        op = BuildOptions.None;
        return false;
    }

    static bool GetBuildOpetionFromString(string opetionString, out BuildOptions op)
    {
        if (opetionString == "Development")
        {
            op = BuildOptions.Development;
            return true;
        }
        op = BuildOptions.None;
        return false;
    }

    static void ExportUIDependence(bool quietly, bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("导出UI图集依赖", log4track);
        AM_UITexPackAtlasInfo texPackInfo = new AM_UITexPackAtlasInfo(quietly);
        AM_PathConfig appUsePathConfig = AssetDatabase.LoadAssetAtPath<AM_PathConfig>("Assets/Scripts/Editor/Asset/AssetManage/AppUsePathConfig.asset");
        AM_AppRefGuard apprefguard = new AM_AppRefGuard(quietly, appUsePathConfig, texPackInfo);
        AM_UIPrefabExporter uiexporter = new AM_UIPrefabExporter(apprefguard, texPackInfo);
        uiexporter.ExportUIPrefab(quietly);
        AssetDatabase.Refresh();
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    static void BuildPlayer(string[] levels, string targetPath, BuildOptions op, bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("生成安装包", log4track);
        string result = BuildPipeline.BuildPlayer(levels, targetPath, EditorUserBuildSettings.activeBuildTarget, op);
        if (!string.IsNullOrEmpty(result) && result.Contains("errors"))
        {
            EditorLogTool.LogError("【生成失败】", result, log4track);
        }
        else
        {
            EditorLogTool.Log("【完成生成】", targetPath, log4track);
        }
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    static void BuildPlayer(EditorBuildSettingsScene[] levels, string targetPath, BuildOptions op, bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("生成安装包", log4track);
        string result = BuildPipeline.BuildPlayer(levels, targetPath, EditorUserBuildSettings.activeBuildTarget, op);
        if (!string.IsNullOrEmpty(result) && result.Contains("errors"))
        {
            EditorLogTool.LogError("【生成失败】", result, log4track);
        }
        else
        {
            EditorLogTool.Log("【完成生成】", targetPath, log4track);
        }
        EditorLogTool.EndLog4TrackBlock(log4track);
    }
}
