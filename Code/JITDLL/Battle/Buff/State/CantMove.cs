
namespace BUFF
{
    /// <summary>
    /// 不能移动
    /// </summary>
    public class CantMove : State
    {
        public override void Enforce(int layer)
        {
            StateBlackboard.CantMove = true;
        }
    }
}
