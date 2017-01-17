using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_LogicObjectPool
{
    protected GameObject _Proto;
    List<GUI_LogicObject> _UsingList = new List<GUI_LogicObject>();
    Stack<GUI_LogicObject> _RecycleList = new Stack<GUI_LogicObject>();
    int _IncreaseStep = 0;

    public GUI_LogicObjectPool(GameObject proto, int initialCount = 0, int increaseStep = 1)
    {
#if UNITY_EDITOR
        Debug.Assert(null != proto);
#endif
        _Proto = proto;
        _IncreaseStep = increaseStep;
        InCreaseLogicPool(initialCount);
    }

    void InCreaseLogicPool(int count)
    {
        for (int index = 0; index < count; ++index)
        {
            GUI_LogicObject lc = GameObject.Instantiate(_Proto).GetComponent<GUI_LogicObject>();
            lc.Init(this);
            _RecycleList.Push(lc);
        }
    }

    public GUI_LogicObject GetOneLogicComponent()
    {
        if (_RecycleList.Count < 1)
        {
            InCreaseLogicPool(_IncreaseStep);
        }
        GUI_LogicObject lc = _RecycleList.Pop();
        _UsingList.Add(lc);
        return lc;
    }

    public void RecycleOneLogicComponent(GUI_LogicObject lc)
    {
        if (null != lc)
        {
            _UsingList.Remove(lc);
            _RecycleList.Push(lc);
        }
    }

    public void RecycleAll()
    {
        for (int index = 0; index < _UsingList.Count; )
        {
            _UsingList[index].Recycle();
        }
    }

    public void ClearPool()
    {
        for (int index = 0; index < _UsingList.Count; ++index)
        {
            GameObject.Destroy(_UsingList[index]);
        }
        _UsingList.Clear();

        while (_RecycleList.Count > 0)
        {
            GUI_LogicObject lc = _RecycleList.Pop();
            GameObject.Destroy(lc);
        }
        _RecycleList.Clear();
    }

    public int GetLogicObjectUsingIndex(GUI_LogicObject lo)
    {
        return _UsingList.IndexOf(lo);
    }
}