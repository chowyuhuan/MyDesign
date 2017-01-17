using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ReformWeaponUI : GUI_Window {
    public GUI_EquipSimpleInfo EquipInfo = null;
    public Text EquipName = null;
    public GUI_EquipBasePropertyInfo AttackValue = null;
    public GUI_EquipBasePropertyInfo AttackSpeed = null;
    public Color NonBigSuccessColor = Color.white;
    public Color BigSuccessColor = Color.white;
    public List<GUI_EquipWordReformInfo> EquipWordInfoList = new List<GUI_EquipWordReformInfo>();
    public Button ResetReformPropertyButton;
    public Button ReformInfoButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ReformWeaponUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ReformWeaponUI_DL>(gameObject, this);
#endif
    }
}
