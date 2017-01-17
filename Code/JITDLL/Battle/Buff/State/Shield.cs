
namespace BUFF
{
    /// <summary>
    /// 护盾
    /// </summary>
    public class Shield : State
    {
        // 值
        private Value value;

        public Shield(Value value)
        {
            this.value = value;
        }
        
        public override void Enforce(int layer)
        {
            StateBlackboard.Shield += value.GetValue();
        }
    }
}
