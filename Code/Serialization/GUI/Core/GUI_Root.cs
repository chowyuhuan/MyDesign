// TODO：确保此文件中使用到的类型已经存在
public class GUI_Root : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject _RootObject = null;
    public UnityEngine.Camera _UICamera = null;
    public UnityEngine.UI.CanvasScaler _ScreenScaler = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_Root_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_Root_DL>(gameObject, this);
#endif
    }
}