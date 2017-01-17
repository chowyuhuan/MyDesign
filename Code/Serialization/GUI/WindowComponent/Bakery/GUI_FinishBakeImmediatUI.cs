using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_FinishBakeImmediatUI : GUI_Window
{
    public Text Name;
    public Image IconImage;
    public Text Product;
    public Text BakeScheduleCount;
    public Slider BakeSchedule;
    public Text BakeScheduleText;
    public Text CostCount;

    public Button FinishImmediatButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_FinishBakeImmediatUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_FinishBakeImmediatUI_DL>(gameObject, this);
#endif
    }
}