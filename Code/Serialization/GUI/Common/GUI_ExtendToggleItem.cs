// TODO：确保此文件中使用到的类型已经存在
public class GUI_ExtendToggleItem : GUI_ToggleItem
{
    public UnityEngine.Events.UnityEvent OnSelectItem = null;
    public UnityEngine.Events.UnityEvent OnDeSelectItem = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ExtendToggleItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExtendToggleItem_DL>(gameObject, this);
#endif
    }
}