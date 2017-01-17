using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_MainUI_DL : GUI_Window_DL, GUI_IOnTopHandler
{
    #region window head display area
    public Image RegimentIcon;
    public Text RegimentLevel;
    public Slider RegimentExpSlider;
    public Text RegimentExp;
    public Text RegimentName;

    public Text Honor;
    public Text Diamond;
    public Text GoldCoin;
    public Text Stamina;
    public Text StaminaCounter;

    void RefreshRegimentInfo()
    {
        RefreshRegimentHero(DataCenter.PlayerDataCenter.RepresentHero);
        Honor.text = DataCenter.PlayerDataCenter.Honor.ToString();
        RegimentLevel.text = DataCenter.PlayerDataCenter.Level.ToString();
        RegimentName.text = DataCenter.PlayerDataCenter.Nickname;
        Diamond.text = DataCenter.PlayerDataCenter.Diamond.ToString();
        GoldCoin.text = DataCenter.PlayerDataCenter.Coin.ToString();
        int exp = CSV_b_hero_group_template.GetCurLevelExp((int)DataCenter.PlayerDataCenter.Exp, DataCenter.PlayerDataCenter.Level);
        int levelExp = CSV_b_hero_group_template.GetLevelGrowUpExp(DataCenter.PlayerDataCenter.Level);
        RegimentExp.text = string.Format("{0}/{1}", exp.ToString(),levelExp.ToString());
        RegimentExpSlider.value = Mathf.Clamp01((float)exp / levelExp);
    }

    void RefreshRegimentHero(uint heroServerId)
    {
        DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(heroServerId);
        if (null == hero && DataCenter.PlayerDataCenter.HeroList.Count > 0)
        {
            hero = DataCenter.PlayerDataCenter.HeroList[0];
        }
        if (null != hero)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData((int)hero.CsvId);
            if (null != heroTemplate)
            {
                GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, RegimentIcon);
            }
        }
    }

    void RefreshStaminaCounter()
    {
        if (DataCenter.PlayerDataCenter.Stamina < DataCenter.PlayerDataCenter.MaxStamina)
        {
            uint timeSianceLastUpdate = DataCenter.PlayerDataCenter.ServerTime - DataCenter.PlayerDataCenter.StaminaUpTime;
            uint recoverCount = timeSianceLastUpdate / DataCenter.PlayerDataCenter.StaminaRecoverInterval;
            uint passedTime = timeSianceLastUpdate - recoverCount * DataCenter.PlayerDataCenter.StaminaRecoverInterval;
            StaminaCounter.text = TimeFormater.FormatShrinkIfNeeded(DataCenter.PlayerDataCenter.StaminaRecoverInterval - passedTime);
        }
        else
        {
            StaminaCounter.text = "";
        }
        Stamina.text = string.Format("{0}{1}{2}", DataCenter.PlayerDataCenter.Stamina.ToString(), "/", DataCenter.PlayerDataCenter.MaxStamina);
    }
    #endregion

    #region object refrence
    public GUI_TweenPosition CityMenuPosTweener;
    public GUI_TweenPosition AdventureMenuPosTweener;
    public ScrollRect SlideBG;
    public RectTransform BGPos;
    public RectTransform CityMenuPos;
    public RectTransform SwichMenuPos;
    public RectTransform AdventureMenuPos;

    #endregion

    #region ui window
    public void OnTop()
    {
        if (DataCenter.PlayerDataCenter.GroupPassiveLevelUpList.Count > 0)
        {
            GUI_Manager.Instance.ShowWindowWithName("UI_RegimentPassiveSkillLevelUp", false);
        }
        CheckGoddessInOut();
    }

    void CheckGoddessInOut()
    {
        DataCenter.GoddessInOutInfo inOutInfo = DataCenter.PlayerDataCenter.GoddessData.GetGoddessInOut();
        if (null != inOutInfo)
        {
            switch (inOutInfo.inOut)
            {
                case 0://入队
                    {
                        GUI_GetNewGoddessUI_DL getNewGoddess = GUI_Manager.Instance.ShowWindowWithName<GUI_GetNewGoddessUI_DL>("UI_Goddess_Join", false);
                        if (null != getNewGoddess)
                        {
                            getNewGoddess.AlertNewGoddess(inOutInfo.CsvId);
                        }
                        break;
                    }
                case 1://离队
                    {
                        GUI_GoddessLeaveUI_DL goddessLeave = GUI_Manager.Instance.ShowWindowWithName<GUI_GoddessLeaveUI_DL>("UI_Goddess_Leave", false);
                        if (null != goddessLeave)
                        {
                            goddessLeave.AlertGoddessLeave(inOutInfo.CsvId);
                        }
                        break;
                    }
            }
        }
    }

    void OnEnable()
    {
        Building_DL.OnBuildingClick += OnBuildingClick;
        DataCenter.PlayerDataCenter.OnChangeRepresentHero += RefreshRegimentHero;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange += RefreshNormalTask;
        DataCenter.PlayerDataCenter.OnDrawTaskAward += OnGetAwardRsp;
        DataCenter.PlayerDataCenter.OnPropertyChange += RefreshRegimentInfo;
    }

    void OnDisable()
    {
        Building_DL.OnBuildingClick -= OnBuildingClick;
        DataCenter.PlayerDataCenter.OnChangeRepresentHero -= RefreshRegimentHero;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange -= RefreshNormalTask;
        DataCenter.PlayerDataCenter.OnDrawTaskAward -= OnGetAwardRsp;
        DataCenter.PlayerDataCenter.OnPropertyChange -= RefreshRegimentInfo;
    }

    protected override void OnStart()
    {
        //SlideBG.onValueChanged.AddListener(OnScrollDrag);

        AudioManager.Instance.PlayMusic(DefaultConfig.GetString("MainCityMusic"));

        _swichX = DefaultConfig.GetFloat("MainCitySwichButtonX");

        RefreshRegimentInfo();
        InvokeRepeating("RefreshStaminaCounter", 0f, 1f);

        InitCommonTaskList();
        for (uint index = 0; index < CommonTaskList.Count; ++index )
        {
            RefreshNormalTask(index);
        }

        if (GUI_Manager.Instance.OnTop(this.WindowName))
        {
            OnTop();
        }

        SetCameraMoveCallback();
    }

    void SetCameraMoveCallback()
    {
        SpringObject so = Camera.main.GetComponent<SpringObject>();
        if (so == null) so = Camera.main.gameObject.AddComponent<SpringObject>();

        so.enabled = false;
        so.onSpring = OnCameraSpring;
    }

    void OnGetAwardRsp(uint awardType, uint awardValue)
    {
        GUI_ItemAwardUI_DL itemAward = GUI_Manager.Instance.ShowWindowWithName<GUI_ItemAwardUI_DL>("UI_MissionComplete", false);
        if (null != itemAward)
        {
            itemAward.ShowAwardItem((PbCommon.EAwardType)awardType, (int)awardValue);
        }
        GUI_MessageManager.Instance.ShowErrorTip("获得奖励");
    }
    #endregion

    #region main ui button area
    List<GameObject> CommonTaskObjectList;
    List<GUI_CommonTask_DL> CommonTaskList;

    void InitCommonTaskList()
    {
        CommonTaskList = new List<GUI_CommonTask_DL>();
        for(int index = 0; index < CommonTaskObjectList.Count; ++index)
        {
            GUI_CommonTask_DL commonTask = CommonTaskObjectList[index].GetComponent<GUI_CommonTask_DL>();
            if(null != commonTask)
            {
                CommonTaskList.Add(commonTask);
            }
        }
    }

    void RefreshTask()
    {
        for(int index = 0; index < CommonTaskList.Count; ++index)
        {
            CommonTaskList[index].RefreshTaskInfo();
        }
    }

    void RefreshNormalTask(uint position)
    {
        int index = (int)position;
        if (index >= 0 && index < CommonTaskList.Count)
        {
            CommonTaskList[index].RefreshTaskInfo();
        }
    }

    void OnBuildingClick(BuildingType buildingType, GameObject go)
    {
        switch (buildingType)
        {
            case BuildingType.Battle:
                {
                    GUI_Manager.Instance.ShowWindowWithName("PlotUI", false);
                    break;
                }
            case BuildingType.Hero:
                {
                    GUI_Manager.Instance.ShowWindowWithName("GUI_HeroManage", false);
                    break;
                }
            default:
                {
                    GUI_MessageManager.Instance.ShowErrorTip(10001);
                    break;
                }
        }
    }

    public void OnRegimentInfoClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_HeroGroup", false);
    }

    public void OnLibraryButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnMailButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }
    #endregion

    #region city menu button area
    public void OnShopButtonClicked()
    {
        Building_DL.FocusBuilding(BuildingType.Store);
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnFriendButtonClicked()
    {
        Building_DL.FocusBuilding(BuildingType.Friend);
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnBakeryButtonClicked()
    {
        Building_DL.FocusBuilding(BuildingType.Bread);
        GUI_Manager.Instance.ShowWindowWithName("GUI_BakeryUI", false);
    }

    public void OnSkillButtonClicked()
    {
        Building_DL.FocusBuilding(BuildingType.Skill);
        GUI_Manager.Instance.ShowWindowWithName("UI_HeroSkill", false);
    }

    public void OnEquipmentButtonClicked()
    {
        Building_DL.FocusBuilding(BuildingType.Equipment);
        GUI_Manager.Instance.ShowWindowWithName("GUI_EquipPackageUI", false);
    }

    public void OnLordButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnHeroButtonClicked()
    {
        Building_DL.FocusBuilding(BuildingType.Hero);
        GUI_Manager.Instance.ShowWindowWithName("GUI_HeroManage", false);
    }

    public void OnGodButtonClicked()
    {
        Building_DL.FocusBuilding(BuildingType.God);
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }
    #endregion

    #region adventure menu button area

    public void OnPracticeButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnExpeditionButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_Exploration", false);
    }

    public void OnSoulFortressButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnWorldBossButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnPlotButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("PlotUI", false);
    }

    #endregion

    #region swich menu button area
    bool _CityMenuShowing = true;
    //bool _ClickSwichMenuButton = false;

    float _swichX = 120;

    void OnCameraSpring(GameObject go)
    {
        if (_CityMenuShowing)
        {
            if (go.transform.position.x > _swichX)
            {
                SwichCityMenu();
            }
        }
        else
        {
            if (go.transform.position.x < _swichX)
            {
                SwichAdventureMenu();
            }
        }
    }

    //public void OnScrollDrag(Vector2 v2)
    //{
    //    if (!_ClickSwichMenuButton)
    //    {
    //        if (_CityMenuShowing)
    //        {
    //            if (BGPos.anchoredPosition.x > SwichMenuPos.anchoredPosition.x)
    //            {
    //                SwichCityMenu();
    //            }
    //        }
    //        else
    //        {
    //            if (BGPos.anchoredPosition.x < SwichMenuPos.anchoredPosition.x)
    //            {
    //                SwichAdventureMenu();
    //            }
    //        }
    //    }
    //}

    //void SetBGPos(Vector3 pos)
    //{
    //    GUI_TweenPosition.Begin(BGPos.gameObject, 0.3f, pos, false, GUI_Tweener.Method.Linear, GUI_Tweener.Style.Once, ResetClickTag);
    //}

    //void ResetClickTag()
    //{
    //    //_ClickSwichMenuButton = false;
    //}

    public void OnAdventureMenuButtonClicked()
    {
        //_ClickSwichMenuButton = true;
        //SetBGPos(AdventureMenuPos.localPosition);
        //SwichCityMenu();

        Building_DL.FocusBuilding(BuildingType.Battle);
        //GUI_Manager.Instance.ShowWindowWithName("PlotUI", false);
    }

    public void OnTownMenuButtonClicked()
    {
        //_ClickSwichMenuButton = true;
        //SetBGPos(CityMenuPos.localPosition);
        //SwichAdventureMenu();
        Building_DL.FocusBuilding(BuildingType.Friend);
    }

    void SwichCityMenu()
    {
        if (_CityMenuShowing)
        {
            CityMenuPosTweener.Play(_CityMenuShowing, SwichAdventureMenu);
        }
        else
        {
            CityMenuPosTweener.Play(_CityMenuShowing);
            _CityMenuShowing = true;
        }
    }

    void SwichAdventureMenu()
    {
        if (_CityMenuShowing)
        {
            AdventureMenuPosTweener.Play(_CityMenuShowing);
            _CityMenuShowing = false;
        }
        else
        {
            AdventureMenuPosTweener.Play(_CityMenuShowing, SwichCityMenu);
        }
    }
    #endregion

    #region left hidden button area
    public GUI_TweenPosition LeftHiddenButtonTweener;
    public RectTransform LeftHiddenButtonArrow;
    bool LeftHiddenButtonShow = false;

    public void OnSwitchLeftHiddenButtonClicked()
    {
        if(null != LeftHiddenButtonTweener)
        {
            LeftHiddenButtonShow = !LeftHiddenButtonShow;
            LeftHiddenButtonArrow.Rotate(0f, 180f, 0f);
            LeftHiddenButtonTweener.Play(LeftHiddenButtonShow);
        }
    }

    public void OnAnnouncementButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnSignInButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnHelpButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnGoddessHouseButtonClicked()
    {
        GUI_GoddessHouseUI_DL goddessHouse = GUI_Manager.Instance.ShowWindowWithName<GUI_GoddessHouseUI_DL>("UI_Goddess", false);
        if(null != goddessHouse)
        {
            goddessHouse.VisiteGoddess();
        }
    }

    public void OnSkinButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnSettingButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnShowRoleIdButtonClicked()
    {
        GUI_MessageManager.Instance.ShowMessage("账号信息", DataCenter.PlayerDataCenter.RoleId.ToString(), "确定", null, null, null, MessageBoxType.Confirm, true);
    }
    #endregion

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_MainUI dataComponent = gameObject.GetComponent<GUI_MainUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_MainUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            RegimentIcon = dataComponent.RegimentIcon;
            RegimentLevel = dataComponent.RegimentLevel;
            RegimentExpSlider = dataComponent.RegimentExpSlider;
            RegimentExp = dataComponent.RegimentExp;
            RegimentName = dataComponent.RegimentName;
            Honor = dataComponent.Honor;
            Diamond = dataComponent.Diamond;
            GoldCoin = dataComponent.GoldCoin;
            Stamina = dataComponent.Stamina;
            StaminaCounter = dataComponent.StaminaCounter;
            CityMenuPosTweener = dataComponent.CityMenuPosTweener;
            AdventureMenuPosTweener = dataComponent.AdventureMenuPosTweener;
            SlideBG = dataComponent.SlideBG;
            BGPos = dataComponent.BGPos;
            CityMenuPos = dataComponent.CityMenuPos;
            SwichMenuPos = dataComponent.SwichMenuPos;
            AdventureMenuPos = dataComponent.AdventureMenuPos;
            CommonTaskObjectList = dataComponent.CommonTaskObjectList;
            LeftHiddenButtonArrow = dataComponent.LeftHiddenButtonArrow;
            LeftHiddenButtonTweener = dataComponent.LeftHiddenButtonTweener;

            dataComponent.RegimentIconButton.onClick.AddListener(OnRegimentInfoClicked);
            dataComponent.HonorQuestionButton.onClick.AddListener(OnLibraryButtonClicked);
            dataComponent.AddDiamondButton.onClick.AddListener(OnLibraryButtonClicked);
            dataComponent.ShopButton.onClick.AddListener(OnShopButtonClicked);
            dataComponent.FriendButton.onClick.AddListener(OnFriendButtonClicked);
            dataComponent.BakeryButton.onClick.AddListener(OnBakeryButtonClicked);
            dataComponent.SkillButton.onClick.AddListener(OnSkillButtonClicked);
            dataComponent.EquipButton.onClick.AddListener(OnEquipmentButtonClicked);
            dataComponent.HeroManageButton.onClick.AddListener(OnHeroButtonClicked);
            dataComponent.AdverntureButton.onClick.AddListener(OnAdventureMenuButtonClicked);
            dataComponent.CityMenuButton.onClick.AddListener(OnTownMenuButtonClicked);
            dataComponent.PricticeButton.onClick.AddListener(OnPracticeButtonClicked);
            dataComponent.ExpeditionButton.onClick.AddListener(OnExpeditionButtonClicked);
            dataComponent.SoulFortressButton.onClick.AddListener(OnSoulFortressButtonClicked);
            dataComponent.WorldBossButton.onClick.AddListener(OnWorldBossButtonClicked);
            dataComponent.SwitchLeftHiddenButton.onClick.AddListener(OnSwitchLeftHiddenButtonClicked);
            dataComponent.AnnouncementButton.onClick.AddListener(OnAnnouncementButtonClicked);
            dataComponent.SignInButton.onClick.AddListener(OnSignInButtonClicked);
            dataComponent.HelpButton.onClick.AddListener(OnHelpButtonClicked);
            dataComponent.GoddessButton.onClick.AddListener(OnGoddessHouseButtonClicked);
            dataComponent.SkinButton.onClick.AddListener(OnSkinButtonClicked);
            dataComponent.SettingButton.onClick.AddListener(OnSettingButtonClicked);
            dataComponent.ShowRoleIdButton.onClick.AddListener(OnShowRoleIdButtonClicked);
        }
    }
}
