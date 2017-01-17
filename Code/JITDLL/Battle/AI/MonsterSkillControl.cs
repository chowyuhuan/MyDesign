using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;


public class MonsterSkillControl : ActorSp
{
    List<MonsterSkillContainer> _skillContainers = new List<MonsterSkillContainer>();
    int _containerIndex;

    // 临时先把hp满值记下来 
    float _maxHp;

    bool _valid = false; // 有效的 
    bool _isStop = true; // 是否停止 
    float _stopEndTime = float.MaxValue;

    MonsterSkillTrigger skillTrigger = new MonsterSkillTrigger();

    int _castSkillId = -1; // 当前释放的技能  

    void OnEnable()
    {
        BattleManager_DL.Instance.OnGameOver += OnGameOver;
    }

    void OnDisable()
    {
        BattleManager_DL.Instance.OnGameOver -= OnGameOver;
    }

    /// <summary>
    ///  初始化并添加技能序列 
    /// </summary>
    public void Initialize(int csvId)
    {
        // Sp技能相关
        _maxHp = Owner.GetValue(ActorField.HP); // 临时先把hp满值记下来,将来可能从Actor取得
        _containerIndex = 0;
        _skillContainers.Clear();

        CSV_b_monster_template monsterCSV = CSV_b_monster_template.FindData(csvId);

        for (int i =0;i < monsterCSV.stateCount; ++i)
        {
            MonsterSkillContainer container = new MonsterSkillContainer();

            container.Initialize(monsterCSV.conditions[i], monsterCSV.skillGroups[i]);

            _skillContainers.Add(container);
        }

        _valid = _skillContainers.Count > 0;

        // 另外的技能触发相关 
        skillTrigger.Initialize(this);
    }

    void RegisterContainerCallback()
    {
        _skillContainers[_containerIndex].OnSpProgressChange += SpProgressChange;
        _skillContainers[_containerIndex].OnSpSkillFilled += SpSkillFilled;
    }

    void UnregisterContainerCallback()
    {
        _skillContainers[_containerIndex].OnSpProgressChange -= SpProgressChange;
        _skillContainers[_containerIndex].OnSpSkillFilled -= SpSkillFilled;
    }

    /// <summary>
    /// 开始 
    /// </summary>
    public void Enter()
    {
        if (_valid)
        {
            _isStop = false;
            _stopEndTime = 0;

            RegisterContainerCallback();

            _skillContainers[_containerIndex].Enter();
        }
    }

    /// <summary>
    /// 停止计时一段时间 
    /// </summary>
    /// <param name="stopTime"></param>
    public void Stop(float stopTime)
    {
        if (_valid)
        {
            _isStop = true;
            _stopEndTime = GameTimer.time + stopTime;
        }
    }

    void OnSkillCasted(Skill skill)
    {
        if (skill.ID == _castSkillId)
        {
            _castSkillId = -1;
            Owner.SkillController.Caster.OnCasted -= OnSkillCasted;

            _skillContainers[_containerIndex].DoNext();
            Stop(0);
        }
    }

    /// <summary>
    /// 减少Sp数值 
    /// </summary>
    /// <param name="amount"></param>
    public override void ReduceSp(int amount)
    {
        if (_valid)
        {
            _skillContainers[_containerIndex].ReduceSp(amount);
        }
    }

    /// <summary>
    /// Sp技能进度 
    /// </summary>
    /// <param name="value"></param>
    public void SpProgressChange(float value)
    {
        this.Value = value;

        if (OnSpProgressChange != null)
            OnSpProgressChange(value);
    }

    /// <summary>
    /// Sp技能填充完毕 
    /// </summary>
    /// <param name="skill"></param>
    public void SpSkillFilled(int skillId)
    {
        _castSkillId = skillId;

        Owner.SkillController.Caster.OnCasted += OnSkillCasted;

        Owner.SkillController.Caster.EnqueueToCast(skillId);

        Stop(float.MaxValue);
    }

    /// <summary>
    /// 触发技能填充完毕 
    /// </summary>
    /// <param name="skill"></param>
    public void TriggerSkillFilled(Skill skill)
    {
        Owner.SkillController.Caster.EnqueueToCast(skill.ID);
    }

    /// <summary>
    /// 根据血量决定是否下一阶段 
    /// </summary>
    void CheckStateTransition()
    {
        if (_skillContainers[_containerIndex].TransitionValue > 0 &&
            Owner.GetValue(ActorField.HP) / _maxHp <= _skillContainers[_containerIndex].TransitionValue)
        {
            Transition();
        }
    }

    void Transition()
    {
        UnregisterContainerCallback();

        _containerIndex++;

        RegisterContainerCallback();

        _skillContainers[_containerIndex].Enter();
    }

	// Update is called once per frame
	void Update () {
        if (Owner.IsDeath) return;

        if (!_valid) return;

        if (!_isStop)
        {
            CheckStateTransition();
        }

        if (_isStop && GameTimer.time > _stopEndTime)
        {
            _isStop = false;
            _stopEndTime = 0;
        }

        if (!_isStop)
        {
            _skillContainers[_containerIndex].Tick();

            skillTrigger.Tick();
        }
	}

    void OnGameOver()
    {
        _valid = false;
    }
}
