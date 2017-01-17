using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class AM_EditorTool {
    public static Dictionary<string, int> GetAllAssetPathFastDic(string[] allAssetPath = null)
    {
        if(null == allAssetPath)
        {
            allAssetPath = AssetDatabase.GetAllAssetPaths();
        }
        Dictionary<string, int> curAssetPathDic = new Dictionary<string, int>();
        for (int index = 0; index < allAssetPath.Length; ++index)
        {
            curAssetPathDic.Add(allAssetPath[index], index);
        }
        return curAssetPathDic;
    }

    public static Dictionary<string, int> GetFastIndexDic(List<string> list)
    {
        Dictionary<string, int> fastList = new Dictionary<string,int>();
        if(null != list)
        {
            for(int index = 0; index < list.Count; ++index)
            {
                fastList.Add(list[index], index);
            }
        }
        return fastList;
    }

    public static bool GetResourcesRelativePath(string sourcePath, out string relativePath)
    {
        if (!string.IsNullOrEmpty(sourcePath))
        {
            int index = sourcePath.IndexOf("/Resources/");
            if (index > -1)
            {
                relativePath = sourcePath.Substring(index + "/Resources/".Length);
                return true;
            }
        }
        relativePath = null;
        return false;
    }

    public static string GetFilePathWithoutExtension(string filePath)
    {
        if(!string.IsNullOrEmpty(filePath))
        {
            string dir = System.IO.Path.GetDirectoryName(filePath);
            string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            if (string.IsNullOrEmpty(dir))
            {
                return name;
            }
            else
            {
                return dir + "/" + name;
            }
        }
        else
        {
            return null;
        }
    }

    public static bool IsSceneFile(string filePath)
    {
        if(!string.IsNullOrEmpty(filePath))
        {
            string suffix = System.IO.Path.GetExtension(filePath);
            if(suffix == ".unity")
            {
                return true;
            }
        }
        return false;
    }

    public static void DisplayProgressBar(bool quietly, string title, string info, float progress)
    {
        if(!quietly)
        {
            EditorUtility.DisplayProgressBar(title, info, progress);
        }
    }

    public static void ClearProgressBar(bool quietly)
    {
        if(!quietly)
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static bool IsAtlas(string assetPath, out GUI_Atlas atlas)
    {
        atlas = AM_AtlasExporter.LoadAtPath(assetPath);
        return null != atlas;
    }

    public static string ParseAtlasName(string spritePackTag)
    {
        int tagpos = spritePackTag.LastIndexOf("]");
        string atlasName = spritePackTag.Substring(tagpos + 1);
        return atlasName;
    }

    public static Dictionary<string, string> ParseCmdLineParams(string[] cmdLines, bool log4track)
    {
        EditorLogTool.BeginLog4TrackBlock("解析命令行参数", log4track);
        Dictionary<string, string> cmdParams = new Dictionary<string, string>();
        if(null != cmdLines)
        {
            char [] spliter = {'='};
            for(int index = 0; index < cmdLines.Length; ++index)
            {
                if(cmdLines[index].Contains("="))
                {
                    string[] paramArray = cmdLines[index].Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                    if(paramArray.Length == 2)
                    {
                        cmdParams.Add(paramArray[0], paramArray[1]);
                        EditorLogTool.Log("【参数" + paramArray[0] + "】", "【" + paramArray[1] + "】", log4track);
                    }
                    else
                    {
                        cmdParams.Add(paramArray[0], null);
                        EditorLogTool.Log("【参数" + paramArray[0] + "】", "【】", log4track);
                    }
                }
            }
        }
        EditorLogTool.Log("【参数总计】", cmdParams.Count.ToString(), log4track);
        EditorLogTool.EndLog4TrackBlock(log4track);
        return cmdParams;
    }

    public static string TimeStampStr(string timeFormat = "yyyy-MM-dd HH:mm:ss.ffff")
    {
        return System.DateTime.Now.ToString(timeFormat);
    }
}
