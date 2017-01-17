using UnityEngine;
using System.Collections;

public class VictoryPoseState : ActorState
{
    public override bool CanMoveHorizontal()
    {
        return false;
    }

    public override void EnterState()
    {
        Owner.ActorReference.ActorAnimEx.Play("win");
    }

    public override void ExitState()
    {
        Owner.ActorReference.ActorAnimEx.Stop("win");
    }
}
