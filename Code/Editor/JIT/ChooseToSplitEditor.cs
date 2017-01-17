using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

public class ChooseToSplitEditor : ScriptableWizard
{
    public static Assembly assembly = null;
    [MenuItem("工具/脚本/热更新/分拆脚本",false,0)]
    static void CreateWindow()
    {
        assembly = Assembly.LoadFile(System.IO.Path.Combine(Application.dataPath, "../Library/ScriptAssemblies/Assembly-CSharp.dll"));
        if (assembly == null)
        {
            UnityEngine.Debug.LogError(LogTag.JIT + "加载Assembly-CSharp.dll失败");
        }
        else
        {
            ScriptableWizard.DisplayWizard<ChooseToSplitEditor>("分拆脚本", "确定");
        }
    }

    void OnWizardUpdate()
    {
        RefreshGUI();
    }

    void OnWizardCreate()
    {
        SplitTool.SplitScript(AssetDatabase.GetAssetPath(Selection.activeObject));
    }

    // 此接口在选择目录时，不被触发...
    void OnSelectionChange()
    {
        RefreshGUI();
    }

    void RefreshGUI()
    {
        bool selected = Selection.activeObject != null;
        if (!selected)
        {
            helpString = "请重新选择";
            errorString = "请选择脚本";
            isValid = false;
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        bool script = path.EndsWith(".cs");
        if (!script)
        {
            helpString = "请重新选择";
            errorString = "选择的对象不是脚本";
            isValid = false;
            return;
        }
        bool assets = path.IndexOf(SplitTool.JITDLLFolder) == 7 || path.IndexOf(SplitTool.SerializationFolder) == 7;
        if (!assets)
        {
            helpString = "请重新选择";
            errorString = "所选择的脚本，未在Assets/Scripts/JITDLL或者Assets/Scripts/Serialization下";
            isValid = false;
            return;
        }

        MonoScript tmpScript = Selection.activeObject as MonoScript;
        if(tmpScript != null)
        {
            if (!tmpScript.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
            {
                helpString = "请重新选择";
                errorString = "所选择的脚本，未继承MonoBehaviour";
                isValid = false;
                return;
            }
        }

        isValid = true;
        helpString = "确定要分拆: [" + Selection.activeObject.name + "]?";
        errorString = "";
    }
}
