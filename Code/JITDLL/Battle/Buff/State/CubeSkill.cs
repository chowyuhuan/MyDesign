
namespace BUFF
{
    /// <summary>
    /// 消块技能
    /// </summary>
    public class CubeSkill : State
    {
        // 技能Id
        private string skillId;

        public CubeSkill(string skillId)
        {
            this.skillId = skillId;
        }

        public override void Enforce(int layer)
        {
            StateBlackboard.CubeSkill = skillId;
        }
    }
}
