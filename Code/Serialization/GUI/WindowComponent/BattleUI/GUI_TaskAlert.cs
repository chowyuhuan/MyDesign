using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_TaskAlert : MonoBehaviour {
    public Text TaskType = null;
    public Text TaskName = null;
    public Text TaskState = null;
    public Text TaskDescription = null;
    public GUI_TweenPosition TweenIn = null;
    public GUI_TweenPosition TweenOut = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_TaskAlert_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_TaskAlert_DL>(gameObject, this);
#endif
    }
}
