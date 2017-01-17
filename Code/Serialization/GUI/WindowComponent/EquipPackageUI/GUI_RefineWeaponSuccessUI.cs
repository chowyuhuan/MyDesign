using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_RefineWeaponSuccessUI : GUI_Window {
    public GUI_EquipSimpleInfo RefinedEquip;
    public GameObject RefinedEquipStarbar;
    public Button ConfirmButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_RefineWeaponSuccessUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_RefineWeaponSuccessUI_DL>(gameObject, this);
#endif
    }
}
