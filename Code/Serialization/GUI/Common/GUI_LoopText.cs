using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_LoopText : MonoBehaviour {
    public float LoopSpeed = 2f;
    public Text TargetText = null;
    public RectMask2D RectMask = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_LoopText_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_LoopText_DL>(gameObject, this);
#endif
    }
}
