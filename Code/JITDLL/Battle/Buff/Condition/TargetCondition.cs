using System.Collections.Generic;
using Assets.Scripts.Observer;

namespace BUFF
{
    /// <summary>
    /// 目标条件基类
    /// </summary>
    public abstract class TargetCondition : Condition
    {
        // 目标
        private ITargetWrapper target;

        public ITargetWrapper Target
        {
            get { return target; }
            set { target = value; }
        }

        public override bool Result() { return true; }

        public override string Message() { return target.Tag(); }

        /// <summary>
        /// 尝试从公告牌中查询目标角色当前帧的通知
        /// </summary>
        /// <param name="observable">观察角色</param>
        /// <returns></returns>
        protected bool TryGetNotifiedFromBulletinBoard(out Observable observable)
        {
            return ActorMonitor.BulletinBoard.GetNotifiedObservableMap().TryGetValue(target.ActorMonitor(), out observable);
        }

        /// <summary>
        /// 尝试获得目标角色当前帧的主题变动通知表
        /// </summary>
        /// <param name="subjectType">主题类别</param>
        /// <param name="subjectList">变动通知表</param>
        /// <returns></returns>
        protected bool TryGetChangedSubjectList(SubjectType subjectType, out List<Subject> subjectList)
        {
            Observable observable;

            if (TryGetNotifiedFromBulletinBoard(out observable))
            {
                ActorMonitor actorMonitor = (ActorMonitor)observable;
                subjectList = actorMonitor.ChangedSubjectMap[subjectType];
                return true;
            }

            subjectList = null;
            return false;
        }

        /// <summary>
        /// 获得目标角色当前帧的主题变动通知表
        /// </summary>
        /// <param name="subjectType">主题类别</param>
        /// <returns></returns>
        protected List<Subject> GetChangedSubjectList(SubjectType subjectType)
        {
            List<Subject> subjectList;

            if (TryGetChangedSubjectList(subjectType, out subjectList))
            {
                return subjectList;
            }

            return new List<Subject>();
        }
    }
}
