
namespace BUFF
{
    /// <summary>
    /// Buff效果量
    /// </summary>
    public class EffectSize: State
    {
        // 比例
        private float rate;

        public EffectSize(float rate)
        {
            this.rate = rate;
        }
        
        public override void Enforce(int layer)
        {
            StateBlackboard.EffectSize += rate * layer;
        }
    }
}
