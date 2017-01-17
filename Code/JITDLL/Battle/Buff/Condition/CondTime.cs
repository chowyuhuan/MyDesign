using UnityEngine;
using System;

namespace BUFF
{
    /// <summary>
    /// 时间条件
    /// </summary>
    public class CondTime : Condition
    {
        // 时间
        private float time;

        // 计时器
        private float timer;

        public CondTime(float time)
        {
            this.time = time;

            timer = time;
        }
        
        public override bool Result()
        {
            timer -= Time.deltaTime;
            
            return timer <= 0;
        }

        public override void Reset()
        {
            timer = time;
        }

        public override object Clone()
        {
            return new CondTime(time);
        }

        public override string Message()
        {
            return "Time " + timer.ToString("f2") + "/" + time;
        }

        public override void Change(Type type, params object[] args)
        {
            if (base.GetType().Equals(type))
            {
                float changeTime = (float)args[0];
                timer += changeTime;
            }
        }
    }
}
