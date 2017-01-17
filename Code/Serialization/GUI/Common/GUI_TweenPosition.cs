using UnityEngine;
using System.Collections;
using System;

public class GUI_TweenPosition : GUI_Tweener
{

    public Vector3 from;
    public Vector3 to;

    [HideInInspector]
    public bool worldSpace = false;

    Transform mTrans;

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    public Vector3 value
    {
        get
        {
            return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
        }
        set
        {
            if (worldSpace)
            {
                cachedTransform.position = value;
            }
            else
            {
                cachedTransform.localPosition = value;
            }
        }
    }

    protected override void OnUpdate(float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

    public void Play(Vector3 startPosition, Vector3 targetPosition, Action onFinished = null)
    {
        from = startPosition;
        to = targetPosition;
        ResetToBeginning();
        Play(true, onFinished);
    }

    public void Play(Vector3 targetPosition, Action onFinished = null)
    {
        from = value;
        to = targetPosition;
        Play(true, onFinished);
    }

    static public GUI_TweenPosition Begin(GameObject go, float duration, Vector3 pos, Method method, Style style, Action onfinished = null)
    {
        GUI_TweenPosition comp = GUI_Tweener.Begin<GUI_TweenPosition>(go, duration, method, style, onfinished);
        comp.from = comp.value;
        comp.to = pos;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    static public GUI_TweenPosition Begin(GameObject go, float duration, Vector3 pos, bool worldSpace, Method method, Style style, Action onfinished = null)
    {
        GUI_TweenPosition comp = GUI_Tweener.Begin<GUI_TweenPosition>(go, duration, method, style, onfinished);
        comp.worldSpace = worldSpace;
        comp.from = comp.value;
        comp.to = pos;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue() { from = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue() { to = value; }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart() { value = from; }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }
}