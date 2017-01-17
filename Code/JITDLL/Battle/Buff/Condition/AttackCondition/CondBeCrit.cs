
namespace BUFF
{
    /// <summary>
    /// 被攻击暴击条件
    /// </summary>
    public class CondBeCrit : CondBeAttack 
    {
        public CondBeCrit() { }

        public CondBeCrit(CondBeCrit cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.crit;
        }

        public override object Clone()
        {
            return new CondBeCrit(this);
        }

        public override string Message()
        {
            return "Be Crit " + base.Message();
        }
    }
}
