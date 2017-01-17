using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_AbortExpeditionUI : GUI_Window {
    public Text AreaInfo = null;
    public Text RemainTime = null;
    public Slider ExpeditionSchedule = null;
    public Button ConfirmButton = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_AbortExpeditionUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_AbortExpeditionUI_DL>(gameObject, this);
#endif
    }
}
