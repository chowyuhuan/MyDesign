using UnityEngine;
using System.Collections;
using SKILL;
using ACTOR;
using System;

/// <summary>
/// 技能生效器
/// 技能生效的结果，见SkillConfig，有详细的分类
/// 生效的过程中，一方面要考虑技能与技能之间的相互作用，另一方面也要考虑主体数值、状态对作用和结果的影响（比如主体有无敌、暴走...状态，单纯技能之间的作用是不行的）
/// 技能之间的相互关系，可以通过生效的优先级控制，比如维护一个map或者类似的有优先级的分组结构（格挡、命中、暴击、防御、伤害...）
/// 挂在角色身上
/// </summary>
public class SkillMixer : MonoCompBase
{
    public delegate void TakeEffectNotify(Actor attack, Actor attecked, int value);
    TakeEffectNotify[] _takeEffectNotify = new TakeEffectNotify[(int)MixEffect.Max];

    public delegate void HitNotify(int skillId, Actor caster, Actor target, int value, bool hit, bool dodge, bool parry, bool crit);
    public HitNotify OnHitDamage;
    public HitNotify OnHitCure;

    // 被SkillDC攻击
    public void Attacked(SkillDC dc, Collider collider)
    {
        bool hit = true;
        if (dc.MetaEx.Intention == Intent.Damage)
        {
            if (Damage(dc.SkillId, dc.MetaEx, dc.Owner))
            {
                if (Owner.IsDeath) // 双，你个鞭尸狂魔，死了尸体就不要乱飞了吧 
                {
                    HighLight();
                }
                else
                {
                    OnDamage(collider.transform.position, dc.MetaEx.HitMotion, dc.MetaEx.Force1, dc.MetaEx.Force2);
                }
            }
            else
            {
                hit = false;
            }
        }
        else
        {
            Cure(dc.SkillId, dc.MetaEx, dc.Owner);
        }
        if (hit)
        {
            OnHit(dc.MetaEx, collider);
            ExcuteBehaviors(dc);
        }
    }

    public void ExcuteBehaviors(SkillDC dc)
    {
        dc.Owner.SkillController.ExcuteBehaviors(dc.MetaEx.Behaviors, dc.SkillId, Owner);
    }

    // 被DCMeta攻击
    public void Attacked(int skillId, DCMeta dc, Actor caster)
    {
        if (dc.Intention == Intent.Damage)
        {
            Damage(skillId, dc, caster);
        }
        else
        {
            Cure(skillId, dc, caster);
        }
        OnHit(dc, null);
    }

    public void OnTakeEffect(Actor attack, Actor attacked, MixEffect effect, int value)
    {
        if (effect < MixEffect.Max)
        {
            TakeEffectNotify notify = _takeEffectNotify[(int)effect];
            if (notify != null)
            {
                notify(attack, attacked, value);
            }
        }
    }

    public void RegisterEffectNotify(MixEffect effect, TakeEffectNotify callBack)
    {
        if (effect < MixEffect.Max)
        {
            _takeEffectNotify[(int)effect] += callBack;
        }
    }

    /// <summary>
    /// 可能用不上
    /// </summary>
    /// <param name="_effect"></param>
    /// <param name="_callBack"></param>
    public void RemoveOnEffectNotify(MixEffect effect, TakeEffectNotify callBack)
    {
        if (effect < MixEffect.Max)
        {
            _takeEffectNotify[(int)effect] -= callBack;
        }
    }

    /// <summary>
    /// 血量变化，显示数值效果, 生命上限控制
    /// </summary>
    /// <param name="aActor"></param>
    /// <param name="dActor"></param>
    /// <param name="damage"></param>
    /// <param name="crit"></param>
    private void ChangeHp(Actor aActor, Actor dActor, int value, bool crit, MixEffect effect)
    {
        if (value != 0)
        {
            if(crit)
            {
                OnTakeEffect(aActor, dActor, MixEffect.Crit, Math.Abs(value));
            }
            else
            {
                OnTakeEffect(aActor, dActor, effect, Math.Abs(value));
            }

            dActor.SetDeltaValue(ActorField.HP, value);

            if (value > 0)
            {
                float hpMax = dActor.GetValue(ActorField.HPMax);
                if (dActor.GetValue(ActorField.HP) > hpMax)
                {
                    dActor.SetValue(ActorField.HP, hpMax);
                }
            }
        }
    }

    /// <summary>
    /// 数值转换
    /// </summary>
    /// <param name="actorField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private float ConvertValue(ActorField actorField, float value)
    {
        switch (actorField)
        {
            case ActorField.Precision:
            case ActorField.CritRate:
            case ActorField.Parry:
            case ActorField.Dodge:
                return value * 100;
            case ActorField.CritDamage:
            case ActorField.Suck:
            case ActorField.DMReduced:
                return value / 100;
        }
        return value;
    }

    /// <summary>
    /// 伤害计算
    /// </summary>
    /// <param name="aActor"></param>
    /// <param name="dActor"></param>
    /// <param name="atkValue"></param>
    /// <param name="critDamage"></param>
    /// <param name="dfMagic"></param>
    /// <param name="apMagic"></param>
    /// <param name="dfPhysics"></param>
    /// <param name="apPhysics"></param>
    /// <param name="dmReduced"></param>
    /// <param name="suck"></param>
    /// <returns></returns>
    private int CalculateDamage(Actor aActor, Actor dActor, Nature nature, float atkValue, float critDamage, float dfMagic, float apMagic, float dfPhysics, float apPhysics, float dmReduced, float suck)
    {
        float dfMagicRate = Mathf.Clamp01(100.0f / ((dfMagic - apMagic) * 0.348f + 100.0f)); // 魔法减伤后
        float dfPhysicsRate = Mathf.Clamp01(100.0f / ((dfPhysics - apPhysics) * 0.348f + 100.0f)); // 物理减伤后
        float dfRate = 1;
        switch (nature)
        {
            case Nature.Magic:
                dfRate = dfMagicRate;
                break;
            case Nature.Physics:
                dfRate = dfPhysicsRate;
                break;
            case Nature.Real:
                break;
        }
        dmReduced = dmReduced < 1 ? dmReduced : 1;
        float value = atkValue * dfRate * (1 + critDamage) * (1 - dmReduced);

        // 最终伤害值
        int damage = (int)value;

        // 伤害减免
        float final = dActor.SkillController.BuffReduceShield(value);
        // 分担伤害
        int remain = (int)BuffShareDamage(aActor, dActor, final);
        ChangeHp(aActor, dActor, -remain, critDamage != 0, MixEffect.Damage);

        // 攻击方吸血
        int blood = (int)(value * suck);
        ChangeHp(aActor, aActor, blood, false, MixEffect.Cure);

        // 攻击方受到荆棘伤害
        int thorns = (int)(value * dActor.SkillController.BuffThornsDamage());
        ChangeHp(dActor, aActor, -thorns, false, MixEffect.Damage);

        return damage;
    }

    /// <summary>
    /// 治疗数值计算
    /// </summary>
    /// <param name="aActor"></param>
    /// <param name="dActor"></param>
    /// <param name="atkVlaue"></param>
    /// <param name="critDamage"></param>
    /// <param name="cureReduced"></param>
    /// <returns></returns>
    private int CalculateCure(Actor aActor, Actor dActor, float atkValue, float critDamage, float cureReduced)
    {
        cureReduced = cureReduced < 1 ? cureReduced : 1;
        float value = atkValue * (1 + critDamage) * (1 - cureReduced);

        int cure = (int)value;
        ChangeHp(aActor, dActor, cure, false, MixEffect.Cure);

        return cure;
    }

    /// <summary>
    /// Buff分担伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="dActor"></param>
    /// <returns></returns>
    public float BuffShareDamage(Actor aActor, Actor dActor, float damage)
    {
        float remain = damage;

        Actor[] partners = ActorManager.Instance.Choose(dActor, SkillUtility.LocateCamp(dActor.SelfCamp, Camp.Comrade), Target.Partner);

        for (int i = 0; i < partners.Length; i++)
        {
            float share = damage * partners[i].SkillController.BuffShareDamage();

            if (share >= remain)
            {
                ChangeHp(aActor, partners[i], (int)-remain, false, MixEffect.Damage);
                remain = 0;
                break;
            }
            else
            {
                ChangeHp(aActor, partners[i], (int)-share, false, MixEffect.Damage);
                remain -= share;
            }
        }

        return remain;
    }

    /// <summary>
    /// Buff追加造成伤害量百分比的伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="aActor"></param>
    /// <param name="dActor"></param>
    /// <param name="critDamage"></param>
    /// <param name="dfMagic"></param>
    /// <param name="apMagic"></param>
    /// <param name="dfPhysics"></param>
    /// <param name="apPhysics"></param>
    /// <param name="dmReduced"></param>
    /// <param name="suck"></param>
    public void BuffAdditionalDamage(int damage, Actor aActor, Actor dActor, float critDamage, float dfMagic, float apMagic, float dfPhysics, float apPhysics, float dmReduced, float suck)
    {
        foreach (var additionalDamage in aActor.SkillController.BuffAdditionalDamage())
        {
            CalculateDamage(aActor, dActor, (Nature)additionalDamage.nature, damage * additionalDamage.factor, 0, dfMagic, apMagic, dfPhysics, apPhysics, dmReduced, suck);
        }
    }

    /// <summary>
    /// Buff追加闪避（独立判断）
    /// </summary>
    /// <param name="dActor"></param>
    /// <returns></returns>
    public bool BuffAdditionalDodge(Actor dActor)
    {
        int rand = UnityEngine.Random.Range(1, 10000);
        return rand <= dActor.SkillController.BuffAdditionalDodge() * 10000;
    }

    /// <summary>
    /// 追加伤害
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="atk"></param>
    /// <returns></returns>
    public int AddDamage(int skillId, Actor caster, Volume.ATK atk)
    {
        Actor aActor = caster; // 攻
        Actor dActor = Owner; // 防

        // ---------------------------- 攻击方 -------------------------------------------
        float apMagic = ConvertValue(ActorField.APMagic, aActor.GetValue(ActorField.APMagic, true));
        float apPhysics = ConvertValue(ActorField.APPhysics, aActor.GetValue(ActorField.APPhysics, true));
        float suck = ConvertValue(ActorField.Suck, aActor.GetValue(ActorField.Suck, true));

        // ---------------------------- 被攻击方 -------------------------------------------
        float dfMagic = ConvertValue(ActorField.DFMagic, dActor.GetValue(ActorField.DFMagic, true));
        float dfPhysics = ConvertValue(ActorField.DFPhysics, dActor.GetValue(ActorField.DFPhysics, true));
        float dmReduced = ConvertValue(ActorField.DMReduced, dActor.GetValue(ActorField.DMReduced, true));

        // 无敌
        if (dActor.SkillController.BuffInvincible())
        {
            return 0;
        }

        int damage = CalculateDamage(aActor, dActor, atk.NatureEx, SkillUtility.GetSkillValueNumeric(atk.Meta, aActor, dActor), 0, dfMagic, apMagic, dfPhysics, apPhysics, dmReduced, suck);
        BuffAdditionalDamage(damage, aActor, dActor, 0, dfMagic, apMagic, dfPhysics, apPhysics, dmReduced, suck);

        if (OnHitDamage != null)
        {
            OnHitDamage(skillId, aActor, dActor, damage, true, false, false, false);
        }

        return damage;
    }

    /// <summary>
    /// 追加治疗
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="caster"></param>
    /// <param name="atk"></param>
    /// <returns></returns>
    public int AddCure(int skillId, Actor caster, Volume.ATK atk)
    {
        Actor aActor = caster; // 攻
        Actor dActor = Owner; // 防

        float cureReduced = dActor.SkillController.BuffCureReduced();

        int cure = CalculateCure(aActor, dActor, SkillUtility.GetSkillValueNumeric(atk.Meta, aActor, dActor), 0, cureReduced);

        if (OnHitCure != null)
        {
            OnHitCure(skillId, aActor, dActor, cure, true, false, false, false);
        }

        return cure;
    }

    bool Damage(int skillId, DCMeta dc, Actor caster)
    {
        Actor aActor = caster; // 攻
        Actor dActor = Owner; // 防

        // 无敌
        if (dActor.SkillController.BuffInvincible())
        {
            return false;
        }

        // ---------------------------- 攻击方 -------------------------------------------
        float precision = ConvertValue(ActorField.Precision, aActor.GetValue(ActorField.Precision, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.Precision, aActor, dActor));
        float critRate = ConvertValue(ActorField.CritRate, aActor.GetValue(ActorField.CritRate, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.CritRate, aActor, dActor));
        float critDamage = ConvertValue(ActorField.CritDamage, aActor.GetValue(ActorField.CritDamage, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.CritDamage, aActor, dActor));
        float suck = ConvertValue(ActorField.Suck, aActor.GetValue(ActorField.Suck, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.Suck, aActor, dActor));
        float apMagic = ConvertValue(ActorField.APMagic, aActor.GetValue(ActorField.APMagic, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.APMagic, aActor, dActor));
        float apPhysics = ConvertValue(ActorField.APPhysics, aActor.GetValue(ActorField.APPhysics, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.APPhysics, aActor, dActor));

        // ---------------------------- 被攻击方 -------------------------------------------
        float parry = ConvertValue(ActorField.Parry, dActor.GetValue(ActorField.Parry, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.Parry, dActor, aActor));
        float dodge = ConvertValue(ActorField.Dodge, dActor.GetValue(ActorField.Dodge, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.Dodge, dActor, aActor));
        float dfMagic = ConvertValue(ActorField.DFMagic, dActor.GetValue(ActorField.DFMagic, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.DFMagic, dActor, aActor));
        float dfPhysics = ConvertValue(ActorField.DFPhysics, dActor.GetValue(ActorField.DFPhysics, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.DFPhysics, dActor, aActor));
        float dmReduced = ConvertValue(ActorField.DMReduced, dActor.GetValue(ActorField.DMReduced, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.DMReduced, dActor, aActor));
        //float hp = dActor.GetValue(ActorField.HP, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.HP, dActor, aActor);

        // 格挡
        int rand = UnityEngine.Random.Range(1, 10000);
        bool bParry = false;
        if (rand <= parry)
        {
            bParry = true;
            OnTakeEffect(aActor, dActor, MixEffect.Parry, 1);
        }

        // 闪避
        rand = UnityEngine.Random.Range(1, 10000);
        bool bDodge = false;
        if (rand <= (dodge - precision) || BuffAdditionalDodge(dActor))
        {
            bDodge = true;
            OnTakeEffect(aActor, dActor, MixEffect.Dodge, 1);
        }

        // 暴击
        bool crit = false;
        rand = UnityEngine.Random.Range(1, 10000);
        if (rand > critRate)
        {
            critDamage = 0;
        }
        else
        {
            crit = true;
        }

        bool hit = false;
        int damageTotal = 0;

        // ---------------------------------- 计算攻击和防御 --------------------------------------------
        if (!bParry && !bDodge) // 未免疫、未闪避
        {
            for (int i = 0; i < dc.VolumeEx.Atks.Length; ++i)
            {
                Volume.ATK atk = dc.VolumeEx.Atks[i];

                int damage = CalculateDamage(aActor, dActor, atk.NatureEx, SkillUtility.GetSkillValueNumeric(atk.Meta, aActor, dActor), critDamage, dfMagic, apMagic, dfPhysics, apPhysics, dmReduced, suck);
                BuffAdditionalDamage(damage, aActor, dActor, 0, dfMagic, apMagic, dfPhysics, apPhysics, dmReduced, suck);

                damageTotal += damage;
            }

            hit = true;
        }
        else
        {
            hit = false;
        }

        if (OnHitDamage != null)
        {
            OnHitDamage(skillId, aActor, dActor, damageTotal, hit, bDodge, bParry, crit);
        }

        return hit;
    }

    void Cure(int skillId, DCMeta dc, Actor caster)
    {
        Actor aActor = caster; // 攻
        Actor dActor = Owner; // 防
        // ---------------------------- 攻击方 -------------------------------------------
        float critRate = ConvertValue(ActorField.CritRate, aActor.GetValue(ActorField.CritRate, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.CritRate, aActor, dActor));
        float critDamage = ConvertValue(ActorField.CritDamage, aActor.GetValue(ActorField.CritDamage, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.CritDamage, aActor, dActor));

        // ---------------------------- 被攻击方 -------------------------------------------
        float hp = ConvertValue(ActorField.HP, aActor.GetValue(ActorField.HP, true) + SkillUtility.GetSkillValueField(dc.VolumeEx, ActorField.HP, aActor, dActor));
        float cureReduced = dActor.SkillController.BuffCureReduced();

        bool crit = false;
        int cureTotal = 0;
        
        // 未暴击
        float rand = UnityEngine.Random.Range(1, 10000);
        if (rand > critRate)
        {
            critDamage = 0;
        }
        else
        {
            crit = true;
        }
        for (int i = 0; i < dc.VolumeEx.Atks.Length; ++i)
        {
            Volume.ATK atk = dc.VolumeEx.Atks[i];

            int cure = CalculateCure(aActor, dActor, SkillUtility.GetSkillValueNumeric(atk.Meta, aActor, dActor), critDamage, cureReduced);

            cureTotal += cure;
        }

        if (OnHitCure != null)
        {
            OnHitCure(skillId, aActor, dActor, cureTotal, true, false, false, crit);
        }
    }

    void OnDamage(Vector2 bulletPos, HitMotion motion, float forceX, float forceY)
    {
        switch (motion)
        {
            case HitMotion.NatureBump:
                Vector2 self1 = Owner.transform.position;
                Vector2 dir1 = (self1 - bulletPos).normalized;
                Owner.ActorReference.ActorFlyEx.HitTargetMotion(ActorFly.FlyType.Fade, dir1.x * forceX, dir1.y * forceX, 0);
                break;
            case HitMotion.ManualBump:
                Owner.ActorReference.ActorFlyEx.HitTargetMotion(ActorFly.FlyType.Fade, forceX, forceY, 0);
                break;
            case HitMotion.Retreat:
                Owner.ActorReference.ActorFlyEx.HitTargetMotion(ActorFly.FlyType.Distance, forceX, 0, forceY);
                break;
            case HitMotion.Transfer:
                Owner.ActorReference.ActorFlyEx.HitTargetMotion(ActorFly.FlyType.Destination, forceX, 0, forceY);
                break;
        }
        HighLight();
    }

    void OnHit(DCMeta dc, Collider col)
    {
        if(!string.IsNullOrEmpty(dc.HitSound))
        {
            AudioManager.Instance.PlaySound(dc.HitSound);
        }
        if (!string.IsNullOrEmpty(dc.HitEffect))
        {
            Vector3 pos = Owner.transform.position;
            if(col != null)
            {
                pos = Owner.ActorReference.ColliderEx.ClosestPointOnBounds(col.transform.position);
            }
            GameObject effect = EntityPool.Spwan(AssetManage.AM_PathHelper.GetActorEffectFullPathByName(dc.HitEffect), pos) as GameObject;
            if (effect != null)
            {
                EntityPool.Destroy(effect, 1);
            }
        }
        //if (dc.Triggers != null && dc.Triggers.Length > 0 && dc.Triggers[0] != null)
        //{
        //    Owner.SkillController.TriggerPossessorEx.Watch(dc.Triggers[0]);
        //    Owner.SkillController.CheckerPossessorEx.Check(Condition.Probability, UnityEngine.Random.Range(0, 100));
        //}
    }

    void HighLight()
    {
        Owner.ActorReference.ActorRenderEx.HighLight();
    }
}
