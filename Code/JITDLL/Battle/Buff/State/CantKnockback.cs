
namespace BUFF
{
    /// <summary>
    /// 不可击退
    /// </summary>
    public class CantKnockback : State
    {
        public override void Enforce(int layer)
        {
            StateBlackboard.CantKnockback = true;
        }
    }
}
