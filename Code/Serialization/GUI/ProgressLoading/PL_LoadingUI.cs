using UnityEngine;
using UnityEngine.UI;

public class PL_LoadingUI : MonoBehaviour
{
    public Slider _ProgressBar = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"PL_LoadingUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<PL_LoadingUI_DL>(gameObject, this);
#endif
    }
}
