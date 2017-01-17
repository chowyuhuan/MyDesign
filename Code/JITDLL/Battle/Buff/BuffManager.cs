using System.Collections.Generic;

namespace BUFF
{
    // Buff通知
    public delegate void BuffNotification(Buff buff);

    /// <summary>
    /// Buff管理
    /// </summary>
    public class BuffManager
    {
        public BuffNotification OnBuffBegin { get; set; } 
        public BuffNotification OnBuffMerge { get; set; }
        public BuffNotification OnBuffEnd { get; set; }
        public BuffNotification OnBuffImmune { get; set; }

        // Buff类别数量
        public const int BuffTypeNumber = (int)BuffType.Hidden + 1;

        // Buff表，按类别存储
        private Dictionary<string, Buff>[] buffList;
        
        // Buff表变动
        private bool buffDirty;

        // Buff生效状态黑板
        private StateBlackboard stateBlackboard;

        public StateBlackboard StateBlackboard
        {
            get { return stateBlackboard; }
        }

        public BuffManager() : this(0) { }

        public BuffManager(int attributeSize)
        {
            buffList = new Dictionary<string, Buff>[BuffTypeNumber];
            for (int i = 0; i < buffList.Length; i++)
            {
                buffList[i] = new Dictionary<string, Buff>();
            }

            buffDirty = false;

            stateBlackboard = new StateBlackboard(attributeSize);
            stateBlackboard.Reset();
        }

        private void Notify(BuffNotification notification, Buff buff)
        {
            if (notification != null)
            {
                notification(buff);
            }
        }

        public List<Buff> GetAllBuff()
        {
            List<Buff> buffAll = new List<Buff>();

            for (int i = 0; i < buffList.Length; i++)
            {
                foreach (Buff buff in new List<Buff>(buffList[i].Values))
                {
                    buffAll.Add(buff);
                }
            }

            return buffAll;
        }

        public void AddBuff(Buff buff)
        {
            if (stateBlackboard.ImmuneBuffType == (int)buff.Type)
            {
                Notify(OnBuffImmune, buff);
                return;
            }

            if (stateBlackboard.BuffTime[(int)buff.Type] != 0)
            {
                buff.LifeCycle.Cond.Change(typeof(CondTime), stateBlackboard.BuffTime[(int)buff.Type]);
            }

            Dictionary<string, Buff> list = buffList[(int)buff.Type];
            Buff existBuff = FindBuff(buff);

            if (existBuff == null)
            {
                list.Add(buff.Id, buff);
                Notify(OnBuffBegin, buff);
            }
            else
            {
                buff.Merge(existBuff);
                list.Remove(existBuff.Id);
                list.Add(buff.Id, buff);
                Notify(OnBuffMerge, buff);
            }

            buffDirty = true;
        }

        public void RemoveBuff(BuffType type, string buffId)
        {
            if (buffList[(int)type].ContainsKey(buffId))
            {
                Notify(OnBuffEnd, buffList[(int)type][buffId]);
                buffList[(int)type].Remove(buffId);
                buffDirty = true;
            }
        }

        public void ClearBuff(Buff buff)
        {
            if (buff.Removable())
            {
                RemoveBuff(buff.Type, buff.Id);
            }
        }

        public void ClearBuff(BuffType type, string buffId)
        {
            if (buffList[(int)type].ContainsKey(buffId))
            {
                ClearBuff(buffList[(int)type][buffId]);
            }
        }

        public void ClearBuff(BuffType type)
        {
            foreach (Buff buff in new List<Buff>(buffList[(int)type].Values))
            {
                ClearBuff(buff);
            }
        }

        public Buff FindBuff(BuffType type, string buffId)
        {
            if (buffList[(int)type].ContainsKey(buffId))
            {
                return buffList[(int)type][buffId];  
            }
            return null;
        }

        public Buff FindBuff(Buff buff)
        {
            return FindBuff(buff.Type, buff.Id);
        }

        public bool HaveBuff(BuffType type, string buffId)
        {
            return FindBuff(type, buffId) != null;
        }

        public bool HaveBuff(Buff buff)
        {
            return HaveBuff(buff.Type, buff.Id);
        }

        public void Update()
        {
            UpdateBuff();

            if (buffDirty)
            {
                buffDirty = false;

                EnforceBuff();
            }
        }

        private void UpdateBuff()
        {
            for (int i = 0; i < BuffTypeNumber; i++)
            {
                UpdateBuffType((BuffType)i);
            }
        }

        /// <summary>
        /// 刷新单类别buff
        /// </summary>
        /// <param name="type"></param>
        private void UpdateBuffType(BuffType type)
        {
            Dictionary<string, Buff> buffDict = buffList[(int)type];

            List<string> buffFinished = new List<string>();

            foreach (KeyValuePair<string, Buff> pair in buffDict)
            {
                if (pair.Value.Finish())
                {
                    buffFinished.Add(pair.Key);
                }
            }

            foreach (string key in buffFinished)
            {
                RemoveBuff(type, key);
            }
        }

        private void EnforceBuff()
        {
            stateBlackboard.Reset();

            for (int i = 0; i < BuffTypeNumber; i++)
            {
                EnforceBuffType((BuffType)i);
            }
        }

        /// <summary>
        /// 生效单类别buff
        /// </summary>
        /// <param name="type"></param>
        private void EnforceBuffType(BuffType type)
        {
            Dictionary<string, Buff> buffDict = buffList[(int)type];

            foreach (KeyValuePair<string, Buff> pair in buffDict)
            {
                pair.Value.Enforce(stateBlackboard);
            }
        }
    }
}
