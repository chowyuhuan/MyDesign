using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

/// <summary>
/// 击飞击退的运动控制
/// </summary>
public class ActorFly : MonoCompBase
{
    float _flyFactor = 1;

    public enum FlyType
    {
        // 速度随时间减少 
        Fade,
        // 速度持续一段时间 
        Time,
        // 速度持续一段距离
        Distance,
        // 速度持续到目的地 
        Destination,
    }

    public enum MotionMode
    {
        /// <summary>
        /// 自主运动(如自己技能前冲)
        /// </summary>
        Active,
        /// <summary>
        /// 外部施加(如被击退等) 
        /// </summary>
        Passive,
    }

    public class VelocityData
    {
        public MotionMode MotionModeEx;
        public FlyType FlyTypeEx;
        public float Value;         // 速度 
        public float Param;         // Fade:加速度;Time:时间;Distance:距离;Destination:距离
        public float ActiveSpeed;

        bool _end = false;

        public void Update(float deltaTime)
        {
            switch (FlyTypeEx)
            {
                case FlyType.Fade:
                    if (Value > 0)
                    {
                        Value -= Param * deltaTime;
                        if (Value <= ActiveSpeed)
                        {
                            _end = true;
                        }
                    }
                    else
                    {
                        Value += Param * deltaTime;
                        if (Value >= -ActiveSpeed)
                        {
                            _end = true;
                        }
                    }

                    break;

                case FlyType.Time:
                    Param -= deltaTime;
                    if (Param <= 0)
                    {
                        _end = true;
                    }

                    break;

                case FlyType.Distance:

                    Param -= (Value > 0 ? Value : -Value) * deltaTime;
                    if (Param <= 0)
                    {
                        _end = true;
                    }

                    break;

                case FlyType.Destination:

                    Param -= (Value > 0 ? Value : -Value) * deltaTime;
                    if (Param <= 0)
                    {
                        _end = true;
                    }

                    break;
            }
        }

        public bool IsEnd()
        {
            return _end;
        }
    }

    float _xAcc = 3f;

    // x轴上人物运行速度
    float _xActiveSpeed = 2f;

    public override void Init(Actor a)
    {
        base.Init(a);

        _xAcc = DefaultConfig.GetFloat("HorizontalAccelerate");
        _xActiveSpeed = DefaultConfig.GetFloat("HorizontalAdditive");

        _flyFactor = Owner.actorPrepareInfo.FlyFactor;
    }

    /// <summary>
    /// 施放者特殊运动 
    /// 关于param: Fade时无用;Time时为持续时间;Distance时为持续距离;Destination时为目的地x坐标;
    /// </summary>
    /// <param name="flyType">效果类型</param>
    /// <param name="x">x轴初速度，正值人物向前，负值人物向后</param>
    /// <param name="y">y轴初速度，为正值</param>
    /// <param name="param">特殊参数</param>
    public void CasterMotion(FlyType flyType, float x, float y, float param)
    {
        Fly(MotionMode.Active, flyType, -x, y, param);
    }

    /// <summary>
    /// 受击者特殊运动  
    /// 关于param: Fade时无用;Time时为持续时间;Distance时为持续距离;Destination时为目的地x坐标;
    /// </summary>
    /// <param name="flyType">效果类型</param>
    /// <param name="x">x轴初速度，正值人物向后，负值人物向前</param>
    /// <param name="y">y轴初速度，为正值</param>
    /// <param name="param">特俗参数</param>
    public void HitTargetMotion(FlyType flyType, float x, float y, float param)
    {
        if (!Owner.ActorReference.ActorControlEx.HasActorState<ImmuneKnockbackState>())
        {
            Fly(MotionMode.Passive, flyType, x, y, param);
        }
    }

    /// <summary>
    /// 击飞相关接口. 
    /// 关于param: Fade时无用;Time时为持续时间;Distance时为持续距离;Destination时为目的地x坐标;
    /// </summary>
    /// <param name="motionMode">释放类别</param>
    /// <param name="flyType">效果类型</param>
    /// <param name="x">x轴初速度，正值人物向后飞，负值人物向前飞</param>
    /// <param name="y">y轴初速度，为正值</param>
    /// <param name="param">特俗参数</param>
    void Fly(MotionMode motionMode, FlyType flyType, float x, float y, float param)
    {
        VelocityData vd = new VelocityData();

        vd.MotionModeEx = motionMode;
        vd.FlyTypeEx = flyType;
        vd.ActiveSpeed = _xActiveSpeed;

        switch (flyType)
        {
            case FlyType.Fade:
                vd.Value = x * ((Owner.SelfCamp == Camp.Comrade) ? -1 : 1);
                if (motionMode == MotionMode.Passive)
                    vd.Value *= _flyFactor;
                vd.Param = _xAcc;
                break;

            case FlyType.Time:
                vd.Value = x * ((Owner.SelfCamp == Camp.Comrade) ? -1 : 1);
                if (motionMode == MotionMode.Passive)
                    vd.Value *= _flyFactor;
                vd.Param = param;
                break;

            case FlyType.Distance:
                vd.Value = x * ((Owner.SelfCamp == Camp.Comrade) ? -1 : 1);
                vd.Param = param;

                break;

            case FlyType.Destination:
                if (param > transform.position.x)
                {
                    vd.Value = x;
                }
                else
                {
                    vd.Value = -x;
                }
                vd.Param = Mathf.Abs(param - transform.position.x);

                break;
        }

        if (vd.Value > 0) vd.Value += _xActiveSpeed;
        else vd.Value -= _xActiveSpeed;

        MotionState motionState = new MotionState();
        motionState.velocityData = vd;

        if (y > 0)
        {
            motionState.YSpeed = y * _flyFactor;
        }

        Owner.ActorReference.ActorControlEx.AddState(motionState);
    }
}
