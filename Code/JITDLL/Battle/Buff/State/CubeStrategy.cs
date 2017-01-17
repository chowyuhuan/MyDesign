using System.Collections.Generic;

namespace BUFF
{
    /// <summary>
    /// 消块策略
    /// </summary>
    public class CubeStrategy : State
    {
        // 消块策略类型
        private CubeStrategyType cubeStrategyType;

        public CubeStrategy(CubeStrategyType cubeStrategyType)
        {
            this.cubeStrategyType = cubeStrategyType;
        }

        public override void Enforce(int layer)
        {
            StateBlackboard.CubeStrategy = CubeStrategyMap.Instance.Strategy[cubeStrategyType];
        }
    }

    /// <summary>
    /// 消块策略表
    /// </summary>
    public class CubeStrategyMap
    {
        private static CubeStrategyMap instance;

        public static CubeStrategyMap Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new CubeStrategyMap();
                }
                return instance; 
            }
        }

        private Dictionary<CubeStrategyType, ICubeStrategy> strategy;

        public Dictionary<CubeStrategyType, ICubeStrategy> Strategy
        {
            get { return strategy; }
        }

        private CubeStrategyMap() 
        {
            strategy = new Dictionary<CubeStrategyType, ICubeStrategy>();
            strategy.Add(CubeStrategyType.SingleToDouble, new CubeSingleToDouble());
            strategy.Add(CubeStrategyType.SingleToTriple, new CubeSingleToTriple());
            strategy.Add(CubeStrategyType.DoubleToTriple, new CubeDoubleToTriple());
            strategy.Add(CubeStrategyType.AnyToTriple, new CubeAnyToTriple());
        }
    }

    public interface ICubeStrategy
    {
        CubeEraseType Convert(CubeEraseType eraseType);
    }

    public class CubeSingleToDouble: ICubeStrategy
    {
        public CubeEraseType Convert(CubeEraseType eraseType)
        {
            if (eraseType == CubeEraseType.Single)
            {
                return CubeEraseType.Double;
            }
            return eraseType; 
        }
    }

    public class CubeSingleToTriple : ICubeStrategy
    {
        public CubeEraseType Convert(CubeEraseType eraseType)
        {
            if (eraseType == CubeEraseType.Single)
            {
                return CubeEraseType.Triple;
            }
            return eraseType; 
        }
    }

    public class CubeDoubleToTriple : ICubeStrategy
    {
        public CubeEraseType Convert(CubeEraseType eraseType)
        {
            if (eraseType == CubeEraseType.Double)
            {
                return CubeEraseType.Triple;
            }
            return eraseType; 
        }
    }

    public class CubeAnyToTriple : ICubeStrategy
    {
        public CubeEraseType Convert(CubeEraseType eraseType)
        {
            if (eraseType == CubeEraseType.Single ||
                eraseType == CubeEraseType.Double ||
                eraseType == CubeEraseType.Triple)
            {
                return CubeEraseType.Triple;
            }
            return eraseType;
        }
    }
}
