
namespace BUFF
{
    /// <summary>
    /// 分担伤害
    /// </summary>
    public class ShareDamage : State
    {
        // 比例
        private float rate;

        public ShareDamage(float rate)
        {
            this.rate = rate;
        }
        
        public override void Enforce(int layer)
        {
            StateBlackboard.ShareDamage += rate * layer;
        }
    }
}
