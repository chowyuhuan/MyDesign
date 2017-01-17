using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using DataCenter;

public class MissionProcess : IBattleProcess
{
    #region Comrade
    List<ActorPrepareInfo> ComradeList = new List<ActorPrepareInfo>();

    GoddessPrepareInfo _comradeGoddess = null;
    #endregion

    #region Enemy
    List<int> _waveIds = new List<int>();        // 总波次Id 

    List<List<ActorPrepareInfo>> _waveDatas = new List<List<ActorPrepareInfo>>();

    int _currentWaveIndex; // 当前波次索引 
    int _hiddenWave; // 隐藏怪波次，大于0有效

    bool _hasBoss;

    bool _hasHidden = false;
    #endregion

    public void Initialize()
    {
        CreateComradeData();
        CreateWaveData();
        CreateComradeGoddessData();
    }

    void CreateComradeData()
    {
        TeamInfo teamInfo = PlayerDataCenter.TeamCollectionData.GetMissionTeam();

        for (int i = 0; i < teamInfo.Members.Count; ++i)
        {
            Hero hero = PlayerDataCenter.GetHero(teamInfo.Members[i]);
            CSV_b_hero_template heroCSV = CSV_b_hero_template.FindData(hero.CsvId);
            CSV_c_school_config schoolCSV = CSV_c_school_config.FindData(heroCSV.School);

            ActorPrepareInfo prepareInfo = new ActorPrepareInfo();

            prepareInfo.ServerId = hero.ServerId;
            prepareInfo.CsvId = hero.CsvId;

            prepareInfo.Level = (int)hero.Level;
            prepareInfo.Star = heroCSV.Star;
            prepareInfo.Name = heroCSV.Name;

            prepareInfo.CampEx = SKILL.Camp.Comrade;

            if (hero.ServerId == teamInfo.LeaderId)
                prepareInfo.IsLeader = true;

            prepareInfo.SpecialSkillId = (int)hero.SkillServerId;

            prepareInfo.NormalRangeId = heroCSV.NormalRangeId;
            prepareInfo.OffsetX = -DefaultConfig.GetFloat("HeroEnterGap") * i;

            prepareInfo.FlyFactor = schoolCSV.FlyFactor;

            prepareInfo.WeaponInfo = hero.GetActorWeaponInfo();

            prepareInfo.PrefabPath = AssetManage.AM_PathHelper.GetActorFullPathByName(heroCSV.Prefab);
            prepareInfo.AnimPath = AssetManage.AM_PathHelper.GetActorAnimCtrlFullPathByName(heroCSV.AnimCtrl);
            prepareInfo.EffectPath = AssetManage.AM_PathHelper.GetActorAnimEffectFullPathByName(heroCSV.AnimEffect);

            prepareInfo.CreateAttributeFiled();
            prepareInfo.SetAttributeFiled(ActorField.HP, hero.TotalAttribute.Hp);
            prepareInfo.SetAttributeFiled(ActorField.HPMax, hero.TotalAttribute.Hp);
            prepareInfo.SetAttributeFiled(ActorField.ATK, hero.TotalAttribute.Attack);
            prepareInfo.SetAttributeFiled(ActorField.DFPhysics, hero.TotalAttribute.PhysicalDefence);
            prepareInfo.SetAttributeFiled(ActorField.DFMagic, hero.TotalAttribute.MagicDefence);

            prepareInfo.SetAttributeFiled(ActorField.CritRate, hero.TotalAttribute.CrticalRate);
            prepareInfo.SetAttributeFiled(ActorField.CritDamage, hero.TotalAttribute.CrticalDamage);
            prepareInfo.SetAttributeFiled(ActorField.Dodge, hero.TotalAttribute.Dodge);
            prepareInfo.SetAttributeFiled(ActorField.Precision, hero.TotalAttribute.Precision);

            prepareInfo.SetAttributeFiled(ActorField.APMagic, hero.TotalAttribute.MagicPenetration);
            prepareInfo.SetAttributeFiled(ActorField.APPhysics, hero.TotalAttribute.PhysicPenetration);
            prepareInfo.SetAttributeFiled(ActorField.Speed, hero.TotalAttribute.AttackSpeed);
            prepareInfo.SetAttributeFiled(ActorField.Suck, hero.TotalAttribute.Suck);
            prepareInfo.SetAttributeFiled(ActorField.DMReduced, hero.TotalAttribute.DamageReduce);

            prepareInfo.NeedAutoCast = false;

            prepareInfo.NormalAttackIDs.AddRange(heroCSV.NormalSkillIDs);

            ComradeList.Add(prepareInfo);
        }
    }

    void CreateWaveData()
    {
        _hasHidden = PlayerDataCenter.EnterLevelData.HasHidden;

        CSV_b_game_level gameLevel = CSV_b_game_level.FindData(PlayerDataCenter.EnterLevelData.PassId);

        _hasBoss = gameLevel.HasBoss;

        _waveIds.Clear();
        _waveIds.AddRange(gameLevel.MonsterWaveIds);

        _currentWaveIndex = 0;

        // 隐藏宝藏怪物 
        if (_hasHidden)
        {
            _hiddenWave = gameLevel.MonsterWaveIds.Count - 1;
            // 插入隐藏宝藏怪物 
            _waveIds.Insert(gameLevel.MonsterWaveIds.Count - 1, gameLevel.HiddenMonsterWave);
        }
        else
        {
            _hiddenWave = -1;
        }

        for (int n = 0; n < _waveIds.Count; ++n)
        {
            List<ActorPrepareInfo> infoList = new List<ActorPrepareInfo>();

            CSV_b_monster_wave waveCSV = CSV_b_monster_wave.FindData(_waveIds[n]);
            for (int i = 0; i < waveCSV.monsterCount; ++i)
            {
                CSV_b_monster_template monsterCSV = CSV_b_monster_template.FindData(waveCSV.monsterIds[i]);

                ActorPrepareInfo prepareInfo = new ActorPrepareInfo();

                prepareInfo.CsvId = waveCSV.monsterIds[i];

                prepareInfo.Level = monsterCSV.Level;
                prepareInfo.Star = monsterCSV.Star;
                prepareInfo.Name = monsterCSV.Name;

                prepareInfo.CampEx = SKILL.Camp.Enemy;

                prepareInfo.IsLeader = false;

                prepareInfo.SpecialSkillId = 0;

                prepareInfo.NormalRangeId = monsterCSV.NormalRangeId;
                prepareInfo.OffsetX = waveCSV.monsterOffsets[i];

                prepareInfo.FlyFactor = monsterCSV.FlyFactor;

                if (!string.IsNullOrEmpty(monsterCSV.WeaponPrefab))
                {
                    ActorWeaponInfo weaponInfo = new ActorWeaponInfo();
                    weaponInfo.prefabPath = monsterCSV.WeaponPrefab;
                    weaponInfo.LocalPosition = monsterCSV.WeaponPosition;
                    weaponInfo.LocalRotation = monsterCSV.WeaponRotation;

                    prepareInfo.WeaponInfo = weaponInfo;
                }

                prepareInfo.PrefabPath = AssetManage.AM_PathHelper.GetActorFullPathByName(monsterCSV.prefab);

                if (!string.IsNullOrEmpty(monsterCSV.AnimCtrl))
                    prepareInfo.AnimPath = AssetManage.AM_PathHelper.GetActorFullPathByName(monsterCSV.AnimCtrl);

                CSV_c_monster_attribute attributeCSV = CSV_c_monster_attribute.FindData(monsterCSV.attributeId);

                prepareInfo.CreateAttributeFiled();
                prepareInfo.SetAttributeFiled(ActorField.HP, attributeCSV.hp);
                prepareInfo.SetAttributeFiled(ActorField.HPMax, attributeCSV.hp);
                prepareInfo.SetAttributeFiled(ActorField.ATK, attributeCSV.attack);
                prepareInfo.SetAttributeFiled(ActorField.DFPhysics, attributeCSV.physicDefense);
                prepareInfo.SetAttributeFiled(ActorField.DFMagic, attributeCSV.magicDefense);

                prepareInfo.SetAttributeFiled(ActorField.CritRate, attributeCSV.crticalRate);
                prepareInfo.SetAttributeFiled(ActorField.CritDamage, attributeCSV.crticalDamage);
                prepareInfo.SetAttributeFiled(ActorField.Dodge, attributeCSV.dodge);
                prepareInfo.SetAttributeFiled(ActorField.Precision, attributeCSV.precision);

                prepareInfo.SetAttributeFiled(ActorField.APMagic, attributeCSV.magicPenetration);
                prepareInfo.SetAttributeFiled(ActorField.APPhysics, attributeCSV.physicPenetration);
                prepareInfo.SetAttributeFiled(ActorField.Speed, attributeCSV.attackSpeed);
                prepareInfo.SetAttributeFiled(ActorField.Suck, attributeCSV.suck);
                prepareInfo.SetAttributeFiled(ActorField.DMReduced, attributeCSV.damageReduce);
                prepareInfo.SetAttributeFiled(ActorField.Parry, attributeCSV.parry);

                prepareInfo.NeedAutoCast = true;

                prepareInfo.NormalAttackIDs.AddRange(monsterCSV.NormalSkillIDs);

                infoList.Add(prepareInfo);
            }

            _waveDatas.Add(infoList);
        }
    }

    void CreateComradeGoddessData()
    {
        TeamInfo teamInfo = PlayerDataCenter.TeamCollectionData.GetMissionTeam();
        if (teamInfo.Goddess > 0)
        {
            CSV_c_goddess_config goddessCSV = CSV_c_goddess_config.FindData((int)teamInfo.Goddess);
            _comradeGoddess = new GoddessPrepareInfo();

            _comradeGoddess.SkillId = goddessCSV.GoddessSkillId;

            _comradeGoddess.PrefabPath = goddessCSV.Prefab;

            _comradeGoddess.Rotation = new Vector3(0, 90, 0);
            
            _comradeGoddess.CampEx = SKILL.Camp.Comrade;
        }
    }

    public List<ActorPrepareInfo> GetComradeList()
    {
        return ComradeList;
    }

    public bool IsEnemyWaveEnd()
    {
        return _currentWaveIndex >= _waveDatas.Count - 1;
    }

    public int CurrentEnemyWave()
    {
        return _currentWaveIndex + 1;
    }

    public int TotalEnemyWave()
    {
        return _waveDatas.Count;
    }

    public void GoNextEnemyWave()
    {
        _currentWaveIndex++;
    }

    public MonsterWaveType GetCurrentEnemyWaveType()
    {
        if (_currentWaveIndex == _waveDatas.Count - 1 && _hasBoss) // boss
        {
            return MonsterWaveType.Boss;
        }
        else if (_hiddenWave == _currentWaveIndex) // 隐藏宝藏怪
        {
            return MonsterWaveType.HiddenMonster;
        }
        else
        {
            return MonsterWaveType.NormalMonster;
        }
    }

    public List<ActorPrepareInfo> GetCurrentEnemyList()
    {
        return _waveDatas[_currentWaveIndex];
    }

    public GoddessPrepareInfo GetComradeGoddess()
    {
        return _comradeGoddess;
    }

    public GoddessPrepareInfo GetEnemyGoddess()
    {
        return null;
    }
}
