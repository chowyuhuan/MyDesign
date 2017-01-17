using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_FailLevelUI : GUI_LevelBonus
{
    public Button RetryButton;
    public Button ReturnCityButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_FailLevelUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_FailLevelUI_DL>(gameObject, this);
#endif
    }
}
