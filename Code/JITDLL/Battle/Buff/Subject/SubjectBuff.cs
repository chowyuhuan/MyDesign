
namespace BUFF
{
    /// <summary>
    /// Buff状态
    /// </summary>
    public enum BuffStatus
    {
        None,
        Begin,
        End,
        Merge,
        Immune,
    }

    /// <summary>
    /// Buff主题
    /// </summary>
    public class SubjectBuff : Subject
    {
        // Buff类型
        public BuffType buffType;

        // Buff Id
        public string buffId;

        // Buff 状态
        public BuffStatus statusFlag;

        public SubjectBuff(ITargetWrapper caster, BuffType buffType, string buffId, BuffStatus statusFlag)
        {
            this.type = SubjectType.Buff;
            Init(caster, buffType, buffId, statusFlag);
        }

        public void Init(ITargetWrapper caster, BuffType buffType, string buffId, BuffStatus statusFlag)
        {
            this.caster = caster;
            this.buffType = buffType;
            this.buffId = buffId;
            this.statusFlag = statusFlag;
        }

        public override string Message()
        {
            string result = base.Message();

            result += " 类型:" + buffType.ToString();
            result += " ID:" + buffId.ToString();
            result += " 状态:" + statusFlag.ToString();

            return result;
        }
    }
}
