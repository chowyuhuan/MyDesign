using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class AM_CopyUIPrefab
{
    const string _GUIPrefabExportPath = "Assets/Resources/GUI/UIPrefab";
    const string _GUIPrefabEditorPath = "Assets/Resources_Editor/GUI/UIPrefab";
    [MenuItem("工具/资源/Prefab/拷贝UIPrefab到Resource文件夹")]
    static void CopyUIPrefab()
    {
        DirectoryInfo di = new DirectoryInfo(_GUIPrefabEditorPath);
        CopyUIPrefab(di, true);
    }
    static void CopyUIPrefab(DirectoryInfo di, bool recursive)
    {
        if (di.Exists)
        {
            string basePath = System.Environment.CurrentDirectory.Replace("\\", "/");
            FileInfo[] files = di.GetFiles("*.prefab");
            for (int index = 0; index < files.Length; ++index)
            {
                string fullName = files[index].FullName.Replace("\\", "/");
                string sourceFile = FileUtil.GetProjectRelativePath(fullName);
                string targefile = sourceFile.Replace(_GUIPrefabEditorPath, _GUIPrefabExportPath);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(targefile);
                if (go == null)
                {
                    AssetDatabase.CopyAsset(sourceFile, targefile);
                    go = AssetDatabase.LoadAssetAtPath<GameObject>(targefile);
                    Debug.Log("copy prefab" + sourceFile);
                }
                else
                {
                    GameObject sourcego = AssetDatabase.LoadAssetAtPath<GameObject>(sourceFile);
                    go = PrefabUtility.ReplacePrefab(sourcego, go);
                    Debug.Log("replace prefab" + sourceFile);
                }
                AssetDatabase.Refresh();
            }
            DirectoryInfo[] subDirs = di.GetDirectories();
            for (int index = 0; index < subDirs.Length; ++index)
            {
                CopyUIPrefab(subDirs[index], recursive);
            }
        }
    }
}
