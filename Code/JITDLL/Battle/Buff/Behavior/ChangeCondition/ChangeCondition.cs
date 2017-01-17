
namespace BUFF
{
    /// <summary>
    /// 动态修改条件基类
    /// </summary>
    public abstract class ChangeCondition : Behavior
    {
        // 修改条件类型
        private ChangeConditionType changeType;

        // 要修改的buff
        private BuffType buffType;

        private string buffId;

        // 要修改的Trigger
        private string triggerTag;

        public void Init(ChangeCondition changeCondition)
        {
            this.changeType = changeCondition.changeType;
            this.buffType = changeCondition.buffType;
            this.buffId = changeCondition.buffId;
            this.triggerTag = changeCondition.triggerTag;
        }

        public void Init(ChangeConditionType changeType, BuffType buffType, string buffId)
        {
            this.changeType = changeType;
            this.buffType = buffType;
            this.buffId = buffId;
        }

        public void Init(ChangeConditionType changeType, string triggerTag)
        {
            this.changeType = changeType;
            this.triggerTag = triggerTag;
        }

        /// <summary>
        /// 查找要修改的条件
        /// </summary>
        /// <returns></returns>
        public Condition FindCondition()
        {
            Condition cond = null;

            Buff buff = null;
            Trigger trigger = null;

            switch (changeType)
            {
                case ChangeConditionType.BuffLifeCycle:
                    buff = Target.FindBuff(buffType, buffId);
                    if (buff != null)
                    {
                        cond = buff.LifeCycle.Cond;
                    }
                    break;
                case ChangeConditionType.TriggerLifeCycle:
                    trigger = Target.FindTrigger(triggerTag);
                    if (trigger != null)
                    {
                        cond = trigger.LifeCycle.Cond;
                    }
                    break;
                case ChangeConditionType.TriggerCondition:
                    trigger = Target.FindTrigger(triggerTag);
                    if (trigger != null)
                    {
                        cond = trigger.Condition;
                    }
                    break;
            }

            return cond;
        }
    }
}
