using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ExpeditionAwardUI : GUI_Window {
    public Text AreaName = null;
    public Text ExpeditionTime = null;

    public List<GameObject> HeroInfoList = new List<GameObject>();
    public GUI_Transform HeroTransTemplate = null;
    public float HeroExpSimulateRate = 0.02f;
    public string HeroAction = null;

    public GameObject RegimentLevelUpRoot;
    public Text RegimentGroupLevel;
    public Slider RegimentGroupExp;
    public GUI_ItemSimpleInfo FixAward1 = null;
    public GUI_ItemSimpleInfo FixAward2 = null;
    public GUI_ItemSimpleInfo FixAward3 = null;

    public GUI_ExpeditionRandomAwardBox RandomBox1 = null;
    public GUI_ExpeditionRandomAwardBox RandomBox2 = null;
    public GUI_ExpeditionRandomAwardBox RandomBox3 = null;

    public GUI_RootedItemSimpleInfo RandomAward1 = null;
    public GUI_RootedItemSimpleInfo RandomAward2 = null;
    public GUI_RootedItemSimpleInfo RandomAward3 = null;

    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_ExpeditionAwardUI_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_ExpeditionAwardUI_DL>(gameObject, this);
#endif
    }
}
