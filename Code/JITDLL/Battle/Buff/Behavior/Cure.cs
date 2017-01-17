
namespace BUFF
{
    /// <summary>
    /// 治疗
    /// </summary>
    public class Cure : Behavior
    {
        // 技能Id
        private string skillId;
        
        // 治疗量
        private object atk;
        
        // 释放者
        private ITargetWrapper caster;

        public Cure(string skillId, object atk, ITargetWrapper caster = null)
        {
            this.skillId = skillId;
            this.atk = atk;
            this.caster = caster;
        }

        public override void ExcuteBehavior()
        {
            if (Target != null && caster != null)
            {
                Target.Cure(skillId, caster, atk);
            }
        }

        public override object Clone()
        {
            return new Cure(skillId, atk, caster);
        }

        public override string Message()
        {
            return base.Message() + "";
        }
    }
}
