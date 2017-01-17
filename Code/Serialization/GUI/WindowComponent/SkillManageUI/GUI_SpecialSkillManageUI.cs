using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_SpecialSkillManageUI : GUI_Window
{
    public Text GotHeroType;
    public List<GameObject> HeroTabPageObjectList = new List<GameObject>();
    public GameObject HeroScrollPageObject = null;
    public GameObject SkillScrollPageObject = null;
    public ToggleGroup HeroToggleGroup = null;
    public ToggleGroup SkillToggleGroup = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SpecialSkillManageUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SpecialSkillManageUI_DL>(gameObject, this);
#endif
    }
}
