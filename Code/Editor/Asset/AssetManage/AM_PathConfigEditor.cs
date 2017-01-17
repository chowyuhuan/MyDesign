using UnityEngine;
using UnityEditor;
using System.Collections;

public class AM_PathConfigEditor {
    public static AM_PathConfig LoadAtPath(string assetPath)
    {
        return AssetDatabase.LoadAssetAtPath<AM_PathConfig>(assetPath);
    }

    [MenuItem("工具/资源/路径/创建路径配置文件")]
    static void CreateAtlasPathConfig()
    {
        string ap = EditorUtility.SaveFilePanel("新建路径配置文件", "Assets/Scripts/Editor/Asset/AssetManage", "PathConfig", "asset");
        if (!string.IsNullOrEmpty(ap))
        {
            ap = ap.Replace("\\", "/");
            ap = FileUtil.GetProjectRelativePath(ap);
            if (null == LoadAtPath(ap))
            {
                AM_PathConfig go = ScriptableObject.CreateInstance<AM_PathConfig>();
                AssetDatabase.CreateAsset(go, ap);
                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
