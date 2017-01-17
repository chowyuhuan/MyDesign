using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AM_AutoABBuilder {
    static Dictionary<string, AM_UITexPackInfo> _AppChangeAssetDic = new Dictionary<string, AM_UITexPackInfo>();
    static Dictionary<string, int> _AssetChangeList;
    static List<string> _AllRelateChangeAssetList;
    static AM_AssetHashDataBase _AssetHashDataBase;
    static Dictionary<string, string> _BuildArgsDic;
    static string _TargetVersion;
    static bool _CompileAndStripScript;

    class ABBuildParam
    {
        public class Remote
        {
            public const string TargetVersion = "TargetVersion";//目标显示版本号
            public const string CompileAndSplitScripts = "CompileAndSplitScripts";
        }
    }

    [MenuItem("工具/资源/测试/AssetBundle/自动检查资源变更并生成AssetBundle")]
    public static void TestAutoBuildAssetBundle()
    {
        BuildAssetBundle(false, true);
    }

    public static void AutoBuildAssetBundle()
    {
        BuildAssetBundle(true, true);
    }

    static void BuildAssetBundle(bool quietly, bool log4track)
    {
        _AppChangeAssetDic.Clear();
        Log4Track.InitLog4Track("AutoBuildAssetBundle", "AutoBuildAssetBundle");
        _BuildArgsDic = AM_EditorTool.ParseCmdLineParams(System.Environment.GetCommandLineArgs(), log4track);
        _CompileAndStripScript = CheckScriptCompileAndStrip(log4track);
        if(_CompileAndStripScript)
        {
            CompileJITDLL.CompileForABUpdate();
            CompileJITDLL.StripJITScripts();
        }
        try
        {
            string[] allPath = AssetDatabase.GetAllAssetPaths();
            _AssetHashDataBase = AM_AssetHashDataBase.GetAssetHashDataBase(true);
            List<string> deletedAssetList = _AssetHashDataBase.GetDeletedAsset(allPath, quietly, log4track);

            Dictionary<string, int> assetAddList;
            _AssetHashDataBase.DumpChangedAssetList(allPath, quietly, log4track, out _AssetChangeList, out assetAddList);

            foreach(string addAsset in assetAddList.Keys)
            {
                _AssetChangeList.Add(addAsset, 0);
            }
            DumpAppChangeAssetList(_AssetChangeList, quietly, log4track);
            AutoNameAssetABName(quietly, log4track);
            BuildAssetBundles(log4track, !quietly, Packer.Execution.Normal);
            CopyBuildResToPublicPath(log4track);
            string targetVersion = GetBundleVersion(log4track);
            AM_HashFileToVersion.GenVer(AM_Config.Asset_Public_Path, targetVersion, log4track);

            UpdateAssetHashDataBase(quietly, log4track);
        }
        catch(System.Exception e)
        {
            EditorLogTool.LogError(e.Message, log4track);
            EditorLogTool.LogError(e.StackTrace, log4track);
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            Log4Track.Close();
            if (_CompileAndStripScript)
            {
                CompileJITDLL.RecoverJITScripts();
            }
        }
    }

    static string GetBundleVersion(bool log4track)
    {
        string version;
        if (_BuildArgsDic.TryGetValue(ABBuildParam.Remote.TargetVersion, out version) && !string.IsNullOrEmpty(version))//没有配置版本
        {
            PlayerSettings.bundleVersion = version;
        }
        else
        {
            AM_HashFileToVersion.CheckCurrentABVersion(AM_Config.Asset_Public_Path, log4track);
            version = null;
        }
        return version;
    }

    static bool CheckScriptCompileAndStrip(bool log4track)
    {
        string compileAndSplit;
        bool cas = false;
        if (_BuildArgsDic.TryGetValue(ABBuildParam.Remote.CompileAndSplitScripts, out compileAndSplit))
        {
            if (bool.TryParse(compileAndSplit.ToLower(), out cas))
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

    static void BuildAssetBundles(bool log4track, bool displayProgressBar, Packer.Execution execution)
    {
        Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, displayProgressBar, execution);
        AM_BuildABScript.BuildAssetBundles(log4track);
    }

    static void UpdateAssetHashDataBase(bool quietly, bool log4track)
    {
        AM_AssetVersionHelper.UpdateAssetHashDataBase(_AssetHashDataBase, _AssetChangeList, quietly, log4track);
    }

    static void CopyBuildResToPublicPath(bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("将AssetBundle拷贝到发布目录", log4track);
        AM_BuildABScript.CopyDirectory(AM_Config.Asset_Build_Base, AM_Config.Asset_Public_Path, null, log4track);
        EditorLogTool.EndLog4TrackBlock(log4track);
    }


    static void AutoNameAssetABName(bool quitly, bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("为资源命名AssetBundleName", log4track);
        if (_AppChangeAssetDic.Count > 0)
        {
            int gcCount = 0;
            int gcLimit = 100;
            AM_AssetToABPathMapper atp = new AM_AssetToABPathMapper();
            atp.OpenNameMapWithWrite();
            int changeCount = 0;
            foreach(string assetPath in _AppChangeAssetDic.Keys)
            {
                ++changeCount;
                if (!quitly)
                {
                    EditorUtility.DisplayProgressBar("资源AssetBundle命名", assetPath, (float)changeCount / (_AppChangeAssetDic.Count));
                }
                string configABpath;
                AssetImporter ai;
                AM_UITexPackInfo uitpi = _AppChangeAssetDic[assetPath];
                if(null == uitpi)
                {

                    GetConfigPathAndABName(assetPath, out configABpath);                       
                    if(NameABToAsset(assetPath, configABpath, out ai, log4track))
                    {
                        EditorUtility.SetDirty(ai);
                        atp.AddNameMap(assetPath, configABpath);
                    }
                }
                else
                {
                    if (NameABToAsset(uitpi.AssetPath, uitpi.GetABName(), out ai, log4track))
                    {
                        EditorUtility.SetDirty(ai);
                        configABpath = ai.assetBundleName;
                        atp.AddNameMap(uitpi.AssetPath, configABpath);
                    }
                }

                AssetDatabase.SaveAssets();
                if (gcCount++ > gcLimit)
                {
                    EditorUtility.UnloadUnusedAssetsImmediate();
                    gcCount = 0;
                }
            }

            atp.SaveNameMap();
            AssetImporter atpai;
            NameABToAsset(atp.GetAssetPath(), atp.GetMapperPath().ToLower(), out atpai, log4track);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
        else
        {
            EditorLogTool.Log("没有文件变更!!", log4track);
        }
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    static bool NameABToAsset(string assetPath, string abName, out AssetImporter ai, bool log4track)
    {
        if(!string.IsNullOrEmpty(assetPath) && !string.IsNullOrEmpty(abName))
        {
            ai = AssetImporter.GetAtPath(assetPath);
            if(null != ai)
            {                
                abName = abName.Replace(" ", "");
                ai.assetBundleName = abName;
                EditorLogTool.Log("【资源AB命名】", "【资源】 " + assetPath + "【ABName】" + abName, log4track);
            }
            else
            {
                EditorLogTool.LogError("【资源AB命名】",assetPath + " 不存在 !!!", log4track);
            }
        }
        else
        {
            ai = null;
            EditorLogTool.Log("【资源AB命名】", "【资源】" + assetPath + "【ABName】:" + abName, log4track);
        }
        return null != ai;
    }

    static void GetConfigPathAndABName(string sourcePath, out string configABName)
    {
        string dir;
        string name;
        GUI_Atlas atlas;
        if(AM_EditorTool.IsAtlas(sourcePath, out atlas)
            && atlas._SpriteList.Count > 0)
        {
            Sprite sp = atlas._SpriteList[0];
            string ap = AssetDatabase.GetAssetPath(sp);
            TextureImporter ti = TextureImporter.GetAtPath(ap) as TextureImporter;
            AM_UITexPackInfo uitpi = new AM_UITexPackInfo(ap, ti.spritePackingTag, ti.spriteImportMode);
            configABName = uitpi.GetABName();
        }
        else
        {
            dir = System.IO.Path.GetDirectoryName(sourcePath);
            name = System.IO.Path.GetFileNameWithoutExtension(sourcePath);
            string abName = dir + "/" + name;
            configABName = abName.ToLower();
        }
    }

    static void DumpAppChangeAssetList(Dictionary<string,int> assetChangeList, bool quietly, bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("获取应用变更资源", log4track);
        AM_UITexPackAtlasInfo texPackInfo = new AM_UITexPackAtlasInfo(quietly);
        AM_PathConfig appUsePathConfig = AssetDatabase.LoadAssetAtPath<AM_PathConfig>("Assets/Scripts/Editor/Asset/AssetManage/AppUsePathConfig.asset");
        AM_AppRefGuard apprefguard = new AM_AppRefGuard(quietly, appUsePathConfig, texPackInfo);
        AM_UIPrefabExporter uiexporter = new AM_UIPrefabExporter(apprefguard, texPackInfo);
        uiexporter.ExportUIPrefab(quietly);
        AssetDatabase.Refresh();
        AM_PathConfig appExportUsePathConfig = AssetDatabase.LoadAssetAtPath<AM_PathConfig>("Assets/Scripts/Editor/Asset/AssetManage/AppExportUsePathConfig.asset");
        AM_AppRefGuard appExportRefGuard = new AM_AppRefGuard(quietly, appExportUsePathConfig, texPackInfo);
        AM_AssetChangeDumper acd = new AM_AssetChangeDumper(appExportRefGuard);
        _AllRelateChangeAssetList = acd.DumpChangeAssetList(quietly, assetChangeList);
        for (int index = 0; index < _AllRelateChangeAssetList.Count; ++index)
        {
            if(!quietly)
            {
                EditorUtility.DisplayProgressBar("获取应用变更资源", _AllRelateChangeAssetList[index], (float)index / _AllRelateChangeAssetList.Count);
            }
            AM_UITexPackInfo uitpi;
            bool uiTex;
            if (apprefguard.IsMultipleDepen(_AllRelateChangeAssetList[index], out uiTex, out uitpi))
            {
                AddToChangeList(_AllRelateChangeAssetList[index], uitpi, log4track);
            }
            else if (apprefguard.IsRootAsset(_AllRelateChangeAssetList[index]))
            {
                AddToChangeList(_AllRelateChangeAssetList[index], uitpi, log4track);
            }
        }
        EditorUtility.ClearProgressBar();
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    static void AddToChangeList(string assetPath, AM_UITexPackInfo uitpi, bool log4track)
    {
        if(!_AppChangeAssetDic.ContainsKey(assetPath))
        {
            _AppChangeAssetDic.Add(assetPath, uitpi);
            EditorLogTool.Log("【添加变更资源】", assetPath, log4track);
        }
        else
        {
            EditorLogTool.LogError("【重复添加变更资源】", assetPath, log4track);
        }
    }
}
