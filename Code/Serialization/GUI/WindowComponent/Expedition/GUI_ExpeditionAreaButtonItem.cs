using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ExpeditionAreaButtonItem : GUI_ToggleItem {
    public Image ButtonIcon = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ExpeditionAreaButtonItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExpeditionAreaButtonItem_DL>(gameObject, this);
#endif
    }
}
