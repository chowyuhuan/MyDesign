using UnityEngine;
using System.Collections;
using ACTOR;
using SKILL;

public class MoveDestinationState : ActorState
{
    float _moveSpeed;

    public float Destination;

    int _moveRight = 1;
    float _dis;
    float _step;

    public override bool CanMoveHorizontal()
    {
        return false;
    }

    public override bool CanMotionActive()
    {
        return false;
    }

    public override void EnterState()
    {
        _moveSpeed = DefaultConfig.GetFloat("ActorMoveSpeed");
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        _dis = Destination - Owner.transform.position.x;
        _moveRight = _dis > 0 ? 1 : -1;
        _step = _moveRight * _moveSpeed * GameTimer.deltaTime;

        if (Mathf.Abs(_dis) <= Mathf.Abs(_step))
        {
            Owner.ActorReference.ActorMovementEx.MovePosition(new Vector2(Destination, Owner.transform.position.y));
            Owner.ActorReference.ActorControlEx.RaiseOnReachDestination();

            Owner.ActorReference.ActorControlEx.RemoveState(this);
        }
        else
        {
            Owner.ActorReference.ActorMovementEx.MovePosition(new Vector2(Owner.transform.position.x + _step, Owner.transform.position.y));
        }
    }
}
