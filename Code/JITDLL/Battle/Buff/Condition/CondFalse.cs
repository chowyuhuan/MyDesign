
namespace BUFF
{
    /// <summary>
    /// 假
    /// </summary>
    public class CondFalse : Condition
    {
        public override bool Result()
        {
            return false;
        }

        public override object Clone()
        {
            return new CondFalse();
        }

        public override string Message()
        {
            return false.ToString();
        }
    }
}
