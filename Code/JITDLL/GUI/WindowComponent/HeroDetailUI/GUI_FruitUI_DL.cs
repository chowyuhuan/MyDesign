using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_FruitUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_FruitUI dataComponent = gameObject.GetComponent<GUI_FruitUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_FruitUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            BigSuccessSlider = dataComponent.BigSuccessValue;
            dataComponent.SelectedFruitIconButtonList[0].onClick.AddListener(OnFruitIcon1Clicked);
            dataComponent.SelectedFruitIconButtonList[1].onClick.AddListener(OnFruitIcon2Clicked);
            dataComponent.SelectedFruitIconButtonList[2].onClick.AddListener(OnFruitIcon3Clicked);
            dataComponent.SelectedFruitIconButtonList[3].onClick.AddListener(OnFruitIcon4Clicked);
            dataComponent.SelectedFruitIconButtonList[4].onClick.AddListener(OnFruitIcon5Clicked);
            dataComponent.SelectedFruitIconButtonList[5].onClick.AddListener(OnFruitIcon6Clicked);

            SelectedFruitIconList = dataComponent.SelectedFruitIconList;
            ExtendFieldAttributeObjectList = dataComponent.FieldAttributes;
            LayoutHelperObject = dataComponent.LayoutHelperObject;
            FruitTabPageObjectList = dataComponent.FruitTabPageObjectList;
            SellPrice = dataComponent.SellPrice;
            EatPrice = dataComponent.EatPrice;
            BigSuccessText = dataComponent.BigSuccessText;
            BigSuccessSlider = dataComponent.BigSuccessValue;
            SellControlArea = dataComponent.SellControlArea;
            ForbiddenEatRoot = dataComponent.ForbiddenEatRoot;
            ForbiddenEatText = dataComponent.ForbiddenEatText;
            FruitBagVolText = dataComponent.FruitBagVolText;

            dataComponent.ExtendFruitPackageButton.onClick.AddListener(OnExtendFruitPackageButtonClick);
            dataComponent.SellButton.onClick.AddListener(OnSellButtonClicked);
            dataComponent.ConfirmSellButton.onClick.AddListener(OnConfirmSellButtonClicked);
            dataComponent.CancelSellButton.onClick.AddListener(OnCancelSellButtonClicked);
            dataComponent.EatButton.onClick.AddListener(OnEatFruitButtonClicked);
        }
    }
    #endregion

    #region field attribute
    DataCenter.Hero Hero;
    CSV_b_hero_fruit_attribute HeroExtendAttributeTemplate;
    List<GameObject> ExtendFieldAttributeObjectList;
    public List<GUI_ExtendFieldAttribute_DL> ExtendFieldAttributes = new List<GUI_ExtendFieldAttribute_DL>();
    Dictionary<PbCommon.EHeroAttributeType, GUI_ExtendFieldAttribute_DL> ExtendFieldFastList;
    Dictionary<PbCommon.EHeroAttributeType, float> ExtendFieldValueFastList;
    public Slider BigSuccessSlider;
    public Text BigSuccessText;
    float BigSuccessRate;
    float BigSuccessAppendPercent;
    public Text SellPrice;
    public Text EatPrice;
    public Text FruitBagVolText;

    void OnExtendFruitPackageButtonClick()
    {
        GUI_ExtendBagUI_DL extendUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExtendBagUI_DL>("UI_Extend_Danyao", false);
        extendUI.ExtendBag(PbCommon.EExendBagType.E_Extend_Fruit_Bag);
    }

    void OnExtendHeroBagRsp(PbCommon.EExendBagType bagType)
    {
        if (bagType == PbCommon.EExendBagType.E_Extend_Fruit_Bag)
        {
            RefreshPackageInfo();
        }
    }

    void RefreshPackageInfo()
    {
        FruitBagVolText.text = string.Format("{0}/{1}", DataCenter.PlayerDataCenter.FruitList.Count.ToString(), DataCenter.PlayerDataCenter.MaxFruitCount.ToString());
    }

    void InitField()
    {
        HeroExtendAttributeTemplate = CSV_b_hero_fruit_attribute.FindData(Hero.CsvId);
        BigSuccessAppendPercent = DefaultConfig.GetFloat("FruitBigSuccessAppendPercent");
    }

    void InitFieldFastDic()
    {
        for (int index = 0; index < ExtendFieldAttributeObjectList.Count; ++index)
        {
            GUI_ExtendFieldAttribute_DL fa = ExtendFieldAttributeObjectList[index].GetComponent<GUI_ExtendFieldAttribute_DL>();
            if (null != fa)
            {
                ExtendFieldAttributes.Add(fa);
            }
        }
        ExtendFieldFastList = new Dictionary<PbCommon.EHeroAttributeType, GUI_ExtendFieldAttribute_DL>();
        ExtendFieldValueFastList = new Dictionary<PbCommon.EHeroAttributeType, float>();
        for (int index = 0; index < ExtendFieldAttributes.Count; ++index)
        {
            ExtendFieldFastList[ExtendFieldAttributes[index].FieldType] = ExtendFieldAttributes[index];
            ExtendFieldValueFastList[ExtendFieldAttributes[index].FieldType] = 0f;
        }
    }

    void SetFieldValue(PbCommon.EHeroAttributeType field, float currentValue, float maxValue)
    {
        GUI_ExtendFieldAttribute_DL fieldAttribute;
        if (ExtendFieldFastList.TryGetValue(field, out fieldAttribute))
        {
            fieldAttribute.RefreshAttribute(currentValue, ExtendFieldValueFastList[field], maxValue, ExtendFieldValueFastList[field] > 0 ? BigSuccessRate : 0f, BigSuccessAppendPercent);
        }
    }

    void RefreshHeroFields()
    {
        if (null != HeroExtendAttributeTemplate)
        {
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack, Hero.FruitAttribute.Attack, HeroExtendAttributeTemplate.Attack);
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp, Hero.FruitAttribute.Hp, HeroExtendAttributeTemplate.Hp);
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage, Hero.FruitAttribute.CrticalDamage, HeroExtendAttributeTemplate.CritDamage);
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate, Hero.FruitAttribute.CrticalRate, HeroExtendAttributeTemplate.CriteRate);
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef, Hero.FruitAttribute.PhysicalDefence, HeroExtendAttributeTemplate.Phdef);
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef, Hero.FruitAttribute.MagicDefence, HeroExtendAttributeTemplate.MDef);
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate, Hero.FruitAttribute.Dodge, HeroExtendAttributeTemplate.EvasionRate);
            SetFieldValue(PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate, Hero.FruitAttribute.Precision, HeroExtendAttributeTemplate.HitRate);
        }
    }

    void ResetFieldAddValue()
    {
        BigSuccessRate = 0;

        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack] = 0f;
        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp] = 0f;
        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage] = 0f;
        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate] = 0f;
        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef] = 0f;
        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef] = 0f;
        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate] = 0f;
        ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate] = 0f;
    }

    void AddFieldValue(PbCommon.EHeroAttributeType attributeType, float value)
    {
        switch(attributeType)
        {
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack:
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp:
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage:
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate:
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef:
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef:
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate:
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate:
                {
                    ExtendFieldValueFastList[attributeType] += value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack] += HeroExtendAttributeTemplate.Attack * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp] += HeroExtendAttributeTemplate.Hp * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage] += HeroExtendAttributeTemplate.CritDamage * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate] += HeroExtendAttributeTemplate.CriteRate * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef] += HeroExtendAttributeTemplate.Phdef * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef] += HeroExtendAttributeTemplate.MDef * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate] += HeroExtendAttributeTemplate.EvasionRate * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate_Percent:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate] += HeroExtendAttributeTemplate.HitRate * value;
                    break;
                }
            case PbCommon.EHeroAttributeType.E_Hero_Attribute_All:
                {
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack] += HeroExtendAttributeTemplate.Attack * value;
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp] += HeroExtendAttributeTemplate.Hp * value;
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage] += HeroExtendAttributeTemplate.CritDamage * value;
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate] += HeroExtendAttributeTemplate.CriteRate * value;
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef] += HeroExtendAttributeTemplate.Phdef * value;
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef] += HeroExtendAttributeTemplate.MDef * value;
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate] += HeroExtendAttributeTemplate.EvasionRate * value;
                    ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate] += HeroExtendAttributeTemplate.HitRate * value;
                    break;
                }
        }
    }
 

    void RefreshFieldAddValue()
    {
        ResetFieldAddValue();
        
        if(!SellFruit)
        {
            for (int index = 0; index < SelectedFruitList.Count; ++index)
            {
                DataCenter.Fruit fruit = DataCenter.PlayerDataCenter.GetFruit(SelectedFruitList[index]);
                if (null != fruit)
                {
                    CSV_b_fruit_template fruitTemplate = CSV_b_fruit_template.FindData(fruit.CsvId);
                    BigSuccessRate += fruitTemplate.SuccessRate;
                    AddFieldValue((PbCommon.EHeroAttributeType)fruitTemplate.AttributeType, fruitTemplate.AttributeValue);
                }
            }
        }

        BigSuccessSlider.value = Mathf.Clamp01((float)BigSuccessRate / 100);
        BigSuccessText.text = string.Format("{0}/100", BigSuccessRate.ToString());
        RefreshHeroFields();
    }
    #endregion

    #region window logic
    public GameObject SellControlArea;
    public GameObject LayoutHelperObject;
    GUI_VerticallayouGroupHelper_DL LayoutHelper;
    public List<GameObject> FruitTabPageObjectList;
    List<GUI_ToggleTabPage_DL> FruitTabPageList;
    int CurrentPageIndex = -1;
    List<uint> SelectedFruitList = new List<uint>();
    bool SellFruit = false;
    const int MaxEatFruitCount = 6;
    CSV_b_hero_template HeroTemplate;
    int HeroMaxSTar;
    public GameObject ForbiddenEatRoot;
    public Text ForbiddenEatText;
    GUI_LogicObjectPool FruitItemPool;

    public void TryEatFruit(DataCenter.Hero hero)
    {
        Hero = hero;
        HeroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
        HeroMaxSTar = DefaultConfig.GetInt("Hero_Max_Star");
        InitField();
    }

    protected override void OnStart()
    {
        InitFruitPage();
        InitFieldFastDic();
        InitHeroInfo();
        if(FruitTabPageList.Count > 0)
        {
            FruitTabPageList[0].Select();
        }

        CheckCondition();
        RefreshUI();
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnSellItem += OnSellFruitRsp;
        DataCenter.PlayerDataCenter.OnFruitListChange += OnFruitListChange;
        DataCenter.PlayerDataCenter.OnHeroFruit += OnEatFruitRsp;
        DataCenter.PlayerDataCenter.OnExtandBag += OnExtendHeroBagRsp;
        GUI_Root_DL.Instance.HideLayer("Default");
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnSellItem -= OnSellFruitRsp;
        DataCenter.PlayerDataCenter.OnFruitListChange -= OnFruitListChange;
        DataCenter.PlayerDataCenter.OnHeroFruit -= OnEatFruitRsp;
        DataCenter.PlayerDataCenter.OnExtandBag -= OnExtendHeroBagRsp;
        GUI_Root_DL.Instance.ShowLayer("Default");
    }

    void InitHeroInfo()
    {

    }

    void InitFruitPage()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Train_Danyao_Detail", true, AssetManage.E_AssetType.UIPrefab);
        FruitItemPool = new GUI_LogicObjectPool(go);

        FruitTabPageList = new List<GUI_ToggleTabPage_DL>();
        for(int index = 0; index < FruitTabPageObjectList.Count; ++index)
        {
            FruitTabPageList.Add(FruitTabPageObjectList[index].GetComponent<GUI_ToggleTabPage_DL>());
        }

        LayoutHelper = LayoutHelperObject.GetComponent<GUI_VerticallayouGroupHelper_DL>();
        LayoutHelper.SetScrollAction(DisplayFruitItem);
        for(int index = 0; index < FruitTabPageList.Count; ++index)
        {
            FruitTabPageList[index].Init(index, LayoutHelper, OnSelectPage, null);
        }
    }

    void DisplayFruitItem(GUI_ScrollItem scrollItem)
    {
        if(null != scrollItem)
        {
            DataCenter.Fruit fruit = DataCenter.PlayerDataCenter.GetFruit((uint)scrollItem.LogicIndex);
            if(null != fruit)
            {
                GUI_FruitItem_DL fruitItem = FruitItemPool.GetOneLogicComponent() as GUI_FruitItem_DL;
                if(null != fruitItem)
                {
                    scrollItem.SetTarget(fruitItem);
                    fruitItem.DisplayFruit(fruit, OnFruitSelect, OnFruitDeSelect, SellingFruitItem, FruitItemSelected);
                }
            }
        }
    }

    bool FruitItemSelected(DataCenter.Fruit fruit)
    {
        if(null != fruit)
        {
            return SelectedFruitList.Contains(fruit.ServerId);
        }
        return false;
    }

    bool SellingFruitItem()
    {
        return SellFruit;
    }

    void OnSelectPage(int pageIndex)
    {
        if(CurrentPageIndex != pageIndex)
        {
            CurrentPageIndex = pageIndex;
        }

        RefreshFruitItem();
    }

    void RefreshFruitItem()
    {
        LayoutHelper.Clear();
        if (0 == CurrentPageIndex)
        {
            ShowAllFruit();
        }
        else
        {
            ShowFruitByType(CurrentPageIndex);
        }
    }

    void ShowAllFruit()
    {
        for (int index = 0; index < DataCenter.PlayerDataCenter.FruitList.Count; ++index)
        {
            LayoutHelper.FillItem((int)DataCenter.PlayerDataCenter.FruitList[index].ServerId);
        }
        LayoutHelper.FillItemEnd();
    }

    void ShowFruitByType(int pageIndex)
    {
        for (int index = 0; index < DataCenter.PlayerDataCenter.FruitList.Count; ++index)
        {
            CSV_b_fruit_template fruitTemplate = CSV_b_fruit_template.FindData(DataCenter.PlayerDataCenter.FruitList[index].CsvId);
            if(pageIndex == fruitTemplate.FruitType)
            {
                LayoutHelper.FillItem((int)DataCenter.PlayerDataCenter.FruitList[index].ServerId);
            }
        }
        LayoutHelper.FillItemEnd();
    }

    void OnFruitSelect(GUI_FruitItem_DL fruitItem)
    {
        if(SellFruit)
        {
            if(!SelectedFruitList.Contains(fruitItem.Fruit.ServerId))
            {
                SelectedFruitList.Add(fruitItem.Fruit.ServerId);
            }
        }
        else
        {
            if(SelectedFruitList.Count < MaxEatFruitCount
                && !SelectedFruitList.Contains(fruitItem.Fruit.ServerId))
            {
                SelectedFruitList.Add(fruitItem.Fruit.ServerId);
            }
            else
            {
                fruitItem.DeSelect();
            }
        }

        RefreshUI();
    }

    void OnFruitDeSelect(GUI_FruitItem_DL fruitItem)
    {
        if (SelectedFruitList.Contains(fruitItem.Fruit.ServerId))
        {
            SelectedFruitList.Remove(fruitItem.Fruit.ServerId);
        }
        RefreshUI();
    }

    void CheckCondition()
    {
        if(SellFruit)
        {
            ForbiddenEatRoot.SetActive(false);
            return;
        }
        string forbiddenText;
        if(HeroTemplate.Star < HeroMaxSTar)
        {
            ForbiddenEatRoot.SetActive(true);
            if(TextLocalization.GetText("Forbidden_Fruit_Until_Max_Star", out forbiddenText))
            {
                ForbiddenEatText.text = forbiddenText;
            }
        }
        else if(AllValueMax())
        {
            ForbiddenEatRoot.SetActive(true);
            if (TextLocalization.GetText("Forbidden_Fruit_For_Max_Value", out forbiddenText))
            {
                ForbiddenEatText.text = forbiddenText;
            }
        }
    }

    void RefreshUI()
    {
        RefreshPackageInfo();
        if (SellFruit)
        {
            UpdateSellPrice();
        }
        else
        {
            UpdateEatPrice();
        }
        RefreshSelectedFruitIcon();
        RefreshFieldAddValue();
    }

    void UpdateSellPrice()
    {
        int totalPrice = 0;
        for (int index = 0; index < SelectedFruitList.Count; ++index)
        {
            DataCenter.Fruit fruit = DataCenter.PlayerDataCenter.GetFruit(SelectedFruitList[index]);
            if (null != fruit)
            {
                CSV_b_fruit_template fruitTemplate = CSV_b_fruit_template.FindData(fruit.CsvId);
                totalPrice += fruitTemplate.SaleCoin;
            }
        }
        SellPrice.text = totalPrice.ToString();
    }

    void UpdateEatPrice()
    {
        int totalPrice = 0;
        for (int index = 0; index < SelectedFruitList.Count; ++index)
        {
            DataCenter.Fruit fruit = DataCenter.PlayerDataCenter.GetFruit(SelectedFruitList[index]);
            if (null != fruit)
            {
                CSV_b_fruit_template fruitTemplate = CSV_b_fruit_template.FindData(fruit.CsvId);
                totalPrice += fruitTemplate.TrainCoin;
            }
        }
        EatPrice.text = totalPrice.ToString();
    }

    void OnSellButtonClicked()
    {
        SellFruit = !SellFruit;
        SelectedFruitList.Clear();
        SellControlArea.SetActive(SellFruit);
        CheckCondition();
        LayoutHelper.RefreshPageData();
        RefreshSelectedFruitIcon();
    }

    void OnCancelSellButtonClicked()
    {
        OnSellButtonClicked();
    }

    void OnConfirmSellButtonClicked()
    {
        gsproto.SellItemReq req = new gsproto.SellItemReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        req.item_ids.AddRange(SelectedFruitList);
        req.item_type = (uint)PbCommon.ESaleItemType.E_Sale_Fruit;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);

    }

    void OnSellFruitRsp(uint itemType, uint coin)
    {
        if (itemType == (uint)PbCommon.ESaleItemType.E_Sale_Fruit)
        {
            SelectedFruitList.Clear();
            RefreshUI();
            RefreshFruitItem();
            GUI_GetGoldCoinUI_DL getGold = GUI_Manager.Instance.ShowWindowWithName<GUI_GetGoldCoinUI_DL>("UI_GetGold", false);
            getGold.GetGoldCoin(coin);
        }
    }

    void OnFruitListChange()
    {
        RefreshUI();
    }

    bool ContainMaxValue()
    {
        return OverHead(Hero.FruitAttribute.Attack, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack], HeroExtendAttributeTemplate.Attack)
            || OverHead(Hero.FruitAttribute.Hp, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp], HeroExtendAttributeTemplate.Hp)
            || OverHead(Hero.FruitAttribute.CrticalDamage, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage], HeroExtendAttributeTemplate.CritDamage)
            || OverHead(Hero.FruitAttribute.CrticalRate, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate], HeroExtendAttributeTemplate.CriteRate)
            || OverHead(Hero.FruitAttribute.PhysicalDefence, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef], HeroExtendAttributeTemplate.Phdef)
            || OverHead(Hero.FruitAttribute.MagicDefence, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef], HeroExtendAttributeTemplate.MDef)
            || OverHead(Hero.FruitAttribute.Dodge, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate], HeroExtendAttributeTemplate.EvasionRate)
            || OverHead(Hero.FruitAttribute.Precision, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate], HeroExtendAttributeTemplate.HitRate);
    }

    bool AllValueMax()
    {
        return ValueMax(Hero.FruitAttribute.Attack, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Attack], HeroExtendAttributeTemplate.Attack)
            && ValueMax(Hero.FruitAttribute.Hp, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Hp], HeroExtendAttributeTemplate.Hp)
            && ValueMax(Hero.FruitAttribute.CrticalDamage, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CritDamage], HeroExtendAttributeTemplate.CritDamage)
            && ValueMax(Hero.FruitAttribute.CrticalRate, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_CriteRate], HeroExtendAttributeTemplate.CriteRate)
            && ValueMax(Hero.FruitAttribute.PhysicalDefence, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_Phdef], HeroExtendAttributeTemplate.Phdef)
            && ValueMax(Hero.FruitAttribute.MagicDefence, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_MDef], HeroExtendAttributeTemplate.MDef)
            && ValueMax(Hero.FruitAttribute.Dodge, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_EvasionRate], HeroExtendAttributeTemplate.EvasionRate)
            && ValueMax(Hero.FruitAttribute.Precision, ExtendFieldValueFastList[PbCommon.EHeroAttributeType.E_Hero_Attribute_HitRate], HeroExtendAttributeTemplate.HitRate);
    }

    bool OverHead(float currentValue, float addValue, float maxValue)
    {
        float totalValue = currentValue + addValue;
        if(BigSuccessRate >= 100)
        {
            totalValue += addValue * BigSuccessAppendPercent;
        }
        return totalValue > maxValue;
    }

    bool ValueMax(float currentValue, float addValue, float maxValue)
    {
        float totalValue = currentValue + addValue;
        if (BigSuccessRate >= 100)
        {
            totalValue += addValue * BigSuccessAppendPercent;
        }
        return totalValue >= maxValue;
    }

    void OnEatFruitButtonClicked()
    {
        if (ContainMaxValue())
        {
            GUI_MessageManager.Instance.ShowMessage("确定使用果实", "包含能力值已打最高值得项目。确定要进行改造吗？", "确定", "取消", DoEatFruit, null, MessageBoxType.ConfirmAndConcell);
        }
        else
        {
            DoEatFruit();
        }
    }

    void DoEatFruit()
    {
        if(SelectedFruitList.Count > 0)
        {
            gsproto.HeroFruitReq req = new gsproto.HeroFruitReq();
            req.fruit_ids.AddRange(SelectedFruitList);
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            req.hero_id = Hero.ServerId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    DataCenter.HeroAttribute StartAttribute = null;
    DataCenter.HeroAttribute EndAttribute = null;
    bool IsBigSuccess = false;

    void OnEatFruitRsp(uint heroServerId, DataCenter.HeroAttribute startAttr, DataCenter.HeroAttribute endAttr, bool isBigSuccess)
    {
        GUI_MessageManager.Instance.ShowMessage("使用果实成功");
        StartAttribute = startAttr;
        EndAttribute = endAttr;
        IsBigSuccess = isBigSuccess;
        StartCoroutine("EatFruitEffect");
    }

    IEnumerator EatFruitEffect()
    {
        yield return null;
        StartCoroutine("GrowUpExtendAttribute");
    }

    IEnumerator GrowUpExtendAttribute()
    {
        SelectedFruitList.Clear();
        yield return null;
        RefreshUI();
        RefreshFruitItem();
    }
    #endregion

    #region selected item area
    public List<Image> SelectedFruitIconList;
    
    void RefreshSelectedFruitIcon()
    {
        int index = 0;
        if(SellFruit)
        {
            for(index = 0; index < SelectedFruitIconList.Count && index < SelectedFruitList.Count; ++index)
            {
                SelectedFruitIconList[index].sprite = null;
            }
            return;
        }
        for(index = 0; index < SelectedFruitIconList.Count && index < SelectedFruitList.Count; ++index)
        {
            DataCenter.Fruit fruit = DataCenter.PlayerDataCenter.GetFruit(SelectedFruitList[index]);
            if(null != fruit)
            {
                CSV_b_fruit_template fruitTemplate = CSV_b_fruit_template.FindData(fruit.CsvId);
                if(null != fruitTemplate)
                {
                    GUI_Tools.IconTool.SetIcon(fruitTemplate.IconAtlas, fruitTemplate.IconSrite, SelectedFruitIconList[index]);
                }
                else
                {
                    SelectedFruitIconList[index].sprite = null;
                }
            }
            else
            {
                SelectedFruitIconList[index].sprite = null;
            }
        }
        for (; index < SelectedFruitIconList.Count; ++index)
        {
            SelectedFruitIconList[index].sprite = null;
        }
    }

    void OnFruitIcon1Clicked()
    {
        OnFruitIconClicked(0);
    }

    void OnFruitIcon2Clicked()
    {
        OnFruitIconClicked(1);
    }

    void OnFruitIcon3Clicked()
    {
        OnFruitIconClicked(2);
    }

    void OnFruitIcon4Clicked()
    {
        OnFruitIconClicked(3);
    }

    void OnFruitIcon5Clicked()
    {
        OnFruitIconClicked(4);
    }

    void OnFruitIcon6Clicked()
    {
        OnFruitIconClicked(5);
    }

    void OnFruitIconClicked(int iconIndex)
    {
        if(iconIndex >= 0 && iconIndex < SelectedFruitIconList.Count && iconIndex < SelectedFruitList.Count)
        {
            SelectedFruitList.RemoveAt(iconIndex);
            RefreshUI();
        }
    }
    #endregion
}
