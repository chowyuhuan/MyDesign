using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public sealed class GUI_HeroOrderMenuItem_DL : GUI_ToggleItem_DL
{
    Action<int> _OnSelect;
    public Text MenuText;
    public E_Hero_OrderType OrderType;
    CompareComponent _CompareFunc;
    public CompareComponent CompareFunc
    {
        get
        {
            if (null == _CompareFunc)
            {
                _CompareFunc = GUI_HeroSimpleInfo_DL.GetCompareFunc(OrderType);
            }
            return _CompareFunc;
        }
    }

    public void SetOrderMenuItem(Action<int> onselect, int toggleIndex)
    {
        _OnSelect = onselect;
        ToggleIndex = toggleIndex;
    }

    protected override void OnSelected()
    {
        if (null != _OnSelect)
        {
            _OnSelect(ToggleIndex);
        }
    }

    protected override void OnDeSelected()
    {
    }

    protected override void OnRecycle()
    {
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroOrderMenuItem dataComponent = gameObject.GetComponent<GUI_HeroOrderMenuItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroOrderMenuItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            MenuText = dataComponent.MenuText;
            OrderType = dataComponent.OrderType;
        }
    }
}
