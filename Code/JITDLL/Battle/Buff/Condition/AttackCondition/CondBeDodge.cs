
namespace BUFF
{
    /// <summary>
    /// 被攻击闪避条件
    /// </summary>
    public class CondBeDodge : CondBeAttack
    {
        public CondBeDodge() { }

        public CondBeDodge(CondBeDodge cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.dodge;
        }

        public override object Clone()
        {
            return new CondBeDodge(this);
        }

        public override string Message()
        {
            return "Be Dodge " + base.Message();
        }
    }
}
