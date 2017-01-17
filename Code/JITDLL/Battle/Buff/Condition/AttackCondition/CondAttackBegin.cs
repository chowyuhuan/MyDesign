
namespace BUFF
{
    /// <summary>
    /// 攻击开始条件
    /// </summary>
    public class CondAttackBegin : CondAttack 
    {
        public CondAttackBegin() { }

        public CondAttackBegin(CondAttackBegin cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && AttackStatus.Begin == subjectAttack.statusFlag;
        }

        public override object Clone()
        {
            return new CondAttackBegin(this);
        }

        public override string Message()
        {
            return "Attack Begin " + base.Message();
        }
    }
}
