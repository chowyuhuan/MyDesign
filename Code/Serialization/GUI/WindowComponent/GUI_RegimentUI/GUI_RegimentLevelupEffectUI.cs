using UnityEngine;
using System.Collections;

public class GUI_RegimentLevelupEffectUI : GUI_Window {
    void Awake()
    {
#if JIT && !UNITY_IOS
        ScriptAssembly.Assemble(gameObject, "GUI_RegimentLevelupEffectUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_RegimentLevelupEffectUI_DL>(gameObject, this);
#endif
    }
}
