using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_GoddessHouseUI : GUI_Window {
    public Text CurGoddessSkillName = null;
    public Text CurGoddessSkillDescription = null;
    public Text GoddessButtonText = null;
    public GameObject GoddessPageObject = null;
    public Button GoddessButton = null;
    public ToggleGroup GoddessGroup = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_GoddessHouseUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GoddessHouseUI_DL>(gameObject, this);
#endif
    }
}
