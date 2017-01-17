// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroSelectSimpleInfo : GUI_HeroSimpleInfo
{
    public UnityEngine.UI.Image SkillIcon = null;
    public UnityEngine.UI.Image SpecialSkillIcon = null;
    public UnityEngine.GameObject CaptainTag = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroSelectSimpleInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroSelectSimpleInfo_DL>(gameObject, this);
#endif
    }
}