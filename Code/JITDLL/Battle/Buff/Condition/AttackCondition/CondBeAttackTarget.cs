using Assets.Scripts.Observer;

namespace BUFF
{
    /// <summary>
    /// 被攻击条件基类
    /// </summary>
    public abstract class CondBeAttackTarget : AttackCondition
    {
        public CondBeAttackTarget() { }

        public CondBeAttackTarget(CondBeAttackTarget cond)
            : base(cond) { }

        /// <summary>
        /// 判定框架不可被覆盖
        /// </summary>
        /// <returns></returns>
        public sealed override bool Result()
        {
            foreach (Observable observable in ActorMonitor.BulletinBoard.GetNotifiedObservableMap().Values)
            {
                ActorMonitor actorMonitor = (ActorMonitor)observable;

                foreach (Subject subject in actorMonitor.ChangedSubjectMap[SubjectType.Attack])
                {
                    SubjectAttack subjectAttack = (SubjectAttack)subject;

                    if (CheckAttackCondition(subjectAttack) && Target.Equals(subjectAttack.target) && CheckTargetCondition(subjectAttack.caster))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
