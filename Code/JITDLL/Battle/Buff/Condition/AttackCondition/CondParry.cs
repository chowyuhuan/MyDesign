
namespace BUFF
{
    /// <summary>
    /// 攻击格挡条件
    /// </summary>
    public class CondParry : CondAttack
    {
        public CondParry() { }

        public CondParry(CondParry cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.parry;
        }

        public override object Clone()
        {
            return new CondParry(this);
        }

        public override string Message()
        {
            return "Parry " + base.Message();
        }
    }
}
