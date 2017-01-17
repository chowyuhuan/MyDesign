using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_CommonTaskDisplayUI : GUI_Window {
    public Text TypeName;
    public Text TaskName;
    public Text TaskDescription;
    public Text ScheduleText;
    public Slider ScheduleSlider;
    public Image AwardIcon;
    public Text AwardCount;
    public Button RefuseButton;
    public Button ChangeButton;
    public Text ChangeText;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_CommonTaskDisplayUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_CommonTaskDisplayUI_DL>(gameObject, this);
#endif
    }
}
