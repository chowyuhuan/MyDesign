using System;

namespace BUFF
{
    /// <summary>
    /// 行为基类
    /// </summary>
    public abstract class Behavior : IBehavior, ICloneable, IMessage
    {
        // 行为目标
        private ITargetWrapper target;

        public ITargetWrapper Target
        {
            get { return target; }
            set { target = value; }
        }

        public abstract void ExcuteBehavior();

        public void Excute()
        {
            ExcuteBehavior();
#if UNITY_EDITOR
            Logger.Log(Message(), Logger.BEHAVIOR_COLOR);
#endif
        }

        public abstract object Clone();

        public virtual string Message()
        {
            return base.GetType().Name + " " + (target == null ? "无" : target.Tag());
        }
    }
}
