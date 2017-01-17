using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_BakeDoneUI : GUI_Window
{
    public GameObject GridHelperObject;
    public Button ConfirmButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_BakeDoneUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_BakeDoneUI_DL>(gameObject, this);
#endif
    }
}