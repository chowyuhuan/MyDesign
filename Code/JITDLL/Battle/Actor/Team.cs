using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SKILL;
using ACTOR;

public class Team
{
    List<Actor> _actors = new List<Actor>();

    Camp _camp;

    public Camp CampEx
    {
        get
        {
            return _camp;
        }
    }

    Actor _leader;
    public Actor Leader
    {
        get
        {
            return _leader;
        }
    }

    public Action<int> OnGoddessSpChange;

    GoddessInBattle goddessInBattle = null;

    public float Frontline
    {
        private set;
        get;
    }

    public bool HasGoddess
    {
        get
        {
            return goddessInBattle != null;
        }
    }

    public void Initialize(Camp camp)
    {
        _camp = camp;

        if (_camp == Camp.Enemy)
            Frontline = float.MaxValue;
        else
            Frontline = float.MinValue;
    }

    public void InitializeGoddess(GoddessPrepareInfo goddessPrepareInfo)
    {
        goddessInBattle = new GoddessInBattle();
        goddessInBattle.Initialize(goddessPrepareInfo, this);
        goddessInBattle.OnGoddessSpChange = RaiseOnGoddessSpChange;
    }

    public void SetGoddessSpeedRate(float speedRate, float duration)
    {
        if (goddessInBattle != null)
        {
            goddessInBattle.SetGoddessSpeedRate(speedRate, duration);
        }
    }

    public void AddGoddessSpByErase(int eraseCount)
    {
        if (goddessInBattle != null)
        {
            goddessInBattle.AddGoddessSpByErase(eraseCount);
        }
    }

    public void AddGoddessSpByValue(int spValue)
    {
        if (goddessInBattle != null)
        {
            goddessInBattle.AddGoddessSpByValue(spValue);
        }
    }

    public void CastGoddessSkill()
    {
        if (goddessInBattle != null)
        {
            goddessInBattle.CastGoddessSkill();
        }
    }

    void RaiseOnGoddessSpChange(int sp)
    {
        if (OnGoddessSpChange != null)
        {
            OnGoddessSpChange(sp);
        }
    }

    public void Clear()
    {
        _actors.Clear();

        if (_camp == Camp.Enemy)
            Frontline = float.MaxValue;
        else
            Frontline = float.MinValue;
    }

    public Actor FindActor(int id)
    {
        for (int i = 0; i < _actors.Count; ++i)
        {
            if (_actors[i].BattleId == id)
            {
                return _actors[i];
            }
        }

        return null;
    }

    public float GetFirstActorX()
    {
        if (_actors.Count > 0)
        {
            return _actors[0].transform.position.x;
        }

        if (_camp == Camp.Enemy)
            return float.MaxValue;
        else
            return float.MinValue;
    }

    public bool Death()
    {
        return _actors.Count == 0;
    }

    public void Add(Actor actor, bool isLeader)
    {
        actor.ActorReference.ActorMovementEx.OnMovePosition += OnActorMovePosition;
        actor.OnDeath += OnActorDeath;

        _actors.Add(actor);

        actor.TeamEx = this;

        if (isLeader)
        {
            _leader = actor;
        }

        SortActor();
    }

    public void Remove(Actor actor)
    {
        actor.ActorReference.ActorMovementEx.OnMovePosition -= OnActorMovePosition;
        actor.OnDeath -= OnActorDeath;

        _actors.Remove(actor);

        actor.TeamEx = null;

        if (_actors.Count > 0)
        {
            // 队长死了 
            if (_leader != null &&
                _leader.BattleId == actor.BattleId)
            {
                _leader = _actors[0];
            }

            SortActor();
        }
        else
        {
            _leader = null;

            if (_camp == Camp.Enemy)
            {
                Frontline = float.MaxValue;
            }
            else
            {
                Frontline = float.MinValue;
            }
        }
    }

    void SortActor()
    {
        if (_camp == Camp.Enemy)
        {
            _actors.Sort(SortPositionRight);

            Frontline = _actors[0].ActorReference.ActorPreDef.XMin;
        }
        else
        {
            _actors.Sort(SortPositionLeft);

            Frontline = _actors[0].ActorReference.ActorPreDef.XMax;
        }
    }

    public int SortPositionRight(Actor a, Actor b)
    {
        ActorMovement ma = a.ActorReference.ActorMovementEx;
        ActorMovement mb = b.ActorReference.ActorMovementEx;

        if (ma.Position.x > mb.Position.x)
            return 1;
        else if (ma.Position.x < mb.Position.x)
            return -1;
        else
            return 0;
    }

    public int SortPositionLeft(Actor a, Actor b)
    {
        return -SortPositionRight(a, b);
    }

    /// <summary>
    /// 技能选择用 
    /// </summary>
    /// <param name="self">自己(只对Target.Partner有用)</param>
    /// <param name="type">类型</param>
    /// <param name="x">x轴范围</param>
    /// <returns></returns>
    public Actor[] Choose(Actor self, Target type, float x)
    {
        if (_actors.Count == 0) return null;

        // 不受选择范围影响的 
        switch (type)
        {
            case Target.All:
                return _actors.ToArray();
                // 多个

            case Target.Partner:
                List<Actor> partners = new List<Actor>();
                for (int i = 0; i < _actors.Count; ++i)
                {
                    if (_actors[i].BattleId != self.BattleId)
                    {
                        partners.Add(_actors[i]);
                    }
                }

                return partners.ToArray();

            case Target.Leader:
                return new Actor[1] { _leader };

            case Target.LowestHP:
                {
                    float lowestHp = float.MaxValue;
                    int index = 0;
                    float temp = 0;
                    for (int i = 0; i < _actors.Count; ++i)
                    {
                        float hp = _actors[i].GetValue(ActorField.HP);
                        float hpM = _actors[i].GetValue(ActorField.HPMax);
                        temp = hp / hpM;
                        if (temp < lowestHp)
                        {
                            index = i;
                            lowestHp = temp;
                        }
                    }

                    return new Actor[1] { _actors[index] };
                }

            case Target.HighestHPMax:
                {
                    float highestHpMax = 0;
                    int index = 0;
                    for (int i = 0; i < _actors.Count; ++i)
                    {
                        float hpMax = _actors[i].GetValue(ActorField.HPMax);
                        if (hpMax > highestHpMax)
                        {
                            index = i;
                            highestHpMax = hpMax;
                        }
                    }

                    return new Actor[1] { _actors[index] };
                }

                // 一个
        }

        // 受选择范围影响的 
        return ChooseByX(type, x, x);
    }

    /// <summary>
    /// 普通攻击选择用 
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="reachX">x轴接触范围</param>
    /// <param name="maxX">x轴最大范围</param>
    /// <returns></returns>
    public Actor[] Choose(Target type, float reachX, float maxX)
    {
        return ChooseByX(type, reachX, maxX);
    }

    Actor[] ChooseByX(Target type, float reachX, float maxX)
    {
        if (_actors.Count == 0) return null;

        List<Actor> cans = new List<Actor>();

        // 先检测接触范围 
        {
            ActorMovement move = _actors[0].ActorReference.ActorMovementEx;
            if (_camp == Camp.Comrade)
            {
                if (maxX < reachX &&
                    move.Position.x < reachX)
                {
                    return null;
                }
            }
            else
            {
                if (maxX > reachX &&
                    move.Position.x > reachX)
                {
                    return null;
                }
            }
        }

        for (int i = 0; i < _actors.Count; ++i)
        {
            ActorMovement move = _actors[i].ActorReference.ActorMovementEx;
            if (_camp == Camp.Comrade)
            {
                if (move.Position.x < maxX)
                {
                    break;
                }
            }
            else
            {
                if (move.Position.x > maxX)
                {
                    break;
                }
            }

            cans.Add(_actors[i]);
        }

        if (cans.Count > 0)
        {
            switch (type)
            {
                case Target.Foward:
                    return new Actor[1] { cans[0] };
                // 一个

                case Target.Middle:
                    return new Actor[1] { cans[cans.Count / 2] };
                // 一个

                case Target.Back:
                    return new Actor[1] { cans[cans.Count - 1] };
                // 一个
            }
        }


        return null;
    }

    void OnActorMovePosition(Actor actor)
    {
        SortActor();
    }

    void OnActorDeath(Actor actor)
    {
        Remove(actor);
    }

    //public void SetSpeedRate(float speedRate)
    //{
    //    for (int i = 0; i < _actors.Count; ++i)
    //    {
    //        _actors[i].ActorReference.ActorControlEx.SetActorSpeedRate(speedRate);
    //    }
    //}
}
