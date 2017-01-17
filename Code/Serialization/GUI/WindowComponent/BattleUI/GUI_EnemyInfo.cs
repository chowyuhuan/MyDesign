// TODO：确保此文件中使用到的类型已经存在
public class GUI_EnemyInfo : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Text _HpNumber = null;
    public UnityEngine.UI.Text _Level = null;
    public UnityEngine.UI.Text _Name = null;
    public UnityEngine.UI.Text _Star = null;
    public UnityEngine.UI.Slider _HpSlider = null;
    public UnityEngine.UI.Slider _SpSlider = null;
    public UnityEngine.UI.Image _HeadIcon = null;
    public GUI_TweenAlpha _AlphaTweener = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_EnemyInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_EnemyInfo_DL>(gameObject, this);
#endif
    }
}