using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_GetGoldCoinUI : GUI_Window {
    public Text GoldCoinCount = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_GetGoldCoinUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GetGoldCoinUI_DL>(gameObject, this);
#endif
    }
}
