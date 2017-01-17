// TODO：确保此文件中使用到的类型已经存在
public abstract class GUI_HeroSimpleInfo : GUI_ToggleItem
{
    public UnityEngine.UI.Text Level = null;
    public UnityEngine.UI.Image SchoolIcon = null;
    public bool WeaponMax = false;
    public UnityEngine.UI.Text TrainingLevel = null;
    public UnityEngine.UI.Image HeadIcon = null;
    public UnityEngine.GameObject HeroStar = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroSimpleInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroSimpleInfo_DL>(gameObject, this);
#endif
    }
}