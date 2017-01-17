using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_SpecialSkillDisplayItem : GUI_ToggleItem {
    public GUI_SkillSimpleInfo SkillSimpleInfo;
    public Text SkillType;
    public Text SkillName;
    public Text ConstrainedHeroStar;
    public Text AttackIntentType;
    public Text Description;
    public Text CostHonor;
    public Text CostGold;
    public Text LevelCondtion;
    public Text LevelDescription;
    public Button GetConditionButton;
    public Button GetSkillButton;
    public Text GetSkillText;
    public GameObject LockMask;

    public GUI_ItemShrinkInfo ShrinkInfo = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_SpecialSkillDisplayItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_SpecialSkillDisplayItem_DL>(gameObject, this);
#endif
    }
}
