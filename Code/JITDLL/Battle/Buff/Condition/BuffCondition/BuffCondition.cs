
namespace BUFF
{
    /// <summary>
    /// Buff条件基类
    /// </summary>
    public abstract class BuffCondition : TargetCondition
    {
        // Buff类型
        protected BuffType buffType;

        // Buff Id
        protected string buffId;

        // Buff层数
        protected int layer;

        public BuffCondition() { }

        public BuffCondition(BuffCondition cond)
        {
            Init(cond.buffType, cond.buffId, cond.layer);
        }

        public void Init(BuffType buffType, string buffId, int layer = 0)
        {
            this.buffType = buffType;
            this.buffId = buffId;
            this.layer = layer;
        }

        public override string Message()
        {
            return base.Message() + " type:" + buffType.ToString() + " id:" + buffId;
        }
    }
}
