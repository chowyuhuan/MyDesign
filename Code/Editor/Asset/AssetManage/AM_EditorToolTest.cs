using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Sprites;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AM_EditorToolTest {
    [MenuItem("工具/资源/测试/UIPrefab/导出UIPrefab")]
    static void TestUIprefabExport()
    {
        bool quietly = false;
        AM_UITexPackAtlasInfo texPackInfo = new AM_UITexPackAtlasInfo(quietly);
        AM_PathConfig appUsePathConfig = AssetDatabase.LoadAssetAtPath<AM_PathConfig>("Assets/Scripts/Editor/Asset/AssetManage/AppUsePathConfig.asset");
        AM_AppRefGuard apprefguard = new AM_AppRefGuard(quietly , appUsePathConfig, texPackInfo);

        AM_UIPrefabExporter uiexporter = new AM_UIPrefabExporter(apprefguard, texPackInfo);
        uiexporter.ExportUIPrefab(quietly);
    }

    [MenuItem("工具/资源/测试/UIPrefab/打印选中的Prefab中sprite信息")]
    static void PrintSpriteInfo()
    {
        Debug.Log("*********************sprite info************************");
        GameObject go = Selection.activeGameObject;
        if(null != go)
        {
            Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
            Image[] ims = go.GetComponentsInChildren<Image>();
            for(int index = 0; index < ims.Length; ++index)
            {
                if(null != ims[index].sprite)
                {
                    string ap = AssetDatabase.GetAssetPath(ims[index].sprite);
                    if(!spriteDic.ContainsKey(ap))
                    {
                        spriteDic.Add(ap, ims[index].sprite);
                    }
                }
            }
            foreach(string ap in spriteDic.Keys)
            {
                Debug.Log("*********************" + spriteDic[ap].name + "************************");
                Texture2D tex = SpriteUtility.GetSpriteTexture(spriteDic[ap], false);
                Debug.Log("unity tex path:" + AssetDatabase.GetAssetPath(tex));
                Debug.Log("sprite tex path" + ap);
            }
        }
        
    }

    [MenuItem("工具/资源/测试/依赖/打印选中资源的所有依赖信息")]
    static void PrintAllDependence()
    {
        Debug.Log("*********************所有依赖信息************************");
        string ap = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (null != ap)
        {
            string[] dependences = AssetDatabase.GetDependencies(ap, true);
            for(int index = 0; index < dependences.Length; ++index)
            {
                Debug.Log(dependences[index]);
            }
        }
    }

    [MenuItem("工具/资源/测试/依赖/打印选中资源的直接依赖信息")]
    static void PrintDirectDependence()
    {
        Debug.Log("*********************直接依赖信息************************");
        string ap = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (null != ap)
        {
            string[] dependences = AssetDatabase.GetDependencies(ap, false);
            for (int index = 0; index < dependences.Length; ++index)
            {
                Debug.Log(dependences[index]);
            }
        }
    }

    [MenuItem("工具/资源/测试/依赖/打印所有依赖于选中物件的资源信息")]
    static void PrintAllDependenceForSelectObject()
    {
        string ap = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(string.Format("*********物件[{0}]的依赖信息*********", ap));
        int totalCount = 0;
        if (null != ap)
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            for (int index = 0; index < allAssets.Length; ++index)
            {
                EditorUtility.DisplayProgressBar("检查中", "依赖信息", (float)index / allAssets.Length);
                string[] depencecies = AssetDatabase.GetDependencies(allAssets[index], true);
                for (int depIndex = 0; depIndex < depencecies.Length; ++depIndex)
                {
                    if (ap == depencecies[depIndex])
                    {
                        Debug.Log(depencecies[depIndex]);
                        ++totalCount;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
        Debug.Log(string.Format("*********************总计：{0}************************", totalCount.ToString()));
    }

    [MenuItem("工具/资源/测试/Texture/打印选中Texture包含的Sprite信息")]
    static void PrintMultipleSpriteAtlasInfo()
    {
        Debug.Log("*********************PrintMultipleSpriteAtlasInfo************************");
        string ap = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (null != ap)
        {
            TextureImporter ti = TextureImporter.GetAtPath(ap) as TextureImporter;
            if(null != ti && ti.spriteImportMode == SpriteImportMode.Multiple)
            {
                SpriteMetaData[] smd = ti.spritesheet;
                for(int index = 0; index < smd.Length; ++index)
                {
                    Debug.Log(smd[index].name);
                }
            }
        }
    }

    [MenuItem("工具/资源/测试/Texture/打印选中Sprite的打包信息")]
    static void PrintSelectSpritePackedAtlasInfo()
    {
        Debug.Log("*********************PrintSelectSpritePackedAtlasInfo************************");
        string ap = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (null != ap)
        {
            Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true, Packer.Execution.Normal);
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(ap);
            string atlasName;
            Texture2D atlasTex;
            Packer.GetAtlasDataForSprite(sp, out atlasName, out atlasTex);
            if(null != atlasTex)
            {
                Debug.Log("图集：" + atlasName + "----width:" + atlasTex.width + "----height:" + atlasTex.height);
            }
            else
            {
                Debug.Log(sp + "没有在任何图集中");
            }
        }
        else
        {
            Debug.LogError("所选物件不是Sprite:" + ap);
        }
    }

    [MenuItem("工具/资源/测试/Texture/查看选中Sprite的打包图集")]
    static void SaveSelectSpritePackedAtlasAsPNG()
    {
        Debug.Log("*********************SaveSelectSpritePackedAtlasAsPNG************************");
        string ap = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (null != ap)
        {
            Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true, Packer.Execution.Normal);
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(ap);
            string atlasName;
            Texture2D atlasTex;
            Packer.GetAtlasDataForSprite(sp, out atlasName, out atlasTex);
            if (null != atlasTex)
            {
                string texAp = EditorUtility.SaveFilePanel("选择要编辑的Atlas文件", "Assets/Resources_Editor/GUI", atlasName, "png");
                texAp = texAp.Replace("\\", "/");
                texAp = FileUtil.GetProjectRelativePath(texAp);
                if(!string.IsNullOrEmpty(texAp))
                {
                    Texture2D newTex = new Texture2D(atlasTex.width, atlasTex.height, atlasTex.format, false);
                    Graphics.CopyTexture(atlasTex, newTex);
                    newTex.Apply();

                    Texture2D saveTex = new Texture2D(atlasTex.width, atlasTex.height, TextureFormat.RGBA32, false);
                    saveTex.SetPixels32(newTex.GetPixels32());
                    saveTex.Apply();

                    byte[] img = saveTex.EncodeToJPG();
                    FileStream file = File.Open(texAp, FileMode.Create);
                    BinaryWriter writer = new BinaryWriter(file);
                    writer.Write(img);
                    file.Close();

                    Debug.Log("生成图片：" + texAp);
                    Debug.Log("编辑器中的图集格式：" + atlasTex.format.ToString());
                    Debug.Log("生成图片格式：" + saveTex.format.ToString());
                    Debug.Log("生成图长度：" + img.Length);
                }
            }
            else
            {
                Debug.Log(sp + "没有在任何图集中");
            }
        }
        else
        {
            Debug.LogError("所选物件不是Sprite:" + ap);
        }
    }

    [MenuItem("工具/资源/测试/清除进度条")]
    static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }
}
