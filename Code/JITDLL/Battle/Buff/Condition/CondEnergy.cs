
namespace BUFF
{
    /// <summary>
    /// 能量SP条件
    /// </summary>
    public class CondEnergy : TargetCondition
    {
        // 比较运算符
        private CompareOperation compare;

        // 值
        private int value;

        public CondEnergy(CompareOperation compare, int value)
        {
            this.compare = compare;
            this.value = value;
        }

        public override bool Result()
        {
            return Condition.Compare(compare, Target.GetEnergy(), value);
        }

        public override object Clone()
        {
            return new CondEnergy(compare, value);
        }

        public override string Message()
        {
            return base.Message() + " Energy";
        }
    }
}
