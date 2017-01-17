using UnityEngine;
using System.Collections;

public sealed class GUI_CommonTask : MonoBehaviour {
    public GUI_CommonTaskInfo TaskUIInfo;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_CommonTask_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_CommonTask_DL>(gameObject, this);
#endif
    }
}
