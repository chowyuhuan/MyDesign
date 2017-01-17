using UnityEngine;
using System.Collections;
using System;
using SKILL;

public class LineMotion : MotionBase
{
    //Vector2 _from;
    Vector2 _direction;
    float _speed;

    protected override void OnStart()
    {

    }

    public static LineMotion Begin(GameObject go, Vector2 from, Vector2 direction, float time, float speed, RotationStyle rotationStyle, float rotationSpeed, Action<GameObject> motionFinish)
    {
        LineMotion comp = MotionBase.Begin<LineMotion>(go, time, motionFinish);

        comp.SlopAngleAdjust = direction.x < 0 ? 180 : 0;

        //comp._from = from;
        comp._direction = direction.normalized;
        comp._speed = speed;

        comp.Value = from;

        comp.RotationStyleEx = rotationStyle;
        comp.RotationSpeedEx = rotationSpeed;

        comp.Step(0);

        return comp;
    }

    public override float GetSlope()
    {
        return _direction.y / _direction.x;
    }

    protected override void UpdateValue(float deltaTime)
    {
        Value += _direction * _speed * deltaTime;
    }
}
