using System;

namespace BUFF
{
    /// <summary>
    /// 单元操作符
    /// </summary>
    public abstract class OpUnary : Condition
    {
        protected Condition cond;

        public OpUnary() { }

        public OpUnary(Condition cond)
        {
            Init(cond);
        }

        public void Init(Condition cond)
        {
            this.cond = cond;
        }

        public override void Change(Type type, params object[] args)
        {
            cond.Change(type, args);
        }
    }
}
