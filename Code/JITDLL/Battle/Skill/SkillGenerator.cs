using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;

public class SkillGenerator : Singleton<SkillGenerator>
{
    // 当前未消除的技能个数 
    int currentCount;

    // 技能产生时间间隔 
    float interval = 1f;

    // 累计的时间 
    float accumilatedTime;

    // 运行中
    bool running;

    // 数据初始化过 
    bool initialized;

    // 开始需要加速的个数 
    int _speedupCount;
    // 产生技能个数
    int _generateCount;

    /// <summary>
    /// 产生技能时的回调，参数：英雄配置Id、英雄战场Id、技能Id、是否活着 
    /// </summary>
    public Action<int, int, int, bool> OnSkillGenerate;

    /// <summary>
    /// 队长更换,参数battleId 
    /// </summary>
    public Action<int> OnCaptainChange;

    class GenerateData
    {
        public int heroConfigId;
        public int heroBattleId;
        public int skillId;
    }

    // 产生队列 
    List<GenerateData> datas = new List<GenerateData>();
    // 当前产生的索引 
    int currentData = 0;

    // 缓存3人的技能数据 
    List<GenerateData> cacheDatas = new List<GenerateData>();

    // 存活成员Id 
    List<int> aliveIds = new List<int>();

    // 队长Id 
    int captainBattleId;

    public void Initialize(List<Actor> actors, uint captainServerId)
    {
        if (actors.Count != 3)
        {
            Debug.LogError("Heros count is not 3 !");
            return;
        }

        accumilatedTime = 0;
        currentCount = 0;

        interval = DefaultConfig.GetFloat("SkillGenerateIntervalFirst");
        _speedupCount = DefaultConfig.GetInt("SkillSpeedupCount");
        _generateCount = 0;

        for (int i = 0; i < actors.Count; ++i)
        {
            if (actors[i].ServerId == captainServerId)
            {
                captainBattleId = actors[i].BattleId;
                break;
            }
        }

        running = true;
        initialized = true;

        CacheGenerateData(actors);
        InitializeGenerateData();
        RandomGenerateData();

        RegisterCallback();
    }

    void RegisterCallback()
    {
        BattleManager_DL.Instance.OnSomeHeroDeath += OnHeroDeath;
        BattleManager_DL.Instance.OnWarning += OnBattleWarning;
        BattleManager_DL.Instance.OnAllMonsterDie += OnAllMonsterDie;
        BattleManager_DL.Instance.OnStageClear += OnStageClear;
        BattleManager_DL.Instance.OnGameOver += OnGameOver;
    }

    void UnregisterCallback()
    {
        BattleManager_DL.Instance.OnSomeHeroDeath -= OnHeroDeath;
        BattleManager_DL.Instance.OnWarning -= OnBattleWarning;
        BattleManager_DL.Instance.OnAllMonsterDie -= OnAllMonsterDie;
        BattleManager_DL.Instance.OnStageClear -= OnStageClear;
        BattleManager_DL.Instance.OnGameOver -= OnGameOver;
    }

    void CacheGenerateData(List<Actor> actors)
    {
        cacheDatas.Clear();
        aliveIds.Clear();

        for (int i = 0; i < actors.Count; ++i)
        {
            GenerateData one = new GenerateData();
            one.heroConfigId = actors[i].ConfigId;
            one.heroBattleId = actors[i].BattleId;
            one.skillId = CSV_b_hero_template.FindData(actors[i].ConfigId).SkillID1;

            cacheDatas.Add(one);

            aliveIds.Add(actors[i].BattleId);
        }
    }

    void InitializeGenerateData()
    {
        datas.Clear();
        currentData = 0;

        // 让队长数据排在头一名 
        if (captainBattleId != cacheDatas[0].heroBattleId)
        {
            GenerateData tmp = null;
            for (int i = 1; i < 3; ++i)
            {
                if (cacheDatas[i].heroBattleId == captainBattleId)
                {
                    tmp = cacheDatas[i];

                    cacheDatas.RemoveAt(i);
                    cacheDatas.Insert(0, tmp);

                    break;
                }
            }
        }

        int actorIndex = 0;
        for (int i = 0; i < 12; ++i)
        {
            if (i < 6)
            {
                actorIndex = 0;
            }
            else if (i < 9)
            {
                actorIndex = 1;
            }
            else
            {
                actorIndex = 2;
            }

            GenerateData one = new GenerateData();
            one.heroConfigId = cacheDatas[actorIndex].heroConfigId;
            one.heroBattleId = cacheDatas[actorIndex].heroBattleId;
            one.skillId = cacheDatas[actorIndex].skillId;

            datas.Add(one);
        }
    }

    void RandomGenerateData()
    {
        currentData = 0;

        int index;
        GenerateData tmp;

        for (int i = 0; i < datas.Count; ++i)
        {
            index = UnityEngine.Random.Range(i, datas.Count);
            tmp = datas[index];
            datas.RemoveAt(index);
            datas.Insert(i, tmp);
        }
    }

    void OnHeroDeath(Actor actor)
    {
        aliveIds.Remove(actor.BattleId);

        // 如果队长挂了，并且有存活人员，则换队长 
        if (captainBattleId == actor.BattleId &&
            aliveIds.Count > 0)
        {
            captainBattleId = aliveIds[0];

            if (OnCaptainChange != null)
            {
                OnCaptainChange(captainBattleId);
            }

            InitializeGenerateData();
            RandomGenerateData();
        }
    }

    public void Continue()
    {
        running = true;
    }

    public void Stop()
    {
        running = false;
    }

    public void Clear()
    {
        cacheDatas.Clear();
        aliveIds.Clear();
        datas.Clear();

        running = false;
        initialized = false;
    }

    public void SkillCast(int heroId, int skillId, int count)
    {
        currentCount -= count;

        //Debug.Log("[技能发生器] 释放:" + count + "消,技能个数:" + currentCount);
    }

    /// <summary>
    /// 技能系(Sp,Buff,Trigger等)生成技能 
    /// </summary>
    /// <param name="hero">英雄</param>
    /// <param name="index">0普通技能 3特殊技能</param>
    public void AppendSkill(Actor hero, int index = 0)
    {
        if (currentCount < 8)
        {
            int skillId = hero.SkillController.SkillPossessorEx.GetPrimitiveSkillID(index);
            if (skillId > 0 && OnSkillGenerate != null)
            {
                OnSkillGenerate(hero.ConfigId, hero.BattleId, skillId, aliveIds.Contains(hero.BattleId));
                currentCount++;

                //Debug.Log("[技能发生器] 生成技能,技能个数:" + currentCount);
            }
        }
    }

    void Generate()
    {
        if (currentData < datas.Count)
        {
        }
        else
        {
            RandomGenerateData();
        }

        if (OnSkillGenerate != null)
        {
            OnSkillGenerate(datas[currentData].heroConfigId, datas[currentData].heroBattleId, datas[currentData].skillId, aliveIds.Contains(datas[currentData].heroBattleId));
            currentCount++;

            //Debug.Log("[技能发生器] 生成技能,技能个数:" + currentCount);
        }

        currentData++;
    }

    void OnBattleWarning(bool warning)
    {
        running = !warning;
    }

    void OnAllMonsterDie()
    {
        running = false;
    }

    void OnStageClear()
    {
        UnregisterCallback();
        Clear();
    }

    void OnGameOver()
    {
        UnregisterCallback();
        Clear();
    }

    public void Tick()
    {
        if (!running || !initialized) return;

        if (currentCount >= 8) return; // 呵呵，暂时写死8个吧 

        accumilatedTime += GameTimer.deltaTime;
        while (accumilatedTime > interval)
        {
            _generateCount++;

            // 前几个块后回复正常速度 
            if (_generateCount >= _speedupCount)
            {
                interval = DefaultConfig.GetFloat("SkillGenerateInterval");
            }

            accumilatedTime -= interval;
            Generate();
        }
    }
}
