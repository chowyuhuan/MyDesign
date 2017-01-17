using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ResetWeaponPropertyUI : GUI_Window {
    public GUI_EquipBasePropertyInfo FirstProperty;
    public GUI_EquipBasePropertyInfo SecondProerty;
    public Button ConfirmButton;
    public Text ItemCost;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ResetWeaponPropertyUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ResetWeaponPropertyUI_DL>(gameObject, this);
#endif
    }
}
