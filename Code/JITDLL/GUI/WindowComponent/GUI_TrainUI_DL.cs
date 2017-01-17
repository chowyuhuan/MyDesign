using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Network;

public sealed class GUI_TrainUI_DL : GUI_Window_DL
{
    public Image HeroIcon;
    public Slider BigSuccess;
    public Text BigSuccessNum;

    public Text TrainStageNum;
    public Color TrainStageHighestColor;
    public Text CurrentTrainingNum;
    public Text CurrentAppendAttribute;
    public Text StageFinishAppendAttribute;
    public Color AppendAttributeMaxColor;
    public List<Image> _SelectedBreadIconList = new List<Image>();
    List<int> _SelectedBreadItem = new List<int>();

    GameObject GroupLayoutObject;
    public GUI_GridLayoutGroupHelper_DL ScrollView;
    public Text CurrentItemCount;

    GameObject MultipleStageObject;
    public GUI_MultipleStageSlider_DL MulitpleStageSlider;
    DataCenter.Hero _TrainingHero;
    CSV_b_hero_template _HeroTemplate;
    CSV_b_hero_limit _HeroLimit;
    CSV_b_hero_train _HeroTrain;
    CSV_b_hero_train _MaxTrain;
    GUI_LogicObjectPool _BreadItemPool;
    Dictionary<int, CSV_b_hero_train> _TrainDic = new Dictionary<int, CSV_b_hero_train>();
    int _FinishedTrainValue = 0;//已完成的训练值在当前训练阶段的值，如果总的训练值超过了当前阶段需要的训练值，此值应为0
    int _FinishedBaseTrainValue = 0;//已完成的基础训练值在当前训练阶段的剩余值
    int _BaseTrainValue = 0;//必然获得的基础训练值
    int _BigSuccessAppendTrainValue = 0;//大成功时可获得的额外训练值
    int _BigSuccessRate = 0;//大成功几率
    int _TotalTrainValue = 0;
    float _BigSuccessFactor = 0.5f;//大成功时的额外获得比例（Todo:改成配表）
    int _FinishedTrainLevel = 0;


    public Button TrainButton;
    public Text TrainCost;
    public Button SellButton;
    public GameObject SellArea;
    int _SelectItemTrainCost = 0;

    public Button ConfirmSellButton;
    public Text SellItemValue;
    public Button SellAllButton;
    public Button CancelButton;
    public GameObject TrainArea;
    int _SelectItemSellValue = 0;

    public GameObject AppendAttributeArea;
    public GameObject TrainValueArea;
    public Text TrainValue;
    public Text CurrentTrainStage;

    public GameObject MaxTrainStageMask;
    public Text FinalFinishedTrainValue;
    public GUI_TweenPosition FinalFinishedValuePosTweener;
    public GUI_TweenAlpha FinalFinishedValueAlphaTweener;
    public GameObject BigSuccessEffect;
    public GUI_TweenScale BigSucessEffectScaleTweener;
    public Text SuccessText;
    public List<GUI_Tweener> SuccessTextTweeners = new List<GUI_Tweener>();
    public GameObject SuccessProgressEffectRoot;
    public GameObject SuccessProgressEffectProt;
    public GUI_Transform SuccessProgressTrans = new GUI_Transform();

    public float TrainValueSimulateRate = 0.02f;

    bool _TrainingLevelChange = false;
    int _HeroFinishedBaseTrainValue = 0;
    int _HeroFinishedTrainLevel = 0;

    public void SetTrainHero(DataCenter.Hero hero, CSV_b_hero_template heroTemplate)
    {
        InitUI();
        _TrainingHero = hero;
        _HeroTemplate = heroTemplate;
        PrepareData();
        RefreshTrainItem();
        RefreshUI();
    }

    void InitUI()
    {
        MulitpleStageSlider = MultipleStageObject.GetComponent<GUI_MultipleStageSlider_DL>();
        ScrollView = GroupLayoutObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        ScrollView.SetScrollAction(DisplayItem);
        GameObject trainItemProto = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Train_Bread_Item", true, AssetManage.E_AssetType.UIPrefab);
        _BreadItemPool = new GUI_LogicObjectPool(trainItemProto);
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnBreadListChange += RefreshTrainItem;
        DataCenter.PlayerDataCenter.OnTrainHeroSuccess += OnHeroTrainSuccess;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnBreadListChange -= RefreshTrainItem;
        DataCenter.PlayerDataCenter.OnTrainHeroSuccess -= OnHeroTrainSuccess;
    }

    void OnHeroTrainSuccess(uint heroServerId, uint trainValue, bool bigSuccess)
    {
        _SelectedBreadItem.Clear();
        FinalFinishedTrainValue.text = trainValue.ToString();
        FinalFinishedTrainValue.gameObject.SetActive(true);
        FinalFinishedValuePosTweener.PlayForward();
        FinalFinishedValueAlphaTweener.PlayForward();
        string successText;
        SuccessText.gameObject.SetActive(true);

        if (bigSuccess)
        {
            //                 BigSuccessEffect.SetActive(bigSuccess);
            //                 BigSucessEffectScaleTweener.PlayForward(OnBigSuccessEffectEnd);
            AudioManager.Instance.PlaySound("success");

            if (TextLocalization.GetText(TextId.BigSuccess, out successText))
            {
                SuccessText.text = successText;
            }
        }
        else
        {
            AudioManager.Instance.PlaySound("success");
            if (TextLocalization.GetText(TextId.Success, out successText))
            {
                SuccessText.text = successText;
            }
        }
        PlaySuccessTextTweeners();
        int totalTrainValue = _BaseTrainValue;
        if (bigSuccess)
        {
            totalTrainValue += (int)(_BaseTrainValue * _BigSuccessFactor);
        }
        StartCoroutine("SimulateProgress", (int)totalTrainValue);
    }

    int GetMaxSuccessTweenrIndex()
    {
        float maxDuration = 0;
        int maxIndex = 0;
        for (int index = 0; index < SuccessTextTweeners.Count; ++index)
        {
            float duration = SuccessTextTweeners[index].duration + SuccessTextTweeners[index].delay;
            if (duration > maxDuration)
            {
                maxIndex = index;
            }
        }
        return maxIndex;
    }

    IEnumerator SimulateProgress(object value)
    {
        GameObject successEffect = GameObject.Instantiate(SuccessProgressEffectProt);
        GUI_Tools.CommonTool.AddUIChild(SuccessProgressEffectRoot, successEffect, SuccessProgressTrans);
        int totalTrainValue = System.Convert.ToInt32(value);
        int baseTrainingLevel = Mathf.Clamp(_HeroFinishedTrainLevel + 1, 0, _MaxTrain.Level);
        CSV_b_hero_train trainingTemplate = _TrainDic[baseTrainingLevel];
        int trainValue = totalTrainValue;
        while (trainValue > 0)
        {
            int step = (int)(totalTrainValue * TrainValueSimulateRate);
            _HeroFinishedBaseTrainValue += step;
            if (_HeroFinishedBaseTrainValue >= _TrainDic[baseTrainingLevel].NeedTrainNum && baseTrainingLevel < _MaxTrain.Level)
            {
                _HeroFinishedBaseTrainValue -= _TrainDic[baseTrainingLevel].NeedTrainNum;
                baseTrainingLevel++;
            }
            _HeroFinishedBaseTrainValue = Mathf.Clamp(_HeroFinishedBaseTrainValue, 0, _TrainDic[baseTrainingLevel].NeedTrainNum);
            MulitpleStageSlider.SetStageData(ESliderStage.Trible, _TrainDic[baseTrainingLevel].NeedTrainNum, _HeroFinishedBaseTrainValue, 0, 0);
            trainValue -= step;
            CurrentTrainingNum.text = _HeroFinishedBaseTrainValue + "/" + _TrainDic[baseTrainingLevel].NeedTrainNum;
            yield return null;
        }
        RefreshUI();
        GameObject.Destroy(successEffect);
    }

    void PlaySuccessTextTweeners()
    {
        int maxDurationIndex = GetMaxSuccessTweenrIndex();
        for (int index = 0; index < SuccessTextTweeners.Count; ++index)
        {
            if (index == maxDurationIndex)
            {
                SuccessTextTweeners[index].PlayForward(OnSuccessTextTweenersEnd);
            }
            else
            {
                SuccessTextTweeners[index].PlayForward();
            }
        }
    }

    void OnSuccessTextTweenersEnd()
    {
        for (int index = 0; index < SuccessTextTweeners.Count; ++index)
        {
            SuccessTextTweeners[index].ResetToBeginning();
        }
        SuccessText.gameObject.SetActive(false);
    }

    void OnBigSuccessEffectEnd()
    {
        BigSuccessEffect.SetActive(false);
        BigSucessEffectScaleTweener.ResetToBeginning();
    }

    void PrepareData()
    {
        GUI_Tools.IconTool.SetIcon(_HeroTemplate.HeadIconAtlas, _HeroTemplate.HeadIcon, HeroIcon);
        _HeroLimit = CSV_b_hero_limit.FindData(_HeroTemplate.Star);
        _HeroTrain = CSV_b_hero_train.FindData((int)_TrainingHero.EnhanceLevel);
        _MaxTrain = CSV_b_hero_train.FindData(_HeroLimit.TrainingLevel);
        _FinishedTrainLevel = (int)_TrainingHero.EnhanceLevel;
        _TrainDic.Clear();
        for (int index = 0; index <= _MaxTrain.Level; ++index)
        {
            _TrainDic.Add(index, CSV_b_hero_train.FindData(index));
        }
    }

    void UpdateTrainingStageInfo()
    {
        string highesetStr;
        _HeroTrain = CSV_b_hero_train.FindData((int)_TrainingHero.EnhanceLevel);
        CurrentAppendAttribute.text = _HeroTrain.AppendAbility + "%";
        if (_TrainingHero.EnhanceLevel >= _MaxTrain.Level)
        {
            TextLocalization.GetText(TextId.Highest_Value, out highesetStr);
            TrainStageNum.text = GUI_Tools.RichTextTool.Color(TrainStageHighestColor, highesetStr);
            string maxStr;
            TextLocalization.GetText(TextId.Max_Value, out maxStr);
            CurrentTrainingNum.text = "";

            StageFinishAppendAttribute.text = GUI_Tools.RichTextTool.Color(AppendAttributeMaxColor, maxStr);
            MaxTrainStageMask.SetActive(true);
        }
        else
        {
            int finishedRemainStageValue = _FinishedTrainValue + _FinishedBaseTrainValue;
            if (_BigSuccessRate >= 100)
            {
                finishedRemainStageValue += _BigSuccessAppendTrainValue;
            }
            if (finishedRemainStageValue >= _TrainDic[_HeroLimit.TrainingLevel].NeedTrainNum)
            {
                TextLocalization.GetText(TextId.Highest_Value, out highesetStr);
                TrainStageNum.text = GUI_Tools.RichTextTool.Color(TrainStageHighestColor, highesetStr);
            }
            else
            {
                TrainStageNum.text = (_FinishedTrainLevel - 1) + "/" + _HeroLimit.TrainingLevel;
            }

            CurrentTrainingNum.text = (finishedRemainStageValue) + "/" + _TrainDic[_FinishedTrainLevel].NeedTrainNum;
            StageFinishAppendAttribute.text = _TrainDic[_FinishedTrainLevel].AppendAbility + "%";
            MaxTrainStageMask.SetActive(false);
        }
        CurrentTrainStage.text = _TrainingHero.EnhanceLevel + "/" + _HeroLimit.TrainingLevel;
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        GUI_BreadItem_DL breadItem = _BreadItemPool.GetOneLogicComponent() as GUI_BreadItem_DL;
        breadItem.SetBreadItem(DataCenter.PlayerDataCenter.BreadList[scrollItem.LogicIndex], OnBreadSelect, OnBreadDeselect);
        scrollItem.SetTarget(breadItem);
        breadItem.ToggleIndex = scrollItem.LogicIndex;
        if (_SelectedBreadItem.Contains(breadItem.ToggleIndex))
        {
            breadItem.Select();
        }
    }

    void RefreshTrainItem()
    {
        CurrentItemCount.text = DataCenter.PlayerDataCenter.BreadList.Count.ToString() + "/" + DataCenter.PlayerDataCenter.MaxBreadCount;
        ScrollView.FillPage(DataCenter.PlayerDataCenter.BreadList.Count);
        SortItemByStar(true);
    }

    void OnBreadSelect(GUI_BreadItem_DL breadItem)
    {
        if (_SelectedBreadItem.Contains(breadItem.ToggleIndex))
        {
            return;
        }
        if (_SelectedBreadItem.Count < _SelectedBreadIconList.Count)
        {
            _SelectedBreadItem.Add(breadItem.ToggleIndex);
            RefreshUI();
        }
        else
        {
            breadItem.DeSelect();
        }
    }

    void OnBreadDeselect(GUI_BreadItem_DL breadItem)
    {
        if (_SelectedBreadItem.Contains(breadItem.ToggleIndex))
        {
            _SelectedBreadItem.Remove(breadItem.ToggleIndex);
            RefreshUI();
        }
    }

    void TrySwitchTrainValueAndAppendValue()
    {
        if (_SelectedBreadItem.Count > 0)
        {
            AppendAttributeArea.SetActive(false);
            TrainValueArea.SetActive(true);
        }
        else
        {
            AppendAttributeArea.SetActive(true);
            TrainValueArea.SetActive(false);
        }
    }

    void UpdateTrainValue()
    {
        _FinishedTrainValue = 0;
        _BaseTrainValue = 0;
        _BigSuccessRate = 0;
        _BigSuccessAppendTrainValue = 0;
        for (int index = 0; index < _SelectedBreadItem.Count; ++index)
        {
            DataCenter.Bread bread = DataCenter.PlayerDataCenter.BreadList[_SelectedBreadItem[index]];
            CSV_b_bread_template breadTemplate = CSV_b_bread_template.FindData(bread.CsvId);
            _BaseTrainValue += breadTemplate.TrainVolume;
            _BigSuccessRate += breadTemplate.SuccessRate;
        }
        if (_BigSuccessRate > 0)
        {
            _BigSuccessAppendTrainValue = (int)(_BaseTrainValue * _BigSuccessFactor);
            if (_BigSuccessRate >= 100)
            {
                _TotalTrainValue = _BaseTrainValue + _BigSuccessAppendTrainValue;
            }
            else
            {
                _TotalTrainValue = _BaseTrainValue;
            }
        }
        else
        {
            _TotalTrainValue = _BaseTrainValue;
        }
        _FinishedBaseTrainValue = _BaseTrainValue;
    }

    void CalculateFinishedTrainValue()
    {
        _TrainingLevelChange = false;
        int baseTrainingLevel = Mathf.Clamp((int)_TrainingHero.EnhanceLevel + 1, 0, _MaxTrain.Level);
        CSV_b_hero_train trainingTemplate = _TrainDic[baseTrainingLevel];
        int upHead = trainingTemplate.NeedTrainNum - (int)_TrainingHero.EnhanceExp;
        while (_FinishedBaseTrainValue >= upHead && baseTrainingLevel < _MaxTrain.Level)
        {
            _TrainingLevelChange = true;
            ++baseTrainingLevel;
            _FinishedBaseTrainValue -= upHead;
            trainingTemplate = _TrainDic[baseTrainingLevel];
            upHead = trainingTemplate.NeedTrainNum;
        }

        if (_BigSuccessRate >= 100)
        {
            int bigSuccessTrainLevel = baseTrainingLevel;
            trainingTemplate = _TrainDic[bigSuccessTrainLevel];
            upHead -= _FinishedBaseTrainValue;
            while (_BigSuccessAppendTrainValue >= upHead && bigSuccessTrainLevel < _MaxTrain.Level)
            {
                _FinishedBaseTrainValue = 0;
                _TrainingLevelChange = true;
                ++bigSuccessTrainLevel;
                _BigSuccessAppendTrainValue -= upHead;
                trainingTemplate = _TrainDic[bigSuccessTrainLevel];
                upHead = trainingTemplate.NeedTrainNum;
            }
            _FinishedTrainLevel = bigSuccessTrainLevel;
            _BigSuccessAppendTrainValue = Mathf.Clamp(_BigSuccessAppendTrainValue, 0, _TrainDic[bigSuccessTrainLevel].NeedTrainNum);
        }
        else
        {
            _FinishedTrainLevel = baseTrainingLevel;
            _BigSuccessAppendTrainValue = Mathf.Clamp(_BigSuccessAppendTrainValue, 0, _TrainDic[baseTrainingLevel].NeedTrainNum - _FinishedBaseTrainValue);
        }
        if (_TrainingLevelChange)
        {
            _FinishedTrainValue = 0;
        }
        else
        {
            _FinishedTrainValue = (int)_TrainingHero.EnhanceExp;
        }
    }

    void UpdateTrainInfo()
    {
        SellItemValue.text = _SelectItemSellValue.ToString();
        TrainCost.text = _SelectItemTrainCost.ToString();
        BigSuccess.value = Mathf.Clamp(((float)_BigSuccessRate / 100), 0f, 1f);
        BigSuccessNum.text = GUI_Tools.RichTextTool.IntProgressString(_BigSuccessRate, 100);
        TrainValue.text = _TotalTrainValue.ToString();
        if (_TrainingHero.EnhanceLevel == _MaxTrain.Level)
        {
            MulitpleStageSlider.SetStageData(ESliderStage.Trible, _MaxTrain.NeedTrainNum, _MaxTrain.NeedTrainNum, 0, 0);
        }
        else
        {

            MulitpleStageSlider.SetStageData(ESliderStage.Trible, _TrainDic[_FinishedTrainLevel].NeedTrainNum, _FinishedTrainValue, _FinishedBaseTrainValue, _BigSuccessAppendTrainValue);
        }
    }

    void RefreshUI()
    {
        RefreshSelectBreadIcon();
        UpdateTrainValue();
        CalculateFinishedTrainValue();
        UpdateTrainInfo();
        UpdateTrainingStageInfo();
        TrySwitchTrainValueAndAppendValue();
    }

    void RefreshSelectBreadIcon()
    {
        _SelectItemTrainCost = 0;
        _SelectItemSellValue = 0;
        int index = 0;
        for (; index < _SelectedBreadItem.Count; ++index)
        {
            DataCenter.Bread bread = DataCenter.PlayerDataCenter.BreadList[_SelectedBreadItem[index]];
            CSV_b_bread_template breadTemplate = CSV_b_bread_template.FindData(bread.CsvId);
            GUI_Tools.IconTool.SetIcon(breadTemplate.IconAtlas, breadTemplate.BreadIcon, _SelectedBreadIconList[index]);
            _SelectItemTrainCost += breadTemplate.GetTrainCoin(_HeroTemplate.Star);
            _SelectItemSellValue += breadTemplate.SaleCoin;
        }
        for (; index < _SelectedBreadIconList.Count; ++index)
        {
            _SelectedBreadIconList[index].sprite = null;
        }
        TrainButton.interactable = (_SelectedBreadItem.Count > 0 && _TrainingHero.EnhanceLevel < _MaxTrain.Level);
    }

    void UpdateBigSuccessValue()
    {
        BigSuccess.value = 0f;
    }

    public void OnTrainButtonClicked()
    {
        if (_SelectedBreadItem.Count > 0)
        {
            _HeroFinishedBaseTrainValue = (int)_TrainingHero.EnhanceExp;
            _HeroFinishedTrainLevel = (int)_TrainingHero.EnhanceLevel;
            gsproto.TrainHeroReq trainHeroReq = new gsproto.TrainHeroReq();
            trainHeroReq.hero_id = _TrainingHero.ServerId;
            trainHeroReq.session_id = DataCenter.PlayerDataCenter.SessionId;
            for (int index = 0; index < _SelectedBreadItem.Count; ++index)
            {
                trainHeroReq.bread_ids.Add(DataCenter.PlayerDataCenter.BreadList[_SelectedBreadItem[index]].ServerId);
            }

            NetworkManager.SendRequest(ProtocolDataType.TcpShort, trainHeroReq);
        }
    }
    public void OnSellButtonClicked()
    {
        TrainArea.SetActive(false);
        SellArea.SetActive(true);
    }
    public void OnConfirmSellButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }
    public void OnCancelSellButtonClicked()
    {
        TrainArea.SetActive(true);
        SellArea.SetActive(false);
    }
    public void OnSellAllButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnSortItemTypeButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnSortItemDirectionClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    public void OnAddItemHoldCountButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    void SortItemByStar(bool decrease)
    {
        ScrollView.Sort(GUI_BreadItem_DL.CompareByStar, decrease);
    }

    public void OnSelectedItemIconClicked(Image icon)
    {
        if (null != icon)
        {
            int index = _SelectedBreadIconList.IndexOf(icon);
            if (index < _SelectedBreadItem.Count && index >= 0)
            {
                bool deselect = false;
                GUI_ScrollItem si = ScrollView.GetAtIndex(_SelectedBreadItem[index]);
                if (null != si)
                {
                    GUI_BreadItem_DL bi = si.LogicObject as GUI_BreadItem_DL;
                    if (null != bi)
                    {
                        bi.DeSelect();
                        deselect = true;
                    }
                }
                if (!deselect)
                {
                    _SelectedBreadItem.RemoveAt(index);
                }
                RefreshUI();
            }
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_TrainUI dataComponent = gameObject.GetComponent<GUI_TrainUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_TrainUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HeroIcon = dataComponent.HeroIcon;
            BigSuccess = dataComponent.BigSuccess;
            BigSuccessNum = dataComponent.BigSuccessNum;

            TrainStageNum = dataComponent.TrainStageNum;
            TrainStageHighestColor = dataComponent.TrainStageHighestColor;
            CurrentTrainingNum = dataComponent.CurrentTrainingNum;
            CurrentAppendAttribute = dataComponent.CurrentAppendAttribute;
            StageFinishAppendAttribute = dataComponent.StageFinishAppendAttribute;
            AppendAttributeMaxColor = dataComponent.AppendAttributeMaxColor;
            _SelectedBreadIconList = dataComponent._SelectedBreadIconList;

            ScrollView = dataComponent.ScrollView.GetComponent<GUI_GridLayoutGroupHelper_DL>();
            CurrentItemCount = dataComponent.CurrentItemCount;

            GroupLayoutObject = dataComponent.ScrollView;
            MultipleStageObject = dataComponent.MulitpleStageSlider;


            TrainButton = dataComponent.TrainButton;
            TrainCost = dataComponent.TrainCost;
            SellButton = dataComponent.SellButton;
            SellArea = dataComponent.SellArea;

            ConfirmSellButton = dataComponent.ConfirmSellButton;
            SellItemValue = dataComponent.SellItemValue;
            SellAllButton = dataComponent.SellAllButton;
            CancelButton = dataComponent.CancelButton;
            TrainArea = dataComponent.TrainArea;

            AppendAttributeArea = dataComponent.AppendAttributeArea;
            TrainValueArea = dataComponent.TrainValueArea;
            TrainValue = dataComponent.TrainValue;
            CurrentTrainStage = dataComponent.CurrentTrainStage;

            MaxTrainStageMask = dataComponent.MaxTrainStageMask;
            FinalFinishedTrainValue = dataComponent.FinalFinishedTrainValue;
            FinalFinishedValuePosTweener = dataComponent.FinalFinishedValuePosTweener;
            FinalFinishedValueAlphaTweener = dataComponent.FinalFinishedValueAlphaTweener;
            BigSuccessEffect = dataComponent.BigSuccessEffect;
            BigSucessEffectScaleTweener = dataComponent.BigSucessEffectScaleTweener;
            SuccessText = dataComponent.SuccessText;
            SuccessTextTweeners = dataComponent.SuccessTextTweeners;
            SuccessProgressEffectRoot = dataComponent.SuccessProgressEffectRoot;
            SuccessProgressEffectProt = dataComponent.SuccessProgressEffectProt;
            SuccessProgressTrans = dataComponent.SuccessProgressTrans;

            TrainValueSimulateRate = dataComponent.TrainValueSimulateRate;

            dataComponent.TrainButton.onClick.AddListener(OnTrainButtonClicked);
            dataComponent.SellButton.onClick.AddListener(OnSellButtonClicked);
            dataComponent.SellAllButton.onClick.AddListener(OnSellAllButtonClicked);
            dataComponent.ConfirmSellButton.onClick.AddListener(OnConfirmSellButtonClicked);
            dataComponent.CancelButton.onClick.AddListener(OnCancelSellButtonClicked);
            dataComponent.ExtendBreadBagButton.onClick.AddListener(OnAddItemHoldCountButtonClicked);
            dataComponent.SortItemTypeButton.onClick.AddListener(OnSortItemTypeButtonClicked);
            dataComponent.SortItemDirctionButton.onClick.AddListener(OnSortItemDirectionClicked);
        }
    }
}