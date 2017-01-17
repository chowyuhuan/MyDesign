using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AM_SceneDependence
{
    List<string> _SceneDependence = new List<string>();

    public AM_SceneDependence(List<string> scenePathList)
    {
        if(null != scenePathList)
        {
            for(int index = 0; index < scenePathList.Count; ++index)
            {
                CollectSceneDependence(scenePathList[index]);
            }
        }
    }

    void CollectSceneDependence(string scenePath)
    {
        if(string.IsNullOrEmpty(scenePath))
        {
            return;
        }
        string[] deps = AssetDatabase.GetDependencies(scenePath, true);
        for (int index = 0; index < deps.Length; ++index)
        {
            EditorUtility.DisplayProgressBar("收集场景依赖:" + scenePath, deps[index], (float)index / deps.Length);
            if (deps[index].Contains(".cs") || _SceneDependence.Contains(deps[index]))
            {
                continue;
            }
            else
            {
                _SceneDependence.Add(deps[index]);
            }
        }
        EditorUtility.ClearProgressBar();
    }

    public bool RefByScene(string assetPath)
    {
        return _SceneDependence.Contains(assetPath);
    }
}
