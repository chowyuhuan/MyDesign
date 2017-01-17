using UnityEngine;
using System;
using System.Collections;
using SKILL;
using ACTOR;

public class ActorMovement : MonoCompBase
{
    public Vector2 Position
    {
        get
        {
            return transform.position;
        }
    }

    public float RelativeForwordX(float distance)
    {
        int adjust = Owner.SelfCamp == Camp.Comrade ? 1 : -1;
        return Position.x + adjust * distance;
    }

    /// <summary>
    /// 移动位置时的委托 
    /// </summary>
    public Action<Actor> OnMovePosition;

    /// <summary>
    /// 移动人物位置必须用这个借口，不允许直接transform.xxx
    /// 移动Actor到目标位置，受双方Actor之间可能的碰撞约束 
    /// </summary>
    /// <param name="position">目标位置</param>
    /// <param name="restrictWithBackline">受不受战场范围约束</param>
    public void MovePosition(Vector2 pos, bool restrictWithBackline = true)
    {
        if (Owner.IsDeath) // 死了就自由了 
        {
            transform.position = pos;
            return;
        }

        // 约束 
        Vector2 to = pos;
        float enemyFrontline;
        float selfFrontline;
        float selfBackline;
        float leftOffset = Owner.ActorReference.ActorPreDef.Offset - Owner.ActorReference.ActorPreDef.Width / 2;
        float rightOffset = Owner.ActorReference.ActorPreDef.Offset + Owner.ActorReference.ActorPreDef.Width / 2;

        if (Owner.SelfCamp == Camp.Comrade)
        {
            // 检查对方线 
            enemyFrontline = ActorManager.Instance.GetFrontline(Camp.Enemy);
            selfFrontline = pos.x + rightOffset;

            if (selfFrontline > enemyFrontline)
            {
                to.x = enemyFrontline - rightOffset;
            }

            // 检查己方后背线 
            if (restrictWithBackline && pos.x < transform.position.x)
            {
                selfBackline = pos.x + leftOffset;
                if (selfBackline < BattleManager_DL.Instance.LeftBound)
                {
                    to.x = BattleManager_DL.Instance.LeftBound - leftOffset;
                }
            }
        }
        else
        {
            // 检查对方线 
            enemyFrontline = ActorManager.Instance.GetFrontline(Camp.Comrade);
            selfFrontline = pos.x + leftOffset;

            if (selfFrontline < enemyFrontline)
            {
                to.x = enemyFrontline - leftOffset;
            }

            // 检查己方后背线 
            if (restrictWithBackline && pos.x > transform.position.x)
            {
                selfBackline = pos.x + rightOffset;
                if (selfBackline > BattleManager_DL.Instance.RightBound)
                {
                    to.x = BattleManager_DL.Instance.RightBound - rightOffset;
                }
            }
        }

        transform.position = to;

        if (OnMovePosition != null)
        {
            OnMovePosition(Owner);
        }
    }

    public void MovePositionY(float y, bool notifyChange = true)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        if (notifyChange && OnMovePosition != null)
        {
            OnMovePosition(Owner);
        }
    }
}
