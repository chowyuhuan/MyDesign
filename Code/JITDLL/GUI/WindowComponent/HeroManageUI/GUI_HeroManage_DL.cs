using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_HeroManage_DL : GUI_Window_DL
{
    protected override void OnStart()
    {
        InitHeroTabPage();
        InitOrderMenuItem();
        if(HeroTabPageList.Count > 0)
        {
            HeroTabPageList[0].Select();
        }
    }

    #region hero tab page
    public Text HeroCount;
    List<GameObject> HeroTabPageObject;
    public List<GUI_ToggleTabPage_DL> HeroTabPageList = new List<GUI_ToggleTabPage_DL>();
    GameObject GridHelperObject;
    public GUI_GridLayoutGroupHelper_DL GridLayoutHelper;
    int _CurrentPage = -1;
    GUI_LogicObjectPool _HeroSimpleInfoPool;
    void InitHeroTabPage()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/HeroManageSimpleInfo", true, AssetManage.E_AssetType.UIPrefab);
#if UNITY_EDITOR
        Debug.Assert(null != go);
#endif
        _HeroSimpleInfoPool = new GUI_LogicObjectPool(go);

        for (int index = 0; index < HeroTabPageObject.Count; ++index)
        {
            HeroTabPageList.Add(HeroTabPageObject[index].GetComponent<GUI_ToggleTabPage_DL>());
        }

        GridLayoutHelper = GridHelperObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        GridLayoutHelper.SetScrollAction(this.DisplayItem);
        for (int index = 0; index < HeroTabPageList.Count; ++index)
        {
            HeroTabPageList[index].Init(index, GridLayoutHelper, OnPageSelect, "", false);
        }
        RefreshRegimentInfo();
    }

    public void OnExtendHeroBagButtonClicked()
    {
        GUI_ExtendBagUI_DL extendUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExtendBagUI_DL>("UI_Extend_Hero", false);
        extendUI.ExtendBag(PbCommon.EExendBagType.E_Extend_Hero_Bag);
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnHeroListChange += OnHeroListChange;
        DataCenter.PlayerDataCenter.OnHeroDataChange += OnHeroDataChange;
        DataCenter.PlayerDataCenter.OnChangeRepresentHero += OnChangeRegimentHero;
        DataCenter.PlayerDataCenter.OnExtandBag += OnExtendHeroBagRsp;
        DataCenter.PlayerDataCenter.OnDismissHero += OnFireHeroRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnHeroListChange -= OnHeroListChange;
        DataCenter.PlayerDataCenter.OnHeroDataChange -= OnHeroDataChange;
        DataCenter.PlayerDataCenter.OnChangeRepresentHero -= OnChangeRegimentHero;
        DataCenter.PlayerDataCenter.OnExtandBag -= OnExtendHeroBagRsp;
        DataCenter.PlayerDataCenter.OnDismissHero -= OnFireHeroRsp;
    }

    void OnExtendHeroBagRsp(PbCommon.EExendBagType bagType)
    {
        if (bagType == PbCommon.EExendBagType.E_Extend_Hero_Bag)
        {
            RefreshRegimentInfo();
        }
    }

    void OnChangeRegimentHero(uint heroServerId)
    {
        OnSetLeaderHeroButtonClicked();
        OnHeroDataChange(heroServerId);
    }

    void OnHeroListChange()
    {
        RefreshPage();
        RefreshRegimentInfo();
    }

    void RefreshPage()
    {
        int currentPage = _CurrentPage;
        _CurrentPage = -1;
        HeroTabPageList[currentPage].RefreshPage();
    }

    public void OnHeroDataChange(uint heroId)
    {
        int currentPage = _CurrentPage;
        _CurrentPage = -1;
        HeroTabPageList[currentPage].RefreshPage();

    }

    void RefreshRegimentInfo()
    {
        HeroCount.text = DataCenter.PlayerDataCenter.HeroList.Count.ToString() + "/" + DataCenter.PlayerDataCenter.MaxHeroCount.ToString();
    }

    void OnPageSelect(int index)
    {
        if (index == _CurrentPage)
        {
            return;
        }
        _CurrentPage = index;
        if (_CurrentPage == 0)
        {
            ShowAllHero();
        }
        else
        {
            ShowHeroBySchool(_CurrentPage - 1);
        }
        SortHero();
    }

    void ShowAllHero()
    {
        for (int index = 0; index < DataCenter.PlayerDataCenter.HeroList.Count; ++index)
        {
            GridLayoutHelper.FillItem(index);
        }
        GridLayoutHelper.FillItem(DataCenter.PlayerDataCenter.HeroList.Count);
        GridLayoutHelper.FillItemEnd();
    }

    void ShowHeroBySchool(int school)
    {

        for (int index = 0; index < DataCenter.PlayerDataCenter.HeroList.Count; ++index)
        {
            CSV_b_hero_template ht = CSV_b_hero_template.FindData((int)DataCenter.PlayerDataCenter.HeroList[index].CsvId);
            if (ht.School == school)
            {
                GridLayoutHelper.FillItem(index);
            }
        }
        GridLayoutHelper.FillItemEnd();
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            if (scrollItem.LogicIndex == (int)DataCenter.PlayerDataCenter.HeroList.Count)
            {
                GUI_HeroManageSimpleInfo_DL hmsi = _HeroSimpleInfoPool.GetOneLogicComponent() as GUI_HeroManageSimpleInfo_DL;
                hmsi.SetHeroManageSimpleInfo(null, OnSelectHero, OnDeselectHero, _FiringHero);
                scrollItem.SetTarget(hmsi);
                hmsi.AsButton();
                HeroTabPageList[_CurrentPage].ShowItem(hmsi);
            }
            else
            {
                GUI_HeroManageSimpleInfo_DL hmsi = _HeroSimpleInfoPool.GetOneLogicComponent() as GUI_HeroManageSimpleInfo_DL;
                hmsi.SetHeroManageSimpleInfo(DataCenter.PlayerDataCenter.HeroList[scrollItem.LogicIndex], OnSelectHero, OnDeselectHero, _FiringHero);
                if (_FiringHero)
                {
                    hmsi.AsToggle();
                    if (_SelectedHeroes.Contains(hmsi.Hero.ServerId))
                    {
                        hmsi.Select();
                    }
                }
                else
                {
                    hmsi.DeSelect();
                    hmsi.AsButton();
                }
                scrollItem.SetTarget(hmsi);
                HeroTabPageList[_CurrentPage].ShowItem(hmsi);
            }
        }
    }
    #endregion

    #region hero order manu
    public GameObject OrderMenu;
    List<GameObject> HeroOrderMenuItemObjectList;
    public List<GUI_HeroOrderMenuItem_DL> HeroOrderMenuItems = new List<GUI_HeroOrderMenuItem_DL>();
    public Text CurrentSelectOrderType;
    bool _DisplayOrderMenu = false;
    public bool DecreaseOrder = true;
    int _SelectedMenuItemIndex = 0;
    public RectTransform OrderDirectonIcon;

    void InitOrderMenuItem()
    {
        for (int index = 0; index < HeroOrderMenuItemObjectList.Count; ++index)
        {
            HeroOrderMenuItems.Add(HeroOrderMenuItemObjectList[index].GetComponent<GUI_HeroOrderMenuItem_DL>());
        }
        for (int index = 0; index < HeroOrderMenuItems.Count; ++index)
        {
            HeroOrderMenuItems[index].SetOrderMenuItem(OnOrderMenuItemSelected, index);
        }
        if (HeroOrderMenuItems.Count > 0)
        {
            HeroOrderMenuItems[0].Select();
        }
        OrderMenu.SetActive(false);
    }

    public void OnOrderManuButtonClicked()
    {
        _DisplayOrderMenu = !_DisplayOrderMenu;
        OrderMenu.SetActive(_DisplayOrderMenu);
    }

    public void OnOrderMenuItemSelected(int index)
    {
        _SelectedMenuItemIndex = index;
        SortHero();
        OnOrderManuButtonClicked();
    }

    void SortHero()
    {
        CurrentSelectOrderType.text = HeroOrderMenuItems[_SelectedMenuItemIndex].MenuText.text;
        GridLayoutHelper.Sort(HeroOrderMenuItems[_SelectedMenuItemIndex].CompareFunc, DecreaseOrder);
    }

    public void OnOrderDirectionClicked()
    {
        OrderDirectonIcon.Rotate(0f, 0f, 180f);
        DecreaseOrder = !DecreaseOrder;
        SortHero();
    }
    #endregion

    #region manage button area
    bool _DisplayFireButton = false;
    bool _FiringHero = false;
    List<uint> _SelectedHeroes = new List<uint>();
    public GameObject ManageButtonArea;
    public GameObject FireButtonArea;
    public GameObject RegimentLeaderHero;
    public Text FireHonor;
    bool _SelectRegimentLeader = false;
    public void OnMakeTeamButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnSetLeaderHeroButtonClicked()
    {
        _SelectRegimentLeader = !_SelectRegimentLeader;
        RegimentLeaderHero.SetActive(_SelectRegimentLeader);
    }

    public void OnFireButtonClicked()
    {
        _SelectedHeroes.Clear();
        RefreshFireHonor();
        _FiringHero = !_FiringHero;
        _DisplayFireButton = !_DisplayFireButton;
        ManageButtonArea.SetActive(!_DisplayFireButton);
        FireButtonArea.SetActive(_DisplayFireButton);
        RefreshPage();
    }

    void RefreshFireHonor()
    {
        int fireHonor = 0;
        for (int index = 0; index < _SelectedHeroes.Count; ++index)
        {
            DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(_SelectedHeroes[index]);
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
            CSV_b_hero_limit heroLimit = CSV_b_hero_limit.FindData(heroTemplate.Star);
            fireHonor += (heroLimit.DismissBase + heroLimit.DismissGrowth * (int)hero.Level);
        }
        FireHonor.text = fireHonor.ToString();
    }

    void OnSelectHero(GUI_HeroManageSimpleInfo_DL hss)
    {
        if (null != hss)
        {
            if (_FiringHero)
            {
                if (!_SelectedHeroes.Contains(hss.Hero.ServerId))
                {
                    _SelectedHeroes.Add(hss.Hero.ServerId);
                    RefreshFireHonor();
                }
            }
            else
            {
                hss.DeSelect();
                if (_SelectRegimentLeader)
                {
                    gsproto.ChangeRepresentHeroReq req = new gsproto.ChangeRepresentHeroReq();
                    req.session_id = DataCenter.PlayerDataCenter.SessionId;
                    req.hero_id = hss.Hero.ServerId;
                    Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
                }
                else
                {
                    hss.DisplayHeroDetail();
                }
            }
        }
    }

    void OnDeselectHero(GUI_HeroManageSimpleInfo_DL hss)
    {
        if (null != hss)
        {
            if (_FiringHero)
            {
                if (_SelectedHeroes.Contains(hss.Hero.ServerId))
                {
                    _SelectedHeroes.Remove(hss.Hero.ServerId);
                    RefreshFireHonor();
                }
            }
        }
    }

    public void OnConfirmFireButtonClicked()
    {
        gsproto.DismissHeroReq req = new gsproto.DismissHeroReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        req.hero_ids.AddRange(_SelectedHeroes);
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnFireHeroRsp(uint honor, List<uint> heroServerIds)
    {
        _SelectedHeroes.Clear();
        RefreshFireHonor();
    }

    public void OnCancelFireButtonClicked()
    {
        _FiringHero = !_FiringHero;
        _DisplayFireButton = !_DisplayFireButton;
        ManageButtonArea.SetActive(!_DisplayFireButton);
        FireButtonArea.SetActive(_DisplayFireButton);
        _SelectedHeroes.Clear();
        RefreshFireHonor();
        RefreshPage();
    }
    #endregion
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroManage dataComponent = gameObject.GetComponent<GUI_HeroManage>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroManage,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HeroCount = dataComponent.HeroCount;
            GridHelperObject = dataComponent.GridLayoutHelper;
            HeroTabPageObject = dataComponent.HeroTabPageList;
            OrderMenu = dataComponent.OrderMenu;
            HeroOrderMenuItemObjectList = dataComponent.HeroOrderMenuItems;

            CurrentSelectOrderType = dataComponent.CurrentSelectOrderType;
            DecreaseOrder = dataComponent.DecreaseOrder;
            OrderDirectonIcon = dataComponent.OrderDirectonIcon;
            ManageButtonArea = dataComponent.ManageButtonArea;
            FireButtonArea = dataComponent.FireButtonArea;
            RegimentLeaderHero = dataComponent.RegimentLeaderHero;
            FireHonor = dataComponent.FireHonor;

            dataComponent.ExtendHeroBagButton.onClick.AddListener(OnExtendHeroBagButtonClicked);
            dataComponent.OrderButton.onClick.AddListener(OnOrderManuButtonClicked);
            dataComponent.OrderDirectionButton.onClick.AddListener(OnOrderDirectionClicked);
            dataComponent.CreateTeamButton.onClick.AddListener(OnMakeTeamButtonClicked);
            dataComponent.SetRepresentHero.onClick.AddListener(OnSetLeaderHeroButtonClicked);
            dataComponent.ConfirmSetRepresentHeroButton.onClick.AddListener(OnSetLeaderHeroButtonClicked);
            dataComponent.FireHeroButton.onClick.AddListener(OnFireButtonClicked);
            dataComponent.ConfirmFireHeroButton.onClick.AddListener(OnConfirmFireButtonClicked);
            dataComponent.CancelFireHeroButton.onClick.AddListener(OnCancelFireButtonClicked);
        }
    }
}
