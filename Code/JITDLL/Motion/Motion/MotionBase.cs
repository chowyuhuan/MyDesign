using UnityEngine;
using System;
using System.Collections;
using SKILL;

public class MotionBase : MonoBehaviour {

    float _speedRate = 1f;
    public float SpeedRate
    {
        get
        {
            return _speedRate;
        }
        set
        {
            _speedRate = value;
            if (_speedRate < 0)
            {
                _speedRate = 0;
            }
        }
    }

    protected RotationStyle RotationStyleEx = RotationStyle.None;

    protected int SlopAngleAdjust = 0;

    protected float RotationSpeedEx;

    protected float Duration = 1f;
    float _delay = 0f;

    public Action<GameObject> OnFinished;

    protected bool Started = false;
    protected float StartTime = 0f;

    protected Vector2 Value;

    bool NotifyFinished = false;

    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {

    }

    public void Update()
    {
        float time = GameTimer.time;

        if (!Started)
        {
            Started = true;
            StartTime = time + _delay;
        }

        if (time < StartTime) return;

        Step(GameTimer.deltaTime * _speedRate);

        if (!NotifyFinished &&
            time >= StartTime + Duration)
        {
            if (OnFinished != null)
            {
                OnFinished(gameObject);
            }

            NotifyFinished = true;
        }
    }

    protected virtual void Step(float deltaTime)
    {
        UpdateValue(deltaTime);

        transform.position = Value;

        if (RotationStyleEx != RotationStyle.None)
        {
            if (RotationStyleEx == RotationStyle.Slope)
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(GetSlope()) * Mathf.Rad2Deg + SlopAngleAdjust);
            }
            else
            {
                transform.Rotate(Vector3.forward, -RotationSpeedEx * deltaTime);
            }
        }
    }

    protected virtual void UpdateValue(float deltaTime)
    {

    }

    public virtual float GetSlope()
    {
        return 0;
    }

    protected static T Begin<T> (GameObject go, float duration, Action<GameObject> motionFinish) where T : MotionBase
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }

        comp.Duration = duration;

        comp.Started = false;
        comp.NotifyFinished = false;
        comp.OnFinished = motionFinish;

        return comp;
    }
}
