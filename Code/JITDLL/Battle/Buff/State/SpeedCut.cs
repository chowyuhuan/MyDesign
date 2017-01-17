
namespace BUFF
{
    /// <summary>
    /// 减速
    /// </summary>
    public class SpeedCut : State
    {
        // 比例
        private float rate;

        public SpeedCut(float rate)
        {
            this.rate = rate;
        }

        public override void Enforce(int layer)
        {
            if (!StateBlackboard.ImmuneSpeedCut)
            {
                StateBlackboard.SpeedCut += rate * layer;
            }
        }
    }
}
