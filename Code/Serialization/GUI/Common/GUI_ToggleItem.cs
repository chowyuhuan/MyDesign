// TODO：确保此文件中使用到的类型已经存在
public abstract class GUI_ToggleItem : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject ExtendCheckMark = null;
    public int ToggleIndex = 0;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ToggleItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ToggleItem_DL>(gameObject, this);
#endif
    }
}