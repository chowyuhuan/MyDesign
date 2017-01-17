
namespace BUFF
{
    /// <summary>
    /// 伤害
    /// </summary>
    public class Damage : Behavior
    {
        // 技能Id
        private string skillId;

        // 伤害量
        private object atk;
        
        // 释放者
        private ITargetWrapper caster;
        
        public Damage(string skillId, object atk, ITargetWrapper caster = null)
        {
            this.skillId = skillId;
            this.atk = atk;
            this.caster = caster;
        }

        public override void ExcuteBehavior()
        {
            if (Target != null && caster != null)
            {
                Target.Damage(skillId, caster, atk);
            }
        }

        public override object Clone()
        {
            return new Damage(skillId, atk, caster);
        }

        public override string Message()
        {
            return base.Message() + "";
        }
    }
}
