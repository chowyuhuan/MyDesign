
namespace BUFF
{
    /// <summary>
    /// 生成方块
    /// </summary>
    public class GenerateCube : Behavior
    {
        public override void ExcuteBehavior()
        {
            Target.GenerateCube();
        }

        public override object Clone()
        {
            return new GenerateCube();
        }
    }
}
