using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_SpecialSkillManageUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_SpecialSkillManageUI dataComponent = gameObject.GetComponent<GUI_SpecialSkillManageUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SpecialSkillManageUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GotHeroType = dataComponent.GotHeroType;
            HeroTabPageObjectList = dataComponent.HeroTabPageObjectList;
            HeroScrollPageObject = dataComponent.HeroScrollPageObject;
            SkillScrollPageObject = dataComponent.SkillScrollPageObject;
            HeroToggleGroup = dataComponent.HeroToggleGroup;
            SkillToggleGroup = dataComponent.SkillToggleGroup;
        }
    }
    #endregion

    #region window logic
    public Text GotHeroType;
    public List<GameObject> HeroTabPageObjectList;
    public GameObject HeroScrollPageObject;
    public GameObject SkillScrollPageObject;
    public ToggleGroup HeroToggleGroup;
    public ToggleGroup SkillToggleGroup;

    List<GUI_ToggleTabPage_DL> HeroTabPageList;
    GUI_VerticallayouGroupHelper_DL HeroScollPage;
    GUI_VerticallayouGroupHelper_DL SkillScrollPage;
    GUI_LogicObjectPool HeroBindSkillItemPool;
    GUI_LogicObjectPool SkillItemPool;

    int CurrentPageIndex = -1;
    DataCenter.Hero CurrentHero;
    DataCenter.SpecialSkillUnit CurrentSkill;

    public void ShowSpecialSkillAndFocusOn(DataCenter.Hero hero)
    {
        CurrentHero = hero;
    }

    protected override void OnStart()
    {
        InitTabPageList();
        if(HeroTabPageList.Count > 0)
        {
            HeroTabPageList[0].Select();
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnEquipSkill += OnHeroGetSkillRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnEquipSkill -= OnHeroGetSkillRsp;
    }

    void InitTabPageList()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Train_HeroSkill_Herolist", true, AssetManage.E_AssetType.UIPrefab);
        HeroBindSkillItemPool = new GUI_LogicObjectPool(go);

        go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Train_HeroSkill_Skilllist", true, AssetManage.E_AssetType.UIPrefab);
        SkillItemPool = new GUI_LogicObjectPool(go);

        HeroTabPageList = new List<GUI_ToggleTabPage_DL>();
        for(int index = 0; index < HeroTabPageObjectList.Count; ++index)
        {
            HeroTabPageList.Add(HeroTabPageObjectList[index].GetComponent<GUI_ToggleTabPage_DL>());
        }
        HeroScollPage = HeroScrollPageObject.GetComponent<GUI_VerticallayouGroupHelper_DL>();
        HeroScollPage.SetScrollAction(DisplayHeroBindItem);
        for (int index = 0; index < HeroTabPageList.Count; ++index)
        {
            HeroTabPageList[index].Init(index, HeroScollPage, OnSelectPage, null, true);
        }
        SkillScrollPage = SkillScrollPageObject.GetComponent<GUI_VerticallayouGroupHelper_DL>();
        SkillScrollPage.SetScrollAction(DisplaySkillItem);
    }

    void OnSelectPage(int pageIndex)
    {
        if (CurrentPageIndex != pageIndex)
        {
            CurrentPageIndex = pageIndex;
        }

        RefreshHeroBindSkillItem();
        RefreshSpecialSkillItem(null != CurrentHero);
    }

    void RefreshHeroBindSkillItem()
    {
        HeroScollPage.Clear();
        if (0 == CurrentPageIndex)
        {
            ShowAllHero();
        }
        else
        {
            ShowHeroBySchool(CurrentPageIndex);
        }
    }

    void ShowAllHero()
    {
        for(int index = 0; index < DataCenter.PlayerDataCenter.HeroList.Count; ++index)
        {
            HeroScollPage.FillItem((int)DataCenter.PlayerDataCenter.HeroList[index].ServerId);
        }
        HeroScollPage.FillItemEnd();
    }

    void ShowHeroBySchool(int school)
    {
        for (int index = 0; index < DataCenter.PlayerDataCenter.HeroList.Count; ++index)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(DataCenter.PlayerDataCenter.HeroList[index].CsvId);
            if(null != heroTemplate && heroTemplate.School == school)
            {
                HeroScollPage.FillItem((int)DataCenter.PlayerDataCenter.HeroList[index].ServerId);
            }
        }
        HeroScollPage.FillItemEnd();
    }

    void RefreshSpecialSkillItem(bool bySelectedHero)
    {
        SkillScrollPage.Clear();
        if(bySelectedHero && null != CurrentHero)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(CurrentHero.CsvId);
            if (null != heroTemplate)
            {
                ShowSkillBySchool(heroTemplate.School);
                return;
            }
        }
        ShowSkillBySchool(Mathf.Clamp(CurrentPageIndex, 1, 6));
    }

    void ShowSkillBySchool(int school)
    {
        List<DataCenter.SpecialSkillUnit> specialSkillUnit = DataCenter.PlayerDataCenter.SpecialSkillData.GetSpecialSkillList(school);
        for(int index = 0; index < specialSkillUnit.Count; ++index)
        {
            SkillScrollPage.FillItem((int)specialSkillUnit[index].GroupId);
        }
        SkillScrollPage.FillItemEnd();
    }

    void DisplayHeroBindItem(GUI_ScrollItem scrollItem)
    {
        if(null != scrollItem)
        {
            GUI_HeroBindSkillItem_DL heroBindSkillItem = HeroBindSkillItemPool.GetOneLogicComponent() as GUI_HeroBindSkillItem_DL;
            if(null != heroBindSkillItem)
            {
                DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero((uint)scrollItem.LogicIndex);
                scrollItem.SetTarget(heroBindSkillItem);
                heroBindSkillItem.ShowHero(hero, OnHeroBindSkillItemSelected, OnHeroBindSkillItemDeSelected, GetCurrentHero);
                heroBindSkillItem.RegistToGroup(HeroToggleGroup);
                if(null == CurrentHero)
                {
                    if (scrollItem.CachedTransform.GetSiblingIndex() == 0)
                    {
                        heroBindSkillItem.Select();
                    }
                }
                else
                {
                    if(hero.ServerId == CurrentHero.ServerId)
                    {
                        heroBindSkillItem.Select();
                    }
                }
            }
        }
    }

    void OnHeroGetSkillRsp(uint heroServerId, uint skillId, bool isBigSuccess)
    {
        GUI_MessageManager.Instance.ShowErrorTip(isBigSuccess ? "大成功" : "成功");
        HeroScollPage.RefreshDisplay();
        GUI_HeroGetNewSpecialSkillUI_DL heroNewSkill = GUI_Manager.Instance.ShowWindowWithName<GUI_HeroGetNewSpecialSkillUI_DL>("UI_HeroSkill_Message", false);
        CSV_b_skill_template skillTemplate = CSV_b_skill_template.FindData((int)skillId);
        if(null != heroNewSkill)
        {
            heroNewSkill.ShowNewSkill(skillTemplate);
        }
    }

    void OnHeroBindSkillItemSelected(GUI_HeroBindSkillItem_DL bindItem)
    {
        if(null != bindItem)
        {
            if(null == CurrentHero)
            {
                CurrentHero = bindItem.Hero;
                RefreshSpecialSkillItem(true);
            }
            else
            {
                CSV_b_hero_template curTem = CSV_b_hero_template.FindData(CurrentHero.CsvId);
                CSV_b_hero_template selectTem = CSV_b_hero_template.FindData(bindItem.Hero.CsvId);
                CurrentHero = bindItem.Hero;
                if (curTem.School != selectTem.School)
                {
                    RefreshSpecialSkillItem(true);
                }
            }

            SkillScrollPage.RefreshPageData();
        }
    }

    void OnHeroBindSkillItemDeSelected(GUI_HeroBindSkillItem_DL bindItem)
    {

    }

    void DisplaySkillItem(GUI_ScrollItem scrollItem)
    {
        if(null != scrollItem)
        {
            DataCenter.SpecialSkillUnit skillUnit = DataCenter.PlayerDataCenter.SpecialSkillData.GetSpecialSkill((uint)scrollItem.LogicIndex);
            if(null != skillUnit)
            {
                GUI_SpecialSkillDisplayItem_DL skillDisplayItem = SkillItemPool.GetOneLogicComponent() as GUI_SpecialSkillDisplayItem_DL;
                skillDisplayItem.DisplaySkill(scrollItem, skillUnit, OnSelectSkillItem, GetCurrentHero);
                skillDisplayItem.RegistToGroup(SkillToggleGroup);
                scrollItem.SetTarget(skillDisplayItem);
            }
        }
    }

    void OnSelectSkillItem(GUI_SpecialSkillDisplayItem_DL skillItem)
    {
        if(null != skillItem)
        {
            CurrentSkill = skillItem.SpecialSkillUnit;
        }
    }

    DataCenter.Hero GetCurrentHero()
    {
        return CurrentHero;
    }
    #endregion
}
