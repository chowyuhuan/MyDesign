using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_GoddessLeaveUI : GUI_Window {
    public string RemindTextFormater = null;
    public Text RemindText = null;
    public Image GoddessIcon = null;
    public Button ConfirmButton = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_GoddessLeaveUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GoddessLeaveUI_DL>(gameObject, this);
#endif
    }
}
