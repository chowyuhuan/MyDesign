using System;
using System.Collections.Generic;

namespace BUFF
{
    /// <summary>
    /// 行为分支
    /// </summary>
    public class BehaviorFork : IBehavior, ICloneable
    {
        // 权重
        private int weight;

        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        // 行为列表
        private List<IBehavior> behaviorList;

        public BehaviorFork()
        {
            behaviorList = new List<IBehavior>();
        }

        public BehaviorFork(BehaviorFork behaviorFork)
        {
            this.weight = behaviorFork.weight;
            this.behaviorList = new List<IBehavior>(behaviorFork.behaviorList);
        }

        public void Add(IBehavior behavior)
        {
            behaviorList.Add(behavior);
        }

        public void Excute()
        {
            foreach (IBehavior behavior in behaviorList)
            {
                behavior.Excute();
            }
        }

        public object Clone()
        {
            return new BehaviorFork(this);
        }
    }
}
