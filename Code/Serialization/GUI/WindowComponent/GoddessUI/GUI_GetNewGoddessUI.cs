using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_GetNewGoddessUI : GUI_Window {
    public string RemindTextFormater = null;
    public Text RemindText = null;
    public Image GoddessIcon = null;
    public Button ConfirmButton = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_GetNewGoddessUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GetNewGoddessUI_DL>(gameObject, this);
#endif
    }
}
