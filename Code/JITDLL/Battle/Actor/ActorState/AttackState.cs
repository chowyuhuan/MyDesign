using UnityEngine;
using System.Collections;

public class AttackState : ActorState
{
    public AttackState()
    {
        NewStateModeEx = NewStateMode.Link;
    }

    public override bool CanMoveHorizontal()
    {
        return false;
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
        if (Owner.SkillController.Caster.HasSkill())
        {
            Owner.SkillController.Caster.TryToCast();
        }
        else
        {
            Owner.ActorReference.ActorAnimEx.Play("run");
        }
    }
}
