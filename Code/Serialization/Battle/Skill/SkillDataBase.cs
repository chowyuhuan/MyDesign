using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

[CreateAssetMenu(fileName="SkillDataBase", menuName = "SkillDataBase")]
public class SkillDataBase : ScriptableObject
{
    public List<Skill> Data = new List<Skill>();
}
