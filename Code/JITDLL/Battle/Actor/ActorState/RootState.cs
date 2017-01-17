using UnityEngine;
using System.Collections;

public class RootState : ActorState
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
    }

    public override void ExitState()
    {
    }
}
