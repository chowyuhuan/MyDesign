using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GUI_HpController_DL : MonoBehaviour
{
    GameObject AttachDisplayObject;
    public GUI_EnemyInfo_DL _AttachDisplay;
    GUI_LogicObjectPool _ComradeHpItemPool;
    GUI_LogicObjectPool _EnemyHpItemPool;
    List<GUI_HeadHpAndSp_DL> _MonsterHpList = new List<GUI_HeadHpAndSp_DL>();
    List<GUI_HeadHpAndSp_DL> _HeroHpList = new List<GUI_HeadHpAndSp_DL>();
    float _MonsterHpUpdateIterval = 3f;//Todo:读取配表
    float _MonsterHpUpdateCounter = 0;
    int _CurDisMonsterHpIndex = 0;
    Dictionary<string, GUI_Atlas> _HeadiconAtlas = new Dictionary<string, GUI_Atlas>();
    public static GUI_HpController_DL Instance { get; protected set; }

    void Awake()
    {
        CopyDataFromDataScript();
        Instance = this;
        GUI_BattleManager.Instance.HpController = this;
        GameObject comradeHpProto = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/HeadHP_Hero", true, AssetManage.E_AssetType.UIPrefab);
        GameObject enmeyHpProto = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/HeadHP_Enemy", true, AssetManage.E_AssetType.UIPrefab);
        Init(comradeHpProto, enmeyHpProto);
    }

    void Start()
    {
        _AttachDisplay = AttachDisplayObject.GetComponent<GUI_EnemyInfo_DL>();
        _AttachDisplay.Attach(null);
    }

    void OnEnable()
    {
        ACTOR.ActorManager.Instance.HeroBorn += OnHeroBorn;
        ACTOR.ActorManager.Instance.MonsterBorn += OnMonsterBorn;
    }

    void OnDisable()
    {
        ACTOR.ActorManager.Instance.HeroBorn -= OnHeroBorn;
        ACTOR.ActorManager.Instance.MonsterBorn -= OnMonsterBorn;
    }

    public void OnHeroBorn(Actor actor)
    {
        GUI_HeadHpAndSp_DL hhas = GetOneHeadHp(actor.SelfCamp);
        DataCenter.Hero herodata = DataCenter.PlayerDataCenter.GetHero(actor.ServerId);
        CSV_b_hero_template hero = CSV_b_hero_template.FindData((int)herodata.ServerId);
        hhas.Display(actor, (int)actor.GetValue(ACTOR.ActorField.HP), actor.ActorStar, actor.ActorLevel, actor.ActorName, true, 0f, 0f, null);
        GUI_BattleManager.Instance.BattleUI.DisplayHeroInfo(actor);
        _HeroHpList.Add(hhas);
        CheckHUD();
    }

    GUI_HeadHpAndSp_DL GetOneHeadHp(SKILL.Camp camp)
    {
        GUI_HeadHpAndSp_DL hhsp;
        if (camp == SKILL.Camp.Comrade)
        {
            hhsp = _ComradeHpItemPool.GetOneLogicComponent() as GUI_HeadHpAndSp_DL;
        }
        else
        {
            hhsp = _EnemyHpItemPool.GetOneLogicComponent() as GUI_HeadHpAndSp_DL;
        }
        return hhsp;
    }

    public void OnMonsterBorn(Actor actor)
    {
        GUI_HeadHpAndSp_DL hhas = DisplayMonsterHpItem(actor, actor.ConfigId);
        GUI_BattleManager.Instance.BattleUI.OnMonsterBorn(actor);
        CheckHUD();
    }

    public void Init(GameObject comradeHpItemProto, GameObject enemyHpItemProto)
    {
        _ComradeHpItemPool = new GUI_LogicObjectPool(comradeHpItemProto);
        _EnemyHpItemPool = new GUI_LogicObjectPool(enemyHpItemProto);
    }

    public GUI_HeadHpAndSp_DL DisplayMonsterHpItem(Actor target, int monsterId)
    {
#if UNITY_EDITOR
        Debug.Assert(null != target);
#endif
        GUI_HeadHpAndSp_DL hhas = GetOneHeadHp(target.SelfCamp);
        CSV_b_monster_template mt = CSV_b_monster_template.FindData(monsterId);
        GUI_Atlas uiatlas;
        if (!_HeadiconAtlas.TryGetValue(mt.HeadIconAtlas, out uiatlas))
        {
            uiatlas = AssetManage.AM_Manager.LoadAssetSync<GUI_Atlas>("GUI/UIAtlas/" + mt.HeadIconAtlas, true, AssetManage.E_AssetType.GUIAtlas);
            _HeadiconAtlas.Add(mt.HeadIconAtlas, uiatlas);
        }
#if UNITY_EDITOR
        Debug.Assert(null != uiatlas);
#endif
        Sprite headIcon = uiatlas.GetSprite(mt.HeadIcon);
        hhas.Display(target, (int)target.GetValue(ACTOR.ActorField.HP), target.ActorStar, target.ActorLevel, target.ActorName, false, mt.HpSpDistance, mt.HpSpOverlayDistance, headIcon);
        _MonsterHpList.Add(hhas);
        RefreshDisplayInfo();
        return hhas;
    }

    public void RecycleHpItem(GUI_HeadHpAndSp_DL hpItem, bool comrade)
    {
        if (null != hpItem)
        {
            if (comrade)
            {
                _HeroHpList.Remove(hpItem);
            }
            else
            {
                _MonsterHpList.Remove(hpItem);
                if (_MonsterHpList.Count == 0)
                {
                    _AttachDisplay.Attach(null);
                }
            }
            CheckHUD();
        }
    }

    public void ClearHpItem()
    {
        _ComradeHpItemPool.RecycleAll();
        _EnemyHpItemPool.RecycleAll();
        _MonsterHpList.Clear();
    }

    void RefreshDisplayInfo()
    {
        if (_MonsterHpList.Count > 0)
        {
            ++_CurDisMonsterHpIndex;
            if (_CurDisMonsterHpIndex >= _MonsterHpList.Count)
            {
                _CurDisMonsterHpIndex = 0;
            }
            _AttachDisplay.Attach(_MonsterHpList[_CurDisMonsterHpIndex]);
        }
        else
        {
            _AttachDisplay.Attach(null);
        }
    }

    void Update()
    {
        _MonsterHpUpdateCounter += GameTimer.deltaTime;
        if (_MonsterHpUpdateCounter > _MonsterHpUpdateIterval)
        {
            _MonsterHpUpdateCounter = 0f;
            RefreshDisplayInfo();
        }
    }

    public int GetUpDistance(int endindex, bool comorade)
    {
        if (comorade)
        {
            return GetUpDistance(_HeroHpList, endindex);
        }
        else
        {
            return GetUpDistance(_MonsterHpList, endindex);
        }
    }

    int GetUpDistance(List<GUI_HeadHpAndSp_DL> hudList, int startindex)
    {
        int upDistance = 0;
        if (startindex < hudList.Count)
        {
            GUI_HeadHpAndSp_DL targetHpSp = hudList[startindex];
            for (int index = startindex + 1; index < hudList.Count; ++index)
            {
                if (targetHpSp.Overlap(hudList[index]))
                {
                    float distance = Mathf.Abs(hudList[index]._TargetActor.transform.localPosition.x - hudList[startindex]._TargetActor.transform.localPosition.x);
                    if (distance < ((hudList[index]._TargetActor.ActorReference.ActorPreDef.Width + hudList[startindex]._TargetActor.ActorReference.ActorPreDef.Width) / 2))
                    {
                        ++upDistance;
                    }
                }
            }
        }
        return upDistance;
    }

    void CheckHUD()
    {
        CheckHUD(_HeroHpList);
        CheckHUD(_MonsterHpList);
    }

    void CheckHUD(List<GUI_HeadHpAndSp_DL> hudList)
    {
        GUI_HeadHpAndSp_DL.SortHpList(hudList);
    }
    protected void CopyDataFromDataScript()
    {
        GUI_HpController dataComponent = gameObject.GetComponent<GUI_HpController>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HpController,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            AttachDisplayObject = dataComponent._AttachDisplay;
        }
    }
}
