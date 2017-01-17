
namespace BUFF
{
    /// <summary>
    /// 操作符与
    /// </summary>
    public class OpAnd : OpBinary
    {
        public OpAnd() { }

        public OpAnd(Condition condA, Condition condB) : base(condA, condB) { }

        public override bool Result()
        {
            return condA.Result() && condB.Result();
        }

        public override void Reset()
        {
            condA.Reset();
            condB.Reset();
        }

        public override object Clone()
        {
            return new OpAnd((Condition)condA.Clone(), (Condition)condB.Clone());
        }

        public override string Message()
        {
            return "(" + condA.Message() + " And " + condB.Message() + ")";
        }
    }
}
