using System.Collections.Generic;
using UnityEngine;
using SKILL;
using BUFF;

public class BuffViewer : MonoCompBase
{
    // Show buffs in inspector.
    public List<string> buffNames = new List<string>();

    // Show triggers in inspector.
    public List<string> triggerNames = new List<string>();

    public override void Init(Actor a)
    {
        base.Init(a);
    }

    void Update()
    {
        buffNames.Clear();
        foreach (BUFF.Buff buff in Owner.SkillController.BuffManagerEx.GetAllBuff())
        {
            buffNames.Add(buff.Detail());
        }

        triggerNames.Clear();
        foreach (BUFF.Trigger trigger in Owner.SkillController.TriggerManagerEx.GetAllTrigger())
        {
            triggerNames.Add(trigger.Detail());
        }
    }
}
