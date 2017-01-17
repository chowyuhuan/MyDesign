using UnityEngine;
using System;
using System.Collections;
using SKILL;

public class SkillSegment : SpChange
{
    protected float Duration;

    protected int SkillId;

    protected float AccumulatedTime;
    protected bool Filled;

    public void Initialize(int skillId, float duration)
    {
        SkillId = skillId;
        Duration = duration;
    }

    public virtual void Reduce(float amount)
    {

    }

    public virtual void Enter()
    {

    }

    public virtual void Tick()
    {

    }
}
