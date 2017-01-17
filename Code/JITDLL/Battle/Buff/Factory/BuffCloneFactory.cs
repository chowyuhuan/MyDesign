using SKILL;

namespace BUFF
{
    /// <summary>
    /// 克隆工厂
    /// </summary>
    public class BuffCloneFactory : BuffFactory
    {
        public override Trigger CreateTrigger(NewTriggerMeta meta, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            return null;
        }

        public override Buff CreateBuff(NewBuffMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            return null;
        }

        //public override Buff CreateBuff(BuffType buffType, string buffId)
        //{
        //    return null;
        //}

        public override State CreateState(BuffStateMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            return null;
        }

        public override IBehavior CreateBehavior(TriggerBehaviorMeta meta, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            return null;
        }

        public override Condition CreateCondition(BuffConditionMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            return null;
        }

        public override Subject CreateCubeSubject(ITargetWrapper caster, CubeEraseType cubeEraseType)
        {
            return null;
        }

        public override Subject CreateBuffSubject(ITargetWrapper caster, BuffType type, string id, BuffStatus status)
        {
            return null;
        }

        public override Subject CreateAttackSubject(ITargetWrapper caster, string skillId, AttackStatus statusFlag,
                                                    AttackType attackType = AttackType.None, ITargetWrapper target = null, float value = 0, 
                                                    bool hit = false, bool dodge = false, bool parry = false, bool crit = false)
        {
            return null;
        }
    }
}
