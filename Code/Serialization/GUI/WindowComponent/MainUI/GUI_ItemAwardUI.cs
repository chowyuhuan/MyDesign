using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ItemAwardUI : GUI_Window {
    public Text ItemName = null;
    public GUI_ItemSimpleInfo ItemInfo = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ItemAwardUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ItemAwardUI_DL>(gameObject, this);
#endif
    }
}
