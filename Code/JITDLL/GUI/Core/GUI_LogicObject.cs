using UnityEngine;
using System.Collections;

public abstract class GUI_LogicObject : MonoBehaviour
{
    GUI_LogicObjectPool _Controller;
    public Transform CachedTransform { get; protected set; }

    public GameObject CachedGameObject { get; protected set; }
    public void Init(GUI_LogicObjectPool lop)
    {
        _Controller = lop;
        CachedTransform = transform;
        CachedGameObject = gameObject;
        OnInit();
    }

    public void Recycle()
    {
        OnRecycle();
        _Controller.RecycleOneLogicComponent(this);
        CachedTransform.SetParent(null);
    }

    abstract protected void OnRecycle();

    protected virtual void OnInit()
    {

    }

    public virtual void RefreshObject()
    {

    }

    protected int GetSelfIndexInUsingList()
    {
        return _Controller.GetLogicObjectUsingIndex(this);
    }
}