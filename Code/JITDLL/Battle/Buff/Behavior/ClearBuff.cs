
namespace BUFF
{
    /// <summary>
    /// 清空buff
    /// </summary>
    public class ClearBuff : Behavior
    {
        // Buff类型
        private BuffType buffType;

        // Buff id 为空时清空该类所有buff
        private string buffId;

        public ClearBuff(BuffType buffType, string buffId)
        {
            this.buffType = buffType;
            this.buffId = buffId;
        }

        public override void ExcuteBehavior()
        {
            if (buffId == "")
            {
                Target.ClearBuff(buffType);
            }
            else
            {
                Target.ClearBuff(buffType, buffId);
            }
        }

        public override object Clone()
        {
            return new ClearBuff(buffType, buffId);
        }

        public override string Message()
        {
            return base.Message() + " " + buffType.ToString() + " " + buffId.ToString();
        }
    }
}
