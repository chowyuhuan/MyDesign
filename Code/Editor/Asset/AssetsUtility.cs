using UnityEngine;
using UnityEditor;
using System.Collections;

public class AssetsUtility
{
    public static void SelectAndHighLightObject(string assetPath)
    {
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        SelectAndHighLightObject(obj);
    }

    public static void SelectAndHighLightObject(Object obj)
    {
        if (obj == null)
        {
            return;
        }
        EditorGUIUtility.PingObject(obj.GetInstanceID());
        EditorUtility.FocusProjectWindow();
        Selection.activeInstanceID = obj.GetInstanceID();
    }
}
