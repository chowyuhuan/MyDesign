using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using SKILL;
using System;

/// <summary>
/// 技能释放器
/// 包含打断（取消：游戏内没有取消的需求）
/// 主要负责技能：施放时机、能否施放、动画表现
/// 技能队列放到这里了，外部的SkillQueue可能就可以去掉了
/// 普通攻击呢？
/// </summary>
public class SkillCaster : MonoCompBase
{
    [HideInInspector]
    public bool Casting = false; // 正在施法，外部模块可能会用到

    public Action OnCasterReady; // 可以施法了

    public Action<Skill, int> OnStartCast; // 施法开始
    public Action<Skill> OnCasted; // 施法回调（前摇结束，开始施法，并不是施法结束）
    public Action<Skill> OnFinished; // 施法结束
    public Action<Skill> OnInterrupt; // 施法中断

    /// <summary>
    /// skill and block
    /// </summary>
    struct SAB
    {
        public int SkillID;
        public int Block; // 几消，从1开始：1、2、3对应1、2、3消，0对应普通攻击，4对应特殊技能
        public SAB(int id, int block) { SkillID = id; Block = block; }
    }
    int _skillQueueLowSize;
    int _skillQueueHighSize;
    Queue<SAB> _skillsToCastLow;
    Queue<SAB> _skillsToCastHigh;
    Skill _skill = null;
    Actor[] _goals = null;
    int _sequenceIndex = 0;
    Emitter _emitter = null;

    public SkillCaster()
    {
        _skillQueueLowSize = DefaultConfig.GetInt("SkillQueue1");
        _skillQueueHighSize = DefaultConfig.GetInt("SkillQueue2");
        _skillsToCastLow = new Queue<SAB>(_skillQueueLowSize);
        _skillsToCastHigh = new Queue<SAB>(_skillQueueHighSize);
    }

    /// <summary>
    /// 打断施法
    /// </summary>
    /// <param name="_effect">被什么打断（目前想到的是被技能效果打断）</param>
    /// <returns>打算成功与否</returns>
    public bool Interrupt(Intent effect)
    {
        // 基于_effect的判断逻辑
        return false;

        if (OnInterrupt != null)
        {
            OnInterrupt(_skill);
        }

        // 被打断
        ClearCasting();

        return true;
    }

    /// <summary>
    /// 取消施法
    /// </summary>
    public void Cancel()
    {
        ClearCasting();
    }

    /// <summary>
    /// 重置所有
    /// </summary>
    public void Reset()
    {
        _skillsToCastLow.Clear();
        _skillsToCastHigh.Clear();
        ClearCasting();
    }

    /// <summary>
    /// 施法（非消块触发技能）
    /// </summary>
    /// <param name="skillID">技能ID</param>
    /// <returns>是否成功</returns>
    public bool EnqueueToCast(int skillID)
    {
        // 状态判断，是否有什么状态不允许压入技能队列，比如boss警告阶段
        // return false;
        if(_skillsToCastLow.Count < _skillQueueLowSize)
        {
            _skillsToCastLow.Enqueue(new SAB(skillID, 0));
        }
        return TryToCast();
    }

    /// <summary>
    /// 施法
    /// </summary>
    /// <param name="skillID">技能ID</param>
    /// <param name="block">几消，怪物技能、普通攻击、二次触发技能为0，特殊技能4，正常消块触发为1、2、3</param>
    /// <returns>是否成功</returns>
    public bool EnqueueToCast(int skillID, int block)
    {
        // 状态判断，是否有什么状态不允许压入技能队列，比如boss警告阶段
        // return false;
        if(_skillsToCastLow.Count < _skillQueueLowSize)
        {
            _skillsToCastLow.Enqueue(new SAB(skillID, block));
        }
        return TryToCast();
    }

    /// <summary>
    /// 施法 高优先级
    /// </summary>
    /// <param name="skillID"></param>
    /// <returns></returns>
    public bool EnqueueToCastHigh(int skillID)
    {
        if(_skillsToCastHigh.Count < _skillQueueHighSize)
        {
            _skillsToCastHigh.Enqueue(new SAB(skillID, 0));
        }
        return TryToCast();
    }

    /// <summary>
    /// 技能出队列
    /// </summary>
    public void DequeueSkill()
    {
        if (_skillsToCastHigh.Count > 0)
        {
            _skillsToCastHigh.Dequeue();
        }
        else
        {
            _skillsToCastLow.Dequeue();
        }
    }

    /// <summary>
    /// 是否有技能未释放 
    /// </summary>
    /// <returns></returns>
    public bool HasSkill()
    {
        return _skillsToCastLow.Count > 0 || _skillsToCastHigh.Count > 0;
    }

    /// <summary>
    /// 尝试普通攻击
    /// </summary>
    /// <param name="_skillID">技能ID</param>
    /// <param name="_goals">目标</param>
    /// <returns>是否成功</returns>
    public bool TryToCastNormalAttack(int skillID, Actor[] goals)
    {
        if (!Casting && !HasSkill())
        {
            EnqueueToCast(skillID, 0);
            _goals = goals;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 最终施法接口
    /// </summary>
    public void Cast()
    {
        if (!Casting)
        {
            return;
        }

        if (_sequenceIndex < _skill.Attacks.Length)
        {
            AttackMeta atk = _skill.Attacks[_sequenceIndex++];
            if (atk.MoveCasting)
            {
                Owner.ActorReference.ActorFlyEx.CasterMotion(ActorFly.FlyType.Distance, atk.Speed, 0, atk.Distance);
            }
            _emitter.ExcuteBehaviors(_skill.ID, atk.Behaviors);
            _emitter.Emit(_skill.ID, atk, _skill.Range, _goals);
        }

        if (OnCasted != null)
        {
            OnCasted(_skill);
        }
    }

    /// <summary>
    /// 施法结束
    /// </summary>
    public void CastFinish()
    {
        if (OnFinished != null)
        {
            OnFinished(_skill);
        }

        ClearCasting();
        //TryToCast();
    }

    /// <summary>
    /// 清除正在施法相关信息
    /// </summary>
    void ClearCasting()
    {
        _skill = null;
        _goals = null;
        _sequenceIndex = 0;
        Casting = false;

        Owner.ActorReference.ActorControlEx.RemoveState(typeof(AttackState));
    }

    /// <summary>
    /// 尝试施法
    /// </summary>
    /// <returns>尝试施法成功</returns>
    public bool TryToCast()
    {
        if (Owner.ActorReference.ActorControlEx.CanCastSkill() &&
            TryToCastFromQueue())
        {
            Owner.ActorReference.ActorControlEx.AddState(new AttackState());
            return true;
        }

        return false;
    }

    /// <summary>
    /// 尝试从施放队列中施法
    /// </summary>
    /// <returns>成功取出并尝试施法</returns>
    public bool TryToCastFromQueue()
    {
        if (_skillsToCastHigh.Count > 0)
        {
            return TryToCastSpecific(_skillsToCastHigh.Peek());
        }
        if (_skillsToCastLow.Count > 0)
        {
            return TryToCastSpecific(_skillsToCastLow.Peek());
        }
        return false;
    }

    /// <summary>
    /// 尝试施放指定技能
    /// </summary>
    /// <param name="_skillID">技能ID</param>
    /// <returns>尝试施放成功</returns>
    bool TryToCastSpecific(SAB sab)
    {
        if (!PrepareForCast(sab.SkillID))
        {
            return false;
        }

        DequeueSkill();

        if (OnStartCast != null)
        {
            OnStartCast(_skill, sab.Block);
        }

        if (_skill.CastAnim == "")
        {
            // 没有施法动画时，直接执行所有行为, 不进入AttackState。例如被动技能。
            ExcuteAllBehaviors();
            return false;
        }
        else
        {
            Casting = true;
            // 两种设计思路
            // 1.动画驱动：事件序列契合度完美，但逻辑和渲染耦合，不能做纯逻辑运算，回调应该是消息，效率低一点
            PlayCastAnim();
            // 2.逻辑驱动：逻辑和渲染耦合，能做纯逻辑运算，效率能高一点，但事件序列契合度不高
            return true;
        }
    }

    bool PrepareForCast(int skillID)
    {
        if (Casting)
        {
            return false;
        }

        // 生成Skill结构
        Skill temp;
        if (!SkillDataCenter.Instance.TryToGetSkill(skillID, out temp))
        {
            Debug.LogError(LogTag.SKILL + "尝试释放不存在的技能：" + skillID);
            DequeueSkill();
            return false;
        }

        _skill = temp;
        return true;
    }

    /// <summary>
    /// 播放施法动画
    /// </summary>
    void PlayCastAnim()
    {
        //QueueMode queueMode = _ownerAnimator.IsPlaying("run") ? QueueMode.PlayNow : QueueMode.CompleteOthers;
        //_ownerAnimator.CrossFadeQueued(_skill.CastAnim, 0.2f, QueueMode.PlayNow);
        Owner.ActorReference.ActorAnimEx.Play(_skill.CastAnim);
    }

    /// <summary>
    /// 执行所有攻击阶段的所有行为
    /// </summary>
    public void ExcuteAllBehaviors()
    {
        for (int i = 0; i < _skill.Attacks.Length; i++)
        {
            AttackMeta atk = _skill.Attacks[i];
            _emitter.ExcuteBehaviors(_skill.ID, atk.Behaviors);
        }
    }

    void Start()
    {
        _emitter = new Emitter();
        _emitter.Init(Owner);

        if (OnCasterReady != null)
        {
            OnCasterReady();
        }
    }

    void Update()
    {
        //TryToCast();
        _emitter.Update();
    }
}
