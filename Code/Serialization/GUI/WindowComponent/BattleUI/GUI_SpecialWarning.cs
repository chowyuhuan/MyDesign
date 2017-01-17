// TODO：确保此文件中使用到的类型已经存在
public class GUI_SpecialWarning : UnityEngine.MonoBehaviour
{
    public float BossWarningTime = 0;
    public UnityEngine.GameObject BossWarning = null;
    public float HiddenWarningTime = 0;
    public UnityEngine.GameObject HiddenWarning = null;
    public float PassLevelEffectTime = 0;
    public UnityEngine.GameObject PassLevelEffect = null;
    public float FailLevelEffectTime = 0;
    public UnityEngine.GameObject FailLevelEffect = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SpecialWarning_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SpecialWarning_DL>(gameObject, this);
#endif
    }
}