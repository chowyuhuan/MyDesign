
namespace BUFF
{
    /// <summary>
    /// 终止正在释放的技能
    /// </summary>
    public class StopSkill : Behavior
    {
        public override void ExcuteBehavior()
        {
            Target.StopSkill();
        }

        public override object Clone()
        {
            return new StopSkill();
        }
    }
}
