using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_RefineWeaponUI : GUI_Window {
    public GUI_EquipSimpleInfo CurrentRefineEquip;
    public GUI_EquipSimpleInfo RefinedEquip;
    public GameObject CurrentEquipStarbar;
    public GameObject RefinedEquipStarbar;
    public Text RefineMat_Crystal;
    public Text RefineMat_Iron;
    public Text RefineCost_GoldCoin;
    public Button ConfirmButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_RefineWeaponUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_RefineWeaponUI_DL>(gameObject, this);
#endif
    }
}
