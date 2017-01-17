
namespace BUFF
{
    /// <summary>
    /// 免疫Buff
    /// </summary>
    public class ImmuneBuff : State
    {
        // Buff类型
        private BuffType buffType;

        public ImmuneBuff(BuffType buffType)
        {
            this.buffType = buffType;
        }

        public override void Enforce(int layer)
        {
            StateBlackboard.ImmuneBuffType = (int)buffType;
        }
    }
}
