using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ReformInfoUI : GUI_Window {
    public GameObject VerticalGroupHelperObject = null;
    public List<GameObject> ReformInfoPageObjectList = new List<GameObject>();
    public Button BigSuccessButton = null;
    public Text SwitchButtonText = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ReformInfoUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ReformInfoUI_DL>(gameObject, this);
#endif
    }
}
