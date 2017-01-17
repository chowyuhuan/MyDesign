using UnityEngine;
using System.Collections;

public class MotionState : ActorState
{
    public ActorFly.MotionMode motionMode;

    public ActorFly.FlyType FlyType;

    public ActorFly.VelocityData velocityData;

    float _xAcc = 5;
    float _xActiveSpeed = 5;
    float _xFinalSpeed = 0;

    bool _apply = true;

    public float YSpeed = 0;
    float _yAcc = 0;
    float _posY = 0;
    bool _grounded = false;

    float _scaledSpeed = 1f;

    public override void EnterState()
    {
        _xAcc = DefaultConfig.GetFloat("HorizontalAccelerate");
        _xActiveSpeed = DefaultConfig.GetFloat("HorizontalAdditive");

        _grounded = false;
        _yAcc = DefaultConfig.GetFloat("VerticalAccelerate");
    }

    public override void UpdateState()
    {
        _scaledSpeed = GameTimer.deltaTime * _speedRate;

        // Y方向 
        if (!_grounded)
        {
            _posY = Owner.transform.position.y;
            YSpeed -= _yAcc * GameTimer.deltaTime;

            _posY += YSpeed * _scaledSpeed;

            if (_posY < 0)
            {
                _posY = 0;
                YSpeed = 0;

                _grounded = true;
            }

            Owner.ActorReference.ActorMovementEx.MovePositionY(_posY, false);
        }

        // X方向 
        velocityData.Update(GameTimer.deltaTime);

        _apply = true;
        if (velocityData.MotionModeEx == ActorFly.MotionMode.Active &&
            !Owner.ActorReference.ActorControlEx.CanMotionActive())
        {
            _apply = false;
        }

        if (_apply)
        {
            _xFinalSpeed = velocityData.Value + _xActiveSpeed;
            Vector3 pos = Owner.transform.position;
            pos = new Vector3(pos.x + _xFinalSpeed * _scaledSpeed, pos.y, pos.z);

            Owner.ActorReference.ActorMovementEx.MovePosition(pos);
        }

        if (velocityData.IsEnd() && _grounded)
        {
            Owner.ActorReference.ActorControlEx.RemoveState(this);
        }
    }

    protected override void AppenData(ActorState newState)
    {
        // 特殊运动都是替换 
        ReplaceData(newState);
    }

    protected override void ReplaceData(ActorState newState)
    {
        MotionState motionState = newState as MotionState;

        velocityData = motionState.velocityData;
        YSpeed = motionState.YSpeed;
        _grounded = false;
    }

    public override bool CanMoveHorizontal()
    {
        return false;
    }
}
