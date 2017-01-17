using UnityEngine;
using UnityEditor;
using System.Collections;

public class NormalizeAssets : MonoBehaviour
{
    #region Animator
    [MenuItem("工具/资源/Prefab/去除勇士动画")]
    static void ClearHeroAnimator()
    {
        EditorUtility.DisplayCancelableProgressBar("去除勇士动画", "正在查找，请稍等...", 0);
        Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        for (int i = 0; i < objects.Length && !EditorUtility.DisplayCancelableProgressBar("去除勇士动画", objects[i] == null ? null : objects[i].name, (float)i / (float)objects.Length); ++i)
        {
            GameObject o = objects[i] as GameObject;
            if(o == null)
            {
                continue;
            }
            if (!AssetDatabase.GetAssetPath(o).EndsWith(".prefab"))
            {
                continue;
            }
            if (o.name.StartsWith("Actor_Mon") || o.name.StartsWith("Actor_Bos"))
            {
                continue;
            }
            Animator anim = o.GetComponent<Animator>();
            if (anim != null)
            {
                GameObject.DestroyImmediate(anim, true);
                EditorUtility.SetDirty(o);
                Debug.LogError("去除勇士动画：" + o.name);
            }
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }
    #endregion


    #region Avatar
    [MenuItem("工具/资源/动画/重定向Avatar")]
    static void CheckAndRetargetAvatarts()
    {
        EditorUtility.DisplayCancelableProgressBar("重定向Avatar", "正在查找，请稍等...", 0);
        Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        RetargetAvatars(objects);
    }
    static void RetargetAvatars(Object[] objects)
    {
        for (int i = 0; i < objects.Length && !EditorUtility.DisplayCancelableProgressBar("重定向Avatar", objects[i] == null ? null : objects[i].name, (float)i / (float)objects.Length); ++i)
        {
            ModelImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(objects[i])) as ModelImporter;
            if (importer != null)
            {
                RetargetAvatar(importer);
            }
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }
    static void RetargetAvatar(ModelImporter importer)
    {
        int idx = importer.assetPath.IndexOf("@");
        if(idx == -1)
        {
            return;
        }
        string modelPath = importer.assetPath.Remove(idx);
        modelPath += ".FBX";
        Avatar sourceAvatar = AssetDatabase.LoadAssetAtPath<Avatar>(modelPath);
        if (importer.sourceAvatar != sourceAvatar)
        {
            importer.sourceAvatar = sourceAvatar;
            importer.SaveAndReimport();
        }
    }
    #endregion
}
