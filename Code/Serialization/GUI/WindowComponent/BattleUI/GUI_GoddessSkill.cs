using UnityEngine.UI;
// TODO：确保此文件中使用到的类型已经存在
public class GUI_GoddessSkill : UnityEngine.MonoBehaviour
{
    public Image HeadIcon = null;
    public Slider SPSlider = null;
    public Button SkillButton = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_GoddessSkill_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_GoddessSkill_DL>(gameObject, this);
#endif
    }
}