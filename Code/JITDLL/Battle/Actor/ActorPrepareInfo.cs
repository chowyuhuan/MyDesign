using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

public class ActorPrepareInfo
{
    public uint ServerId = 0;

    public int CsvId = 0;

    public int Level;

    public int Star;

    public string Name;

    public Camp CampEx;

    public bool IsLeader;

    public int SpecialSkillId;

    public int NormalRangeId;

    public float OffsetX;

    public float FlyFactor;

    public ActorWeaponInfo WeaponInfo = null;

    public string PrefabPath = string.Empty;

    public string AnimPath = string.Empty;

    public string EffectPath = string.Empty;

    public ACTOR.FloatField[] AttributeFileds = new ACTOR.FloatField[(int)ACTOR.ActorField.Max];

    public bool NeedAutoCast = false;

    public List<int> NormalAttackIDs = new List<int>();

    public void CreateAttributeFiled()
    {
        for (int i = 0; i < (int)ACTOR.ActorField.Max; ++i)
        {
            AttributeFileds[i] = new ACTOR.FloatField();
        }
    }

    public void SetAttributeFiled(ACTOR.ActorField field, float value)
    {
        AttributeFileds[(int)field].Value = value;
    }

    public float GetAttributeFiled(ACTOR.ActorField field)
    {
        return AttributeFileds[(int)field].Value;
    }
}

public class GoddessPrepareInfo
{
    public int SkillId;

    public string PrefabPath = string.Empty;

    public Vector3 Rotation = Vector3.zero; 

    public Camp CampEx;
}
