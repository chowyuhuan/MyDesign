
namespace BUFF
{
    /// <summary>
    /// 能量SP
    /// </summary>
    public class Energy : Behavior
    {
        // 值
        private int value;

        public Energy(int value)
        {
            this.value = value;
        }
        
        public override void ExcuteBehavior()
        {
            Target.AddEnergy(value);
        }

        public override object Clone()
        {
            return new Energy(value);
        }

        public override string Message()
        {
            return base.Message() + " " + value.ToString();
        }
    }
}
