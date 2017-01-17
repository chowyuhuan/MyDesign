
namespace BUFF
{
    /// <summary>
    /// 攻击条件基类
    /// </summary>
    public abstract class AttackCondition : TargetCondition
    {
        // 技能Id
        protected string skillId;

        // 带条件的目标
        protected TargetCondition targetCondition;

        public AttackCondition() { }

        public AttackCondition(AttackCondition cond)
        {
            Init(cond.Target, cond.skillId, cond.targetCondition == null ? null : (TargetCondition)targetCondition.Clone());
        }

        public void Init(ITargetWrapper target, string skillId, TargetCondition targetCondition = null)
        {
            this.Target = target;
            this.skillId = skillId;
            this.targetCondition = targetCondition;
        }

        /// <summary>
        /// 抽象模板方法
        /// </summary>
        /// <param name="subjectAttack">攻击主题</param>
        /// <returns></returns>
        protected virtual bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return skillId == "" || skillId == subjectAttack.skillId;
        }

        /// <summary>
        /// 检查带条件的目标
        /// </summary>
        /// <param name="target">带条件的目标</param>
        /// <returns></returns>
        protected bool CheckTargetCondition(ITargetWrapper target)
        {
            if (targetCondition != null)
            {
                targetCondition.Target = target;
                return targetCondition.Result();
            }
            return true;
        }

        public override string Message()
        {
            return base.Message() + " Skill Id:" + skillId + (targetCondition == null ? "" : "Target:" + targetCondition.Message());
        }
    }
}
