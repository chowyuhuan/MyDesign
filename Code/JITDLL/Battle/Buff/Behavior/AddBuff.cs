using System;

namespace BUFF
{
    /// <summary>
    /// 添加Buff
    /// </summary>
    public class AddBuff : Behavior
    {
        // 技能Id
        private string skillId;
        
        // Buff索引
        private int buffIndex;

        // 释放者
        private ITargetWrapper caster;

        public AddBuff(string skillId, int buffIndex, ITargetWrapper caster = null)
        {
            this.skillId = skillId;
            this.buffIndex = buffIndex;
            this.caster = caster;
        }

        public override void ExcuteBehavior()
        {
            var meta = SkillDataCenter.Instance.GetSkillBuff(Convert.ToInt32(skillId), buffIndex);
            if (meta != null)
            {
                Buff buff = BuffStandardFactory.Instance.CreateBuff(meta, caster, Target);
                Target.AddBuff(buff);
            }
        }

        public override object Clone()
        {
            return new AddBuff(skillId, buffIndex, caster);
        }
    }
}
