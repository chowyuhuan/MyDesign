using System.Collections.Generic;
using Assets.Scripts.Observer;

namespace BUFF
{
    /// <summary>
    /// 角色监视器, 继承于被观察者
    /// </summary>
    public class ActorMonitor : Observable
    {
        // 观察者:全局公告牌，用于公布当前帧所有发来通知的角色
        private static BulletinBoard bulletinBoard = new BulletinBoard();

        public static BulletinBoard BulletinBoard
        {
            get { return bulletinBoard; }
        }

        // 角色主题变动通知表, 按类别存储
        private Dictionary<SubjectType, List<Subject>> changedSubjectMap;

        public Dictionary<SubjectType, List<Subject>> ChangedSubjectMap
        {
            get { return changedSubjectMap; }
        }

        // 角色标签
        private string tag;

        public string Tag
        {
            get { return tag; }
        }

        public ActorMonitor(string tag)
        {
            this.tag = tag;
            AddObserver(bulletinBoard);
            InitChangedSubjectMap();
        }

        public void Update()
        {
            NotifyObserver();
        }

        /// <summary>
        /// 初始化通知表
        /// </summary>
        public void InitChangedSubjectMap()
        {
            changedSubjectMap = new Dictionary<SubjectType, List<Subject>>();
            for (int i = 0; i < (int)SubjectType.Count; i++)
            {
                changedSubjectMap.Add((SubjectType)i, new List<Subject>());
            }
        }

        /// <summary>
        /// 清空通知表
        /// </summary>
        public void ClearChangedSubjectMap()
        {
            foreach (List<Subject> subjectList in changedSubjectMap.Values)
            {
                subjectList.Clear();
            }
        }

        /// <summary>
        /// 添加通知主题
        /// </summary>
        /// <param name="subject"></param>
        public void AddSubject(Subject subject)
        {
            changedSubjectMap[subject.type].Add(subject);
            SetChanged();
#if UNITY_EDITOR
            switch (subject.type)
            {
                case SubjectType.Cube:
                    Logger.Log(subject.Message(), Logger.CUBE_COLOR);
                    break;
                case SubjectType.Attack:
                    Logger.Log(subject.Message(), Logger.ATTACK_COLOR);
                    break;
                case SubjectType.Buff:
                    Logger.Log(subject.Message(), Logger.BUFF_COLOR);
                    break;
            }
#endif
        }
    }
}
