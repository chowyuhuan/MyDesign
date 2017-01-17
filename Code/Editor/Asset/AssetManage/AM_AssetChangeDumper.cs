using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AM_AssetChangeDumper
{
    AM_AppRefGuard _AppRefGuard = null;
    List<string> _ChangeList = new List<string>();

    public AM_AssetChangeDumper(AM_AppRefGuard appRefGuard)
    {
        _AppRefGuard = appRefGuard;
    }

    void Reset()
    {
        _ChangeList.Clear();
    }

    public List<string> DumpChangeAssetList( bool quietly,Dictionary<string, int> obList)
    {
        Reset();
        if(null != obList)
        {
            int index = 0;
            foreach(string changeOb in obList.Keys)
            {
                ++index;
                if(!quietly)
                {
                    EditorUtility.DisplayProgressBar("收集变更资源信息", changeOb, (float)index / obList.Count);
                }
                string changeAsset = changeOb.Replace(AM_UIPrefabExporter.GUIPrefab_EditorPath, AM_UIPrefabExporter.GUIPrefab_ExportPath);
                DumpChangeList(quietly, changeAsset);
            }
            if(!quietly)
            {
                EditorUtility.ClearProgressBar();
            }
        }
        return _ChangeList;
    }

    public List<string> DumpChangeAssetList(bool quietly, List<Object> obList)
    {
        Reset();
        if(null != obList)
        {
            for (int index = 0; index < obList.Count; ++index)
            {
                DumpChangeList(quietly, obList[index]);
            }
        }
        return _ChangeList;
    }

    public List<string> DumpChangeAssetListForSelectioin(bool quietly)
    {
        Reset();
        Object[] selections = Selection.objects;
        for (int index = 0; index < selections.Length; ++index)
        {
            DumpChangeList(quietly, selections[index]);
        }
        return _ChangeList;
    }

    void DumpChangeList(bool quietly, Object ob)
    {
        if (null != ob)
        {
            string ap = AssetDatabase.GetAssetPath(ob);
            DumpChangeList(quietly, ap);
        }
    }

    void DumpChangeList(bool quietly, string assetPath)
    {
        DumpDepen( quietly, assetPath);
        Dictionary<string, int> inverseDepenDic;
        if (_AppRefGuard.GetInverseDependenceDic(assetPath, out inverseDepenDic))
        {
            foreach(string  inversDepen in inverseDepenDic.Keys)
            {
                DumpChangeList(quietly, inversDepen);
            }
        }
    }

    void DumpDepen( bool quietly, string ap)
    {
        if (!string.IsNullOrEmpty(ap))
        {
            if (_AppRefGuard.RefByApp(ap))
            {
                string[] depens = AssetDatabase.GetDependencies(ap, true);
                for (int index = 0; index < depens.Length; ++index)
                {
                    if (!quietly)
                    {
                        EditorUtility.DisplayProgressBar("收集变更资源依赖", depens[index], (float)index / depens.Length);
                    }
                    if (depens[index].Contains(".cs"))
                    {
                        continue;
                    }
                    AddToChangeList(depens[index]);
                }
                if(!quietly)
                {
                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                Debug.LogError("not referenced by app:" + ap);
            }
        }
    }

    void AddToChangeList(string depen)
    {
        if (!_ChangeList.Contains(depen))
        {
            _ChangeList.Add(depen);
        }
    }
}
