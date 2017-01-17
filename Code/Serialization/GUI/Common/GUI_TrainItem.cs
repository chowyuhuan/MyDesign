// TODO：确保此文件中使用到的类型已经存在
public abstract class GUI_TrainItem : GUI_ToggleItem
{
    public UnityEngine.UI.Image Icon = null;
    public UnityEngine.UI.Text Name = null;
    public UnityEngine.UI.Text AttributeValue = null;
    public UnityEngine.UI.Text BigSuccessRate = null;
    public UnityEngine.UI.Text SellPrice = null;
    public UnityEngine.GameObject SellPriceRoot = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_TrainItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_TrainItem_DL>(gameObject, this);
#endif
    }
}