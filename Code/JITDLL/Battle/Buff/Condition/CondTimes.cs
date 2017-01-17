
namespace BUFF
{
    /// <summary>
    /// 次数条件
    /// </summary>
    public class CondTimes : Condition
    {
        // 次数
        private int times;

        // 计数器
        private int counter;

        public CondTimes(int times)
        {
            this.times = times;

            counter = 0;
        }

        public override bool Result()
        {
            return ++counter == times;
        }

        public override void Reset()
        {
            counter = 0;
        }

        public override object Clone()
        {
            return new CondTimes(times);
        }

        public override string Message()
        {
            return "Times " + counter + "/" + times;
        }
    }
}
