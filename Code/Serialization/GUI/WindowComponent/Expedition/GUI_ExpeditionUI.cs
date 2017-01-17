using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ExpeditionUI : GUI_Window {
    public Text MissionName = null;
    public GUI_ItemSimpleInfo FixAward1 = null;
    public GUI_ItemSimpleInfo FixAward2 = null;
    public GUI_ItemSimpleInfo FixAward3 = null;
    public Text HeroGroupExp = null;
    public Text HeroExp = null;

    public GUI_ExpeditionRandomAwardBox RandomBox1 = null;
    public GUI_ExpeditionRandomAwardBox RandomBox2 = null;
    public GUI_ExpeditionRandomAwardBox RandomBox3 = null;
    public GUI_ColorfulSlider ConditionProgress = null;

    public List<GUI_ExpeditionRandomAwardCondition> ConditionList = new List<GUI_ExpeditionRandomAwardCondition>(10);
    public GameObject FilterOptionRootObject = null;
    public List<Toggle> SchoolFilterList = new List<Toggle>();
    public List<Toggle> StarFilterList = new List<Toggle>();
    public List<Toggle> TypeFilterList = new List<Toggle>();
    public List<Toggle> NationalityFilterList = new List<Toggle>();

    public Text ExpeditionTime = null;
    public Text HeroCount = null;
    public Text RemindText = null;
    public GameObject RemindMark = null;
    public GameObject ExpeditioningMask = null;
    public List<Image> SelectedHeroIcons = new List<Image>(4);
    public List<Button> SelectedHeroIconButton = new List<Button>(4);

    public GameObject HeroScrollPage = null;

    public GameObject PrepareButtonRoot = null;
    public Button SelectMenu = null;
    public Button OrderDirection = null;
    public Button RecommondButton = null;
    public Button BeginButton;

    public GameObject ExpeditioningButtonRoot = null;
    public Button AbortExpeditionButton = null;
    public Button QuickExpeditionButton = null;
    public Text QuickExpeditionCost = null;
    public GameObject ExpeditionFinishButtonRoot = null;
    public Button ExpeditionFinishButton = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_AdventureUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExpeditionUI_DL>(gameObject, this);
#endif
    }
}
