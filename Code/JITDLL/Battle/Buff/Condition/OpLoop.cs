
namespace BUFF
{
    /// <summary>
    /// 循环 每次达成后重置条件
    /// </summary>
    public class OpLoop : OpUnary
    {
        public OpLoop() { }

        public OpLoop(Condition cond) : base(cond) { }

        public override bool Result()
        {
            if (cond.Result())
            {
                cond.Reset();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object Clone()
        {
            return new OpLoop((Condition)cond.Clone());
        }

        public override string Message()
        {
            return "(Loop " + cond.Message() + ")";
        }
    }
}
