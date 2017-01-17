using UnityEngine;
using System.Collections;
using ACTOR;
using SKILL;

public class ActorRef : MonoCompBase
{
    public ActorControl ActorControlEx;

    public ActorPreDefine ActorPreDef;
    public ActorMovement ActorMovementEx;
    public ActorFly ActorFlyEx;
    public ActorSp ActorSpEx;
    public ActorWeapon ActorWeaponEx;
    public ActorRender ActorRenderEx;
    public ActorAnim ActorAnimEx;
    public Collider ColliderEx;

    public override void Init(Actor a)
    {
        base.Init(a);

        GameObject ownerObj = Owner.gameObject;

        ActorPreDef = ownerObj.GetComponent<ActorPreDefine>();
        ActorMovementEx = ownerObj.AddComponent<ActorMovement>();
        ActorMovementEx.Init(a);

        ActorFlyEx = ownerObj.AddComponent<ActorFly>();
        ActorFlyEx.Init(a);

        ActorRenderEx = new ActorRender();
        ActorRenderEx.Init(a);
        ActorAnimEx = new ActorAnim();
        ActorAnimEx.Init(a);
        ColliderEx = ownerObj.GetComponent<BoxCollider>();

        ActorControlEx = ownerObj.AddComponent<ActorControl>();
        ActorControlEx.Init(a);


        if (Owner.actorPrepareInfo.NeedAutoCast)
        {
            ActorSpEx = Owner.GetComponent<MonsterSkillControl>();
            if (ActorSpEx == null) ActorSpEx = ownerObj.AddComponent<MonsterSkillControl>();
        }
        else
        {
            ActorSpEx = Owner.GetComponent<ActorSp>();
            if (ActorSpEx == null) ActorSpEx = ownerObj.AddComponent<HeroSp>();
        }
        ActorSpEx.Init(a);

        ActorWeaponEx = new ActorWeapon();
        ActorWeaponEx.Init(a);
    }

    void Update()
    {
        ActorRenderEx.CUpdate();
        ActorAnimEx.CUpdate();
    }
}
