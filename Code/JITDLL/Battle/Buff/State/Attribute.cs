
namespace BUFF
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Attribute : State
    {
        // 属性字段
        private int field;

        //值
        private Value value;

        public Attribute(int field, Value value)
        {
            this.field = field;
            this.value = value;
        }

        public override void Enforce(int layer)
        {
            StateBlackboard.Attributes[field] += value.GetValue() * layer;
        }
    }
}
