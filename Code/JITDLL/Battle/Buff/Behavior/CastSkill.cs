using System;

namespace BUFF
{
    /// <summary>
    /// 释放技能
    /// </summary>
    public class CastSkill : Behavior
    {
        // 技能Id
        private string skillId;

        // 释放优先级
        private SkillPriority priority;

        public CastSkill(string skillId, SkillPriority priority)
        {
            this.skillId = skillId;
            this.priority = priority;
        }

        public override void ExcuteBehavior()
        {
            Target.CastSkill(Convert.ToInt32(skillId), priority);
        }

        public override object Clone()
        {
            return new CastSkill(skillId, priority);
        }

        public override string Message()
        {
            return base.Message() + " " + skillId;
        }
    }
}
