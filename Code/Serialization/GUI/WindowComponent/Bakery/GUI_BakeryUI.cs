using UnityEngine;
using System.Collections;

public sealed class GUI_BakeryUI : GUI_Window
{
    public GameObject GridHelperObject;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_BakeryUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_BakeryUI_DL>(gameObject, this);
#endif
    }
}