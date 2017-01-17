using System;

namespace BUFF
{
    /// <summary>
    /// 数值
    /// </summary>
    public class Value : ICloneable
    {
        private ValueType valueType;
        private float value;
        private ValueDependency valueDependency;

        public Value(ValueType valueType, float value, ValueDependency valueDependency = null)
        {
            this.valueType = valueType;
            this.value = value;
            this.valueDependency = valueDependency;
        }

        public float GetValue()
        {
            switch (valueType)
            {
                case ValueType.Number:
                    return value;
                case ValueType.Factor:
                    return value * valueDependency.GetValue();
                default:
                    return 0;
            }
        }

        public object Clone()
        {
            return new Value(valueType, value, valueDependency == null ? null : (ValueDependency)valueDependency.Clone());
        }
    }
    
    /// <summary>
    /// 数值依赖
    /// </summary>
    public class ValueDependency : ICloneable
    {
        private int baseField;
        private AttributePoint basePoint;
        private ITargetWrapper baseTarget;

        public ValueDependency(int baseField, AttributePoint basePoint, ITargetWrapper baseTarget)
        {
            this.baseField = baseField;
            this.basePoint = basePoint;
            this.baseTarget = baseTarget;
        }

        public float GetValue()
        {
            return baseTarget.Attribute(baseField, basePoint == AttributePoint.Current);
        }

        public object Clone()
        {
            return new ValueDependency(baseField, basePoint, baseTarget);
        }
    }
}
