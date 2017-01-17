using UnityEngine;
using System.Collections;

public class StunState : ActorState
{
    public override bool CanMoveHorizontal()
    {
        return false;
    }

    public override bool CanMotionActive()
    {
        return false;
    }

    public override bool CanCastSkill()
    {
        return false;
    }

    public override bool CanNormalAttack()
    {
        return false;
    }

    public override void EnterState()
    {
        //Owner.ActorReference.ActorControlEx.CrossFadeAnimation("stun");
    }

    public override void ExitState()
    {
        //Owner.ActorReference.ActorControlEx.StopAnimation("stun");
        if (Owner.SkillController.Caster.HasSkill())
        {
            Owner.SkillController.Caster.TryToCast();
        }
    }
}
