// TODO：确保此文件中使用到的类型已经存在
public class GUI_BreadItem : GUI_TrainItem
{
    public UnityEngine.UI.Text StarNum = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_BreadItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_BreadItem_DL>(gameObject, this);
#endif
    }
}