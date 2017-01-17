using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AM_AppRefGuard
{
    Dictionary<string, int> _AppReferenceAssets = new Dictionary<string, int>();
    Dictionary<string, Dictionary<string, int>> _AppInverseReferenceDic = new Dictionary<string, Dictionary<string, int>>();
    Dictionary<string, Dictionary<string, int>> _PackedTextureInverseRefrenceDic = new Dictionary<string, Dictionary<string, int>>(); 
    AM_UITexPackAtlasInfo _UITexPackAtlasInfo;

    public AM_AppRefGuard(bool quietly, AM_PathConfig appUsePathConfig, AM_UITexPackAtlasInfo packAtlasInfo)
    {
        _UITexPackAtlasInfo = packAtlasInfo;
        AM_PathConfig scenePathConfig = AssetDatabase.LoadAssetAtPath<AM_PathConfig>("Assets/Scripts/Editor/Asset/AssetManage/ScenePathConfig.asset");
        CollectAppUseAssets(appUsePathConfig, quietly);
        CollectSceneInverseRefAssets(scenePathConfig, quietly);
    }

    void CollectSceneInverseRefAssets(AM_PathConfig scenePathConfig, bool quietly)
    {
        foreach(AM_PathConfigItem pci in scenePathConfig._PathItemList)
        {
            if(pci.Valid())
            {
                CollectAppUseAsset(pci.GetItemPath(), quietly);
            }
            else
            {
                Debug.LogError("【路径配置文件包含不存在的场景，请检查路径配置文件后重试】");
            }
        }
    }

    void CollectAppUseAssets(AM_PathConfig appUsePathConfig, bool quietly)
    {
        foreach (AM_PathConfigItem pci in appUsePathConfig._PathItemList)
        {
            if(pci.Valid())
            {
                string assetPath = pci.GetItemPath();
                if (pci._IsFolder)
                {
                    DirectoryInfo dir = new DirectoryInfo(assetPath);
                    CollectAppUseAssets(dir, pci._Recursive, quietly);
                }
                else
                {
                    CollectAppUseAsset(assetPath, quietly);
                }
            }
            else
            {
                Debug.LogError("【路径配置文件包含空值，请检查路径配置文件后重试】");
            }
        }
    }

    void CollectAppUseAssets(DirectoryInfo dir, bool recursive, bool quietly)
    {
        if(dir.Exists && !dir.Name.Contains(".svn"))
        {
            FileInfo[] files = dir.GetFiles();
            for(int index = 0; index < files.Length; ++index)
            {
                AM_EditorTool.DisplayProgressBar(quietly, "收集应用使用的文件", files[index].FullName, (float)index / files.Length);
                if(files[index].FullName.Contains(".meta"))
                {
                    continue;
                }
                string fullName = files[index].FullName.Replace("\\", "/");
                string sourceFile = FileUtil.GetProjectRelativePath(fullName);
                CollectAppUseAsset(sourceFile, quietly);
            }
            AM_EditorTool.ClearProgressBar(quietly);
            if (recursive)
            {
                DirectoryInfo[] subDirs = dir.GetDirectories();
                for (int index = 0; index < subDirs.Length; ++index)
                {
                    CollectAppUseAssets(subDirs[index], recursive, quietly);
                }
            }
        }
    }

    void CollectAppUseAsset(string assetPath, bool quietly)
    {
        AddToAppRefenceDic(assetPath);
        string[] depens = AssetDatabase.GetDependencies(assetPath, false);//获取直接的引用依赖
        if (null != depens)
        {
            for (int index = 0; index < depens.Length; ++index)
            {
                AM_EditorTool.DisplayProgressBar(quietly, "收集应用使用的文件", depens[index], (float)index / depens.Length);
                AddToInverseDepenDic(depens[index], assetPath);
                TryAddToPackedTextureInverseDepenDic(depens[index], assetPath);
            }
        }
    }

    void AddToInverseDepenDic(string inverseDepen, string obpath)
    {
        if (inverseDepen.Contains(".cs"))
        {
            return;
        }
        AddToAppRefenceDic(inverseDepen);
        if (!_AppInverseReferenceDic.ContainsKey(inverseDepen))
        {
            Dictionary<string, int> inverseDependencies = new Dictionary<string,int>();
            inverseDependencies.Add(obpath, 0);
            _AppInverseReferenceDic.Add(inverseDepen, inverseDependencies);
        }
        else if (!_AppInverseReferenceDic[inverseDepen].ContainsKey(obpath))
        {
            _AppInverseReferenceDic[inverseDepen].Add(obpath, 0);
        }
    }

    void TryAddToPackedTextureInverseDepenDic(string spritePath, string uiObjectPath)
    {
        if (spritePath.Contains(".cs"))
        {
            return;
        }
        AM_UITexPackInfo uitpi;
        if(_UITexPackAtlasInfo.GetUITexPackInfo(spritePath, out uitpi))
        {
            if(uitpi.PackedInTexture())
            {
                string packedTextureName = uitpi.GetPackedTextureName();
                Dictionary<string, int> inverseDepenDic;
                if (_PackedTextureInverseRefrenceDic.TryGetValue(packedTextureName, out inverseDepenDic))
                {
                    if(!inverseDepenDic.ContainsKey(uiObjectPath))
                    {
                        inverseDepenDic.Add(uiObjectPath, 0);
                    }
                }
                else
                {
                    inverseDepenDic = new Dictionary<string, int>();
                    inverseDepenDic.Add(uiObjectPath, 0);
                    _PackedTextureInverseRefrenceDic.Add(packedTextureName, inverseDepenDic);
                }
            }
        }
    }

    void AddToAppRefenceDic(string assetPath)
    {
        if (!_AppReferenceAssets.ContainsKey(assetPath))
        {
            _AppReferenceAssets.Add(assetPath, 0);
        }
    }

    public bool RefByApp(string assetPath)
    {
        return _AppReferenceAssets.ContainsKey(assetPath) || _UITexPackAtlasInfo.PackedInAtlas(assetPath);
    }

    public int GetInverseReferenceCount(string assetPath)
    {
        Dictionary<string, int> inversDic;
        if(_AppInverseReferenceDic.TryGetValue(assetPath, out inversDic))
        {
            return inversDic.Count;
        }
        return 0;
    }

    public bool GetInverseDependenceDic(string assetPath, out Dictionary<string, int> inverDependeceDic)
    {
        return _AppInverseReferenceDic.TryGetValue(assetPath, out inverDependeceDic);
    }
    
    int GetTextureInverseRefrenceCount(string packedTextureName)
    {
        Dictionary<string, int> inversDic;
        if (_PackedTextureInverseRefrenceDic.TryGetValue(packedTextureName, out inversDic))
        {
            return inversDic.Count;
        }
        return 0;
    }

    public bool SpliteSprite(Sprite sp, out AM_UITexPackInfo uitpi)
    {
        string ap = AssetDatabase.GetAssetPath(sp);
        _UITexPackAtlasInfo.GetUITexPackInfo(ap, out uitpi);
        return SpliteUITexture(uitpi);
    }

    public bool SpliteUITexture(AM_UITexPackInfo uitpi)
    {
        if (null != uitpi)
        {
            if (uitpi.PackedInTexture())
            {
                return GetTextureInverseRefrenceCount(uitpi.GetPackedTextureName()) > 1;
            }
            else
            {
                return GetInverseReferenceCount(uitpi.AssetPath) > 1;
            }
        }
        return false;
    }

    public bool IsMultipleDepen(string assetPath, out bool uiTex, out AM_UITexPackInfo uitpi)
    {
        uiTex = IsUITextrue(assetPath, out uitpi);
        if (uiTex)
        {
            return SpliteUITexture(uitpi);
        }
        else
        {
            return GetInverseReferenceCount(assetPath) > 1;
        }
    }

    //没有被任何资源引用的资源(动态加载的资源)
    public bool IsRootAsset(string assetPath)
    {
        return GetInverseReferenceCount(assetPath) == 0;
    }

    public bool IsUITextrue(string assetpath, out AM_UITexPackInfo uitpi)
    {
        return _UITexPackAtlasInfo.GetUITexPackInfo(assetpath, out uitpi);
    }
}
