using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using SKILL;

public class NormalAttackCheckState : ActorState
{
    public Actor NormalAttackTarget = null;

    List<int> _normalAttackIDs = new List<int>();   // 普通攻击技能Id 
    int _targetType = 1;                 // 目标选择类型 
    float _normalAttackInterval = 2f;    // 普通攻击间隔时间 
    float _bestNormalAttackRange = 3;    // 最佳普通攻击距离 
    float _maxNormalAttackRange = 9;     // 最大普通攻击距离(需要打后排会用在这个范围内的敌人) 
    float _nextNormalAttackTime;         // 下次可以普通攻击的时间 

    List<Skill> _normalSkills = new List<Skill>();

    int _normalRangeId;

    public void SetNormalRangeId(int normalRangeId)
    {
        _normalRangeId = normalRangeId;
    }

    public override void EnterState()
    {
        CSV_c_normal_range normalRangeCSV = CSV_c_normal_range.FindData(_normalRangeId);

        _normalAttackIDs.AddRange(Owner.actorPrepareInfo.NormalAttackIDs);

        _targetType = normalRangeCSV.target;
        _normalAttackInterval = normalRangeCSV.interval;
        _bestNormalAttackRange = normalRangeCSV.bestRange;
        _maxNormalAttackRange = normalRangeCSV.maxRange;

        for (int i = 0; i < _normalAttackIDs.Count; ++i)
        {
            Skill skill = null;
            SkillDataCenter.Instance.TryToGetSkill(_normalAttackIDs[i], out skill);
            _normalSkills.Add(skill);
        }
    }

    public override void UpdateState()
    {
        Actor[] targets = null;
        NormalAttackTarget = null;

        targets = ActorManager.Instance.Choose(Owner.SelfCamp == Camp.Comrade ? Camp.Enemy : Camp.Comrade, (Target)_targetType, Owner.ActorReference.ActorMovementEx.RelativeForwordX(_normalSkills[0].Range), Owner.ActorReference.ActorMovementEx.RelativeForwordX(_maxNormalAttackRange));
        if (targets != null)
        {
            NormalAttackTarget = targets[0];

            if (Owner.ActorReference.ActorControlEx.CanNormalAttack() &&
                GameTimer.time >= _nextNormalAttackTime)
            {
                if (Owner.SkillController.Caster.TryToCastNormalAttack(_normalAttackIDs[UnityEngine.Random.Range(0, _normalAttackIDs.Count)], targets))
                {
                    _nextNormalAttackTime = GameTimer.time + _normalAttackInterval;
                }
            }
        }
    }
}
