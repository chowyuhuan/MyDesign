using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_RefuseTaskUI : GUI_Window {
    public Button ConfirmRefuseButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_RefuseTaskUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_RefuseTaskUI_DL>(gameObject, this);
#endif
    }
}
