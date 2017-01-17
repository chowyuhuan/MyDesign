using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_SellWeaponAlertUI : GUI_Window {
    public Text AlertContent;
    public Button ConfirmSellButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SellWeaponAlertUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SellWeaponAlertUI_DL>(gameObject, this);
#endif
    }
}
