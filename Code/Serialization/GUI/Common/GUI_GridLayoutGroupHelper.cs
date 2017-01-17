// TODO：确保此文件中使用到的类型已经存在
public sealed class GUI_GridLayoutGroupHelper : GUI_GroupLayoutHelper
{
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_GridLayoutGroupHelper_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GridLayoutGroupHelper_DL>(gameObject, this);
#endif
    }
}