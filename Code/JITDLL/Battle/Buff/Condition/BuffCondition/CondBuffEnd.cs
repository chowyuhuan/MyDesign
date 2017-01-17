
namespace BUFF
{
    /// <summary>
    /// Buff结束条件
    /// </summary>
    public class CondBuffEnd : BuffCondition
    {
        public CondBuffEnd() { }

        public CondBuffEnd(CondBuffEnd cond)
            : base(cond) { }

        public override bool Result()
        {
            foreach (Subject subject in GetChangedSubjectList(SubjectType.Buff))
            {
                SubjectBuff subjectBuff = (SubjectBuff)subject;

                if (buffType == subjectBuff.buffType && buffId == subjectBuff.buffId && BuffStatus.End == subjectBuff.statusFlag)
                {
                    return true;
                }
            }

            return false;
        }

        public override object Clone()
        {
            return new CondBuffEnd(this);
        }

        public override string Message()
        {
            return "Buff End " + base.Message();
        }
    }
}
