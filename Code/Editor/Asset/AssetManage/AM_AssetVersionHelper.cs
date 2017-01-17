using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AM_AssetVersionHelper  {
    
    [MenuItem("工具/资源/测试/资源Hash/获取当前的资源Hash")]
    public static void GetCurAssetHash()
    {
        Log4Track.InitLog4Track("AssetHashDataBase", "AssetHashDataBase");
        CreateAssetHashDataBase(false, true);
        Log4Track.Close();
    }
    static void CreateAssetHashDataBase(bool quietly, bool log4Track)
    {
        AM_AssetHashDataBase ahdb = AM_AssetHashDataBase.GetAssetHashDataBase(false);
        ahdb.Clear();

        string[] allpath = AssetDatabase.GetAllAssetPaths();
        for (int index = 0; index < allpath.Length; ++index)
        {
            if(!quietly)
            {
                EditorUtility.DisplayProgressBar("获取资源Hash128", allpath[index], (float)index / allpath.Length);
            }
            if (File.Exists(allpath[index])
                && !allpath[index].Contains(".cs")
                && allpath[index].Contains("Assets/"))
            {
                Hash128 hs = AssetDatabase.GetAssetDependencyHash(allpath[index]);
                ahdb.Add(allpath[index], hs);
                if(log4Track)
                {
                    Log4Track.Log("Hash " + hs, "AssetPath  " + allpath[index]);
                }
            }
        }
        ahdb.Save();
        EditorUtility.ClearProgressBar();

    }

    public static void UpdateAssetHashDataBase(AM_AssetHashDataBase ahdb, Dictionary<string, int> changeList, bool quietly, bool log4track)
    {
        if(null != ahdb)
        {
            string[] allpath = AssetDatabase.GetAllAssetPaths();
            for (int index = 0; index < allpath.Length; ++index)
            {
                AM_EditorTool.DisplayProgressBar(quietly, "检查资源Hash128", allpath[index], (float)index / allpath.Length);
                if (File.Exists(allpath[index])
                    && !allpath[index].Contains(".cs")
                    && allpath[index].Contains("Assets/")
                    && !changeList.ContainsKey(allpath[index])
                )
                {
                    Hash128 hash = AssetDatabase.GetAssetDependencyHash(allpath[index]);
                    string oldHash;
                    if (ahdb.GetAssetHash(allpath[index], out oldHash))
                    {
                        if (oldHash != hash.ToString())
                        {
                            ahdb.UpdateAssetHash(allpath[index], hash);
                            EditorLogTool.Log("【更新资源Hash】 【旧值】 " + oldHash + "【新值】" + hash.ToString() + "【资源】" + allpath[index], log4track);
                        }
                    }
                }
            }
            ahdb.Save();
            AM_EditorTool.ClearProgressBar(quietly);
        }
    }

    [MenuItem("工具/资源/测试/资源Hash/检查资源变更")]
    static void CheckAssetChange()
    {
        Log4Track.InitLog4Track("AssetHashCompare", "AssetHashCompare");
        DumpAssetChangeInfo(AssetDatabase.GetAllAssetPaths(), false, true);
        Log4Track.Close();
    }

    public static void DumpAssetChangeInfo(string[] allpath, bool quietly, bool log4track)
    {
        AM_AssetHashDataBase ahdb = AM_AssetHashDataBase.GetAssetHashDataBase(true);
        if(null == allpath)
        {
            allpath = AssetDatabase.GetAllAssetPaths();
        }
        ahdb.GetDeletedAsset(allpath, quietly, log4track);

        Dictionary<string, int> assetChangeList;
        Dictionary<string, int> assetAddList;
        ahdb.DumpChangedAssetList(allpath, quietly, log4track, out assetChangeList, out assetAddList);
    }
}
