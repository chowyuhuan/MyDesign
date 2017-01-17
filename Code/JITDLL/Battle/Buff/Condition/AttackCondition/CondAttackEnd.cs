
namespace BUFF
{
    /// <summary>
    /// 攻击结束条件
    /// </summary>
    public class CondAttackEnd : CondAttack 
    {
        public CondAttackEnd() { }

        public CondAttackEnd(CondAttackEnd cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && AttackStatus.End == subjectAttack.statusFlag;
        }

        public override object Clone()
        {
            return new CondAttackEnd(this);
        }

        public override string Message()
        {
            return "Attack End " + base.Message();
        }
    }
}
