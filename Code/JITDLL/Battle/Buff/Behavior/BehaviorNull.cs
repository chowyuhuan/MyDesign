
namespace BUFF
{
    /// <summary>
    /// 空行为
    /// </summary>
    public class BehaviorNull : Behavior
    {
        public override void ExcuteBehavior() { }

        public override object Clone()
        {
            return new BehaviorNull();
        }
    }
}
