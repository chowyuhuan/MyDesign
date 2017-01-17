using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurvesTransferer
{
    public static string DuplicatePrefix = "";
    public static string DuplicatePostfix = "";

    [MenuItem("工具/资源/动画/同步动画信息到副本")]
    static void CopyCurvesToDuplicate()
    {
        // Get selected AnimationClip
        AnimationClip imported = Selection.activeObject as AnimationClip; 
        if (imported == null)
        { 
            Debug.Log("选择的对象不是AnimationClip");
            return; 
        }
        Transfer(imported,false);
    }

    [MenuItem("工具/资源/动画/同步动画信息到副本（删除原动画）")]
    static void CopyCurvesToDuplicateAndDeleteOrigin()
    {
        // Get selected AnimationClip
        AnimationClip imported = Selection.activeObject as AnimationClip;
        if (imported == null)
        {
            Debug.Log("选择的对象不是AnimationClip");
            return;
        }
        Transfer(imported,true);
    }
    public static AnimationClip Transfer(AnimationClip origin, bool delete)
    {
        // Find path of copy
        string originPath = AssetDatabase.GetAssetPath(origin);
        string copyPath = originPath.Substring(0, originPath.LastIndexOf("/"));
        copyPath += "/" + DuplicatePrefix + origin.name + DuplicatePostfix + ".anim";
        AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
        if (copy == null)
        {
            copy = CopyClip(originPath, copyPath);
        }
        else if(copy.legacy != origin.legacy)
        {
            AnimationClip copyBackup = new AnimationClip();
            EditorUtility.CopySerialized(copy,copyBackup);
            AssetDatabase.DeleteAsset(copyPath);
            AnimationClip newCopy = CopyClip(origin, copyPath);
            Transfer(copyBackup, newCopy, false);
            copy = newCopy;
        }
        Transfer(origin, copy, true);
        if (delete)
        {
            AssetDatabase.DeleteAsset(originPath);
        }
        return copy;
    }
    public static AnimationClip Transfer(string originPath, bool delete)
    {
        AnimationClip origin = AssetDatabase.LoadAssetAtPath<AnimationClip>(originPath);
        string copyPath = originPath.Substring(0, originPath.LastIndexOf("/"));
        copyPath += "/" + DuplicatePrefix + origin.name + DuplicatePostfix + ".anim";
        AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
        if (copy == null)
        {
            copy = CopyClip(originPath, copyPath);
        }
        //else if(copy.frameRate != origin.frameRate) // 美术可能修改帧数，这样的话，考虑在这里提示帧变了，需要删除旧的copy，重新创建一个新的copy。暂时美术只会用30，所以先注掉
        //{

        //}
        Transfer(origin, copy, true);
        if (delete)
        {
            AssetDatabase.DeleteAsset(originPath);
        }
        return copy;
    }

    public static void Transfer(AnimationClip origin, AnimationClip to, bool log)
    {
        // ---------------------------------------------------------------------------
        // 清除to动画中骨骼相关的动画信息
        List<EditorCurveBinding> toBinds = new List<EditorCurveBinding>();
        toBinds.AddRange(AnimationUtility.GetCurveBindings(to));
        string name;
        for (int i = 0; i < toBinds.Count; ++i)
        {
            name = toBinds[i].path;
            int idx = name.LastIndexOf('/');
            name = name.Substring(idx == -1 ? 0 : idx + 1);
            if(name.StartsWith("Bip") || name.StartsWith("Bone"))
            {
                toBinds.RemoveAt(i);
                --i;
            }
        }

        // 将to动画中非骨骼相关的动画重新绑回to
        List<AnimationCurve> toCurves = new List<AnimationCurve>();
        for (int i = 0; i < toBinds.Count; ++i)
        {
            toCurves.Add(AnimationUtility.GetEditorCurve(to, toBinds[i]));
        }
        to.ClearCurves();
        for (int i = 0; i < toBinds.Count; ++i)
        {
            AnimationUtility.SetEditorCurve(to, toBinds[i], toCurves[i]);
        }

        // ------- 以上部分是为了解决：to中记录的骨骼可能多于origin，要清理掉这些 --------

        // 从orgin同步动画信息到to
        //AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(origin, true);
        EditorCurveBinding[] curveDatas = AnimationUtility.GetCurveBindings(origin);
        for (int i = 0; i < curveDatas.Length; i++)
        {
            //AnimationUtility.SetEditorCurve(to, curveDatas[i].path, curveDatas[i].type, curveDatas[i].propertyName, curveDatas[i].curve);
            AnimationUtility.SetEditorCurve(to, curveDatas[i], AnimationUtility.GetEditorCurve(origin, curveDatas[i]));
        }

        // 这里没有做存在的验证，直接将origin的所有event都加到to中了
        if(origin.events != null && origin.events.Length > 0)
        {
            AnimationUtility.SetAnimationEvents(to, origin.events);
        }

        EditorUtility.SetDirty(to);
        //AssetDatabase.Refresh();
        if (log)
        {
            Debug.Log(origin.name + "动画信息同步到" + to.name + "完成");
        }
    }

    static AnimationClip CopyClip(string importedPath, string copyPath) 
    { 
        AnimationClip src = AssetDatabase.LoadAssetAtPath(importedPath, typeof(AnimationClip)) as AnimationClip;
        return CopyClip(src,copyPath);
    }

    static AnimationClip CopyClip(AnimationClip src, string copyPath)
    {
        AnimationClip newClip = new AnimationClip();
        newClip.name = DuplicatePrefix + src.name + DuplicatePostfix;
        EditorUtility.CopySerialized(src, newClip);
        AssetDatabase.CreateAsset(newClip, copyPath);
        //AssetDatabase.Refresh();
        return newClip;
    }
}