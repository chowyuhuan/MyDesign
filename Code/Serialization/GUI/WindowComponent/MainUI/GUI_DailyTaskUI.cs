using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_DailyTaskUI : GUI_Window {
    public List<GameObject> TaskTabPageObjectList = new List<GameObject>();
    public GameObject TaskScrollPageObject = null;
    public ToggleGroup TaskToggleGroup = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_DailyTaskUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_DailyTaskUI_DL>(gameObject, this);
#endif
    }
}
