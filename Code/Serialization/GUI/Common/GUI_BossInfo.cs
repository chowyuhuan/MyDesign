// TODO：确保此文件中使用到的类型已经存在
public class GUI_BossInfo : GUI_ActorBattleSimpleInfo
{
    public System.Collections.Generic.List<UnityEngine.UI.Image> AttackInfoIcon = new System.Collections.Generic.List<UnityEngine.UI.Image>();
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_BossInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_BossInfo_DL>(gameObject, this);
#endif
    }
}