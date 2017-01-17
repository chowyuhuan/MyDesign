using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MonsterWaveType
{
    NormalMonster,
    HiddenMonster,
    Boss,
}

public interface IBattleProcess
{
    void Initialize();

    List<ActorPrepareInfo> GetComradeList();

    bool IsEnemyWaveEnd();

    int CurrentEnemyWave();

    int TotalEnemyWave();

    void GoNextEnemyWave();

    MonsterWaveType GetCurrentEnemyWaveType();

    List<ActorPrepareInfo> GetCurrentEnemyList();

    GoddessPrepareInfo GetComradeGoddess();

    GoddessPrepareInfo GetEnemyGoddess();
}
