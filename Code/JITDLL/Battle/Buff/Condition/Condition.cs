using System;

namespace BUFF
{
    /// <summary>
    /// 条件基类
    /// </summary>
    public abstract class Condition : ICondition, ICloneable, IMessage
    {
        public abstract bool Result();

        public virtual void Reset() { }

        public abstract object Clone();

        public abstract string Message();

        public virtual void Change(Type type, params object[] args) { }

        public static bool Compare(CompareOperation op, float value1, float value2)
        {
            switch (op)
            {
                case CompareOperation.LessThan:
                    return value1 < value2;
                case CompareOperation.LessThanOrEqualTo:
                    return value1 <= value2;
                case CompareOperation.EqualTo:
                    return value1 == value2;
                case CompareOperation.NotEqualTo:
                    return value1 != value2;
                case CompareOperation.GreaterThanOrEqualTo:
                    return value1 >= value2;
                case CompareOperation.GreaterThan:
                    return value1 > value2;
                default:
                    return false;
            }
        }
    }
}
