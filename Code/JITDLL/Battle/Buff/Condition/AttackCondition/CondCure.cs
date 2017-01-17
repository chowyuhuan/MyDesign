
namespace BUFF
{
    /// <summary>
    /// 治疗条件
    /// </summary>
    public class CondCure : CondAttackTarget
    {
        public CondCure() { }

        public CondCure(CondCure cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && AttackType.Cure == subjectAttack.attackType;
        }

        public override object Clone()
        {
            return new CondCure(this);
        }

        public override string Message()
        {
            return "Cure " + base.Message();
        }
    }
}
