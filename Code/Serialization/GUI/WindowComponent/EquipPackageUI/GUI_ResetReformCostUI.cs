using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ResetReformCostUI : GUI_Window {
    public GUI_EquipSimpleInfo EquipSimpleInfo;
    public Text EquipName;
    public Text ReformCost_Current;
    public Text ReformCost_Reset;
    public Text ItemCost;
    public Button ConfirmButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ResetReformCostUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ResetReformCostUI_DL>(gameObject, this);
#endif
    }
}
