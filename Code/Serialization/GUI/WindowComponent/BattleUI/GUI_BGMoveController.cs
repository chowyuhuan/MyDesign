// TODO：确保此文件中使用到的类型已经存在
public class GUI_BGMoveController : UnityEngine.MonoBehaviour
{
    public UnityEngine.Transform _LeftCullPoint = null;
    public UnityEngine.Transform _RightCullPoint = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_BGMoveController_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_BGMoveController_DL>(gameObject, this);
#endif
    }
}