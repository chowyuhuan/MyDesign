// TODO：确保此文件中使用到的类型已经存在
public class GUI_ActorBattleSimpleInfo : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Image HeadIcon = null;
    public UnityEngine.UI.Image SchoolIcon = null;
    public UnityEngine.UI.Text LevelNumber = null;
    public UnityEngine.UI.Text StarNumber = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ActorBattleSimpleInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ActorBattleSimpleInfo_DL>(gameObject, this);
#endif
    }
}