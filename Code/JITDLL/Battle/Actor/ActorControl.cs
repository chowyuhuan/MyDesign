using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;

public class ActorControl : MonoCompBase
{
    public Action<Actor> OnReachDestination;

    List<ActorState> stateList = new List<ActorState>();
    Dictionary<System.Type, ActorState> stateDic = new Dictionary<Type, ActorState>();

    float _speedRate = 1f;
    public float SpeedRate
    {
        get
        {
            return SpeedRate;
        }
        set
        {
            _speedRate = value;
            if (_speedRate < 0)
            {
                _speedRate = 0;
            }

            SetActorStateSpeedRate(_speedRate);
            Owner.ActorReference.ActorAnimEx.Speed = _speedRate;
        }
    }

    public void Initialize(int normalRangeId)
    {
        NormalAttackCheckState normalAttackCheckState = new NormalAttackCheckState();
        normalAttackCheckState.SetNormalRangeId(normalRangeId);
        AddState(normalAttackCheckState);

        MoveHorizontalState moveHorizontalActiveState = new MoveHorizontalState();
        moveHorizontalActiveState.SetNormalRangeId(normalRangeId);
        AddState(moveHorizontalActiveState);

        InitAnimationState();
    }

    void InitAnimationState()
    {
        Owner.ActorReference.ActorAnimEx.Play("run");
    }

    public void SetDestination(float dst)
    {
        MoveDestinationState dstState = new MoveDestinationState();

        dstState.Destination = dst;

        AddState(dstState);
    }

    public void ClearDestination()
    {
        RemoveState(typeof(MoveDestinationState));
    }

    public void RaiseOnReachDestination()
    {
        if (OnReachDestination != null)
        {
            OnReachDestination(Owner);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Owner.IsDeath) return;

        for (int i = 0; i < stateList.Count; ++i)
        {
            stateList[i].UpdateState();
        }
	}

    public void AddState(ActorState state)
    {
        ActorState exist = null;
        stateDic.TryGetValue(state.GetCacheType(), out exist);
        if (exist != null)
        {
            exist.CreateDataWithNewState(state);
        }
        else
        {
            stateList.Add(state);
            stateDic.Add(state.GetCacheType(), state);

            state.AddOwner(Owner);
        }
    }

    public void RemoveState(ActorState state)
    {
        stateList.Remove(state);
        stateDic.Remove(state.GetCacheType());

        state.RemoveOwner();

        if (state.next != null)
        {
            AddState(state.next);
        }
    }

    public void RemoveState(System.Type stateType)
    {
        ActorState exist = null;
        stateDic.TryGetValue(stateType, out exist);
        if (exist != null)
        {
            RemoveState(exist);
        }
    }

    public T GetActorState<T>() where T : ActorState
    {
        ActorState s = GetActorState(typeof(T));

        return s as T;
    }

    public ActorState GetActorState(System.Type stateType)
    {
        ActorState exist = null;
        stateDic.TryGetValue(stateType, out exist);
        if (exist != null)
        {
            return exist;
        }

        return null;
    }

    public bool HasActorState<T>() where T : ActorState
    {
        return stateDic.ContainsKey(typeof(T));
    }

    public bool CanMoveHorizontal()
    {
        for (int i = 0; i < stateList.Count; ++i)
        {
            if (!stateList[i].CanMoveHorizontal())
            {
                return false;
            }
        }

        return true;
    }

    public bool CanMotionActive()
    {
        for (int i = 0; i < stateList.Count; ++i)
        {
            if (!stateList[i].CanMotionActive())
            {
                return false;
            }
        }

        return true;
    }

    public bool CanCastSkill()
    {
        for (int i = 0; i < stateList.Count; ++i)
        {
            if (!stateList[i].CanCastSkill())
            {
                return false;
            }
        }

        return true;
    }

    public bool CanNormalAttack()
    {
        for (int i = 0; i < stateList.Count; ++i)
        {
            if (!stateList[i].CanNormalAttack())
            {
                return false;
            }
        }

        return true;
    }

    void SetActorStateSpeedRate(float speedRate)
    {
        for (int i = 0; i < stateList.Count; ++i)
        {
            stateList[i].SetSpeedRate(speedRate);
        }
    }
}
