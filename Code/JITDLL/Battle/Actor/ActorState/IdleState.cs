using UnityEngine;
using System.Collections;

public class IdleState : ActorState
{

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
        Owner.ActorReference.ActorAnimEx.Play("fight");
    }

    public override void ExitState()
    {
        //Owner.ActorReference.ActorAnimEx.Stop("fight");
        Owner.ActorReference.ActorAnimEx.Play("run");
    }
}
