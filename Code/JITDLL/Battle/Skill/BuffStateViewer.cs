using UnityEngine;
using System.Collections.Generic;
using ACTOR;
using SKILL;
using BUFF;

public class BuffStateViewer : MonoCompBase
{
    // 方块技能
    public string cubeSkill;
    // 方块策略（1消视为3消，任意消视为3消等）
    public string cubeStrategy;
    // 不可移动
    public bool cantMove;
    // 不可击退
    public bool cantKnockback;
    // 减速
    public float speedCut;
    // 免疫减速
    public bool immuneSpeedCut;
    // 眩晕
    public bool stun;
    // 免疫眩晕
    public bool immuneStun;
    // 免疫buff类型
    public string immuneBuffType;
    // 无敌 （免伤）
    public bool invincible;
    // 护盾
    public float shield;
    // 恢复减免
    public float cureReduced;
    // 荆棘伤害
    public float thornsDamage;
    // 承担队友伤害
    public float shareDamage;
    // 追加闪避（独立判断）
    public float additionalDodge;
    // 追加造成伤害量百分比的伤害
    [System.Serializable]
    public class DamageValue
    {
        public Nature nature;
        public float factor;
    }
    public List<DamageValue> additionalDamage = new List<DamageValue>();
    // 效果量
    public float effectSize;
    // buff持续时间
    public float buffTime;
    // debuff持续时间
    public float debuffTime;
    // 生命上限
    public float hpMax; 
    // 生命值
    public float hp; 
    // 攻击力
    public float atk; 
    // 魔法防御
    public float defenseMagic; 
    // 物理防御
    public float defensePhysics; 
    // 暴击率
    public float critRate; 
    // 暴击伤害
    public float critDamage; 
    // 闪避
    public float dodge; 
    // 命中
    public float precision;
    // 魔法穿透
    public float penetrateMagic;
    // 物理穿透
    public float penetratePhysics; 
    // 急速
    public float speed;
    // 吸血
    public float suck; 
    // 伤害减免
    public float damageReduced; 
    // 格挡
    public float parry; 

    public override void Init(Actor a)
    {
        base.Init(a);
    }

    void Update()
    {
        cubeSkill = Owner.SkillController.BuffCubeSkill();
        cubeStrategy = Owner.SkillController.BuffCubeStrategyName();
        cantMove = Owner.SkillController.BuffCantMove();
        cantKnockback = Owner.SkillController.BuffCantKnockback();
        speedCut = Owner.SkillController.BuffSpeedCut();
        immuneSpeedCut = Owner.SkillController.BuffImmuneSpeedCut();
        stun = Owner.SkillController.BuffStun();
        immuneStun = Owner.SkillController.BuffImmuneStun();
        int buffType = Owner.SkillController.BuffImmuneBuffType();
        immuneBuffType = buffType == -1 ? "None" : ((BuffType)buffType).ToString();
        invincible = Owner.SkillController.BuffInvincible();
        shield = Owner.SkillController.BuffShield();
        cureReduced = Owner.SkillController.BuffCureReduced();
        thornsDamage = Owner.SkillController.BuffThornsDamage();
        shareDamage = Owner.SkillController.BuffShareDamage();
        effectSize = Owner.SkillController.BuffEffectSize();
        buffTime = Owner.SkillController.BuffBuffTime(BuffType.Buff);
        debuffTime = Owner.SkillController.BuffBuffTime(BuffType.DeBuff);
        
        hpMax = Owner.SkillController.BuffAttribute((int)ActorField.HPMax); 
        hp = Owner.SkillController.BuffAttribute((int)ActorField.HP);  
        atk = Owner.SkillController.BuffAttribute((int)ActorField.ATK); 
        defenseMagic = Owner.SkillController.BuffAttribute((int)ActorField.DFMagic); 
        defensePhysics = Owner.SkillController.BuffAttribute((int)ActorField.DFPhysics); 
        critRate = Owner.SkillController.BuffAttribute((int)ActorField.CritRate); 
        critDamage = Owner.SkillController.BuffAttribute((int)ActorField.CritDamage); 
        dodge = Owner.SkillController.BuffAttribute((int)ActorField.Dodge); 
        precision = Owner.SkillController.BuffAttribute((int)ActorField.Precision);
        penetrateMagic = Owner.SkillController.BuffAttribute((int)ActorField.APMagic);
        penetratePhysics = Owner.SkillController.BuffAttribute((int)ActorField.APPhysics); 
        speed = Owner.SkillController.BuffAttribute((int)ActorField.Speed); 
        suck = Owner.SkillController.BuffAttribute((int)ActorField.Suck); 
        damageReduced = Owner.SkillController.BuffAttribute((int)ActorField.DMReduced); 
        parry = Owner.SkillController.BuffAttribute((int)ActorField.Parry);
        additionalDodge = Owner.SkillController.BuffAdditionalDodge();

        additionalDamage.Clear();
        foreach (StateBlackboard.DamageStruct damage in Owner.SkillController.BuffAdditionalDamage())
        {
            DamageValue value = new DamageValue();
            value.nature = (Nature)damage.nature;
            value.factor = damage.factor;
        }
    }
}
