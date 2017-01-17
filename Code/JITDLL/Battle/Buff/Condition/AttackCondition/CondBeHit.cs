
namespace BUFF
{
    /// <summary>
    /// 被攻击命中条件
    /// </summary>
    public class CondBeHit : CondBeAttack 
    {
        public CondBeHit() { }

        public CondBeHit(CondBeHit cond)
            : base(cond) { }

        protected override bool CheckAttackCondition(SubjectAttack subjectAttack)
        {
            return base.CheckAttackCondition(subjectAttack) && subjectAttack.hit;
        }

        public override object Clone()
        {
            return new CondBeHit(this);
        }

        public override string Message()
        {
            return "Be Hit " + base.Message();
        }
    }
}
