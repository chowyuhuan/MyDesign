using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


public sealed class GUI_ScrollItem : GUI_LogicObject
{
    int _ItemIndex;
    public int LogicIndex { get; protected set; }
    public GUI_LogicObject LogicObject { get; protected set; }
    public RectTransform CachedRectTransform { get; protected set; }
    public LayoutElement ItemLayout { get; protected set; }

    Action<GUI_ScrollItem> _ScrollAction;
    GUI_GroupLayoutHelper_DL LayouController;

    protected override void OnInit()
    {
        CachedRectTransform = GetComponent<RectTransform>();
        ItemLayout = CachedGameObject.AddComponent<LayoutElement>();
    }

    protected override void OnRecycle()
    {
        _ItemIndex = -1;
        LogicIndex = -1;
        LayouController = null;
        if (null != LogicObject)
        {
            LogicObject.Recycle();
            LogicObject = null;
        }
    }

    public void SetTarget(GUI_LogicObject target)
    {
        LogicObject = target;
        if (null != LogicObject)
        {
            GUI_Tools.CommonTool.AddUIChild(CachedGameObject, LogicObject.CachedGameObject, false);
        }
    }

    public void ResetDisplay()
    {
        OnOutScrollView();
        LogicIndex = _ItemIndex;
    }

    public void AnchorLogic(int logicIndex)
    {
        LogicIndex = logicIndex;
    }

    public void AnchorItem(int itemIndex, Action<GUI_ScrollItem> scrollAction, GUI_GroupLayoutHelper_DL layoutController)
    {
        _ItemIndex = itemIndex;
        LogicIndex = itemIndex;
        LogicObject = null;
        _ScrollAction = scrollAction;
        LayouController = layoutController;
    }

    public void SetDirty()
    {
        if(null != LayouController)
        {
            LayouController.RefreshLayout();
        }
    }

    public void FocusOn()
    {
        if (null != LayouController)
        {
            LayouController.LocateAtIndex(CachedTransform.GetSiblingIndex());
        }
    }

    public override void RefreshObject()
    {
        if (null != LogicObject)
        {
            LogicObject.RefreshObject();
        }
    }

    public void OnEnterScrollView()
    {
        if (null != _ScrollAction && null == LogicObject)
        {
            _ScrollAction(this);
            return;
        }
    }

    public void OnOutScrollView()
    {
        if (null != LogicObject)
        {
            LogicObject.Recycle();
            LogicObject = null;
        }
    }
}
