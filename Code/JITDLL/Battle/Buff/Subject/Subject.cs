
namespace BUFF
{
    /// <summary>
    /// 通知主题类别
    /// </summary>
    public enum SubjectType
    {
        Cube,
        Attack,
        Buff,
        Count
    }

    /// <summary>
    /// 主题基类
    /// </summary>
    public abstract class Subject : IMessage
    {
        public SubjectType type;

        // 释放者
        public ITargetWrapper caster;

        public virtual string Message()
        {
            return type.ToString() + " " +
                   caster.Tag();
        }
    }
}
