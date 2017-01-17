using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class GUI_GroupLayoutHelper_DL : MonoBehaviour {
    public bool UseConfigCellSize;
    public Vector2 CellSize;
    public LayoutGroup LayoutComponent;
    public RectTransform ContentRect;
    public ScrollRect ScrollViewRect;
    public RectTransform ScrollCullRect;

    protected List<GUI_ScrollItem> _ScrollItems = new List<GUI_ScrollItem>();
    public int ItemCount
    {
        get { return _ScrollItems.Count; }
    }

    GUI_LogicObjectPool _ScrollItemPool;
    Action<GUI_ScrollItem> _ScrollAction;

    protected RectTransform ViewRect;

    void Awake()
    {
        CopyDataFromDataScript();
        InitHelper();
        OnAwake();
    }

    void InitHelper()
    {
        GameObject scrollItem = new GameObject("ScrollItem", typeof(RectTransform));
        scrollItem.AddComponent<GUI_ScrollItem>();
        scrollItem.AddComponent<RectMask2D>();
        _ScrollItemPool = new GUI_LogicObjectPool(scrollItem);
        ScrollViewRect.onValueChanged.AddListener(OnScroll);
        ViewRect = ScrollViewRect.GetComponent<RectTransform>();
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void CopyDataFromDataScript()
    {
        GUI_GroupLayoutHelper dataComponent = gameObject.GetComponent<GUI_GroupLayoutHelper>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GroupLayoutHelper,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            UseConfigCellSize = dataComponent.UseConfigCellSize;
            CellSize = dataComponent.CellSize;
            LayoutComponent = dataComponent.LayoutComponent;
            ContentRect = dataComponent.ContentRect;
            ScrollViewRect = dataComponent.ScrollViewRect;
            ScrollCullRect = dataComponent.ScrollCullRect;
        }
    }

    public void SetScrollAction(Action<GUI_ScrollItem> scrollAction)
    {
        _ScrollAction = scrollAction;
    }

    public void RefreshPageData()
    {
        for (int index = 0; index < _ScrollItems.Count; ++index)
        {
            _ScrollItems[index].RefreshObject();
        }
    }

    public void FillPage(int totalCount)
    {
        Clear();
        for (int index = 0; index < totalCount; ++index)
        {
            FillItem(index);
        }
        FillItemEnd();
    }

    public virtual void RefreshLayout()
    {

    }

    public void FillItem(int index)
    {
        GUI_ScrollItem newItem = _ScrollItemPool.GetOneLogicComponent() as GUI_ScrollItem;
        newItem.AnchorItem(index, _ScrollAction, this);
        GUI_Tools.CommonTool.AddUIChild(LayoutComponent.gameObject, newItem.CachedGameObject, false);
        if(UseConfigCellSize)
        {
            newItem.ItemLayout.preferredHeight = CellSize.y;
            newItem.ItemLayout.preferredWidth = CellSize.x;
        }
        _ScrollItems.Add(newItem);
        OnFillItem(newItem);
    }

    protected virtual void OnFillItem(GUI_ScrollItem scrollItem)
    {

    }

    public void FillItemEnd()
    {
        OnFillItemEnd();
        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentRect);
        StartCoroutine(UpdateItems());
    }

    protected virtual void OnFillItemEnd()
    {

    }

    IEnumerator UpdateItems()
    {
        yield return Yielders.EndOfFrame;
        RefreshItems();
    }

    void RefreshItems()
    {
        for (int index = 0; index < _ScrollItems.Count; ++index)
        {
            Vector2 pos = GUI_Root_DL.Instance.UICamera.WorldToScreenPoint(_ScrollItems[index].CachedTransform.position);
            if (RectTransformUtility.RectangleContainsScreenPoint(ScrollCullRect, pos, GUI_Root_DL.Instance.UICamera))
            {
                _ScrollItems[index].OnEnterScrollView();
            }
            else
            {
                _ScrollItems[index].OnOutScrollView();
            }
        }
    }

    public void Clear()
    {
        for (int index = 0; index < _ScrollItems.Count; ++index)
        {
            _ScrollItems[index].Recycle();
        }
        _ScrollItems.Clear();
    }

    public void ResetDisplay()
    {
        for (int index = 0; index < _ScrollItems.Count; ++index)
        {
            _ScrollItems[index].ResetDisplay();
        }
    }

    public void RefreshDisplay()
    {
        for (int index = 0; index < _ScrollItems.Count; ++index)
        {
            _ScrollItems[index].RefreshObject();
        }
    }

    public GUI_ScrollItem GetAtIndex(int index)
    {
        if (index >= 0 && index < _ScrollItems.Count)
        {
            return _ScrollItems[index];
        }
        return null;
    }

    public abstract GUI_ScrollItem LocateAtIndex(int index);


    int GetRange(int index, int rangeLimit, out int rangeValue)
    {
        int range = index / rangeLimit;
        rangeValue = index % rangeLimit;
        if (rangeValue > 0)
        {
            range++;
        }
        return range;
    }

    public void OnScroll(Vector2 eventData)
    {
        RefreshItems();
    }

    public void Sort(CompareComponent compareFunc, bool decrease)
    {
        if (null != compareFunc)
        {
            ResetDisplay();
            for (int index = 0; index < _ScrollItems.Count; ++index)
            {
                BubbleUp(compareFunc, index, decrease);
            }
            RefreshItems();
            //UpdateItems();
        }
    }

    void BubbleUp(CompareComponent compareFunc, int startIdex, bool decrease)
    {
        if (null != compareFunc && startIdex < _ScrollItems.Count && startIdex >= 0)
        {
            int bubblePos = startIdex;
            for (int index = startIdex + 1; index < _ScrollItems.Count; ++index)
            {
                int compareResult = compareFunc(_ScrollItems[bubblePos].LogicIndex, _ScrollItems[index].LogicIndex);
                if (decrease && compareResult < 0)
                {
                    bubblePos = index;
                }
                else if (!decrease && compareResult > 0)
                {
                    bubblePos = index;
                }
            }
            SwapScrollItem(startIdex, bubblePos);
        }
    }

    void SwapScrollItem(int a, int b)
    {
        if (a != b)
        {
            int aLogic = _ScrollItems[a].LogicIndex;
            _ScrollItems[a].AnchorLogic(_ScrollItems[b].LogicIndex);
            _ScrollItems[b].AnchorLogic(aLogic);
        }
    }
}
