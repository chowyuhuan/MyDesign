// TODO：确保此文件中使用到的类型已经存在
public class GUI_SkillCubeItem : UnityEngine.MonoBehaviour
{
    public int _SkillId = 0;
    public int _HeroBattleId = 0;
    public int _GroupId = 0;
    public bool _SpacialSkill = false;
    public UnityEngine.UI.Image _SkillIcon = null;
    public UnityEngine.UI.Image _SkillBGBox = null;
    public string _BGBoxAtlasName = null;
    public System.Collections.Generic.List<System.String> _BGBoxName = new System.Collections.Generic.List<System.String>();
    public string _SpecialBGBoxAtlasName = null;
    public System.Collections.Generic.List<System.String> _SpecialBGBoxName = new System.Collections.Generic.List<System.String>();
    public UnityEngine.UI.Image GreyMask = null;
    public bool Tweening = false;
    public int _CurIndex = 7;
    public UnityEngine.UI.Button ItemButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SkillCubeItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SkillCubeItem_DL>(gameObject, this);
#endif
    }
}