// TODO：确保此文件中使用到的类型已经存在
public class GUI_SkillCastWarn : UnityEngine.MonoBehaviour
{
    public int _WarnCamp = 0;
    public UnityEngine.UI.Text _WarnTitle = null;
    public UnityEngine.UI.Text _Description = null;
    public GUI_TweenPosition _SlideInTweener = null;
    public GUI_TweenPosition _SlideOutTweener = null;
    public UnityEngine.RectTransform _SkillNameTransform = null;
 
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SkillCastWarn_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SkillCastWarn_DL>(gameObject, this);
#endif
    }
}