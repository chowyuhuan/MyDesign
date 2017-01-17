
namespace BUFF
{
    /// <summary>
    /// 操作符非
    /// </summary>
    public class OpNot : OpUnary
    {
        public OpNot() { }

        public OpNot(Condition cond) : base(cond) { }

        public override bool Result()
        {
            return !cond.Result();
        }

        public override void Reset()
        {
            cond.Reset();
        }

        public override object Clone()
        {
            return new OpNot((Condition)cond.Clone());
        }

        public override string Message()
        {
            return "(Not " + cond.Message() + ")";
        }
    }
}
