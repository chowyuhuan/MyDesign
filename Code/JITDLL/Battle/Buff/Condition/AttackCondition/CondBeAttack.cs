
namespace BUFF
{
    /// <summary>
    /// 被攻击条件
    /// </summary>
    public class CondBeAttack : CondBeAttackTarget
    {
        public CondBeAttack() { }

        public CondBeAttack(CondBeAttack cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && AttackType.Damage == subjectAttack.attackType;
        }

        public override object Clone()
        {
            return new CondBeAttack(this);
        }

        public override string Message()
        {
            return "Be Attack " + base.Message();
        }
    }
}
