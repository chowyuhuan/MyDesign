using UnityEngine;
using System.Collections;
using System;

public abstract class GUI_Tweener : MonoBehaviour
{
    public enum Method
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        BounceIn,
        BounceOut,
    }

    public enum Style
    {
        Once,
        Loop,
        PingPong,
    }

    public Method method = Method.Linear;
    public Style style = Style.Once;
    public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
    public bool ignoreTimeScale = true;
    public float delay = 0f;
    public float duration = 1f;
    public bool steeperCurves = false;

    protected bool mStarted = false;
    protected bool mAction = false;  // delay时间过去之后，置为true
    protected float mStartTime = 0f;
    protected float mDuration = 0f;
    protected float mAmountPerDelta = 1000f;
    protected float mFactor = 0f;

    public Action onFinished;

    public float amountPerDelta
    {
        get
        {
            if (mDuration != duration)
            {
                mDuration = duration;
                mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f) * Mathf.Sign(mAmountPerDelta);
            }
            return mAmountPerDelta;
        }
    }

    /// <summary>
    /// Tween factor, 0-1 range.
    /// </summary>

    public float tweenFactor { get { return mFactor; } set { mFactor = Mathf.Clamp01(value); } }

    void Reset()
    {
        if (!mStarted)
        {
            SetStartToCurrentValue();
            SetEndToCurrentValue();
        }
    }

    protected virtual void Start() { Update(); }
    public virtual void Update()
    {

        float delta = GameTimer.deltaTime;
        float time = GameTimer.time;

        if (!mStarted)
        {
            mStarted = true;
            mStartTime = time + delay;
        }

        if (time < mStartTime) return;

        // Advance the sampling factor
        mFactor += amountPerDelta * delta;

        // Loop style simply resets the play factor after it exceeds 1.
        if (style == Style.Loop)
        {
            if (mFactor > 1f)
            {
                mFactor -= Mathf.Floor(mFactor);
            }
        }
        else if (style == Style.PingPong)
        {
            // Ping-pong style reverses the direction
            if (mFactor > 1f)
            {
                mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                mAmountPerDelta = -mAmountPerDelta;
            }
            else if (mFactor < 0f)
            {
                mFactor = -mFactor;
                mFactor -= Mathf.Floor(mFactor);
                mAmountPerDelta = -mAmountPerDelta;
            }
        }

        // If the factor goes out of range and this is a one-time tweening operation, disable the script
        if ((style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f))
        {
            mFactor = Mathf.Clamp01(mFactor);
            Sample(mFactor, true);

            if (onFinished != null)
            {
                onFinished();
            }

            // Disable this script unless the function calls above changed something
            if (duration == 0f || (mFactor == 1f && mAmountPerDelta > 0f || mFactor == 0f && mAmountPerDelta < 0f))
                enabled = false;
        }
        else Sample(mFactor, false);
    }

    void OnDisable() { mStarted = false; }

    /// <summary>
    /// Sample the tween at the specified factor.
    /// </summary>

    public void Sample(float factor, bool isFinished)
    {
        // Calculate the sampling value
        float val = Mathf.Clamp01(factor);

        if (method == Method.EaseIn)
        {
            val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val));
            if (steeperCurves) val *= val;
        }
        else if (method == Method.EaseOut)
        {
            val = Mathf.Sin(0.5f * Mathf.PI * val);

            if (steeperCurves)
            {
                val = 1f - val;
                val = 1f - val * val;
            }
        }
        else if (method == Method.EaseInOut)
        {
            const float pi2 = Mathf.PI * 2f;
            val = val - Mathf.Sin(val * pi2) / pi2;

            if (steeperCurves)
            {
                val = val * 2f - 1f;
                float sign = Mathf.Sign(val);
                val = 1f - Mathf.Abs(val);
                val = 1f - val * val;
                val = sign * val * 0.5f + 0.5f;
            }
        }
        else if (method == Method.BounceIn)
        {
            val = BounceLogic(val);
        }
        else if (method == Method.BounceOut)
        {
            val = 1f - BounceLogic(1f - val);
        }

        // Call the virtual update
        OnUpdate((animationCurve != null) ? animationCurve.Evaluate(val) : val, isFinished);
    }

    /// <summary>
    /// Main Bounce logic to simplify the Sample function
    /// </summary>

    float BounceLogic(float val)
    {
        if (val < 0.363636f) // 0.363636 = (1/ 2.75)
        {
            val = 7.5625f * val * val;
        }
        else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
        {
            val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
        }
        else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
        {
            val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
        }
        else
        {
            val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
        }
        return val;
    }

    public void ResetToBeginning()
    {
        mStarted = false;
        mAction = false;
        mFactor = (amountPerDelta < 0f) ? 1f : 0f;
        Sample(mFactor, false);
    }

    public void PlayForward(Action onfinished = null) { Play(true, onfinished); }
    public void PlayForward(float durationTime, Action onfinished = null) { duration = durationTime; ResetToBeginning(); Play(true, onfinished); }

    public void PlayReverse(Action onfinished = null) { Play(false, onfinished); }
    public void PlayReverse(float durationTime, Action onfinished = null) { duration = durationTime; ResetToBeginning(); Play(false, onfinished); }

    public void Play(bool forward, Action onfinished = null)
    {
        mAmountPerDelta = Mathf.Abs(amountPerDelta);
        if (!forward) mAmountPerDelta = -mAmountPerDelta;
        enabled = true;
        onFinished = onfinished;
        Update();
    }

    static public void PlayTweener(GUI_Tweener tweener, bool forward = true, Action onFinished = null)
    {
        if (null != tweener)
        {
            tweener.Play(forward, onFinished);
        }
    }

    static public T Begin<T>(GameObject go, float duration, Method method, Style style, Action onfinished) where T : GUI_Tweener
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();

            if (comp == null)
            {
                return null;
            }
        }
        comp.onFinished = onfinished;
        comp.mStarted = false;
        comp.mAction = false;
        comp.duration = duration;
        comp.mFactor = 0f;
        comp.mAmountPerDelta = Mathf.Abs(comp.amountPerDelta);
        comp.style = style;
        comp.method = method;
        comp.animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        comp.enabled = true;
        return comp;
    }

    abstract protected void OnUpdate(float factor, bool isFinished);

    public virtual void SetStartToCurrentValue() { }

    public virtual void SetEndToCurrentValue() { }
}
