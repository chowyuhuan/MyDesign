using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ResolveWeaponAlertUI : GUI_Window {
    public Text AlertContent;
    public Button ConfirmResovleButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ResolveWeaponAlertUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ResolveWeaponAlertUI_DL>(gameObject, this);
#endif
    }
}
