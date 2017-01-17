using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ExpeditionCandidateHeroInfo : GUI_ToggleItem {
    public Image HeroIcon = null;
    public Image TypeIcon = null;
    public Image SchoolIcon = null;
    public GameObject StarBar = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ExpeditionCandidateHeroInfo_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExpeditionCandidateHeroInfo_DL>(gameObject, this);
#endif
    }
}
