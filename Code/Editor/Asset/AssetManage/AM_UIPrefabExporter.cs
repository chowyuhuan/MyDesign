using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AM_UIPrefabExporter {
    const string _GUIAtlasExportBasePath = "Assets/Resources/GUI/UIAtlas";
    public const string GUIPrefab_ExportPath = "Assets/Resources/GUI/UIPrefab";
    public const string GUIPrefab_EditorPath = "Assets/Resources_Editor/GUI/UIPrefab";
    Dictionary<string, int> _SpliteAtlasList = new Dictionary<string, int>();
    Dictionary<string, int> _SpliteMutipleSpriteTexList = new Dictionary<string, int>();
    Dictionary<string, int> _UIDependSpriteList = new Dictionary<string, int>();
    Dictionary<string, Dictionary<string, int>> _UIDependAtlasInfo = new Dictionary<string, Dictionary<string, int>>();
    AM_AppRefGuard _AppRefGuard;
    AM_UITexPackAtlasInfo _UITexPackInfo;

    public AM_UIPrefabExporter(AM_AppRefGuard appRefGuard, AM_UITexPackAtlasInfo uitexPackInfo)
    {
        _AppRefGuard = appRefGuard;
        _UITexPackInfo = uitexPackInfo;
    }

    public void ExportUIPrefab(bool quietly)
    {
        DirectoryInfo di = new DirectoryInfo(GUIPrefab_EditorPath);
        CollectDynamicLoadAtlas(di, true, quietly);
        DumpDynamicLoadAtlas();
        ExportUIPrefab(di, true, quietly);
        ExportGUIAtlas();
    }

    public void ExportUIPrefab(DirectoryInfo di, bool recursive, bool quietly)
    {
        if (di.Exists)
        {
            string basePath = System.Environment.CurrentDirectory.Replace("\\", "/");
            FileInfo[] files = di.GetFiles("*.prefab");
            for (int index = 0; index < files.Length; ++index)
            {
                AM_EditorTool.DisplayProgressBar(quietly, "导出UIPrefab", files[index].FullName, (float)index / files.Length);
                string fullName = files[index].FullName.Replace("\\", "/");
                string sourceFile = FileUtil.GetProjectRelativePath(fullName);
                string targefile = sourceFile.Replace(GUIPrefab_EditorPath, GUIPrefab_ExportPath);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(targefile);
                 if(go == null)
                 {
                     AssetDatabase.CopyAsset(sourceFile, targefile);
                     go = AssetDatabase.LoadAssetAtPath<GameObject>(targefile);
                 }
                 else
                 {
                     GameObject sourcego = AssetDatabase.LoadAssetAtPath<GameObject>(sourceFile);
                     go = PrefabUtility.ReplacePrefab(sourcego, go);
                 }
                SpliteAtlas(go, quietly);
            }
            AM_EditorTool.ClearProgressBar(quietly);
            if(recursive)
            {
                DirectoryInfo[] subDirs = di.GetDirectories();
                for (int index = 0; index < subDirs.Length; ++index)
                {
                    ExportUIPrefab(subDirs[index], recursive, quietly);
                }
            }
        }
    }

    void CollectDynamicLoadAtlas(DirectoryInfo di, bool recursive, bool quietly)
    {
        if (di.Exists)
        {
            string basePath = System.Environment.CurrentDirectory.Replace("\\", "/");
            FileInfo[] files = di.GetFiles("*.prefab");
            for (int index = 0; index < files.Length; ++index)
            {
                AM_EditorTool.DisplayProgressBar(quietly, "检查UIPrefab", files[index].FullName, (float)index / files.Length);
                string fullName = files[index].FullName.Replace("\\", "/");
                string sourceFile = FileUtil.GetProjectRelativePath(fullName);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(sourceFile);
                CollectDynamicLoadAtlas(go, quietly);
            }
            AM_EditorTool.ClearProgressBar(quietly);
            if (recursive)
            {
                DirectoryInfo[] subDirs = di.GetDirectories();
                for (int index = 0; index < subDirs.Length; ++index)
                {
                    CollectDynamicLoadAtlas(subDirs[index], recursive, quietly);
                }
            }
        }
    }

    void CollectDynamicLoadAtlas(GameObject go, bool quietly)
    {
        if (null != go)
        {
            Image[] allImages = go.GetComponentsInChildren<Image>(true);
            for (int index = 0; index < allImages.Length; ++index)
            {
                AM_EditorTool.DisplayProgressBar(quietly, "收集UI图片依赖", go.name + ":" + allImages[index].name, (float)index / allImages.Length);
                AM_UITexPackInfo uitpi;
                if (allImages[index].sprite != null)
                {
                    string spPath = AssetDatabase.GetAssetPath(allImages[index].sprite);
                    _UITexPackInfo.GetUITexPackInfo(spPath, out uitpi);
                    CollectUIDependAtlasInfo(uitpi);
                }
            }
            AM_EditorTool.ClearProgressBar(quietly);
        }
    }

    void DumpDynamicLoadAtlas()
    {
        foreach (string atlasName in _UIDependAtlasInfo.Keys)
        {
            Dictionary<string, int> spriteList = _UITexPackInfo.GetAtlasSprite(atlasName);
            if (spriteList.Count != _UIDependAtlasInfo[atlasName].Count)//UI依赖的Sprite数量与赋有打包Atlas标记的sprite数量不一致，可能存在动态加载的被打包图片，但是没有在图集多引用检查的过程中检出，应该将此图集资源导出
            {
                SpliteAtlas(atlasName);
            }
        }
    }

    public void ExportGUIAtlas()
    {
        foreach(string atlasName in _SpliteAtlasList.Keys)
        {
            AM_AtlasExporter.ExportGUIAtlas(GetAtlasExportPath(atlasName), _UITexPackInfo);
        }

        foreach (string multipleSpriteTex in _SpliteMutipleSpriteTexList.Keys)
        {
            AM_AtlasExporter.CheckMultipleSpriteTexAtlas(multipleSpriteTex);
        }
    }

    public static string GetAtlasExportPath(string atlasName)
    {
        return string.Format("{0}{1}{2}{3}", _GUIAtlasExportBasePath, "/", atlasName, ".prefab");
    }

    public void SpliteAtlas(GameObject go, bool quietly)
    {
        if(null != go)
        {
            Image[] allImages = go.GetComponentsInChildren<Image>(true);
            for(int index = 0; index < allImages.Length; ++index)
            {
                AM_EditorTool.DisplayProgressBar(quietly, "检查UI图片依赖", go.name + ":" + allImages[index].name, (float)index / allImages.Length);
                AM_UITexPackInfo uitpi;
                if(allImages[index].sprite != null)
                {
                    if(_AppRefGuard.SpliteSprite(allImages[index].sprite, out uitpi)
                        || (null != uitpi && _SpliteAtlasList.ContainsKey(uitpi.GetAtlasName()))
                        )
                    {
                        if (uitpi.PackedInAtlas())
                        {
                            GUI_Sprite uisprite = allImages[index].gameObject.AddComponent<GUI_Sprite>();
                            uisprite._Name = allImages[index].sprite.name;
                            uisprite._AtlasName = uitpi.GetAtlasName();
                            allImages[index].sprite = null;
                            if (uitpi.MultipleSpriteTex)
                            {
                                SpliteMultipleSpriteTex(uitpi.AssetPath);
                            }
                            else
                            {
                                SpliteAtlas(uisprite._AtlasName);
                            }
                        }
                    }
                }
            }
            EditorUtility.SetDirty(go);
            AssetDatabase.SaveAssets();
            AM_EditorTool.ClearProgressBar(quietly);
        }
    }

    void AddToUIDependSpriteList(string spriteAssetPath)
    {
        if (!_UIDependSpriteList.ContainsKey(spriteAssetPath))
        {
            _UIDependSpriteList.Add(spriteAssetPath, 0);
        }
    }

    void CollectUIDependAtlasInfo(AM_UITexPackInfo uitpi)
    {
        if(null != uitpi && !_UIDependSpriteList.ContainsKey(uitpi.AssetPath))
        {
            _UIDependSpriteList.Add(uitpi.AssetPath, 0);
            if (uitpi.PackedInAtlas() && !uitpi.MultipleSpriteTex)
            {
                Dictionary<string, int> atlasSpriteList;
                if (_UIDependAtlasInfo.TryGetValue(uitpi.GetAtlasName(), out atlasSpriteList))
                {
                    atlasSpriteList.Add(uitpi.AssetPath, 0);
                }
                else
                {
                    atlasSpriteList = new Dictionary<string, int>();
                    atlasSpriteList.Add(uitpi.AssetPath, 0);
                    _UIDependAtlasInfo.Add(uitpi.GetAtlasName(), atlasSpriteList);
                }
            }
        }
    }

    void SpliteAtlas(string atlasName)
    {
        if(!string.IsNullOrEmpty(atlasName)
            && !_SpliteAtlasList.ContainsKey(atlasName))
        {
            _SpliteAtlasList.Add(atlasName, 0);
        }
    }

    void SpliteMultipleSpriteTex(string texPath)
    {
        if(!string.IsNullOrEmpty(texPath)
            && !_SpliteMutipleSpriteTexList.ContainsKey(texPath))
        {
            _SpliteMutipleSpriteTexList.Add(texPath, 0);
        }
    }
}
