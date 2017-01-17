using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ResolveAllWeaponUI : GUI_Window
{
    public Text SelectWeaponCount = null;
    public Text ResolveItemCount = null;
    public Toggle RemakeWeaponFilter = null;
    public Toggle QuenchingAndAncentWeaponFilter = null;
    public Button ConfirmButton = null;
    public List<Toggle> StarFilterList = new List<Toggle>();

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ResolveAllWeaponUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ResolveAllWeaponUI_DL>(gameObject, this);
#endif
    }
}
