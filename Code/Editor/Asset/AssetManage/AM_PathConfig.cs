using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class AM_PathConfigItem
{
    public Object _ConfigItem;
    public bool _IsFolder;
    public bool _Recursive = false;

    public string GetItemPath()
    {
        return AssetDatabase.GetAssetPath(_ConfigItem);
    }

    public bool Valid()
    {
        return null != _ConfigItem;
    }
}

public class AM_PathConfig : ScriptableObject {
    public List<AM_PathConfigItem> _PathItemList = new List<AM_PathConfigItem>();
    List<string> _PathList = new List<string>();
    bool _InitDone = false;

    public void Init()
    {
        for(int index = 0; index < _PathItemList.Count ; ++index)
        {
            string ap = _PathItemList[index].GetItemPath();
            if(!string.IsNullOrEmpty(ap) && !_PathList.Contains(ap))
            {
                _PathList.Add(ap);
            }
        }
        _InitDone = true;
    }

    public bool LocateInConfigedPath(string assetPath)
    {
        if(!_InitDone)
        {
            Init();
        }
        for (int index = 0; index < _PathList.Count; ++index)
        {
            if (assetPath.Contains(_PathList[index]))
            {
                return true;
            }
        }
        return false;
    }

    public List<string> GetConfigPathList()
    {
        if(!_InitDone)
        {
            Init();
        }

        return _PathList;
    }
}
