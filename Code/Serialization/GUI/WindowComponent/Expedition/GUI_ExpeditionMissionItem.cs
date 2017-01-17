using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ExpeditionMissionItem : GUI_ToggleItem {
    public Text ExpeditionStateText = null;
    public Image ExpeditionIcon = null;
    public Text MissionName = null;
    public List<GameObject> ExpeditionHeroList = new List<GameObject>();
    public Text ExpeditionScheduleText = null;
    public Slider ExpeditionScheduleValue = null;
    public GameObject FinishedTag = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ExpeditionMissionItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExpeditionMissionItem_DL>(gameObject, this);
#endif
    }
}
