using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ExpeditionEntryUI : GUI_Window {
    public Text ExpeditionTitle = null;
    public Text ExpeditionDialog = null;
    public Button ExpeditionButton = null;
    public Text ExpeditionButtonText = null;
    public GameObject ExpeditionMissionList = null;
    public ToggleGroup MissionGroup = null;
    public List<GameObject> AreaButtonList = new List<GameObject>();
    public ScrollRect MapRect = null;
    public float UnitPerSecond = 2f;
    public RectTransform AreaFocusPos = null;
    public GUI_TweenPosition MissionListTweener = null;
    public Button MissionListTweenerButton = null;
    public RectTransform TweenerTrans = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ExpeditionEntryUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExpeditionEntryUI_DL>(gameObject, this);
#endif
    }
}
