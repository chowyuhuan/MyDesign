using UnityEngine;
using System.Collections;
using System;

public enum ETargetCountType
{
    Single,
    Multiple,
}
public enum ETweenType
{
    UI,
    Sprite,
}
//[RequireComponent(typeof(CanvasRenderer))]
public class GUI_TweenAlpha : GUI_Tweener
{

    [Range(0f, 1f)]
    public float from = 1f;
    [Range(0f, 1f)]
    public float to = 1f;

    bool mCached = false;
    CanvasGroup _CanvasGroup;
    CanvasRenderer _CanvasRender;
    SpriteRenderer _SpriteRender;
    SpriteRenderer[] _SpriteRenderGroup;
    public ETargetCountType _TargetCountType = ETargetCountType.Single;
    public ETweenType TweenType = ETweenType.UI;


    void Cache()
    {
        mCached = true;
        if (TweenType == ETweenType.UI)
        {
            if (_TargetCountType == ETargetCountType.Single)
            {
                _CanvasRender = GetComponent<CanvasRenderer>();
            }
            else
            {
                _CanvasGroup = GetComponent<CanvasGroup>();
                if (null == _CanvasGroup)
                {
                    _CanvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }
        else
        {
            if (_TargetCountType == ETargetCountType.Single)
            {
                _SpriteRender = GetComponent<SpriteRenderer>();
            }
            else
            {
                _SpriteRenderGroup = GetComponentsInChildren<SpriteRenderer>(true);
            }
        }
    }

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public float value
    {
        get
        {
            if (!mCached) Cache();
            if (TweenType == ETweenType.UI)
            {
                if (_TargetCountType == ETargetCountType.Single)
                {
                    return _CanvasRender.GetAlpha();
                }
                else
                {
                    return _CanvasGroup.alpha;
                }
            }
            else
            {
                return _SpriteRender.color.a;
            }
        }
        set
        {
            if (!mCached) Cache();
            if (TweenType == ETweenType.UI)
            {
                if (_TargetCountType == ETargetCountType.Single)
                {
                    _CanvasRender.SetAlpha(value);
                }
                else
                {
                    _CanvasGroup.alpha = value;
                }
            }
            else
            {
                if (_TargetCountType == ETargetCountType.Single)
                {
                    _SpriteRender.material.color = new Color(_SpriteRender.color.r, _SpriteRender.color.g, _SpriteRender.color.b, value);
                }
                else
                {
                    for (int index = 0; index < _SpriteRenderGroup.Length; ++index)
                    {
                        Color newColor = new Color(_SpriteRenderGroup[index].color.r, _SpriteRenderGroup[index].color.g, _SpriteRenderGroup[index].color.b, value);
                        _SpriteRenderGroup[index].material.color = newColor;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public GUI_TweenAlpha Begin(GameObject go, float duration, float alpha, Method method, Style style, Action onfinished = null)
    {
        GUI_TweenAlpha comp = GUI_Tweener.Begin<GUI_TweenAlpha>(go, duration, method, style, onfinished);
        comp.from = comp.value;
        comp.to = alpha;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    public override void SetStartToCurrentValue() { from = value; }
    public override void SetEndToCurrentValue() { to = value; }
}