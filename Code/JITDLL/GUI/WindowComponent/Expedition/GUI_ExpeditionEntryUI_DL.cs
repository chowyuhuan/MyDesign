using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ExpeditionEntryUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ExpeditionEntryUI dataComponent = gameObject.GetComponent<GUI_ExpeditionEntryUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExpeditionEntryUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ExpeditionTitle = dataComponent.ExpeditionTitle;
            ExpeditionDialog = dataComponent.ExpeditionDialog;
            ExpeditionButton = dataComponent.ExpeditionButton;
            ExpeditionButtonText = dataComponent.ExpeditionButtonText;
            ExpeditionMissionList = dataComponent.ExpeditionMissionList;
            MissionGroup = dataComponent.MissionGroup;
            AreaButtonList = dataComponent.AreaButtonList;
            MapRect = dataComponent.MapRect;
            UnitPerSecond = dataComponent.UnitPerSecond;
            AreaFocusPos = dataComponent.AreaFocusPos;

            dataComponent.ExpeditionButton.onClick.AddListener(OnExpeditionButtonClicked);
            dataComponent.MissionListTweenerButton.onClick.AddListener(OnMissionTweenButtonClicked);
            MissionListTweener = dataComponent.MissionListTweener;
            TweenerTrans = dataComponent.TweenerTrans;
        }
    }
    #endregion

    #region window logic
    public Text ExpeditionTitle;
    public Text ExpeditionDialog;
    public Button ExpeditionButton;
    public Text ExpeditionButtonText;
    public GameObject ExpeditionMissionList;
    public ToggleGroup MissionGroup;
    public List<GameObject> AreaButtonList;
    public ScrollRect MapRect;
    public float UnitPerSecond;
    public RectTransform AreaFocusPos;

    public GUI_TweenPosition MissionListTweener;
    public RectTransform TweenerTrans;

    GUI_VerticallayouGroupHelper_DL MissionListPage;
    GUI_LogicObjectPool ExpeditionItemPool;

    Dictionary<int, GUI_ExpeditionAreaButtonItem_DL> AreaButtonDic;

    GUI_ExpeditionMissionItem_DL CurrentExpeditionItem;
    bool MissionListShow = true;

    protected override void OnStart()
    {
        InitAreaButtonList();
        HideAreaButton();
        InitMissionList();
        RefreshMissionListPage();
        RefreshAreaButton();
        FocusOnArea(null);
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnBeginExpedition += OnBeginExpeditionRsp;
        DataCenter.PlayerDataCenter.OnCancelExpedition += OnAbortExpeditionRsp;
        DataCenter.PlayerDataCenter.OnEndExpedition += OnQuickExpeditionRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnCancelExpedition -= OnAbortExpeditionRsp;
        DataCenter.PlayerDataCenter.OnBeginExpedition -= OnBeginExpeditionRsp;
        DataCenter.PlayerDataCenter.OnEndExpedition -= OnQuickExpeditionRsp;
    }

    void OnBeginExpeditionRsp(int csvId)
    {
        RefreshMissionListPage();
        RefreshAreaButton();
        FocusOnArea(null);
    }

    void OnAbortExpeditionRsp(int csvId)
    {
        RefreshMissionListPage();
        RefreshAreaButton();
        FocusOnArea(null);
    }

    void OnQuickExpeditionRsp(DataCenter.GroupAddExpInfo groupAddExp, List<DataCenter.HeroAddExpInfo> heroAddExpList, List<DataCenter.AwardInfo> extraAwardList)
    {
        RefreshMissionListPage();
        RefreshAreaButton();
        FocusOnArea(null);
    }

    void OnMissionTweenButtonClicked()
    {
        MissionListTweener.Play(MissionListShow);
        TweenerTrans.Rotate(0f, 180f, 0f);
        MissionListShow = !MissionListShow;
    }

    void InitAreaButtonList()
    {
        AreaButtonDic = new Dictionary<int, GUI_ExpeditionAreaButtonItem_DL>();
        if (null != AreaButtonList)
        {
            for (int index = 0; index < AreaButtonList.Count; ++index)
            {
                GUI_ExpeditionAreaButtonItem_DL areaButton = AreaButtonList[index].GetComponent<GUI_ExpeditionAreaButtonItem_DL>();
                if (null != areaButton)
                {
                    AreaButtonDic.Add(areaButton.ToggleIndex, areaButton);
                    areaButton.InitAreaButton(OnAreaButtonSelect, OnAreaButtonDeselect);
                }
            }
        }
    }

    void RefreshAreaButton()
    {
        List<DataCenter.Expedition> expeditionList = DataCenter.PlayerDataCenter.ExpeditionData.GetExpeditionList();
        for (int index = 0; index < expeditionList.Count; ++index)
        {
            GUI_ExpeditionAreaButtonItem_DL areaButton;
            if (AreaButtonDic.TryGetValue(expeditionList[index].expeditionCSV.Id, out areaButton))
            {
                areaButton.ShowButton(true);
            }
        }
    }

    void HideAreaButton()
    {
        foreach (int id in AreaButtonDic.Keys)
        {
            var element = AreaButtonDic[id];
            element.ShowButton(false);
        }
    }

    void OnAreaButtonSelect(GUI_ExpeditionAreaButtonItem_DL areaButton)
    {
        FocusOnArea(areaButton);
        for (int index = 0; index < MissionListPage.ItemCount; ++index)
        {
            GUI_ScrollItem scrollItem = MissionListPage.GetAtIndex(index);
            if (null != scrollItem)
            {
                GUI_ExpeditionMissionItem_DL missionItem = scrollItem.LogicObject as GUI_ExpeditionMissionItem_DL;
                if (null != missionItem && missionItem.ExpeditionMissionTemplate.Id == areaButton.MissionTemplate.Id)
                {
                    missionItem.Select();
                }
            }
        }
    }

    void OnAreaButtonDeselect(GUI_ExpeditionAreaButtonItem_DL areaButton)
    {
        FocusOnArea(null);
        for (int index = 0; index < MissionListPage.ItemCount; ++index)
        {
            GUI_ScrollItem scrollItem = MissionListPage.GetAtIndex(index);
            if (null != scrollItem)
            {
                GUI_ExpeditionMissionItem_DL missionItem = scrollItem.LogicObject as GUI_ExpeditionMissionItem_DL;
                if (null != missionItem && missionItem.ExpeditionMissionTemplate.Id == areaButton.MissionTemplate.Id)
                {
                    missionItem.DeSelect();
                }
            }
        }
    }

    void FocusOnArea(int areaIndex)
    {
        GUI_ExpeditionAreaButtonItem_DL areaButton;
        if (AreaButtonDic.TryGetValue(areaIndex, out areaButton))
        {
            FocusOnArea(areaButton);
        }
        else
        {
            FocusOnArea(null);
        }
    }

    void FocusOnArea(GUI_ExpeditionAreaButtonItem_DL areaButton)
    {
        RefreshExpeditionInfo(areaButton);
        if (null != areaButton)
        {
            Vector2 buttonPos = GUI_Root_DL.Instance.UICamera.WorldToScreenPoint(areaButton.CachedTransform.position);
            Vector2 focusPos = GUI_Root_DL.Instance.UICamera.WorldToScreenPoint(AreaFocusPos.position);
            MapRect.velocity = (focusPos - buttonPos) / 0.5f;
        }
    }

    void ShowExpeditionInfo(CSV_b_expedition_template expedition)
    {
        if (null != expedition)
        {
            GUI_Tools.ObjectTool.ActiveObject(ExpeditionTitle.gameObject, true);
            ExpeditionTitle.text = expedition.Name;

            GUI_Tools.ObjectTool.ActiveObject(ExpeditionDialog.gameObject, true);
            ExpeditionDialog.text = expedition.Name;
        }
        else
        {
            GUI_Tools.ObjectTool.ActiveObject(ExpeditionTitle.gameObject, false);
            GUI_Tools.ObjectTool.ActiveObject(ExpeditionDialog.gameObject, false);
        }
    }

    void InitMissionList()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Exploration_Item", true, AssetManage.E_AssetType.UIPrefab);
        ExpeditionItemPool = new GUI_LogicObjectPool(go);

        MissionListPage = ExpeditionMissionList.GetComponent<GUI_VerticallayouGroupHelper_DL>();
        MissionListPage.SetScrollAction(DisplayExpeditionMission);
    }

    void RefreshMissionListPage()
    {
        MissionListPage.Clear();

        List<DataCenter.Expedition> expeditionList = DataCenter.PlayerDataCenter.ExpeditionData.GetExpeditionList();
        if (null != expeditionList)
        {
            for (int index = 0; index < expeditionList.Count; ++index)
            {
                MissionListPage.FillItem(expeditionList[index].CsvId);
            }
            MissionListPage.FillItemEnd();
        }
    }

    void DisplayExpeditionMission(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            DataCenter.Expedition expedition = DataCenter.PlayerDataCenter.ExpeditionData.GetExpedition(scrollItem.LogicIndex);
            if (null != expedition)
            {
                GUI_ExpeditionMissionItem_DL missionItem = ExpeditionItemPool.GetOneLogicComponent() as GUI_ExpeditionMissionItem_DL;
                if (null != missionItem)
                {
                    missionItem.ShowExpeditionMission(expedition, OnExpeditionSelect, OnExpeditionDeSelect);
                    missionItem.RegistToGroup(MissionGroup);
                    scrollItem.SetTarget(missionItem);
                }
            }
        }
    }

    void OnExpeditionSelect(GUI_ExpeditionMissionItem_DL missionItem)
    {
        CurrentExpeditionItem = missionItem;
        if (null != missionItem)
        {
            GUI_ExpeditionAreaButtonItem_DL areaButton;
            if (AreaButtonDic.TryGetValue(missionItem.ExpeditionMissionTemplate.Id, out areaButton))
            {
                areaButton.Select();
            }
        }
        FocusOnArea(missionItem.ExpeditionMissionTemplate.Id);
        RefreshExpeditionButton(missionItem);
    }

    void OnExpeditionDeSelect(GUI_ExpeditionMissionItem_DL missionItem)
    {
        if (CurrentExpeditionItem == missionItem)
        {
            CurrentExpeditionItem = null;
        }
        if (null != missionItem)
        {
            GUI_ExpeditionAreaButtonItem_DL areaButton;
            if (AreaButtonDic.TryGetValue(missionItem.ExpeditionMissionTemplate.Id, out areaButton))
            {
                areaButton.DeSelect();
            }
        }
        FocusOnArea(-1);
        RefreshExpeditionButton(null);
    }

    void RefreshExpeditionButton(GUI_ExpeditionMissionItem_DL missionItem)
    {
        if (null != missionItem)
        {
            ExpeditionButton.interactable = true;
            if (missionItem.Expedition.FinishTime > 0)
            {
                if (missionItem.Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)
                {
                    TextLocalization.SetTextById(ExpeditionButtonText, TextId.Expedition_OnGoing);
                }
                else
                {
                    TextLocalization.SetTextById(ExpeditionButtonText, TextId.Expedition_Finish);
                }
            }
            else
            {
                TextLocalization.SetTextById(ExpeditionButtonText, TextId.Expedition_Prepare);
            }
        }
        else
        {
            ExpeditionButton.interactable = false;
            TextLocalization.SetTextById(ExpeditionButtonText, TextId.Expedition_Prepare);
        }
    }

    void RefreshExpeditionInfo(GUI_ExpeditionAreaButtonItem_DL areaItem)
    {
        if (null != areaItem && null != areaItem.MissionTemplate)
        {
            ExpeditionDialog.text = areaItem.MissionTemplate.Dialog;
            CSV_b_expedition_template expeditionTemplate = CSV_b_expedition_template.FindData(areaItem.MissionTemplate.QuestGroup);
            if(null != expeditionTemplate)
            {                
                ExpeditionTitle.text = expeditionTemplate.Name;
                GUI_Tools.ObjectTool.ActiveObject(ExpeditionDialog.gameObject, true);
            }
            else
            {
                GUI_Tools.ObjectTool.ActiveObject(ExpeditionDialog.gameObject, false);
            }
        }
        else
        {
            ExpeditionTitle.text = "";
            ExpeditionDialog.text = "";
            GUI_Tools.ObjectTool.ActiveObject(ExpeditionDialog.gameObject, false);
        }
    }

    void OnExpeditionButtonClicked()
    {
        if (null != CurrentExpeditionItem)
        {
            GUI_ExpeditionUI_DL expeditionUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExpeditionUI_DL>("UI_Prepare_Exploration", false);
            if (null != expeditionUI)
            {
                expeditionUI.ShowExpedition(CurrentExpeditionItem.Expedition, CurrentExpeditionItem.ExpeditionMissionTemplate);
            }
        }
    }
    #endregion
}
