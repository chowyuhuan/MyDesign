
namespace BUFF
{
    /// <summary>
    /// 眩晕
    /// </summary>
    public class Stun : State
    {
        public override void Enforce(int layer)
        {
            if (!StateBlackboard.ImmuneStun)
            {
                StateBlackboard.Stun = true;
            }
        }
    }
}
