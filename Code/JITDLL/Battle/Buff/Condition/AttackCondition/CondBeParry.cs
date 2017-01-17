
namespace BUFF
{
    /// <summary>
    /// 被攻击格挡条件
    /// </summary>
    public class CondBeParry : CondBeAttack
    {
        public CondBeParry() { }

        public CondBeParry(CondBeParry cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.parry;
        }

        public override object Clone()
        {
            return new CondBeParry(this);
        }

        public override string Message()
        {
            return "Be Parry " + base.Message();
        }
    }
}
