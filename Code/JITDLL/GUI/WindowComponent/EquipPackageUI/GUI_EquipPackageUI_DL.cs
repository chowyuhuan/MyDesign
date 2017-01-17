using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GUI_EquipPackageUI_DL : GUI_Window_DL
{
    protected override void OnStart()
    {
        InitHeroTabPage();
        InitEquipScrollPage();
        RefreshDisplay();

        if (HeroTabPageList.Count > 0)
        {
            HeroTabPageList[0].Select();
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnExtandBag += OnExtendEquipBagRsp;
        DataCenter.PlayerDataCenter.OnEquipListChange += OnEquipListChange;
        DataCenter.PlayerDataCenter.OnSellItem += OnSellRsp;
        DataCenter.PlayerDataCenter.OnWeaponBreak += OnResolveRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnExtandBag -= OnExtendEquipBagRsp;
        DataCenter.PlayerDataCenter.OnEquipListChange -= OnEquipListChange;
        DataCenter.PlayerDataCenter.OnSellItem -= OnSellRsp;
        DataCenter.PlayerDataCenter.OnWeaponBreak -= OnResolveRsp;
    }

    #region equip toggle pag area
    List<GameObject> HeroTabPageObjectList;
    public List<GUI_ToggleTabPage_DL> HeroTabPageList;
    GameObject GridHelperObject;
    public GUI_GridLayoutGroupHelper_DL HeroListHelper;
    GameObject DefaultDisplayObject;
    public GUI_ExtendToggleItem_DL DefaultEquipDisplay;
    public GameObject ResolveButtonObject;
    int _CurrentPage = -1;
    GUI_LogicObjectPool _HeroItemPool;
    bool _DisplayWeap = true;
    GameObject WeaponPageToggleObject;
    GameObject RingPageToggleObject;
    DataCenter.Hero _CurrentHero;
    public DataCenter.Hero CurrentHero { 
        get
        {
            return _CurrentHero;
        }
        protected set
        {
            _CurrentHero = value;
            if(null != _CurrentHero)
            {
                bool prioritySchoolChange = false;
                CSV_b_hero_template newHeroTemplate = CSV_b_hero_template.FindData(_CurrentHero.CsvId);
                if(null != CurrentHeroTemplate && null != newHeroTemplate && CurrentHeroTemplate.School != newHeroTemplate.School)
                {
                    prioritySchoolChange = true;
                }
                CurrentHeroTemplate = newHeroTemplate;
                if(prioritySchoolChange)
                {
                    RefreshEquipScrollPage();
                }
                if(DisplayWeapon())
                {
                    FocusOnEquip(_CurrentHero.Weapon);
                }
                else
                {
                    FocusOnEquip(_CurrentHero.Ring);
                }
            }
            else
            {
                CurrentHeroTemplate = null;
            }
            HeroChange();
        }
    }
    public CSV_b_hero_template CurrentHeroTemplate { get; protected set; }

    void InitHeroTabPage()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Train_HeroWeaponManage_Herolist_Item", true, AssetManage.E_AssetType.UIPrefab);
        _HeroItemPool = new GUI_LogicObjectPool(go);

        WeaponPageToggleObject.GetComponent<GUI_ExtendToggleItem_DL>().OnSelectItem.AddListener(OnWeaponButtonClicked);
        RingPageToggleObject.GetComponent<GUI_ExtendToggleItem_DL>().OnSelectItem.AddListener(OnRingButtonClicked);

        HeroTabPageList = new List<GUI_ToggleTabPage_DL>();
        for (int index = 0; index < HeroTabPageObjectList.Count; ++index)
        {
            HeroTabPageList.Add(HeroTabPageObjectList[index].GetComponent<GUI_ToggleTabPage_DL>());
        }
        HeroListHelper = GridHelperObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        DefaultEquipDisplay = DefaultDisplayObject.GetComponent<GUI_ExtendToggleItem_DL>();
        HeroListHelper.SetScrollAction(DisplayHeroItem);

        for (int index = 0; index < HeroTabPageList.Count; ++index)
        {
            HeroTabPageList[index].Init(index, HeroListHelper, OnPageSelect, "", false);
        }
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
            ShowHeroBySchool(_CurrentPage);
        }
        RefreshDisplay();
    }

    void ShowAllHero()
    {
        DataCenter.Hero firstHero = null;
        for (int index = 0; index < DataCenter.PlayerDataCenter.HeroList.Count; ++index)
        {
            if (null == firstHero)
            {
                firstHero = DataCenter.PlayerDataCenter.HeroList[index];
            }
            HeroListHelper.FillItem((int)DataCenter.PlayerDataCenter.HeroList[index].ServerId);
        }
        HeroListHelper.FillItemEnd();
        FocusOnHero(firstHero);
    }

    void ShowHeroBySchool(int school)
    {
        DataCenter.Hero firstHero = null;
        for (int index = 0; index < DataCenter.PlayerDataCenter.HeroList.Count; ++index)
        {
            if (DataCenter.PlayerDataCenter.HeroList[index].HeroCSV.School == school)
            {
                if (null == firstHero)
                {
                    firstHero = DataCenter.PlayerDataCenter.HeroList[index];
                }
                HeroListHelper.FillItem((int)DataCenter.PlayerDataCenter.HeroList[index].ServerId);
            }
        }
        HeroListHelper.FillItemEnd();
        FocusOnHero(firstHero);
    }

    public void FocusOnHero(DataCenter.Hero hero)
    {
        CurrentHero = hero;
    }

    public void DisplayHeroItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero((uint)scrollItem.LogicIndex);
            if (null != hero)
            {
                GUI_HeroBindWeaponItem_DL hbwi = _HeroItemPool.GetOneLogicComponent() as GUI_HeroBindWeaponItem_DL;
                hbwi.SetHero(hero, OnHeroSelect, OnHeroDeSelect, DisplayWeapon);
                scrollItem.SetTarget(hbwi);
                HeroTabPageList[_CurrentPage].ShowItem(hbwi);

                if (null == CurrentHero
                    || CurrentHero.ServerId == hero.ServerId)
                {
                    hbwi.Select();
                }
            }
        }
    }

    void RefreshHeroBindItem()
    {
        for(int index = 0; index < HeroListHelper.ItemCount; ++index)
        {
            GUI_ScrollItem scrollItem = HeroListHelper.GetAtIndex(index);
            if(null != scrollItem && null != scrollItem.LogicObject)
            {
                GUI_HeroBindWeaponItem_DL hbwi = scrollItem.LogicObject as GUI_HeroBindWeaponItem_DL;
                hbwi.RefreshBindEquip();
            }
        }
    }

    bool DisplayWeapon()
    {
        return _DisplayWeap;
    }

    void OnHeroSelect(GUI_HeroBindWeaponItem_DL heroBindWeapon)
    {
        if(null != heroBindWeapon)
        {
            CurrentHero = heroBindWeapon.BindHero;
        }
    }

    void OnHeroDeSelect(GUI_HeroBindWeaponItem_DL heroBindWeapon)
    {
        if (null != heroBindWeapon)
        {
            
        }
    }

    public void OnWeaponButtonClicked()
    {
        if (!_DisplayWeap)
        {
            _DisplayWeap = !_DisplayWeap;
            RefreshDisplay();
        }
    }

    public void OnRingButtonClicked()
    {
        if (_DisplayWeap)
        {
            _DisplayWeap = false;
            RefreshDisplay();
        }
    }
    #endregion

    #region bag info area
    public Text EquipHoldText;
    public Text EquipBagVolume;

    void RefreshDisplay()
    {
        RefreshBagInfo();
        RefreshInfoButton();
        RefreshHeroBindItem();
        RefreshEquipScrollPage();
    }

    void RefreshBagInfo()
    {
        string tex;
        if (_DisplayWeap)
        {
            if (TextLocalization.GetText("Hold_Weapon_Text", out tex))
            {
                EquipHoldText.text = tex;
            }
            EquipBagVolume.text = string.Format("{0}/{1}", DataCenter.PlayerDataCenter.EquipList.Count.ToString(), DataCenter.PlayerDataCenter.MaxWeaponCount.ToString());
            if (!ResolveButtonObject.activeInHierarchy)
            {
                ResolveButtonObject.SetActive(true);
            }
        }
        else
        {
            if (TextLocalization.GetText("Hold_Ring_Text", out tex))
            {
                EquipHoldText.text = tex;
            }
            EquipBagVolume.text = string.Format("{0}/{1}", DataCenter.PlayerDataCenter.RingList.Count.ToString(), DataCenter.PlayerDataCenter.MaxRingCount.ToString());
            if (ResolveButtonObject.activeInHierarchy)
            {
                ResolveButtonObject.SetActive(false);
            }
        }
    }

    void RefreshInfoButton()
    {
        if(_DisplayWeap)
        {
            if(!ReformInfoButtonObject.activeInHierarchy)
            {
                ReformInfoButtonObject.SetActive(true);
            }
            if(RingInfoButtonObject.activeInHierarchy)
            {
                RingInfoButtonObject.SetActive(false);
            }
        }
        else
        {
            if (ReformInfoButtonObject.activeInHierarchy)
            {
                ReformInfoButtonObject.SetActive(false);
            }
            if (!RingInfoButtonObject.activeInHierarchy)
            {
                RingInfoButtonObject.SetActive(true);
            }
        }
    }

    public void OnExtendEquipBagButtonClicked()
    {
        GUI_ExtendBagUI_DL extendUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExtendBagUI_DL>("UI_Extend_Hero", false);
        if (_DisplayWeap)
        {
            extendUI.ExtendBag(PbCommon.EExendBagType.E_Extend_Weapon_Bag);
        }
        else
        {
            extendUI.ExtendBag(PbCommon.EExendBagType.E_Extend_Ring_Bag);
        }
    }

    void OnExtendEquipBagRsp(PbCommon.EExendBagType bagType)
    {
        if (bagType == PbCommon.EExendBagType.E_Extend_Weapon_Bag
            || bagType == PbCommon.EExendBagType.E_Extend_Ring_Bag)
        {
            RefreshDisplay();
        }
    }
    #endregion

    #region equip scroll page
    GameObject _EquipScrollPageObject;
    GUI_VerticallayouGroupHelper_DL _EquipGroupLayoutHelper;
    GUI_LogicObjectPool _EquipDisplayItemPool;
    ToggleGroup _EquipToggleGroup;
    Dictionary<int, List<DataCenter.Equip>> EquipBuckets;

    void InitEquipScrollPage()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Train_HeroWeaponManage_Weaponlist_Item", true, AssetManage.E_AssetType.UIPrefab);
        _EquipDisplayItemPool = new GUI_LogicObjectPool(go);

        _EquipGroupLayoutHelper = _EquipScrollPageObject.GetComponent<GUI_VerticallayouGroupHelper_DL>();
        _EquipGroupLayoutHelper.SetScrollAction(DisplayEquipItem);
        _EquipToggleGroup = _EquipScrollPageObject.GetComponent<ToggleGroup>();

        EquipBuckets = new Dictionary<int, List<DataCenter.Equip>>(6);
        for(int index = 1; index <= 6; ++index)
        {
            EquipBuckets.Add(index, new List<DataCenter.Equip>());
        }
    }

    void RefreshEquipScrollPage()
    {
        DistributeEquipBuckets();
        _EquipGroupLayoutHelper.Clear();
        int prioritySchool = null != CurrentHero ? CurrentHeroTemplate.School : 1;
        ShowEquipBuckect(prioritySchool);
        for (int school = 1; school <= 6; ++school )
        {
            if(school != prioritySchool)
            {
                ShowEquipBuckect(school);
            }
        }
        _EquipGroupLayoutHelper.FillItem(-1);
        _EquipGroupLayoutHelper.FillItemEnd();
        _EquipGroupLayoutHelper.LocateAtIndex(0);
    }

    void ShowEquipBuckect(int prioritySchool)
    {
        List<DataCenter.Equip> equipBucket;
        if(EquipBuckets.TryGetValue(prioritySchool, out equipBucket))
        {
            if (null != equipBucket)
            {
                for (int index = 0; index < equipBucket.Count; ++index)
                {
                    if (CanShowEquip(equipBucket[index], _CurrentPage))
                    {
                        _EquipGroupLayoutHelper.FillItem((int)equipBucket[index].ServerId);
                    }
                }
            }
        }
    }

    void DistributeEquipBuckets()
    {
        for(int index = 1; index <= 6; ++index)
        {
            EquipBuckets[index].Clear();
        }
        List<DataCenter.Equip> equipList = _DisplayWeap ? DataCenter.PlayerDataCenter.WeaponList : DataCenter.PlayerDataCenter.RingList;
        if (null != equipList)
        {
            for (int index = 0; index < equipList.Count; ++index)
            {
                CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData(equipList[index].CsvId);
                if(null != equipTemplate)
                {
#if UNITY_EDITOR
                    Debug.Assert((1 <= equipTemplate.School && equipTemplate.School <= 6), "equip school not match school type !!!!");
#endif
                    EquipBuckets[equipTemplate.School].Add(equipList[index]);
                }
            }
        }
    }

    bool CanShowEquip(DataCenter.Equip equip, int school)
    {
        bool canshow = false;
        if(null != equip)
        {
            CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData((int)equip.CsvId);
            if (0 == school || school == equipTemplate.School)
            {
                DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(equip.HeroServerId);
                if(SellButtonArea.activeInHierarchy)
                {
                    canshow = (null == hero && equipTemplate.CanSale != 0);
                }
                else if(ResolveButtonArea.activeInHierarchy)
                {
                    canshow = (null == hero && equipTemplate.CanBreak != 0);
                }
                else
                {
                    canshow = true;
                }
            }
        }
        return canshow;
    }

    void FocusOnEquip(DataCenter.Equip equip)
    {
        if(null != equip)
        {
            for (int index = 0; index < _EquipGroupLayoutHelper.ItemCount; ++index)
            {
                GUI_ScrollItem scrollItem = _EquipGroupLayoutHelper.GetAtIndex(index);
                if (null != scrollItem && equip.ServerId == (uint)scrollItem.LogicIndex)
                {
                    _EquipGroupLayoutHelper.LocateAtIndex(index);
                    break;
                }
            }
        }
    }

    void HeroChange()
    {
        for(int index = 0; index < _EquipGroupLayoutHelper.ItemCount; ++index)
        {
            GUI_ScrollItem scrollItem = _EquipGroupLayoutHelper.GetAtIndex(index);
            if(null != scrollItem && null != scrollItem.LogicObject)
            {
                GUI_HeroEquipDisplayItem_DL equipItem = scrollItem.LogicObject as GUI_HeroEquipDisplayItem_DL;
                equipItem.OnHeroChange(CurrentHero, false);
            }
        }
    }

    void GroupEquipItem(bool group)
    {
        for (int index = 0; index < _EquipGroupLayoutHelper.ItemCount; ++index)
        {
            GUI_ScrollItem scrollItem = _EquipGroupLayoutHelper.GetAtIndex(index);
            if (null != scrollItem && null != scrollItem.LogicObject)
            {
                GUI_HeroEquipDisplayItem_DL equipItem = scrollItem.LogicObject as GUI_HeroEquipDisplayItem_DL;
                equipItem.RegistToGroup(group ? _EquipToggleGroup : null);
            }
        }
    }

    void DisplayEquipItem(GUI_ScrollItem scrollItem)
    {
        if(scrollItem.LogicIndex < 0)
        {
            GUI_HeroEquipDisplayItem_DL equipItem = _EquipDisplayItemPool.GetOneLogicComponent() as GUI_HeroEquipDisplayItem_DL;
            scrollItem.SetTarget(equipItem);
            equipItem.DisplayEquip(scrollItem, null, null, null, null, DisplayWeapon);
            equipItem.CheckResolveSafeMask(false);
            equipItem.CheckSellSafeMask(false);
        }
        else
        {
            DataCenter.Equip equip = DataCenter.PlayerDataCenter.GetEquip((uint)scrollItem.LogicIndex);
            if (null != equip)
            {
                GUI_HeroEquipDisplayItem_DL equipItem = _EquipDisplayItemPool.GetOneLogicComponent() as GUI_HeroEquipDisplayItem_DL;
                scrollItem.SetTarget(equipItem);
                equipItem.DisplayEquip(scrollItem, equip, OnSelectEquipItem, OnDeSelectEquipItem, CanEquip, DisplayWeapon);
                if (AllowMultipleEquipSelect())
                {
                    equipItem.RegistToGroup(null);
                }
                else
                {
                    equipItem.RegistToGroup(_EquipToggleGroup);
                }
                if(SellButtonArea.activeInHierarchy)
                {
                    equipItem.CheckSellSafeMask(true);
                }
                else if(ResolveButtonArea.activeInHierarchy)
                {
                    equipItem.CheckResolveSafeMask(true);
                }
                else
                {
                    equipItem.CheckResolveSafeMask(false);
                    equipItem.CheckSellSafeMask(false);
                }
                equipItem.OnHeroChange(CurrentHero, SellButtonArea.activeInHierarchy || ResolveButtonArea.activeInHierarchy);
                if (SelectedEquipItems.Contains(equip.ServerId))
                {
                    equipItem.Select();
                }
            }
        }
    }

    bool CanEquip(CSV_b_equip_template equipTemplate)
    {
        bool canEquip = false;
        if (null != CurrentHero && null != equipTemplate)
        {
            switch ((PbCommon.EWeaponType)equipTemplate.WeaponType)
            {
                case PbCommon.EWeaponType.E_Weapon_Type_Common:
                case PbCommon.EWeaponType.E_Weapon_Type_Original:
                case PbCommon.EWeaponType.E_Weapon_Type_Special:
                    {
                        canEquip = (CurrentHero.HeroCSV.School == equipTemplate.School);
                        break;
                    }
                case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                    {
                        canEquip = (CurrentHero.HeroCSV.School == equipTemplate.School
                            && (CurrentHero.CsvId == equipTemplate.Hero1 || CurrentHero.CsvId == equipTemplate.Hero2 || CurrentHero.CsvId == equipTemplate.Hero3));
                        break;
                    }
            }

        }
        return canEquip;
    }

    bool AllowMultipleEquipSelect()
    {
        return (SellButtonArea.activeInHierarchy || ResolveButtonArea.activeInHierarchy);
    }

    bool OnSelectEquipItem(GUI_HeroEquipDisplayItem_DL equipItem)
    {
        bool multipleSelect = AllowMultipleEquipSelect();
        if (null != equipItem)
        {
            if (!SelectedEquipItems.Contains(equipItem.Equip.ServerId))
            {
                if (multipleSelect)
                {
                    SelectedEquipItems.Add(equipItem.Equip.ServerId);
                }
            }
            if(SellButtonArea.activeInHierarchy)
            {
                UpdateSellPrice();
            }
            else if (ResolveButtonArea.activeInHierarchy)
            {
                UpdateResolveMat();
            }
        }
        return multipleSelect;
    }

    bool OnDeSelectEquipItem(GUI_HeroEquipDisplayItem_DL equipItem)
    {
        if (null != equipItem && SelectedEquipItems.Contains(equipItem.Equip.ServerId))
        {
            SelectedEquipItems.Remove(equipItem.Equip.ServerId);
        }
        if (SellButtonArea.activeInHierarchy)
        {
            UpdateSellPrice();
        }
        else if (ResolveButtonArea.activeInHierarchy)
        {
            UpdateResolveMat();
        }
        return AllowMultipleEquipSelect();
    }
    #endregion

    #region manage button area
    List<uint> SelectedEquipItems = new List<uint>();
    public GameObject ControlButtonArea;
    public GameObject SellButtonArea;
    public GameObject ResolveButtonArea;
    public GameObject ReformInfoButtonObject;
    public GameObject RingInfoButtonObject;
    public Text SellPrice;
    public Text ResolveMat;

    void UpdateSellPrice()
    {
        int price = 0;
        for(int index = 0; index < SelectedEquipItems.Count; ++index)
        {
            DataCenter.Equip equip = DataCenter.PlayerDataCenter.GetEquip(SelectedEquipItems[index]);
            CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData(equip.CsvId);
            price += equipTemplate.SaleCoin;
        }
        SellPrice.text = price.ToString();
    }

    void UpdateResolveMat()
    {
        int price = 0;
        for (int index = 0; index < SelectedEquipItems.Count; ++index)
        {
            DataCenter.Equip equip = DataCenter.PlayerDataCenter.GetEquip(SelectedEquipItems[index]);
            CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData(equip.CsvId);
            price += equipTemplate.BreakIron;
        }
        ResolveMat.text = price.ToString();
    }

    public void OnSellButtonClicked()
    {
        ControlButtonArea.SetActive(false);
        SellButtonArea.SetActive(true);
        UpdateSellPrice();
        RefreshEquipScrollPage();
        GroupEquipItem(false);
    }

    void OnSellRsp(uint itemType, uint coin)
    {
        if (itemType == (int)PbCommon.ESaleItemType.E_Sale_Equip
            && SellButtonArea.activeInHierarchy)
        {
            OnCancelButtonClicked();
        }
        RefreshBagInfo();
    }

    public void OnResolveButtonClicked()
    {
        ControlButtonArea.SetActive(false);
        ResolveButtonArea.SetActive(true);
        UpdateResolveMat();
        RefreshEquipScrollPage();
        GroupEquipItem(false);
    }

    void OnResolveRsp(List<DataCenter.AwardInfo> awardList, List<DataCenter.AwardInfo> extraAwardList, bool isBigSuccess)
    {
        if (ResolveButtonArea.activeInHierarchy)
        {
            OnCancelButtonClicked();
        }
        RefreshBagInfo();
    }

    public void OnCancelButtonClicked()
    {
        ControlButtonArea.SetActive(true);
        if (SellButtonArea.activeInHierarchy)
        {
            SellButtonArea.SetActive(false);
        }
        if (ResolveButtonArea.activeInHierarchy)
        {
            ResolveButtonArea.SetActive(false);
        }
        SelectedEquipItems.Clear();
        RefreshEquipScrollPage();
        GroupEquipItem(true);
    }

    public void OnConfirmSellButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip("ConfirmSell");
        if (SelectedEquipItems.Count > 0)
        {
            GUI_SellWeaponAlertUI_DL alertUI = GUI_Manager.Instance.ShowWindowWithName<GUI_SellWeaponAlertUI_DL>("UI_SellAffirm", false);
            if (null != alertUI)
            {
                alertUI.SellEquip(SelectedEquipItems);
            }
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("Error_No_Select_Sell_Weapon", true);
        }
    }

    public void OnSellAllButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_SellAllWeapon", false);
    }

    void OnEquipListChange()
    {
        RefreshEquipScrollPage();
    }

    public void OnConfirmResolveButtonClicked()
    {
        if(SelectedEquipItems.Count > 0)
        {
            GUI_ResolveWeaponAlertUI_DL alertUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ResolveWeaponAlertUI_DL>("UI_Decompose_Confirm", false);
            if (null != alertUI)
            {
                alertUI.ResolveEquip(SelectedEquipItems);
            }
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("Error_No_Select_Resolve_Weapon", true);
        }
    }

    public void OnResolveAllButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_ResolveAllWeapon", false);
    }

    void OnReformInfoButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_Remould_Detail", false);
    }

    void OnRingInfoButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_Remould_Detail", false);
    }
    #endregion

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_EquipPackageUI dataComponent = gameObject.GetComponent<GUI_EquipPackageUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_EquipPackageUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HeroTabPageObjectList = dataComponent.HeroTabPageObjectList;
            GridHelperObject = dataComponent.GroupLayoutHelperObject;
            DefaultDisplayObject = dataComponent.DefaultEquipDisplayObject;
            _EquipScrollPageObject = dataComponent.EquipScrollPageObject;

            ResolveButtonObject = dataComponent.ResolveButtonObject;
            EquipHoldText = dataComponent.EquipHoldText;
            EquipBagVolume = dataComponent.EquipBagVolume;
            ControlButtonArea = dataComponent.ControlButtonArea;
            SellButtonArea = dataComponent.SellButtonArea;
            ResolveButtonArea = dataComponent.ResolveButtonArea;
            SellPrice = dataComponent.SellPrice;
            ResolveMat = dataComponent.ResolveMat;

            WeaponPageToggleObject = dataComponent.WeaponPageToggleObject;
            RingPageToggleObject = dataComponent.RingPageToggleObject;

            dataComponent.ExtendEquipBagButton.onClick.AddListener(OnExtendEquipBagButtonClicked);
            dataComponent.SellButton.onClick.AddListener(OnSellButtonClicked);
            dataComponent.ResolveButton.onClick.AddListener(OnResolveButtonClicked);
            dataComponent.CancelSellButton.onClick.AddListener(OnCancelButtonClicked);
            dataComponent.ConfirmSellButton.onClick.AddListener(OnConfirmSellButtonClicked);
            dataComponent.SellAllButton.onClick.AddListener(OnSellAllButtonClicked);
            dataComponent.CancelResolveButton.onClick.AddListener(OnCancelButtonClicked);
            dataComponent.ConfirmResolveButton.onClick.AddListener(OnConfirmResolveButtonClicked);
            dataComponent.ResolveAllButton.onClick.AddListener(OnResolveAllButtonClicked);
            dataComponent.ReformInfoButton.onClick.AddListener(OnReformInfoButtonClicked);
            dataComponent.RingInfoButton.onClick.AddListener(OnRingInfoButtonClicked);
            ReformInfoButtonObject = dataComponent.ReformInfoButton.gameObject;
            RingInfoButtonObject = dataComponent.RingInfoButton.gameObject;
        }
    }
}
