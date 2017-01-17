// TODO：确保此文件中使用到的类型已经存在
public class GUI_RegimentGroupPassiveItem : UnityEngine.MonoBehaviour
{
    public UnityEngine.UI.Image DisplayIcon = null;
    public UnityEngine.UI.Image EffectBuffIcon = null;
    public UnityEngine.UI.Text LevelText = null;
    public UnityEngine.UI.Text Name = null;
    public UnityEngine.UI.Image EffectDesIcon = null;
    public UnityEngine.UI.Text EffectDescription = null;
    public UnityEngine.UI.Text ConditionText = null;
    public UnityEngine.UI.Text ConditionScheduleText = null;
    public UnityEngine.UI.Slider ConditionSchedule = null;
    public UnityEngine.GameObject LockMask = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_RegimentGroupPassiveItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_RegimentGroupPassiveItem_DL>(gameObject, this);
#endif
    }
}