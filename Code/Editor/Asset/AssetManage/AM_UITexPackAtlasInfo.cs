using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AM_UITexPackAtlasInfo {
    Dictionary<string, Dictionary<string, int>> _AtlasInfoDic = new Dictionary<string, Dictionary<string, int>>();
    Dictionary<string, string> _SpriteToAtlasDic = new Dictionary<string, string>();
    Dictionary<string, int> _SingleSpriteTex = new Dictionary<string, int>();
    Dictionary<string, int> _MultipleSpriteTex = new Dictionary<string, int>();
    Dictionary<string, AM_UITexPackInfo> _UITexPackInfo = new Dictionary<string, AM_UITexPackInfo>();

    public AM_UITexPackAtlasInfo(bool quietly)
    {
        AM_PathConfig atlasPathConfig = AssetDatabase.LoadAssetAtPath<AM_PathConfig>("Assets/Scripts/Editor/Asset/AssetManage/TexturePathConfig.asset");

        List<string> atlaspath = atlasPathConfig.GetConfigPathList();
        foreach(AM_PathConfigItem pci in atlasPathConfig._PathItemList)
        {
            if(pci._IsFolder)
            {
                DirectoryInfo dir = new DirectoryInfo(pci.GetItemPath());
                CollectAtPath(dir, pci._Recursive, quietly);
            }
            else
            {
                CollectSpriteInfo(pci.GetItemPath());
            }
        }
    }

    public bool PackedInAtlas(string spritePath)
    {
        return _SpriteToAtlasDic.ContainsKey(spritePath) || _MultipleSpriteTex.ContainsKey(spritePath);
    }

    public bool GetUITexPackInfo(string spritePath, out AM_UITexPackInfo texPackInfo)
    {
        return _UITexPackInfo.TryGetValue(spritePath, out texPackInfo);
    }

    public bool GetPackedAtlas(string spritePath, out string atlasName)
    {
        AM_UITexPackInfo uipi;
        if (_UITexPackInfo.TryGetValue(spritePath, out uipi))
        {
            if (uipi.ImportMode == SpriteImportMode.Multiple)
            {
                atlasName = System.IO.Path.GetFileNameWithoutExtension(spritePath);
            }
            else
            {
                _SpriteToAtlasDic.TryGetValue(spritePath, out atlasName);
            }
            return true;
        }
        else
        {
            Debug.LogError("【UI图集信息】没有找到图片的打包信息 :" + spritePath);
            atlasName = null;
            return false;
        }
    }

    public Dictionary<string, int> GetAtlasSprite(string atlasName)
    {
        Dictionary<string, int> spriteList;
        _AtlasInfoDic.TryGetValue(atlasName, out spriteList);
        return spriteList;
    }

    public void PrintCollectResult()
    {
        Debug.Log("********************altas collect result***********************");
        foreach (string atlasName in _AtlasInfoDic.Keys)
        {
            Dictionary<string, int> splist = _AtlasInfoDic[atlasName];
            foreach(string sprite in splist.Keys)
            {
                Debug.Log(sprite);
            }
        }
        Debug.Log("********************multiple sprite result***********************");
        foreach (string multipleSprite in _MultipleSpriteTex.Keys)
        {
            Debug.Log(multipleSprite);
        }
        Debug.Log("********************singlePic sprite result***********************");
        foreach (string singlePic in _SingleSpriteTex.Keys)
        {
            Debug.Log(singlePic);
        }
        Debug.Log("***********************end********************\n\n");
    }

    public void CollectAtPath(DirectoryInfo di, bool recursive, bool quietly)
    {
        if (di.Exists)
        {
            FileInfo[] fis = di.GetFiles();
            for (int index = 0; index < fis.Length; ++index)
            {
                if (!quietly)
                {
                    EditorUtility.DisplayProgressBar("收集图集信息", fis[index].FullName, (float)index / fis.Length);
                }
                if (!fis[index].FullName.Contains(".meta"))
                {
                    int ap = fis[index].FullName.Replace("\\", "/").IndexOf("/Assets/");
                    string assetpath = fis[index].FullName.Substring(ap + 1);
                    CollectSpriteInfo(assetpath);
                }
            }
            if (!quietly)
            {
                EditorUtility.ClearProgressBar();
            }
            if (recursive)
            {
                DirectoryInfo[] dirs = di.GetDirectories();
                for (int index = 0; index < dirs.Length; ++index)
                {
                    CollectAtPath(dirs[index], recursive, quietly);
                }
            }
        }
        else
        {
            Debug.LogError("【UI图集信息】文件夹不存在" + di.FullName);
        }
    }

    public void CollectSpriteInfo(string spritePath)
    {
        TextureImporter ti = TextureImporter.GetAtPath(spritePath) as TextureImporter;
        if (null != ti)
        {
            string ap = AssetDatabase.GetAssetPath(ti);
            AM_UITexPackInfo packInfo = new AM_UITexPackInfo(ap, ti.spritePackingTag, ti.spriteImportMode);
            _UITexPackInfo.Add(ap, packInfo);
            switch (ti.spriteImportMode)
            {
                case SpriteImportMode.Single:
                case SpriteImportMode.Polygon:
                    {
                        CollectSingleSpriteTex(ti, ap);
                        break;
                    }
                case SpriteImportMode.Multiple:
                    {
                        CollectMultipleSpriteTex(ti, ap);
                        break;
                    }
                case SpriteImportMode.None:
                    {
                        Debug.LogError("【UI图集信息】不是Sprite类型" + spritePath);
                        break;
                    }
            }
        }
        else
        {
            Debug.Log("【UI图集信息】Texture不存在 : " + spritePath);
        }
    }

    void CollectMultipleSpriteTex(TextureImporter ti, string assetPath)
    {
        if (null != ti)
        {
            if (!_MultipleSpriteTex.ContainsKey(assetPath))
            {
                _MultipleSpriteTex.Add(assetPath, 0);
            }
        }
    }

    void CollectSingleSpriteTex(TextureImporter ti, string assetPath)
    {
        if (null != ti)
        {
            if (string.IsNullOrEmpty(ti.spritePackingTag))
            {
                if (!_SingleSpriteTex.ContainsKey(assetPath))
                {
                    _SingleSpriteTex.Add(assetPath, 0);
                }
            }
            else
            {
                string atlasName = AM_EditorTool.ParseAtlasName(ti.spritePackingTag);
                Dictionary<string, int> spriteList = null;
                if (!_SpriteToAtlasDic.ContainsKey(assetPath))
                {
                    if (_AtlasInfoDic.TryGetValue(atlasName, out spriteList))
                    {
                        spriteList.Add(assetPath, 0);
                    }
                    else
                    {
                        spriteList = new Dictionary<string, int>();
                        spriteList.Add(assetPath, 0);
                        _AtlasInfoDic.Add(atlasName, spriteList);
                    }
                    _SpriteToAtlasDic.Add(assetPath, atlasName);
                }
            }
        }
    }
}
