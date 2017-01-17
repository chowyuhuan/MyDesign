
namespace BUFF
{
    /// <summary>
    /// 达成次数
    /// </summary>
    public class OpTrueTimes : OpUnary
    {
        private int times;

        public int Times
        {
            get { return times; }
            set { times = value; counter = 0; }
        }

        private int counter;

        public OpTrueTimes() { }

        public OpTrueTimes(Condition cond, int times) : base(cond)
        {
            this.Times = times;
        }

        public override bool Result()
        {
            if (cond.Result())
            {
                counter++;
            }

            return counter == times;
        }

        public override void Reset()
        {
            counter = 0;
        }

        public override object Clone()
        {
            return new OpTrueTimes((Condition)cond.Clone(), times);
        }

        public override string Message()
        {
            return "(TrueTimes " + counter + "/" + times + " " + cond.Message() + ")";
        }
    }
}
