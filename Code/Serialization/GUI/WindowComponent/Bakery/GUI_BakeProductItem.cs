// TODO：确保此文件中使用到的类型已经存在
public class GUI_BakeProductItem : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Text StarNum = null;
    public UnityEngine.UI.Image Icon = null;
    public UnityEngine.UI.Text Name = null;
    public int CountSize = 0;
    public UnityEngine.UI.Text AttributeValue = null;
    public UnityEngine.UI.Text BigSuccessRate = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_BakeProductItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_BakeProductItem_DL>(gameObject, this);
#endif
    }
}