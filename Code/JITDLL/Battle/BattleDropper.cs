using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropContent
{
    public int gold;
    public int chest;
    public int chestType;

    public override string ToString()
    {
        return string.Format("gold {0} chest {1}", gold, chest);
    }
}

public class BattleDropper
{
    protected class MonsterAsset
    {
        public int gold = 0;
        public bool hasChest = false;
    }

    /// <summary>
    /// 活动掉落箱子数量 
    /// </summary>
    protected int _activeChestCount;
    /// <summary>
    /// 隐藏怪掉落箱子数量 
    /// </summary>
    protected int _hiddenChestCount;
    /// <summary>
    /// boss掉落箱子数量 
    /// </summary>
    protected int _bossChestCount;
    /// <summary>
    /// 普通怪掉落箱子数量 
    /// </summary>
    protected int _normalChestCount;
    /// <summary>
    /// 关卡金币总数 
    /// </summary>
    protected int _totalGold;
    /// <summary>
    /// boss掉落金币总数 
    /// </summary>
    protected int _bossTotalGold;
    /// <summary>
    /// 小怪掉落金币总数 
    /// </summary>
    protected int _normalTotalGold;
    /// <summary>
    /// 每个小怪掉落金币数 
    /// </summary>
    protected List<int> _normalGolds = new List<int>();
    /// <summary>
    /// 每个boss掉落金币数 
    /// </summary>
    protected List<int> _bossGolds = new List<int>();
    /// <summary>
    /// 隐藏怪掉落金币数 
    /// </summary>
    protected int _hiddenTotalGold;

    protected List<MonsterAsset> _normalMonsterAssets = new List<MonsterAsset>();

    protected List<MonsterAsset> _bossMonsterAssets = new List<MonsterAsset>();

    int _getGolds = 0;
    List<gsproto.ChestInfo> _getChestInfoList = new List<gsproto.ChestInfo>();
    List<gsproto.KillMonsterInfo> _getMonsterInfoList = new List<gsproto.KillMonsterInfo>();

    int _monsterDropCoinCountId;
    string[] coinPrefabPaths = new string[3];
    string[] chestPrefabPaths = new string[3];
    float _treasureSpeed = 5;

    public void DummyEnterData(int levelId)
    {
        DataCenter.PassEnterInfo enterData = DataCenter.PlayerDataCenter.EnterLevelData;

        enterData.PassId = (int)levelId;
        enterData.SkillList = null;
        enterData.HasHidden = false;
        enterData.NormalCoin = 300;
        enterData.HiddenCoin = 200;
        enterData.BossChest = 2;
        enterData.ActiveChest = 1;
        enterData.NormalChest = 2;
    }

    public virtual void Initialize()
    {
        _monsterDropCoinCountId = DefaultConfig.GetInt("MonsterDropCoinCountId");

        coinPrefabPaths[0] = "Treasure/" + DefaultConfig.GetString("TreasureCoin1");
        coinPrefabPaths[1] = "Treasure/" + DefaultConfig.GetString("TreasureCoin2");
        coinPrefabPaths[2] = "Treasure/" + DefaultConfig.GetString("TreasureCoin3");

        chestPrefabPaths[0] = "Treasure/" + DefaultConfig.GetString("TreasureChest1");
        chestPrefabPaths[1] = "Treasure/" + DefaultConfig.GetString("TreasureChest2");
        chestPrefabPaths[2] = "Treasure/" + DefaultConfig.GetString("TreasureChest3");

        _treasureSpeed = DefaultConfig.GetFloat("MonsterTreasureSpeed");

        DataCenter.PassEnterInfo enterData = DataCenter.PlayerDataCenter.EnterLevelData;

        CSV_b_game_level gameLevelCSV = CSV_b_game_level.FindData(enterData.PassId);

        for (int n = 0; n < gameLevelCSV.MonsterWaveIds.Count; ++n)
        {
            CSV_b_monster_wave waveCSV = CSV_b_monster_wave.FindData(gameLevelCSV.MonsterWaveIds[n]);
            for (int i = 0; i < waveCSV.monsterCount; ++i)
            {
                MonsterAsset monsterAsset = new MonsterAsset();

                CSV_b_monster_template monsterCSV = CSV_b_monster_template.FindData(waveCSV.monsterIds[i]);
                if (monsterCSV.MonsterType == (int)PbCommon.EMonsterType.E_Monster_Type_Normal)
                {
                    if (i == waveCSV.monsterCount - 1)
                    {
                        monsterAsset.hasChest = true;
                    }
                    _normalMonsterAssets.Add(monsterAsset);
                }
                else if (monsterCSV.MonsterType == (int)PbCommon.EMonsterType.E_Monster_Type_Boss)
                {
                    _bossMonsterAssets.Add(monsterAsset);
                }
            }
        }

        if (_bossMonsterAssets.Count > 0)
        {
            _bossMonsterAssets[_bossMonsterAssets.Count - 1].hasChest = true;
        }

        // 金币 
        _hiddenTotalGold = enterData.HiddenCoin;
        _totalGold = enterData.NormalCoin;

        //Debug.Log(string.Format("normalGold: {0} hiddenGold: {1}", _totalGold, _hiddenTotalGold));
        if (_bossMonsterAssets.Count > 0 && _normalMonsterAssets.Count > 0)
        {
            _bossTotalGold = (int)(_totalGold * (DefaultConfig.GetFloat("BossDropCoinRate") + Random.Range(-9, 10))/ 100);
            _normalTotalGold = _totalGold - _bossTotalGold;
        }
        else if (_normalMonsterAssets.Count == 0)
        {
            _bossTotalGold = _totalGold;
        }
        else
        {
            _normalTotalGold = _totalGold;
        }

        if (_normalMonsterAssets.Count > 0)
        {
            _normalGolds = MathHelper.RandomAverageDivide(_normalTotalGold, _normalMonsterAssets.Count);
            AssignGold(_normalMonsterAssets, _normalGolds);
        }
        if (_bossMonsterAssets.Count > 0)
        {
            _bossGolds = MathHelper.RandomAverageDivide(_bossTotalGold, _bossMonsterAssets.Count);
            AssignGold(_bossMonsterAssets, _bossGolds);
        }

        // 箱子 
        _hiddenChestCount = enterData.HasHidden ? 1 : 0;
        _bossChestCount = enterData.BossChest;
        _activeChestCount = enterData.ActiveChest;
        _normalChestCount = enterData.NormalChest;
    }

    void AssignGold(List<MonsterAsset> monsterAssets, List<int> golds)
    {
        for (int i = 0; i < monsterAssets.Count && i < golds.Count; ++i)
        {
            monsterAssets[i].gold = golds[i];
        }
    }

    public virtual DropContent NormalDropContent()
    {
        DropContent dropContent = new DropContent();

        MonsterAsset monsterAsset = _normalMonsterAssets[0];
        _normalMonsterAssets.RemoveAt(0);

        dropContent.gold = monsterAsset.gold;
        if (monsterAsset.hasChest)
        {
            if (_activeChestCount > 0)
            {
                _activeChestCount--;
                dropContent.chest = 1;
                dropContent.chestType = (int)PbCommon.EChestType.E_Chest_Active;
            }
            else if (_normalChestCount > 0)
            {
                _normalChestCount--;
                dropContent.chest = 1;
                dropContent.chestType = (int)PbCommon.EChestType.E_Chest_Normal;
            }
        }

        return dropContent;
    }

    public virtual DropContent BossDropContent()
    {
        DropContent dropContent = new DropContent();

        MonsterAsset monsterAsset = _bossMonsterAssets[0];
        _bossMonsterAssets.RemoveAt(0);

        dropContent.gold = monsterAsset.gold;
        if (monsterAsset.hasChest)
        {
            if (_bossChestCount > 0)
            {
                dropContent.chest = _bossChestCount;
                dropContent.chestType = (int)PbCommon.EChestType.E_Chest_Normal;
            }
        }

        return dropContent;
    }

    public virtual DropContent HiddenDropContent()
    {
        DropContent dropContent = new DropContent();

        dropContent.gold = _hiddenTotalGold;
        dropContent.chest = _hiddenChestCount;
        dropContent.chestType = (int)PbCommon.EChestType.E_Chest_Hidden;

        return dropContent;
    }

    public virtual void MonsterDropAsset(Actor actor)
    {
        DropContent dropContent = null;
        CSV_b_monster_template monsterCSV = CSV_b_monster_template.FindData(actor.ConfigId);
        switch (monsterCSV.MonsterType)
        {
            case (int)PbCommon.EMonsterType.E_Monster_Type_Normal:
                dropContent = NormalDropContent();
                break;

            case (int)PbCommon.EMonsterType.E_Monster_Type_Boss:
                dropContent = BossDropContent();
                break;

            case (int)PbCommon.EMonsterType.E_Monster_Type_Hidden:
                dropContent = HiddenDropContent();
                break;
        }

        //Debug.Log("DropContent: " + dropContent.ToString());
        _getGolds += dropContent.gold;

        // 掉落金币 
        int coinDivideCount = CSV_b_random_count_group.GetRandomCount(_monsterDropCoinCountId);
        List<int> coinDivideList = MathHelper.RandomDivide(dropContent.gold, coinDivideCount);

        for (int i = 0; i < coinDivideList.Count; ++i)
        {
            if (coinDivideList[i] == 0)
            {
                continue;
            }

            int prefabIndex = 0;
            if (coinDivideList[i] < 10)
            {
                prefabIndex = 0;
            }
            else if (coinDivideList[i] < 100)
            {
                prefabIndex = 1;
            }
            else
            {
                prefabIndex = 2;
            }

            GameObject coinGo = EntityPool.Spwan(coinPrefabPaths[prefabIndex]) as GameObject;
            coinGo.transform.position = actor.transform.position + Vector3.up * 5;

            TreasureFall.Begin(coinGo, _treasureSpeed, GetTreasureDropDirection(), TreasureFall.TreasureType.Coin, coinDivideList[i]);
        }

        // 掉落箱子 
        if (dropContent.chest > 0)
        {
            for (int i = 0; i < dropContent.chest; ++i)
            {
                gsproto.ChestInfo chestInfo = new gsproto.ChestInfo();
                chestInfo.chest_type = (uint)dropContent.chestType;
                _getChestInfoList.Add(chestInfo);

                int prefabIndex = 0;
                switch (dropContent.chestType)
                {
                    case (int)PbCommon.EChestType.E_Chest_Normal:
                        prefabIndex = 0;
                        break;

                    case (int)PbCommon.EChestType.E_Chest_Hidden:
                        prefabIndex = 1;
                        break;

                    case (int)PbCommon.EChestType.E_Chest_Active:
                        prefabIndex = 2;
                        break;
                }

                GameObject chestGo = EntityPool.Spwan(chestPrefabPaths[prefabIndex]) as GameObject;
                chestGo.transform.position = actor.transform.position + Vector3.up * 5;

                TreasureFall.Begin(chestGo, _treasureSpeed, GetTreasureDropDirection(), TreasureFall.TreasureType.Chest, 1);
            }
        }
    }

    Vector2 GetTreasureDropDirection()
    {
        float angle = Random.Range(1, DefaultConfig.GetInt("MonsterTreasureAngleRange"));
        angle = angle * Mathf.Deg2Rad;
        float cosValue = Mathf.Cos(angle);
        float sinValue = Mathf.Sin(angle);

        Vector2 dir = new Vector2(cosValue, sinValue);

        return dir;
    }

    public void RecordKillMonster(Actor actor)
    {
        for (int i = 0 ;i < _getMonsterInfoList.Count; ++i)
        {
            if (_getMonsterInfoList[i].monster_template_id == actor.ConfigId)
            {
                _getMonsterInfoList[i].monster_count++;
                return;
            }
        }

        gsproto.KillMonsterInfo killMonsterInfo = new gsproto.KillMonsterInfo();
        killMonsterInfo.monster_template_id = (uint)actor.ConfigId;
        killMonsterInfo.monster_count = 1;

        _getMonsterInfoList.Add(killMonsterInfo);
    }

    public void AttachPassOverReq(ref gsproto.PassOverReq req, PbCommon.EPassResult battleResult)
    {
        req.monster_info.AddRange(_getMonsterInfoList); 

        if (battleResult != PbCommon.EPassResult.E_Result_Exit)
        {
            req.chest_list.AddRange(_getChestInfoList);
            req.gold_coin = (uint)_getGolds;
        }
    }

    public void ShowFinalDropContent()
    {
        int getNormalChest = 0;
        int getHiddenChest = 0;
        int getActiveChest = 0;
        for (int i = 0; i < _getChestInfoList.Count; ++i)
        {
            switch (_getChestInfoList[i].chest_type)
            {
                case (uint)PbCommon.EChestType.E_Chest_Normal:
                    getNormalChest += 1;
                    break;

                case (uint)PbCommon.EChestType.E_Chest_Hidden:
                    getHiddenChest += 1;
                    break;

                case (uint)PbCommon.EChestType.E_Chest_Active:
                    getActiveChest += 1;
                    break;
            }
        }

        Debug.Log(string.Format("TotalGold: {0} normalChest: {1} hiddenChese: {2} activeChest: {3}", _getGolds, getNormalChest, getHiddenChest, getActiveChest));
    }
}
