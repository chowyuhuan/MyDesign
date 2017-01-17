
namespace BUFF
{
    /// <summary>
    /// 恢复减免
    /// </summary>
    public class CureReduced : State
    {
        // 减少比例
        private float rate;

        public CureReduced(float rate)
        {
            this.rate = rate;
        }

        public override void Enforce(int layer)
        {
            StateBlackboard.CureReduced += rate * layer;
        }
    }
}