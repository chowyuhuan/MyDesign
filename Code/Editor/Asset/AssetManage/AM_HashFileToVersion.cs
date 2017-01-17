using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AM_HashFileToVersion 
{
    const string _InitABVersion = "1.0.0";
    [MenuItem("工具/资源/测试/版本配置文件/检查安装包版本")]
    static void CheckAppVersion()
    {
        Log4Track.InitLog4Track("CheckVer", "Version/CheckVer");
        CheckVer_Imp(GetAppVersionPath(), true);
        Log4Track.Close();
    }

    [MenuItem("工具/资源/测试/版本配置文件/生成安装包版本")]
    static void GenAppVersionTest()
    {
        Log4Track.InitLog4Track("GenVer", "GenVer/GenAppVer");
        GenAppVersion(null, true);
        Log4Track.Close();
    }

    static string GetAppVersionPath()
    {
        return Application.streamingAssetsPath;
    }

    public static void CheckVer(string targetPath, bool log4track = false)
    {
        CheckVer_Imp(targetPath, log4track);
    }

    public static void GenAppVersion(string targetVersion, bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("安装包版本信息", log4track);
        CheckVer_Imp(GetAppVersionPath(), false);
        GenVer_Imp(GetAppVersionPath(), targetVersion, log4track);
        EditorLogTool.EndLog4TrackBlock(log4track);
    }

    public static string CheckCurrentABVersion(string targetPath, bool log4track)
    {
        string dic = targetPath;
        ver = AM_Verall.Read(dic);
        if (ver == null)
        {
            EditorLogTool.Log("【当前没有AssetBundle版本】", log4track);
            EditorLogTool.Log("【设定初次AssetBundle版本】", _InitABVersion, log4track);
            return _InitABVersion;
        }
        else
        {
            EditorLogTool.Log("【当前配置文件中的版本号】", ver.GetVerString(), log4track);
        }
        return ver.GetVerString();
    }

    public static void GenVer(string targetPath, string targetVersion = null, bool log4track = false)
    {
        Log4Track.BeginBlock("生成资源版本文件");
        CheckVer_Imp(targetPath, log4track);
        GenVer_Imp(targetPath, targetVersion, log4track);
        Log4Track.EndBlock();
    }

    static AM_Verall ver = null;
    static AM_Verall vernew = null;

    static void ResetData()
    {
        ver = null;
        vernew = null;
    }

    static void InitVerAndPath(string targetPath,bool log4track = false)
    {
        string dic = targetPath;
        ver = AM_Verall.Read(dic);
        if (ver == null)
        {
            ver = new AM_Verall();
            string[] groups = System.IO.Directory.GetDirectories(dic);
            foreach (var g in groups)
            {
                string path = g.Substring(g.IndexOf(dic)+dic.Length+1).ToLower();
                EditorLogTool.Log("分支被添加：" + path, log4track);
                ver.groups[path] = new AM_VerInfo(path);
            }
        }
    }

    static void CheckVer_Imp(string targetPath, bool log4track = false)
    {
        InitVerAndPath(targetPath);

        vernew = new AM_Verall();
        int delcount = 0;
        int updatecount = 0;
        int addcount = 0;

        string dic = targetPath;
        string[] groups = System.IO.Directory.GetDirectories(dic);
        List<string> branches = new List<string>();
        foreach (var g in groups)
        {
            string path = g.Substring(g.IndexOf(dic) + dic.Length + 1).ToLower();
            branches.Add(path);
        }

        // 哪些分支被删除了
        List<string> toDel = new List<string>();
        foreach(var g in ver.groups)
        {
            if(!branches.Contains(g.Key))
            {
                EditorLogTool.Log("分支被删除：" + g.Key, log4track);
                toDel.Add(g.Key);
                foreach (var f in g.Value.filehash)
                {
                    EditorLogTool.Log("文件被删除：" + g.Key + ":" + f.Key, log4track);
                    delcount++;
                }
            }
        }
        foreach(var d in toDel)
        {
            ver.groups.Remove(d);
        }

        // 添加了哪些分支
        foreach(var b in branches)
        {
            if(!ver.groups.ContainsKey(b))
            {
                EditorLogTool.Log("分支被添加：" + b, log4track);
                vernew.groups[b] = new AM_VerInfo(b);
                vernew.groups[b].GenHash(dic);
                foreach (var f in vernew.groups[b].filehash)
                {
                    EditorLogTool.Log("文件增加：" + b + ":" + f.Key, log4track);
                    addcount++;
                }
            }
        }

        // 分支依然存在
        foreach (var g in ver.groups)
        {
            vernew.groups[g.Key] = new AM_VerInfo(g.Key);
            vernew.groups[g.Key].GenHash(dic);
            foreach (var f in g.Value.filehash)
            {
                if (vernew.groups[g.Key].filehash.ContainsKey(f.Key) == false)
                {
                    EditorLogTool.Log("文件被删除：" + g.Key + ":" + f.Key, log4track);
                    delcount++;
                }
                else
                {
                    string hash = vernew.groups[g.Key].filehash[f.Key];
                    string oldhash = g.Value.filehash[f.Key];
                    if (hash != oldhash)
                    {
                        EditorLogTool.Log("文件更新：" + g.Key + ":" + f.Key, log4track);
                        updatecount++;
                    }
                }
            }
            foreach (var f in vernew.groups[g.Key].filehash)
            {
                if (g.Value.filehash.ContainsKey(f.Key) == false)
                {
                    EditorLogTool.Log("文件增加：" + g.Key + ":" + f.Key, false);
                    addcount++;
                }
            }
        }

        if (addcount == 0 && delcount == 0 && updatecount == 0)
        {
            vernew.Edition = ver.Edition;
            vernew.CodeVer = ver.CodeVer;
            vernew.ResVer = ver.ResVer;
            EditorLogTool.Log("无变化 ver=" + vernew.Edition + '.' + vernew.CodeVer + '.' + vernew.ResVer, log4track);
        }
        else
        {
            vernew.Edition = ver.Edition;
            vernew.CodeVer = ver.CodeVer;
            vernew.ResVer = ver.ResVer + 1;
            EditorLogTool.Log("检查变化结果 add:" + addcount + " remove:" + delcount + " update:" + updatecount, log4track);
            EditorLogTool.Log("版本号变为:" + vernew.Edition + '.' + vernew.CodeVer + '.' + vernew.ResVer, log4track);
        }
    }
    static void GenVer_Imp(string targetPath, string targetVersion = null, bool log4track = false)
    {
        if (vernew == null)
        {
            CheckVer_Imp(targetPath, log4track);
        }
        if (vernew.ResVer == ver.ResVer)
        {
            if(null == targetVersion)
            {
                EditorLogTool.Log("版本无变化", log4track);
                return;
            }
        }
        if(null != targetVersion)
        {
            EditorLogTool.Log("【使用指定的版本号】", targetVersion, log4track);
            vernew.SetVer(targetVersion);
        }
        else
        {
            EditorLogTool.Log("【使用配置文件中的版本号迭代】", ver.GetVerString(), log4track);
        }
        vernew.SaveToPath(targetPath);
        EditorLogTool.Log("生成OK Ver:" + vernew.Edition + '.' + vernew.CodeVer + '.' + vernew.ResVer, log4track);
        ver = vernew;
    }
}
