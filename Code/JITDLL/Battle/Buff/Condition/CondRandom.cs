using UnityEngine;

namespace BUFF
{
    /// <summary>
    /// 随机条件
    /// </summary>
    public class CondRandom : Condition
    {
        // 概率
        private float probability;
        
        public CondRandom(float probability)
        {
            this.probability = probability;
        }

        public override bool Result()
        {
            return Random.Range(0, 100) <= probability;
        }

        public override object Clone()
        {
            return new CondRandom(probability);
        }

        public override string Message()
        {
            return "Random " + probability;
        }
    }
}
