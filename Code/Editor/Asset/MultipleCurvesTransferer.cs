using UnityEditor;
using UnityEngine;

using System.IO;
using System.Collections;

/// <summary>
/// 这个脚本先不用
/// </summary>
public class MultipleCurvesTransferer
{
    const string duplicatePostfix = "Edit";
    const string animationFolder = "Animations";

    static void CopyClip(string importedPath, string copyPath)
    {
        AnimationClip src = AssetDatabase.LoadAssetAtPath(importedPath, typeof(AnimationClip)) as AnimationClip;
        AnimationClip newClip = new AnimationClip();
        newClip.name = src.name + duplicatePostfix;
        AssetDatabase.CreateAsset(newClip, copyPath);
        AssetDatabase.Refresh();
    }

    //[MenuItem("Assets/Transfer Multiple Clips Curves to Copy")]
    static void CopyCurvesToDuplicate()
    {
        // Get selected AnimationClip
        Object[] imported = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.Unfiltered);
        if (imported.Length == 0)
        {
            Debug.LogWarning("Either no objects were selected or the objects selected were not AnimationClips.");
            return;
        }

        //If necessary, create the animations folder.
        if (Directory.Exists("Assets/" + animationFolder) == false)
        {
            AssetDatabase.CreateFolder("Assets", animationFolder);
        }

        foreach (AnimationClip clip in imported)
        {



            string importedPath = AssetDatabase.GetAssetPath(clip);

            //If the animation came from an FBX, then use the FBX name as a subfolder to contain the animations.
            string copyPath;
            if (importedPath.Contains(".fbx"))
            {
                //With subfolder.
                string folder = importedPath.Substring(importedPath.LastIndexOf("/") + 1, importedPath.LastIndexOf(".") - importedPath.LastIndexOf("/") - 1);
                if (!Directory.Exists("Assets/Animations/" + folder))
                {
                    AssetDatabase.CreateFolder("Assets/Animations", folder);
                }
                copyPath = "Assets/Animations/" + folder + "/" + clip.name + duplicatePostfix + ".anim";
            }
            else
            {
                //No Subfolder
                copyPath = "Assets/Animations/" + clip.name + duplicatePostfix + ".anim";
            }

            Debug.Log("CopyPath: " + copyPath);

            CopyClip(importedPath, copyPath);

            AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
            if (copy == null)
            {
                Debug.Log("No copy found at " + copyPath);
                return;
            }
            // Copy curves from imported to copy
            AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(clip, true);
            for (int i = 0; i < curveDatas.Length; i++)
            {
                AnimationUtility.SetEditorCurve(
                    copy,
                    curveDatas[i].path,
                    curveDatas[i].type,
                    curveDatas[i].propertyName,
                    curveDatas[i].curve
                );
            }

            Debug.Log("Copying curves into " + copy.name + " is done");
        }
    }
}