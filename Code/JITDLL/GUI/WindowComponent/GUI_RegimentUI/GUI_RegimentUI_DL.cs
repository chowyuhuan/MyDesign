using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_RegimentUI_DL : GUI_Window_DL
{
    #region window area
    GameObject RegimentPassiveObject;
    public GUI_ExtendToggleItem_DL RegimentPassive;
    protected override void OnStart()
    {
        RefreshRegimentInfo();
        InitRegimentPassiveArea();
        RegimentPassive = RegimentPassiveObject.GetComponent<GUI_ExtendToggleItem_DL>();
        RegimentPassive.Select();
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnChangeRepresentHero += OnChangeRegimentHero;
        DataCenter.PlayerDataCenter.OnGroupPassiveUpdate += OnRegimentPassiveUpdate;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnChangeRepresentHero -= OnChangeRegimentHero;
        DataCenter.PlayerDataCenter.OnGroupPassiveUpdate -= OnRegimentPassiveUpdate;
    }


    #endregion

    #region regiment info area
    public Text RegimentLevel;
    public Text RegimentExp;
    public Slider RegimentExpSchedule;
    public Text RegimentName;
    public Image RegimentIcon;

    void RefreshRegimentInfo()
    {
        RegimentLevel.text = DataCenter.PlayerDataCenter.Level.ToString();
        int exp = CSV_b_hero_group_template.GetCurLevelExp((int)DataCenter.PlayerDataCenter.Exp, DataCenter.PlayerDataCenter.Level);
        int levelExp = CSV_b_hero_group_template.GetLevelGrowUpExp(DataCenter.PlayerDataCenter.Level);
        RegimentExp.text = string.Format("{0}/{1}", exp.ToString(), levelExp.ToString());
        RegimentExpSchedule.value = Mathf.Clamp01((float)exp / levelExp);
        RegimentName.text = DataCenter.PlayerDataCenter.Nickname;
        OnChangeRegimentHero(DataCenter.PlayerDataCenter.RepresentHero);
    }

    void OnChangeRegimentHero(uint heroServerId)
    {
        DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(heroServerId);
        if (null != hero)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
            if (null != heroTemplate)
            {
                GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, RegimentIcon);
            }
        }
    }

    public void OnRegimentRepresentHeroClick()
    {
        GUI_HeroManage_DL heroManage = GUI_Manager.Instance.ShowWindowWithName<GUI_HeroManage_DL>("GUI_HeroManage", false) as GUI_HeroManage_DL;
        if (null != heroManage)
        {
            heroManage.OnSetLeaderHeroButtonClicked();
        }
    }

    public void OnRegimentLevelUpEffectClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_PassiveSkill_LVUPDetail", false);
    }
    #endregion

    #region regiment property
    public GameObject RegimentPropertyPage;
    public void OnRegimentPropertyPageSelect()
    {
        RegimentPropertyPage.SetActive(true);
        RefreshRegimentProperty();
    }

    public void OnRegimentPropertyPageDeSelect()
    {
        RegimentPropertyPage.SetActive(false);
    }

    /// <summary>
    /// shop item
    /// </summary>
    public Text GoldCoin;
    public Text Diamond;

    /// <summary>
    /// adventrue item
    /// </summary>
    public Text Stamina;

    /// <summary>
    /// arena item
    /// </summary>
    public Text Hornor;
    public Text ArenaTicket;

    /// <summary>
    /// Weapon item
    /// </summary>
    public Text Iron;
    public Text ExclusiveWeaponPropertyResetTicket;
    public Text CrystalPowder;
    public Text CrystalPiece;
    public Text CrystalRime;
        

    void RefreshRegimentProperty()
    {
        GoldCoin.text = DataCenter.PlayerDataCenter.Coin.ToString();
        Diamond.text = DataCenter.PlayerDataCenter.Diamond.ToString();

        Stamina.text = DataCenter.PlayerDataCenter.Stamina.ToString();

        Hornor.text = DataCenter.PlayerDataCenter.Honor.ToString();
        ArenaTicket.text = DataCenter.PlayerDataCenter.ArenaTicket.ToString();

        Iron.text = DataCenter.PlayerDataCenter.Iron.ToString();
        ExclusiveWeaponPropertyResetTicket.text = DataCenter.PlayerDataCenter.ResetTicket.ToString();
        CrystalPowder.text = DataCenter.PlayerDataCenter.CrystalPowder.ToString();
        CrystalPiece.text = DataCenter.PlayerDataCenter.CrystalPiece.ToString();
        CrystalRime.text = DataCenter.PlayerDataCenter.CrystalRime.ToString();
    }

    public void OnAddGoldCoinButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddDiamondButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddStaminaButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddHornorButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddArenaTicketButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddIronButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddExclusiveResetTicketButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddCrystalPowderButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddCrystalPieceButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddCrystalRimeButtonClick()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }
    #endregion

    #region regiment passive
    public GameObject RegimentPassivePage;
    GameObject GridHelperObject;
    public GUI_GridLayoutGroupHelper_DL RegimentPassiveSkillScollPage;
    public RectTransform RegimentPassiveSkillArea;
    GUI_LogicObjectPool _RegimentPassiveItemPool;

    void InitRegimentPassiveArea()
    {
        RegimentPassiveSkillScollPage = GridHelperObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        RegimentPassiveSkillScollPage.SetScrollAction(DisplayItem);
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Train_HeroGroup_Skill_Item", true, AssetManage.E_AssetType.UIPrefab);
        _RegimentPassiveItemPool = new GUI_LogicObjectPool(go);

        for (int index = 0; index < DataCenter.PlayerDataCenter.GroupPassiveList.Count; ++index)
        {
            DataCenter.GroupPassiveInfo passiveInfo = DataCenter.PlayerDataCenter.GroupPassiveList[index];
            int logicIndex = (int)(passiveInfo.PassiveType * GUI_Const.Config_Type_Distance + passiveInfo.PassiveLevel);
            RegimentPassiveSkillScollPage.FillItem(logicIndex);
        }
        RegimentPassiveSkillScollPage.FillItemEnd();
    }

    public void OnRegimentPassivePageSelect()
    {
        RegimentPassivePage.SetActive(true);
    }

    public void OnRegimentPassivePageDeSelect()
    {
        RegimentPassivePage.SetActive(false);
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(RegimentPassiveSkillArea, scrollItem.CachedTransform.position))
            {
                DisplayRegimentPassiveSkillItem(scrollItem);
            }
        }
    }

    void OnRegimentPassiveUpdate(DataCenter.GroupPassiveInfo passiveInfo)
    {
        if (null != passiveInfo)
        {
            int logicIndex = (int)(passiveInfo.PassiveType * GUI_Const.Config_Type_Distance + passiveInfo.PassiveLevel);
            for (int index = 0; index < RegimentPassiveSkillScollPage.ItemCount; ++index)
            {
                GUI_ScrollItem scrollItem = RegimentPassiveSkillScollPage.GetAtIndex(index);
                if (scrollItem.LogicIndex == logicIndex)
                {
                    GUI_RegimentGroupPassiveItem_DL regimentPassiveItem = scrollItem.LogicObject as GUI_RegimentGroupPassiveItem_DL;
                    if (null != regimentPassiveItem)
                    {
                        regimentPassiveItem.SetGroupPassvie(passiveInfo);
                    }
                }
            }
        }
    }

    void DisplayRegimentPassiveSkillItem(GUI_ScrollItem scrollItem)
    {
        int effectType = scrollItem.LogicIndex / GUI_Const.Config_Type_Distance;
        DataCenter.GroupPassiveInfo passiveInfo = GetGroupPassiveInfo((uint)effectType);
        if (null != passiveInfo)
        {
            GUI_RegimentGroupPassiveItem_DL passItem = _RegimentPassiveItemPool.GetOneLogicComponent() as GUI_RegimentGroupPassiveItem_DL;
            passItem.SetGroupPassvie(passiveInfo);
            scrollItem.SetTarget(passItem);
        }
    }

    DataCenter.GroupPassiveInfo GetGroupPassiveInfo(uint effectType)
    {
        for (int index = 0; index < DataCenter.PlayerDataCenter.GroupPassiveList.Count; ++index)
        {
            if (DataCenter.PlayerDataCenter.GroupPassiveList[index].PassiveType == effectType)
            {
                return DataCenter.PlayerDataCenter.GroupPassiveList[index];
            }
        }
        return null;
    }
    #endregion
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_RegimentUI dataComponent = gameObject.GetComponent<GUI_RegimentUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_RegimentUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            RegimentPassiveObject = dataComponent.RegimentPassive;
            RegimentLevel = dataComponent.RegimentLevel;
            RegimentExp = dataComponent.RegimentExp;
            RegimentExpSchedule = dataComponent.RegimentExpSchedule;
            RegimentName = dataComponent.RegimentName;
            RegimentIcon = dataComponent.RegimentIcon;
            RegimentPropertyPage = dataComponent.RegimentPropertyPage;
            GoldCoin = dataComponent.GoldCoin;
            Diamond = dataComponent.Diamond;
            Stamina = dataComponent.Stamina;
            Hornor = dataComponent.Hornor;
            ArenaTicket = dataComponent.ArenaTicket;
            Iron = dataComponent.Iron;
            ExclusiveWeaponPropertyResetTicket = dataComponent.ExclusiveWeaponReset;
            CrystalPowder = dataComponent.CrystalPowder;
            CrystalPiece = dataComponent.CrystalPiece;
            CrystalRime = dataComponent.CrystalRime;
            RegimentPassivePage = dataComponent.RegimentPassivePage;
            GridHelperObject = dataComponent.RegimentPassiveSkillScollPage;
            
            RegimentPassiveSkillArea = dataComponent.RegimentPassiveSkillArea;

            dataComponent.RegimentLevelUpEffectButton.onClick.AddListener(OnRegimentLevelUpEffectClicked);
            dataComponent.RepresentHeroButton.onClick.AddListener(OnRegimentRepresentHeroClick);
            dataComponent.AddDiamondButton.onClick.AddListener(OnAddDiamondButtonClick);
            dataComponent.AddGoldCoinButton.onClick.AddListener(OnAddGoldCoinButtonClick);
            dataComponent.AddStaminaButton.onClick.AddListener(OnAddStaminaButtonClick);
            dataComponent.AddHornorButton.onClick.AddListener(OnAddHornorButtonClick);
            dataComponent.AddArenaTicketButton.onClick.AddListener(OnAddArenaTicketButtonClick);
            dataComponent.AddIronButton.onClick.AddListener(OnAddIronButtonClick);
            dataComponent.AddWeaponResetTicketButton.onClick.AddListener(OnAddExclusiveResetTicketButtonClick);
            dataComponent.AddCrystalPowderButton.onClick.AddListener(OnAddCrystalPowderButtonClick);
            dataComponent.AddCrystalPieceButton.onClick.AddListener(OnAddCrystalPieceButtonClick);
            dataComponent.AddCrystalRimeButton.onClick.AddListener(OnAddCrystalRimeButtonClick);
            dataComponent.RegimentPassiveToggle.OnSelectItem.AddListener(OnRegimentPassivePageSelect);
            dataComponent.RegimentPassiveToggle.OnDeSelectItem.AddListener(OnRegimentPassivePageDeSelect);
            dataComponent.RegimentPropertyToggle.OnSelectItem.AddListener(OnRegimentPropertyPageSelect);
            dataComponent.RegimentPropertyToggle.OnDeSelectItem.AddListener(OnRegimentPropertyPageDeSelect);
        }
    }
}
