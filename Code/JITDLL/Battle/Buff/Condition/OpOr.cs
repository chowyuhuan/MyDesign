
namespace BUFF
{
    /// <summary>
    /// 操作符或
    /// </summary>
    public class OpOr : OpBinary
    {
        public OpOr() { }

        public OpOr(Condition condA, Condition condB) : base(condA, condB) { }

        public override bool Result()
        {
            return condA.Result() || condB.Result();
        }

        public override void Reset()
        {
            condA.Reset();
            condB.Reset();
        }

        public override object Clone()
        {
            return new OpOr((Condition)condA.Clone(), (Condition)condB.Clone());
        }

        public override string Message()
        {
            return "(" + condA.Message() + " Or " + condB.Message() + ")";
        }
    }
}
