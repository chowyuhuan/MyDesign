using UnityEngine;
using System;
using System.Collections;
using ACTOR;
using SKILL;
using BUFF;

public class Actor : MonoBehaviour, ITargetWrapper
{
    public int ConfigId = -1;       // 配置Id 
    public int BattleId;            // 战斗中唯一Id 
    public uint ServerId;           // 服务器Id

    public int specialSkillId = 0;
    public int weaponSkillId = 0;
    public int ringSkillId = 0;

    public Team TeamEx;

    public ActorPrepareInfo actorPrepareInfo;

    public string ActorTag
    {
        private set;
        get;
    }

    public string ActorName
    {
        private set;
        get;
    }

    public int ActorLevel
    {
        private set;
        get;
    }

    public int ActorStar
    {
        private set;
        get;
    }

    // 可能会有需要属性上限的存储
    public FloatField[] BaseFields = new FloatField[(int)ActorField.Max];

    float[] BuffFileds = new float[(int)ActorField.Max]; // TODO:优化，乱的话整理，每帧最开始时，汇总身上的buff的数值
    [HideInInspector]
    public bool Immune = false; // TODO:优化，乱的话整理
    int FrameIndex = -1; // TODO:优化，乱的话整理

    public Action<Actor> OnDeath;
    public bool IsDeath { private set; get; }

    #region 引用
    [HideInInspector]
    public SkillControl SkillController = null;
    [HideInInspector]
    public ActorRef ActorReference = null;
    #endregion

    public Camp SelfCamp
    {
        private set;
        get;
    }

    public void Initialize(ActorPrepareInfo prepareInfo, int battleId)
    {
        ActorTag = prepareInfo.Name + battleId;

        actorPrepareInfo = prepareInfo;

        ConfigId = prepareInfo.CsvId;
        BattleId = battleId;
        ServerId = prepareInfo.ServerId;
        SelfCamp = prepareInfo.CampEx;

        specialSkillId = prepareInfo.SpecialSkillId;


        IsDeath = false;

        for (int i = 0; i < (int)ActorField.Max; ++i)
        {
            BaseFields[i] = new FloatField();
        }

        SkillController = gameObject.AddComponent<SkillControl>();
        SkillController.Init(this);

        ActorReference = gameObject.AddComponent<ActorRef>();
        ActorReference.Init(this);

        ActorLevel = prepareInfo.Level;
        ActorStar = prepareInfo.Star;
        ActorName = prepareInfo.Name;

        SetValue(ActorField.HP, prepareInfo.GetAttributeFiled(ActorField.HP));
        SetValue(ActorField.HPMax, prepareInfo.GetAttributeFiled(ActorField.HPMax));
        SetValue(ActorField.ATK, prepareInfo.GetAttributeFiled(ActorField.ATK));
        SetValue(ActorField.DFPhysics, prepareInfo.GetAttributeFiled(ActorField.DFPhysics));
        SetValue(ActorField.DFMagic, prepareInfo.GetAttributeFiled(ActorField.DFMagic));

        SetValue(ActorField.CritRate, prepareInfo.GetAttributeFiled(ActorField.CritRate));
        SetValue(ActorField.CritDamage, prepareInfo.GetAttributeFiled(ActorField.CritDamage));
        SetValue(ActorField.Dodge, prepareInfo.GetAttributeFiled(ActorField.Dodge));
        SetValue(ActorField.Precision, prepareInfo.GetAttributeFiled(ActorField.Precision));

        SetValue(ActorField.APMagic, prepareInfo.GetAttributeFiled(ActorField.APMagic));
        SetValue(ActorField.APPhysics, prepareInfo.GetAttributeFiled(ActorField.APPhysics));
        SetValue(ActorField.Speed, prepareInfo.GetAttributeFiled(ActorField.Speed));
        SetValue(ActorField.Suck, prepareInfo.GetAttributeFiled(ActorField.Suck));
        SetValue(ActorField.DMReduced, prepareInfo.GetAttributeFiled(ActorField.DMReduced));
        SetValue(ActorField.Parry, prepareInfo.GetAttributeFiled(ActorField.Parry));

        if (prepareInfo.NeedAutoCast)
        {
            SetMonsterCastControl();
        }
    }

    /// <summary>
    /// 设置英雄属性 
    /// </summary>
    //void SetHeroAttribute()
    //{
    //    DataCenter.Hero heroData = DataCenter.PlayerDataCenter.GetHero(ServerId);

    //    SetValue(ActorField.HP, heroData.TotalAttribute.Hp);
    //    SetValue(ActorField.HPMax, heroData.TotalAttribute.Hp);
    //    SetValue(ActorField.ATK, heroData.TotalAttribute.Attack);
    //    SetValue(ActorField.DFPhysics, heroData.TotalAttribute.PhysicalDefence);
    //    SetValue(ActorField.DFMagic, heroData.TotalAttribute.MagicDefence);

    //    SetValue(ActorField.CritRate, heroData.TotalAttribute.CrticalRate / 100);
    //    SetValue(ActorField.CritDamage, heroData.TotalAttribute.CrticalDamage / 100);
    //    SetValue(ActorField.Dodge, heroData.TotalAttribute.Dodge / 100);
    //    SetValue(ActorField.Precision, heroData.TotalAttribute.Precision / 100);

    //    SetValue(ActorField.APMagic, heroData.TotalAttribute.MagicPenetration);
    //    SetValue(ActorField.APPhysics, heroData.TotalAttribute.PhysicPenetration);
    //    SetValue(ActorField.Speed, heroData.TotalAttribute.AttackSpeed);
    //    SetValue(ActorField.Suck, heroData.TotalAttribute.Suck / 100);
    //    SetValue(ActorField.DMReduced, heroData.TotalAttribute.DamageReduce / 100);
    //}

    /// <summary>
    /// 设置怪属性 
    /// </summary>
    //void SetMonsterAttribute()
    //{
    //    CSV_b_monster_template monsterCSV = CSV_b_monster_template.FindData(ConfigId);
    //    CSV_c_monster_attribute attributeCSV = CSV_c_monster_attribute.FindData(monsterCSV.attributeId);

    //    SetValue(ActorField.HP, attributeCSV.hp);
    //    SetValue(ActorField.HPMax, attributeCSV.hp);
    //    SetValue(ActorField.ATK, attributeCSV.attack);
    //    SetValue(ActorField.DFPhysics, attributeCSV.physicDefense);
    //    SetValue(ActorField.DFMagic, attributeCSV.magicDefense);

    //    SetValue(ActorField.CritRate, attributeCSV.crticalRate / 100);
    //    SetValue(ActorField.CritDamage, attributeCSV.crticalDamage / 100);
    //    SetValue(ActorField.Dodge, attributeCSV.dodge / 100);
    //    SetValue(ActorField.Precision, attributeCSV.precision / 100);

    //    SetValue(ActorField.APMagic, attributeCSV.magicPenetration);
    //    SetValue(ActorField.APPhysics, attributeCSV.physicPenetration);
    //    SetValue(ActorField.Speed, attributeCSV.attackSpeed);
    //    SetValue(ActorField.Suck, attributeCSV.suck / 100);
    //    SetValue(ActorField.DMReduced, attributeCSV.damageReduce / 100);
    //    SetValue(ActorField.Parry, attributeCSV.parry / 100);
    //}

    /// <summary>
    /// 设置怪的技能释放控制 
    /// </summary>
    void SetMonsterCastControl()
    {
        ((MonsterSkillControl)ActorReference.ActorSpEx).Initialize(ConfigId);

        ((MonsterSkillControl)ActorReference.ActorSpEx).Enter();
    }

    public void SetDeltaValue(ActorField _field, float _value)
    {
        if (_field < ActorField.Max)
        {
            BaseFields[(int)_field].Value += _value;

            // 挂了 
            if (!IsDeath && _field == ActorField.HP && BaseFields[(int)ActorField.HP].Value <= 0)
            {
                Die();
            }
        }
    }

    public void SetValue(ActorField _field, float _value)
    {
        if (_field < ActorField.Max)
        {
            BaseFields[(int)_field].Value = _value;

            // 挂了 
            if (!IsDeath && _field == ActorField.HP && BaseFields[(int)ActorField.HP].Value <= 0)
            {
                Die();
            }
        }
    }

    public float GetValue(ActorField _field, bool _buff = false)
    {
        if (_field >= ActorField.Max)
        {
            Debug.LogError(LogTag.Actor + "获取字段失败：" + _field.ToString());
            return 0;
        }
        else
        {
            if (_buff)
            {
                //if (FrameIndex != GameTimer.frameCount) // 每帧计算一次，应该会有问题（同帧内，buff有受击次数限制；这种设计，要接受一帧内的误差）
                //{
                //    SKILL.Buff[] buffs = GetComponents<SKILL.Buff>(); // TODO:待优化
                //    int length = (int)ActorField.Max;
                //    for (int i = 0; i < length; ++i)
                //    {
                //        BuffFileds[i] = 0;
                //    }
                //    Immune = false;
                //    for (int i = 0; i < buffs.Length; ++i)
                //    {
                //        Immune |= buffs[i].BeState(BuffEffect.Immune);
                //        for (int j = 0; j < length; ++j)
                //        {
                //            BuffFileds[j] += buffs[i].GetFloatValue((ActorField)j);
                //        }
                //    }
                //}
                //return BaseFields[(int)_field].Value + BuffFileds[(int)_field];
                return BaseFields[(int)_field].Value + SkillController.BuffAttribute((int)_field);
            }
            else
            {
                return BaseFields[(int)_field].Value;
            }
        }
    }

    public void RegisterFieldChangedNotify(ActorField _field, Action<int> _callBack)
    {
        if (_field < ActorField.Max)
        {
            Action<int> tmp = BaseFields[(int)_field].OnValueChanged;
            if (tmp != null)
            {
                tmp += _callBack;
            }
        }
    }

    void Die()
    {
        IsDeath = true;

        SkillController.Caster.Reset();

        //ActorReference.ActorControlEx.StopAnimation();
        ActorReference.ActorAnimEx.Play("falldown");
        if (transform.position.y > 0)
            ActorReference.ActorFlyEx.HitTargetMotion(ActorFly.FlyType.Fade, 0, 0, 0);
        ActorReference.ActorControlEx.enabled = false;

        if (ActorReference.ColliderEx != null)
        {
            ActorReference.ColliderEx.enabled = false;
        }

        AudioManager.Instance.PlaySound(DefaultConfig.GetString("heroDeathSound"));

        EntityPool.Destroy(gameObject, 2f);
        if (OnDeath != null) OnDeath(this);
    }

    public void CastPassiveSkill()
    {
        int skillId = SkillController.SkillPossessorEx.GetPassiveSkillID();
        if (skillId != -1)
        {
            CastSkill(skillId, SkillPriority.Low);
        }
    }

    public string Tag()
    {
        return ActorTag;
    }

    public ActorMonitor ActorMonitor()
    {
        return SkillController.ActorMonitorEx;
    }

    public void AddTrigger(BUFF.Trigger trigger)
    {
        SkillController.TriggerManagerEx.AddTrigger(trigger);
    }

    public BUFF.Trigger FindTrigger(string tag)
    {
        return SkillController.TriggerManagerEx.FindTrigger(tag);
    }

    public bool HaveBuff(BuffType type, string buffId)
    {
        return SkillController.BuffManagerEx.HaveBuff(type, buffId);
    }

    public BUFF.Buff FindBuff(BuffType type, string buffId)
    {
        return SkillController.BuffManagerEx.FindBuff(type, buffId);
    }

    public void AddBuff(BUFF.Buff buff)
    {
        SkillController.BuffManagerEx.AddBuff(buff);
    }

    public void ClearBuff(BuffType type, string buffId)
    {
        SkillController.BuffManagerEx.ClearBuff(type, buffId);
    }

    public void ClearBuff(BuffType type)
    {
        SkillController.BuffManagerEx.ClearBuff(type);
    }

    public void CastSkill(int skillId, SkillPriority priority)
    {
        switch (priority)
        {
            case SkillPriority.Low:
                SkillController.Caster.EnqueueToCast(skillId);
                break;
            case SkillPriority.High:
                SkillController.Caster.EnqueueToCastHigh(skillId);
                break;
        }
    }

    public void StopSkill()
    {
        SkillController.Caster.Cancel();
    }

    public int GetEnergy()
    {
        return ActorReference.ActorSpEx.SpPoint;
    }

    public void AddEnergy(int value)
    {
        if (value >= 0)
        {
            ActorReference.ActorSpEx.IncreaseSp(value);
        }
        else
        {
            ActorReference.ActorSpEx.ReduceSp(-value);
        }
    }

    public void GenerateCube()
    {
        SkillGenerator.Instance.AppendSkill(this);
    }

    public float Attribute(int field, bool buff = true)
    {
        return GetValue((ActorField)field, buff);
    }

    public void Damage(string skillId, ITargetWrapper caster, object atk)
    {
        Actor aActor = (Actor)caster;

        SkillController.Mixer.AddDamage(Convert.ToInt32(skillId), aActor, (Volume.ATK)atk);
    }

    public void Cure(string skillId, ITargetWrapper caster, object atk)
    {
        Actor aActor = (Actor)caster;

        SkillController.Mixer.AddCure(Convert.ToInt32(skillId), aActor, (Volume.ATK)atk);
    }
}
