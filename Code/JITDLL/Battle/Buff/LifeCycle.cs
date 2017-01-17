
namespace BUFF
{
    /// <summary>
    /// 生命周期
    /// </summary>
    public class LifeCycle
    {
        // 结束条件
        private Condition cond;

        public Condition Cond
        {
            get { return cond; }
        }

        // 可被移除
        private bool removable;

        public bool Removable
        {
            get { return removable; }
        }

        public LifeCycle(Condition cond) : this(cond, false)
        {
        }

        public LifeCycle(Condition cond, bool removable)
        {
            this.cond = cond;
            this.removable = removable;
        }

        public bool Finish()
        {
            return cond.Result();
        }

        public string Detail()
        {
            return cond.Message();
        }
    }
}
