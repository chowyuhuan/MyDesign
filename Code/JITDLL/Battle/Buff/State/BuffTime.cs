
namespace BUFF
{
    /// <summary>
    /// Buff持续时间
    /// </summary>
    public class BuffTime : State
    {
        // Buff类型
        private BuffType buffType;

        // 时间
        private float time;

        public BuffTime(BuffType buffType, float time)
        {
            this.buffType = buffType;
            this.time = time;
        }
        
        public override void Enforce(int layer)
        {
            StateBlackboard.BuffTime[(int)buffType] += time * layer;
        }
    }
}
