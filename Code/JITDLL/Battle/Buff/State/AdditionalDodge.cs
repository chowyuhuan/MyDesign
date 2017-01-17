
namespace BUFF
{
    /// <summary>
    /// 追加闪避
    /// </summary>
    public class AdditionalDodge : State
    {
        // 几率
        private float rate;

        public AdditionalDodge(float rate)
        {
            this.rate = rate;
        }
        
        public override void Enforce(int layer)
        {
            StateBlackboard.AdditionalDodge += rate * layer;
        }
    }
}
