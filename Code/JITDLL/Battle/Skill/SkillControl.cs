using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using SKILL;
using BUFF;

public class SkillControl : MonoCompBase
{
    #region 引用

    [HideInInspector]
    public SKILL.InputCenter InputEx = null;
    [HideInInspector]
    public SkillPossessor SkillPossessorEx = null;
    [HideInInspector]
    public SkillCaster Caster = null;
    [HideInInspector]
    public SkillMixer Mixer = null;
    [HideInInspector]
    public TriggerManager TriggerManagerEx = null;
    [HideInInspector]
    public BuffManager BuffManagerEx = null;
    [HideInInspector]
    public BuffEffectEmitter BuffEffectEmitter = null;
    [HideInInspector]
    public BuffViewer BuffViewer = null;
    [HideInInspector]
    public BuffStateViewer BuffStateViewer = null;
    [HideInInspector]
    public ActorMonitor ActorMonitorEx = null;
    [HideInInspector]
    public ActorAudio ActorAudioEx; // 目前，角色身上的音效，只负责技能。如果后期有非技能音效需求，将ActorAudio拿到ActorRef中
    #endregion

    public override void Init(Actor a)
    {
        base.Init(a);
        InputEx = new SKILL.InputCenter();
        InputEx.Init(a);
        SkillPossessorEx = new SkillPossessor();
        SkillPossessorEx.Init(a);
        Caster = Owner.gameObject.AddComponent<SkillCaster>();
        Mixer = Owner.gameObject.AddComponent<SkillMixer>();
        Caster.Init(a);
        Mixer.Init(a);
        ActorAudioEx = new ActorAudio();
        ActorAudioEx.Init(a);
        TriggerManagerEx = new TriggerManager();
        BuffManagerEx = new BuffManager(a.BaseFields.Length);
        BuffEffectEmitter = new BuffEffectEmitter();
        BuffEffectEmitter.Init(a);
        ActorMonitorEx = new ActorMonitor(a.ActorTag);

        RegisterCallBack();

#if UNITY_EDITOR
        BuffViewer = Owner.gameObject.AddComponent<BuffViewer>();
        BuffViewer.Init(a);
        BuffStateViewer = Owner.gameObject.AddComponent<BuffStateViewer>();
        BuffStateViewer.Init(a);
#endif
    }

    void OnEnable()
    {
        BattleManager_DL.Instance.OnWarning += OnWarning;
    }

    void OnDisable()
    {
        BattleManager_DL.Instance.OnWarning -= OnWarning;

        UnregisterCallBack();
    }

    void RegisterCallBack()
    {
        Caster.OnCasterReady += OnCasterReady;
        Caster.OnStartCast += OnAttackBegin;
        Caster.OnFinished += OnAttackEnd;
        Caster.OnInterrupt += OnAttackInterrupt;

        Mixer.OnHitDamage += OnAttackHitDamage;
        Mixer.OnHitCure += OnAttackHitCure;

        BuffManagerEx.OnBuffBegin += OnBuffBegin;
        BuffManagerEx.OnBuffEnd += OnBuffEnd;
        BuffManagerEx.OnBuffMerge += OnBuffMerge;
        BuffManagerEx.OnBuffImmune += OnBuffImmune;
    }

    void UnregisterCallBack()
    {
        Caster.OnCasterReady -= OnCasterReady;
        Caster.OnStartCast -= OnAttackBegin;
        Caster.OnFinished -= OnAttackEnd;
        Caster.OnInterrupt -= OnAttackInterrupt;

        Mixer.OnHitDamage -= OnAttackHitDamage;
        Mixer.OnHitCure -= OnAttackHitCure;

        BuffManagerEx.OnBuffBegin -= OnBuffBegin;
        BuffManagerEx.OnBuffEnd -= OnBuffEnd;
        BuffManagerEx.OnBuffMerge -= OnBuffMerge;
        BuffManagerEx.OnBuffImmune -= OnBuffImmune;
    }

    void Update()
    {
        BuffEffectEmitter.Update();
        SkillPossessorEx.Update();
        ActorMonitorEx.Update();
    }

    void LateUpdate()
    {
        // trigger和buff的update一定要在所有的ActorMonitor之后，而不只是自己的ActorMonitor之后，所以放在这里。
        BuffManagerEx.Update();
        TriggerManagerEx.Update();
        UpdateActorStateByBuff();
    }

    void OnWarning(bool warning)
    {
        if(warning)
        {
            Caster.Reset();
            Caster.enabled = false;
        }
        else
        {
            Caster.enabled = true;
        }
    }

    public static void ExcuteSkillBehaviors(int skillId, Actor caster)
    {
        Subject subject = BuffStandardFactory.Instance.CreateAttackSubject(caster, skillId.ToString(), AttackStatus.Begin);
        caster.ActorMonitor().AddSubject(subject);

        Skill skill;
        if (SkillDataCenter.Instance.TryToGetSkill(skillId, out skill))
        {
            for (int i = 0; i < skill.Attacks.Length; i++)
            {
                AttackMeta atk = skill.Attacks[i];

                foreach (TriggerBehaviorMeta meta in atk.Behaviors)
                {
                    IBehavior behavior = BuffStandardFactory.Instance.CreateBehavior(meta, skill.ID.ToString(), caster);
                    behavior.Excute();
                }
            }
        }
    }

    public void ExcuteBehaviors(List<TriggerBehaviorMeta> behaviors, int skillId, params Actor[] hitters)
    {
        foreach (TriggerBehaviorMeta meta in behaviors)
        {
            IBehavior behavior = BuffStandardFactory.Instance.CreateBehavior(meta, skillId.ToString(), Owner, hitters);
            behavior.Excute();
        }
    }

    private void OnCasterReady()
    {
        Owner.CastPassiveSkill();
    }

    private void OnBuffBegin(BUFF.Buff buff)
    {
        ActorMonitorEx.AddSubject(BuffStandardFactory.Instance.CreateBuffSubject(Owner, buff.Type, buff.Id, BuffStatus.Begin));
        BuffEffectEmitter.SpwanEffect(buff);
    }

    private void OnBuffEnd(BUFF.Buff buff)
    {
        ActorMonitorEx.AddSubject(BuffStandardFactory.Instance.CreateBuffSubject(Owner, buff.Type, buff.Id, BuffStatus.End));
        BuffEffectEmitter.DestroyEffect(buff);
    }

    private void OnBuffMerge(BUFF.Buff buff)
    {
        ActorMonitorEx.AddSubject(BuffStandardFactory.Instance.CreateBuffSubject(Owner, buff.Type, buff.Id, BuffStatus.Merge));
    }

    private void OnBuffImmune(BUFF.Buff buff)
    {
        ActorMonitorEx.AddSubject(BuffStandardFactory.Instance.CreateBuffSubject(Owner, buff.Type, buff.Id, BuffStatus.Immune));
    }

    public void EraseCube(int cubeCount)
    {
        Subject subject = BuffStandardFactory.Instance.CreateCubeSubject(Owner, (CubeEraseType)cubeCount);
        ActorMonitorEx.AddSubject(subject);
    }

    private void OnAttackBegin(Skill skill, int block)
    {
        Subject subject = BuffStandardFactory.Instance.CreateAttackSubject(Owner, skill.ID.ToString(), AttackStatus.Begin);
        ActorMonitorEx.AddSubject(subject);
    }

    private void OnAttackEnd(Skill skill)
    {
        if (skill != null)
        {
            Subject subject = BuffStandardFactory.Instance.CreateAttackSubject(Owner, skill.ID.ToString(), AttackStatus.End);
            ActorMonitorEx.AddSubject(subject);
        }
    }

    private void OnAttackInterrupt(Skill skill)
    {
        Subject subject = BuffStandardFactory.Instance.CreateAttackSubject(Owner, skill.ID.ToString(), AttackStatus.Interrupt);
        ActorMonitorEx.AddSubject(subject);
    }

    private void OnAttackHitDamage(int skillId, Actor caster, Actor target, int value, bool hit, bool dodge, bool parry, bool crit)
    {
        Subject subject = BuffStandardFactory.Instance.CreateAttackSubject(caster, skillId.ToString(), AttackStatus.Hit, AttackType.Damage, target, value, hit, dodge, parry, crit);
        caster.ActorMonitor().AddSubject(subject);
    }

    private void OnAttackHitCure(int skillId, Actor caster, Actor target, int value, bool hit, bool dodge, bool parry, bool crit)
    {
        Subject subject = BuffStandardFactory.Instance.CreateAttackSubject(caster, skillId.ToString(), AttackStatus.Hit, AttackType.Cure, target, value, hit, dodge, parry, crit);
        caster.ActorMonitor().AddSubject(subject);
    }

    public string BuffCubeSkill()
    {
        return BuffManagerEx.StateBlackboard.CubeSkill;
    }

    public int BuffCubeStrategy(int cubeCount)
    {
        if (BuffManagerEx.StateBlackboard.CubeStrategy != null)
        {
            return (int)BuffManagerEx.StateBlackboard.CubeStrategy.Convert((CubeEraseType)cubeCount);
        }
        return cubeCount;
    }

    public string BuffCubeStrategyName()
    {
        if (BuffManagerEx.StateBlackboard.CubeStrategy != null)
        {
            return BuffManagerEx.StateBlackboard.CubeStrategy.GetType().Name;
        }
        return "";
    }

    public bool BuffCantMove()
    {
        return BuffManagerEx.StateBlackboard.CantMove;
    }

    public bool BuffCantKnockback()
    {
        return BuffManagerEx.StateBlackboard.CantKnockback;
    }

    public float BuffSpeedCut()
    {
        return BuffManagerEx.StateBlackboard.SpeedCut;
    }

    public bool BuffImmuneSpeedCut()
    {
        return BuffManagerEx.StateBlackboard.ImmuneSpeedCut;
    }

    public bool BuffStun()
    {
        return BuffManagerEx.StateBlackboard.Stun;
    }

    public bool BuffImmuneStun()
    {
        return BuffManagerEx.StateBlackboard.ImmuneStun;
    }

    public int BuffImmuneBuffType()
    {
        return BuffManagerEx.StateBlackboard.ImmuneBuffType;
    }

    public float BuffBuffTime(BuffType buffType)
    {
        return BuffManagerEx.StateBlackboard.BuffTime[(int)buffType];
    }

    public bool BuffInvincible()
    {
        return BuffManagerEx.StateBlackboard.Invincible;
    }

    public float BuffShield()
    {
        return BuffManagerEx.StateBlackboard.Shield;
    }

    public float BuffReduceShield(float value)
    {
        float dValue = 0;
        if (BuffManagerEx.StateBlackboard.Shield >= value)
        {
            BuffManagerEx.StateBlackboard.Shield -= value;
        }
        else
        {
            dValue = value - BuffManagerEx.StateBlackboard.Shield;
            BuffManagerEx.StateBlackboard.Shield = 0;
        }
        return dValue;
    }

    public float BuffCureReduced()
    {
        return BuffManagerEx.StateBlackboard.CureReduced;
    }

    public float BuffThornsDamage()
    {
        return BuffManagerEx.StateBlackboard.ThornsDamage;
    }

    public float BuffShareDamage()
    {
        return BuffManagerEx.StateBlackboard.ShareDamage;
    }

    public float BuffEffectSize()
    {
        return BuffManagerEx.StateBlackboard.EffectSize;
    }

    public float BuffAttribute(int field)
    {
        return BuffManagerEx.StateBlackboard.Attributes[field] * BuffEffectSize();
    }

    public List<StateBlackboard.DamageStruct> BuffAdditionalDamage()
    {
        return BuffManagerEx.StateBlackboard.AdditionalDamage;
    }

    public float BuffAdditionalDodge()
    {
        return BuffManagerEx.StateBlackboard.AdditionalDodge;
    }

    public void UpdateActorStateByBuff()
    {
        ActorControl actorControl = Owner.ActorReference.ActorControlEx;

        if (BuffCantMove())
        {
            actorControl.AddState(new RootState());
        }
        else
        {
            actorControl.RemoveState(typeof(RootState));
        }

        if (BuffCantKnockback())
        {
            actorControl.AddState(new ImmuneKnockbackState());
        }
        else
        {
            actorControl.RemoveState(typeof(ImmuneKnockbackState));
        }

        if (BuffSpeedCut() != 0)
        {
            actorControl.AddState(new SlowDownState(BuffSpeedCut()));
        }
        else
        {
            actorControl.RemoveState(typeof(SlowDownState));
        }

        if (BuffStun())
        {
            actorControl.AddState(new StunState());
        }
        else
        {
            actorControl.RemoveState(typeof(StunState));
        }
    }
}
