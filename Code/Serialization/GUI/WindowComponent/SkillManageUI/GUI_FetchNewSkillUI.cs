using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_FetchNewSkillUI : GUI_Window {
    public Image Icon = null;
    public Text Name = null;
    public Text Description = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_FetchNewSkillUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_FetchNewSkillUI_DL>(gameObject, this);
#endif
    }
}
