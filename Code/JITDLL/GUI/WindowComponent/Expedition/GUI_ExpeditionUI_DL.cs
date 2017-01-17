using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GUI_ExpeditionUI_DL : GUI_Window_DL {
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ExpeditionUI dataComponent = gameObject.GetComponent<GUI_ExpeditionUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExpeditionUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            MissionName = dataComponent.MissionName;
            FixAward1 = dataComponent.FixAward1;
            FixAward2 = dataComponent.FixAward2;
            FixAward3 = dataComponent.FixAward3;
            HeroGroupExp = dataComponent.HeroGroupExp;
            HeroExp = dataComponent.HeroExp;

            RandomBox1 = dataComponent.RandomBox1;
            RandomBox2 = dataComponent.RandomBox2;
            RandomBox3 = dataComponent.RandomBox3;
            ConditionProgress = dataComponent.ConditionProgress;

            ConditionList = dataComponent.ConditionList;

            ExpeditionTime = dataComponent.ExpeditionTime;
            HeroCount = dataComponent.HeroCount;
            RemindText = dataComponent.RemindText;
            ExpeditioningMask = dataComponent.ExpeditioningMask;
            SelectedHeroIcons = dataComponent.SelectedHeroIcons;
            RegistHeroIconEvent(dataComponent);

            HeroScrollPage = dataComponent.HeroScrollPage;

            PrepareButtonRoot = dataComponent.PrepareButtonRoot;
            BeginButton = dataComponent.BeginButton;
            dataComponent.SelectMenu.onClick.AddListener(OnFilterButtonClicked);
            dataComponent.OrderDirection.onClick.AddListener(OnOrderDirectionButtonClicked);
            dataComponent.RecommondButton.onClick.AddListener(OnRecommondHeroButtonClicked);
            dataComponent.BeginButton.onClick.AddListener(OnBeginExpeditionButtonClicked);

            ExpeditioningButtonRoot = dataComponent.ExpeditioningButtonRoot;
            QuickExpeditionButton = dataComponent.QuickExpeditionButton;
            QuickExpeditionCost = dataComponent.QuickExpeditionCost;
            dataComponent.AbortExpeditionButton.onClick.AddListener(OnAbortExpeditionButtonClicked);
            dataComponent.QuickExpeditionButton.onClick.AddListener(OnQuickFinishButtonClicked);

            FilterOptionRootObject = dataComponent.FilterOptionRootObject;
            SchoolFilterList = dataComponent.SchoolFilterList;
            StarFilterList = dataComponent.StarFilterList;
            TypeFilterList = dataComponent.TypeFilterList;
            NationalityFilterList = dataComponent.NationalityFilterList;
            ExpeditionFinishButtonRoot = dataComponent.ExpeditionFinishButtonRoot;
            dataComponent.ExpeditionFinishButton.onClick.AddListener(OnFinishExpeditionButtonClicked);
        }
    }

    void RegistHeroIconEvent(GUI_ExpeditionUI dataComponent)
    {
        dataComponent.SelectedHeroIconButton[0].onClick.AddListener(OnHero1IconClicked);
        dataComponent.SelectedHeroIconButton[1].onClick.AddListener(OnHero2IconClicked);
        dataComponent.SelectedHeroIconButton[2].onClick.AddListener(OnHero3IconClicked);
        dataComponent.SelectedHeroIconButton[3].onClick.AddListener(OnHero4IconClicked);
    }

    void RegistConditionEvent(GUI_ExpeditionUI dataComponent)
    {

    }
    #endregion

    #region window logic
    public Text MissionName;
    public GUI_ItemSimpleInfo FixAward1;
    public GUI_ItemSimpleInfo FixAward2;
    public GUI_ItemSimpleInfo FixAward3;
    public Text HeroGroupExp;
    public Text HeroExp;

    public GUI_ExpeditionRandomAwardBox RandomBox1;
    public GUI_ExpeditionRandomAwardBox RandomBox2;
    public GUI_ExpeditionRandomAwardBox RandomBox3;
    public GUI_ColorfulSlider ConditionProgress;

    public List<GUI_ExpeditionRandomAwardCondition> ConditionList;

    public Text ExpeditionTime;
    public Text HeroCount;
    public Text RemindText;
    public GameObject ExpeditioningMask;
    public List<Image> SelectedHeroIcons;

    public GameObject HeroScrollPage;

    public GameObject PrepareButtonRoot;

    public GameObject ExpeditioningButtonRoot;
    public Button BeginButton;
    public Button QuickExpeditionButton;
    public Text QuickExpeditionCost;

    public GameObject ExpeditionFinishButtonRoot;

    DataCenter.Expedition Expedition;
    CSV_b_expedition_quest_template MissionTemplate;
    CSV_b_expedition_random_award RandomAwardTemplate;
    CSV_b_expedition_template ExpeditionTemplate;
    bool CountingMission = false;


    public void ShowExpedition(DataCenter.Expedition expedition, CSV_b_expedition_quest_template missionTemplate)
    {
        Expedition = expedition;
        MissionTemplate = missionTemplate;
        if (null == Expedition || null == MissionTemplate)
        {
            HideWindow();
        }
        else
        {
            ExpeditionTemplate = CSV_b_expedition_template.FindData(missionTemplate.QuestGroup);
            RandomAwardTemplate = Expedition.extraCSV;
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnEndExpedition += OnFinishExpeditionRsp;
        DataCenter.PlayerDataCenter.OnBeginExpedition += OnBeginExpeditionRsp;
        DataCenter.PlayerDataCenter.OnCancelExpedition += OnAbortExpeditionRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnEndExpedition -= OnFinishExpeditionRsp;
        DataCenter.PlayerDataCenter.OnBeginExpedition -= OnBeginExpeditionRsp;
        DataCenter.PlayerDataCenter.OnCancelExpedition -= OnAbortExpeditionRsp;
    }

    public override void PreHideWindow()
    {
        StopCount();
    }

    protected override void OnStart()
    {
        SetFixAward();
        SetRandomAwardBox();
        InitCondition();
        InitHeroPage();
        InitFilters();
        CheckExpeditionState();
    }

    void CheckExpeditionState()
    {
        StopCount();
        SelectedHeroList.Clear();
        if (Expedition.FinishTime > 0)//expeditioning
        {
            StartCount();
           
            CandidateHeroList = DataCenter.PlayerDataCenter.ExpeditionData.GetFreeHeroList();
            for (int index = 0; index < Expedition.heroIds.Count; ++index )
            {
                SelectedHeroList.Add(DataCenter.PlayerDataCenter.GetHero(Expedition.heroIds[index]));
            }
        }
        RefreshCandidateInfo();
    }

    void StartCount()
    {
        if (!CountingMission)
        {
            CountingMission = true;
            InvokeRepeating("RefreshExpeditionState", 0.05f, 60f);//update per minute
        }
    }

    void StopCount()
    {
        if (CountingMission)
        {
            CountingMission = false;
            CancelInvoke("RefreshExpeditionState");
        }
    }

    void RefreshExpeditionState()
    {
        if (null != Expedition && null != MissionTemplate)
        {
            if (Expedition.FinishTime > 0)
            {
                if(Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)
                {
                    ActiveExpeditioningState(true, false);
                    UpdateRemainExpeditioningTime();
                }
                else
                {
                    ActiveExpeditioningState(false, true);
                }
            }
            else//not recieve mission
            {
                ActiveExpeditioningState(false, false);
            }
        }
    }

    void UpdateRemainExpeditioningTime()
    {
        if (Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)//not finish
        {
            uint leftTime = Expedition.FinishTime - DataCenter.PlayerDataCenter.ServerTime;
            uint leftMinutes = leftTime / (uint)ConstDefine.SECOND_PER_MINUTE;
            uint totalMinutes = (uint)(MissionTemplate.ExpeditionTime / ConstDefine.SECOND_PER_MINUTE);
            uint leftHours = leftTime / (uint)ConstDefine.SECOND_PER_HOUR;
            TextLocalization.SetTextById(RemindText, TextId.Expedition_OnGoing);

            if (leftHours > 0)
            {
                if (leftMinutes > 0)
                {
                    ++leftHours;
                }
                GUI_Tools.TextTool.SetHour(ExpeditionTime, (int)leftHours);
            }
            else
            {
                uint leftSeconds = leftTime % (uint)ConstDefine.SECOND_PER_MINUTE;
                if (leftSeconds > 0)
                {
                    ++leftMinutes;
                }
                GUI_Tools.TextTool.SetMinute(ExpeditionTime, (int)leftMinutes);
                if(leftMinutes > 0)
                {
                    ++leftHours;
                }
            }
            int totalHour = (MissionTemplate.ExpeditionTime + 3599) / ConstDefine.SECOND_PER_HOUR;
            float mapCount = Mathf.Ceil((float)leftHours * MissionTemplate.MapPieceCount / totalHour);
            QuickExpeditionCost.text = ((int)mapCount).ToString();
        }
        else//finished
        {
            TextLocalization.SetTextById(RemindText, TextId.Expedition_Finish);
            TextLocalization.SetTextById(ExpeditionTime, TextId.Finish);
            StopCount();
            ActiveExpeditioningState(false, true);
        }
    }

    void ActiveExpeditioningState(bool expeditioning, bool finish)
    {
        GUI_Tools.ObjectTool.ActiveObject(ExpeditioningMask, expeditioning);
        GUI_Tools.ObjectTool.ActiveObject(PrepareButtonRoot, !expeditioning && !finish);
        GUI_Tools.ObjectTool.ActiveObject(ExpeditioningButtonRoot, expeditioning);
        GUI_Tools.ObjectTool.ActiveObject(ExpeditionFinishButtonRoot, finish);
    }
    #endregion

    #region hero scroll page
    GUI_GridLayoutGroupHelper_DL HeroScrollPageHelper;
    GUI_LogicObjectPool CandidateHeroItemPool;
    List<DataCenter.Hero> CandidateHeroList;
    List<DataCenter.Hero> SelectedHeroList;

    void InitHeroPage()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Prepare_Exploration_HeroItem", true, AssetManage.E_AssetType.UIPrefab);
        CandidateHeroItemPool = new GUI_LogicObjectPool(go);

        SelectedHeroList = new List<DataCenter.Hero>();
        HeroScrollPageHelper = HeroScrollPage.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        if(null != HeroScrollPageHelper)
        {
            HeroScrollPageHelper.SetScrollAction(DisplayHero);
        }

        CandidateHeroList = DataCenter.PlayerDataCenter.ExpeditionData.GetFreeHeroList();
        RefreshCandidateHeroPage();
    }

    void RefreshCandidateHeroPage()
    {
        if (null != HeroScrollPageHelper)
        {
            HeroScrollPageHelper.Clear();
            if(null != CandidateHeroList)
            {
                if (DescendOrder)
                {
                    for (int index = 0; index < CandidateHeroList.Count; ++index)
                    {
                        HeroScrollPageHelper.FillItem((int)CandidateHeroList[index].ServerId);
                    }
                }
                else
                {
                    for (int index = CandidateHeroList.Count - 1; index >= 0; --index)
                    {
                        HeroScrollPageHelper.FillItem((int)CandidateHeroList[index].ServerId);
                    }
                }
                HeroScrollPageHelper.FillItemEnd();
            }
        }
    }

    void DisplayHero(GUI_ScrollItem scrollItem)
    {
        if(null != scrollItem)
        {
            DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero((uint)scrollItem.LogicIndex);
            if(null != hero)
            {
                GUI_ExpeditionCandidateHeroInfo_DL heroItem = CandidateHeroItemPool.GetOneLogicComponent() as GUI_ExpeditionCandidateHeroInfo_DL;
                if(null != heroItem)
                {
                    heroItem.DisplayHero(hero, OnCandidateHeroSelect, OnCandidateHeroDeselect, IsHeroSelect);
                    scrollItem.SetTarget(heroItem);
                }
            }
        }
    }

    bool IsHeroSelect(DataCenter.Hero hero)
    {
        if(null != hero)
        {
            return SelectedHeroList.Contains(hero);
        }
        return false;
    }

    void OnCandidateHeroSelect(GUI_ExpeditionCandidateHeroInfo_DL candidateHero)
    {
        if(Expedition.FinishTime == 0 && null != candidateHero)
        {
            if(!SelectedHeroList.Contains(candidateHero.CandidateHero))
            {
                if(SelectedHeroList.Count < MissionTemplate.HeroCount)
                {
                    SelectedHeroList.Add(candidateHero.CandidateHero);

                    RefreshCandidateInfo();
                }
                else
                {
                    candidateHero.DeSelect();
                }
            }
        }
    }

    void OnCandidateHeroDeselect(GUI_ExpeditionCandidateHeroInfo_DL candidateHero)
    {
        if (Expedition.FinishTime == 0 && null != candidateHero)
        {
            if (SelectedHeroList.Contains(candidateHero.CandidateHero))
            {
                SelectedHeroList.Remove(candidateHero.CandidateHero);

                RefreshCandidateInfo();
            }
        }
    }

    void RefreshCandidateInfo()
    {
        if(SelectedHeroList.Count > 0)
        {
            List<ExpeditionAwardCondition> matchConditions;
            List<DataCenter.Hero> recommondHeroList;
            DataCenter.PlayerDataCenter.ExpeditionData.FindHeroesMatchConditions(SelectedHeroList, RandomAwardTemplate.ExtraConditions, MissionTemplate.HeroCount, out recommondHeroList, out matchConditions);
            RefreshFullFillCondition(matchConditions);
        }
        else
        {
            UpdateFullFillCondition(null, true);
        }

        RefreshSelectedHeroIcon();
        RefreshExpeditionStateInfo();
    }

    void RefreshSelectedHeroIcon()
    {
        for(int index = 0; index < SelectedHeroIcons.Count; ++index)
        {
            if(index < SelectedHeroList.Count)
            {
                CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(SelectedHeroList[index].CsvId);
                if(null != heroTemplate)
                {
                    GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, SelectedHeroIcons[index]);
                }
                else
                {
                    SelectedHeroIcons[index].sprite = null;
                }
            }
            else
            {
                SelectedHeroIcons[index].sprite = null;
            }
        }
    }

    void RefreshExpeditionStateInfo()
    {
        int hour = MissionTemplate.ExpeditionTime / ConstDefine.SECOND_PER_HOUR;
        ExpeditionTime.text = hour.ToString();

        bool enoughHero = SelectedHeroList.Count == MissionTemplate.HeroCount;
        BeginButton.interactable = enoughHero;
        TextLocalization.SetTextById(RemindText, enoughHero ? "Remind_Begin_Expedition" : "Remind_Select_Expedition_Hero");
        HeroCount.text = string.Format("{0}/{1}", SelectedHeroList.Count.ToString(), MissionTemplate.HeroCount.ToString());
    }

    void OnRecommondHeroButtonClicked()
    {
        SelectedHeroList.Clear();
        List<ExpeditionAwardCondition> matchConditions;
        List<DataCenter.Hero> recommondHeroList;
        DataCenter.PlayerDataCenter.ExpeditionData.FindHeroesMatchConditions(CandidateHeroList, RandomAwardTemplate.ExtraConditions, MissionTemplate.HeroCount, out recommondHeroList, out matchConditions);
        RefreshFullFillCondition(matchConditions);
        SelectedHeroList.AddRange(recommondHeroList);
        HeroScrollPageHelper.RefreshPageData();
        RefreshSelectedHeroIcon();
        RefreshExpeditionStateInfo();
    }

    void OnFinishExpeditionButtonClicked()
    {
        if(Expedition.FinishTime <= DataCenter.PlayerDataCenter.ServerTime)
        {
            gsproto.EndExpeditionReq req = new gsproto.EndExpeditionReq();
            req.end_type = (uint)PbCommon.EEndExpeditionType.E_EndExpedition_Normal;
            req.expedition_quest_id = (uint)MissionTemplate.Id;
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    void OnFinishExpeditionRsp(DataCenter.GroupAddExpInfo groupAddExp, List<DataCenter.HeroAddExpInfo> heroAddExpList, List<DataCenter.AwardInfo> extraAwardList)
    {
        GUI_ExpeditionAwardUI_DL awardUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExpeditionAwardUI_DL>("UI_FinishExploration", false);
        if (null != awardUI)
        {
            awardUI.ShowAwardInfo(Expedition, MissionTemplate, groupAddExp, heroAddExpList, extraAwardList);
        }
        HideWindow();
    }

    void OnQuickFinishButtonClicked()
    {
        if (Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)
        {
            GUI_QuickExpeditionUI_DL quickExpedition = GUI_Manager.Instance.ShowWindowWithName<GUI_QuickExpeditionUI_DL>("UI_Exploration_FastPass", false);
            if(null != quickExpedition)
            {
                quickExpedition.TryQuickFinish(Expedition, MissionTemplate);
            }
        }
    }

    void OnBeginExpeditionButtonClicked()
    {
        if(SelectedHeroList.Count == MissionTemplate.HeroCount)
        {
            gsproto.BeginExpeditionReq req = new gsproto.BeginExpeditionReq();
            req.expedition_quest_id = (uint)MissionTemplate.Id;
            for (int index = 0; index < SelectedHeroList.Count; ++index)
            {
                req.hero_ids.Add(SelectedHeroList[index].ServerId);
            }
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    void OnBeginExpeditionRsp(int csvId)
    {
        CheckExpeditionState();
        RefreshCandidateHeroPage();
    }

    void OnAbortExpeditionButtonClicked()
    {
        GUI_AbortExpeditionUI_DL abortExpetion = GUI_Manager.Instance.ShowWindowWithName<GUI_AbortExpeditionUI_DL>("UI_Exploration_Cancel", false);
        if(null != abortExpetion)
        {
            abortExpetion.TryAbortExpedition(Expedition, MissionTemplate);
        }
    }

    void OnAbortExpeditionRsp(int csvId)
    {
        HideWindow();
    }

    void OnHero1IconClicked()
    {
        if (Expedition.FinishTime == 0 && SelectedHeroList.Count > 0)
        {
            if(!DeselectHero(SelectedHeroList[0]))
            {
                SelectedHeroList.RemoveAt(0);
            }
        }
    }

    void OnHero2IconClicked()
    {
        if (Expedition.FinishTime == 0 && SelectedHeroList.Count > 1)
        {
            if(!DeselectHero(SelectedHeroList[1]))
            {
                SelectedHeroList.RemoveAt(1);
            }
        }
    }

    void OnHero3IconClicked()
    {
        if (Expedition.FinishTime == 0 && SelectedHeroList.Count > 2)
        {
            if (!DeselectHero(SelectedHeroList[2]))
            {
                SelectedHeroList.RemoveAt(2);
            }
        }
    }

    void OnHero4IconClicked()
    {
        if (Expedition.FinishTime == 0 && SelectedHeroList.Count > 3)
        {
            if (!DeselectHero(SelectedHeroList[3]))
            {
                SelectedHeroList.RemoveAt(3);
            }
        }
    }

    bool DeselectHero(DataCenter.Hero hero)
    {
        if(null != hero)
        {
            for(int index = 0; index < HeroScrollPageHelper.ItemCount; ++index)
            {
                GUI_ScrollItem scrollItem = HeroScrollPageHelper.GetAtIndex(index);
                if(null != scrollItem)
                {
                    GUI_ExpeditionCandidateHeroInfo_DL candidateHero = scrollItem.LogicObject as GUI_ExpeditionCandidateHeroInfo_DL;
                    if(null != candidateHero)
                    {
                        if(candidateHero.CandidateHero.ServerId == hero.ServerId)
                        {
                            candidateHero.DeSelect();
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    #endregion

    #region award area
    void SetFixAward()
    {
        if(null != MissionTemplate)
        {
            if(null != ExpeditionTemplate)
            {
                MissionName.text = string.Format("[{0}]{1}", ExpeditionTemplate.Name, MissionTemplate.Name);
            }

            HeroGroupExp.text = MissionTemplate.HeroGroupExp.ToString();
            HeroExp.text = MissionTemplate.HeroExp.ToString();

            GUI_Tools.ItemTool.SetAwardItemInfo(MissionTemplate.FixAwardType1, null, FixAward1.Name_Star_Count, FixAward1.Icon, MissionTemplate.FixAwardValue1);
            GUI_Tools.ItemTool.SetAwardItemInfo(MissionTemplate.FixAwardType2, null, FixAward2.Name_Star_Count, FixAward2.Icon, MissionTemplate.FixAwardValue2);
            GUI_Tools.ItemTool.SetAwardItemInfo(MissionTemplate.FixAwardType3, null, FixAward3.Name_Star_Count, FixAward3.Icon, MissionTemplate.FixAwardValue3);
        }
    }

    void SetRandomAwardBox()
    {
        if(null != RandomAwardTemplate)
        {
            int randomAwardCount = GetRandomAwardBoxCount();
            switch(randomAwardCount)
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

    void RefreshRandomAwardBox(int fullFillCount)
    {
        if (null != RandomAwardTemplate)
        {
            int randomAwardCount = GetRandomAwardBoxCount();
            switch (randomAwardCount)
            {
                case 1:
                    {
                        RefreshRandomAwardBox(RandomBox2, fullFillCount >= RandomAwardTemplate.ConditionCount1);
                        break;
                    }
                case 2:
                    {
                        RefreshRandomAwardBox(RandomBox1, fullFillCount >= RandomAwardTemplate.ConditionCount1);
                        RefreshRandomAwardBox(RandomBox3, fullFillCount >= RandomAwardTemplate.ConditionCount2);
                        break;
                    }
                case 3:
                    {
                        RefreshRandomAwardBox(RandomBox1, fullFillCount >= RandomAwardTemplate.ConditionCount1);
                        RefreshRandomAwardBox(RandomBox2, fullFillCount >= RandomAwardTemplate.ConditionCount2);
                        RefreshRandomAwardBox(RandomBox3, fullFillCount >= RandomAwardTemplate.ConditionCount3);
                        break;
                    }
                default:
                    {
                        RefreshRandomAwardBox(RandomBox1, false);
                        RefreshRandomAwardBox(RandomBox2, false);
                        RefreshRandomAwardBox(RandomBox3, false);
                        break;
                    }
            }

        }
    }

    void RefreshRandomAwardBox(GUI_ExpeditionRandomAwardBox randomBox, bool fullFill)
    {
        if(null != randomBox)
        {
            randomBox.CheckMark.isOn = fullFill;
            randomBox.MaskToggle.isOn = fullFill;
        }
    }

    int GetRandomAwardBoxCount()
    {
        int count = 0;
        if(null != RandomAwardTemplate)
        {
            if(RandomAwardTemplate.ConditionCount1 > 0)
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

    void SetRandomAwardBox(int conditionCount, string boxAtlas, string boxIcon, GUI_ExpeditionRandomAwardBox awardBox)
    {
        if(null != awardBox)
        {
            if (conditionCount > 0)
            {
                awardBox.ConditionCount.text = conditionCount.ToString();
                GUI_Tools.ObjectTool.ActiveObject(awardBox.RootObject, true);
                GUI_Tools.IconTool.SetIcon(boxAtlas, boxIcon, awardBox.AwardBox.Item);
            }
            else
            {
                GUI_Tools.ObjectTool.ActiveObject(awardBox.RootObject, false);
            }
        }
    }
    #endregion

    #region condition area
    BitArray FullFillCondition;

    void InitCondition()
    {
        FullFillCondition = new BitArray(10);
        if(null != RandomAwardTemplate)
        {
            for(int index = 0; index < ConditionList.Count && index < RandomAwardTemplate.ExtraConditions.Count; ++index)
            {
                SetCondition(RandomAwardTemplate.ExtraConditions[index].ConditionType, RandomAwardTemplate.ExtraConditions[index].ConditionValue, ConditionList[index]);
            }
        }
    }

    void SetCondition(int conditionType, int conditionValue, GUI_ExpeditionRandomAwardCondition condition)
    {
        if(null == condition)
        {
            return;
        }
        switch((PbCommon.EExpeditionConditionType)conditionType)
        {
            case PbCommon.EExpeditionConditionType.E_Condition_Hero_Level:
                {
                    GUI_Tools.IconTool.SetIcon("", "", condition.Condition.Icon);
                    GUI_Tools.ObjectTool.ActiveObject(condition.Condition.Icon.gameObject, false);
                    condition.Condition.Name_Star_Count.text = conditionValue.ToString();
                    break;
                }
            case PbCommon.EExpeditionConditionType.E_Condition_Hero_Nationality:
                {
                    GUI_Tools.IconTool.SetHeroNationalityIcon(conditionValue, condition.Condition.Icon);
                    GUI_Tools.ObjectTool.ActiveObject(condition.Condition.Icon.gameObject, true);
                    condition.Condition.Name_Star_Count.text = "";
                    break;
                }
            case PbCommon.EExpeditionConditionType.E_Condition_Hero_Sex:
                {
                    GUI_Tools.IconTool.SetSexIcon( (PbCommon.EHeroSexType)conditionValue, condition.Condition.Icon);
                    GUI_Tools.ObjectTool.ActiveObject(condition.Condition.Icon.gameObject, true);
                    condition.Condition.Name_Star_Count.text = "";
                    break;
                }
            case PbCommon.EExpeditionConditionType.E_Condition_Hero_Star:
                {
                    GUI_Tools.IconTool.SetIcon("", "", condition.Condition.Icon);
                    GUI_Tools.ObjectTool.ActiveObject(condition.Condition.Icon.gameObject, false);
                    condition.Condition.Name_Star_Count.text = "*" + conditionValue.ToString();
                    break;
                }
            case PbCommon.EExpeditionConditionType.E_Condition_Hero_type:
                {
                    GUI_Tools.IconTool.SetHeroTypeIcon((PbCommon.EHeroType)conditionValue, condition.Condition.Icon);
                    GUI_Tools.ObjectTool.ActiveObject(condition.Condition.Icon.gameObject, true);
                    condition.Condition.Name_Star_Count.text = "";
                    break;
                }
            case PbCommon.EExpeditionConditionType.E_Condition_School:
                {
                    GUI_Tools.IconTool.SetShoolIcon(conditionValue, condition.Condition.Icon, false);
                    GUI_Tools.ObjectTool.ActiveObject(condition.Condition.Icon.gameObject, true);
                    condition.Condition.Name_Star_Count.text = "";
                    break;
                }
        }
    }

    void UpdateFullFillCondition(List<ExpeditionAwardCondition> matchConditions, bool clearOld)
    {
        if(clearOld)
        {
            FullFillCondition.SetAll(false);
        }
        int matchCount = 0;
        if (null != matchConditions)
        {
            for (int index = 0; index < matchConditions.Count && index < FullFillCondition.Count; ++index)
            {
                int conditionIndex = RandomAwardTemplate.ExtraConditions.IndexOf(matchConditions[index]);
                FullFillCondition.Set(conditionIndex, true);
            }
            matchCount = matchConditions.Count;
        }

        for (int index = 0; index < ConditionList.Count && index < FullFillCondition.Count; ++index)
        {
            GUI_Tools.ObjectTool.ActiveObject(ConditionList[index].CheckMark, FullFillCondition[index]);
        }

        RefreshFullFillSchedule(matchCount);
        RefreshRandomAwardBox(matchCount);
    }

    void RefreshFullFillCondition(List<ExpeditionAwardCondition> matchConditions)
    {
        UpdateFullFillCondition(matchConditions, true);
    }

    int GetRandomAwardMaxMatchCount()
    {
        int count = 0;
        if (null != RandomAwardTemplate)
        {
            if (RandomAwardTemplate.ConditionCount2 > 0)
            {
                count = RandomAwardTemplate.ConditionCount2;
            }
            if (RandomAwardTemplate.ConditionCount3 > 0)
            {
                count = RandomAwardTemplate.ConditionCount3;
            }
        }
        return count;
    }

    void RefreshFullFillSchedule(int matchCount)
    {
        int maxMatchCount = GetRandomAwardMaxMatchCount();
        int baseMatchCount = RandomAwardTemplate.ConditionCount1;
        maxMatchCount -= baseMatchCount;
        int baseValue = Mathf.Clamp(matchCount - baseMatchCount, 0, maxMatchCount);
        ConditionProgress.Slider.value = Mathf.Clamp01((float)baseValue / maxMatchCount);
    }
    #endregion

    #region hero filter
    public GameObject FilterOptionRootObject;

    public List<Toggle> SchoolFilterList;
    public List<Toggle> StarFilterList;
    public List<Toggle> TypeFilterList;
    public List<Toggle> NationalityFilterList;

    bool DescendOrder = true;
    BitArray SchoolFilter;
    BitArray StarFilter;
    BitArray TypeFilter;
    BitArray NationalityFilter;

    int[] FilterCounterArray;

    void InitFilters()
    {
        FilterCounterArray = new int[4] { 0, 0, 0, 0 };

        SchoolFilter = new BitArray(16, false);//16位应该是足够用的
        StarFilter = new BitArray(16, false);//16位应该是足够用的
        TypeFilter = new BitArray(16, false);//16位应该是足够用的
        NationalityFilter = new BitArray(16, false);//16位应该是足够用的

        RegistFilterListener(SchoolFilterList);
        RegistFilterListener(StarFilterList);
        RegistFilterListener(TypeFilterList);
        RegistFilterListener(NationalityFilterList);
    }

    void RegistFilterListener(List<Toggle> filterList)
    {
        if(null != filterList)
        {
            for (int index = 0; index < filterList.Count; ++index)
            {
                filterList[index].onValueChanged.AddListener(OnFilterOpetionValueChange);
            }
        }
    }

    void OnFilterOpetionValueChange(bool value)
    {
        RefreshConditionFilter(SchoolFilter, SchoolFilterList, 1);
        RefreshConditionFilter(StarFilter, StarFilterList, 4);//4星开始
        RefreshConditionFilter(TypeFilter, TypeFilterList, 1);
        RefreshConditionFilter(NationalityFilter, NationalityFilterList, 1);

        RefreshFilteredCandidateHeros();
    }

    void RefreshConditionFilter(BitArray filter, List<Toggle> selectedValues, int startIndex)
    {
        if(null != filter && null != selectedValues)
        {
            filter.SetAll(false);
            for (int index = 0; index < selectedValues.Count; ++index)
            {
                filter.Set(index + startIndex, selectedValues[index].isOn);
            }
        }
    }

    bool HeroFullFillCondition(DataCenter.Hero hero)
    {
        if(null != hero)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
            {
                if(null != heroTemplate)
                {
                    return SchoolFilter[heroTemplate.School] || StarFilter[heroTemplate.Star] || TypeFilter[heroTemplate.HeroType] || NationalityFilter[heroTemplate.Nationality];
                }
            }
        }
        return false;
    }

    void OnFilterButtonClicked()
    {
        GUI_Tools.ObjectTool.ActiveObject(FilterOptionRootObject, !FilterOptionRootObject.activeInHierarchy);
    }

    void OnOrderDirectionButtonClicked()
    {
        DescendOrder = !DescendOrder;
    }

    bool HasCandidataCondition()
    {
        SchoolFilter.CopyTo(FilterCounterArray, 0);
        StarFilter.CopyTo(FilterCounterArray, 1);
        TypeFilter.CopyTo(FilterCounterArray, 2);
        NationalityFilter.CopyTo(FilterCounterArray, 3);

        return FilterCounterArray[0] != 0 || FilterCounterArray[1] != 0 || FilterCounterArray[2] != 0 || FilterCounterArray[3] != 0;
    }

    void RefreshFilteredCandidateHeros()
    {
        CandidateHeroList.Clear();
        List<DataCenter.Hero> freeHero = DataCenter.PlayerDataCenter.ExpeditionData.GetFreeHeroList();
        if(null != freeHero)
        {
            if(HasCandidataCondition())
            {
                for (int index = 0; index < freeHero.Count; ++index)
                {
                    if (HeroFullFillCondition(freeHero[index]))
                    {
                        CandidateHeroList.Add(freeHero[index]);
                    }
                }
            }
            else
            {
                CandidateHeroList.AddRange(freeHero);
            }
        }
        RefreshCandidateHeroPage();
    }
    #endregion
}
