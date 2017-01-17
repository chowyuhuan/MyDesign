using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_HeroDetailUI_DL : GUI_Window_DL, GUI_IOnTopHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region simple info area
    public Image MaxLevelIcon;
    public Text HeroName;

    GameObject StarBarObject;
    public GUI_HeroStarBar_DL HeroStar;
    public GameObject LockIcon;
    public GameObject UnlockIcon;

    public void OnSafeLockClicked()
    {
        gsproto.LockHeroReq req = new gsproto.LockHeroReq();
        req.hero_id = _DisplayHero.ServerId;
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        if (_DisplayHero.IsLock)
        {
            //unlock 0
            req.operation_type = 0;
        }
        else
        {
            //lock 1
            req.operation_type = 1;
        }
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    public void OnHeroStoryButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    void RefreshSimplaInfo()
    {
        HeroStar.SetStarNum(_HeroTemplate.Star);
        HeroName.text = _HeroTemplate.Name;
        SchoolName.text = GUI_Tools.TextTool.GetSchoolText((PbCommon.ESchoolType)_HeroTemplate.School);
        LockIcon.SetActive(_DisplayHero.IsLock);
        UnlockIcon.SetActive(!_DisplayHero.IsLock);
    }
    #endregion

    #region field attribute
    public Text Level;
    public Slider Experience;
    public Image SchoolIcon;
    public Text SchoolName;
    List<GameObject> FieldAttributeObjectList;
    public List<GUI_FieldAttribute_DL> FieldAttributes = new List<GUI_FieldAttribute_DL>();
    Dictionary<ACTOR.ActorField, GUI_FieldAttribute_DL> _FieldFastList;

    void InitFieldFastDic()
    {
        for (int index = 0; index < FieldAttributeObjectList.Count; ++index )
        {
            GUI_FieldAttribute_DL fa = FieldAttributeObjectList[index].GetComponent<GUI_FieldAttribute_DL>();
            if(null != fa)
            {
                FieldAttributes.Add(fa);
            }
        }
        _FieldFastList = new Dictionary<ACTOR.ActorField, GUI_FieldAttribute_DL>();
        for (int index = 0; index < FieldAttributes.Count; ++index)
        {
            _FieldFastList[FieldAttributes[index].FieldType] = FieldAttributes[index];
        }
    }

    void SetFieldValue(ACTOR.ActorField field, float baseValue, float extendValue)
    {
        GUI_FieldAttribute_DL fieldAttribute;
        if (_FieldFastList.TryGetValue(field, out fieldAttribute))
        {
            fieldAttribute.SetPropertyValue(baseValue, extendValue);
        }
    }

    void SetHeroFields(DataCenter.Hero hero, CSV_b_hero_template heroTemplate)
    {
        CSV_b_hero_limit heroLimit = CSV_b_hero_limit.FindData(heroTemplate.Star);
        Level.text = string.Format("{0}{1}{2}{3}", "等级", hero.Level.ToString(), "/", heroLimit.MaxLevel.ToString());
        int nextLevel;
        if (hero.Level == heroLimit.MaxLevel)
        {
            nextLevel = heroLimit.MaxLevel;
        }
        else
        {
            nextLevel = (int)hero.Level + 1;
        }
        CSV_b_level_template levelTemplete = CSV_b_level_template.FindData(nextLevel);
        Experience.value = hero.Exp / levelTemplete.Exp;

        SetFieldValue(ACTOR.ActorField.ATK, hero.BaseAttribute.Attack, hero.ExtraAttribute.Attack);
        SetFieldValue(ACTOR.ActorField.HP, hero.BaseAttribute.Hp, hero.ExtraAttribute.Hp);
        SetFieldValue(ACTOR.ActorField.CritDamage, hero.BaseAttribute.CrticalDamage, hero.ExtraAttribute.CrticalDamage);
        SetFieldValue(ACTOR.ActorField.CritRate, hero.BaseAttribute.CrticalRate, hero.ExtraAttribute.CrticalRate);
        SetFieldValue(ACTOR.ActorField.DFPhysics, hero.BaseAttribute.PhysicalDefence, hero.ExtraAttribute.PhysicalDefence);
        SetFieldValue(ACTOR.ActorField.DFMagic, hero.BaseAttribute.MagicDefence, hero.ExtraAttribute.MagicDefence);
        SetFieldValue(ACTOR.ActorField.Dodge, hero.BaseAttribute.Dodge, hero.ExtraAttribute.Dodge);
        SetFieldValue(ACTOR.ActorField.Precision, hero.BaseAttribute.Precision, hero.ExtraAttribute.Precision);
    }
    #endregion

    #region equipment
    public Image WeaponIcon;
    public Image RingIcon;
    CSV_b_equip_template EquipTemplate;
    public void OnEquipButtonClicked()
    {
        GUI_Root_DL.Instance.HideLayer("Default");
        GUI_Manager.Instance.ShowWindowWithName("GUI_EquipPackageUI", false);
    }

    void SetEquipInfo()
    {
        if(null != _DisplayHero && null != _DisplayHero.Weapon)
        {
            EquipTemplate = CSV_b_equip_template.FindData(_DisplayHero.Weapon.CsvId);
            if(null != EquipTemplate)
            {
                GUI_Tools.IconTool.SetIcon(EquipTemplate.IconAtlas, EquipTemplate.IconSprite, WeaponIcon);
            }
        }
    }
    #endregion

    #region skill
    public Image CubeSkillIcon;
    public Text CubeSkillLevel;
    public Text CubeSkillType;
    public Image SpecialSkillIcon;
    public Text SpecialSkillLevel;
    public Text SpecialSkillType;
    public RectTransform CubeSkillIconClickArea;
    public GameObject SkillTipObject;
    public Text SkillName;
    public Text SkillDescription;
    public Text SkillPassiveEffect;
    bool _CubeSkillTipShowing = false;
    public void OnGetSkillButtonClicked()
    {
        GUI_Root_DL.Instance.HideLayer("Default");
        Building_DL.FocusBuilding(BuildingType.Skill);
        GUI_SpecialSkillManageUI_DL specialSkillUI = GUI_Manager.Instance.ShowWindowWithName<GUI_SpecialSkillManageUI_DL>("UI_HeroSkill", false);
        if(null != specialSkillUI)
        {
            specialSkillUI.ShowSpecialSkillAndFocusOn(_DisplayHero);
        }
    }

    void SetHeroSkill(DataCenter.Hero hero, CSV_b_hero_template heroTemplate)
    {
        SetCubeSkill(heroTemplate);
        SetSpecialSkill(hero);
    }

    void SetCubeSkill(CSV_b_hero_template heroTemplate)
    {
        if(null == heroTemplate)
        {
            return;
        }
        SKILL.Skill cubeSkill;
        if (SkillDataCenter.Instance.TryToGetSkill(heroTemplate.SkillID1, out cubeSkill))
        {
            GUI_Tools.IconTool.SetIcon(cubeSkill.IconAtlas, cubeSkill.IconSprite, CubeSkillIcon);
            CSV_c_skill_description cubeDes = CSV_c_skill_description.FindData(cubeSkill.ID);
            if (null != cubeDes)
            {
                CubeSkillLevel.text = cubeDes.LevelDescription;
                CubeSkillType.text = cubeDes.TypeDescription;
            }
            else
            {
                CubeSkillLevel.text = "";
                CubeSkillType.text = "";
            }
        }
        else
        {
            CubeSkillLevel.text = "";
            CubeSkillType.text = "";
        }
    }

    void SetSpecialSkill(DataCenter.Hero hero)
    {
        if(null == hero)
        {
            return;
        }
        CSV_b_skill_template skillTemplate = CSV_b_skill_template.FindData((int)hero.SkillServerId);
        if (null != skillTemplate)
        {
            string levelText;
            DataCenter.SpecialSkillUnit skillUnit = DataCenter.PlayerDataCenter.SpecialSkillData.GetSpecialSkill((uint)skillTemplate.GroupId);
            if (TextLocalization.GetText(skillUnit.Level == skillUnit.MaxLevel ? TextId.Top_Level : TextId.Level, out levelText))
            {
                SpecialSkillLevel.text = levelText + skillTemplate.Level.ToString();
            }
            else
            {
                SpecialSkillLevel.text = skillTemplate.Level.ToString();
            }

            GUI_Tools.IconTool.SetSkillIcon(skillTemplate.Id, SpecialSkillIcon);
        }
        else
        {
            SpecialSkillLevel.text = "";
            SpecialSkillType.text = "";
        }
    }

    public void OnCubeSkillIconClicked()
    {
        _CubeSkillTipShowing = !_CubeSkillTipShowing;
        SkillTipObject.SetActive(_CubeSkillTipShowing);
        if (_CubeSkillTipShowing)
        {
            CSV_c_skill_description sd = CSV_c_skill_description.FindData(_HeroTemplate.SkillID1);
            if (null != sd)
            {
                SkillName.text = sd.Name;
                SkillDescription.text = sd.Description;
                SkillPassiveEffect.text = sd.PassiveDescription;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_CubeSkillTipShowing && !RectTransformUtility.RectangleContainsScreenPoint(CubeSkillIconClickArea, eventData.position))
        {
            OnCubeSkillIconClicked();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(RectTransformUtility.RectangleContainsScreenPoint(LeftRotateTouchRect, eventData.position))
        {
            _LeftRotateButtonPressed = true;
            _RightRotateButtonPressed = false;
        }
        else if(RectTransformUtility.RectangleContainsScreenPoint(RightRotateTouchRect, eventData.position))
        {
            _LeftRotateButtonPressed = false;
            _RightRotateButtonPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _LeftRotateButtonPressed = false;
        _RightRotateButtonPressed = false;
    }
    #endregion

    #region manage button
    public Button EvolutionButton;
    public void OnEvolutionButtonClicked()
    {
        GUI_EvolutionUI_DL evolutionUI = GUI_Manager.Instance.ShowWindowWithName<GUI_EvolutionUI_DL>("GUI_EvolutionUI", false);
        evolutionUI.SetEvolutionInfo(_DisplayHero, _HeroTemplate);
    }

    public void OnTrainButtonClicked()
    {
        GUI_TrainUI_DL trainUI = GUI_Manager.Instance.ShowWindowWithName<GUI_TrainUI_DL>("GUI_TrainUI", false);
        trainUI.SetTrainHero(_DisplayHero, _HeroTemplate);
    }

    public void OnFruitButtonClicked()
    {
        GUI_FruitUI_DL fruitUI = GUI_Manager.Instance.ShowWindowWithName<GUI_FruitUI_DL>("UI_Danyao", false);
        if(null != fruitUI)
        {
            fruitUI.TryEatFruit(_DisplayHero);
        }
    }
    #endregion

    #region display control
    public RectTransform LeftRotateTouchRect;
    public RectTransform RightRotateTouchRect;
    bool _InitDone = false;
    DataCenter.Hero _DisplayHero;
    CSV_b_hero_template _HeroTemplate;
    public RectTransform _ModelDisplayRect;
    Transform _ModelTransform;
    public GUI_Transform ModelTrans;
    public string _ModelAction = "run";
    public float _ButtonRotateSpeed = 300f;
    public float _DragRotateSpeed = 100f;
    bool _LeftRotateButtonPressed = false;
    bool _RightRotateButtonPressed = false;
    GameObject _CurHeroModel;
    bool _DragingModel = false;
    public void ShowHero(DataCenter.Hero hero, CSV_b_hero_template heroTemplate)
    {
        _DisplayHero = hero;
        _HeroTemplate = heroTemplate;
        Init();
        RefreshUI();
        RefreshHeroModel();
    }

    void RefreshHeroModel()
    {
        if (_CurHeroModel != null)
        {
            GameObject.Destroy(_CurHeroModel);
        }
        if (GUI_Tools.ModelTool.SpawnModel(_ModelDisplayRect.gameObject, _HeroTemplate.Prefab, ModelTrans, out _CurHeroModel))
        {
            _ModelTransform = _CurHeroModel.transform;
            Animator anim;
            if(GUI_Tools.ModelTool.AnimateUIModel(_CurHeroModel, _HeroTemplate.UiAnimCtrl, out anim))
            {
                anim.Play(_ModelAction);
            }
            ActorWeaponInfo actorWeaponInfo = _DisplayHero.GetActorWeaponInfo();
            ActorWeaponHelper.SetActorWeapon(_CurHeroModel, ActorWeaponHelper.SpawnActorWeaponWithUIScale(_ModelDisplayRect.gameObject.transform, ModelTrans, actorWeaponInfo), actorWeaponInfo);
        }
    }

    void RefreshUI()
    {
        RefreshSimplaInfo();
        CSV_c_school_config school = CSV_c_school_config.FindData(_HeroTemplate.School);
        GUI_Tools.IconTool.SetIcon(school.Atlas, school.Icon, SchoolIcon);
        SetHeroFields(_DisplayHero, _HeroTemplate);
        CheckEvolution();
        SetHeroSkill(_DisplayHero, _HeroTemplate);
        SetEquipInfo();
    }

    void CheckEvolution()
    {
        CSV_b_hero_limit heroLimit = CSV_b_hero_limit.FindData(_HeroTemplate.Star);
#if TEST
        EvolutionButton.interactable = true;
#else
            if(_DisplayHero.EnhanceLevel == heroLimit.TrainingLevel)
            {
                EvolutionButton.interactable = true;
            }
            else
            {
                EvolutionButton.interactable = false;
            }
#endif
    }

    void Init()
    {
        if (!_InitDone)
        {
            _InitDone = true;
            HeroStar = StarBarObject.GetComponent<GUI_HeroStarBar_DL>();
            HeroStar.gameObject.SetActive(false);
            InitFieldFastDic();
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnHeroDataChange += OnHeroDataChange;
        DataCenter.PlayerDataCenter.OnHeroEvolution += OnHeroDataChange;
        GUI_Root_DL.Instance.ShowLayer("Default");
    }

    public void OnTop()
    {
        GUI_Root_DL.Instance.ShowLayer("Default");
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnHeroDataChange -= OnHeroDataChange;
        DataCenter.PlayerDataCenter.OnHeroEvolution -= OnHeroDataChange;
        GUI_Root_DL.Instance.HideLayer("Default");
    }

    public override void PreHideWindow()
    {
        GUI_Manager.Instance.HideWindowWithName("GUI_TrainUI");
    }

    public void OnHeroDataChange(uint heroServerId)
    {
        if (heroServerId == _DisplayHero.ServerId)
        {
            if (_HeroTemplate.Id != (int)_DisplayHero.CsvId)
            {
                _HeroTemplate = CSV_b_hero_template.FindData(_DisplayHero.CsvId);
                RefreshHeroModel();
            }

            RefreshUI();
        }
    }

    protected override void OnUpdate()
    {
        if (_LeftRotateButtonPressed)
        {
            OnLeftRotateButtonClicked();
        }
        if (_RightRotateButtonPressed)
        {
            OnRightRotateButtonClicked();
        }
    }

    public void OnLeftRotateButtonPointEvent()
    {
        _LeftRotateButtonPressed = !_LeftRotateButtonPressed;
    }

    public void OnRightRotateButtonPointEvent()
    {
        _RightRotateButtonPressed = !_RightRotateButtonPressed;
    }

    public void OnLeftRotateButtonClicked()
    {
        if (null != _ModelTransform)
        {
            _ModelTransform.Rotate(new Vector3(0, Time.deltaTime * _ButtonRotateSpeed, 0));
        }
    }

    public void OnRightRotateButtonClicked()
    {
        if (null != _ModelTransform)
        {
            _ModelTransform.Rotate(new Vector3(0, -Time.deltaTime * _ButtonRotateSpeed, 0));
        }
    }

    public void OnDrag(PointerEventData pe)
    {
        if (_DragingModel || RectTransformUtility.RectangleContainsScreenPoint(_ModelDisplayRect, pe.position, GUI_Root_DL.Instance.UICamera))
        {
            _ModelTransform.Rotate(new Vector3(0, pe.delta.x * _DragRotateSpeed, 0));
        }
    }

    public void OnBeginDrag(PointerEventData pe)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(_ModelDisplayRect, pe.position, GUI_Root_DL.Instance.UICamera))
        {
            _DragingModel = true;
        }
    }

    public void OnEndDrag(PointerEventData pe)
    {
        _DragingModel = false;
    }
    #endregion

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroDetailUI dataComponent = gameObject.GetComponent<GUI_HeroDetailUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroDetailUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            MaxLevelIcon = dataComponent.MaxLevelIcon;
            HeroName = dataComponent.HeroName;
            StarBarObject = dataComponent.HeroStar;
            LockIcon = dataComponent.LockIcon;
            UnlockIcon = dataComponent.UnlockIcon;
            Level = dataComponent.Level;
            Experience = dataComponent.Experience;
            SchoolIcon = dataComponent.SchoolIcon;
            SchoolName = dataComponent.SchoolName;
            FieldAttributeObjectList = dataComponent.FieldAttributes;
            WeaponIcon = dataComponent.WeaponIcon;
            RingIcon = dataComponent.RingIcon;
            CubeSkillIcon = dataComponent.CubeSkillIcon;
            CubeSkillLevel = dataComponent.CubeSkillLevel;
            CubeSkillType = dataComponent.CubeSkillType;
            SpecialSkillIcon = dataComponent.SpecialSkillIcon;
            SpecialSkillLevel = dataComponent.SpecialSkillLevel;
            SpecialSkillType = dataComponent.SpecialSkillType;
            CubeSkillIconClickArea = dataComponent.CubeSkillIconClickArea;
            SkillTipObject = dataComponent.SkillTipObject;
            SkillName = dataComponent.SkillName;
            SkillDescription = dataComponent.SkillDescription;
            SkillPassiveEffect = dataComponent.SkillPassiveEffect;
            _ModelDisplayRect = dataComponent._ModelDisplayRect;
            EvolutionButton = dataComponent.EvolutionButton;
            ModelTrans = dataComponent.ModelTrans;

            LeftRotateTouchRect = dataComponent.LeftRotateTouchRect;
            RightRotateTouchRect = dataComponent.RightRotateTouchRect;

            //dataComponent.LeftRotateButton.onClick.AddListener(OnLeftRotateButtonClicked);
            //dataComponent.RightRotateButton.onClick.AddListener(OnRightRotateButtonClicked);
            dataComponent.SafeLockButton.onClick.AddListener(OnSafeLockClicked);
            dataComponent.StoryButton.onClick.AddListener(OnHeroStoryButtonClicked);
            dataComponent.EvolutionButton.onClick.AddListener(OnEvolutionButtonClicked);
            dataComponent.TrainButton.onClick.AddListener(OnTrainButtonClicked);

            dataComponent.EquipWeaponButton.onClick.AddListener(OnEquipButtonClicked);
            dataComponent.GetSkillButton.onClick.AddListener(OnGetSkillButtonClicked);
            dataComponent.CubeSkillButton.onClick.AddListener(OnCubeSkillIconClicked);
            dataComponent.FruitButton.onClick.AddListener(OnFruitButtonClicked);
        }
    }
}
