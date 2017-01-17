
namespace BUFF
{
    /// <summary>
    /// 攻击命中条件
    /// </summary>
    public class CondHit : CondAttack 
    {
        public CondHit() { }

        public CondHit(CondHit cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.hit;
        }

        public override object Clone()
        {
            return new CondHit(this);
        }

        public override string Message()
        {
            return "Hit " + base.Message();
        }
    }
}
