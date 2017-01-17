// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroInfo : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Text _LevelInfoText = null;
    public UnityEngine.UI.Slider _HpSlider = null;
    public UnityEngine.UI.Slider _SpSlider = null;
    public UnityEngine.UI.Image _GrayMask = null;
    public UnityEngine.UI.Image _CaptainIcon = null;
    public UnityEngine.UI.Image _HeadIcon = null;
    public UnityEngine.UI.Text _StarNum = null;
    public UnityEngine.UI.Text _CurHpText = null;
    public UnityEngine.UI.Text _CurSpText = null;
    public UnityEngine.GameObject DCInfoObject = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroInfo_DL>(gameObject, this);
#endif
    }
}