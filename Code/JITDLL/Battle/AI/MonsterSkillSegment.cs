using UnityEngine;
using System.Collections;
using SKILL;

public class MonsterSkillSegment : SkillSegment
{
    public override void Reduce(float amount)
    {
        // 数值换算成时间再减 
        AccumulatedTime -= Duration * amount / 100;

        if (AccumulatedTime < 0) AccumulatedTime = 0;

        RaiseSpProgressChange(AccumulatedTime / Duration);
    }

    public override void Enter()
    {
        AccumulatedTime = 0;
        Filled = false;

        RaiseSpProgressChange(AccumulatedTime / Duration);
    }

    public override void Tick()
    {
        AccumulatedTime += GameTimer.deltaTime;

        if (AccumulatedTime >= Duration)
        {
            AccumulatedTime = Duration;
            Filled = true;
        }

        RaiseSpProgressChange(AccumulatedTime / Duration);

        if (Filled)
        {
            RaiseSpSkillFilled(SkillId);
        }
    }
}
