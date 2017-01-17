using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_WarnNumberManager : MonoBehaviour
{
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_WarnNumberManager_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_WarnNumberManager_DL>(gameObject, this);
#endif
    }
}