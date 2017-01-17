// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroManageSimpleInfo : GUI_HeroSimpleInfo
{
    public UnityEngine.GameObject TeamLeaderIcon = null;
    public UnityEngine.UI.Image UpdateIcon = null;
    public UnityEngine.GameObject ExtendMask = null;
    public UnityEngine.GameObject SafeLockMask = null;
    public UnityEngine.UI.Button ExtendButton = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroManageSimpleInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroManageSimpleInfo_DL>(gameObject, this);
#endif
    }
}