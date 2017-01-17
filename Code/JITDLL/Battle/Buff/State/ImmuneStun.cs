
namespace BUFF
{
    /// <summary>
    /// 免疫眩晕
    /// </summary>
    public class ImmuneStun : State
    {
        public override void Enforce(int layer)
        {
            StateBlackboard.ImmuneStun = true;
        }
    }
}
