using UnityEngine;
using UnityEditor.Sprites;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AM_AtlasExporter {
    const string _GUIAtlasTempletePath = "Assets/Scripts/Editor/Asset/AssetManage/GUIAtlasTemplete.prefab";

    public static void CheckMultipleSpriteTexAtlas(string texPath)
    {
        TextureImporter ti = TextureImporter.GetAtPath(texPath) as TextureImporter;
        if(null != ti)
        {
            string ap = System.IO.Path.GetFileNameWithoutExtension(texPath);
            string atlasPath = AM_UIPrefabExporter.GetAtlasExportPath(ap);
            GUI_Atlas atlas = LoadAtPath(atlasPath);
            if(null != atlas)
            {
                atlas.Init();
                SpriteMetaData[] smd = ti.spritesheet;
                for(int index = 0; index < smd.Length; ++index)
                {
                    if(!atlas.HasSprite(smd[index].name))
                    {
                        Debug.LogError("多sprite图片" + texPath + "对应的图集中缺少sprite " + smd[index].name + "的引用");
                    }
                }
            }
            else
            {
                Debug.LogError("多sprite图片没有对应的图集文件：" + texPath);
            }
        }
        else
        {
            Debug.LogError("错误：该资源不是多图片的贴图" + texPath);
        }
    }

    public static void ExportGUIAtlas(string atlasPath, AM_UITexPackAtlasInfo atlasInfo)
    {
        if(!string.IsNullOrEmpty(atlasPath) && null != atlasInfo)
        {
            GUI_Atlas uiatlas = LoadAtPath(atlasPath);
            if (null == uiatlas)
            {
                uiatlas = CreateEmptyGUIAtlas(atlasPath);
                string atlasName = System.IO.Path.GetFileNameWithoutExtension(atlasPath);
                uiatlas._Name = atlasName;
            }
            UpdateGUIAtlas(uiatlas, atlasInfo);
        }
    }

    static void UpdateGUIAtlas(GUI_Atlas uiatlas, AM_UITexPackAtlasInfo atlasInfo)
    {
        Dictionary<string, int> curSpriteList = atlasInfo.GetAtlasSprite(uiatlas._Name);
        Dictionary<string, int> oldSpriteList = GetGUIAtlasSpritePathList(uiatlas);
        bool dirty = false;
        if (null != curSpriteList && null != oldSpriteList)
        {
            List<string> deleteList = new List<string>();
            Dictionary<string, bool> visitTag = new Dictionary<string, bool>();
            foreach (string sp in oldSpriteList.Keys)
            {
                if (!curSpriteList.ContainsKey(sp))
                {                    
                    deleteList.Add(sp);
                }
                else
                {
                    visitTag[sp] = false;
                }
            }
            dirty = deleteList.Count > 0;
            for (int index = 0; index < deleteList.Count; ++index )
            {
                uiatlas.RemoveSpriteFromFullPath(deleteList[index]);
            }
            foreach (string sp in curSpriteList.Keys)
            {
                if (!visitTag.ContainsKey(sp))
                {
                    dirty = true;
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(sp);
                    uiatlas.AddSprite(sprite);
                }
            }
        }
        if(dirty)
        {
            EditorUtility.SetDirty(uiatlas);
            AssetDatabase.SaveAssets();
        }
    }

    public static GUI_Atlas CreateEmptyGUIAtlas(string atlasPath)
    {
        AssetDatabase.CopyAsset(_GUIAtlasTempletePath, atlasPath);
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(atlasPath);
        return go.GetComponent<GUI_Atlas>();
    }

    static Dictionary<string, int> GetGUIAtlasSpritePathList(GUI_Atlas uiatlas)
    {
        Dictionary<string, int> spritePathList = new Dictionary<string, int>();
        if(null != uiatlas)
        {
            for(int index = 0; index < uiatlas._SpriteList.Count; ++index)
            {
                Sprite sprite = uiatlas._SpriteList[index];
                string spPath = AssetDatabase.GetAssetPath(sprite);
                if(!string.IsNullOrEmpty(spPath))
                {
                    spritePathList.Add(spPath, 0);
                }
            }
        }
        return spritePathList;
    }

    public static GUI_Atlas LoadAtPath(string atlasPath)
    {
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(atlasPath);
        if (null != go)
        {
            return go.GetComponent<GUI_Atlas>();
        }
        return null;
    }
}
