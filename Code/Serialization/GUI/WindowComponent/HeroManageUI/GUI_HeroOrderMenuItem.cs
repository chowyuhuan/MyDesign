// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroOrderMenuItem : GUI_ToggleItem
{
    public UnityEngine.UI.Text MenuText = null;
    public E_Hero_OrderType OrderType = E_Hero_OrderType.Attack;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroOrderMenuItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroOrderMenuItem_DL>(gameObject, this);
#endif
    }
}