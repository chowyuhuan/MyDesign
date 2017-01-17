
namespace BUFF
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class State : IState
    {
        private StateBlackboard stateBlackboard;

        public StateBlackboard StateBlackboard
        {
            get { return stateBlackboard; }
            set { stateBlackboard = value; }
        }

        public abstract void Enforce(int layer);
    }
}
