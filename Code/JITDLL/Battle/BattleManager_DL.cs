using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using DataCenter;

public enum ProgressEvent
{
    NormalMonster,
    HiddenMonster,
    Boss,
}

public enum BattleState
{
    Enter,
    Fighting,
    Warning,
    Success,
    VictoryPose,
    Failed,
}

/// <summary>
/// 要不要把生怪过程抽出来以便将来对应不同关卡？？？ 
/// </summary>
public class BattleManager_DL : MonoBehaviour
{
    private static BattleManager_DL _instance;
    public static BattleManager_DL Instance
    {
        get { return _instance; }
    }

    public Action<MonsterWaveType, int, int> OnWaveProgressChange; // 刷怪波次 

    /// <summary>
    /// 怪死光了
    /// </summary>
    public Action OnAllMonsterDie;
    /// <summary>
    /// 关卡结束  
    /// </summary>
    public Action OnStageClear;
    /// <summary>
    /// 失败了  
    /// </summary>
    public Action OnGameOver;
    /// <summary>
    /// 警告 
    /// </summary>
    public Action<bool> OnWarning;
    /// <summary>
    /// 某个英雄死亡,对单个英雄的监听有时注册反注册有困难,可以用这个 
    /// </summary>
    public Action<Actor> OnSomeHeroDeath;

    public bool InitDone { get; private set; }

    float _heroSpeed;
    float _heroEnterTime;
    float _heroEnterGap;

    List<Actor> _monsters = new List<Actor>();   // 当前波次的怪 

    float _slowTime;
    float _slowTimeScale;

    float _warningTime = 5f; // 警告时间 
    float _distance = 30f;   // 怪出生离己方的距离 
    float _rightBoundOffset = 50f;  // 每次战斗右边界偏移 

    uint _heroCaptainId;
    List<Actor> _heroes = new List<Actor>();     // 英雄 
    float _victoryOffset = 10;                   // 胜利后向前移动距离 
    float _poseGap = 5;                          // 胜利后装x造型间隔  
    // 到达胜利后位置的英雄个数 
    int _reachCount = 0;

    BattleState _battleState = BattleState.Enter;

    BattleDropper _battleDropper = null;

    IBattleProcess _battleProcess = null;

    public bool InWarning
    {
        get;
        private set;
    }

    // 战场左边界 
    public float LeftBound
    {
        get;
        private set;
    }
    // 战场右边界 
    public float RightBound
    {
        get;
        private set;
    }

    void Awake()
    {
        CopyDataFromDataScript();
        _instance = this;
        InitDone = false;

        SetConfig();

        gameObject.AddComponent<ActorManager>();
        gameObject.AddComponent<CameraControl>();
        gameObject.AddComponent<BuffBulletinBoard>();
    }

    void Start()
    {
        GUI_Manager.Instance.ShowWindowWithName("BattleUI", false);
        AudioManager.Instance.PlayMusic(GUI_BattleManager.Instance.SelectedLevel.MusicName);
        StartCoroutine(Initialize());
    }

    void SetConfig()
    {
        _heroSpeed = DefaultConfig.GetFloat("ActorMoveSpeed");
        _heroEnterTime = DefaultConfig.GetFloat("HeroEnterTime");
        _heroEnterGap = DefaultConfig.GetFloat("HeroEnterGap");
        _warningTime = DefaultConfig.GetFloat("WarningTime");
        _distance = DefaultConfig.GetFloat("SpawnMonsterDistance");
        _rightBoundOffset = DefaultConfig.GetFloat("BattleRightBoundOffset");
        _slowTime = DefaultConfig.GetFloat("WinSlowTime");
        _slowTimeScale = DefaultConfig.GetFloat("WinSlowTimeScale");
    }

    /// <summary>
    /// 初始化 
    /// </summary>
    /// <param name="heroIds">英雄服务器Id</param>
    /// <param name="csvId">关卡Id</param>
    /// <param name="hasHidden">是否有隐藏怪</param>
    IEnumerator Initialize()
    {
        yield return null;

        GameObject bound = GameObject.Find("BattleBound");
        if (bound != null)
        {
            bound.AddComponent<BattleBound>();
        }

        Team comradeTeam = new Team();
        comradeTeam.Initialize(SKILL.Camp.Comrade);

        Team enemyTeam = new Team();
        enemyTeam.Initialize(SKILL.Camp.Enemy);

        // 设置Team到ActorManager 
        ActorManager.Instance.Initialize();
        ActorManager.Instance.SetComradeTeam(comradeTeam);
        ActorManager.Instance.SetEnemyTeam(enemyTeam);

        // 我方人员
        List<uint> heroIds = new List<uint>();
        TeamInfo teamInfo = DataCenter.PlayerDataCenter.TeamCollectionData.GetMissionTeam();
        heroIds.AddRange(teamInfo.Members);
        _heroCaptainId = teamInfo.LeaderId;

        LeftBound = float.MinValue;
        RightBound = float.MaxValue;

        // 掉落控制 
        _battleDropper = new BattleDropper();
        //_battleDropper.DummyEnterData(csvId);
        _battleDropper.Initialize();

        // 关卡类型的战斗
        _battleProcess = new MissionProcess();
        _battleProcess.Initialize();

        EnterStageBegin();

        InitDone = true;
    }

    void EnterStageBegin()
    {
        _battleState = BattleState.Enter;

        SpawnHeroes(_heroEnterTime * _heroSpeed);

        Invoke("EnterStageTimeover", _heroEnterTime);
    }

    void EnterStageEnd()
    {
        CameraControl.Instance.BeginFollow();
        SkillGenerator.Instance.Initialize(_heroes, _heroCaptainId);
    }

    void FightingStateEnter()
    {
        _battleState = BattleState.Fighting;

        SpawnMonsters();
    }

    void FightingStageEnd()
    {
        ActorManager.Instance.EnemyTeamClear();

        if (_battleProcess.IsEnemyWaveEnd())
        {
            SuccessStateBegin();
        }
        else
        {
            _battleProcess.GoNextEnemyWave();
            CheckMonsterWave();
        }
    }

    void WarningStateBegin()
    {
        _battleState = BattleState.Warning;

        InWarning = true;

        if (OnWarning != null)
            OnWarning(true);

        // 发呆 
        //HeroesCrossFadeAnimation("fight");
        HeroesAddIdleState();

        Invoke("WarningStateTimeover", _warningTime);
    }

    void WaringStateEnd()
    {
        InWarning = false;

        if (OnWarning != null)
            OnWarning(false);

        //HeroesStopAnimation("fight");
        HeroesRemoveState(typeof(IdleState));
    }

    void SuccessStateBegin()
    {
        _battleState = BattleState.Success;

        //_battleDropper.ShowFinalDropContent();

        AllMonsterDie();
    }

    void SuccessStateEnd()
    {
        VictoryPoseStateBegin();
    }

    void FailedStateBegin()
    {
        Debug.Log("OnGameOver");

        _battleState = BattleState.Failed;

        if (OnGameOver != null)
            OnGameOver();

        Invoke("FailedStateEnd", 2f);
    }

    void FailedStateEnd()
    {
        SendPassOverReq(PbCommon.EPassResult.E_Result_Fail);
    }

    void VictoryPoseStateBegin()
    {
        Debug.Log("OnStageClear");

        _battleState = BattleState.VictoryPose;

        if (OnStageClear != null)
        {
            OnStageClear();
        }

        // 任务追踪 
        DataCenter.PlayerDataCenter.TaskData.CheckTaskTracker(
            PbCommon.ETaskTargetType.E_Target_Pass_Level_Times, 1,
            DataCenter.PlayerDataCenter.EnterLevelData.PassId);

        HeroesRotate();

        // 胜利动作 
        HeroesAddVictoryPoseState();

        Invoke("VictoryPoseStateEnd", 2f);
    }

    void VictoryPoseStateEnd()
    {
        //HeroesRemoveState(typeof(VictoryPoseState));

        SendPassOverReq(PbCommon.EPassResult.E_Result_Pass);
    }

    void EnterStageTimeover()
    {
        EnterStageEnd();

        CheckMonsterWave();
    }

    void WarningStateTimeover()
    {
        WaringStateEnd();

        FightingStateEnter();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleResult">1胜利2失败3暂停退出</param>
    public void SendPassOverReq(PbCommon.EPassResult battleResult)
    {
        gsproto.PassOverReq passOverReq = new gsproto.PassOverReq();

        passOverReq.session_id = DataCenter.PlayerDataCenter.SessionId;
        passOverReq.pass_id = (uint)DataCenter.PlayerDataCenter.EnterLevelData.PassId;
        passOverReq.pass_result = (uint)battleResult;

        _battleDropper.AttachPassOverReq(ref passOverReq, battleResult);

        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, passOverReq);
    }

    void SpawnHeroes(float spawnX)
    {
        List<ActorPrepareInfo> heroPrepareList = _battleProcess.GetComradeList();
        for (int i = 0; i < heroPrepareList.Count; ++i)
        {
            SpawnHero(heroPrepareList[i], -spawnX);
        }

        GoddessPrepareInfo goddessPrepareInfo = _battleProcess.GetComradeGoddess();
        if (goddessPrepareInfo != null)
        {
            ActorManager.Instance.GetTeam(SKILL.Camp.Comrade).InitializeGoddess(goddessPrepareInfo);
        }
    }

    public Actor SpawnHero(ActorPrepareInfo prepareInfo, float x)
    {
        Actor actor = ActorManager.Instance.SpawnHero(prepareInfo, x);
        actor.OnDeath += OnHeroDeath;

        _heroes.Add(actor);
        return actor;
    }

    void OnHeroDeath(Actor actor)
    {
        if (OnSomeHeroDeath != null)
        {
            OnSomeHeroDeath(actor);
        }

        actor.OnDeath -= OnHeroDeath;

        _heroes.Remove(actor);

        if (ActorManager.Instance.TeamDeath(SKILL.Camp.Comrade))
        {
            FailedStateBegin();
        }
    }

    void SpawnMonsters()
    {
        float Frontline = ActorManager.Instance.GetFrontline(SKILL.Camp.Comrade);

        LeftBound = Frontline - CameraControl.Instance.WorldHalfWidth;
        RightBound = Frontline + _rightBoundOffset;

        List<ActorPrepareInfo> spawnInfoList = _battleProcess.GetCurrentEnemyList();
        for (int i = 0; i < spawnInfoList.Count; ++i)
        {
            SpawnOneMonster(spawnInfoList[i], Frontline + _distance);
        }
    }

    void SpawnOneMonster(ActorPrepareInfo prepareInfo, float x)
    {
        Actor actor = ActorManager.Instance.SpawnMonster(prepareInfo, x);
        actor.OnDeath += OnMonsterDeath;

        _monsters.Add(actor);
    }

    void CheckMonsterWave()
    {

        MonsterWaveType waveType = _battleProcess.GetCurrentEnemyWaveType();
        if (waveType == MonsterWaveType.NormalMonster)
        {
            FightingStateEnter();
        }
        else
        {
            WarningStateBegin();
        }

        if (OnWaveProgressChange != null)
        {
            OnWaveProgressChange(waveType, _battleProcess.CurrentEnemyWave(), _battleProcess.TotalEnemyWave());
        }
    }

    void OnReachVictoryDestination(Actor actor)
    {
        actor.ActorReference.ActorControlEx.OnReachDestination -= OnReachVictoryDestination;

        actor.ActorReference.ActorControlEx.AddState(new IdleState());

        _reachCount++;
        if (_reachCount >= _heroes.Count)
        {
            SuccessStateEnd();
        }
    }

    void RecoveryTimeScale()
    {
        Time.timeScale = 1f;
    }

    // 怪死光了 
    void AllMonsterDie()
    {
        Debug.Log("OnAllMonsterDie");

        LeftBound = float.MinValue;

        float toCenter = ActorManager.Instance.GetFrontline(SKILL.Camp.Comrade) + _victoryOffset;
        RightBound = toCenter + CameraControl.Instance.WorldHalfWidth;
        for (int i = 0; i < _heroes.Count; ++i)
        {
            float toX = toCenter + (_heroes.Count - 1) * _poseGap / 2 - i * _poseGap;

            _heroes[i].SkillController.Caster.Reset();
            _heroes[i].ActorReference.ActorControlEx.SetDestination(toX);
            _heroes[i].ActorReference.ActorControlEx.OnReachDestination += OnReachVictoryDestination;
        }


        if (OnAllMonsterDie != null)
            OnAllMonsterDie();

        Time.timeScale = _slowTimeScale;

        Invoke("RecoveryTimeScale", _slowTime * Time.timeScale);
    }

    void OnMonsterDeath(Actor actor)
    {
        actor.OnDeath -= OnMonsterDeath;
        _monsters.Remove(actor);

        _battleDropper.MonsterDropAsset(actor);
        _battleDropper.RecordKillMonster(actor);
        // 任务追踪
        DataCenter.PlayerDataCenter.TaskData.CheckTaskTracker(PbCommon.ETaskTargetType.E_Target_Kill_Monster_Count, 1, actor.ConfigId);

        // 当前波死光光 
        if (_monsters.Count == 0)
        {
            FightingStageEnd();
        }
    }

    void HeroesCrossFadeAnimation(string animationName)
    {
        for (int i = 0; i < _heroes.Count; ++i)
        {
            _heroes[i].ActorReference.ActorAnimEx.Play(animationName);
        }
    }

    void HeroesStopAnimation(string animationName)
    {
        for (int i = 0; i < _heroes.Count; ++i)
        {
            _heroes[i].ActorReference.ActorAnimEx.Stop(animationName);
        }
    }

    void HeroesRotate()
    {
        for (int i = 0; i < _heroes.Count; ++i)
        {
            _heroes[i].transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void HeroesAddIdleState()
    {
        for (int i = 0; i < _heroes.Count; ++i)
        {
            _heroes[i].ActorReference.ActorControlEx.AddState(new IdleState());
        }
    }

    void HeroesAddVictoryPoseState()
    {
        for (int i = 0; i < _heroes.Count; ++i)
        {
            _heroes[i].ActorReference.ActorControlEx.AddState(new VictoryPoseState());
        }
    }

    void HeroesRemoveState(System.Type stateType)
    {
        for (int i = 0; i < _heroes.Count; ++i)
        {
            _heroes[i].ActorReference.ActorControlEx.RemoveState(stateType);
        }
    }

    void Update()
    {
        SkillGenerator.Instance.Tick();
    }
    
    void OnDestroy()
    {
        ActorAudio.ClearSources();
        Yielders.ClearWaitForSeconds();
    }

    protected void CopyDataFromDataScript()
    {
        BattleManager dataComponent = gameObject.GetComponent<BattleManager>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：BattleManager,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
        }
    }
}
