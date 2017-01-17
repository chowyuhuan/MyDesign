using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_SelectResolveTypeUI : GUI_Window {
    public Text MidLevelResolveCost;
    public Toggle NormalResolve;
    public Button ConfirmResolveButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SelectResolveTypeUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SelectResolveTypeUI_DL>(gameObject, this);
#endif
    }
}
