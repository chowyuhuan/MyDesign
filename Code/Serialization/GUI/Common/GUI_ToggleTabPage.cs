// TODO：确保此文件中使用到的类型已经存在
public class GUI_ToggleTabPage : GUI_ToggleItem
{
    public UnityEngine.UI.Text PageName = null;
    public UnityEngine.GameObject ScrollGroupObject = null;
    public UnityEngine.UI.ToggleGroup ItemToggleGroup = null;
    public bool UseDefaultSelect = false;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ToggleTabPage_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ToggleTabPage_DL>(gameObject, this);
#endif
    }
}