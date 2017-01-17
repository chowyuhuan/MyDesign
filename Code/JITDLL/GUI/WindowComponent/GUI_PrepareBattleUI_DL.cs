using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_PrepareBattleUI_DL : GUI_Window_DL, GUI_IOnTopHandler
{
    GameObject GridHelperObject;
    public GUI_GridLayoutGroupHelper_DL GridHelper;
    public Text SelectedLevel;
    public GameObject SelectCaptainMask;
    List<GameObject> HeroTabPageObjectList;
    public List<GUI_ToggleTabPage_DL> HeroTabPageList = new List<GUI_ToggleTabPage_DL>();
    List<GameObject> HeroDisplayObjectList;
    public List<GUI_HeroBattleSimpleInfo_DL> HeroDisplayList = new List<GUI_HeroBattleSimpleInfo_DL>();
    List<uint> _SelectedHero = new List<uint>();
    int _CurrentPage = -1;
    int _CaptainIndex = 0;
    bool _SelectingCaptain = false;
    GUI_LogicObjectPool _HeroSelectInfoPool;

    #region boss info
    List<GameObject> BossInfoObjectList;
    public List<GUI_BossInfo_DL> BossInfoList = new List<GUI_BossInfo_DL>();

    void ShowBossInfo(List<int> bossList)
    {
        for (int bossInfoIndex = 0; bossInfoIndex < BossInfoObjectList.Count; ++bossInfoIndex)
        {
            BossInfoList.Add(BossInfoObjectList[bossInfoIndex].GetComponent<GUI_BossInfo_DL>());
        }

        int index = 0;
        for (; index < bossList.Count && index < BossInfoList.Count; ++index)
        {
            BossInfoList[index].SetBossInfo(bossList[index]);
            BossInfoList[index].gameObject.SetActive(true);
        }
        for (; index < BossInfoList.Count; ++index)
        {
            BossInfoList[index].gameObject.SetActive(false);
        }
    }
    #endregion

    protected override void OnStart()
    {
        GridHelper = GridHelperObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        GridHelper.SetScrollAction(DisplayItem);
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/HeroSelectSimpleInfo", true, AssetManage.E_AssetType.UIPrefab);
        _HeroSelectInfoPool = new GUI_LogicObjectPool(go);

        ShowBossInfo(GUI_BattleManager.Instance.GetCurLevelBossList());

        HeroTabPageList = new List<GUI_ToggleTabPage_DL>();
        for (int index = 0; index < HeroTabPageObjectList.Count; ++index)
        {
            HeroTabPageList.Add(HeroTabPageObjectList[index].GetComponent<GUI_ToggleTabPage_DL>());
        }
        for (int index = 0; index < HeroTabPageList.Count; ++index)
        {
            HeroTabPageList[index].Init(index, GridHelper, OnPageSelect, "");
        }

        InitTeamInfo();
        HeroTabPageList[0].Select();
        SelectedLevel.text = GUI_BattleManager.Instance.SelectedLevel.LevelName;
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnEnterPass += OnEnterLevelRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnEnterPass -= OnEnterLevelRsp;
    }

    void DoDefaultHeroSelect(int heroIndex)
    {
        if (heroIndex >= 0)
        {
            if (DataCenter.PlayerDataCenter.HeroList.Count > heroIndex)
            {
                _SelectedHero.Add(DataCenter.PlayerDataCenter.HeroList[heroIndex].ServerId);
            }
        }
    }

    void OnPageSelect(int index)
    {
        if (index == _CurrentPage)
        {
            return;
        }
#if UNITY_EDITOR
        Debug.Assert(index >= 0 && index < HeroTabPageList.Count);
#endif
        _CurrentPage = index;
        if (index == 0)
        {
            ShowAllHero();
        }
        else
        {
            ShowHeroBySchool(index);
        }
    }

    void ShowAllHero()
    {
        GridHelper.FillPage(DataCenter.PlayerDataCenter.HeroList.Count);
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            GUI_HeroSelectSimpleInfo_DL hss = _HeroSelectInfoPool.GetOneLogicComponent() as GUI_HeroSelectSimpleInfo_DL;
            DataCenter.Hero hero = DataCenter.PlayerDataCenter.HeroList[scrollItem.LogicIndex];
            hss.SetHeroInfo(hero, OnSelectHero, OnDeSelectHero, OnRefreshSelectionInfo);
            if (_SelectedHero.Count > 0)
            {
                if (_SelectedHero.Contains(hero.ServerId))
                {
                    hss.Select();
                }
                if (_SelectedHero[_CaptainIndex] == hero.ServerId)
                {
                    hss.TagAsCaptain(true);
                }
            }
            scrollItem.SetTarget(hss);
            HeroTabPageList[_CurrentPage].ShowItem(hss);
        }
    }

    void ShowHeroBySchool(int school)
    {
        List<int> heroList = new List<int>();
        for (int index = 0; index < DataCenter.PlayerDataCenter.HeroList.Count; ++index)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(DataCenter.PlayerDataCenter.HeroList[index].CsvId);
            if (null != heroTemplate && heroTemplate.School == school)
            {
                GridHelper.FillItem(index);
            }
        }

        GridHelper.FillItemEnd();
    }

    void OnSelectHero(GUI_HeroSelectSimpleInfo_DL hss)
    {
        if (null != hss && null != hss.Hero)
        {
            if (_SelectedHero.Count < 3)
            {
                if (!_SelectedHero.Contains(hss.Hero.ServerId))
                {
                    _SelectedHero.Add(hss.Hero.ServerId);
                    ReOrderSelectedHero();
                    DisPlaySelectedHero();
                    if (_SelectedHero.Count == 1)
                    {
                        _CaptainIndex = 0;
                    }
                    SetCaptain();
                }
                GridHelper.RefreshPageData();
            }
            else if (!_SelectedHero.Contains(hss.Hero.ServerId))
            {
                GUI_MessageManager.Instance.ShowErrorTip("已选择三名勇士");//策划有具体方案后替换
            }
        }
    }

    void OnDeSelectHero(GUI_HeroSelectSimpleInfo_DL hss)
    {
        if (null != hss && null != hss.Hero)
        {
            int index = _SelectedHero.IndexOf(hss.Hero.ServerId);
            if (index >= 0 && index < _SelectedHero.Count)
            {
                uint captain = _SelectedHero[_CaptainIndex];
                _SelectedHero.Remove(hss.Hero.ServerId);
                if (_SelectedHero.Count == 0)
                {
                    _CaptainIndex = -1;
                }
                else
                {
                    if (captain == hss.Hero.ServerId)
                    {
                        _CaptainIndex = 0;
                    }
                    else
                    {
                        _CaptainIndex = _SelectedHero.IndexOf(captain);
                    }
                }
                ReOrderSelectedHero();
                DisPlaySelectedHero();
                SetCaptain();
                GridHelper.RefreshPageData();
            }
        }
    }

    void OnRefreshSelectionInfo(GUI_HeroSelectSimpleInfo_DL hss)
    {
        if (null != hss && null != hss.Hero)
        {
            if (_SelectedHero.Contains(hss.Hero.ServerId))
            {
                if (!hss.IsSelect)
                {
                    hss.Select();
                }
                if (_SelectedHero.IndexOf(hss.Hero.ServerId) == _CaptainIndex)
                {
                    hss.TagAsCaptain(true);
                }
                else
                {
                    hss.TagAsCaptain(false);
                }
            }
            else
            {
                if (hss.IsSelect)
                {
                    hss.DeSelect();
                    hss.TagAsCaptain(false);
                }
            }
        }
    }

    void RepeatSelect()
    {
        Debug.Log("RepeatSelect");
    }

    void SetCaptain()
    {
        for (int index = 0; index < HeroDisplayList.Count; ++index)
        {
            HeroDisplayList[index].TagAsCaptain(index == _CaptainIndex);
        }
    }

    void InitDisplayHeroList()
    {
        if(HeroDisplayList.Count < 1)
        {
            for(int index = 0; index < HeroDisplayObjectList.Count; ++index)
            {
                HeroDisplayList.Add(HeroDisplayObjectList[index].GetComponent<GUI_HeroBattleSimpleInfo_DL>());
            }
        }
    }

    void DisPlaySelectedHero()
    {
        InitDisplayHeroList();
        int index = 0;
        for (; index < _SelectedHero.Count && index < HeroDisplayList.Count; ++index)
        {
            HeroDisplayList[index].SetHeroSimgpleInfo(DataCenter.PlayerDataCenter.GetHero(_SelectedHero[index]), OnBattleHeroClicked);
        }
        for (; index < HeroDisplayList.Count; ++index)
        {
            HeroDisplayList[index].SetHeroSimgpleInfo(null, null);
        }
    }

    //按照前中后排的规则重新对英雄显示排序
    void ReOrderSelectedHero()
    {

    }

    public void OnSelectCaptainBtnClicked()
    {
        if (_SelectingCaptain)
        {
            SelectCaptain(false);
        }
        else
        {
            SelectCaptain(true);
        }

    }

    void SelectCaptain(bool select)
    {
        _SelectingCaptain = select;
        SelectCaptainMask.SetActive(select);
        for (int index = 0; index < _SelectedHero.Count; ++index)
        {
            HeroDisplayList[index].AlertCaptain(select);
        }
    }

    void OnBattleHeroClicked(DataCenter.Hero hero)
    {
        if (null != hero)
        {
            int index = _SelectedHero.IndexOf(hero.ServerId);
            if (_SelectingCaptain)
            {
                if (index != _CaptainIndex)
                {
                    _CaptainIndex = index;
                    SetCaptain();
                }
                SelectCaptain(false);
            }
            else
            {
                uint captain = _SelectedHero[_CaptainIndex];
                _SelectedHero.Remove(hero.ServerId);
                if (_SelectedHero.Count == 0)
                {
                    _CaptainIndex = -1;
                }
                else
                {
                    if (captain == hero.ServerId)
                    {
                        _CaptainIndex = 0;
                    }
                    else
                    {
                        _CaptainIndex = _SelectedHero.IndexOf(captain);
                    }
                }
                ReOrderSelectedHero();
                DisPlaySelectedHero();
                SetCaptain();
            }
            GridHelper.RefreshPageData();
        }
    }

    public void OnBeginButtonClicked()
    {
        if (_SelectedHero.Count >= 3)
        {
            gsproto.EnterPassReq enterPassReq = new gsproto.EnterPassReq();
            enterPassReq.session_id = DataCenter.PlayerDataCenter.SessionId;
            enterPassReq.pass_id = (uint)GUI_BattleManager.Instance.SelectedLevel.LevelId;
            enterPassReq.leader_id = _SelectedHero[_CaptainIndex];
            enterPassReq.team_members.AddRange(_SelectedHero);
            enterPassReq.team_goddess = TeamInfo.Goddess;

            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, enterPassReq);
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("选择勇士不足3名");
        }
    }

    void OnEnterLevelRsp()
    {
        GUI_BattleManager.Instance.UseDynamicLoadMode = true;//有完整的流程执行后删除
        List<DataCenter.Hero> selectedHero = new List<DataCenter.Hero>();
        for (int index = 0; index < _SelectedHero.Count && index < HeroDisplayList.Count; ++index)
        {
            selectedHero.Add(DataCenter.PlayerDataCenter.GetHero(_SelectedHero[index]));
        }
        GUI_BattleManager.Instance.SelectHero(selectedHero, _CaptainIndex);
        GUI_Manager.Instance.HideAllWindow();
        PL_Manager_DL.Instance.LoadSceneAsync("Battle", true);
    }

    #region goddess info
    public Image GoddessIcon;
    public Button GoddessButton;
    DataCenter.TeamInfo TeamInfo;

    public void OnTop()
    {
        RefreshGoddessInfo();
    }

    void InitTeamInfo()
    {
        TeamInfo = GetAdventureTeam();
        _SelectedHero.Clear();
        if (null != TeamInfo && TeamInfo.Members.Count > 0)
        {
            _SelectedHero.AddRange(TeamInfo.Members);
            if(_SelectedHero.Count > 0)
            {
                _CaptainIndex = _SelectedHero.IndexOf(TeamInfo.LeaderId);
            }
        }
        else
        {
            _CaptainIndex = 0;
            DoDefaultHeroSelect(0);
            DoDefaultHeroSelect(1);
            DoDefaultHeroSelect(2);
        }
        DisPlaySelectedHero();
        SetCaptain();
        RefreshGoddessInfo();
    }

    void RefreshGoddessInfo()
    {
        if(null != TeamInfo)
        {
            CSV_c_goddess_config goddessConfig = CSV_c_goddess_config.FindData((int)TeamInfo.Goddess);
            if (null != goddessConfig)
            {
                GUI_Tools.IconTool.SetIcon(goddessConfig.HeadIconAtlas, goddessConfig.HeadIcon, GoddessIcon);
            }
            else
            {
                List<DataCenter.Goddess> goddessList = DataCenter.PlayerDataCenter.GoddessData.GetGoddessList();
                if(null != goddessList)
                {
                    for (int index = 0; index < goddessList.Count; ++index )
                    {
                        if(goddessList[index].LockChapter == 0)
                        {
                            TeamInfo.Goddess = (uint)goddessList[index].csvId;
                        }
                    }
                }
            }
        }
    }

    DataCenter.TeamInfo GetAdventureTeam()
    {
        DataCenter.TeamInfo teamInfo = null;
        switch ((E_ChapterType)GUI_BattleManager.Instance.SelectedChapter.ChapterType)
        {
            case E_ChapterType.Plot:
                {
                    teamInfo = DataCenter.PlayerDataCenter.TeamCollectionData.GetMissionTeam();
                    break;
                }
        }
        return teamInfo;
    }

    void OnGoddessButtonClicked()
    {
        DataCenter.TeamInfo teamInfo = GetAdventureTeam();
        GUI_GoddessHouseUI_DL goddessHouse = GUI_Manager.Instance.ShowWindowWithName<GUI_GoddessHouseUI_DL>("UI_Goddess", false);
        if (null != goddessHouse)
        {
            goddessHouse.InviteGoddess(teamInfo);
        }
    }
    #endregion

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_PrepareBattleUI dataComponent = gameObject.GetComponent<GUI_PrepareBattleUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_PrepareBattleUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GridHelperObject = dataComponent.GridHelper;
            HeroTabPageObjectList = dataComponent.HeroTabPageList;
            SelectedLevel = dataComponent.SelectedLevel;
            SelectCaptainMask = dataComponent.SelectCaptainMask;
            HeroDisplayObjectList = dataComponent.HeroDisplayList;
            BossInfoObjectList = dataComponent.BossInfoList;
            dataComponent.SelectCaptainButton.onClick.AddListener(OnSelectCaptainBtnClicked);
            dataComponent.BeginButton.onClick.AddListener(OnBeginButtonClicked);
            dataComponent.GoddessButton.onClick.AddListener(OnGoddessButtonClicked);
            GoddessIcon = dataComponent.GoddessIcon;
            GoddessButton = dataComponent.GoddessButton;
        }
    }
}
