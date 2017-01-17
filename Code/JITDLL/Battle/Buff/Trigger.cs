using UnityEngine;

namespace BUFF
{
    /// <summary>
    /// Trigger
    /// </summary>
    public class Trigger
    {
        // 标签
        private string tag;

        public string Tag
        {
            get { return tag; }
        }

        // 生命周期
        private LifeCycle lifeCycle;

        public LifeCycle LifeCycle
        {
            get { return lifeCycle; }
        }

        // 触发条件
        private Condition condition;

        public Condition Condition
        {
            get { return condition; }
        }

        // 行为分支
        private BehaviorFork[] behaviorForkList;

        // 计算总权重
        private int forkWeightSum;

        public Trigger(string tag, LifeCycle lifeCycle, Condition condition, BehaviorFork[] behaviorForkList)
        {
            this.tag = tag;
            this.lifeCycle = lifeCycle;
            this.condition = condition;
            this.behaviorForkList = behaviorForkList;

            CalculateForkWeightSum();
        }

        public void CalculateForkWeightSum()
        {
            forkWeightSum = 0;

            foreach (BehaviorFork fork in behaviorForkList)
            {
                forkWeightSum += fork.Weight;
            }
        }

        public void Update()
        {
            if (condition.Result())
            {
                Excute();
            }
        }

        public void Excute()
        {
            int random = Random.Range(0, forkWeightSum);

            foreach (BehaviorFork fork in behaviorForkList)
            {
                if (random <= fork.Weight)
                {
                    fork.Excute();
                    break;
                }
            }
        }

        public bool Finish()
        {
            return lifeCycle.Finish();
        }

        public string Detail()
        {
            return tag + " " + lifeCycle.Detail();
        }
    }
}
