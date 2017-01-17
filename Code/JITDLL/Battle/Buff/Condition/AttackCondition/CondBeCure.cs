
namespace BUFF
{
    /// <summary>
    /// 被治疗条件
    /// </summary>
    public class CondBeCure : CondBeAttackTarget
    {
        public CondBeCure() { }

        public CondBeCure(CondBeCure cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && AttackType.Cure == subjectAttack.attackType;
        }

        public override object Clone()
        {
            return new CondBeCure(this);
        }

        public override string Message()
        {
            return "Be Cure " + base.Message();
        }
    }
}
