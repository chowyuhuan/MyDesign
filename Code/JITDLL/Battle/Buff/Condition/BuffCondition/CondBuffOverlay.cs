
namespace BUFF
{
    /// <summary>
    /// Buff叠层条件
    /// </summary>
    public class CondBuffOverlay : BuffCondition
    {
        public CondBuffOverlay() { }

        public CondBuffOverlay(CondBuffOverlay cond)
            : base(cond) { }

        public override bool Result()
        {
            Buff buff = Target.FindBuff(buffType, buffId);

            if (buff != null)
            {
                return layer == buff.Layer();
            }

            return false;
        }

        public override object Clone()
        {
            return new CondBuffOverlay(this);
        }

        public override string Message()
        {
            return "Buff Overlay " + base.Message() + " Layer:" + layer;
        }
    }
}
