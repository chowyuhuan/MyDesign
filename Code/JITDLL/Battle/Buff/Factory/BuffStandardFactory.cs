using System.Collections.Generic;
using ACTOR;
using SKILL;

namespace BUFF
{
    /// <summary>
    /// 常规工厂
    /// </summary>
    public class BuffStandardFactory : BuffFactory
    {
        private static BuffStandardFactory instance;

        public static BuffStandardFactory Instance
        {
            get 
            { 
                if (instance == null)
                {
                    instance = new BuffStandardFactory();
                }
                return instance; 
            }
        }

        private BuffStandardFactory() { }

        public override Trigger CreateTrigger(NewTriggerMeta meta, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            return new Trigger(meta.Tag,
                               new LifeCycle(CreateCondition(meta.LifeCycle.ConditionMeta, caster, hitters)), 
                               CreateCondition(meta.Condition.ConditionMeta, caster, hitters),
                               CreateBehaviorForkList(meta.BehaviorForkList, skillId, caster, hitters));
        }

        public override Buff CreateBuff(NewBuffMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            Buff buff = new Buff(caster, meta.BuffTypeEx, meta.BuffId, meta.Effect,
                                 new LifeCycle(CreateCondition(meta.LifeCycle.ConditionMeta, caster, hitters), meta.Removable),
                                 new Overlay(meta.Limit),
                                 CreateStateList(meta.States, caster, hitters));
            return buff;
        }

        //public override Buff CreateBuff(BuffType buffType, string buffId)
        //{
        //    return null;
        //}

        private State[] CreateStateList(List<BuffStateMeta> metaList, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            State[] stateList = new State[metaList.Count];

            for (int i = 0; i < stateList.Length; i++ )
            {
                stateList[i] = CreateState(metaList[i], caster, hitters);
            }

            return stateList;
        }

        private Value CreateValue(BuffStateMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            ValueType valueType = (ValueType)meta.GetIntValue(StateValueMeta.ValueType);
            float value = meta.GetFloatValue(StateValueMeta.Value);

            if (valueType == ValueType.Number)
            {
                return new Value(valueType, value);
            }
            else
            {
                Camp baseCamp = (Camp)meta.GetIntValue(StateValueMeta.BaseCamp);
                Target baseTarget = (Target)meta.GetIntValue(StateValueMeta.BaseTarget);

                ITargetWrapper baseTargetOne = TargetManager.Instance.ChooseOne(baseCamp, baseTarget, caster, hitters);

                if (baseTargetOne == null)
                {
                    return null;
                }
                else
                {
                    int baseField = meta.GetIntValue(StateValueMeta.BaseField);
                    AttributePoint basePoint = (AttributePoint)meta.GetIntValue(StateValueMeta.BasePoint);
                    return new Value(valueType, value, new ValueDependency(baseField, basePoint, baseTargetOne));
                }
            }
        }

        private State CreateAttributeState(BuffStateMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            int field = meta.GetIntValue(StateAttributeMeta.FieldType);

            Value value = CreateValue(meta, caster, hitters);

            return value == null ? StateNull : new Attribute(field, value);
        }

        private State CreateShieldState(BuffStateMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            Value value = CreateValue(meta, caster, hitters);

            return value == null ? StateNull : new Shield(value);
        }

        public override State CreateState(BuffStateMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            State state = null;

            switch (meta.Type)
            {
                case BuffStateType.Attribute:
                    state = CreateAttributeState(meta, caster, hitters);
                    break;
                case BuffStateType.EffectSize:
                    state = new EffectSize(meta.GetFloatValue(StateRateMeta.Rate));
                    break;
                case BuffStateType.BuffTime:
                    state = new BuffTime((BuffType)meta.GetFloatValue(StateBuffTimeMeta.BuffType), meta.GetFloatValue(StateBuffTimeMeta.Time));
                    break;
                case BuffStateType.CubeSkill:
                    state = new CubeSkill(meta.GetStringValue(StateCubeSkillMeta.SkillId));
                    break;
                case BuffStateType.CubeStrategy:
                    state = new CubeStrategy((CubeStrategyType)meta.GetIntValue(StateCubeStrategyMeta.StrategyType));
                    break;
                case BuffStateType.Shield:
                    state = CreateShieldState(meta, caster, hitters);
                    break;
                case BuffStateType.CureReduced:
                    state = new CureReduced(meta.GetFloatValue(StateRateMeta.Rate));
                    break;
                case BuffStateType.ThornsDamage:
                    state = new ThornsDamage(meta.GetFloatValue(StateRateMeta.Rate));
                    break;
                case BuffStateType.ShareDamage:
                    state = new ShareDamage(meta.GetFloatValue(StateRateMeta.Rate));
                    break;
                case BuffStateType.AdditionalDamage:
                    state = new AdditionalDamage(meta.GetIntValue(StateAdditionalDamageMeta.NatureEx), meta.GetFloatValue(StateAdditionalDamageMeta.Factor));
                    break;
                case BuffStateType.AdditionalDodge:
                    state = new AdditionalDodge(meta.GetFloatValue(StateRateMeta.Rate));
                    break;
                case BuffStateType.CantKnockback:
                    state = new CantKnockback();
                    break;
                case BuffStateType.CantMove:
                    state = new CantMove();
                    break;
                case BuffStateType.ImmuneBuff:
                    state = new ImmuneBuff((BuffType)meta.GetIntValue(StateImmuneBuffMeta.BuffType));
                    break;
                case BuffStateType.Invinible:
                    state = new Invincible();
                    break;
                case BuffStateType.Stun:
                    state = new Stun();
                    break;
                case BuffStateType.ImmuneStun:
                    state = new ImmuneStun();
                    break;
                case BuffStateType.SpeedCut:
                    state = new SpeedCut(meta.GetFloatValue(StateRateMeta.Rate));
                    break;
                case BuffStateType.ImmuneSpeedCut:
                    state = new ImmuneSpeedCut();
                    break;
                default:
                    state = StateNull;
                    break;
            }

            return state;
        }

        private BehaviorFork[] CreateBehaviorForkList(List<TriggerBehaviorForkMeta> metaList, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            BehaviorFork[] behaviorList = new BehaviorFork[metaList.Count];

            for (int i = 0; i < behaviorList.Length; i++ )
            {
                behaviorList[i] = CreateBehaviorFork(metaList[i], skillId, caster, hitters);
            }

            return behaviorList;
        }

        private BehaviorFork CreateBehaviorFork(TriggerBehaviorForkMeta meta, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            BehaviorFork behaviorFork = new BehaviorFork();
            behaviorFork.Weight = meta.Weight;

            foreach (TriggerBehaviorMeta behaviorMeta in meta.BehaviorFork)
            {
                behaviorFork.Add(CreateBehavior(behaviorMeta, skillId, caster, hitters));
            }

            return behaviorFork;
        }

        private IBehavior CreateBehaviorForTargets(Behavior behavior, ITargetWrapper[] targets)
        {
            if (targets.Length == 0)
            {
                return BehaviorNull;
            }
            else if (targets.Length == 1)
            {
                behavior.Target = targets[0];
                return behavior;
            }
            else
            {
                BehaviorFork behaviorFork = new BehaviorFork();

                for (int i = 0; i < targets.Length; i++)
                {
                    Behavior clone = (Behavior)behavior.Clone();
                    clone.Target = targets[i];
                    behaviorFork.Add(clone);
                }

                return behaviorFork;
            }
        }

        public Volume.ATK CreateVolumeAtk(TriggerBehaviorMeta meta)
        {
            Nature natureEx = (Nature)meta.GetIntValue(BehaviorVolumeAtkMeta.NatureEx);
            NumericMeta.Base baseEx = (NumericMeta.Base)meta.GetIntValue(BehaviorVolumeAtkMeta.BaseEx);
            ActorField baseField = (ActorField)meta.GetIntValue(BehaviorVolumeAtkMeta.BaseField);
            Op opEx = (Op)meta.GetIntValue(BehaviorVolumeAtkMeta.OpEx);
            float factor = meta.GetFloatValue(BehaviorVolumeAtkMeta.Factor);

            Volume.ATK atk = new Volume.ATK();
            atk.NatureEx = natureEx;
            atk.Meta = new NumericMeta();
            NumericMeta.ValueUnit valueUnit = new NumericMeta.ValueUnit(baseEx, baseField, opEx, factor);
            atk.Meta.Nums.Add(valueUnit);

            return atk;
        }

        private Behavior CreateChangeConditionBehavior(TriggerBehaviorMeta meta)
        {
            ChangeConditionType changeCondType = (ChangeConditionType)meta.GetIntValue(BehaviorChangeConditionMeta.ChangeCondType);
            BuffConditionType condType = (BuffConditionType)meta.GetIntValue(BehaviorChangeConditionMeta.CondType);

            ChangeCondition behavior = null;

            switch (condType)
            {
                case BuffConditionType.Time:
                    behavior = new ChangeTimeCondition(meta.GetFloatValue(BehaviorChangeConditionMeta.Time));
                    break;
                default:
                    return BehaviorNull;
            }

            switch (changeCondType)
            {
                case ChangeConditionType.BuffLifeCycle:
                    behavior.Init(changeCondType, (BuffType)meta.GetIntValue(BehaviorChangeConditionMeta.BuffType), meta.GetStringValue(BehaviorChangeConditionMeta.BuffId));
                    break;
                case ChangeConditionType.TriggerLifeCycle:
                case ChangeConditionType.TriggerCondition:
                    behavior.Init(changeCondType, meta.GetStringValue(BehaviorChangeConditionMeta.TriggerTag));
                    break;
            }

            return behavior;
        }

        public override IBehavior CreateBehavior(TriggerBehaviorMeta meta, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            ITargetWrapper[] targets = null;

            if (!ChooseTargets(out targets, meta.CampEx, meta.TargetEx, caster, hitters))
            {
                return BehaviorNull;
            }

            Behavior behavior = null;

            switch (meta.Type)
            {
                case TriggerBehaviorType.GenerateCube:
                    behavior = new GenerateCube();
                    break;
                case TriggerBehaviorType.ChangeCubeSkill:
                    behavior = new ChangeCubeSkill(meta.GetStringValue(BehaviorChangeCubeSkillMeta.SkillId));
                    break;
                case TriggerBehaviorType.CastSkill:
                    behavior = new CastSkill(meta.GetStringValue(BehaviorCastSkillMeta.SkillId), (SkillPriority)meta.GetIntValue(BehaviorCastSkillMeta.Priority));
                    break;
                case TriggerBehaviorType.StopSkill:
                    behavior = new StopSkill();
                    break;
                case TriggerBehaviorType.AddTrigger:
                    behavior = new AddTrigger(skillId, meta.GetIntValue(BehaviorAddTriggerMeta.TriggerIndex), caster);
                    break;
                case TriggerBehaviorType.AddBuff:
                    behavior = new AddBuff(skillId, meta.GetIntValue(BehaviorAddBuffMeta.BuffIndex), caster);
                    break;
                case TriggerBehaviorType.ClearBuff:
                    behavior = new ClearBuff((BuffType)meta.GetIntValue(BehaviorClearBuffMeta.BuffType), meta.GetStringValue(BehaviorClearBuffMeta.BuffId));
                    break;
                case TriggerBehaviorType.Damage:
                    behavior = new Damage(skillId, CreateVolumeAtk(meta), caster);
                    break;
                case TriggerBehaviorType.Cure:
                    behavior = new Cure(skillId, CreateVolumeAtk(meta), caster);
                    break;
                case TriggerBehaviorType.Energy:
                    behavior = new Energy(meta.GetIntValue(BehaviorEnergyMeta.Energy));
                    break;
                case TriggerBehaviorType.Dodge:
                    behavior = new Dodge();
                    break;
                case TriggerBehaviorType.Parry:
                    behavior = new Parry();
                    break;
                case TriggerBehaviorType.ChangeCondition:
                    behavior = CreateChangeConditionBehavior(meta);
                    break;
                default:
                    behavior = BehaviorNull;
                    break;
            }

            return CreateBehaviorForTargets(behavior, targets);
        }

        private Condition MergeConditions<T>(params Condition[] conditions)
            where T : OpBinary, new()
        {
            return MergeConditions<T>(new List<Condition>(conditions));
        }

        private Condition MergeConditions<T>(List<Condition> conditions)
            where T : OpBinary, new()
        {
            if (conditions.Count == 0)
            {
                return DefaultCondition;
            }
            else if (conditions.Count == 1)
            {
                return conditions[0];
            }
            else
            {
                T t = new T();
                t.Init(conditions[0], MergeConditions<T>(conditions.GetRange(1, conditions.Count - 1)));
                return t;
            }
        }

        private Condition CreateOpBinary<T>(BuffConditionMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
            where T : OpBinary, new()
        {
            Condition condA = (Condition)CreateCondition(meta.SubCondition[0], caster, hitters);
            Condition condB = (Condition)CreateCondition(meta.SubCondition[1], caster, hitters);

            T t = new T();
            t.Init(condA, condB);

            return MergeConditions<T>(condA, condB);
        }

        private Condition CreateOpUnary<T>(BuffConditionMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
            where T : OpUnary, new()
        {
            Condition cond = (Condition)CreateCondition(meta.SubCondition[0], caster, hitters);

            T t = new T();
            t.Init(cond);

            return t;
        }

        private Condition CreateConditionForTargets<T>(TargetCondition condition, ITargetWrapper[] targets)
            where T : OpBinary, new()
        {
            List<Condition> conditions = new List<Condition>();

            for (int i = 0; i < targets.Length; i++)
            {
                TargetCondition clone = (TargetCondition)condition.Clone();
                clone.Target = targets[i];
                conditions.Add(clone);
            }

            return MergeConditions<T>(conditions);
        }

        private Condition CreateAttackCondition<T>(BuffConditionMeta meta, ITargetWrapper[] targets, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
            where T : AttackCondition, new()
        {
            string skillId = meta.GetStringValue(CondAttackMeta.SkillId);
            bool haveTargetCondition = meta.GetBoolValue(CondAttackMeta.HaveTargetCondition);

            TargetCondition targetCondition = null;
            if (haveTargetCondition)
            {
                targetCondition = (TargetCondition)CreateCondition(meta.SubCondition[0], caster, hitters);
            }

            T t = new T();
            t.Init(null, skillId, targetCondition);

            return CreateConditionForTargets<OpOr>(t, targets);
        }

        private Condition CreateBuffCondition<T>(BuffConditionMeta meta, ITargetWrapper[] targets)
            where T : BuffCondition, new()
        {
            BuffType buffType = (BuffType)meta.GetIntValue(CondHaveBuffMeta.BuffType);
            string buffId = meta.GetStringValue(CondHaveBuffMeta.BuffId);
            int layer = meta.GetIntValue(CondBuffOverlayMeta.Layer);

            T t = new T();
            t.Init(buffType, buffId, layer);

            return CreateConditionForTargets<OpOr>(t, targets);
        }

        private Condition CreateAttributeCondition(BuffConditionMeta meta, ITargetWrapper[] targets, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            int field = meta.GetIntValue(CondAttributeMeta.FieldType);
            CompareOperation compare = (CompareOperation)meta.GetIntValue(CondAttributeMeta.Compare);
            ValueType valueType = (ValueType)meta.GetIntValue(CondAttributeMeta.ValueType);
            float value = meta.GetFloatValue(CondAttributeMeta.Value);

            if (valueType == ValueType.Number)
            {
                CondAttribute cond = new CondAttribute(field, compare, new Value(valueType, value));
                return CreateConditionForTargets<OpOr>(cond, targets);
            }
            else
            {
                Camp baseCamp = (Camp)meta.GetIntValue(CondAttributeMeta.BaseCamp);
                Target baseTarget = (Target)meta.GetIntValue(CondAttributeMeta.BaseTarget);

                ITargetWrapper[] baseTargets = null;

                if (ChooseTargets(out baseTargets, baseCamp, baseTarget, caster, hitters))
                {
                    int baseField = meta.GetIntValue(CondAttributeMeta.BaseField);

                    List<Condition> conditions = new List<Condition>();

                    for (int i = 0; i < baseTargets.Length; i++)
                    {
                        CondAttribute cond = new CondAttribute(field, compare, new Value(valueType, value, new ValueDependency(baseField, AttributePoint.Current, baseTargets[i])));
                        conditions.Add(CreateConditionForTargets<OpOr>(cond, targets));
                    }

                    return MergeConditions<OpOr>(conditions);
                }
                else
                {
                    return DefaultCondition;
                }
            }
        }

        private bool IsConditionTarget(BuffConditionMeta meta)
        {
            return meta.HaveValue(CondTargetMeta.Camp) && meta.HaveValue(CondTargetMeta.Target);
        }

        private bool ChooseTargets(out ITargetWrapper[] targets, Camp camp, Target target, ITargetWrapper caster, params ITargetWrapper[] hitters)
        {
            targets = TargetManager.Instance.Choose(camp, target, caster, hitters);
            return targets.Length != 0;
        }

        public override Condition CreateCondition(BuffConditionMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            ITargetWrapper[] targets = null;

            if (IsConditionTarget(meta) && !ChooseTargets(out targets, (Camp)meta.GetIntValue(CondTargetMeta.Camp), (Target)meta.GetIntValue(CondTargetMeta.Target), caster, hitters))
            {
                return DefaultCondition;
            }

            Condition condition = null;

            switch (meta.Type)
            {
                case BuffConditionType.Cube:
                    CondCube cube = new CondCube((CubeEraseType)meta.GetIntValue(CondCubeMeta.CubeEraseType), meta.GetIntValue(CondCubeMeta.Count));
                    condition = CreateConditionForTargets<OpOr>(cube, targets);
                    break;
                case BuffConditionType.Random:
                    condition = new CondRandom(meta.GetFloatValue(CondRandomMeta.Probability));
                    break;
                case BuffConditionType.Time:
                    condition = new CondTime(meta.GetFloatValue(CondTimeMeta.Time));
                    break;
                case BuffConditionType.Times:
                    condition = new CondTimes(meta.GetIntValue(CondTimesMeta.Times));
                    break;
                case BuffConditionType.Attribute:
                    condition = CreateAttributeCondition(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.Energy:
                    CondEnergy energy = new CondEnergy((CompareOperation)meta.GetIntValue(CondEnergyMeta.Compare), meta.GetIntValue(CondEnergyMeta.Value));
                    condition = CreateConditionForTargets<OpOr>(energy, targets);
                    break;
                case BuffConditionType.AttackBegin:
                    condition = CreateAttackCondition<CondAttackBegin>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.AttackEnd:
                    condition = CreateAttackCondition<CondAttackEnd>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.Attack:
                    condition = CreateAttackCondition<CondAttack>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.Crit:
                    condition = CreateAttackCondition<CondCrit>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.Dodge:
                    condition = CreateAttackCondition<CondDodge>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.Hit:
                    condition = CreateAttackCondition<CondHit>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.Parry:
                    condition = CreateAttackCondition<CondParry>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.Cure:
                    condition = CreateAttackCondition<CondCure>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.BeAttack:
                    condition = CreateAttackCondition<CondBeAttack>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.BeCrit:
                    condition = CreateAttackCondition<CondBeCrit>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.BeDodge:
                    condition = CreateAttackCondition<CondBeDodge>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.BeHit:
                    condition = CreateAttackCondition<CondBeHit>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.BeParry:
                    condition = CreateAttackCondition<CondBeParry>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.BeCure:
                    condition = CreateAttackCondition<CondBeCure>(meta, targets, caster, hitters);
                    break;
                case BuffConditionType.HaveBuff:
                    condition = CreateBuffCondition<CondHaveBuff>(meta, targets);
                    break;
                case BuffConditionType.BuffEnd:
                    condition = CreateBuffCondition<CondBuffEnd>(meta, targets);
                    break;
                case BuffConditionType.BuffOverlay:
                    condition = CreateBuffCondition<CondBuffOverlay>(meta, targets);
                    break;
                case BuffConditionType.False:
                    condition = FalseCondition;
                    break;
                case BuffConditionType.True:
                    condition = TrueCondition;
                    break;
                case BuffConditionType.OpAnd:
                    condition = CreateOpBinary<OpAnd>(meta, caster, hitters);
                    break;
                case BuffConditionType.OpNot:
                    condition = CreateOpUnary<OpNot>(meta, caster, hitters);
                    break;
                case BuffConditionType.OpOr:
                    condition = CreateOpBinary<OpOr>(meta, caster, hitters);
                    break;
                case BuffConditionType.OpTimeInterval:
                    condition = CreateOpUnary<OpTimeInterval>(meta, caster, hitters);
                    OpTimeInterval timeInterval = (OpTimeInterval)condition;
                    timeInterval.Time = meta.GetFloatValue(CondOpTimeIntervalMeta.Time);
                    break;
                case BuffConditionType.OpTrueTimes:
                    condition = CreateOpUnary<OpTrueTimes>(meta, caster, hitters);
                    OpTrueTimes trueTimes = (OpTrueTimes)condition;
                    trueTimes.Times = meta.GetIntValue(CondOpTrueTimesMeta.Times);
                    break;
                case BuffConditionType.OpLoop:
                    condition = CreateOpUnary<OpLoop>(meta, caster, hitters);
                    break;
                default:
                    condition = DefaultCondition;
                    break;
            }

            return condition;
        }

        public override Subject CreateCubeSubject(ITargetWrapper caster, CubeEraseType cubeEraseType)
        {
            return new SubjectCube(caster, cubeEraseType);
        }

        public override Subject CreateBuffSubject(ITargetWrapper caster, BuffType type, string id, BuffStatus status)
        {
            return new SubjectBuff(caster, type, id, status);
        }

        public override Subject CreateAttackSubject(ITargetWrapper caster, string skillId, AttackStatus statusFlag,
                                                    AttackType attackType = AttackType.None, ITargetWrapper target = null, float value = 0, 
                                                    bool hit = false, bool dodge = false, bool parry = false, bool crit = false)
        {
            return new SubjectAttack(caster, skillId, statusFlag, attackType, target, value, hit, dodge, parry, crit);
        }
    }
}
