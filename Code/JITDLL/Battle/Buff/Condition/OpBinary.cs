using System;

namespace BUFF
{
    /// <summary>
    /// 二元操作符
    /// </summary>
    public abstract class OpBinary : Condition
    {
        protected Condition condA;
        protected Condition condB;

        public OpBinary() { }

        public OpBinary(Condition condA, Condition condB)
        {
            Init(condA, condB);
        }

        public void Init(Condition condA, Condition condB)
        {
            this.condA = condA;
            this.condB = condB;
        }

        public override void Change(Type type, params object[] args)
        {
            condA.Change(type, args);
            condB.Change(type, args);
        }
    }
}
