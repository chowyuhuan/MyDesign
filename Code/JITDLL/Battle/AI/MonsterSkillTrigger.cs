using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

/// <summary>
/// 不知道叫啥名了
/// 可能会依照血量触发，或者另一个时间计时触发
/// </summary>
public class MonsterSkillTrigger
{
    MonsterSkillControl _skillControl;

    // 可能会随机选者一个？？？ 
    List<Skill> skills = new List<Skill>();

    public void Initialize(MonsterSkillControl skillControl)
    {
        _skillControl = skillControl;
    }

    public void Tick()
    {

    }
}
