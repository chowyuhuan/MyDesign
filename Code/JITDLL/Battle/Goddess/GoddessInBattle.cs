using UnityEngine;
using System.Collections;
using System;
using SKILL;

public class GoddessInBattle
{
    int _goddessId;
    int _skillId;

    int _sp;
    int _skillNeedSp = 50;
    int _totalSkillCount = 3;
    int _maxSp;
    int[] _spAdd = new int[3];

    bool _canCast = false;

    public Action<int> OnGoddessSpChange;

    GoddessControl goddessControl;

    Team _team;

    public void Initialize(GoddessPrepareInfo goddessPrepareInfo, Team team)
    {
        _team = team;

        _totalSkillCount = DefaultConfig.GetInt("GoddessSkillCount");
        _skillNeedSp = DefaultConfig.GetInt("GoddessSkillSp");
        _spAdd[0] = DefaultConfig.GetInt("GoddessSkillSpAdd1");
        _spAdd[1] = DefaultConfig.GetInt("GoddessSkillSpAdd2");
        _spAdd[2] = DefaultConfig.GetInt("GoddessSkillSpAdd3");
        _maxSp = _skillNeedSp * _totalSkillCount;

        SpawnGoddess(goddessPrepareInfo, team);
    }

    void SpawnGoddess(GoddessPrepareInfo goddessPrepareInfo, Team team)
    {
        GameObject go = EntityPool.Spwan(goddessPrepareInfo.PrefabPath) as GameObject;
        go.transform.rotation = Quaternion.Euler(goddessPrepareInfo.Rotation);

        goddessControl = go.AddComponent<GoddessControl>();
        goddessControl.Initialize(goddessPrepareInfo, team, this);
    }

    public void SetGoddessSpeedRate(float speedRate, float duration)
    {
        goddessControl.SetSpeedRate(speedRate, duration);
    }

    public void AddGoddessSpByErase(int eraseCount)
    {
        AddGoddessSpByValue(_spAdd[eraseCount - 1]);
    }

    public void AddGoddessSpByValue(int spValue)
    {
        _sp += spValue;
        if (_sp > _maxSp)
        {
            _sp = _maxSp;
        }

        if (OnGoddessSpChange != null)
        {
            OnGoddessSpChange(_sp);
        }

        //Debug.Log("GoddessSp " + _sp);
        CheckReadyParticles();
    }

    public void CastGoddessSkill()
    {
        if (_sp >= _skillNeedSp)
        {
            _sp -= _skillNeedSp;
            if (OnGoddessSpChange != null)
            {
                OnGoddessSpChange(_sp);
            }

            //Debug.Log("GoddessSp " + _sp);

            goddessControl.PlayAnimation("skill");
        }
    }

    public void CheckReadyParticles()
    {
        goddessControl.SetReadyParticles(_sp >= _skillNeedSp);
    }
}
