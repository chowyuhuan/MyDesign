using UnityEngine;
using System.Collections;
using System;

public class GUI_TweenScale : GUI_Tweener
{
    public Vector3 from = Vector3.one;
    public Vector3 to = Vector3.one;
    public bool updateTable = false;

    protected Transform mTrans;

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    public Vector3 value { get { return cachedTransform.localScale; } set { cachedTransform.localScale = value; } }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = from * (1f - factor) + to * factor;
    }

    static public GUI_TweenScale Begin(GameObject go, float duration, Vector3 scale, Method method, Style style, Action onfinished = null)
    {
        GUI_TweenScale comp = GUI_Tweener.Begin<GUI_TweenScale>(go, duration, method, style, onfinished);
        comp.from = comp.value;
        comp.to = scale;

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