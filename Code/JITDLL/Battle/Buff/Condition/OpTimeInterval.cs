using UnityEngine;

namespace BUFF
{
    /// <summary>
    /// 时间间隔
    /// </summary>
    public class OpTimeInterval : OpUnary
    {
        private float time;

        public float Time
        {
            get { return time; }
            set { time = value; timer = time; }
        }

        private float timer;

        public OpTimeInterval() { }

        public OpTimeInterval(Condition cond, float time) : base(cond)
        {
            this.Time = time;
        }

        public override bool Result()
        {
            if (timer >= time && cond.Result())
            {
                timer = 0;
                return true;
            }
            else
            {
                timer += UnityEngine.Time.deltaTime;
                return false;
            }
        }

        public override void Reset()
        {
            timer = time;
        }

        public override object Clone()
        {
            return new OpTimeInterval((Condition)cond.Clone(), time);
        }

        public override string Message()
        {
            return "(CD " + timer + "/" + time + " " + cond.Message() + ")";
        }
    }
}
