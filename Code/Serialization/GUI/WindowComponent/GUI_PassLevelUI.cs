using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_PassLevelUI : GUI_LevelBonus
{
    public Button RetryButton;
    public Button NextLevelButton;
    public Button ReturnCityButton;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_PassLevelUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_PassLevelUI_DL>(gameObject, this);
#endif
    }
}