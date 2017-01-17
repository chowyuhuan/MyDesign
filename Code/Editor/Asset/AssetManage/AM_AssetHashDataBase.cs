using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

[CreateAssetMenu(fileName = "AssetHashDataBase", menuName = "AssetHashDataBase")]
public class AM_AssetHashDataBase : ScriptableObject {
    const string Asset_Hash_File_Path = "Assets/Scripts/Editor/Asset/AssetManage/AssetHashDataBase.asset";
    [SerializeField]
    List<string> _AssetPath = new List<string>();
    [SerializeField]
    List<string> _AssetHash = new List<string>();

    Dictionary<string, string> _AssetHashDic = new Dictionary<string, string>();

    public static AM_AssetHashDataBase GetAssetHashDataBase(bool initFastDic)
    {
        AM_AssetHashDataBase ahdb = AssetDatabase.LoadAssetAtPath<AM_AssetHashDataBase>(Asset_Hash_File_Path);
        if(initFastDic)
        {
            ahdb.InitFastDic();
        }
        return ahdb;
    }

    void InitFastDic()
    {
        Debug.Assert(_AssetPath.Count == _AssetHash.Count);
        _AssetHashDic.Clear();
        for(int index = 0; index < _AssetPath.Count; ++index)
        {
            _AssetHashDic.Add(_AssetPath[index], _AssetHash[index]);
        }
    }

    void RefreshFastDic()
    {
        InitFastDic();
    }

    public bool GetAssetHash(string ap, out string hash)
    {
        return _AssetHashDic.TryGetValue(ap, out hash);
    }

    public void Clear()
    {
        _AssetPath.Clear();
        _AssetHash.Clear();
        _AssetHashDic.Clear();
    }

    public void Add(string ap, Hash128 hash)
    {
        _AssetPath.Add(ap);
        _AssetHash.Add(hash.ToString());
    }

    public void Remove(string ap)
    {
        string hash;
        if(_AssetHashDic.TryGetValue(ap, out hash))
        {
            _AssetPath.Remove(ap);
            _AssetHash.Remove(hash);
        }
    }

    public void UpdateAssetHash(string assetPath, Hash128 hash)
    {
        int index = _AssetPath.IndexOf(assetPath);
        string hashValue = hash.ToString();
        _AssetHash[index] = hashValue;
        _AssetHashDic[assetPath] = hashValue;
    }

    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public List<string> GetDeletedAsset(string[] allAssetPath, bool quietly, bool log4track)
    {
        Dictionary<string, int> fastDic = AM_EditorTool.GetAllAssetPathFastDic(allAssetPath);
        return GetDeletedAsset(fastDic, quietly, log4track);
    }

    public List<string> GetDeletedAsset(Dictionary<string, int> curAssetPathDic, bool quietly, bool log4track)
    {
        if(log4track)
        {
            Log4Track.BeginBlock("GetDeletedAsset");
        }
        List<string> removedAssetList = new List<string>();
        if(null != curAssetPathDic)
        {
            for(int index =0; index < _AssetPath.Count; ++index)
            {
                if(!quietly)
                {
                    EditorUtility.DisplayProgressBar("检查删除的资源项", _AssetPath[index], (float)index / _AssetPath.Count);
                }
                if(!curAssetPathDic.ContainsKey(_AssetPath[index]))
                {
                    if(log4track)
                    {
                        Log4Track.Log("Deleted  ", _AssetPath[index]);
                    }
                    removedAssetList.Add(_AssetPath[index]);
                }
            }
            if(!quietly)
            {
                EditorUtility.ClearProgressBar();
            }
        }
        if(log4track)
        {
            Log4Track.EndBlock();
        }
        return removedAssetList;
    }

    public bool DumpChangedAssetList(string[] allpath, bool quietly, bool log4track, out Dictionary<string, int> assetChangeList, out Dictionary<string, int> assetAddList)
    {
        assetChangeList = new Dictionary<string, int>();
        assetAddList = new Dictionary<string, int>();

        for (int index = 0; index < allpath.Length; ++index)
        {
            if (!quietly)
            {
                EditorUtility.DisplayProgressBar("检查资源Hash", allpath[index], (float)index / allpath.Length);
            }
            if (File.Exists(allpath[index])
                && !allpath[index].Contains(".cs")
                && allpath[index].Contains("Assets/"))
            {
                string oldHash;
                if (GetAssetHash(allpath[index], out oldHash))
                {
                    Hash128 curHash = AssetDatabase.GetAssetDependencyHash(allpath[index]);
                    if (oldHash != curHash.ToString())
                    {
                        if(!assetChangeList.ContainsKey(allpath[index]))
                        {
                            assetChangeList.Add(allpath[index], 0);
                            EditorLogTool.Log("【改动】", allpath[index], log4track);
                        }
                        else
                        {
                            EditorLogTool.LogError("【资源变动导出】【重复改动】", allpath[index], log4track);
                        }
                    }
                }
                else
                {
                    if(!assetAddList.ContainsKey(allpath[index]))
                    {
                        assetAddList.Add(allpath[index], 0);
                        EditorLogTool.Log("【添加】", allpath[index], log4track);
                    }
                    else
                    {
                        EditorLogTool.LogError("【资源变动导出】【重复新资源】", allpath[index], log4track);
                    }
                }
            }
        }
        if (!quietly)
        {
            EditorUtility.ClearProgressBar();
        }
        return assetChangeList.Count > 0 || assetAddList.Count > 0;
    }
}
