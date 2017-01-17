using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_QuickExpeditionUI : GUI_Window {
    public Text AreaInfo = null;
    public Text RemainTime = null;
    public Slider ExpeditionSchedule = null;
    public Text MapPieceCount = null;
    public Button ConfirmButton = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_QuickExpeditionUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_QuickExpeditionUI_DL>(gameObject, this);
#endif
    }
}
