using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate bool EquipItemSelect(GUI_HeroEquipDisplayItem_DL equipItem);
public sealed class GUI_HeroEquipDisplayItem_DL : GUI_ToggleItem_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroEquipDisplayItem dataComponent = gameObject.GetComponent<GUI_HeroEquipDisplayItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroEquipDisplayItem_DL,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ItemExtendRate = dataComponent.ItemExtendRate;
            ItemShrinkRate = dataComponent.ItemShrinkRate;
            ShrinkHeight = dataComponent.ShrinkHeight;
            ExtendHeight = dataComponent.ExtendHeight;

            EquipSimpleInfo = dataComponent.EquipSimpleInfo;
            EquipName = dataComponent.EquipName;
            HeroIcon = dataComponent.HeroIcon;
            HeroStarObject = dataComponent.HeroStar;
            ExclusiveText = dataComponent.ExclusiveText;

            AttackProperty = dataComponent.AttackProperty;
            AttackSpeedProperty = dataComponent.AttackSpeedProperty;
            NonBigSuccessColor = dataComponent.NonBigSuccessColor;
            BigSuccessColor = dataComponent.BigSuccessColor;
            EquipWordInfoList = dataComponent.EquipWordInfoList;
            ExclusiveWordLoopObject = dataComponent.ExclusiveWordLoopObject;
            NonWordPropertyTip = dataComponent.NonWordPropertyTip;

            DismissEquip = dataComponent.DismissEquip;
            DismissButtonText = dataComponent.DismissButtonText;
            ReformEquip = dataComponent.ReformEquip;
            RefineEquip = dataComponent.RefineEquip;
            EquipLockButtonText = dataComponent.EquipLockButtonText;
            dataComponent.EquipLockButton.onClick.AddListener(OnSafeLockClicked);
            dataComponent.DismissEquip.onClick.AddListener(OnDismissButtonClicked);
            dataComponent.ReformEquip.onClick.AddListener(OnReformButtonClieked);
            dataComponent.RefineEquip.onClick.AddListener(OnRefineButtonClicked);
            RefineEquipText = dataComponent.RefineEquipText;
            RefineEquipRemind = dataComponent.RefineEquipRemind;
            WordPropertyRoot = dataComponent.WordPropertyRoot;
            ExtendBagRoot = dataComponent.ExtendBagRoot;
            dataComponent.ExtendBagButton.onClick.AddListener(OnExtendBagMarkClicked);
            LockMask = dataComponent.LockMask;
            LockIcon = dataComponent.LockIcon;
            LockMaskButton = dataComponent.LockMaskButton;
        }
    }
    #endregion

    #region toggle logic
    GUI_ScrollItem _BindItem;
    float ItemExtendRate = 0.05f;
    float ItemShrinkRate = 0.05f;
    float ShrinkHeight = 120f;
    float ExtendHeight = 220f;
    bool ExtendingItem = false;
    bool ShrinkingItem = false;
    EquipItemSelect OnItemSelected;
    EquipItemSelect OnItemDeSelected;
    public delegate bool CanEquipCurrentHero(CSV_b_equip_template equipTemplate);
    CanEquipCurrentHero CanEquip;
    GUI_HeroBindWeaponItem_DL.DisplayWeapon DisplayWeapon;

    protected override void OnSelected()
    {
        if(null != _BindItem)
        {
            if(null == OnItemSelected || !OnItemSelected(this))
            {
                if(null != ExclusiveWordLooper)
                {
                    ExclusiveWordLooper.StartLoop();
                }
                StartCoroutine("ExtendItemHeight");
            }
        }
    }

    IEnumerator ExtendItemHeight()
    {
        ExtendingItem = true;
        int extendCounter = 0;
        while (_BindItem.ItemLayout.preferredHeight < ExtendHeight)
        {
            float oldHeight = _BindItem.ItemLayout.preferredHeight;
            _BindItem.ItemLayout.preferredHeight = Mathf.Lerp(ShrinkHeight, ExtendHeight, ItemExtendRate * extendCounter);
            ++extendCounter;
            CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y + (_BindItem.ItemLayout.preferredHeight - oldHeight) / 2, CachedTransform.localPosition.z);
            _BindItem.SetDirty();
            _BindItem.FocusOn();
            yield return null;
        }
        ExtendingItem = false;
        _BindItem.SetDirty();
        _BindItem.FocusOn();
    }

    protected override void OnDeSelected()
    {
        if (null != _BindItem)
        {
            if(null == OnItemDeSelected || !OnItemDeSelected(this))
            {
                if (null != ExclusiveWordLooper)
                {
                    ExclusiveWordLooper.StopLoop();
                }
                StartCoroutine("ShrinkItemHeight");
            }
        }
    }

    IEnumerator ShrinkItemHeight()
    {
        ShrinkingItem = true;
        int ShrinkCounter = 0;
        while (_BindItem.ItemLayout.preferredHeight > ShrinkHeight)
        {
            float oldHeight = _BindItem.ItemLayout.preferredHeight;
            _BindItem.ItemLayout.preferredHeight = Mathf.Lerp(ExtendHeight, ShrinkHeight, ItemExtendRate * ShrinkCounter);
            ++ShrinkCounter;
            CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y - (oldHeight - _BindItem.ItemLayout.preferredHeight) / 2, CachedTransform.localPosition.z);
            _BindItem.SetDirty();
            _BindItem.FocusOn();
            yield return null;
        }
        ShrinkingItem = false;
        _BindItem.SetDirty();
        _BindItem.FocusOn();
    }

    protected override void OnRecycle()
    {
        if (null != ExclusiveWordLooper)
        {
            ExclusiveWordLooper.StopLoop();
        }
        if(ExtendingItem)
        {
            StopCoroutine("ExtendItemHeight");
            ExtendingItem = false;
        }
        if(ShrinkingItem)
        {
            StopCoroutine("ShrinkItemHeight");
            ShrinkingItem = false;
        }
        _BindItem = null;
        EquipTemplate = null;
        OnItemSelected = null;
        OnItemDeSelected = null;
        DeSelect();
        UnRegistEvent();
    }

    void OnDisable()
    {
        UnRegistEvent();
    }

    public void DisplayEquip(GUI_ScrollItem bindItem, DataCenter.Equip equip, EquipItemSelect onSelected, EquipItemSelect onDeSelected, CanEquipCurrentHero canEquip, GUI_HeroBindWeaponItem_DL.DisplayWeapon displayWeapon)
    {
        _BindItem = bindItem;
        _BindItem.ItemLayout.preferredHeight = ShrinkHeight;
        OnItemSelected = onSelected;
        OnItemDeSelected = onDeSelected;
        CanEquip = canEquip;
        DisplayWeapon = displayWeapon;
        if(null == ExclusiveWordLooper)
        {
            if(null != ExclusiveWordLoopObject)
            {
                ExclusiveWordLooper = ExclusiveWordLoopObject.GetComponent<GUI_LoopText_DL>();
            }
        }
        SetItemInfo(equip);
        RefreshBindHeroInfo();
        RegisteEvent();
    }

    public void CheckSellSafeMask(bool show)
    {
        if(show)
        {
            bool beLock = false;
            if (null != Equip && null != EquipTemplate)
            {
                beLock = (Equip.IsLock || EquipTemplate.CanSale == 0);
            }
            GUI_Tools.ObjectTool.ActiveObject(LockMask, beLock);
            GUI_Tools.ObjectTool.ActiveObject(LockIcon, beLock);
            LockMaskButton.enabled = beLock;
        }
        else
        {
            GUI_Tools.ObjectTool.ActiveObject(LockMask, false);
            LockMaskButton.enabled = false;
        }
    }

    public void CheckResolveSafeMask(bool show)
    {
        if (show)
        {
            bool beLock = false;
            if (null != Equip && null != EquipTemplate)
            {
                beLock = (Equip.IsLock || EquipTemplate.CanBreak == 0);
            }
            GUI_Tools.ObjectTool.ActiveObject(LockMask, beLock);
            GUI_Tools.ObjectTool.ActiveObject(LockIcon, beLock);
            LockMaskButton.enabled = beLock;
        }
        else
        {
            GUI_Tools.ObjectTool.ActiveObject(LockMask, false);
            LockMaskButton.enabled = false;
        }
    }

    public void OnHeroChange(DataCenter.Hero hero, bool ignoreHero)
    {
        RefreshButton();
        if(!ignoreHero)
        {
            CheckEquipable();
            if(null != hero 
                && null != Equip 
                && ((null != hero.Weapon && hero.Weapon.ServerId == Equip.ServerId) || (null != hero.Ring && hero.Ring.ServerId == Equip.ServerId)))
            {
                if(!IsSelect)
                {
                    Select();
                }
            }
        }
    }

    void CheckEquipable()
    {
        if (null != CanEquip)
        {
            bool canEquipHero = CanEquip(EquipTemplate);
            GUI_Tools.ObjectTool.ActiveObject(LockMask, !canEquipHero);
            GUI_Tools.ObjectTool.ActiveObject(LockIcon, false);
            LockMaskButton.enabled = false;
        }
        else
        {
            GUI_Tools.ObjectTool.ActiveObject(LockMask, false);
        }
    }

    public void RefreshButton()
    {
        if(null == Equip)
        {
            return;
        }
        DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(Equip.HeroServerId);
        if(null != hero)//equiped
        {
            TextLocalization.SetTextById(DismissButtonText, TextId.Dismiss);
            DismissEquip.interactable = true;
        }
        else
        {
            TextLocalization.SetTextById(DismissButtonText, TextId.Equip);
            DismissEquip.interactable = (null != CanEquip && CanEquip(EquipTemplate));
        }

        switch((PbCommon.EWeaponType)EquipTemplate.WeaponType)
        {
            case PbCommon.EWeaponType.E_Weapon_Type_Original:
                {
                    RefineEquip.gameObject.SetActive(true);
                    RefineEquip.interactable = true;
                    ReformEquip.gameObject.SetActive(false);
                    TextLocalization.SetTextById(RefineEquipText, TextId.Refine);
                    break;
                }
            case PbCommon.EWeaponType.E_Weapon_Type_Special:
                {
                    RefineEquip.gameObject.SetActive(false);
                    ReformEquip.gameObject.SetActive(false);
                    break;
                }
            case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                {
                    RefineEquip.gameObject.SetActive(true);
                    ReformEquip.gameObject.SetActive(true);
                    bool ultimate = EquipTemplate.Star >= DefaultConfig.GetInt("Weapon_Max_Star");
                    RefineEquip.interactable = !ultimate;
                    TextLocalization.SetTextById(RefineEquipText, ultimate ? TextId.Ultimate : TextId.Refine);
                    break;
                }
            case PbCommon.EWeaponType.E_Weapon_Type_Common:
                {
                    RefineEquip.gameObject.SetActive(false);
                    ReformEquip.gameObject.SetActive(true);
                    break;
                }
        }

        TextLocalization.SetTextById(EquipLockButtonText, Equip.IsLock ? TextId.Open : TextId.Close);
    }
    #endregion

    #region item info
    public DataCenter.Equip Equip { get; protected set; }
    CSV_b_equip_template EquipTemplate;

    GUI_EquipSimpleInfo EquipSimpleInfo;
    Text EquipName;
    Image HeroIcon;
    public Text ExclusiveText;
    GameObject HeroStarObject;
    GUI_HeroStarBar_DL HeroStar;
    GUI_EquipBasePropertyInfo AttackProperty;
    GUI_EquipBasePropertyInfo AttackSpeedProperty;
    Color NonBigSuccessColor;
    Color BigSuccessColor;
    List<GUI_EquipWordPropertyInfo> EquipWordInfoList;
    public GameObject ExclusiveWordLoopObject;
    GUI_LoopText_DL ExclusiveWordLooper;

    GameObject WordPropertyRoot;
    GameObject ExtendBagRoot;
    Text NonWordPropertyTip;

    Text EquipLockButtonText;
    Text DismissButtonText;
    Button DismissEquip;
    Button ReformEquip;
    Button RefineEquip;
    Text RefineEquipText;
    Text RefineEquipRemind;
    public GameObject LockMask;
    public GameObject LockIcon;
    public Button LockMaskButton;

    void SetItemInfo(DataCenter.Equip equip)
    {
        Equip = equip;
        if(null != equip)
        {
            if (null != equip)
            {
                EquipTemplate = CSV_b_equip_template.FindData(equip.CsvId);
            }
            AttackProperty.Description.text = equip.Attack.ToString();
            AttackSpeedProperty.Description.text = equip.AttackSpeed.ToString();
            if(null != EquipTemplate)
            {
                EquipName.text = EquipTemplate.Name;
                GUI_Tools.IconTool.SetIcon(EquipTemplate.IconAtlas, EquipTemplate.IconSprite, EquipSimpleInfo.EquipIcon);
                EquipSimpleInfo.StarText.text = EquipTemplate.Star.ToString();                
            }

            CSV_c_school_config equipSchool = CSV_c_school_config.FindData(EquipTemplate.School);
            if(null != equipSchool)
            {
                int maxRefineStar = DefaultConfig.GetInt("MaxRefineStar");
                GUI_Tools.IconTool.SetIcon(equipSchool.Atlas, EquipTemplate.Star == maxRefineStar ? equipSchool.ExclusiveWeaponMaxIcon : equipSchool.Icon, EquipSimpleInfo.SchoolIcon);
            }
            ActiveExtendMark(false);
            ActiveWordProperty(true);
            RefreshWordItem();
        }
        else
        {
            ActiveWordProperty(false);
            ActiveExtendMark(true);
        }
    }

    void RefreshBindHeroInfo()
    {
        if(null == Equip)
        {
            return;
        }
        if (null == HeroStar)
        {
            HeroStar = HeroStarObject.GetComponent<GUI_HeroStarBar_DL>();
        }
        DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(Equip.HeroServerId);
        if (null != hero)
        {
            ExclusiveText.text = null;
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData((int)hero.CsvId);
            if (null != heroTemplate)
            {
                GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, HeroIcon);
                HeroStar.SetStarNum(heroTemplate.Star);
            }
            else
            {
                HeroIcon.sprite = null;
                HeroStar.SetStarNum(0);
            }
        }
        else
        {
            HeroIcon.sprite = null;
            HeroStar.SetStarNum(0);
            if(null != EquipTemplate)
            {
                CSV_b_hero_template targetHero = CSV_b_hero_template.FindData(EquipTemplate.Hero1);
                string exclusive;
                if (null != targetHero && TextLocalization.GetText("Weapon_Exclusive_Text", out exclusive))
                {
                    ExclusiveText.text = string.Format("{0}{1}", targetHero.Name, exclusive);
                }
                else
                {
                    ExclusiveText.text = null;
                }
            }
            else
            {
                ExclusiveText.text = null;
            }
        }
    }

    void ActiveWordProperty(bool active)
    {
        if(active)
        {
            if (!WordPropertyRoot.activeInHierarchy)
            {
                WordPropertyRoot.SetActive(true);
            }
        }
        else
        {
            if (WordPropertyRoot.activeInHierarchy)
            {
                WordPropertyRoot.SetActive(false);
            }
        }
    }

    void ActiveExtendMark(bool active)
    {
        if (active)
        {
            if (!ExtendBagRoot.activeInHierarchy)
            {
                ExtendBagRoot.SetActive(true);
            }
        }
        else
        {
            if (ExtendBagRoot.activeInHierarchy)
            {
                ExtendBagRoot.SetActive(false);
            }
        }
    }

    void RefreshWordItem()
    {
        switch ((PbCommon.EWeaponType)EquipTemplate.WeaponType)
        {
            case PbCommon.EWeaponType.E_Weapon_Type_Common:
            case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                {
                    ActiveWordProperty(true);
                    int bigSuccessCount = 0;
                    for (int index = 0; index < EquipWordInfoList.Count; ++index)
                    {
                        if (index < Equip.ReformList.Count)
                        {
                            if(Equip.ReformList[index].IsBigSuccess)
                            {
                                ++bigSuccessCount;
                            }
                            SetEquipWordItem(Equip.ReformList[index], EquipWordInfoList[index], index);
                        }
                        else
                        {
                            SetEquipWordItem(null, EquipWordInfoList[index], index);
                        }                        
                    }
                    if (bigSuccessCount > 0 && bigSuccessCount == Equip.ReformList.Count)
                    {
                        TextLocalization.SetTextById(EquipSimpleInfo.ReformText, TextId.BigSuccess);
                    }
                    else
                    {
                        GUI_Tools.TextTool.SetText(EquipSimpleInfo.ReformText, bigSuccessCount > 0 ? "+" + bigSuccessCount : null);
                    }
                    break;
                }
            case PbCommon.EWeaponType.E_Weapon_Type_Original:
                {
                    ActiveWordProperty(false);
                    GUI_Tools.TextTool.SetText(EquipSimpleInfo.ReformText, null);
                    string tip;
                    if(TextLocalization.GetText("Ancient_Weapon_Word_Tip", out tip))
                    {
                        NonWordPropertyTip.text = tip;
                    }
                    break;
                }
            case PbCommon.EWeaponType.E_Weapon_Type_Special:
                {
                    ActiveWordProperty(false);
                    GUI_Tools.TextTool.SetText(EquipSimpleInfo.ReformText, null);
                    string tip = null;
                    if(EquipTemplate.CanSale == 1)//卖钱的武器
                    {
                        TextLocalization.GetText("Sell_For_Gold_Weapon_Tip", out tip);
                    }
                    else if(EquipTemplate.CanBreak == 1)//分解铁的武器
                    {
                        TextLocalization.GetText("Break_For_Iron_Weapon_Tip", out tip);
                    }
                    NonWordPropertyTip.text = tip;
                    break;
                }
        }
        
    }

    void SetEquipWordItem(DataCenter.EquipReform equipReform, GUI_EquipWordPropertyInfo wordProperty, int wordIndex)
    {
        if(null != equipReform)
        {
            if (null != wordProperty)
            {
                GUI_Tools.ObjectTool.ActiveObject(wordProperty.DescriptionRootObject, true);
                GUI_Tools.ObjectTool.ActiveObject(wordProperty.WorldLevlRootObject, true);
                wordProperty.WordDescription.text = GUI_Tools.TextTool.GetWordPropertyText((PbCommon.EReformType)equipReform.ReformType);
                string reformFormater = GUI_Tools.TextTool.GetReformTextFormater((PbCommon.EPropertyType)equipReform.ReformProperty);
                wordProperty.ReformText.text = string.Format(reformFormater, equipReform.ReformValue);

                string bigSuccessText;
                if(equipReform.IsBigSuccess)
                {
                    TextLocalization.GetText("Reform_Big_Success_Text", out bigSuccessText);
                    wordProperty.WordLevelText.text = GUI_Tools.RichTextTool.Color(BigSuccessColor, bigSuccessText);
                }
                else
                {
                    TextLocalization.GetText("Reform_Non_Big_Sucess_Text", out bigSuccessText);
                    wordProperty.WordLevelText.text = GUI_Tools.RichTextTool.Color(NonBigSuccessColor, bigSuccessText);
                }                
            }
        }
        else if (null != wordProperty)
        {
            switch ((PbCommon.EWeaponType)EquipTemplate.WeaponType)
            {
                case PbCommon.EWeaponType.E_Weapon_Type_Common:
                    {
                        GUI_Tools.ObjectTool.ActiveObject(wordProperty.DescriptionRootObject, false);
                        TextLocalization.SetTextById(wordProperty.RemindText, "Reform_Weapon_Common_Tip_" + wordIndex);
                        break;
                    }
                case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                    {
                        GUI_Tools.ObjectTool.ActiveObject(wordProperty.DescriptionRootObject, true);
                        GUI_Tools.ObjectTool.ActiveObject(wordProperty.WorldLevlRootObject, false);
                        TextLocalization.SetTextById(wordProperty.WordDescription, "Reform_Exclusive_Text");
                        GUI_Tools.TextTool.SetText(wordProperty.ReformText, EquipTemplate.ExclusiveDescription);
                        break;
                    }
            }
        }
    }

    void RegisteEvent()
    {
        if(null != Equip)
        {
            DataCenter.PlayerDataCenter.OnLockEquip += OnSafeLockRsp;
            DataCenter.PlayerDataCenter.OnEquipUpToHero += OnEquipUpRsp;
            DataCenter.PlayerDataCenter.OnEquipDownFromHero += OnEquipDismissRsp;
            DataCenter.PlayerDataCenter.OnWeaponRefine += OnRefineRsp;
            DataCenter.PlayerDataCenter.OnSpecialWeaponReformReset += OnResetReformPropertyRsp;
            DataCenter.PlayerDataCenter.OnWeaponReform += OnReformWordPropertyRsp;
        }
    }

    void UnRegistEvent()
    {
        DataCenter.PlayerDataCenter.OnLockEquip -= OnSafeLockRsp;
        DataCenter.PlayerDataCenter.OnEquipUpToHero -= OnEquipUpRsp;
        DataCenter.PlayerDataCenter.OnEquipDownFromHero -= OnEquipDismissRsp;
        DataCenter.PlayerDataCenter.OnWeaponRefine -= OnRefineRsp;
        DataCenter.PlayerDataCenter.OnSpecialWeaponReformReset -= OnResetReformPropertyRsp;
        DataCenter.PlayerDataCenter.OnWeaponReform -= OnReformWordPropertyRsp;
    }

    void OnSafeLockClicked()
    {
        gsproto.LockEquipReq req = new gsproto.LockEquipReq();
        req.equip_id = Equip.ServerId;
        req.operation_type = Equip.IsLock ? 0u : 1u;
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnSafeLockRsp(uint equipServerId, uint operationType)
    {
        if(equipServerId == Equip.ServerId)
        {
            RefreshButton();
        }
    }

    void OnDismissButtonClicked()
    {
        DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(Equip.HeroServerId);
        if(null != hero)//equip dismiss
        {
            gsproto.EquipDownFromHeroReq req = new gsproto.EquipDownFromHeroReq();
            req.equip_id = Equip.ServerId;
            req.hero_id = hero.ServerId;
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
        else//equip up
        {
            GUI_EquipPackageUI_DL equipPackegeUI = GUI_Manager.Instance.FindWindowWithName<GUI_EquipPackageUI_DL>("GUI_EquipPackageUI", false);
            if (null != equipPackegeUI && null != equipPackegeUI.CurrentHero)
            {
                gsproto.EquipUpToHeroReq req = new gsproto.EquipUpToHeroReq();
                req.equip_id = Equip.ServerId;
                req.hero_id = equipPackegeUI.CurrentHero.ServerId;
                req.session_id = DataCenter.PlayerDataCenter.SessionId;
                Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
            }
        }
    }

    void OnEquipDismissRsp(uint heroServerId, uint equipServerId)
    {
        if(equipServerId == Equip.ServerId)
        {
            RefreshBindHeroInfo();
            RefreshButton();
        }
    }

    void OnEquipUpRsp(uint heroServerId, uint equipServerId)
    {
        if (equipServerId == Equip.ServerId)
        {
            RefreshBindHeroInfo();
            RefreshButton();
        }
    }

    void OnReformButtonClieked()
    {
        GUI_ReformWeaponUI_DL reformUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ReformWeaponUI_DL>("UI_ReformWeapon", false);
        if(null != reformUI)
        {
            reformUI.ReformWeapon(Equip);
        }
    }

    void OnRefineButtonClicked()
    {
        if(EquipTemplate.CanRefine == 1)
        {
            GUI_RefineWeaponUI_DL refineWeaponUI = GUI_Manager.Instance.ShowWindowWithName<GUI_RefineWeaponUI_DL>("UI_Weapon_jingcui", false);
            if (null != refineWeaponUI)
            {
                refineWeaponUI.RefineEquip(Equip, EquipTemplate);
            }
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("Error_Non_Ancient_Weapon", true);
        }

    }

    void OnRefineRsp(uint oldWeaponServerId, uint newWeaponServerId)
    {
        if (oldWeaponServerId == Equip.ServerId)
        {
            Equip = DataCenter.PlayerDataCenter.GetEquip(newWeaponServerId);
            SetItemInfo(Equip);
            RefreshBindHeroInfo();
            RefreshButton();
            _BindItem.AnchorLogic((int)newWeaponServerId);
        }
    }

    void OnResetReformPropertyRsp(uint weaponServerId)
    {
        if (weaponServerId == Equip.ServerId)
        {
            SetItemInfo(Equip);
            RefreshBindHeroInfo();
            RefreshButton();
        }
    }

    void OnReformWordPropertyRsp(uint equipServerId, uint reformIndex)
    {
        if (equipServerId == Equip.ServerId)
        {
            RefreshWordItem();
        }
    }

    void OnExtendBagMarkClicked()
    {
        GUI_ExtendBagUI_DL extendUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExtendBagUI_DL>("UI_Extend_Hero", false);
        if (null == DisplayWeapon || DisplayWeapon())
        {
            extendUI.ExtendBag(PbCommon.EExendBagType.E_Extend_Weapon_Bag);
        }
        else
        {
            extendUI.ExtendBag(PbCommon.EExendBagType.E_Extend_Ring_Bag);
        }
    }
    #endregion
}
