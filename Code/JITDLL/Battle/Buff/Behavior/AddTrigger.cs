using System;

namespace BUFF
{
    /// <summary>
    /// 添加Trigger
    /// </summary>
    public class AddTrigger : Behavior
    {
        // 技能Id
        private string skillId;

        // Trigger索引
        private int triggerIndex;
        
        // 释放者
        private ITargetWrapper caster;

        public AddTrigger(string skillId, int triggerIndex, ITargetWrapper caster = null)
        {
            this.skillId = skillId;
            this.triggerIndex = triggerIndex;
            this.caster = caster;
        }

        public override void ExcuteBehavior()
        {
            var meta = SkillDataCenter.Instance.GetSkillTrigger(Convert.ToInt32(skillId), triggerIndex);
            if (meta != null)
            {
                Trigger trigger = BuffStandardFactory.Instance.CreateTrigger(meta, skillId, caster, Target);
                Target.AddTrigger(trigger);
            }
        }

        public override object Clone()
        {
            return new AddTrigger(skillId, triggerIndex, caster);
        }
    }
}
