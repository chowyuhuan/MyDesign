
namespace BUFF
{
    /// <summary>
    /// 改变消块技能
    /// </summary>
    public class ChangeCubeSkill : Behavior
    {
        // 技能Id
        private string skillId;

        public ChangeCubeSkill(string skillId)
        {
            this.skillId = skillId;
        }

        public override void ExcuteBehavior()
        {

        }

        public override object Clone()
        {
            return new ChangeCubeSkill(skillId);
        }

        public override string Message()
        {
            return base.Message() + " " + skillId;
        }
    }
}
