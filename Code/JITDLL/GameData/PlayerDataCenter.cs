using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Network;

namespace DataCenter
{
    public sealed class HeroAttribute
    {
        public float Hp;
        public float Attack;
        public float MagicDefence;
        public float PhysicalDefence;

        public float CrticalRate;
        public float CrticalDamage;
        public float Dodge;
        public float Precision;

        public float MagicPenetration;
        public float PhysicPenetration;
        public float AttackSpeed;
        public float Suck;
        public float DamageReduce;

        public void Reset()
        {
            Hp = 0;
            Attack = 0;
            MagicDefence = 0;
            PhysicalDefence = 0;

            CrticalRate = 0;
            CrticalDamage = 0;
            Dodge = 0;
            Precision = 0;

            MagicPenetration = 0;
            PhysicPenetration = 0;
            AttackSpeed = 0;
            Suck = 0;
            DamageReduce = 0;
        }

        //public static HeroAttribute operator +(HeroAttribute a, HeroAttribute b)
        //{
        //    HeroAttribute result = new HeroAttribute();

        //    result.Reset();

        //    result.Hp = a.Hp + b.Hp;
        //    result.Attack = a.Attack + b.Attack;
        //    result.MagicDefence = a.MagicDefence + b.MagicDefence;
        //    result.PhysicalDefence = a.PhysicalDefence + b.PhysicalDefence;

        //    result.CrticalRate = a.CrticalRate + b.CrticalRate;
        //    result.CrticalDamage = a.CrticalDamage + b.CrticalDamage;
        //    result.Dodge = a.Dodge + b.Dodge;
        //    result.Precision = a.Precision + b.Precision;

        //    result.MagicPenetration = a.MagicPenetration + b.MagicPenetration;
        //    result.PhysicPenetration = a.PhysicPenetration + b.PhysicPenetration;
        //    result.AttackSpeed = a.AttackSpeed + b.AttackSpeed;
        //    result.Suck = a.Suck + b.Suck;
        //    result.DamageReduce = a.DamageReduce + b.DamageReduce;

        //    return result;
        //}

        public HeroAttribute Plus(HeroAttribute other)
        {
            this.Hp += other.Hp;
            this.Attack += other.Attack;
            this.MagicDefence += other.MagicDefence;
            this.PhysicalDefence += other.PhysicalDefence;

            this.CrticalRate += other.CrticalRate;
            this.CrticalDamage += other.CrticalDamage;
            this.Dodge += other.Dodge;
            this.Precision += other.Precision;

            this.MagicPenetration += other.MagicPenetration;
            this.PhysicPenetration += other.PhysicPenetration;
            this.AttackSpeed += other.AttackSpeed;
            this.Suck += other.Suck;
            this.DamageReduce += other.DamageReduce;

            return this;
        }

        public HeroAttribute Minus(HeroAttribute other)
        {
            this.Hp -= other.Hp;
            this.Attack -= other.Attack;
            this.MagicDefence -= other.MagicDefence;
            this.PhysicalDefence -= other.PhysicalDefence;

            this.CrticalRate -= other.CrticalRate;
            this.CrticalDamage -= other.CrticalDamage;
            this.Dodge -= other.Dodge;
            this.Precision -= other.Precision;

            this.MagicPenetration -= other.MagicPenetration;
            this.PhysicPenetration -= other.PhysicPenetration;
            this.AttackSpeed -= other.AttackSpeed;
            this.Suck -= other.Suck;
            this.DamageReduce -= other.DamageReduce;

            return this;
        }

        public void AddByEquip(Equip equip, HeroAttribute baseAttribute)
        {
            Attack += equip.Attack;
            AttackSpeed += equip.AttackSpeed / 100f;

            for (int i = 0; i < equip.ReformList.Count; ++i)
            {
                switch(equip.ReformList[i].ReformType)
                {
                    case (uint)PbCommon.EPropertyType.E_Damage_Add_Value:
                        Attack += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Crit_Damage_Add_Percent:
                        CrticalDamage += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Physical_Penetration_Add_Value:
                        PhysicPenetration += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Magical_Penetration_Add_Value:
                        MagicPenetration += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Damage_Add_Percent:
                        Attack += baseAttribute.Attack * equip.ReformList[i].ReformValue / 100f;
                        break;

                    case (uint)PbCommon.EPropertyType.E_HP_Add_Value:
                        Hp += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Be_Damage_Reduce_Percent:
                        DamageReduce += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Physical_Defense_Add_Value:
                        PhysicalDefence += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Magical_Defense_Add_Value:
                        MagicDefence += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_HP_Add_Percent:
                        Hp += baseAttribute.Hp * equip.ReformList[i].ReformValue / 100f;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Crit_Rate_Add_Percent:
                        CrticalRate += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Attack_Speed_Add_Percent:
                        AttackSpeed += equip.ReformList[i].ReformValue / 100f;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Suck_Rate_Add_Percent:
                        Suck += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Evasion_Rate_Add_Percent:
                        Dodge += equip.ReformList[i].ReformValue;
                        break;

                    case (uint)PbCommon.EPropertyType.E_Hit_Add_Percent:
                        Precision += equip.ReformList[i].ReformValue;
                        break;
                }
            }
        }
    }

    public sealed class Hero
    {
        public uint ServerId;

        public int CsvId;

        public uint Level;

        public uint Exp;

        public uint EnhanceLevel;

        public uint EnhanceExp;

        public Equip Weapon;

        public Equip Ring;

        public uint SkillServerId;

        public bool IsLock;

        public uint SoulFortressTime;

        public bool InExpedition;

        public CSV_b_hero_template HeroCSV;

        /// <summary>
        /// 总属性 
        /// </summary>
        public HeroAttribute TotalAttribute = new HeroAttribute();

        /// <summary>
        /// 基础属性(等级 + 面包) 
        /// </summary>
        public HeroAttribute BaseAttribute = new HeroAttribute();

        /// <summary>
        /// 额外属性 = 总属性 - 基础属性
        /// </summary>
        public HeroAttribute ExtraAttribute = new HeroAttribute();

        /// <summary>
        /// 果实属性 
        /// </summary>
        public HeroAttribute FruitAttribute = new HeroAttribute();

        /// <summary>
        /// 装备属性 
        /// </summary>
        public HeroAttribute EquipmentAttribute = new HeroAttribute();

        public void SyncData(immortaldb.Hero syncHero)
        {
            ServerId = syncHero.id;
            CsvId = (int)syncHero.template_id;
            Level = syncHero.level;
            Exp = syncHero.exp;
            EnhanceLevel = syncHero.enhance_level;
            EnhanceExp = syncHero.enhance_exp;
            Weapon = PlayerDataCenter.GetEquip(syncHero.weapon_id);
            Ring = PlayerDataCenter.GetEquip(syncHero.ring_id);
            SkillServerId = syncHero.skill_id;
            IsLock = syncHero.is_lock != 0;
            SoulFortressTime = syncHero.soul_fortress_time;

            HeroCSV = CSV_b_hero_template.FindData(CsvId);

            UpdateFruitAttribute(syncHero);

            CalculateAttribute();
        }

        public ActorWeaponInfo GetActorWeaponInfo()
        {
            string weaponPrefabPath = GetWeaponPrefabPath();

            if (!string.IsNullOrEmpty(weaponPrefabPath))
            {
                ActorWeaponInfo actorWeaponInfo = new ActorWeaponInfo();

                actorWeaponInfo.prefabPath = weaponPrefabPath;
                actorWeaponInfo.LocalPosition = HeroCSV.WeaponPosition;
                actorWeaponInfo.LocalRotation = HeroCSV.WeaponRotation;

                return actorWeaponInfo;
            }

            return null;
        }

        string GetWeaponPrefabPath()
        {
            if (Weapon != null)
            {
                return CSV_b_equip_template.FindData(Weapon.CsvId).Prefab;
            }
            else
            {
                switch(HeroCSV.School)
                {
                    case (int)PbCommon.ESchoolType.E_School_Sword:
                        return DefaultConfig.GetString("SwordmanDefaultWeapon");

                    case (int)PbCommon.ESchoolType.E_School_Knight:
                        return DefaultConfig.GetString("KnightDefaultWeapon");

                    case (int)PbCommon.ESchoolType.E_School_Archer:
                        return DefaultConfig.GetString("ArcherDefaultWeapon");

                    case (int)PbCommon.ESchoolType.E_School_Hunter:
                        return DefaultConfig.GetString("ShooterDefaultWeapon");

                    case (int)PbCommon.ESchoolType.E_School_Wizard:
                        return DefaultConfig.GetString("MagicianDefaultWeapon");

                    case (int)PbCommon.ESchoolType.E_School_Flamen:
                        return DefaultConfig.GetString("PrisetDefaultWeapon");
                }
            }

            return "";
        }

        public static int Compare(Hero h1, Hero h2)
        {
            CSV_b_hero_template csv1 = CSV_b_hero_template.FindData(h1.CsvId);
            CSV_b_hero_template csv2 = CSV_b_hero_template.FindData(h2.CsvId);

            return (csv1.School * 10000 - (int)h1.Level) - (csv2.School * 10000 - (int)h2.Level);
        }

        public void AddExp(uint addExp)
        {
            CSV_b_hero_template heroCSV = CSV_b_hero_template.FindData(CsvId);
            CSV_b_hero_limit heroLimtCSV = CSV_b_hero_limit.FindData(heroCSV.Star);

            Exp += addExp;
            Level = (uint)CSV_b_level_template.GetLevel(Exp);
            if (Level > heroLimtCSV.MaxLevel)
            {
                Level = (uint)heroLimtCSV.MaxLevel;
                Exp = (uint)CSV_b_level_template.GetLevelTotalExp(Level);
            }

            CalculateAttribute();

            // 图鉴 
            PlayerDataCenter.HandbookData.UpdateHeroHandbook(this);
        }

        public void CalculateAttribute()
        {
            CalculateBaseAttribute();
            CalculateEquipmentAttribute();

            TotalAttribute.Reset();
            TotalAttribute.Plus(BaseAttribute)
                          .Plus(FruitAttribute)
                          .Plus(EquipmentAttribute);


            ExtraAttribute.Reset();
            ExtraAttribute.Plus(TotalAttribute)
                          .Minus(BaseAttribute);

        }

        public void CalculateBaseAttribute()
        {
            CSV_b_hero_template heroCSV = CSV_b_hero_template.FindData(CsvId);
            CSV_c_attribute_template defaultCSV = CSV_c_attribute_template.FindData(heroCSV.AttributeTemplateId);
            CSV_c_attribute_grow growCSV = CSV_c_attribute_grow.FindData(heroCSV.AttributeGrowthId);

            BaseAttribute.Reset();

            BaseAttribute.Hp = (defaultCSV.Hp + (Level - 1) * growCSV.Hp) * (1 + EnhanceLevel * 0.1f);
            BaseAttribute.Attack = (defaultCSV.Attack + (Level - 1) * growCSV.Attack) * (1 + EnhanceLevel * 0.1f);
            BaseAttribute.PhysicalDefence = (defaultCSV.PhysicDefense + (Level - 1) * growCSV.PhysicDefense) * (1 + EnhanceLevel * 0.1f);
            BaseAttribute.MagicDefence = (defaultCSV.MagicDefense + (Level - 1) * growCSV.MagicDefense) * (1 + EnhanceLevel * 0.1f);

            BaseAttribute.CrticalRate = defaultCSV.CrticalRate;
            BaseAttribute.CrticalDamage = defaultCSV.CrticalDamage;
        }

        public void CalculateEquipmentAttribute()
        {
            EquipmentAttribute.Reset();

            if (Weapon != null)
            {
                EquipmentAttribute.AddByEquip(Weapon, BaseAttribute);
            }

            if (Ring != null)
            {
                EquipmentAttribute.AddByEquip(Weapon, BaseAttribute);
            }
        }

        public void UpdateFruitAttribute(immortaldb.Hero hero)
        {
            FruitAttribute.Hp = hero.hp;
            FruitAttribute.Attack = hero.attack;
            FruitAttribute.MagicDefence = hero.magical_defence;
            FruitAttribute.PhysicalDefence = hero.physical_defence;

            FruitAttribute.CrticalRate = hero.crit_rate;
            FruitAttribute.CrticalDamage = hero.crit_damage;
            FruitAttribute.Dodge = hero.evasion_rate;
            FruitAttribute.Precision = hero.hit_rate;

            CalculateAttribute();
        }

        public void UpEquip(uint equipServerId)
        {
            UpEquip(PlayerDataCenter.GetEquip(equipServerId));
        }

        public void UpEquip(Equip equip)
        {
            equip.HeroServerId = ServerId;

            switch (equip.EquipType)
            {
                case (int)PbCommon.EEquipType.E_Equip_Type_Weapon:
                    Weapon = equip;
                    break;

                case (int)PbCommon.EEquipType.E_Equip_Type_Ring:
                    Ring = equip;
                    break;
            }

            CalculateAttribute();
        }

        public void DownEquip(uint equipServerId)
        {
            DownEquip(PlayerDataCenter.GetEquip(equipServerId));
        }

        public void DownEquip(Equip equip)
        {
            equip.HeroServerId = 0;

            switch (equip.EquipType)
            {
                case (int)PbCommon.EEquipType.E_Equip_Type_Weapon:
                    Weapon = null;
                    break;

                case (int)PbCommon.EEquipType.E_Equip_Type_Ring:
                    Ring = null;
                    break;
            }

            CalculateAttribute();
        }

        public Equip GetEquip(int equipType)
        {
            switch (equipType)
            {
                case (int)PbCommon.EEquipType.E_Equip_Type_Weapon:
                    return Weapon;

                case (int)PbCommon.EEquipType.E_Equip_Type_Ring:
                    return Ring;
            }

            return null;
        }

        public bool IsMaxLevel()
        {
            CSV_b_hero_template heroCSV = CSV_b_hero_template.FindData(CsvId);
            CSV_b_hero_limit heroLimtCSV = CSV_b_hero_limit.FindData(heroCSV.Star);

            return Level == heroLimtCSV.MaxLevel;
        }

        public void EquipSpecialSkill(uint groupId, uint level)
        {
            CSV_b_skill_template skillCSV = CSV_b_skill_template.FindData((int)groupId, (int)level);
            SkillServerId = (uint)skillCSV.Id;
        }
    }

    public sealed class Bread
    {
        public uint ServerId;

        public int CsvId;
    }

    public sealed class Equip
    {
        public uint ServerId;

        public int CsvId;

        public int EquipType;

        public bool IsLock;

        public uint HeroServerId;

        public float Attack;

        public float AttackSpeed;

        public List<EquipReform> ReformList = new List<EquipReform>();

        public void SyncData(immortaldb.Equip syncEquip)
        {
            ServerId = syncEquip.id;
            CsvId = (int)syncEquip.template_id;
            IsLock = syncEquip.is_lock != 0;
            HeroServerId = syncEquip.hero_id;

            UpdateEquipReformList(syncEquip.reform_info);

            CSV_b_equip_template equipCSV = CSV_b_equip_template.FindData(CsvId);

            EquipType = equipCSV.EquipType;
            Attack = equipCSV.Attack;
            AttackSpeed = equipCSV.AttackSpeed;
        }

        public void ResetReformCount(uint reformIndex)
        {
            EquipReform equipReform = GetEquipReform(reformIndex);

            equipReform.ReformCount = 0;
        }

        public void AddReformCount(uint reformIndex)
        {
            EquipReform equipReform = GetEquipReform(reformIndex);

            equipReform.ReformCount++;
        }

        public void DownFromHero()
        {
            Hero hero = PlayerDataCenter.GetHero(HeroServerId);
            hero.DownEquip(this);

            HeroServerId = 0;
        }

        public void UpToHero(uint heroServerId)
        {
            Hero hero = PlayerDataCenter.GetHero(heroServerId);
            UpToHero(hero);
        }

        public void UpToHero(Hero hero)
        {
            hero.UpEquip(this);

            HeroServerId = hero.ServerId;
        }

        public EquipReform GetEquipReform(uint index)
        {
            for (int i = 0; i < ReformList.Count; ++i)
            {
                if (ReformList[i].ReformIndex == index)
                {
                    return ReformList[i];
                }
            }

            return null;
        }

        public void UpdateEquipReformList(List<immortaldb.ReformInfo> reformInfoList)
        {
            if (reformInfoList != null)
            {
                for (int i = 0; i < reformInfoList.Count; ++i)
                {
                    UpdateEquipReform(reformInfoList[i]);
                }
            }
        }

        public void ResetEquipReformList(List<gsproto.ReformItem> reformItemList)
        {
            for (int i = 0; i < reformItemList.Count; ++i)
            {
                EquipReform equipReform = GetEquipReform(reformItemList[i].reform_index);
                equipReform.Reset(reformItemList[i].reform_type);
            }
        }

        public void UpdateEquipReform(immortaldb.ReformInfo reformInfo)
        {
            EquipReform equipReform = GetEquipReform(reformInfo.reform_index);
            if (equipReform == null)
            {
                equipReform = new EquipReform();
                ReformList.Add(equipReform);
            }

            equipReform.ReformIndex = reformInfo.reform_index;
            equipReform.ReformType = reformInfo.reform_type;
            equipReform.ReformProperty = reformInfo.reform_property;
            equipReform.ReformValue = reformInfo.reform_value;
            equipReform.ReformCount = reformInfo.reform_count;
            equipReform.IsBigSuccess = reformInfo.is_big_success;
        }

        public void UpdateEquipReform(uint reformIndex, uint reformProperty, float reformValue, bool isBigSuccess)
        {
            EquipReform equipReform = GetEquipReform(reformIndex);

            equipReform.ReformProperty = reformProperty;
            equipReform.ReformValue = reformValue;
            equipReform.IsBigSuccess = isBigSuccess;
        }
    }

    public sealed class EquipReform
    {
        public uint ReformIndex;

        public uint ReformType;

        public uint ReformProperty;

        public float ReformValue;

        public uint ReformCount;

        public bool IsBigSuccess;

        public void Reset(uint newReformType)
        {
            ReformType = newReformType;
            ReformProperty = 0;
            ReformValue = 0;
            ReformCount = 0;
            IsBigSuccess = false;
        }
    }

    public sealed class Fruit
    {
        public uint ServerId;

        public int CsvId;
    }

    public sealed class PassEnterInfo
    {
        public int PassId;

        public List<uint> SkillList = null;

        public bool HasHidden = false;

        public int NormalCoin = 0;

        public int HiddenCoin = 0;

        public int NormalChest = 0;

        public int BossChest = 0;

        public int ActiveChest = 0;
    }

    public sealed class PassOverInfo
    {
        public uint PassResult;

        public uint Coin;

        public Hero DropHero = null;

        public List<DropChestInfo> DropChestList = new List<DropChestInfo>();

        public List<HeroAddExpInfo> HeroAddExpList = new List<HeroAddExpInfo>();

        public GroupAddExpInfo GroupAddExpData = new GroupAddExpInfo();
    }

    public sealed class DropChestInfo
    {
        public PbCommon.EChestType ChestType;

        public PbCommon.EAwardType AwardType;

        public uint AwardValue;
    }

    public sealed class HeroAddExpInfo
    {
        public Hero HeroData;

        public uint StartLevel;
        public uint EndLevel;

        public uint StartExp;
        public uint EndExp;
    }

    public sealed class GroupAddExpInfo
    {
        public uint StartLevel;
        public uint EndLevel;

        public uint StartExp;
        public uint EndExp;
    }

    public sealed class GroupPassiveInfo
    {
        public uint PassiveType;
        public uint PassiveLevel;
        public uint EffectValue;
        public uint CurrentCount;
        public uint ConditionCount;
    }

    public sealed class GroupPassiveLevelUpInfo
    {
        public uint PassiveType;
        public uint StartPassiveLevel;
        public uint StartEffectValue;
        public uint EndPassiveLevel;
        public uint EndEffectValue;
    }

    public sealed class AwardInfo
    {
        public uint AwardType;

        public uint AwardValue;
    }

    public sealed class HeroHandbook
    {
        public int CsvId;

        public int School;

        public int Star;

        public int HeroType;// PbCommon.EHeroType 

        public bool IsBeginer;

        public bool Existed;

        public bool IsReachMaxLevel;

        public void SetInfoFromCsv(CSV_b_hero_template heroCSV)
        {
            CsvId = heroCSV.Id;
            School = heroCSV.School;
            Star = heroCSV.Star;
            HeroType = heroCSV.HeroType;
            IsBeginer = heroCSV.IsHandbookBeginer;
            Existed = false;
        }

        public void UpdateInfo(immortaldb.HeroHandbook heroHandbook)
        {
            Existed = true;
            IsReachMaxLevel = heroHandbook.is_reach_max_level > 0;
        }

        public void UpdateInfo(Hero hero)
        {
            Existed = true;
            if (!IsReachMaxLevel)
            {
                IsReachMaxLevel = hero.IsMaxLevel();
            }
        }
    }

    public sealed class HandbookInfo
    {
        List<HeroHandbook> _heroHandbookList = new List<HeroHandbook>();

        public void ClearHeroHandbook()
        {
            _heroHandbookList.Clear();
        }

        public bool IsHeroReachMaxLevel(int csvId)
        {
            for (int i = 0; i < _heroHandbookList.Count; ++i)
            {
                if (_heroHandbookList[i].CsvId == csvId)
                {
                    return _heroHandbookList[i].IsReachMaxLevel;
                }
            }

            return false;
        }

        public bool IsHeroHandbookExist(int csvId)
        {
            for (int i = 0; i < _heroHandbookList.Count; ++i)
            {
                if (_heroHandbookList[i].Existed &&
                    _heroHandbookList[i].CsvId == csvId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取勇士图鉴中显示的起始者 
        /// </summary>
        /// <param name="school">0全部 1~6各职业</param>
        /// <returns></returns>
        public List<HeroHandbook> GetHeroHandbookBeginer(int school)
        {
            List<HeroHandbook> beginerList = new List<HeroHandbook>();

            for (int i = 0; i < _heroHandbookList.Count; ++i)
            {
                if (_heroHandbookList[i].IsBeginer)
                {
                    if (school > 0)
                    {
                        if (school == _heroHandbookList[i].School)
                        {
                            beginerList.Add(_heroHandbookList[i]);
                        }
                    }
                    else
                    {
                        beginerList.Add(_heroHandbookList[i]);
                    }
                }
            }

            return beginerList;
        }

        /// <summary>
        /// 获取图鉴中英雄个数 
        /// </summary>
        /// <param name="school">0全部,1~6各职业</param>
        /// <returns></returns>
        public int GetHeroHandbookCount(int school)
        {
            int count = 0;

            for (int i = 0; i < _heroHandbookList.Count; ++i)
            {
                if (!_heroHandbookList[i].Existed)
                {
                    continue;
                }

                if (school == 0)
                {
                    count++;
                }
                else
                {
                    if (_heroHandbookList[i].School == school)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// 获取图鉴中英雄个数（满足星级条件）  
        /// </summary>
        /// <param name="school">0全部,1~6各职业</param>
        /// <param name="star">英雄大于等于这个星级</param>
        /// <returns></returns>
        public int GetHeroHandbookCountWithCondition(int school, int star)
        {
            int count = 0;

            for (int i = 0; i < _heroHandbookList.Count; ++i)
            {
                if (!_heroHandbookList[i].Existed)
                {
                    continue;
                }

                if (school == 0)
                {
                    if (_heroHandbookList[i].Star >= star)
                    {
                        count++;
                    }
                }
                else
                {
                    if (_heroHandbookList[i].School == school && _heroHandbookList[i].Star >= star)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public HeroHandbook GetHeroHandbook(int csvId)
        {
            for (int i = 0; i < _heroHandbookList.Count; ++i)
            {
                if (_heroHandbookList[i].CsvId == csvId)
                {
                    return _heroHandbookList[i];
                }
            }

            return null;
        }

        public void ReadDataFromCsv()
        {
            for (int i = 0; i < CSV_b_hero_template.DateCount; ++i)
            {
                HeroHandbook heroHandbook = new HeroHandbook();
                heroHandbook.SetInfoFromCsv(CSV_b_hero_template.GetData(i));

                _heroHandbookList.Add(heroHandbook);
            }
        }

        public void UpdateHeroHandbook(immortaldb.HeroHandbook heroHandbook)
        {
            HeroHandbook newHandbook = GetHeroHandbook((int)heroHandbook.hero_template_id);
            newHandbook.UpdateInfo(heroHandbook);
        }

        public void UpdateHeroHandbooks(List<immortaldb.HeroHandbook> heroHandbooks)
        {
            for (int i = 0; i < heroHandbooks.Count; ++i)
            {
                UpdateHeroHandbook(heroHandbooks[i]);
            }
        }

        public void UpdateHeroHandbook(Hero hero)
        {
            HeroHandbook heroHandbook = GetHeroHandbook(hero.CsvId);
            heroHandbook.UpdateInfo(hero);
        }
    }

    public sealed class SpecialSkillUnit
    {
        public enum SkillState
        {
            /// <summary>
            /// 未获取 
            /// </summary>
            NotAcquired,
            /// <summary>
            /// 已经获取 
            /// </summary>
            Acquired, 
            /// <summary>
            /// 能升级 
            /// </summary>
            CanUpgrade,
        }

        public uint Level;

        public uint MaxLevel;

        public uint GroupId;

        public int School;

        public int ShowSkillId;

        public SkillState State;

        public void UpdateInfo(immortaldb.SkillGItem skillGItem)
        {
            Level = skillGItem.skill_glevel;

            CSV_b_skill_template skillCSV = CSV_b_skill_template.FindData((int)GroupId, (int)Level);
            School = skillCSV.School;
            ShowSkillId = skillCSV.Id;
        }

        public void UpdateInfo(uint level)
        {
            Level = level;

            CSV_b_skill_template skillCSV =CSV_b_skill_template.FindData((int)GroupId, (int)Level);
            School = skillCSV.School;
            ShowSkillId = skillCSV.Id;
        }

        public void CheckState()
        {
            if (Level < MaxLevel - 1)
            {
                CSV_b_skill_template skillCSV;
                if (Level == 0)
                {
                    skillCSV = CSV_b_skill_template.FindData(ShowSkillId);
                }
                else
                {
                    skillCSV = CSV_b_skill_template.FindData((int)GroupId, (int)Level + 1);
                }

                int count = 0;
                if (skillCSV.AcquiredCondition == 0)
                {
                    count = PlayerDataCenter.HandbookData.GetHeroHandbookCountWithCondition(skillCSV.School, skillCSV.AcquiredCondArg1);
                }
                else
                {
                    if (PlayerDataCenter.HandbookData.IsHeroHandbookExist(skillCSV.AcquiredCondArg1))
                    {
                        count = 1;
                    }
                }

                if (count >= skillCSV.AcquiredCondArg2)
                {
                    State = SkillState.CanUpgrade;
                }
                else if (Level == 0)
                {
                    State = SkillState.NotAcquired;
                }
                else
                {
                    State = SkillState.Acquired;
                }
            }
            else // 已经到能升级的最大级 
            {
                State = SkillState.Acquired;
                return;
            }


        }
    }

    public sealed class SpecialSkillInfo
    {
        List<SpecialSkillUnit> _specialSkillList = new List<SpecialSkillUnit>();

        public void ClearSpecialSkill()
        {
            _specialSkillList.Clear();
        }

        /// <summary>
        /// 取得技能列表 
        /// </summary>
        /// <param name="school">0全部,1~6各职业</param>
        /// <returns></returns>
        public List<SpecialSkillUnit> GetSpecialSkillList(int school)
        {
            List<SpecialSkillUnit> list = new List<SpecialSkillUnit>();
            if (school == 0)
            {
                list.AddRange(_specialSkillList);
            }
            else
            {
                for (int i = 0; i < _specialSkillList.Count; ++i)
                {
                    if (_specialSkillList[i].School == school)
                    {
                        list.Add(_specialSkillList[i]);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 把表里1级的都读入 
        /// </summary>
        public void ReadFromCsv()
        {
            List<CSV_b_skill_template.SkillGroupInfo> simleInfoList = CSV_b_skill_template.GetAllSkillGroupInfo();
            for (int i = 0; i < simleInfoList.Count; ++i)
            {
                SpecialSkillUnit skillUnit = new SpecialSkillUnit();

                skillUnit.Level = 0;
                skillUnit.MaxLevel = (uint)simleInfoList[i].MaxLevel;
                skillUnit.GroupId = (uint)simleInfoList[i].GroupId;
                skillUnit.School = simleInfoList[i].School;
                skillUnit.ShowSkillId = simleInfoList[i].Level1SkillId;
                skillUnit.State = SpecialSkillUnit.SkillState.NotAcquired;

                _specialSkillList.Add(skillUnit);
            }
        }

        public SpecialSkillUnit GetSpecialSkill(uint groupId)
        {
            for (int i = 0; i < _specialSkillList.Count; ++i)
            {
                if (_specialSkillList[i].GroupId == groupId)
                {
                    return _specialSkillList[i];
                }
            }

            return null;
        }

        public void UpdateSkillInfos(List<immortaldb.SkillGItem> skillGItemList)
        {
            for (int i = 0; i < skillGItemList.Count; ++i)
            {
                SpecialSkillUnit specialSkillUnit = GetSpecialSkill(skillGItemList[i].skill_gid);
                specialSkillUnit.UpdateInfo(skillGItemList[i]);
            }
        }

        public void UpdateSkillInfo(uint groupId, uint level)
        {
            SpecialSkillUnit specialSkillUnit = GetSpecialSkill(groupId);
            specialSkillUnit.UpdateInfo(level);
            specialSkillUnit.CheckState();
        }

        public void CheckSkillState()
        {
            for (int i = 0; i < _specialSkillList.Count; ++i)
            {
                _specialSkillList[i].CheckState();
            }
        }

        public bool HasSkillCanUpgrade()
        {
            for (int i = 0; i < _specialSkillList.Count; ++i)
            {
                if (_specialSkillList[i].State == SpecialSkillUnit.SkillState.CanUpgrade)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class Task
    {
        public int CsvId;

        public int TaskType;

        public int TaskTarget;

        public uint TaskState;
        /// <summary>
        /// 任务进度 
        /// </summary>
        public uint TaskSchedule;
        /// <summary>
        /// 任务目标参数:关卡id 
        /// </summary>
        public int TaskTargetParam;
        /// <summary>
        /// 任务目标值 
        /// </summary>
        public int TaskTargetValue;
        /// <summary>
        /// 三个任务的位置（1,2,3）,日常和周任务的位置设为0 
        /// </summary>
        public uint TaskPosition;
        /// <summary>
        /// 冷却完成时间, 日常和周任务没有用
        /// </summary>
        public uint CountdownFinishTime;

        public void CopyFrom(immortaldb.Task taskItem)
        {
            CSV_b_task_template taskCSV = CSV_b_task_template.FindData((int)taskItem.task_id);

            CsvId = (int)taskItem.task_id;
            TaskType = taskCSV.Type;
            TaskTarget = taskCSV.Target;
            TaskState = taskItem.task_state;
            TaskSchedule = taskItem.task_schedule;
            TaskTargetParam = taskCSV.TargetParam;
            TaskTargetValue = taskCSV.TargetValue;
            TaskPosition = taskItem.task_pos;
        }

        public Task CloneData()
        {
            Task newTask = new Task();

            newTask.CsvId = CsvId;
            newTask.TaskType = TaskType;
            newTask.TaskTarget = TaskTarget;
            newTask.TaskState = TaskState;
            newTask.TaskSchedule = TaskSchedule;
            newTask.TaskTargetParam = TaskTargetParam;
            newTask.TaskTargetValue = TaskTargetValue;
            newTask.TaskPosition = TaskPosition;

            return newTask;
        }

        public void Update(immortaldb.Task taskItem)
        {
            TaskState = taskItem.task_state;
            TaskSchedule = taskItem.task_schedule;
        }

        public void CheckTracker(int count, int param)
        {
        }

        public int GetTargetWeight()
        {
            if (TaskTarget == (int)PbCommon.ETaskTargetType.E_Target_Daily_Task_Count)
            {
                return -1000;
            }

            return (int)TaskTarget;
        }

        public bool IsTaskType(int taskType)
        {
            return this.TaskType == taskType;
        }

        public bool IsNormalTask()
        {
            if (IsTaskType((int)PbCommon.ETaskType.E_Task_Epic) ||
                IsTaskType((int)PbCommon.ETaskType.E_Task_Urgent) ||
                IsTaskType((int)PbCommon.ETaskType.E_Task_Plot) ||
                IsTaskType((int)PbCommon.ETaskType.E_Task_Loop))
            {
                return true;
            }

            return false;
        }

        public void SetState(uint state)
        {
            TaskState = state;
        }

        public static int ComparePosition(Task t1, Task t2)
        {
            return (int)t1.TaskPosition - (int)t2.TaskPosition;
        }

        public static int CompareTargetWeight(Task t1, Task t2)
        {
            return (int)t1.GetTargetWeight() - (int)t2.GetTargetWeight();
        }

        public static int CompareTaskType(Task t1, Task t2)
        {
            return (int)t1.TaskType - (int)t2.TaskType;
        }
    }

    public sealed class TaskInfo
    {
        List<Task> _taskList = new List<Task>();

        public uint DailyTaskRefreshTime;
        public uint WeeklyTaskRefreshTime;

        List<Task> _trackerList = new List<Task>();

        public Action<Task> OnTaskTracker;
        void RaiseOnTaskTracker(Task task)
        {
            if (OnTaskTracker != null)
            {
                OnTaskTracker(task);
            }
        }

        public void Clear()
        {
            _taskList.Clear();
        }

        public void PrepareTaskTracker()
        {
            _trackerList.Clear();

            List<Task> normalTasks = GetNormalTaskList();
            for (int i = 0; i < normalTasks.Count; ++i)
            {
                _trackerList.Add(normalTasks[i].CloneData());
            }

            _trackerList.Sort(Task.CompareTaskType);
        }

        public void CheckTaskTracker(PbCommon.ETaskTargetType targetType, int count, int param)
        {
            for (int i = 0; i < _trackerList.Count; ++i)
            {
                if (_trackerList[i].TaskState == (uint)PbCommon.ETaskStateType.E_Task_State_Not_Finish &&
                    _trackerList[i].TaskTarget == (int)targetType &&
                    _trackerList[i].TaskTargetParam == param)
                {
                    _trackerList[i].TaskSchedule += (uint)count;
                    if (_trackerList[i].TaskSchedule >= _trackerList[i].TaskTargetValue)
                    {
                        _trackerList[i].TaskSchedule = (uint)_trackerList[i].TaskTargetValue;
                        _trackerList[i].TaskState = (uint)PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award;
                    }

                    RaiseOnTaskTracker(_trackerList[i]);
                }
            }
        }

        public void SetDailyTaskAlarm(Action callback)
        {
            AlarmClock.Instance.AddDailyAlarm(
                DefaultConfig.GetInt("DailyTaskRefreshHour"),
                DefaultConfig.GetInt("DailyTaskRefreshMinute"),
                DefaultConfig.GetInt("DailyTaskRefreshSecond"),
                callback
                );
        }

        public void SetWeeklyTaskAlarm(Action callback)
        {
            AlarmClock.Instance.AddWeeklyAlarm(
                DefaultConfig.GetInt("WeeklyTaskRefreshWeek"),
                DefaultConfig.GetInt("WeeklyTaskRefreshHour"),
                DefaultConfig.GetInt("WeeklyTaskRefreshMinute"),
                DefaultConfig.GetInt("WeeklyTaskRefreshSecond"),
                callback
                );
        }

        public PbCommon.ETaskStateType GetAwardState(List<Task> tasks)
        {
            PbCommon.ETaskStateType result = PbCommon.ETaskStateType.E_Task_State_Draw_Award;

            for (int i = 0; i < tasks.Count; ++i)
            {
                if (tasks[i].TaskState == (uint)PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award)
                {
                    return PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award;
                }
                else if (tasks[i].TaskState == (uint)PbCommon.ETaskStateType.E_Task_State_Not_Finish)
                {
                    result = PbCommon.ETaskStateType.E_Task_State_Not_Finish;
                }
            }

            return result;
        }

        public PbCommon.ETaskStateType GetAllTaskAwardState()
        {
            return GetAwardState(_taskList);
        }

        public PbCommon.ETaskStateType GetDailyTaskAwardState()
        {
            List<Task> tasks = GetDailyTaskList();
            return GetAwardState(tasks);
        }

        public PbCommon.ETaskStateType GetWeeklyTaskAwardState()
        {
            List<Task> tasks = GetWeeklyTaskList();
            return GetAwardState(tasks);
        }

        public PbCommon.ETaskStateType GetTaskAwardStateExceptNormal()
        {
            List<Task> tasks = GetTaskListExceptNormal();
            return GetAwardState(tasks);
        }

        public void AddCountdowns(List<immortaldb.TaskPosCoolTime> taskPosCoolTimes)
        {
            for (int i = 0; i < taskPosCoolTimes.Count; ++i)
            {
                Task task = GetNormalTaskByPosition(taskPosCoolTimes[i].position);
                if (task != null)
                {
                    task.CountdownFinishTime = taskPosCoolTimes[i].cool_finish_time;
                }
            }
        }

        public void AddTasks(List<immortaldb.Task> taskItems)
        {
            for (int i= 0; i< taskItems.Count; ++i)
            {
                AddTask(taskItems[i]);
            }
        }

        public void AddTask(immortaldb.Task taskItem)
        {
            Task task = new Task();

            task.CopyFrom(taskItem);

            _taskList.Add(task);
        }

        public void RemoveTask(int csvId)
        {
            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].CsvId == csvId)
                {
                    _taskList.RemoveAt(i);
                    return;
                }
            }

            return;
        }

        public void RemoveTask(Task task)
        {
            _taskList.Remove(task);
        }

        public void UpdateTasks(List<immortaldb.Task> taskItems)
        {
            for (int i = 0; i < taskItems.Count; ++i)
            {
                UpdateTask(taskItems[i]);
            }
        }

        public void UpdateTask(immortaldb.Task taskItem)
        {
            Task task = GetTask((int)taskItem.task_id);
            if (task == null)
            {
                AddTask(taskItem);
            }
            else
            {
                task.Update(taskItem);
            }
        }

        public Task GetTask(int csvId)
        {
            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].CsvId == csvId)
                {
                    return _taskList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 取得普通任务（右侧3个）列表 
        /// </summary>
        /// <returns></returns>
        public List<Task> GetNormalTaskList()
        {
            List<Task> list = new List<Task>();

            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].IsNormalTask())
                {
                    list.Add(_taskList[i]);
                }
            }

            list.Sort(Task.ComparePosition);

            return list;
        }

        public List<Task> GetTaskListExceptNormal()
        {
            List<Task> list = new List<Task>();

            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (!_taskList[i].IsNormalTask())
                {
                    list.Add(_taskList[i]);
                }
            }

            return list;
        }

        public List<Task> GetDailyTaskList()
        {
            List<Task> list = new List<Task>();

            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].IsTaskType((int)PbCommon.ETaskType.E_Task_Daily))
                {
                    list.Add(_taskList[i]);
                }
            }

            list.Sort(Task.CompareTargetWeight);

            return list;
        }

        public void RemoveDailyTask()
        {
            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].IsTaskType((int)PbCommon.ETaskType.E_Task_Daily))
                {
                    _taskList.RemoveAt(i);
                    i--;
                }
            }
        }

        public List<Task> GetWeeklyTaskList()
        {
            List<Task> list = new List<Task>();

            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].IsTaskType((int)PbCommon.ETaskType.E_Task_Weekly))
                {
                    list.Add(_taskList[i]);
                }
            }

            list.Sort(Task.CompareTargetWeight);

            return list;
        }

        public void RemoveWeeklyTask()
        {
            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].IsTaskType((int)PbCommon.ETaskType.E_Task_Weekly))
                {
                    _taskList.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// 取得普通任务（右侧3个） 
        /// </summary>
        /// <param name="location">在右侧的位置</param>
        /// <returns></returns>
        public Task GetNormalTaskByPosition(uint position)
        {
            for (int i = 0; i < _taskList.Count; ++i)
            {
                if (_taskList[i].IsNormalTask())
                {

                    if (_taskList[i].TaskPosition == position)
                    {
                        return _taskList[i];
                    }
                }
            }

            return null;
        }
    }

    public sealed class Expedition
    {
        public int CsvId;

        public uint Duration;

        public uint FinishTime;

        public List<uint> heroIds = new List<uint>();

        public uint GroupExp;

        public CSV_b_expedition_quest_template expeditionCSV;
        public CSV_b_expedition_random_award extraCSV;
    }

    public sealed class ExpeditionInfo
    {
        List<Expedition> _expeditionList = new List<Expedition>();

        public void Clear()
        {
            _expeditionList.Clear();
        }

        public void SetFinishAlarm(uint serverTime, Action callback)
        {
            for (int i = 0; i < _expeditionList.Count; ++i)
            {
                if (_expeditionList[i].FinishTime > 0 && _expeditionList[i].FinishTime < serverTime)
                {
                    AlarmClock.Instance.AddDateAlarm(_expeditionList[i].FinishTime, callback);
                }
            }
        }

        public void SetHeroInExpedition(List<Hero> heroList)
        {
            for (int i = 0; i < heroList.Count; ++i)
            {
                heroList[i].InExpedition = IsHeroInExpedition(heroList[i].ServerId);
            }
        }

        public bool HasFinishExpedition()
        {
            for (int i = 0; i < _expeditionList.Count; ++i)
            {
                if (_expeditionList[i].FinishTime >= PlayerDataCenter.ServerTime)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Expedition> GetExpeditionList()
        {
            return _expeditionList;
        }

        public Expedition GetExpedition(int csvId)
        {
            for (int i = 0; i < _expeditionList.Count; ++i)
            {
                if (_expeditionList[i].CsvId == csvId)
                {
                    return _expeditionList[i];
                }
            }

            return null;
        }

        public void RemoveExpedition(int csvId)
        {
            for (int i = 0; i < _expeditionList.Count; ++i)
            {
                if (_expeditionList[i].CsvId == csvId)
                {
                    _expeditionList.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveExpedition(Expedition expedition)
        {
            _expeditionList.Remove(expedition);
        }

        public void AddExpeditions(List<immortaldb.ExpeditionUnit> expeditionUnits)
        {
            for (int i = 0; i < expeditionUnits.Count; ++i)
            {
                AddExpedition(expeditionUnits[i]);
            }
        }

        public void AddExpedition(immortaldb.ExpeditionUnit expeditionUnit)
        {
            Expedition newExpedition = new Expedition();

            CSV_b_expedition_quest_template expeditionCSV = CSV_b_expedition_quest_template.FindData((int)expeditionUnit.expedition_quest_id);

            if (expeditionCSV != null)
            {
                newExpedition.CsvId = (int)expeditionUnit.expedition_quest_id;
                newExpedition.Duration = (uint)expeditionCSV.ExpeditionTime;
                if (expeditionUnit.expedition_begin_time > 0)
                {
                    newExpedition.FinishTime = expeditionUnit.expedition_begin_time + newExpedition.Duration;
                }
                newExpedition.GroupExp = (uint)expeditionCSV.HeroGroupExp;

                newExpedition.heroIds.AddRange(expeditionUnit.expedition_hero_ids);

                newExpedition.expeditionCSV = expeditionCSV;
                newExpedition.extraCSV = CSV_b_expedition_random_award.FindData((int)expeditionUnit.expedition_award_id);

                _expeditionList.Add(newExpedition);
            }
        }

        public bool IsHeroInExpedition(uint heroServerId)
        {
            for (int i = 0; i < _expeditionList.Count; ++i)
            {
                for (int n = 0; n < _expeditionList[i].heroIds.Count; ++n)
                {
                    if (_expeditionList[i].heroIds[n] == heroServerId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsHeroesInExpedition(List<uint> heroServerIds)
        {
            for (int i = 0; i < heroServerIds.Count; ++i)
            {
                if (IsHeroInExpedition(heroServerIds[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public List<Hero> GetFreeHeroList()
        {
            List<Hero> list = new List<Hero>();

            for (int i = 0; i < PlayerDataCenter.HeroList.Count; ++i)
            {
                if (PlayerDataCenter.HeroList[i].HeroCSV.Star >= 4 &&
                    !PlayerDataCenter.HeroList[i].InExpedition)
                {
                    list.Add(PlayerDataCenter.HeroList[i]);
                }
            }

            list.Sort(Hero.Compare);

            return list;
        }

        bool IsHeroMatchCondition(Hero hero, ExpeditionAwardCondition condition)
        {
            CSV_b_hero_template heroCSV = CSV_b_hero_template.FindData(hero.CsvId);

            switch(condition.ConditionType)
            {
                case (int)PbCommon.EExpeditionConditionType.E_Condition_School:
                    return heroCSV.School == condition.ConditionValue;

                case (int)PbCommon.EExpeditionConditionType.E_Condition_Hero_type:
                    return heroCSV.HeroType == condition.ConditionValue;

                case (int)PbCommon.EExpeditionConditionType.E_Condition_Hero_Sex:
                    return heroCSV.Sex == condition.ConditionValue;

                case (int)PbCommon.EExpeditionConditionType.E_Condition_Hero_Star:
                    return heroCSV.Star >= condition.ConditionValue;

                case (int)PbCommon.EExpeditionConditionType.E_Condition_Hero_Level:
                    return hero.Level >= condition.ConditionValue;

                case (int)PbCommon.EExpeditionConditionType.E_Condition_Hero_Nationality:
                    return heroCSV.Nationality == condition.ConditionValue;
            }

            return false;
        }

        /// <summary>
        /// 找出英雄能满足条件列表中的哪些项 
        /// </summary>
        /// <param name="hero">英雄</param>
        /// <param name="conditionList">条件列表</param>
        /// <returns>满足的条件</returns>
        public List<ExpeditionAwardCondition> HeroMatchConditions(Hero hero, List<ExpeditionAwardCondition> conditionList)
        {
            conditionList.Sort(ExpeditionAwardCondition.Compare);

            List<ExpeditionAwardCondition> list = new List<ExpeditionAwardCondition>();
            for (int i = 0; i < conditionList.Count; ++i)
            {
                if (list.Find(x => x.ConditionType == conditionList[i].ConditionType) == null &&
                    IsHeroMatchCondition(hero, conditionList[i]))
                {
                    list.Add(conditionList[i]);
                }
            }

            return list;
        }

        void FindHeroMaxMatchConditions(List<Hero> heroList, List<ExpeditionAwardCondition> conditionList, out Hero hero, out List<ExpeditionAwardCondition> matchConditionList)
        {
            hero = heroList[heroList.Count - 1];
            matchConditionList = new List<ExpeditionAwardCondition>();

            List<ExpeditionAwardCondition> current = null; ;
            for (int i = 0; i < heroList.Count; ++i)
            {
                current = HeroMatchConditions(heroList[i], conditionList);

                if (current.Count > matchConditionList.Count)
                {
                    matchConditionList = current;
                    hero = heroList[i];

                    current = null;
                }
            }
        }

        /// <summary>
        /// 从英雄列表中选出最满足条件列表的英雄 
        /// </summary>
        /// <param name="heroList">英雄列表</param>
        /// <param name="conditionList">条件列表</param>
        /// <param name="heroCount">英雄个数</param>
        /// <param name="findHeroList">找出的英雄</param>
        /// <param name="matchConditionList">满足的条件</param>
        public void FindHeroesMatchConditions(List<Hero> heroList, List<ExpeditionAwardCondition> conditionList, int heroCount, out List<Hero> findHeroList, out List<ExpeditionAwardCondition> matchConditionList)
        {
            findHeroList = new List<Hero>();
            matchConditionList = new List<ExpeditionAwardCondition>();

            List<Hero> cacheHeroList = new List<Hero>();
            cacheHeroList.AddRange(heroList);
            List<ExpeditionAwardCondition> cacheConditionList = new List<ExpeditionAwardCondition>();
            cacheConditionList.AddRange(conditionList);

            while (cacheHeroList.Count > 0 && findHeroList.Count < heroCount)
            {
                Hero hero = null;
                List<ExpeditionAwardCondition> matchList = null;

                FindHeroMaxMatchConditions(cacheHeroList, cacheConditionList, out hero, out matchList);

                findHeroList.Add(hero);
                matchConditionList.AddRange(matchList);

                cacheHeroList.Remove(hero);
                cacheConditionList.RemoveAll(x => matchList.Contains(x));
            }
        }
    }

    public sealed class TeamInfo
    {
        public uint CreateType;

        public uint TeamType;

        public string TeamName;

        public uint LeaderId;

        public List<uint> Members = new List<uint>();

        public uint Goddess;
    }

    public sealed class TeamCollectionInfo
    {
        List<TeamInfo> _teamList = new List<TeamInfo>();

        public void Clear()
        {
            _teamList.Clear();
        }

        public void AddTeam(TeamInfo teamInfo)
        {
            _teamList.Add(teamInfo);
        }

        public void AddTeam(immortaldb.Team team)
        {
            TeamInfo teamInfo = new TeamInfo();

            teamInfo.CreateType = team.team_create_type;
            teamInfo.TeamType = team.team_type;
            teamInfo.TeamName = team.team_name;
            teamInfo.LeaderId = team.leader_id;
            teamInfo.Members.AddRange(team.team_members);
            teamInfo.Goddess = team.team_goddess;

            _teamList.Add(teamInfo);
        }

        public void AddTeams(List<immortaldb.Team> teams)
        {
            for (int i = 0; i < teams.Count; ++i)
            {
                AddTeam(teams[i]);
            }
        }

        public void SetMissionTeam(uint leaderId, List<uint> members, uint goddess)
        {
            TeamInfo old = GetMissionTeam();

            old.LeaderId = leaderId;
            old.Members.Clear();
            old.Members.AddRange(members);
            old.Goddess = goddess;
        }

        public void SetMissionTeam(TeamInfo teamInfo)
        {
            TeamInfo old = GetMissionTeam();

            old.TeamName = teamInfo.TeamName;
            old.LeaderId = teamInfo.LeaderId;
            old.Members.Clear();
            old.Members.AddRange(teamInfo.Members);
            old.Goddess = teamInfo.Goddess;
        }

        public TeamInfo GetMissionTeam()
        {
            for (int i = 0; i < _teamList.Count; ++i)
            {
                if (_teamList[i].TeamType == (int)PbCommon.ETeamType.E_Team_NormalPass)
                {
                    return _teamList[i];
                }
            }

            TeamInfo teamInfo = new TeamInfo();

            teamInfo.CreateType = (uint)PbCommon.ETeamCreateType.E_Team_Create_System;
            teamInfo.TeamType = (uint)PbCommon.ETeamType.E_Team_NormalPass;

            _teamList.Add(teamInfo);

            return teamInfo;
        }
    }

    public sealed class Goddess
    {
        public int csvId;

        public uint LockChapter;

        public static int Compare(Goddess g1, Goddess g2)
        {
            return g1.csvId - g2.csvId;
        }
    }

    public sealed class GoddessInOutInfo
    {
        public int CsvId;

        /// <summary>
        /// 0入队,1离队
        /// </summary>
        public int inOut;
    }

    public sealed class GoddessInfo
    {
        List<Goddess> _goddessList = new List<Goddess>();

        List<GoddessInOutInfo> _goddessInOutList = new List<GoddessInOutInfo>();

        public void ClearGoddessInOut()
        {
            _goddessInOutList.Clear();
        }

        public GoddessInOutInfo GetGoddessInOut()
        {
            GoddessInOutInfo ret = null;
            if (_goddessInOutList.Count > 0)
            {
                ret = _goddessInOutList[0];
                _goddessInOutList.RemoveAt(0);
            }

            return ret;
        }

        void AddGoddessInOut(int csvId, int inOut)
        {
            GoddessInOutInfo goddessInOutInfo = null;

            for (int i = 0; i < _goddessInOutList.Count; ++i)
            {
                if (_goddessInOutList[i].CsvId == csvId)
                {
                    goddessInOutInfo = _goddessInOutList[i];
                }
            }

            if (goddessInOutInfo == null)
            {
                goddessInOutInfo = new GoddessInOutInfo();

                goddessInOutInfo.CsvId = csvId;

                _goddessInOutList.Add(goddessInOutInfo);
            }

            goddessInOutInfo.inOut = inOut;
        }

        public void Clear()
        {
            _goddessList.Clear();
        }

        public List<Goddess> GetGoddessList()
        {
            _goddessList.Sort(Goddess.Compare);

            return _goddessList;
        }

        public Goddess GetGoddess(int csvId)
        {
            for (int i = 0; i < _goddessList.Count; ++i)
            {
                if (_goddessList[i].csvId == csvId)
                {
                    return _goddessList[i];
                }
            }

            return null;
        }

        public void UpdateGoddesses(List<immortaldb.Goddess> goddesses)
        {
            for (int i = 0; i < goddesses.Count; ++i)
            {
                Goddess goddess = GetGoddess((int)goddesses[i].template_id);
                if (goddess == null)
                {
                    AddGoddess(goddesses[i]);

                    AddGoddessInOut((int)goddesses[i].template_id, goddesses[i].lock_chapter == 0 ? 0 : 1);
                }
                else
                {
                    uint tempLockChapter = goddess.LockChapter;

                    goddess.csvId = (int)goddesses[i].template_id;
                    goddess.LockChapter = goddesses[i].lock_chapter;

                    if (tempLockChapter == 0 && goddess.LockChapter > 0)
                    {
                        AddGoddessInOut(goddess.csvId, 1);
                    } 
                    else if (tempLockChapter > 0 && goddess.LockChapter == 0)
                    {
                        AddGoddessInOut(goddess.csvId, 0);
                    }
                }
            }
        }

        public void AddGoddess(immortaldb.Goddess goddess)
        {
            Goddess newGoddess = new Goddess();

            newGoddess.csvId = (int)goddess.template_id;
            newGoddess.LockChapter = goddess.lock_chapter;

            _goddessList.Add(newGoddess);
        }

        public void AddGoddesses(List<immortaldb.Goddess> goddesses)
        {
            for (int i = 0; i < goddesses.Count; ++i)
            {
                AddGoddess(goddesses[i]);
            }
        }
    }

    public sealed class PlayerDataCenter
    {
        public static string Account;

        public static string SessionId;

        public static int Aid;

        public static int Sid;

        public static ulong Gid;

        public static byte[] UserToken;

        // 临时先用 
        public static string DeviceId
        {
            get
            {
                return SystemInfo.deviceUniqueIdentifier;
            }
        }

        public static string Nickname = "";

        public static ulong RoleId;

        static uint _serverTime = 0;

        static float _serverTimeStart = 0;
        public static uint ServerTime
        {
            private set
            {
                _serverTime = value;
                _serverTimeStart = Time.realtimeSinceStartup;
            }
            get
            {
                return _serverTime + (uint)Mathf.FloorToInt(Time.realtimeSinceStartup - _serverTimeStart - 0.001f);
            }
        }

        /// <summary>
        /// 勇士团等级 
        /// </summary>
        public static uint Level;
        /// <summary>
        /// 勇士团经验 
        /// </summary>
        public static uint Exp;
        /// <summary>
        /// 金币 
        /// </summary>
        public static uint Coin;
        /// <summary>
        /// 体力 
        /// </summary>
        public static uint Stamina;
        /// <summary>
        /// 体力上限 
        /// </summary>
        public static uint MaxStamina;
        /// <summary>
        /// 体力更新时间 
        /// </summary>
        public static uint StaminaUpTime;
        /// <summary>
        /// 体力恢复时间间隔(秒) 
        /// </summary>
        public static uint StaminaRecoverInterval = 360;
        /// <summary>
        /// 钻石 
        /// </summary>
        public static uint Diamond;
        /// <summary>
        /// 荣誉 
        /// </summary>
        public static uint Honor;
        /// <summary>
        /// 铁  
        /// </summary>
        public static uint Iron;
        /// <summary>
        /// 水晶粉末 
        /// </summary>
        public static uint CrystalPowder;
        /// <summary>
        /// 水晶碎片 
        /// </summary>
        public static uint CrystalPiece;
        /// <summary>
        /// 水晶结晶 
        /// </summary>
        public static uint CrystalRime;
        /// <summary>
        /// 属性重置券 
        /// </summary>
        public static uint ResetTicket;
        /// <summary>
        /// 地牢钥匙 
        /// </summary>
        public static uint DungeonKey;
        /// <summary>
        /// 地牢钥匙更新时间 
        /// </summary>
        public static uint DungeonKeyUpTime;
        /// <summary>
        /// 地图碎片 
        /// </summary>
        public static uint MapPiece;
        /// <summary>
        /// 竞技场券 
        /// </summary>
        public static uint ArenaTicket;
        /// <summary>
        /// 竞技场券更新时间 
        /// </summary>
        public static uint ArenaTicketUpTime;

        /// <summary>
        /// 代表英雄 
        /// </summary>
        public static uint RepresentHero;

        public static uint MaxHeroCount;

        public static List<Hero> HeroList = new List<Hero>();

        public static uint MaxBreadCount;

        public static List<Bread> BreadList = new List<Bread>();

        public static uint MaxWeaponCount;

        public static uint MaxRingCount;

        public static uint MaxFruitCount;

        public static List<Equip> EquipList = new List<Equip>();

        public static List<Equip> WeaponList = new List<Equip>();

        public static List<Equip> RingList = new List<Equip>();

        public static List<Fruit> FruitList = new List<Fruit>();

        /// <summary>
        /// 面包烘焙完成时间,0代表没有 
        /// </summary>
        public static uint BakeriesFinishTime;

        /// <summary>
        /// 面包烘焙类型,0代表没有 
        /// </summary>
        public static uint BakeriesType;

        public static PassEnterInfo EnterLevelData = new PassEnterInfo();

        public static PassOverInfo OverLevelData = new PassOverInfo();

        public static List<GroupPassiveInfo> GroupPassiveList = new List<GroupPassiveInfo>();

        public static List<GroupPassiveLevelUpInfo> GroupPassiveLevelUpList = new List<GroupPassiveLevelUpInfo>();


        public static HandbookInfo HandbookData = new HandbookInfo();

        public static SpecialSkillInfo SpecialSkillData = new SpecialSkillInfo();

        public static TaskInfo TaskData = new TaskInfo();

        public static ExpeditionInfo ExpeditionData = new ExpeditionInfo();

        public static TeamCollectionInfo TeamCollectionData = new TeamCollectionInfo();

        public static GoddessInfo GoddessData = new GoddessInfo();

        #region member func

        static System.DateTime GetNowTime()
        {
            return TimeFormater.GetDateTime(ServerTime);
        }

        static void AddGroupPassive(List<gsproto.HeroGroupEffectInfo> infoList)
        {
            for (int i = 0; i < infoList.Count; ++i)
            {
                GroupPassiveInfo groupPassive = new GroupPassiveInfo();

                groupPassive.PassiveType = infoList[i].effect_type;
                groupPassive.PassiveLevel = infoList[i].effect_level;
                groupPassive.EffectValue = infoList[i].effect_value;
                groupPassive.CurrentCount = infoList[i].update_current_value;
                groupPassive.ConditionCount = infoList[i].update_required_value;

                GroupPassiveList.Add(groupPassive);
            }
        }

        public static GroupPassiveInfo GetGroupPassiveInfo(uint passiveType)
        {
            for (int i = 0; i < GroupPassiveList.Count; ++i)
            {
                if (GroupPassiveList[i].PassiveType == passiveType)
                {
                    return GroupPassiveList[i];
                }
            }

            return null;
        }

        static void UpdateGroupPassiveList(List<gsproto.HeroGroupEffectInfo> infoList)
        {
            for (int i = 0 ;i < infoList.Count; ++i)
            {
                UpdateGroupPassive(infoList[i]);
            }
        }

        static void UpdateGroupPassive(gsproto.HeroGroupEffectInfo info)
        {
            GroupPassiveInfo groupPassive = GetGroupPassiveInfo(info.effect_type);

            if (groupPassive.PassiveLevel != info.effect_level)
            {
                GroupPassiveLevelUpInfo levelUp = GetGroupPassiveLevelUpInfo(info.effect_type);
                if (levelUp == null)
                {
                    levelUp = new GroupPassiveLevelUpInfo();

                    levelUp.PassiveType = groupPassive.PassiveType;
                    levelUp.StartPassiveLevel = groupPassive.PassiveLevel;
                    levelUp.StartEffectValue = groupPassive.EffectValue;

                    GroupPassiveLevelUpList.Add(levelUp);
                }

                levelUp.EndPassiveLevel = info.effect_level;
                levelUp.EndEffectValue = info.effect_value;

                switch (info.effect_type)
                {
                    case (uint)PbCommon.EHeroGroupEffectType.E_HeroGroupEffect_IncStaLim:
                        AdjustMaxStamina();
                        break;

                    case (uint)PbCommon.EHeroGroupEffectType.E_HeroGroupEffect_RedStaRecTime:
                        AdjustStaminaRecoverInterval();
                        break;
                }
            }

            groupPassive.PassiveLevel = info.effect_level;
            groupPassive.EffectValue = info.effect_value;
            groupPassive.CurrentCount = info.update_current_value;
            groupPassive.ConditionCount = info.update_required_value;

            RaiseOnGroupPassiveUpdate(groupPassive);
        }

        static GroupPassiveLevelUpInfo GetGroupPassiveLevelUpInfo(uint passiveType)
        {
            for (int i = 0; i < GroupPassiveLevelUpList.Count; ++i)
            {
                if (GroupPassiveLevelUpList[i].PassiveType == passiveType)
                {
                    return GroupPassiveLevelUpList[i];
                }
            }

            return null;
        }

        static void AddGroupExp(uint addExp)
        {
            Exp += addExp;
            Level = (uint)CSV_b_hero_group_template.GetLevel(Exp);
            if (Level >= CSV_b_hero_group_template.DateCount)
            {
                Level = (uint)CSV_b_hero_group_template.DateCount;
                Exp = (uint)CSV_b_hero_group_template.GetLevelTotalExp(Level);
            }

            AdjustMaxStamina();
        }

        static void AdjustMaxStamina()
        {
            MaxStamina = (uint)CSV_b_hero_group_template.FindData((int)Level).Stamina;
            GroupPassiveInfo groupPassive = GetGroupPassiveInfo((uint)PbCommon.EHeroGroupEffectType.E_HeroGroupEffect_IncStaLim);
            if (groupPassive != null)
            {
                MaxStamina += groupPassive.EffectValue;
            }
        }

        public static void AdjustStaminaRecoverInterval()
        {
            StaminaRecoverInterval = (uint)DefaultConfig.GetInt("StaminaRecoverSpeed") * ConstDefine.SECOND_PER_MINUTE;
            GroupPassiveInfo groupPassive = GetGroupPassiveInfo((uint)PbCommon.EHeroGroupEffectType.E_HeroGroupEffect_RedStaRecTime);
            if (groupPassive != null)
            {
                StaminaRecoverInterval -= groupPassive.EffectValue * ConstDefine.SECOND_PER_MINUTE;
            }
        }

        public static Hero GetHero(uint serverId)
        {
            for (int i = 0; i < HeroList.Count; ++i)
            {
                if (HeroList[i].ServerId == serverId)
                {
                    return HeroList[i];
                }
            }

            return null;
        }

        static void AddHero(immortaldb.Hero hero)
        {
            Hero newHero = new Hero();

            newHero.SyncData(hero);

            HeroList.Add(newHero);

            // 图鉴 
            HandbookData.UpdateHeroHandbook(newHero);
        }

        static void AddHeroes(List<immortaldb.Hero> heroes)
        {
            for (int i = 0; i < heroes.Count; ++i)
            {
                AddHero(heroes[i]);
            }
        }

        static void RemoveHero(uint serverId)
        {
            for (int i = 0; i < HeroList.Count; ++i)
            {
                if (HeroList[i].ServerId == serverId)
                {
                    HeroList.RemoveAt(i);
                    return;
                }
            }
        }

        static void RemoveHeroes(List<uint> heroServerIds)
        {
            for (int i = 0; i < heroServerIds.Count; ++i)
            {
                RemoveHero(heroServerIds[i]);
            }
        }

        public static Bread GetBread(uint serverId)
        {
            for (int i = 0; i < BreadList.Count; ++i)
            {
                if (BreadList[i].ServerId == serverId)
                {
                    return BreadList[i];
                }
            }

            return null;
        }

        static void AddBread(immortaldb.Bread bread)
        {
            AddBread((int)bread.template_id, bread.id);
        }

        public static immortaldb.Hero GetSyncDataHero(gsproto.DataSync dataSync)
        {
            if (dataSync.heros != null && dataSync.heros.Count != 0)
            {
                return dataSync.heros[0];
            }

            return null;
        }

        public static List<immortaldb.Bread> GetSyncDataHeroBreadList(gsproto.DataSync dataSync)
        {
            if (dataSync.breads != null)
            {
                return dataSync.breads;
            }

            return null;
        }

        static void SyncPlayerData(gsproto.DataSync dataSync)
        {
            if (dataSync == null)
            {
                return;
            }

            if (dataSync.breads != null)
                SyncBreads(dataSync.breads);

            if (dataSync.equips != null)
                SyncEquips(dataSync.equips);

            if (dataSync.fruits != null)
                SyncFruits(dataSync.fruits);

            if (dataSync.hero_group_effect_list != null &&
                dataSync.hero_group_effect_list.hero_group_effects != null)
                SyncHeroGroupEffect(dataSync.hero_group_effect_list.hero_group_effects);

            if (dataSync.tasks != null)
                SyncTasks(dataSync.tasks);

            if (dataSync.heros != null)
                SyncHeroes(dataSync.heros);

            if (dataSync.properties != null)
            {
                SyncProperties(dataSync.properties);
                RaiseOnPropertyChange();
            }
        }

        static void SyncBreads(List<immortaldb.Bread> syncBreadList)
        {
            AddBreads(syncBreadList);
        }

        static void SyncEquips(List<immortaldb.Equip> syncEquipList)
        {
            for (int i = 0; i < syncEquipList.Count; ++i)
            {
                SyncEquip(syncEquipList[i]);
            }
        }

        static void SyncEquip(immortaldb.Equip syncEquip)
        {
            Equip equip = GetEquip(syncEquip.id);
            if (equip == null)
            {
                AddEquip(syncEquip);
            }
            else
            {
                equip.SyncData(syncEquip);
            }
        }

        static void SyncHeroes(List<immortaldb.Hero> syncHeroList)
        {
            for (int i = 0; i < syncHeroList.Count; ++i)
            {
                SyncHero(syncHeroList[i]);
            }
        }

        static void SyncHero(immortaldb.Hero syncHero)
        {
            Hero hero = GetHero(syncHero.id);
            if (hero == null)
            {
                AddHero(syncHero);
            }
            else
            {
                hero.SyncData(syncHero);

                // 图鉴 
                HandbookData.UpdateHeroHandbook(hero);
            }
        }

        static void SyncFruits(List<immortaldb.Fruit> syncFruitList)
        {
            AddFruits(syncFruitList);
        }

        static void SyncHeroGroupEffect(List<gsproto.HeroGroupEffectInfo> syncHeroGroupEffectList)
        {
            UpdateGroupPassiveList(syncHeroGroupEffectList);
        }

        static void SyncTasks(List<immortaldb.Task> syncTaskList)
        {
            TaskData.UpdateTasks(syncTaskList);
        }

        static void SyncProperties(List<gsproto.Property> syncPropertyList)
        {
            for (int i = 0; i < syncPropertyList.Count; ++i)
            {
                SyncProperty(syncPropertyList[i]);
            }
        }

        static void SyncProperty(gsproto.Property syncProperty)
        {
            switch (syncProperty.type)
            {
                case (uint)PbCommon.EAwardType.E_Award_Type_Gold_Coin:
                    Coin = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Type_Diamond:
                    Diamond = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Map_Piece:
                    MapPiece = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Dungeon_Key:
                    DungeonKey = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Iron:
                    Iron = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Crystal_Powder:
                    CrystalPowder = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Crystal_Piece:
                    CrystalPiece = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Crystal_Rime:
                    CrystalRime = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Reset_Ticket:
                    ResetTicket = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Arena_Ticket:
                    ArenaTicket = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Honor:
                    Honor = syncProperty.value;
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Stamina:
                    Stamina = syncProperty.value;
                    break;
            }
        }

        static void AddBread(int csvId, uint serverId)
        {
            Bread newBread = new Bread();

            newBread.ServerId = serverId;
            newBread.CsvId = csvId;

            BreadList.Add(newBread);
        }

        static void AddBreads(List<immortaldb.Bread> breads)
        {
            for (int i = 0; i < breads.Count; ++i)
            {
                AddBread(breads[i]);
            }
        }

        static void RemoveBread(uint serverId)
        {
            for (int i = 0; i < BreadList.Count; ++i)
            {
                if (BreadList[i].ServerId == serverId)
                {
                    BreadList.RemoveAt(i);
                    return;
                }
            }
        }

        static void RemoveBreads(List<uint> serverIds)
        {
            for (int i = 0; i < serverIds.Count; ++i)
            {
                RemoveBread(serverIds[i]);
            }
        }

        public static Equip GetEquip(uint serverId)
        {
            for (int i = 0; i < EquipList.Count; ++i)
            {
                if (EquipList[i].ServerId == serverId)
                {
                    return EquipList[i];
                }
            }

            return null;
        }

        static void AddEquip(immortaldb.Equip equip)
        {
            Equip newEquip = new Equip();

            newEquip.SyncData(equip);

            EquipList.Add(newEquip);

            CSV_b_equip_template equipCSV = CSV_b_equip_template.FindData(newEquip.CsvId);
            switch(equipCSV.EquipType)
            {
                case (int)PbCommon.EEquipType.E_Equip_Type_Weapon:
                    WeaponList.Add(newEquip);
                    break;

                case (int)PbCommon.EEquipType.E_Equip_Type_Ring:
                    RingList.Add(newEquip);
                    break;
            }
        }

        static void AddEquip(int csvId, uint serverId)
        {
            Equip newEquip = new Equip();

            newEquip.ServerId = serverId;
            newEquip.CsvId = csvId;
            newEquip.IsLock = false;

            EquipList.Add(newEquip);

            CSV_b_equip_template equipCSV = CSV_b_equip_template.FindData(newEquip.CsvId);
            switch (equipCSV.EquipType)
            {
                case (int)PbCommon.EEquipType.E_Equip_Type_Weapon:
                    WeaponList.Add(newEquip);
                    break;

                case (int)PbCommon.EEquipType.E_Equip_Type_Ring:
                    RingList.Add(newEquip);
                    break;
            }

            newEquip.EquipType = equipCSV.EquipType;
            newEquip.Attack = equipCSV.Attack;
            newEquip.AttackSpeed = equipCSV.AttackSpeed;
        }

        static void AddEquips(List<immortaldb.Equip> equips)
        {
            for (int i = 0; i < equips.Count; ++i)
            {
                AddEquip(equips[i]);
            }
        }

        static void RemoveEquip(uint serverId)
        {
            for (int i = 0; i < EquipList.Count; ++i)
            {
                if (EquipList[i].ServerId == serverId)
                {
                    WeaponList.Remove(EquipList[i]);
                    RingList.Remove(EquipList[i]);

                    EquipList.RemoveAt(i);
                    return;
                }
            }
        }

        static void RemoveEquips(List<uint> serverIds)
        {
            for (int i = 0; i < serverIds.Count; ++i)
            {
                RemoveEquip(serverIds[i]);
            }
        }

        static void AddFruits(List<immortaldb.Fruit> fruits)
        {
            for (int i = 0; i < fruits.Count; ++i)
            {
                AddFruit(fruits[i]);
            }
        }

        static void AddFruit(immortaldb.Fruit fruit)
        {
            AddFruit((int)fruit.template_id, fruit.id);
        }

        static void AddFruit(int csvId, uint serverId)
        {
            Fruit newFruit = new Fruit();

            newFruit.ServerId = serverId;
            newFruit.CsvId = csvId;

            FruitList.Add(newFruit);
        }

        static void RemoveFruit(uint serverId)
        {
            for (int i = 0; i < FruitList.Count; ++i)
            {
                if (FruitList[i].ServerId == serverId)
                {
                    FruitList.RemoveAt(i);
                    return;
                }
            }
        }

        static void RemoveFruits(List<uint> fruitIds)
        {
            for (int i = 0; i < fruitIds.Count; ++i)
            {
                RemoveFruit(fruitIds[i]);
            }
        }

        public static Fruit GetFruit(uint serverId)
        {
            for (int i = 0; i < FruitList.Count; ++i)
            {
                if (FruitList[i].ServerId == serverId)
                {
                    return FruitList[i];
                }
            }

            return null;
        }

        static List<AwardInfo> ConvertAwardInfoList(List<gsproto.AwardInfo> awardInfoList)
        {
            List<AwardInfo> list = new List<AwardInfo>();

            for (int i = 0; i < awardInfoList.Count; ++i)
            {
                AwardInfo info = new AwardInfo();
                info.AwardType = awardInfoList[i].award_type;
                info.AwardValue = awardInfoList[i].award_value;

                list.Add(info);
            }

            return list;
        }

        static void AddTaskAward(uint awardType, uint awardValue, uint serverId)
        {
            uint value = awardValue;
            switch(awardType)
            {
                case (uint)PbCommon.EAwardType.E_Award_Type_Equip:
                    value = serverId;

                    RaiseOnEquipListChange();
                    break;

                case (uint)PbCommon.EAwardType.E_Award_Type_Gold_Coin:
                    Coin += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Type_Diamond:
                    Diamond += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Map_Piece:
                    MapPiece += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Dungeon_Key:
                    DungeonKey += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Iron:
                    Iron += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Crystal_Powder:
                    CrystalPowder += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Crystal_Piece:
                    CrystalPiece += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Crystal_Rime:
                    CrystalRime += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Reset_Ticket:
                    ResetTicket += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Arena_Ticket:
                    ArenaTicket += (uint)awardValue;

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Type_Bread:
                    value = serverId;

                    RaiseOnBreadListChange();

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Type_Fruit:
                    value = serverId;

                    RaiseOnFruitListChange();

                    break;

                case (uint)PbCommon.EAwardType.E_Award_Type_Hero:
                    value = serverId;

                    RaiseOnHeroListChange();

                    break;
            }

            RaiseOnDrawTaskAward(awardType, value);
        }

        static void RequestDailyTask()
        {
            gsproto.GetDailyTaskReq req = new gsproto.GetDailyTaskReq();
            req.session_id = PlayerDataCenter.SessionId;

            NetworkManager.SendRequest(ProtocolDataType.TcpShort, req);
        }

        static void RequestWeeklyTask()
        {
            gsproto.GetWeeklyTaskReq req = new gsproto.GetWeeklyTaskReq();
            req.session_id = PlayerDataCenter.SessionId;

            NetworkManager.SendRequest(ProtocolDataType.TcpShort, req);
        }

        static void CheckNormalTaskUpdateRaise(List<immortaldb.Task> taskItems)
        {
            for (int i = 0; i < taskItems.Count; ++i)
            {
                if (taskItems[i].task_pos > 0)
                {
                    RaiseOnNormalTaskDataChange(taskItems[i].task_pos);
                }
            }
        }

        static void CheckDailyAndWeeklyAwardState()
        {
            PbCommon.ETaskStateType state = TaskData.GetTaskAwardStateExceptNormal();

            RaiseOnUpdateDailyAndWeeklyAwardState(state);
        }

        static void CheckBakeriesState()
        {
            RaiseOnUpdateBakeriesFinishState(BakeriesFinishTime > 0 && ServerTime >= BakeriesFinishTime);
        }

        static void CheckNewSpecialSkillState()
        {
            SpecialSkillData.HasSkillCanUpgrade();
        }

        #endregion

        static void CheckExpeditionFinishState()
        {
            RaiseOnUpdateExpeditionFinishState(ExpeditionData.HasFinishExpedition());
        }

        #region callback for others

        public static Action OnPropertyChange;
        /// <summary>
        /// 资源属性变更(钱，体力，钥匙...) 
        /// </summary>
        static void RaiseOnPropertyChange()
        {
            if (OnPropertyChange != null)
            {
                OnPropertyChange();
            }
        }

        public static Action<uint, uint, uint, uint> OnGroupExpChange;
        /// <summary>
        /// 勇士团经验变化 
        /// </summary>
        /// <param name="oldExp">变化前经验</param>
        /// <param name="newExp">变化后经验</param>
        /// <param name="oldLevel">变化前等级</param>
        /// <param name="newLevel">变化后等级</param>
        static void RaiseOnGroupExpChange(uint oldExp, uint newExp, uint oldLevel, uint newLevel)
        {
            if (OnGroupExpChange != null)
            {
                OnGroupExpChange(oldExp, newExp, oldLevel, newLevel);
            }
        }


        public static Action<uint> OnHeroDataChange;
        /// <summary>
        /// 英雄数据有变化 
        /// </summary>
        /// <param name="heroServerId">英雄服务器ID</param>
        static void RaiseOnHeroDataChange(uint heroServerId)
        {
            if (OnHeroDataChange != null)
            {
                OnHeroDataChange(heroServerId);
            }
        }

        public static Action OnHeroListChange;
        /// <summary>
        /// 英雄列表有变化 
        /// </summary>
        static void RaiseOnHeroListChange()
        {
            if (OnHeroListChange != null)
            {
                OnHeroListChange();
            }
        }

        public static Action<uint, uint, bool> OnTrainHeroSuccess;
        /// <summary>
        /// 训练英雄成功 
        /// </summary>
        /// <param name="heroServerId">英雄服务器ID</param>
        /// <param name="growExp">经验增长值</param>
        /// <param name="isBigSuccess">是否大成功</param>
        static void RaiseOnTrainHeroSuccess(uint heroServerId, uint growExp, bool isBigSuccess)
        {
            if (OnTrainHeroSuccess != null)
            {
                OnTrainHeroSuccess(heroServerId, growExp, isBigSuccess);
            }
        }

        public static Action<uint, List<uint>> OnDismissHero;
        /// <summary>
        /// 英雄解聘 
        /// </summary>
        /// <param name="honor">获得荣誉</param>
        /// <param name="heroServerIds">被解聘的服务器id</param>
        static void RaiseOnDismissHero(uint honor, List<uint> heroServerIds)
        {
            if (OnDismissHero != null)
            {
                OnDismissHero(honor, heroServerIds);
            }
        }

        public static Action<uint> OnChangeRepresentHero;
        static void RaiseOnChangeRepresentHero(uint heroServerId)
        {
            if (OnChangeRepresentHero != null)
            {
                OnChangeRepresentHero(heroServerId);
            }
        }

        public static Action OnBreadListChange;
        /// <summary>
        /// 面包列表有变化 
        /// </summary>
        static void RaiseOnBreadListChange()
        {
            if (OnBreadListChange != null)
            {
                OnBreadListChange();
            }
        }

        public static Action<PbCommon.EExendBagType> OnExtandBag;
        /// <summary>
        /// 背包扩展（英雄、面包、武器、戒指） 
        /// </summary>
        static void RaiseOnExtandBag(PbCommon.EExendBagType bagType)
        {
            if (OnExtandBag != null)
            {
                OnExtandBag(bagType);
            }
        }

        public static Action<uint> OnHeroEvolution;
        /// <summary>
        /// 英雄进化 
        /// </summary>
        /// <param name="serverId">英雄服务器ID</param>
        static void RaiseOnHeroEvolution(uint serverId)
        {
            if (OnHeroEvolution != null)
            {
                OnHeroEvolution(serverId);
            }
        }

        public static Action OnStartRoast;
        /// <summary>
        /// 开始烘焙面包 
        /// </summary>
        static void RaiseOnStartRoast()
        {
            if (OnStartRoast != null)
            {
                OnStartRoast();
            }
        }

        public static Action OnCancelRoast;
        /// <summary>
        /// 取消烘焙面包 
        /// </summary>
        static void RaiseOnCancelRoast()
        {
            if (OnCancelRoast != null)
            {
                OnCancelRoast();
            }
        }

        public static Action<uint, List<immortaldb.Bread>> OnReceiveBread;
        /// <summary>
        /// 烘焙完成后接收面包 
        /// </summary>
        /// <param name="bakeriesType">烘焙类型</param>
        /// <param name="breads">面包</param>
        static void RaiseOnReceiveBread(uint bakeriesType, List<immortaldb.Bread> breads)
        {
            if (OnReceiveBread != null)
            {
                OnReceiveBread(bakeriesType, breads);
            }
        }

        public static Action<uint, List<immortaldb.Bread>> OnFinishRoast;
        /// <summary>
        /// 花钻直接完成烘焙 
        /// </summary>
        /// <param name="bakeriesType">烘焙类型</param>
        /// <param name="breads">面包</param>
        static void RaiseOnFinishRoast(uint bakeriesType, List<immortaldb.Bread> breads)
        {
            if (OnFinishRoast != null)
            {
                OnFinishRoast(bakeriesType, breads);
            }
        }

        public static Action<bool> OnUpdateBakeriesFinishState;
        /// <summary>
        /// 烘焙面包完成状态通知 
        /// </summary>
        /// <param name="state">是否完成</param>
        static void RaiseOnUpdateBakeriesFinishState(bool state)
        {
            if (OnUpdateBakeriesFinishState != null)
            {
                OnUpdateBakeriesFinishState(state);
            }
        }

        public static Action OnEquipListChange;
        /// <summary>
        /// 装备列表变化 
        /// </summary>
        static void RaiseOnEquipListChange()
        {
            if (OnEquipListChange != null)
            {
                OnEquipListChange();
            }
        }

        public static Action OnPassOver;
        /// <summary>
        /// 过关协议回调 
        /// </summary>
        static void RaiseOnPassOver()
        {
            if (OnPassOver != null)
            {
                OnPassOver();
            }
        }

        public static Action OnEnterPass;
        /// <summary>
        /// 进入关卡协议回调 
        /// </summary>
        static void RaiseOnEnterPass()
        {
            if (OnEnterPass != null)
            {
                OnEnterPass();
            }
        }

        public static Action<GroupPassiveInfo> OnGroupPassiveUpdate;
        /// <summary>
        /// 勇士团被动升级 
        /// </summary>
        /// <param name="info"></param>
        static void RaiseOnGroupPassiveUpdate(GroupPassiveInfo info)
        {
            if (OnGroupPassiveUpdate != null)
            {
                OnGroupPassiveUpdate(info);
            }
        }

        public static Action OnFruitListChange;
        /// <summary>
        /// 果实列表变化 
        /// </summary>
        static void RaiseOnFruitListChange()
        {
            if (OnFruitListChange != null)
            {
                OnFruitListChange();
            }
        }

        public static Action<uint, HeroAttribute, HeroAttribute, bool> OnHeroFruit;
        /// <summary>
        /// 勇士吃水果 
        /// </summary>
        /// <param name="heroServerId"></param>
        /// <param name="startAttr"></param>
        /// <param name="endAttr"></param>
        /// <param name="isBigSuccess"></param>
        static void RaiseOnHeroFruit(uint heroServerId, HeroAttribute startAttr, HeroAttribute endAttr, bool isBigSuccess)
        {
            if (OnHeroFruit != null)
            {
                OnHeroFruit(heroServerId, startAttr, endAttr, isBigSuccess);
            }
        }

        public static Action<uint, uint> OnEquipUpToHero;
        /// <summary>
        /// 勇士上装备
        /// </summary>
        /// <param name="heroServerId">勇士Id</param>
        /// <param name="equipServerId">装备Id</param>
        static void RaiseOnEquipUpToHero(uint heroServerId, uint equipServerId)
        {
            if (OnEquipUpToHero != null)
            {
                OnEquipUpToHero(heroServerId, equipServerId);
            }
        }

        public static Action<uint, uint> OnEquipDownFromHero;
        /// <summary>
        /// 勇士卸装备
        /// </summary>
        /// <param name="heroServerId">勇士Id</param>
        /// <param name="equipServerId">装备Id</param>
        static void RaiseOnEquipDownFromHero(uint heroServerId, uint equipServerId)
        {
            if (OnEquipDownFromHero != null)
            {
                OnEquipDownFromHero(heroServerId, equipServerId);
            }
        }

        public static Action<uint, uint> OnLockEquip;
        /// <summary>
        /// 锁定装备 
        /// </summary>
        /// <param name="equipServerId">装备Id</param>
        /// <param name="operationType">1 锁定 0 解锁</param>
        static void RaiseOnLockEquip(uint equipServerId, uint operationType)
        {
            if (OnLockEquip != null)
            {
                OnLockEquip(equipServerId, operationType);
            }
        }

        public static Action<uint, uint> OnWeaponReform;
        /// <summary>
        /// 武器词条改造 
        /// </summary>
        /// <param name="equipServerId"></param>
        /// <param name="index"></param>
        static void RaiseOnWeaponReform(uint equipServerId, uint index)
        {
            if (OnWeaponReform != null)
            {
                OnWeaponReform(equipServerId, index);
            }
        }

        public static Action<List<AwardInfo>, List<AwardInfo>, bool> OnWeaponBreak;
        /// <summary>
        /// 武器分解 
        /// </summary>
        /// <param name="awardList">奖励列表</param>
        /// <param name="extraAwardList">额外奖励列表</param>
        /// <param name="isBigSuccess">是否大成功</param>
        static void RaiseOnWeaponBreak(List<AwardInfo> awardList, List<AwardInfo> extraAwardList, bool isBigSuccess)
        {
            if (OnWeaponBreak != null)
            {
                OnWeaponBreak(awardList, extraAwardList, isBigSuccess);
            }
        }

        public static Action<uint, uint> OnWeaponRefine;
        /// <summary>
        /// 武器精粹 
        /// </summary>
        /// <param name="oldWeaponServerId"></param>
        /// <param name="newWeaponServerId"></param>
        static void RaiseOnWeaponRefine(uint oldWeaponServerId, uint newWeaponServerId)
        {
            if (OnWeaponRefine != null)
            {
                OnWeaponRefine(oldWeaponServerId, newWeaponServerId);
            }
        }

        public static Action<uint> OnReformCostReset;
        /// <summary>
        /// 武器词条花费重置 
        /// </summary>
        /// <param name="equipServerId"></param>
        static void RaiseOnReformCostReset(uint weaponServerId)
        {
            if (OnReformCostReset != null)
            {
                OnReformCostReset(weaponServerId);
            }
        }

        public static Action<uint> OnSpecialWeaponReformReset;
        /// <summary>
        /// 武器词条重置 
        /// </summary>
        /// <param name="weaponServerId"></param>
        static void RaiseSpecialWeaponReformReset(uint weaponServerId)
        {
            if (OnSpecialWeaponReformReset != null)
            {
                OnSpecialWeaponReformReset(weaponServerId);
            }
        }

        public static Action<uint, uint> OnSellItem;
        /// <summary>
        /// 卖物品返回 
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <param name="coin">钱数</param>
        static void RaiseOnSellItem(uint itemType, uint coin)
        {
            if (OnSellItem != null)
            {
                OnSellItem(itemType, coin);
            }
        }

        public static Action<uint> OnAcquireSkill;
        /// <summary>
        /// 特殊技能获得返回 
        /// </summary>
        /// <param name="groupId">特殊技能组Id</param>
        static void RaiseOnAcquireSkill(uint groupId)
        {
            if (OnAcquireSkill != null)
            {
                OnAcquireSkill(groupId);
            }
        }

        public static Action<uint, uint, bool> OnEquipSkill;
        static void RaiseOnEquipSkill(uint heroServerId, uint skillId, bool isBigSuccess)
        {
            if (OnEquipSkill != null)
            {
                OnEquipSkill(heroServerId, skillId, isBigSuccess);
            }
        }

        public static Action<bool> OnUpdateNewSkillState;
        /// <summary>
        /// 新技能状态通知 
        /// </summary>
        /// <param name="state">是否有新技能</param>
        static void RaiseOnUpdateNewSkillState(bool state)
        {
            if (OnUpdateNewSkillState != null)
            {
                OnUpdateNewSkillState(state);
            }
        }

        //public static Action<uint> OnReceiveTask;
        ///// <summary>
        ///// 接受任务 
        ///// </summary>
        ///// <param name="position">任务位置</param>
        //static void RaiseReceiveTask(uint position)
        //{
        //    if (OnReceiveTask != null)
        //    {
        //        OnReceiveTask(position);
        //    }
        //}

        //public static Action<uint> OnRefuseTask;
        ///// <summary>
        ///// 拒绝任务 
        ///// </summary>
        ///// <param name="position">任务位置</param>
        //static void RaiseOnRefuseTask(uint position)
        //{
        //    if (OnRefuseTask != null)
        //    {
        //        OnRefuseTask(position);
        //    }
        //}

        public static Action<uint, uint> OnDrawTaskAward;
        /// <summary>
        /// 领取任务奖励 
        /// </summary>
        /// <param name="awardType">类型</param>
        /// <param name="awardValue">数值或者Id</param>
        static void RaiseOnDrawTaskAward(uint awardType, uint awardValue)
        {
            if (OnDrawTaskAward != null)
            {
                OnDrawTaskAward(awardType, awardType);
            }
        }

        public static Action<uint> OnNormalTaskDataChange;
        /// <summary>
        /// 普通任务（右侧3个）内容变化 
        /// </summary>
        /// <param name="position">任务位置</param>
        static void RaiseOnNormalTaskDataChange(uint position)
        {
            if (OnNormalTaskDataChange != null)
            {
                OnNormalTaskDataChange(position);
            }
        }

        public static Action<int> OnTaskDataChange;
        /// <summary>
        /// 任务内容变化 
        /// </summary>
        /// <param name="csvId"></param>
        static void RaiseOnTaskDataChange(int csvId)
        {
            if (OnTaskDataChange != null)
            {
                OnTaskDataChange(csvId);
            }
        }

        public static Action OnDailyTaskListChange;
        /// <summary>
        /// 日常任务列表变化 
        /// </summary>
        static void RaiseOnDailyTaskListChange()
        {
            if (OnDailyTaskListChange != null)
            {
                OnDailyTaskListChange();
            }
        }

        public static Action OnWeeklyTaskListChange;
        /// <summary>
        /// 周常任务列表变化
        /// </summary>
        static void RaiseOnWeeklyTaskListChange()
        {
            if (OnWeeklyTaskListChange != null)
            {
                OnWeeklyTaskListChange();
            }
        }

        public static Action<bool> OnHasOtherTaskNotDrawAward;
        static void RaiseOnHasOtherTaskNotDrawAward(bool result)
        {
            if (OnHasOtherTaskNotDrawAward != null)
            {
                RaiseOnHasOtherTaskNotDrawAward(result);
            }
        }

        public static Action<PbCommon.ETaskStateType> OnUpdateDailyAndWeeklyAwardState;
        /// <summary>
        /// 日常周常任务集体的奖励状态更新 
        /// </summary>
        /// <param name="state"></param>
        static void RaiseOnUpdateDailyAndWeeklyAwardState(PbCommon.ETaskStateType state)
        {
            if (OnUpdateDailyAndWeeklyAwardState != null)
            {
                OnUpdateDailyAndWeeklyAwardState(state);
            }
        }

        public static Action OnExpeditionListChange;
        /// <summary>
        /// 探险列表变化 
        /// </summary>
        static void RaiseOnExpeditionListChange()
        {
            if (OnExpeditionListChange != null)
            {
                OnExpeditionListChange();
            }
        }

        public static Action<int> OnBeginExpedition;
        /// <summary>
        /// 开始探险 
        /// </summary>
        /// <param name="csvId">探险Id</param>
        static void RaiseOnBeginExpedition(int csvId)
        {
            if (OnBeginExpedition != null)
            {
                OnBeginExpedition(csvId);
            }
        }

        public static Action<GroupAddExpInfo, List<HeroAddExpInfo>, List<AwardInfo>> OnEndExpedition;
        /// <summary>
        /// 结束探险 
        /// </summary>
        /// <param name="groupAddExp">勇士团经验改变</param>
        /// <param name="heroAddExpList">英雄经验改变列表</param>
        /// <param name="extraAwardList">额外奖励列表</param>
        static void RaiseOnEndExpedition(GroupAddExpInfo groupAddExp, List<HeroAddExpInfo> heroAddExpList, List<AwardInfo> extraAwardList)
        {
            if (OnEndExpedition != null)
            {
                OnEndExpedition(groupAddExp, heroAddExpList, extraAwardList);
            }
        }

        public static Action<int> OnCancelExpedition;
        /// <summary>
        /// 放弃探险 
        /// </summary>
        /// <param name="csvId">tanxianId</param>
        static void RaiseOnCancelExpedition(int csvId)
        {
            if (OnCancelExpedition != null)
            {
                OnCancelExpedition(csvId);
            }
        }

        public static Action<bool> OnUpdateExpeditionFinishState;
        /// <summary>
        /// 探险时间完成 
        /// </summary>
        /// <param name="hasFinish">是否有完成的</param>
        static void RaiseOnUpdateExpeditionFinishState(bool hasFinish)
        {
            if (OnUpdateExpeditionFinishState != null)
            {
                OnUpdateExpeditionFinishState(hasFinish);
            }
        }

        #endregion

        /// <summary>
        /// 协议回调注册 
        /// </summary>
        //[RuntimeInitializeOnLoadMethod]
        public static void RegisterHandler()
        {
            NetworkManager.RegisterHandler((uint)PbLogin.command.CMD_VERIFY_RSP, OnVerifyRsp);
            NetworkManager.RegisterHandler((uint)PbLogin.command.CMD_CREATE_RSP, OnCreatRoleRsp);
            NetworkManager.RegisterHandler((uint)PbLogin.command.CMD_LOGON_RSP, OnLogonRsp);

            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_LOGIN_RSP, OnLoginGsRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_DATA_RSP, OnPlayerDataRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_TRAIN_HERO_RSP, OnTrainHeroRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_HERO_EVOLUTION_RSP, OnHeroEvolutionRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_EXTEND_BAG_RSP, OnExtandBagRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_SELL_ITEM_RSP, OnSellItemRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_START_ROAST_RSP, OnStartRoastRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_CANCEL_ROAST_RSP, OnCancelRoastRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_RECEIVE_BREAD_RSP, OnReceiveBreadRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_FINISH_ROAST_RSP, OnFinishRoastRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_DISMISS_HERO_RSP, OnDismissHeroRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_LOCK_HERO_RSP, OnLockHeroRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_CHANGE_REPRESENT_HERO_RSP, OnChangeRepresentHeroRsp);

            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_ENTER_PASS_RSP, OnEnterPassRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_PASS_OVER_RSP, OnPassOverRsp);

            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_HERO_FRUIT_RSP, OnHeroFruitRsp);

            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_EQUIP_UP_TO_HERO_RSP, OnEquipUpToHeroRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_EQUIP_DOWN_FROM_HERO_RSP, OnEquipDownFromHeroRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_LOCK_EQUIP_RSP, OnLockEquipRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_WEAPON_REFORM_RSP, OnWeaponReformRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_WEAPON_BREAK_RSP, OnWeaponBreakRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_WEAPON_REFINE_RSP, OnWeaponRefineRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_REFORM_COST_RESET_RSP, OnReformCostResetRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_SPECIAL_WEAPON_REFORM_RESET_RSP, OnSpecialWeaponReformResetRsp);

            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_ACQUIRE_SKILL_RSP, OnAcquireSkillRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_EQUIP_SKILL_RSP, OnEquipSkillRsp);

            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_RECEIVE_TASK_RSP, OnReceiveTaskRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_REFUSE_TASK_RSP, OnRefuseTaskRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_DRAW_TASK_AWARD_RSP, OnDrawTaskAwardRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_GET_DAILY_TASK_RSP, OnGetDailyTaskRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_GET_WEEKLY_TASK_RSP, OnGetWeeklyTaskRsp);

            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_BEGIN_EXPEDITION_RSP, OnBeginExpeditionRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_END_EXPEDITION_RSP, OnEndExpeditionRsp);
            NetworkManager.RegisterHandler((uint)gsproto.command.CMD_CANCEL_EXPEDITION_RSP, OnCancelExpeditionRsp);
        }

        #region protocol callback
        // ---------------------------- 协议回调 ---------------------------- 
        static void OnVerifyRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnVerifyRsp " + result);
            if (result == 0)
            {
                PbLogin.VerifyRsp verifyRsp = response as PbLogin.VerifyRsp;

                Account = verifyRsp.account;
                SessionId = verifyRsp.session_id;

                if (verifyRsp.roles.Count > 0)
                {
                    RoleId = verifyRsp.roles[0].roleid;
                    Account = verifyRsp.roles[0].account;
                    Nickname = verifyRsp.roles[0].name;

                    Aid = (int)verifyRsp.roles[0].aid;
                    Sid = (int)verifyRsp.roles[0].sid;
                    Gid = verifyRsp.roles[0].gid;
                }
            }
        }

        static void OnCreatRoleRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnCreatRoleRsp " + result);
            if (result == 0)
            {
                PbLogin.CreateRsp createRsp = response as PbLogin.CreateRsp;

                RoleId = createRsp.roleid;
                Account = createRsp.account;
            }
        }

        static void OnLogonRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnLogonRsp " + result);
            if (result == 0)
            {
                PbLogin.LogonRsp logonRsp = response as PbLogin.LogonRsp;

                UserToken = new byte[logonRsp.key.Length];
                System.Array.Copy(logonRsp.key, 0, UserToken, 0, logonRsp.key.Length);
            }
        }

        static void OnLoginGsRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnLoginGsRsp " + result);
            if (result == 0)
            {
                gsproto.LoginRsp loginRsp = response as gsproto.LoginRsp;

                SessionId = loginRsp.session_id;
            }
        }

        static void OnPlayerDataRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnPlayerDataRsp " + result);
            if (result == 0)
            {
                gsproto.PlayerDataRsp rsp = response as gsproto.PlayerDataRsp;

                ServerTime = rsp.server_time;

                AlarmClock.Instance.RegisterNowTimeFunction(GetNowTime);

                // ---------------------- 图鉴 begin ---------------------- 
                HandbookData.ClearHeroHandbook();
                HandbookData.ReadDataFromCsv();
                if (rsp.player_data.hero_handbook_list != null &&
                    rsp.player_data.hero_handbook_list.hero_handbook_list != null)
                {
                    for (int i = 0; i < rsp.player_data.hero_handbook_list.hero_handbook_list.Count; ++i)
                    {
                        HandbookData.UpdateHeroHandbooks(rsp.player_data.hero_handbook_list.hero_handbook_list);
                    }
                }

                // ---------------------- 图鉴 end ---------------------- 

                // ---------------------- 装备 begin ---------------------- 
                EquipList.Clear();
                WeaponList.Clear();
                RingList.Clear();
                if (rsp.player_data.equip_list != null)
                {
                    MaxWeaponCount = rsp.player_data.equip_list.max_weapon_count;
                    MaxRingCount = rsp.player_data.equip_list.max_ring_count;

                    if (rsp.player_data.equip_list.equip_list != null)
                    {
                        AddEquips(rsp.player_data.equip_list.equip_list);
                    }
                }

                // ---------------------- 装备 end ---------------------- 

                // ---------------------- 勇士团 begin ---------------------- 
                Level = rsp.player_data.level;
                Exp = rsp.player_data.exp;
                Coin = rsp.player_data.coin;
                Stamina = rsp.player_data.stamina;
                Diamond = rsp.player_data.diamond;

                RepresentHero = rsp.player_data.represent_hero;

                if (rsp.player_data.assets != null)
                {
                    Iron = rsp.player_data.assets.iron;
                    CrystalPowder = rsp.player_data.assets.crystal_powder;
                    CrystalPiece = rsp.player_data.assets.crystal_piece;
                    CrystalRime = rsp.player_data.assets.crystal_rime;
                    ResetTicket = rsp.player_data.assets.reset_ticket;
                    DungeonKey = rsp.player_data.assets.dungeon_key;
                    DungeonKeyUpTime = rsp.player_data.assets.dungeon_key_up_time;
                    MapPiece = rsp.player_data.assets.map_piece;
                    ArenaTicket = rsp.player_data.assets.arena_ticket;
                    ArenaTicketUpTime = rsp.player_data.assets.arena_ticket_up_time;
                    Honor = rsp.player_data.assets.honor;
                }

                if (rsp.player_data.common_info != null)
                {
                    StaminaUpTime = rsp.player_data.common_info.up_sta_time;
                }

                GroupPassiveList.Clear();
                if (rsp.hero_group_effect_list != null &&
                    rsp.hero_group_effect_list.hero_group_effects != null)
                {
                    AddGroupPassive(rsp.hero_group_effect_list.hero_group_effects);
                }

                AdjustMaxStamina();
                AdjustStaminaRecoverInterval();

                // ---------------------- 勇士团 end ---------------------- 

                // ---------------------- 英雄 begin ---------------------- 
                HeroList.Clear();
                if (rsp.player_data.hero_list != null)
                {
                    MaxHeroCount = rsp.player_data.hero_list.max_hero_count;

                    if (rsp.player_data.hero_list.hero_list != null)
                    {
                        AddHeroes(rsp.player_data.hero_list.hero_list);

                        RaiseOnHeroListChange();
                    }
                }
                // ---------------------- 英雄 end ---------------------- 

                // ---------------------- 面包 begin ---------------------- 
                BreadList.Clear();
                if (rsp.player_data.bread_list != null)
                {
                    MaxBreadCount = rsp.player_data.bread_list.max_bread_count;

                    if (rsp.player_data.bread_list.bread_list != null)
                    {
                        AddBreads(rsp.player_data.bread_list.bread_list);

                        RaiseOnBreadListChange();
                    }
                }
                // ---------------------- 面包 end ---------------------- 

                // ---------------------- 面包坊 begin ---------------------- 

                BakeriesType = rsp.player_data.common_info.bakeries_type;
                BakeriesFinishTime = rsp.player_data.common_info.bakeries_finish_time;

                // ---------------------- 面包坊 end ---------------------- 

                // ---------------------- 果实 begin ---------------------- 
                FruitList.Clear();
                if (rsp.player_data.fruit_list != null)
                {
                    MaxFruitCount = rsp.player_data.fruit_list.max_fruit_count;

                    if (rsp.player_data.fruit_list.fruit_list != null)
                    {
                        AddFruits(rsp.player_data.fruit_list.fruit_list);
                    }
                }

                // ---------------------- 果实 end ---------------------- 

                // ---------------------- 特殊技能 begin ---------------------- 
                SpecialSkillData.ClearSpecialSkill();
                SpecialSkillData.ReadFromCsv();
                if (rsp.player_data.skill_list!= null &&
                    rsp.player_data.skill_list.skills != null)
                {
                    SpecialSkillData.UpdateSkillInfos(rsp.player_data.skill_list.skills);
                }
                SpecialSkillData.CheckSkillState();
                CheckNewSpecialSkillState();
                // ---------------------- 特殊技能 end ---------------------- 

                // ---------------------- 任务 begin ---------------------- 
                TaskData.Clear();
                if (rsp.player_data.task_list != null &&
                    rsp.player_data.task_list.tasks != null)
                {
                    TaskData.AddTasks(rsp.player_data.task_list.tasks);
                }
                if (rsp.player_data.common_info.task_pos_cools != null)
                {
                    TaskData.AddCountdowns(rsp.player_data.common_info.task_pos_cools);
                }

                TaskData.SetDailyTaskAlarm(RequestDailyTask);
                TaskData.SetWeeklyTaskAlarm(RequestWeeklyTask);
                // ---------------------- 任务 end ---------------------- 

                // ---------------------- 探险 begin ---------------------- 
                ExpeditionData.Clear();
                if (rsp.player_data.common_info != null &&
                    rsp.player_data.common_info.expedition_list != null)
                {
                    ExpeditionData.AddExpeditions(rsp.player_data.common_info.expedition_list);
                }
                ExpeditionData.SetHeroInExpedition(HeroList);
                ExpeditionData.SetFinishAlarm(ServerTime, CheckExpeditionFinishState);
                // ---------------------- 探险 end ---------------------- 

                // ---------------------- 队伍保存 begin ---------------------- 
                TeamCollectionData.Clear();
                if (rsp.player_data.team_list != null &&
                    rsp.player_data.team_list.team_list != null)
                {
                    TeamCollectionData.AddTeams(rsp.player_data.team_list.team_list);
                }
                // ---------------------- 队伍保存 end ---------------------- 

                // ---------------------- 女神 begin ---------------------- 
                GoddessData.Clear();
                GoddessData.ClearGoddessInOut();
                if (rsp.player_data.goddess_list != null &&
                    rsp.player_data.goddess_list.goddess_list != null)
                {
                    GoddessData.AddGoddesses(rsp.player_data.goddess_list.goddess_list);
                }
                // ---------------------- 女神 end ---------------------- 
            }
        }

        static void OnTrainHeroRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnTrainHeroRsp " + result);
            if (result == 0)
            {
                gsproto.TrainHeroReq req = request as gsproto.TrainHeroReq;
                gsproto.TrainHeroRsp rsp = response as gsproto.TrainHeroRsp;

                Hero hero = GetHero(req.hero_id);
                uint growExp = hero.EnhanceExp;

                SyncPlayerData(rsp.data_syncs);

                growExp = hero.EnhanceExp - growExp;

                RaiseOnTrainHeroSuccess(req.hero_id, growExp, rsp.is_big_success);
                RaiseOnHeroDataChange(req.hero_id);

                RemoveBreads(req.bread_ids);

                RaiseOnBreadListChange();

                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();
            }
        }

        static void OnHeroEvolutionRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnHeroEvolutionRsp " + result);
            if (result == 0)
            {
                gsproto.HeroEvolutionReq req = request as gsproto.HeroEvolutionReq;
                gsproto.HeroEvolutionRsp rsp = response as gsproto.HeroEvolutionRsp;

                SyncPlayerData(rsp.data_syncs);

                RaiseOnHeroDataChange(req.hero_id);
                RaiseOnHeroEvolution(req.hero_id);

                // 检查特殊技能状态 
                SpecialSkillData.CheckSkillState();
                CheckNewSpecialSkillState();

                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();
            }
        }

        static void OnSellItemRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnSellItemRsp " + result);
            if (result == 0)
            {
                gsproto.SellItemReq req = request as gsproto.SellItemReq;
                gsproto.SellItemRsp rsp = response as gsproto.SellItemRsp;

                uint sellCoin = Coin;
                SyncPlayerData(rsp.data_syncs);
                sellCoin = Coin - sellCoin;

                switch (req.item_type)
                {
                    case (uint)PbCommon.ESaleItemType.E_Sale_Bread:
                        RemoveBreads(req.item_ids);

                        RaiseOnBreadListChange();

                        break;

                    case (uint)PbCommon.ESaleItemType.E_Sale_Equip:
                        RemoveEquips(req.item_ids);

                        RaiseOnEquipListChange();

                        break;

                    case (uint)PbCommon.ESaleItemType.E_Sale_Fruit:
                        RemoveFruits(req.item_ids);

                        RaiseOnFruitListChange();

                        break;
                }

                RaiseOnSellItem(req.item_type, sellCoin);
            }
        }

        static void OnExtandBagRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnExtandBagRsp " + result);
            if (result == 0)
            {
                gsproto.ExtendBagReq req = request as gsproto.ExtendBagReq;
                gsproto.ExtendBagRsp rsp = response as gsproto.ExtendBagRsp;

                SyncPlayerData(rsp.data_syncs);

                switch (req.bag_type)
                {
                    case (uint)PbCommon.EExendBagType.E_Extend_Bread_Bag:
                        MaxBreadCount += (uint)DefaultConfig.GetInt("HeroBagExtendSize");
                        break;

                    case (uint)PbCommon.EExendBagType.E_Extend_Hero_Bag:
                        MaxHeroCount += (uint)DefaultConfig.GetInt("BreadBagExtendSize");
                        break;

                    case (uint)PbCommon.EExendBagType.E_Extend_Weapon_Bag:
                        MaxWeaponCount += (uint)DefaultConfig.GetInt("WeaponBagExtendSize");
                        break;

                    case (uint)PbCommon.EExendBagType.E_Extend_Ring_Bag:
                        MaxRingCount += (uint)DefaultConfig.GetInt("RingBagExtendSize");
                        break;

                    case (uint)PbCommon.EExendBagType.E_Extend_Fruit_Bag:
                        MaxFruitCount += (uint)DefaultConfig.GetInt("FruitBagExtendSize");
                        break;
                }

                RaiseOnExtandBag((PbCommon.EExendBagType)req.bag_type);
            }
        }

        static void OnStartRoastRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnStartRoastRsp " + result);
            if (result == 0)
            {
                gsproto.StartRoastRsp rsp = response as gsproto.StartRoastRsp;
                gsproto.StartRoastReq req = request as gsproto.StartRoastReq;

                CSV_b_bakeries_template bakeriesCSV = CSV_b_bakeries_template.FindData((int)req.bakeries_type);

                BakeriesType = req.bakeries_type;
                BakeriesFinishTime = ServerTime + (uint)bakeriesCSV.NeedTime * ConstDefine.SECOND_PER_MINUTE;

                AlarmClock.Instance.AddDateAlarm(BakeriesFinishTime, CheckBakeriesState);

                RaiseOnStartRoast();
            }
        }

        static void OnCancelRoastRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnCancelRoastRsp " + result);
            if (result == 0)
            {
                AlarmClock.Instance.RemoveDateAlarm(BakeriesFinishTime, CheckBakeriesState);

                BakeriesType = 0;
                BakeriesFinishTime = 0;

                RaiseOnCancelRoast();
            }
        }

        static void OnReceiveBreadRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnReceiveBreadRsp " + result);
            if (result == 0)
            {
                gsproto.ReceiveBreadRsp rsp = response as gsproto.ReceiveBreadRsp;

                SyncPlayerData(rsp.data_syncs);

                uint tmpBakeriesType = BakeriesType;

                AlarmClock.Instance.RemoveDateAlarm(BakeriesFinishTime, CheckBakeriesState);

                BakeriesType = 0;
                BakeriesFinishTime = 0;

                RaiseOnBreadListChange();

                RaiseOnReceiveBread(tmpBakeriesType, rsp.data_syncs.breads);

                CheckBakeriesState();

                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();
            }
        }

        static void OnFinishRoastRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnFinishRoastRsp " + result);
            if (result == 0)
            {
                gsproto.FinishRoastRsp rsp = response as gsproto.FinishRoastRsp;

                SyncPlayerData(rsp.data_syncs);

                uint tmpBakeriesType = BakeriesType;

                BakeriesType = 0;
                BakeriesFinishTime = 0;

                RaiseOnBreadListChange();

                RaiseOnFinishRoast(tmpBakeriesType, rsp.data_syncs.breads);

                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();
            }
        }

        static void OnDismissHeroRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnDismissHeroRsp " + result);
            if (result == 0)
            {
                gsproto.DismissHeroRsp rsp = response as gsproto.DismissHeroRsp;

                SyncPlayerData(rsp.data_syncs);

                RemoveHeroes(rsp.hero_ids);

                RaiseOnHeroListChange();

                RaiseOnDismissHero(rsp.got_hornors, rsp.hero_ids);
            }
        }

        static void OnLockHeroRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnLockHeroRsp " + result);
            if (result == 0)
            {
                gsproto.LockHeroReq req = request as gsproto.LockHeroReq;

                Hero hero = GetHero(req.hero_id);
                hero.IsLock = req.operation_type == 1 ? true : false;

                RaiseOnHeroDataChange(req.hero_id);
            }
        }

        static void OnChangeRepresentHeroRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnChangeRepresentHeroRsp " + result);
            if (result == 0)
            {
                gsproto.ChangeRepresentHeroRsp rsp = response as gsproto.ChangeRepresentHeroRsp;

                RepresentHero = rsp.represent_hero_id;

                RaiseOnChangeRepresentHero(rsp.represent_hero_id);
            }
        }

        static void OnEnterPassRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnEnterPassRsp " + result);
            if (result == 0)
            {
                gsproto.EnterPassReq req = request as gsproto.EnterPassReq;
                gsproto.EnterPassRsp rsp = response as gsproto.EnterPassRsp;

                TeamInfo teamInfo = TeamCollectionData.GetMissionTeam();
                teamInfo.Members.Clear();
                teamInfo.Members.AddRange(req.team_members);
                teamInfo.LeaderId = req.leader_id;
                teamInfo.Goddess = req.team_goddess;

                EnterLevelData.PassId = (int)req.pass_id;
                EnterLevelData.SkillList = rsp.skill_lists;
                EnterLevelData.HasHidden = rsp.is_hidden_boss > 0 ? true: false;
                EnterLevelData.NormalCoin = (int)rsp.normal_coin_count;
                EnterLevelData.HiddenCoin = (int)rsp.hidden_coin_count;
                EnterLevelData.BossChest = (int)rsp.boss_chest_count;
                EnterLevelData.ActiveChest = (int)rsp.active_chest_count;
                EnterLevelData.NormalChest = (int)rsp.normal_chest_count;

                CSV_b_game_level gameLevelCSV = CSV_b_game_level.FindData((int)req.pass_id);
                switch(gameLevelCSV.CostType)
                {
                    case (int)PbCommon.ECostType.E_Cost_Stamina:
                        Stamina -= (uint)gameLevelCSV.CostCount;
                        break;

                    case (int)PbCommon.ECostType.E_Cost_Dungeon_Key:
                        DungeonKey -= (uint)gameLevelCSV.CostCount;
                        break;

                    case (int)PbCommon.ECostType.E_Cost_Map_Piece:
                        MapPiece -= (uint)gameLevelCSV.CostCount;
                        break;
                }

                TaskData.PrepareTaskTracker();

                // 女神 
                if (rsp.goddess_list != null &&
                    rsp.goddess_list.goddess_list != null)
                {
                    GoddessData.UpdateGoddesses(rsp.goddess_list.goddess_list);
                }

                RaiseOnEnterPass();
            }
        }

        static void OnPassOverRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnPassOverRsp " + result);
            if (result == 0)
            {
                gsproto.PassOverReq req = request as gsproto.PassOverReq;
                gsproto.PassOverRsp rsp = response as gsproto.PassOverRsp;

                // 战斗结果 
                OverLevelData.PassResult = req.pass_result;

                // 英雄经验 
                OverLevelData.HeroAddExpList.Clear();
                TeamInfo teamInfo = TeamCollectionData.GetMissionTeam();
                for (int i = 0; i < teamInfo.Members.Count; ++i)
                {
                    Hero hero = GetHero(teamInfo.Members[i]);

                    HeroAddExpInfo heroAddExpInfo = new HeroAddExpInfo();

                    heroAddExpInfo.HeroData = hero;
                    heroAddExpInfo.StartExp = hero.Exp;
                    heroAddExpInfo.StartLevel = hero.Level;

                    hero.AddExp(rsp.hero_exp_got);
                    heroAddExpInfo.EndExp = hero.Exp;
                    heroAddExpInfo.EndLevel = hero.Level;

                    OverLevelData.HeroAddExpList.Add(heroAddExpInfo);
                }

                // 数据同步 
                SyncPlayerData(rsp.data_syncs);

                // 箱子 
                OverLevelData.DropChestList.Clear();
                if (rsp.chest_award_info != null)
                {
                    for (int i = 0; i < rsp.chest_award_info.Count; ++i)
                    {
                        DropChestInfo dropChestInfo = new DropChestInfo();

                        dropChestInfo.ChestType = (PbCommon.EChestType)req.chest_list[i].chest_type;
                        dropChestInfo.AwardType = (PbCommon.EAwardType)rsp.chest_award_info[i].award_type;
                        dropChestInfo.AwardValue = rsp.chest_award_info[i].award_value;

                        OverLevelData.DropChestList.Add(dropChestInfo);

                        switch(rsp.chest_award_info[i].award_type)
                        {
                            case (uint)PbCommon.EAwardType.E_Award_Type_Gold_Coin:
                                OverLevelData.Coin -= rsp.chest_award_info[i].award_value;
                                break;
                        }
                    }
                }

                // 英雄 
                OverLevelData.DropHero = null;
                immortaldb.Hero dropHero = GetSyncDataHero(rsp.data_syncs);
                if (dropHero != null)
                {
                    OverLevelData.DropHero = GetHero(dropHero.id);
                }

                // 检查特殊技能状态 
                SpecialSkillData.CheckSkillState();
                CheckNewSpecialSkillState();

                // 团队经验 
                OverLevelData.GroupAddExpData.StartExp = Exp;
                OverLevelData.GroupAddExpData.StartLevel = Level;
                AddGroupExp(rsp.player_exp_got);
                OverLevelData.GroupAddExpData.EndExp = Exp;
                OverLevelData.GroupAddExpData.EndLevel = Level;
 
                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();

                // 女神 
                if (rsp.goddess_list != null &&
                    rsp.goddess_list.goddess_list != null)
                {
                    GoddessData.UpdateGoddesses(rsp.goddess_list.goddess_list);
                }

                RaiseOnPassOver();

                if (OverLevelData.GroupAddExpData.StartExp != OverLevelData.GroupAddExpData.EndExp)
                {
                    RaiseOnGroupExpChange(OverLevelData.GroupAddExpData.StartExp,
                                        OverLevelData.GroupAddExpData.EndExp,
                                        OverLevelData.GroupAddExpData.StartLevel,
                                        OverLevelData.GroupAddExpData.EndLevel);
                }
            }
        }

        static void OnHeroFruitRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnHeroFruitRsp " + result);
            if (result == 0)
            {
                gsproto.HeroFruitReq req = request as gsproto.HeroFruitReq;
                gsproto.HeroFruitRsp rsp = response as gsproto.HeroFruitRsp;

                RemoveFruits(req.fruit_ids);

                HeroAttribute startAttr = new HeroAttribute();
                HeroAttribute endAttr = new HeroAttribute();

                Hero hero = GetHero(rsp.data_syncs.heros[0].id);

                startAttr.Plus(hero.FruitAttribute);

                SyncPlayerData(rsp.data_syncs);

                endAttr.Plus(hero.FruitAttribute);

                RaiseOnFruitListChange();

                RaiseOnHeroFruit(hero.ServerId, startAttr, endAttr, rsp.is_big_success);

                RaiseOnHeroDataChange(hero.ServerId);

                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();
            }
        }

        static void OnEquipUpToHeroRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnEquipUpToHeroRsp " + result);
            if (result == 0)
            {
                gsproto.EquipUpToHeroReq req = request as gsproto.EquipUpToHeroReq;
                gsproto.EquipUpToHeroRsp rsp = response as gsproto.EquipUpToHeroRsp;

                Equip equip = GetEquip(req.equip_id);
                Hero hero = GetHero(req.hero_id);
                Equip oldEquip = hero.GetEquip(equip.EquipType);

                hero.UpEquip(equip);

                RaiseOnHeroDataChange(req.hero_id);

                if (oldEquip != null)
                {
                    oldEquip.HeroServerId = 0;
                    RaiseOnEquipDownFromHero(req.hero_id, oldEquip.ServerId);
                }

                RaiseOnEquipUpToHero(req.hero_id, req.equip_id);
            }
        }

        static void OnEquipDownFromHeroRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnEquipDownFromHeroRsp " + result);
            if (result == 0)
            {
                gsproto.EquipDownFromHeroReq req = request as gsproto.EquipDownFromHeroReq;
                gsproto.EquipDownFromHeroRsp rsp = response as gsproto.EquipDownFromHeroRsp;

                Hero hero = GetHero(req.hero_id);
                hero.DownEquip(req.equip_id);

                RaiseOnHeroDataChange(req.hero_id);

                RaiseOnEquipDownFromHero(req.hero_id, req.equip_id);
            }
        }

        static void OnLockEquipRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnLockEquipRsp " + result);
            if (result == 0)
            {
                gsproto.LockEquipReq req = request as gsproto.LockEquipReq;
                gsproto.LockEquipRsp rsp = response as gsproto.LockEquipRsp;

                Equip equip = GetEquip(req.equip_id);
                equip.IsLock = req.operation_type > 0 ? true : false;

                RaiseOnLockEquip(req.equip_id, req.operation_type);
            }
        }

        static void OnWeaponReformRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnWeaponReformRsp " + result);
            if (result == 0)
            {
                gsproto.WeaponReformReq req = request as gsproto.WeaponReformReq;
                gsproto.WeaponReformRsp rsp = response as gsproto.WeaponReformRsp;

                SyncPlayerData(rsp.data_syncs);

                Equip equip = GetEquip(req.weapon_id);
                equip.AddReformCount(req.reform_index);
                equip.UpdateEquipReform(req.reform_index, rsp.reform_property, rsp.reform_value, rsp.is_big_success);

                if (equip.HeroServerId > 0)
                {
                    Hero hero = GetHero(equip.HeroServerId);
                    hero.CalculateAttribute();

                    RaiseOnHeroDataChange(equip.HeroServerId);
                }

                RaiseOnWeaponReform(req.weapon_id, req.reform_index);

                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();
            }
        }

        static void OnWeaponBreakRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnWeaponBreakRsp " + result);
            if (result == 0)
            {
                gsproto.WeaponBreakReq req = request as gsproto.WeaponBreakReq;
                gsproto.WeaponBreakRsp rsp = response as gsproto.WeaponBreakRsp;

                SyncPlayerData(rsp.data_syncs);

                RemoveEquips(req.weapon_ids);

                RaiseOnEquipListChange();

                RaiseOnWeaponBreak(ConvertAwardInfoList(rsp.break_res), ConvertAwardInfoList(rsp.extra_break_res), rsp.is_big_success);
            }
        }

        static void OnWeaponRefineRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnWeaponRefineRsp " + result);
            if (result == 0)
            {
                gsproto.WeaponRefineReq req = request as gsproto.WeaponRefineReq;
                gsproto.WeaponRefineRsp rsp = response as gsproto.WeaponRefineRsp;

                SyncPlayerData(rsp.data_syncs);

                Equip oldEquip = GetEquip(req.weapon_id);
                if (oldEquip.HeroServerId > 0)
                {
                    oldEquip.DownFromHero();
                }

                RemoveEquip(req.weapon_id);

                Equip newEquip = GetEquip(rsp.data_syncs.equips[0].id);
                if (newEquip.HeroServerId > 0)
                {
                    newEquip.UpToHero(newEquip.HeroServerId);

                    RaiseOnHeroDataChange(newEquip.HeroServerId);
                }

                RaiseOnWeaponRefine(req.weapon_id, rsp.data_syncs.equips[0].id);
                RaiseOnEquipListChange();
            }
        }

        static void OnReformCostResetRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnReformCostResetRsp " + result);
            if (result == 0)
            {
                gsproto.ReformCostResetReq req = request as gsproto.ReformCostResetReq;
                gsproto.ReformCostResetRsp rsp = response as gsproto.ReformCostResetRsp;

                SyncPlayerData(rsp.data_syncs);

                Equip equip = GetEquip(req.weapon_id);
                equip.ResetReformCount(req.reform_index);

                RaiseOnReformCostReset(req.weapon_id);
            }
        }

        static void OnSpecialWeaponReformResetRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnSpecialWeaponReformResetRsp " + result);
            if (result == 0)
            {
                gsproto.SpecialWeaponReformResetReq req = request as gsproto.SpecialWeaponReformResetReq;
                gsproto.SpecialWeaponReformResetRsp rsp = response as gsproto.SpecialWeaponReformResetRsp;

                SyncPlayerData(rsp.data_syncs);

                RaiseSpecialWeaponReformReset(req.weapon_id);
            }
        }

        static void OnAcquireSkillRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnAcquireSkillRsp " + result);
            if (result == 0)
            {
                gsproto.AcquireSkillReq req = request as gsproto.AcquireSkillReq;
                gsproto.AcquireSkillRsp rsp = response as gsproto.AcquireSkillRsp;

                SpecialSkillData.UpdateSkillInfo(req.skill_gid, req.skill_glevel);

                RaiseOnAcquireSkill(req.skill_gid);
            }
        }

        static void OnEquipSkillRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnEquipSkillRsp " + result);
            if (result == 0)
            {
                gsproto.EquipSkillReq req = request as gsproto.EquipSkillReq;
                gsproto.EquipSkillRsp rsp = response as gsproto.EquipSkillRsp;

                SyncPlayerData(rsp.data_syncs);

                bool isBigSuccess = false;

                Hero hero = GetHero(req.hero_id);
                hero.EquipSpecialSkill(req.skill_gid, req.skill_glevel);

                RaiseOnEquipSkill(req.hero_id, hero.SkillServerId, isBigSuccess);
            }
        }

        static void OnReceiveTaskRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnReceiveTaskRsp " + result);
            if (result == 0)
            {
                gsproto.ReceiveTaskReq req = request as gsproto.ReceiveTaskReq;
                gsproto.ReceiveTaskRsp rsp = response as gsproto.ReceiveTaskRsp;

                Task task = TaskData.GetTask((int)req.task_id);
                task.SetState((uint)PbCommon.ETaskStateType.E_Task_State_Not_Finish);

                RaiseOnNormalTaskDataChange(task.TaskPosition);
            }
        }

        static void OnRefuseTaskRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnRefuseTaskRsp " + result);
            if (result == 0)
            {
                gsproto.RefuseTaskReq req = request as gsproto.RefuseTaskReq;
                gsproto.RefuseTaskRsp rsp = response as gsproto.RefuseTaskRsp;

                TaskData.RemoveTask((int)req.task_id);
                TaskData.AddTask(rsp.task);
                Task task = TaskData.GetTask((int)rsp.task.task_id);
                task.CountdownFinishTime = ServerTime + (uint)DefaultConfig.GetInt("TaskPosCoolTime") * ConstDefine.SECOND_PER_MINUTE;

                RaiseOnNormalTaskDataChange(task.TaskPosition);
            }
        }

        static void OnDrawTaskAwardRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnDrawTaskAwardRsp " + result);
            if (result == 0)
            {
                gsproto.DrawTaskAwardReq req = request as gsproto.DrawTaskAwardReq;
                gsproto.DrawTaskAwardRsp rsp = response as gsproto.DrawTaskAwardRsp;

                SyncPlayerData(rsp.data_syncs);

                CSV_b_task_template taskCSV = CSV_b_task_template.FindData((int)req.task_id);
                uint serverId = 0;

                if (rsp.data_syncs.breads != null && rsp.data_syncs.breads.Count > 0)
                {
                    serverId = rsp.data_syncs.breads[0].id;
                }
                else if (rsp.data_syncs.fruits != null && rsp.data_syncs.fruits.Count > 0)
                {
                    serverId = rsp.data_syncs.fruits[0].id;
                }
                else if (rsp.data_syncs.equips != null && rsp.data_syncs.equips.Count > 0)
                {
                    serverId = rsp.data_syncs.equips[0].id;
                }
                else if (rsp.data_syncs.heros != null && rsp.data_syncs.heros.Count > 0)
                {
                    serverId = rsp.data_syncs.heros[0].id;
                }

                AddTaskAward((uint)taskCSV.AwardType, (uint)taskCSV.AwardValue, serverId);

                Task task = TaskData.GetTask((int)req.task_id);


                if (task.TaskPosition > 0) // 右侧三个 
                {
                    TaskData.RemoveTask(task);

                    RaiseOnNormalTaskDataChange(task.TaskPosition);
                }
                else
                {
                    RaiseOnTaskDataChange(task.CsvId);

                    CheckDailyAndWeeklyAwardState();
                }

                // 检查特殊技能状态 
                SpecialSkillData.CheckSkillState();
                CheckNewSpecialSkillState();
            }
        }

        static void OnGetDailyTaskRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnGetDailyTaskRsp " + result);
            if (result == 0)
            {
                gsproto.GetDailyTaskReq req = request as gsproto.GetDailyTaskReq;
                gsproto.GetDailyTaskRsp rsp = response as gsproto.GetDailyTaskRsp;

                TaskData.RemoveDailyTask();
                TaskData.AddTasks(rsp.tasks);
                TaskData.DailyTaskRefreshTime = rsp.daily_task_refresh_time;

                RaiseOnDailyTaskListChange();
            }
        }

        static void OnGetWeeklyTaskRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnGetWeeklyTaskRsp " + result);
            if (result == 0)
            {
                gsproto.GetWeeklyTaskReq req = request as gsproto.GetWeeklyTaskReq;
                gsproto.GetWeeklyTaskRsp rsp = response as gsproto.GetWeeklyTaskRsp;

                TaskData.RemoveWeeklyTask();
                TaskData.AddTasks(rsp.tasks);
                TaskData.WeeklyTaskRefreshTime = rsp.weekly_task_refresh_time;

                RaiseOnWeeklyTaskListChange();
            }
        }

        static void OnBeginExpeditionRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnBeginExpeditionRsp " + result);
            if (result == 0)
            {
                gsproto.BeginExpeditionReq req = request as gsproto.BeginExpeditionReq;
                gsproto.BeginExpeditionRsp rsp = response as gsproto.BeginExpeditionRsp;

                Expedition expedition = ExpeditionData.GetExpedition((int)req.expedition_quest_id);
                expedition.FinishTime = ServerTime + expedition.Duration;
                expedition.heroIds.AddRange(req.hero_ids);
                for (int i = 0; i < expedition.heroIds.Count; ++i)
                {
                    Hero hero = GetHero(expedition.heroIds[i]);
                    hero.InExpedition = true;
                }

                System.DateTime dateTime = TimeFormater.GetDateTime(expedition.FinishTime);
                AlarmClock.Instance.AddDateAlarm(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, CheckExpeditionFinishState);

                RaiseOnBeginExpedition(expedition.CsvId);
            }
        }

        static void OnEndExpeditionRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnEndExpeditionRsp " + result);
            if (result == 0)
            {
                gsproto.EndExpeditionReq req = request as gsproto.EndExpeditionReq;
                gsproto.EndExpeditionRsp rsp = response as gsproto.EndExpeditionRsp;

                Expedition expedition = ExpeditionData.GetExpedition((int)req.expedition_quest_id);
                ExpeditionData.RemoveExpedition(expedition);
                ExpeditionData.AddExpedition(rsp.new_expedition);

                List<HeroAddExpInfo> heroAddExpInfoList = new List<HeroAddExpInfo>();
                for (int i = 0; i < expedition.heroIds.Count; ++i)
                {
                    Hero hero = GetHero(expedition.heroIds[i]);

                    HeroAddExpInfo heroAddExpInfo = new HeroAddExpInfo();

                    heroAddExpInfo.HeroData = hero;
                    heroAddExpInfo.StartExp = hero.Exp;
                    heroAddExpInfo.StartLevel = hero.Level;

                    hero.AddExp(rsp.hero_exp_got);
                    heroAddExpInfo.EndExp = hero.Exp;
                    heroAddExpInfo.EndLevel = hero.Level;

                    heroAddExpInfoList.Add(heroAddExpInfo);
                }

                for (int i = 0; i < expedition.heroIds.Count; ++i)
                {
                    Hero hero = GetHero(expedition.heroIds[i]);
                    hero.InExpedition = false;
                }

                RaiseOnExpeditionListChange();

                CheckExpeditionFinishState();

                // 经验 
                GroupAddExpInfo groupAddExpInfo = new GroupAddExpInfo();

                groupAddExpInfo.StartExp = Exp;
                groupAddExpInfo.StartLevel = Level;
                AddGroupExp(expedition.GroupExp);
                groupAddExpInfo.EndExp = Exp;
                groupAddExpInfo.EndLevel = Level;

                SyncPlayerData(rsp.data_syncs);

                RaiseOnEndExpedition(groupAddExpInfo, heroAddExpInfoList, ConvertAwardInfoList(rsp.chest_award_info));

                // 任务 
                if (rsp.data_syncs != null && rsp.data_syncs.tasks != null)
                    CheckNormalTaskUpdateRaise(rsp.data_syncs.tasks);
                CheckDailyAndWeeklyAwardState();
            }
        }

        static void OnCancelExpeditionRsp(ushort result, object response, object request)
        {
            //Debug.Log("OnCancelExpeditionRsp " + result);
            if (result == 0)
            {
                gsproto.CancelExpeditionReq req = request as gsproto.CancelExpeditionReq;
                gsproto.CancelExpeditionRsp rsp = response as gsproto.CancelExpeditionRsp;

                Expedition expedition = ExpeditionData.GetExpedition((int)req.expedition_quest_id);

                for (int i = 0; i < expedition.heroIds.Count; ++i)
                {
                    Hero hero = GetHero(expedition.heroIds[i]);
                    hero.InExpedition = false;
                }

                System.DateTime dateTime = TimeFormater.GetDateTime(expedition.FinishTime);
                AlarmClock.Instance.RemoveDateAlarm(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, CheckExpeditionFinishState);

                expedition.FinishTime = 0;
                expedition.heroIds.Clear();

                RaiseOnCancelExpedition(expedition.CsvId);
            }
        }

        #endregion
    }
}

