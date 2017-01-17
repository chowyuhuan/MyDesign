
namespace BUFF
{
    /// <summary>
    /// 追加伤害
    /// </summary>
    public class AdditionalDamage : State
    {
        // 伤害性质
        private int nature;

        // 系数
        private float factor;

        public AdditionalDamage(int nature, float factor)
        {
            this.nature = nature;
            this.factor = factor;
        }
        
        public override void Enforce(int layer)
        {
            StateBlackboard.DamageStruct damage = new StateBlackboard.DamageStruct();
            damage.nature = nature;
            damage.factor = factor * layer;
            StateBlackboard.AdditionalDamage.Add(damage);
        }
    }
}
