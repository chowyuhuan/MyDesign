
namespace BUFF
{
    /// <summary>
    /// 攻击条件
    /// </summary>
    public class CondAttack : CondAttackTarget
    {
        public CondAttack() { }

        public CondAttack(CondAttack cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && AttackType.Damage == subjectAttack.attackType;
        }

        public override object Clone()
        {
            return new CondAttack(this);
        }

        public override string Message()
        {
            return "Attack " + base.Message();
        }
    }
}
