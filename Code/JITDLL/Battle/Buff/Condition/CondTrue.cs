
namespace BUFF
{
    /// <summary>
    /// 真
    /// </summary>
    public class CondTrue : Condition
    {
        public override bool Result()
        {
            return true;
        }

        public override object Clone()
        {
            return new CondTrue();
        }

        public override string Message()
        {
            return true.ToString();
        }
    }
}
