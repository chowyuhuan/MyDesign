
namespace BUFF
{
    /// <summary>
    /// 攻击条件基类
    /// </summary>
    public abstract class CondAttackTarget : AttackCondition 
    {
        public CondAttackTarget() { }

        public CondAttackTarget(CondAttackTarget cond)
            : base(cond) { }

        /// <summary>
        /// 判定框架不可被覆盖
        /// </summary>
        /// <returns></returns>
        public sealed override bool Result()
        {
            foreach (Subject subject in GetChangedSubjectList(SubjectType.Attack))
            {
                SubjectAttack subjectAttack = (SubjectAttack)subject;

                if (CheckAttackCondition(subjectAttack) && CheckTargetCondition(subjectAttack.target))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
