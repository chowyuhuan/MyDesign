using UnityEngine;
using System.Collections;


public class GUI_EvolutionAnimUI : GUI_Window
{
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_EvolutionAnimUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_EvolutionAnimUI_DL>(gameObject, this);
#endif
    }
}
