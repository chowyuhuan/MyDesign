using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_GoddessHouseItem : GUI_ToggleItem {
    public Text GoddessName;
    public Image GoddessIcon;
    public GameObject GoddessLeaveTag;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_GoddessHouseItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GoddessHouseItem_DL>(gameObject, this);
#endif
    }
}
