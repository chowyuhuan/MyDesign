
namespace BUFF
{
    /// <summary>
    /// 攻击闪避条件
    /// </summary>
    public class CondDodge : CondAttack
    {
        public CondDodge() { }

        public CondDodge(CondDodge cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.dodge;
        }

        public override object Clone()
        {
            return new CondDodge(this);
        }

        public override string Message()
        {
            return "Dodge " + base.Message();
        }
    }
}
