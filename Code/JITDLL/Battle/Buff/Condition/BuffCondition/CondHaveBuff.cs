
namespace BUFF
{
    /// <summary>
    /// 是否持有Buff条件
    /// </summary>
    public class CondHaveBuff : BuffCondition
    {
        public CondHaveBuff() { }

        public CondHaveBuff(CondHaveBuff cond)
            : base(cond) { }
        
        public override bool Result()
        {
            return Target.HaveBuff(buffType, buffId);
        }

        public override object Clone()
        {
            return new CondHaveBuff(this);
        }

        public override string Message()
        {
            return "Have Buff " + base.Message();
        }
    }
}
