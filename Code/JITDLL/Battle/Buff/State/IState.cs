
namespace BUFF
{
    /// <summary>
    /// 状态接口
    /// </summary>
    public interface IState
    {
        void Enforce(int layer);
    }
}
