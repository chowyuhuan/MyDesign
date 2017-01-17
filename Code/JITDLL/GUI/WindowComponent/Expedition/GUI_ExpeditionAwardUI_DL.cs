using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ExpeditionAwardUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ExpeditionAwardUI dataComponent = gameObject.GetComponent<GUI_ExpeditionAwardUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExpeditionAwardUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            AreaName = dataComponent.AreaName;
            ExpeditionTime = dataComponent.ExpeditionTime;
            HeroTransTemplate = dataComponent.HeroTransTemplate;
            HeroExpSimulateRate = dataComponent.HeroExpSimulateRate;
            HeroAction = dataComponent.HeroAction;
            RegimentLevelUpRoot = dataComponent.RegimentLevelUpRoot;
            RegimentGroupLevel = dataComponent.RegimentGroupLevel;
            RegimentGroupExp = dataComponent.RegimentGroupExp;
            FixAward1 = dataComponent.FixAward1;
            FixAward2 = dataComponent.FixAward2;
            FixAward3 = dataComponent.FixAward3;
            RandomBox1 = dataComponent.RandomBox1;
            RandomBox2 = dataComponent.RandomBox2;
            RandomBox3 = dataComponent.RandomBox3;
            RandomAward1 = dataComponent.RandomAward1;
            RandomAward2 = dataComponent.RandomAward2;
            RandomAward3 = dataComponent.RandomAward3;
            HeroInfoObjectList = dataComponent.HeroInfoList;
        }
    }
    #endregion

    #region window logic
    public Text AreaName;
    public Text ExpeditionTime;

    public GUI_Transform HeroTransTemplate;
    public float HeroExpSimulateRate;
    public string HeroAction;

    public GameObject RegimentLevelUpRoot;
    public Text RegimentGroupLevel;
    public Slider RegimentGroupExp;
    public GUI_ItemSimpleInfo FixAward1;
    public GUI_ItemSimpleInfo FixAward2;
    public GUI_ItemSimpleInfo FixAward3;

    public GUI_ExpeditionRandomAwardBox RandomBox1;
    public GUI_ExpeditionRandomAwardBox RandomBox2;
    public GUI_ExpeditionRandomAwardBox RandomBox3;

    public GUI_RootedItemSimpleInfo RandomAward1;
    public GUI_RootedItemSimpleInfo RandomAward2;
    public GUI_RootedItemSimpleInfo RandomAward3;

    List<GameObject> HeroInfoObjectList;
    public List<GUI_HeroBonusSimpleInfo_DL> HeroInfoList;

    DataCenter.Expedition Expedition;
    CSV_b_expedition_quest_template MissionTemplate;
    CSV_b_expedition_random_award RandomAwardTemplate;
    DataCenter.GroupAddExpInfo GroupAddExp;
    List<DataCenter.HeroAddExpInfo> HeroAddExpList;
    List<DataCenter.AwardInfo> ExtraAwardList;

    public void ShowAwardInfo(DataCenter.Expedition expeiditon, CSV_b_expedition_quest_template missionTemplate, DataCenter.GroupAddExpInfo groupAddExp, List<DataCenter.HeroAddExpInfo> heroAddExpList, List<DataCenter.AwardInfo> extraAwardList)
    {
        Expedition = expeiditon;
        MissionTemplate = missionTemplate;
        RandomAwardTemplate = expeiditon.extraCSV;
        GroupAddExp = groupAddExp;
        HeroAddExpList = heroAddExpList;
        ExtraAwardList = extraAwardList;
        if (null == Expedition || null == MissionTemplate)
        {
            HideWindow();
        }
    }

    void OnEnable()
    {
        GUI_Root_DL.Instance.ShowLayer("Default");
    }

    void OnDisable()
    {
        GUI_Root_DL.Instance.HideLayer("Default");
    }

    protected override void OnStart()
    {
        InitBaseInfo();
        InitHeroInfo();
        StartCoroutine("RegimentExpGrowUp");
        InitFixAwardInfo();
        InitRandomAwardBox();
        InitRandomAwardInfo();
    }

    void InitBaseInfo()
    {
        CSV_b_expedition_template areaTemplate = CSV_b_expedition_template.FindData(MissionTemplate.QuestGroup);
        if (null != areaTemplate)
        {
            AreaName.text = string.Format("[{0}]{1}", areaTemplate.Name, MissionTemplate.Name);
        }

        int exp = CSV_b_hero_group_template.GetCurLevelExp((int)DataCenter.PlayerDataCenter.Exp, DataCenter.PlayerDataCenter.Level);
        int levelExp = CSV_b_hero_group_template.GetLevelGrowUpExp(DataCenter.PlayerDataCenter.Level);
    }

    void InitHeroInfo()
    {
        HeroInfoList = new List<GUI_HeroBonusSimpleInfo_DL>();
        for (int index = 0; index < HeroInfoObjectList.Count; ++index)
        {
            HeroInfoList.Add(HeroInfoObjectList[index].GetComponent<GUI_HeroBonusSimpleInfo_DL>());
        }

        for (int index = 0; index < HeroAddExpList.Count && index < HeroInfoList.Count; ++index)
        {
            HeroInfoList[index].SetHeroInfo(HeroAddExpList[index], HeroTransTemplate, HeroAction, HeroExpSimulateRate);
            HeroInfoList[index].SimulateExpUp(null);
        }
    }

    void InitFixAwardInfo()
    {
        GUI_Tools.ItemTool.SetAwardItemInfo(MissionTemplate.FixAwardType1, null, FixAward1.Name_Star_Count, FixAward1.Icon, MissionTemplate.FixAwardValue1);
        GUI_Tools.ItemTool.SetAwardItemInfo(MissionTemplate.FixAwardType2, null, FixAward2.Name_Star_Count, FixAward2.Icon, MissionTemplate.FixAwardValue2);
        GUI_Tools.ItemTool.SetAwardItemInfo(MissionTemplate.FixAwardType3, null, FixAward3.Name_Star_Count, FixAward3.Icon, MissionTemplate.FixAwardValue3);
    }

    void InitRandomAwardInfo()
    {
        switch(RandomAwardTemplate.ExtraChests.Count)
        {
            case 1 :
                {
                    SetRandomAwardInfo(RandomAward1, null, false);
                    SetRandomAwardInfo(RandomAward2, ExtraAwardList.Count > 0 ? ExtraAwardList[0] : null, true);
                    SetRandomAwardInfo(RandomAward3, null, false);
                    break;
                }
            case 2:
                {
                    SetRandomAwardInfo(RandomAward1, ExtraAwardList.Count > 0 ? ExtraAwardList[0] : null, true);
                    SetRandomAwardInfo(RandomAward2, null, false);
                    SetRandomAwardInfo(RandomAward3, ExtraAwardList.Count > 1 ? ExtraAwardList[1] : null, true);
                    break;
                }
            case 3:
                {
                    SetRandomAwardInfo(RandomAward1, ExtraAwardList.Count > 0 ? ExtraAwardList[0] : null, true);
                    SetRandomAwardInfo(RandomAward2, ExtraAwardList.Count > 1 ? ExtraAwardList[1] : null, true);
                    SetRandomAwardInfo(RandomAward3, ExtraAwardList.Count > 2 ? ExtraAwardList[2] : null, true);
                    break;
                }
        }
    }

    void SetRandomAwardInfo(GUI_RootedItemSimpleInfo rootedItem, DataCenter.AwardInfo awardInfo, bool showAwardBox)
    {
        if (null != rootedItem)
        {
            GUI_Tools.ObjectTool.ActiveObject(rootedItem.BgRoot, showAwardBox);
            if (null != awardInfo)
            {

                GUI_Tools.ObjectTool.ActiveObject(rootedItem.InfoRoot, true);
                GUI_Tools.ItemTool.SetAwardItemInfo((int)awardInfo.AwardType, null, rootedItem.Item.Name_Star_Count, rootedItem.Item.Icon, (int)awardInfo.AwardValue);
            }
            else
            {
                GUI_Tools.ObjectTool.ActiveObject(rootedItem.InfoRoot, false);
            }
        }
    }

    void InitRandomAwardBox()
    {
        if (null != RandomAwardTemplate)
        {
            int randomAwardCount = GetRandomAwardBoxCount();
            switch (randomAwardCount)
            {
                case 1:
                    {
                        SetRandomAwardBox(0, null, null, RandomBox1);
                        SetRandomAwardBox(RandomAwardTemplate.ConditionCount1, RandomAwardTemplate.BoxAtlas1, RandomAwardTemplate.BoxIcon1, RandomBox2);
                        SetRandomAwardBox(0, null, null, RandomBox3);
                        break;
                    }
                case 2:
                    {
                        SetRandomAwardBox(RandomAwardTemplate.ConditionCount1, RandomAwardTemplate.BoxAtlas1, RandomAwardTemplate.BoxIcon1, RandomBox1);
                        SetRandomAwardBox(0, null, null, RandomBox2);
                        SetRandomAwardBox(RandomAwardTemplate.ConditionCount2, RandomAwardTemplate.BoxAtlas2, RandomAwardTemplate.BoxIcon2, RandomBox3);
                        break;
                    }
                case 3:
                    {
                        SetRandomAwardBox(RandomAwardTemplate.ConditionCount1, RandomAwardTemplate.BoxAtlas1, RandomAwardTemplate.BoxIcon1, RandomBox1);
                        SetRandomAwardBox(RandomAwardTemplate.ConditionCount2, RandomAwardTemplate.BoxAtlas2, RandomAwardTemplate.BoxIcon2, RandomBox2);
                        SetRandomAwardBox(RandomAwardTemplate.ConditionCount3, RandomAwardTemplate.BoxAtlas3, RandomAwardTemplate.BoxIcon3, RandomBox3);
                        break;
                    }
                default:
                    {
                        SetRandomAwardBox(0, null, null, RandomBox1);
                        SetRandomAwardBox(0, null, null, RandomBox2);
                        SetRandomAwardBox(0, null, null, RandomBox3);
                        break;
                    }
            }

        }
    }

    int GetRandomAwardBoxCount()
    {
        int count = 0;
        if (null != RandomAwardTemplate)
        {
            if (RandomAwardTemplate.ConditionCount1 > 0)
            {
                ++count;
            }
            if (RandomAwardTemplate.ConditionCount2 > 0)
            {
                ++count;
            }
            if (RandomAwardTemplate.ConditionCount3 > 0)
            {
                ++count;
            }
        }
        return count;
    }

    void SetRandomAwardBox(int condtionCount, string boxAtlas, string boxIcon, GUI_ExpeditionRandomAwardBox awardBox)
    {
        if (null != awardBox)
        {
            if (condtionCount > 0)
            {
                GUI_Tools.ObjectTool.ActiveObject(awardBox.RootObject, true);
                GUI_Tools.IconTool.SetIcon(boxAtlas, boxIcon, awardBox.AwardBox.Item);
            }
            else
            {
                GUI_Tools.ObjectTool.ActiveObject(awardBox.RootObject, false);
            }
        }
    }

    IEnumerator RegimentExpGrowUp()
    {
        RegimentLevelUp(false);
        DataCenter.GroupAddExpInfo regimentExpInfo = GroupAddExp;
        int curExp = (int)regimentExpInfo.StartExp;
        int endExp = (int)regimentExpInfo.EndExp;
        int totalGrowExp = endExp - curExp;
        uint curLevel = regimentExpInfo.StartLevel;
        uint endLevel = regimentExpInfo.EndLevel;
        int curLevelGrowExp;
        int curLevelExp;
        CSV_b_hero_group_template curHeroGroupTemplate = CSV_b_hero_group_template.FindData((int)curLevel);
        while (curExp < endExp)
        {
            int growStep = (int)(totalGrowExp * HeroExpSimulateRate);
            curExp += growStep;
            if (curExp > endExp)
            {
                curExp = endExp;
            }
            if (curExp > curHeroGroupTemplate.Exp)
            {
                ++curLevel;
                RegimentLevelUp(true);
                if (curLevel > endLevel)
                {
                    curLevel = endLevel;
                }
            }
            curLevelGrowExp = CSV_b_hero_group_template.GetLevelGrowUpExp(curLevel);
            curLevelExp = CSV_b_hero_group_template.GetCurLevelExp(curExp, curLevel);
            RegimentGroupLevel.text = curLevel.ToString();
            RegimentGroupExp.value = Mathf.Clamp01((float)curLevelExp / (float)curLevelGrowExp);
            yield return null;
        }

        curLevelGrowExp = CSV_b_hero_group_template.GetLevelGrowUpExp(endLevel);
        curLevelExp = CSV_b_hero_group_template.GetCurLevelExp(endExp, endLevel);
        RegimentGroupLevel.text = endLevel.ToString();
        RegimentGroupExp.value = Mathf.Clamp01((float)curLevelExp / (float)curLevelGrowExp);
    }

    void RegimentLevelUp(bool levelUp)
    {
        RegimentLevelUpRoot.SetActive(levelUp);
    }
    #endregion
}
