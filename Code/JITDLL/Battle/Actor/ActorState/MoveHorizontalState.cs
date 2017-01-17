using UnityEngine;
using System.Collections;
using SKILL;
using ACTOR;

public class MoveHorizontalState : ActorState
{
    float _xSpeed;
    float _finalSpeed;

    float _bestX;
    float _step;
    float _dis;

    int _moveRight;
    bool _needBack;
    float _runFastDistance;

    float _bestNormalAttackRange = 3;

    NormalAttackCheckState _nacs;
    NormalAttackCheckState _normalAttackCheckState
    {
        get
        {
            if (_nacs == null)
            {
                _nacs = Owner.ActorReference.ActorControlEx.GetActorState(typeof(NormalAttackCheckState)) as NormalAttackCheckState;
            }

            return _nacs;
        }
    }

    int _normalRangeId;

    public void SetNormalRangeId(int normalRangeId)
    {
        _normalRangeId = normalRangeId;
    }

    public override void EnterState()
    {
        _xSpeed = DefaultConfig.GetFloat("ActorMoveSpeed");
        _runFastDistance = DefaultConfig.GetFloat("RunFastDistance");

        CSV_c_normal_range normalRangeCSV = CSV_c_normal_range.FindData(_normalRangeId);
        _bestNormalAttackRange = normalRangeCSV.bestRange;
    }

    float GetBestNormalAttackX()
    {
        float ret;

        if (Owner.SelfCamp == Camp.Comrade)
        {
            ret = ActorManager.Instance.GetTeam(Camp.Enemy).GetFirstActorX() - _bestNormalAttackRange;
        }
        else
        {
            ret = ActorManager.Instance.GetTeam(Camp.Comrade).GetFirstActorX() + _bestNormalAttackRange;
        }

        return ret;
    }

    public override void UpdateState()
    {
        if (Owner.ActorReference.ActorControlEx.CanMoveHorizontal())
        {
            _finalSpeed = _xSpeed;

            _bestX = GetBestNormalAttackX();
            _moveRight = _bestX > Owner.ActorReference.ActorMovementEx.Position.x ? 1 : -1;

            _needBack = false;
            if (Mathf.Abs(_bestX - Owner.ActorReference.ActorMovementEx.Position.x) < _bestNormalAttackRange)
            {
                _needBack = true;
            }

            // 前进时距离前线太远的话加速
            if (Owner.SelfCamp == Camp.Comrade)
            {
                float frontline = ActorManager.Instance.GetFrontline(Owner.SelfCamp);
                if (Mathf.Abs(frontline - Owner.transform.position.x) > _runFastDistance)
                {
                    _finalSpeed = _xSpeed * 3;
                }
            }

            _step = _finalSpeed * _moveRight * GameTimer.deltaTime * _speedRate;
            _dis = _bestX - Owner.transform.position.x;
            if (Mathf.Abs(_dis) <= Mathf.Abs(_step))
            {
                Owner.ActorReference.ActorMovementEx.MovePosition(new Vector2(_bestX, Owner.transform.position.y));
            }
            else
            {
                Owner.ActorReference.ActorMovementEx.MovePosition(new Vector2(Owner.transform.position.x + _step, Owner.transform.position.y));
            }
        }
    }
}
