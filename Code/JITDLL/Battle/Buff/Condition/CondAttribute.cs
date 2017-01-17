
namespace BUFF
{
    /// <summary>
    /// 属性条件
    /// </summary>
    public class CondAttribute : TargetCondition
    {
        // 属性字段
        private int field;

        // 比较运算符
        private CompareOperation compare;

        // 值
        private Value value;

        public CondAttribute(int field, CompareOperation compare, Value value)
        {
            this.field = field;
            this.compare = compare;
            this.value = value;
        }

        public override bool Result()
        {
            return Condition.Compare(compare, Target.Attribute(field), value.GetValue());
        }

        public override object Clone()
        {
            return new CondAttribute(field, compare, (Value)value.Clone());
        }

        public override string Message()
        {
            return base.Message() + " Attribute";
        }
    }
}
