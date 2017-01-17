using UnityEngine;
using System.Collections;

public enum NewStateMode
{
    Append,     // 新数据叠加到旧数据上 
    Replace,    // 新数据替换旧数据 
    Discard,    // 新数据丢弃旧数据不变 
    Link,       // 连续,旧的退出开始新的 
}

public abstract class ActorState
{
    public NewStateMode NewStateModeEx = NewStateMode.Replace;

    protected Actor Owner;

    public ActorState next;

    protected float _speedRate = 1f;

    public virtual void AddOwner(Actor actor)
    {
        Owner = actor;

        EnterState();
    }

    public virtual void RemoveOwner()
    {
        ExitState();

        Owner = null;
    }

    public virtual void EnterState()
    {

    }

    public virtual void UpdateState()
    {

    }

    public virtual void ExitState()
    {

    }

    public System.Type GetCacheType()
    {
        return this.GetType();
    }

    public void CreateDataWithNewState(ActorState newState)
    {
        switch(newState.NewStateModeEx)
        {
            case NewStateMode.Append:
                AppenData(newState);
                break;

            case NewStateMode.Replace:
                ReplaceData(newState);
                break;

            case NewStateMode.Link:
                next = newState;
                break;

            default:
                break;
        }
    }

    protected virtual void AppenData(ActorState newState)
    {

    }

    protected virtual void ReplaceData(ActorState newState)
    {

    }

    /// <summary>
    /// 能做水平移动吗,指没有特殊状态(定身.击飞.前冲等)的基本运动 
    /// </summary>
    /// <returns></returns>
    public virtual bool CanMoveHorizontal()
    {
        return true;
    }

    /// <summary>
    /// 能做自触发的特殊运动(如技能施加的前冲) 
    /// </summary>
    /// <returns></returns>
    public virtual bool CanMotionActive()
    {
        return true;
    }

    /// <summary>
    /// 能释放技能吗 
    /// </summary>
    /// <returns></returns>
    public virtual bool CanCastSkill()
    {
        return true;
    }

    /// <summary>
    /// 能进行普通攻击吗 
    /// </summary>
    /// <returns></returns>
    public virtual bool CanNormalAttack()
    {
        return true;
    }

    /// <summary>
    /// 设置状态速率 
    /// </summary>
    /// <param name="speedRate"></param>
    public void SetSpeedRate(float speedRate)
    {
        _speedRate = speedRate;
        if (_speedRate < 0)
        {
            _speedRate = 0;
        }
    }
}
