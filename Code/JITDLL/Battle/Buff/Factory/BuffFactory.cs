using SKILL;

namespace BUFF
{
    /// <summary>
    /// 工厂基类
    /// </summary>
    public abstract class BuffFactory
    {
        // 全局条件
        public static Condition TrueCondition = new CondTrue();
        public static Condition FalseCondition = new CondFalse();
        public static Condition DefaultCondition = FalseCondition;

        // 全局状态
        public static State StateNull = new StateNull();

        // 全局行为
        public static Behavior BehaviorNull = new BehaviorNull();

        /// <summary>
        /// 创建Trigger
        /// </summary>
        /// <param name="meta">配置表</param>
        /// <param name="skillId">所属技能Id</param>
        /// <param name="caster">释放者</param>
        /// <param name="hitters">命中的目标</param>
        /// <returns></returns>
        public abstract Trigger CreateTrigger(NewTriggerMeta meta, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters);

        /// <summary>
        /// 创建Buff
        /// </summary>
        /// <param name="meta">配置表</param>
        /// <param name="caster">释放者</param>
        /// <param name="hitters">命中的目标</param>
        /// <returns></returns>
        public abstract Buff CreateBuff(NewBuffMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters);
        
        //public abstract Buff CreateBuff(BuffType buffType, string buffId);

        /// <summary>
        /// 创建Buff状态
        /// </summary>
        /// <param name="meta">配置表</param>
        /// <param name="caster">释放者</param>
        /// <param name="hitters">命中的目标</param>
        /// <returns></returns>
        public abstract State CreateState(BuffStateMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters);

        /// <summary>
        /// 创建Trigger行为
        /// </summary>
        /// <param name="meta">配置表</param>
        /// <param name="skillId">所属技能Id</param>
        /// <param name="caster">释放者</param>
        /// <param name="hitters">命中的目标</param>
        /// <returns></returns>
        public abstract IBehavior CreateBehavior(TriggerBehaviorMeta meta, string skillId, ITargetWrapper caster = null, params ITargetWrapper[] hitters);

        /// <summary>
        /// 创建条件
        /// </summary>
        /// <param name="meta">配置表</param>
        /// <param name="caster">释放者</param>
        /// <param name="hitters">命中的目标</param>
        /// <returns></returns>
        public abstract Condition CreateCondition(BuffConditionMeta meta, ITargetWrapper caster = null, params ITargetWrapper[] hitters);

        /// <summary>
        /// 创建消块主题
        /// </summary>
        /// <param name="caster">释放者</param>
        /// <param name="cubeEraseType">消块类型</param>
        /// <returns></returns>
        public abstract Subject CreateCubeSubject(ITargetWrapper caster, CubeEraseType cubeEraseType);

        /// <summary>
        /// 创建Buff主题
        /// </summary>
        /// <param name="caster">释放者</param>
        /// <param name="type">Buff类型</param>
        /// <param name="id">Buff Id</param>
        /// <param name="status">Buff状态</param>
        /// <returns></returns>
        public abstract Subject CreateBuffSubject(ITargetWrapper caster, BuffType type, string id, BuffStatus status);

        /// <summary>
        /// 创建攻击主题
        /// </summary>
        /// <param name="caster">释放者</param>
        /// <param name="skillId">技能Id</param>
        /// <param name="statusFlag">攻击状态</param>
        /// <param name="attackType">攻击类型</param>
        /// <param name="target">目标</param>
        /// <param name="value">伤害值</param>
        /// <param name="hit">命中</param>
        /// <param name="dodge">闪避</param>
        /// <param name="parry">格挡</param>
        /// <param name="crit">暴击</param>
        /// <returns></returns>
        public abstract Subject CreateAttackSubject(ITargetWrapper caster, string skillId, AttackStatus statusFlag,
                                                    AttackType attackType = AttackType.None, ITargetWrapper target = null, float value = 0,
                                                    bool hit = false, bool dodge = false, bool parry = false, bool crit = false);
    }
}
