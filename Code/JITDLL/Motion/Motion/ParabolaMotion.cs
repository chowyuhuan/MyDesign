using UnityEngine;
using System.Collections;
using System;
using SKILL;

public class ParabolaMotion : MotionBase
{
    Vector2 _from;
    //Vector2 to;
    //float offsetH;
    
    Parabola _parabola = new Parabola();
    float _speed;

    protected override void OnStart()
    {
        
    }

    public static ParabolaMotion Begin(GameObject go, float speed, Vector2 from, Vector2 to, float offsetH, RotationStyle rotationStyle, float rotationSpeed, Action<GameObject> motionFinish)
    {
        ParabolaMotion comp = MotionBase.Begin<ParabolaMotion>(go, float.MaxValue, motionFinish); // 曲线没有时限，假定曲线必然会命中或者落地

        comp.SlopAngleAdjust = (to.x < from.x) ? 180 : 0;

        comp._from = from;
        //comp.to = to;
        //comp.offsetH = offsetH;

        comp._speed = from.x < to.x ? speed : -speed;
        //comp.value.x = from.x;
        comp.Value = from;

        float py = offsetH > 0 ? Mathf.Max(from.y, to.y) + offsetH : Mathf.Min(from.y, to.y) + offsetH;
        comp._parabola.Set(from.x, from.y, to.x, to.y, py, offsetH > 0);

        comp.RotationStyleEx = rotationStyle;
        comp.RotationSpeedEx = rotationSpeed;

        comp.Step(0);

        return comp;
    }

    public override float GetSlope()
    {
        return _parabola.GetSlope(Value.x);
    }

    protected override void UpdateValue(float deltaTime)
    {
        Value.x += _speed * deltaTime;
        Value.y = _parabola.GetY(Value.x);
    }

    protected override void Step(float deltaTime)
    {
        base.Step(deltaTime);

        // 如果落地,则到通知时间了 
        if (Value.y < 0 && Value.x != _from.x)
        {
            Duration = 0;
        }
    }
}
