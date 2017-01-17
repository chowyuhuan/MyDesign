using UnityEngine;
using System.Collections;

public class SlowDownState : ActorState
{
    public float Speed = 1;

    public SlowDownState()
    {

    }

    public SlowDownState(float speed)
    {
        this.Speed = speed;
    }

    public override void EnterState()
    {
        Owner.ActorReference.ActorControlEx.SpeedRate = Speed;
    }

    public override void ExitState()
    {
        Owner.ActorReference.ActorControlEx.SpeedRate = 1;
    }
}
