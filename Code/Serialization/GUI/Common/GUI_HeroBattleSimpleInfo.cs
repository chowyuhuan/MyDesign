// TODO：确保此文件中使用到的类型已经存在
public class GUI_HeroBattleSimpleInfo : GUI_ActorBattleSimpleInfo
{
    public UnityEngine.GameObject CaptainAlertIcon = null;
    public UnityEngine.GameObject CaptainTagIcon = null;
    public UnityEngine.UI.Button ItemButton;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_HeroBattleSimpleInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_HeroBattleSimpleInfo_DL>(gameObject, this);
#endif
    }
}