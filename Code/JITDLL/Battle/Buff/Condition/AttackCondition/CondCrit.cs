
namespace BUFF
{
    /// <summary>
    /// 攻击暴击条件
    /// </summary>
    public class CondCrit : CondAttack 
    {
        public CondCrit() { }

        public CondCrit(CondCrit cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.crit;
        }

        public override object Clone()
        {
            return new CondCrit(this);
        }

        public override string Message()
        {
            return "Crit " + base.Message();
        }
    }
}
