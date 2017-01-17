using UnityEngine;

// TODO：确保此文件中使用到的类型已经存在
public class GUI_HpController : MonoBehaviour
{
    public GameObject _AttachDisplay = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HpController_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HpController_DL>(gameObject, this);
#endif
    }
}