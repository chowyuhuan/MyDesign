using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_AbortBakeUI : GUI_Window
{
    public Text Name;
    public Image IconImage;
    public Text Product;
    public Text BakeScheduleCount;
    public Slider BakeSchedule;
    public Text BakeScheduleText;
    public Button ConfirmAbortButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_AbortBakeUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_AbortBakeUI_DL>(gameObject, this);
#endif
    }
}
