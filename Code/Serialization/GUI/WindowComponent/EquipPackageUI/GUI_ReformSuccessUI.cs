using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ReformSuccessUI : GUI_Window {
    public Text EquipName;
    public Image EquipIcon;
    public GameObject StarBar;
    public Text ReformWordProperty;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ReformSuccessUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ReformSuccess_DL>(gameObject, this);
#endif
    }
}
