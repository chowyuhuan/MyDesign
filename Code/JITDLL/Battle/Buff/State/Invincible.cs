
namespace BUFF
{
    /// <summary>
    /// 无敌
    /// </summary>
    public class Invincible : State
    {
        public override void Enforce(int layer)
        {
            StateBlackboard.Invincible = true;
        }
    }
}
