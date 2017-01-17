using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_BattleUI_DL : GUI_Window_DL
{
    public GameObject MonsterWarnObject;
    public GUI_MonsterWave_DL _WaveWarn;
    public GameObject SpecialWarnObject;
    public GUI_SpecialWarning_DL _SpecialWarning;
    public GameObject _SkillScrow;
    public GameObject SkillCubeSpawnRoot;
    public Transform SkillCubeSpawnPos;
    public List<Transform> _SkillCubePosList;
    List<GameObject> HeroInfoObjectList;
    public List<GUI_HeroInfo_DL> _HeroInfoList;
    public Text MonsterWave;
    Dictionary<int, int> _HeroDisplayOrder = new Dictionary<int, int>();
    int _HeroCount = 0;

    public Text TreasureBoxCount;
    public Text GoldCoinCount;
    int _TreaserBoxCount;
    int _GoldCoinCount;

    public Text ChapterDescription;
    public Text ChapterDetailInfo;
    public GameObject ChapterInfo;
    public float ChapterInfoShowTime;
    public GameObject EnemyHeadHpSpawnRoot;
    public GameObject HeroHeadHpSpawnRoot;
    List<GameObject> SkillHeadUpWarnObjectList;
    public List<GUI_SkillHeadUpWarnTip_DL> SkillHeadUpWarnTipList = new List<GUI_SkillHeadUpWarnTip_DL>();

    public List<GameObject> CubeClickEffectProt;
    public List<GameObject> DoubleCubeAlignProt;
    public List<GameObject> TribleCubeAlignProt;

    public GUI_Transform CubeClickTrans;
    public GUI_Transform DoubleCubeAlignTrans;
    public GUI_Transform TribleCubeAlignTrans;

    public Image BoxImage;
    public Image GoldImage;

    Dictionary<int, GUI_LogicObjectPool> _CubeClickEffectPool = new Dictionary<int, GUI_LogicObjectPool>();
    Dictionary<int, GUI_LogicObjectPool> _DoubleCubeAlignPool = new Dictionary<int, GUI_LogicObjectPool>();
    Dictionary<int, GUI_LogicObjectPool> _TribleCubeAlignPool = new Dictionary<int, GUI_LogicObjectPool>();
    Dictionary<int, GUI_EffectPlayer_DL> _CubeClickEffects = new Dictionary<int, GUI_EffectPlayer_DL>();

    void InitCubeEffectPool(int index)
    {
#if UNITY_EDITOR
        Debug.Assert(index < CubeClickEffectProt.Count && index < DoubleCubeAlignProt.Count && index < TribleCubeAlignProt.Count);
#endif
        _CubeClickEffectPool.Add(index, new GUI_LogicObjectPool(CubeClickEffectProt[index]));
        _DoubleCubeAlignPool.Add(index, new GUI_LogicObjectPool(DoubleCubeAlignProt[index]));
        _TribleCubeAlignPool.Add(index, new GUI_LogicObjectPool(TribleCubeAlignProt[index]));
    }

    protected override void OnStart()
    {
        _HeroInfoList = new List<GUI_HeroInfo_DL>();
        _WaveWarn = MonsterWarnObject.GetComponent<GUI_MonsterWave_DL>();
        _SpecialWarning = SpecialWarnObject.GetComponent<GUI_SpecialWarning_DL>();
        for (int index = 0; index < HeroInfoObjectList.Count; ++index )
        {
            _HeroInfoList.Add(HeroInfoObjectList[index].GetComponent<GUI_HeroInfo_DL>());
        }
        for (int index = 0; index < SkillHeadUpWarnObjectList.Count; ++index)
        {
            SkillHeadUpWarnTipList.Add(SkillHeadUpWarnObjectList[index].GetComponent<GUI_SkillHeadUpWarnTip_DL>());
            SkillHeadUpWarnObjectList[index].SetActive(false);
        }
        GoldCoinCount.text = _GoldCoinCount.ToString();
        TreasureBoxCount.text = _TreaserBoxCount.ToString();
        ChapterInfo.SetActive(true);
        ChapterDescription.text = GUI_BattleManager.Instance.SelectedLevel.StartDescription;
        ChapterDetailInfo.text = GUI_BattleManager.Instance.SelectedLevel.StartInformation;
        Invoke("OnChapterInfoEnd", ChapterInfoShowTime);
    }

    public void OnChapterInfoEnd()
    {
        ChapterInfo.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="displayIndex">从左到右：0，1，2</param>
    /// <param name="heroId"></param>
    /// <param name="heroLevel"></param>
    /// <param name="captain"></param>
    /// <param name="hp"></param>
    /// <param name="sp"></param>
    /// <returns></returns>
    public GUI_HeroInfo_DL DisplayHeroInfo(Actor hero)
    {
        if(_HeroCount < _HeroInfoList.Count)
        {
            bool captain = GUI_BattleManager.Instance.IsCaptain(_HeroCount);
            _HeroInfoList[_HeroCount].Init(hero, hero.BattleId, hero.ActorLevel, captain, (int)hero.GetValue(ACTOR.ActorField.HP));
            _HeroDisplayOrder.Add(hero.BattleId, _HeroCount);
            hero.OnDeath += this.OnHeroDead;
            return _HeroInfoList[_HeroCount++];
        }
        return null;
    }

    public void OnHeroDead(Actor hero)
    {
        hero.OnDeath -= this.OnHeroDead;
        for (int index = 0; index < _CachedSkill.Count; ++index)
        {
            if (_CachedSkill[index]._HeroBattleId == hero.BattleId)
            {
                _CachedSkill[index].HeroAlive(false);
            }
        }
        GUI_SkillCubeManager.Instance.OnHeroDead(hero);
    }

    public void OnMonsterBorn(Actor actor)
    {
        for (int index = 0; index < _HeroInfoList.Count; ++index)
        {
            actor.SkillController.Mixer.RegisterEffectNotify(SKILL.MixEffect.Damage, _HeroInfoList[index].OnHeroDamaged);
        }
        actor.OnDeath += OnMonsterDead;
    }

    public void OnMonsterDead(Actor actor)
    {
        for (int index = 0; index < _HeroInfoList.Count; ++index)
        {
            actor.SkillController.Mixer.RemoveOnEffectNotify(SKILL.MixEffect.Damage, _HeroInfoList[index].OnHeroDamaged);
        }
        actor.OnDeath -= OnMonsterDead;
    }

    public void OnCaptainChange(int battleId)
    {
        int captainIndex = _HeroDisplayOrder[battleId];
        for (int index = 0; index < _HeroInfoList.Count; ++index)
        {
            _HeroInfoList[index].SetCaptainTag(index == captainIndex);
        }
    }

    public void OnProgressEvent(MonsterWaveType waveType, int cur, int total)
    {
        if (waveType == MonsterWaveType.NormalMonster)
        {
            _WaveWarn.Warn(waveType, cur, total);
        }
        else if (waveType == MonsterWaveType.Boss)
        {
            _SpecialWarning.Warning(ESpecialWarningType.Boss);
        }
        else
        {
            _SpecialWarning.Warning(ESpecialWarningType.Hidden);
        }
        MonsterWave.text = string.Format("{0}{1}{2}", cur, "/", total);
    }

    void OnTreasureReach(TreasureFall.TreasureType treasureType, int count)
    {
        switch (treasureType)
        {
            case TreasureFall.TreasureType.Coin:
                {
                    _GoldCoinCount += count;
                    GoldCoinCount.text = _GoldCoinCount.ToString();
                    break;
                }
            case TreasureFall.TreasureType.Chest:
                {
                    _TreaserBoxCount += count;
                    TreasureBoxCount.text = _TreaserBoxCount.ToString();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    void OnEnable()
    {
        SkillGenerator.Instance.OnSkillGenerate += OnSkillGenerate;
        SkillGenerator.Instance.OnCaptainChange += OnCaptainChange;
        BattleManager_DL.Instance.OnWaveProgressChange += OnProgressEvent;
        TreasureFallInterface.OnTreasuseReach += OnTreasureReach;
        DataCenter.PlayerDataCenter.OnPassOver += OnPassOver;
    }

    void OnDisable()
    {
        SkillGenerator.Instance.OnSkillGenerate -= OnSkillGenerate;
        SkillGenerator.Instance.OnCaptainChange -= OnCaptainChange;
        BattleManager_DL.Instance.OnWaveProgressChange -= OnProgressEvent;
        TreasureFallInterface.OnTreasuseReach -= OnTreasureReach;
        DataCenter.PlayerDataCenter.OnPassOver -= OnPassOver;
    }

    void OnPassOver()
    {
        if (DataCenter.PlayerDataCenter.OverLevelData.PassResult == 1)
        {
            if (DataCenter.PlayerDataCenter.OverLevelData.DropHero != null)
            {
                GUI_Manager.Instance.ShowWindowWithName("UI_HeroAward", false);
            }
            else
            {
                _SpecialWarning.Warning(ESpecialWarningType.PassLevel);
            }
        }
        else
        {
            _SpecialWarning.Warning(ESpecialWarningType.FailLevel);
        }
    }

    public void OnPauseButtonClick()
    {
        SkillGenerator.Instance.Stop();
        Time.timeScale = 0f;
        GUI_Manager.Instance.ShowWindowWithName("PauseUI", false);
    }

    void Update()
    {
        if (LastCubeSpawnTime < SkillCubeSpawnIntervel)
        {
            LastCubeSpawnTime += GameTimer.deltaTime;
        }
        CheckSkillCubeQueue();
    }

    List<GUI_SkillCubeItem_DL> _CachedSkill = new List<GUI_SkillCubeItem_DL>();
    public float SkillCubeMoveTime = 0.25f;
    public float SkillCubeSpawnIntervel;
    public float SkillCubeDistance;
    float LastCubeSpawnTime = 0f;
    public void OnSkillGenerate(int heroConfigId, int heroBattleId, int skillId, bool alive)
    {
        GUI_SkillCubeItem_DL sci = GUI_SkillCubeManager.Instance.GetOneFreeSkillCube();
        int index;
        bool specialSkill = false;
        if(_HeroDisplayOrder.TryGetValue(heroBattleId, out index))
        {
            if( _HeroInfoList[index].DisplayHero.SkillServerId == (uint)skillId)
            {
                specialSkill = true;
            }
        }
        sci.SetCubeData(skillId, heroBattleId, specialSkill, _HeroDisplayOrder[heroBattleId], alive);
        _CachedSkill.Add(sci);
        CheckSkillCubeQueue();
    }

    void CheckSkillCubeQueue()
    {
        if (_CachedSkill.Count > 0 && LastCubeSpawnTime > SkillCubeSpawnIntervel)
        {
            LastCubeSpawnTime = 0f;
            GUI_SkillCubeItem_DL sci = _CachedSkill[0];
            _CachedSkill.RemoveAt(0);
            GUI_Tools.CommonTool.AddUIChild(SkillCubeSpawnRoot, sci.gameObject, false);
            sci.CachedTransform.position = SkillCubeSpawnPos.position;
            //sci.CachedTransform.SetSiblingIndex(1);
            GUI_SkillCubeManager.Instance.UseSkillCube(sci);
        }
    }

    public void ShowCubeClickEffect(int heroDisplayOrder, int index)
    {
        GUI_EffectPlayer_DL clickEffect = null;
        if (_CubeClickEffects.TryGetValue(index, out clickEffect))
        {
            clickEffect.Recycle();
            _CubeClickEffects.Remove(index);
        }
        clickEffect = _CubeClickEffectPool[heroDisplayOrder].GetOneLogicComponent() as GUI_EffectPlayer_DL;
        if (null != clickEffect)
        {
            _CubeClickEffects.Add(index, clickEffect);
            GUI_Tools.CommonTool.AddUIChild(_SkillCubePosList[index].gameObject, clickEffect.CachedGameObject, CubeClickTrans);
            clickEffect.Play();
        }
    }

    public void ShowDoubleAlignEffect(GUI_SkillCubeItem_DL sci)
    {
        if (!sci.PlayingDoubleAlignEffect())
        {
            GUI_EffectPlayer_DL doubleEffect = _DoubleCubeAlignPool[sci.HeroDisplayOrder].GetOneLogicComponent() as GUI_EffectPlayer_DL;
            sci.PlayDoubleAlignEffect(doubleEffect, DoubleCubeAlignTrans);
        }
    }

    public void ShowTribleAlignEffect(GUI_SkillCubeItem_DL sci)
    {
        if (!sci.PlayingTribleAlignEffect())
        {
            GUI_EffectPlayer_DL tribleEffect = _TribleCubeAlignPool[sci.HeroDisplayOrder].GetOneLogicComponent() as GUI_EffectPlayer_DL;
            sci.PlayTribleAlignEffect(tribleEffect, TribleCubeAlignTrans);
        }
    }

    public override void PreHideWindow()
    {
        GUI_WarnNumberManager_DL.Instance.ClearWarning();
        GUI_HpController_DL.Instance.ClearHpItem();
        GUI_SkillCubeManager.Instance.OnBattleEnd();
    }

    public void ShowHeadUpWarnTip(Actor actor, CSV_c_skill_description skillDes, CSV_c_skill_cast_warn_pattern warnPattern)
    {
        for (int index = 0; index < SkillHeadUpWarnTipList.Count; ++index)
        {
            if (!SkillHeadUpWarnTipList[index].Warning
                || SkillHeadUpWarnTipList[index].TargetActor.BattleId == actor.BattleId)
            {
                SkillHeadUpWarnTipList[index].ShowHeadUpWarnTip(actor, skillDes, warnPattern);
                break;
            }
        }
    }

    protected override void OnAwake()
    {
        GUI_BattleManager.Instance.BattleUI = this;
        GUI_SkillCubeManager.Instance.InitSkillCubePos(this);

        for (int index = 0; index < 3; ++index)
        {
            InitCubeEffectPool(index);
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_BattleUI dataComponent = gameObject.GetComponent<GUI_BattleUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BattleUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            MonsterWarnObject = dataComponent._WaveWarn;
            SpecialWarnObject = dataComponent._SpecialWarning;
            _SkillScrow = dataComponent._SkillScrow;
            SkillCubeSpawnRoot = dataComponent.SkillCubeSpawnRoot;
            SkillCubeSpawnPos = dataComponent.SkillCubeSpawnPos;
            _SkillCubePosList = dataComponent._SkillCubePosList;
            HeroInfoObjectList = dataComponent._HeroInfoList;
            MonsterWave = dataComponent.MonsterWave;
            TreasureBoxCount = dataComponent.TreasureBoxCount;
            GoldCoinCount = dataComponent.GoldCoinCount;
            ChapterDescription = dataComponent.ChapterDescription;
            ChapterDetailInfo = dataComponent.ChapterDetailInfo;
            ChapterInfo = dataComponent.ChapterInfo;
            ChapterInfoShowTime = dataComponent.ChapterInfoShowTime;
            EnemyHeadHpSpawnRoot = dataComponent.EnemyHeadHpSpawnRoot;
            HeroHeadHpSpawnRoot = dataComponent.HeroHeadHpSpawnRoot;
            SkillHeadUpWarnObjectList = dataComponent.SkillHeadUpWarnTipList;
            CubeClickEffectProt = dataComponent.CubeClickEffectProt;
            DoubleCubeAlignProt = dataComponent.DoubleCubeAlignProt;
            TribleCubeAlignProt = dataComponent.TribleCubeAlignProt;
            CubeClickTrans = dataComponent.CubeClickTrans;
            DoubleCubeAlignTrans = dataComponent.DoubleCubeAlignTrans;
            TribleCubeAlignTrans = dataComponent.TribleCubeAlignTrans;
            BoxImage = dataComponent.BoxImage;
            GoldImage = dataComponent.GoldImage;
            SkillCubeMoveTime = dataComponent.SkillCubeMoveTime;
            SkillCubeSpawnIntervel = dataComponent.SkillCubeSpawnIntervel;
            SkillCubeDistance = dataComponent.SkillCubeDistance;

            dataComponent.PauseButton.onClick.AddListener(OnPauseButtonClick);
        }
    }
}
