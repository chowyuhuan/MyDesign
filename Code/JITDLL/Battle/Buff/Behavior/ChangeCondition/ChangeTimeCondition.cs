
namespace BUFF
{
    /// <summary>
    /// 修改时间条件
    /// </summary>
    public class ChangeTimeCondition : ChangeCondition
    {
        // 修改的时间
        private float time;

        public ChangeTimeCondition( float time)
        {
            this.time = time;
        }

        public override void ExcuteBehavior()
        {
            Condition cond = FindCondition();

            if (cond != null)
            {
                cond.Change(typeof(CondTime), time);
            }
        }

        public override object Clone()
        {
            ChangeTimeCondition behavior = new ChangeTimeCondition(time);
            behavior.Init(this);
            return behavior;
        }
    }
}
