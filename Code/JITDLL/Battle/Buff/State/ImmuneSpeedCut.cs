
namespace BUFF
{
    /// <summary>
    /// 免疫减速
    /// </summary>
    public class ImmuneSpeedCut : State
    {
        public override void Enforce(int layer)
        {
            StateBlackboard.ImmuneSpeedCut = true;
        }
    }
}
