using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ResolveSuccessUI : GUI_Window {
    public Text SuccessTitle;
    public string SuccessString;
    public string BigSuccessString;
    public Button ConfirmSuccessButton;
    public GameObject AwardItemSpawnRoot;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ResolveSuccessUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ResolveSuccessUI_DL>(gameObject, this);
#endif
    }
}
