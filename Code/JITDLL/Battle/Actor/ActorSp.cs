using UnityEngine;
using System;
using System.Collections;
using SKILL;
using ACTOR;

/// <summary>
/// Sp进度变化数据 
/// 目前想法英雄手动Sp与怪自动Sp都通过这个接口
/// </summary>
public class ActorSp : MonoCompBase
{
    float _spProgress = 0;
    /// <summary>
    /// 当前Sp进度 
    /// </summary>
    public float Value
    {
        protected set
        {
            _spProgress = value;
            SpPoint = Mathf.RoundToInt(_spProgress * 100);
        }
        get
        {
            return _spProgress;
        }
    }

    public int SpPoint
    {
        protected set;
        get;
    }

    /// <summary>
    /// Sp进度回调  
    /// </summary>
    public Action<float> OnSpProgressChange;

    public override void Init(Actor a)
    {
        base.Init(a);

        Value = 0;
    }

    public virtual void IncreaseSp(int amount)
    {

    }

    public virtual void ReduceSp(int amount)
    {

    }
}
