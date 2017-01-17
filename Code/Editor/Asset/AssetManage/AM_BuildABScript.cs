using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AM_BuildABScript {
    [MenuItem("工具/资源/测试/AssetBundle/直接生成AssetBundle")]
    static void BuildAB()
    {
        BuildAssetBundles(false);
    }

    const string kAssetBundlesOutputPath = "AssetBundles";

    static string GetOutPutPath()
    {
        string outputPath = Path.Combine(AM_Config.Asset_Build_Path,
                                         AM_PathHelper_Editor.GetAssetBundlePlatFormFolder());
        Debug.Log(outputPath);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        return outputPath;
    }

    public static void CopyDirectory(string srcDir, string tgtDir, List<string> splitFileList = null, bool log4track = false)
    {
        if (string.IsNullOrEmpty(srcDir) || string.IsNullOrEmpty(tgtDir))
        {
            return;
        }
        DirectoryInfo source = new DirectoryInfo(srcDir);
        DirectoryInfo target = new DirectoryInfo(tgtDir);

        if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
        {
            EditorLogTool.LogError("父目录不能拷贝到子目录！", log4track);
            throw new Exception("父目录不能拷贝到子目录！");
        }

        if (!source.Exists)
        {
            EditorLogTool.LogError("The given path does not exits : " + srcDir, log4track);
            return;
        }

        if (!target.Exists)
        {
            EditorLogTool.Log("【创建目录】" , tgtDir, log4track);
            target.Create();
        }

        FileInfo[] files = source.GetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].FullName.Contains(".manifest") || files[i].FullName.Contains("genhash.exe") || files[i].FullName.Contains(".meta"))
            {
                continue;
            }
            if (null != splitFileList && splitFileList.Count > 0)
            {
                bool split = false;
                for (int splitIndex = 0; splitIndex < splitFileList.Count; splitIndex++)
                {
                    if (files[i].FullName.Contains(splitFileList[splitIndex]))
                    {
                        split = true;
                        break;
                    }
                }
                if (split)
                {
                    continue;
                }
            }
            EditorUtility.DisplayProgressBar("拷贝文件", files[i].Name, (float)i / files.Length);
            File.Copy(files[i].FullName, target.FullName + @"\" + files[i].Name, true);
        }
        EditorUtility.ClearProgressBar();

        EditorLogTool.Log("【拷贝文件】", srcDir + "      ---->    " + tgtDir, log4track);

        DirectoryInfo[] dirs = source.GetDirectories();

        for (int j = 0; j < dirs.Length; j++)
        {
            CopyDirectory(dirs[j].FullName, target.FullName + @"\" + dirs[j].Name, splitFileList, log4track);
        }
    }

    public static void DeleteManifestFiles()
    {
        string basePath = System.Environment.CurrentDirectory;
        DeleteManifestFiles(System.IO.Path.Combine(basePath, GetOutPutPath()));
    }

    public static void ClearBuildPathOldBundles()
    {
        string outputPath = GetOutPutPath();
        RemovePath(outputPath, true);
    }

    public static void ClearPublicPathOldBundles()
    {
        string publicPath = System.IO.Path.Combine(AM_Config.Asset_Public_Path, AM_PathHelper_Editor.GetAssetBundlePlatFormFolder());
        RemovePath(publicPath, true);
    }

    public static void ClearStreamingAssetsBundles(bool log4track = false)
    {
        string basePath = System.IO.Path.Combine(AM_Config.Asset_Folder, AM_Config.Asset_Streaming_Folder);
        basePath = System.IO.Path.Combine(basePath, AM_Config.Asset_Res_Folder);
        string bundlePath = System.IO.Path.Combine(basePath, AM_PathHelper_Editor.GetAssetBundlePlatFormFolder());

        if (!string.IsNullOrEmpty(bundlePath))
        {
            if (Directory.Exists(bundlePath))
            {
                AssetDatabase.DeleteAsset(bundlePath);
                Debug.Log("Clear path [" + bundlePath + "] done !");
                if (log4track)
                {
                    Log4Track.Log("Clear path [" + bundlePath + "] done !");
                }
            }
            else
            {
                Debug.LogError("Path [" + bundlePath + "] does not exist !!");
                if (log4track)
                {
                    Log4Track.Log("Path [" + bundlePath + "] does not exist !!");
                }
            }
        }
        else
        {
            Debug.LogError("Path can not be null or empty !!");
            if (log4track)
            {
                Log4Track.Log("Path can not be null or empty !!");
            }
        }
    }

    public static void RemovePath(string path, bool log4track = false)
    {
        if (!string.IsNullOrEmpty(path))
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Debug.Log("Clear path [" + path + "] done !");
                if (log4track)
                {
                    Log4Track.Log("Clear path [" + path + "] done !");
                }
            }
            else
            {
                Debug.LogError("Path [" + path + "] does not exist !!");
                if (log4track)
                {
                    Log4Track.Log("Path [" + path + "] does not exist !!");
                }
            }
        }
        else
        {
            Debug.LogError("Path can not be null or empty !!");
            if (log4track)
            {
                Log4Track.Log("Path can not be null or empty !!");
            }
        }
    }

    public static void ClearPath(string path, bool log4track = false)
    {
        if (!string.IsNullOrEmpty(path))
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                if (null != files)
                {
                    for (int index = 0; index < files.Length; index++)
                    {
                        System.IO.File.Delete(files[index]);
                    }
                }
                string[] directries = Directory.GetDirectories(path);
                if (null != directries)
                {
                    for (int index = 0; index < directries.Length; index++)
                    {
                        Directory.Delete(directries[index], true);
                    }
                }
                Debug.Log("Clear path [" + path + "] done !");
                if (log4track)
                {
                    Log4Track.Log("Clear path [" + path + "] done !");
                }
            }
            else
            {
                Debug.LogError("Path [" + path + "] does not exist !!");
                if (log4track)
                {
                    Log4Track.Log("Path [" + path + "] does not exist !!");
                }
            }
        }
        else
        {
            Debug.LogError("Path can not be null or empty !!");
            if (log4track)
            {
                Log4Track.Log("Path can not be null or empty !!");
            }
        }
    }

    [MenuItem("工具/资源/测试/AssetBundle/将AB从生成目录拷贝到发布目录")]
    public static void CopyBuildResToPublicFolder()
    {
        CopyDirectory(AM_Config.Asset_Build_Path, AM_Config.Asset_Public_Path);
    }

    public static void BuildAssetBundles(bool log4track = false)
    {
        EditorLogTool.BeginLog4TrackBlock("生成资源AssetBundle", log4track);
        string outputPath = GetOutPutPath();
        Caching.CleanCache();
        BuildPipeline.BuildAssetBundles(outputPath, 0, EditorUserBuildSettings.activeBuildTarget);
        EditorLogTool.Log("生成资源AssetBundle结束,输出路径为  " + outputPath, log4track);
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    static void DeleteManifestFiles(string path)
    {
        Debug.Log(path);
        if (System.IO.Directory.Exists(path))
        {
            string[] files = System.IO.Directory.GetFiles(path);
            for (int index = 0; index < files.Length; index++)
            {
                if (files[index].LastIndexOf(".manifest") > 0)
                {
                    System.IO.File.Delete(files[index]);
                    Debug.Log("delete file: " + files[index]);
                }
            }

            string[] directories = System.IO.Directory.GetDirectories(path);
            for (int index = 0; index < directories.Length; index++)
            {
                DeleteManifestFiles(directories[index]);
            }
        }
        else
        {
            Debug.Log("directory not exist : " + path);
        }
    }

    [MenuItem("工具/资源/测试/AssetBundle/去除所有的AB命名")]
    static void RemoveAllABName()
    {
        Caching.CleanCache();
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        for (int index = 0; index < allAssets.Length; ++index )
        {
            EditorUtility.DisplayProgressBar("去除资源bundle名字", allAssets[index], (float)index / allAssets.Length);
            AssetImporter ai = AssetImporter.GetAtPath(allAssets[index]);
            if(!string.IsNullOrEmpty(ai.assetBundleName))
            {
                Debug.LogError(ai.assetBundleName);
                ai.SetAssetBundleNameAndVariant("", "");
                EditorUtility.SetDirty(ai);
                AssetDatabase.SaveAssets();
                Debug.Log("【资源】去除资源bundle名字" + ai.assetPath);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.RemoveUnusedAssetBundleNames();
        EditorUtility.ClearProgressBar();
    }
}
