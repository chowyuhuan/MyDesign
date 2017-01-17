
namespace BUFF
{
    /// <summary>
    /// 荆棘伤害
    /// </summary>
    public class ThornsDamage : State
    {
        // 比例
        private float rate;

        public ThornsDamage(float rate)
        {
            this.rate = rate;
        }

        public override void Enforce(int layer)
        {
            StateBlackboard.ThornsDamage += rate * layer;
        }
    }
}
