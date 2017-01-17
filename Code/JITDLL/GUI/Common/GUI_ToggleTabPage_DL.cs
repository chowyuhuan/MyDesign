using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GUI_ToggleTabPage_DL : GUI_ToggleItem_DL
{
    public Text PageName;
    GameObject ScrollGroupObject;
    public GUI_GroupLayoutHelper_DL LayoutHelper;
    public ToggleGroup ItemToggleGroup;
    GameObject _ToggleItemProto;
    List<GUI_ToggleItem_DL> _ToggleItems = new List<GUI_ToggleItem_DL>();
    Action<int> _SelectPage;
    int _PageIndex;
    public bool UseDefaultSelect;

    public void Init(int pageIndex, GUI_GroupLayoutHelper_DL layoutHelper, Action<int> onSelectPage, string pageName, bool useDefalutSelect = true)
    {
        _PageIndex = pageIndex;
        LayoutHelper = layoutHelper;
        _SelectPage = onSelectPage;
        if(!string.IsNullOrEmpty(pageName)
            && null != PageName)
        {
            PageName.text = pageName;
        }
        UseDefaultSelect = useDefalutSelect;
    }

    public void SelectItem(int itemIndex)
    {
        GUI_ScrollItem si = LayoutHelper.LocateAtIndex(itemIndex);
        if (null != si)
        {
            GUI_ToggleItem_DL ti = si.LogicObject as GUI_ToggleItem_DL;
            ti.Select();
        }
    }

    public void ShowItem(GUI_ToggleItem_DL ti)
    {
        _ToggleItems.Add(ti);
        ti.RegistToGroup(ItemToggleGroup);
    }

    public void DeselectAllItems()
    {
        for (int index = 0; index < _ToggleItems.Count; ++index)
        {
            if (_ToggleItems[index].IsSelect)
            {
                _ToggleItems[index].DeSelect();
            }
        }
    }

    public void ItemAsButton()
    {
        for (int index = 0; index < _ToggleItems.Count; ++index)
        {
            _ToggleItems[index].AsButton();
        }
    }

    public void ItemAsToggle()
    {
        for (int index = 0; index < _ToggleItems.Count; ++index)
        {
            _ToggleItems[index].AsToggle();
        }
    }

    public void RefreshPage()
    {
        OnSelected();
    }

    protected override void OnDeSelected()
    {
        //UnRegistToggleGroup();
        _ToggleItems.Clear();
    }

    protected override void OnSelected()
    {
        UnRegistToggleGroup();
        _ToggleItems.Clear();
        LayoutHelper.Clear();
        DisplayPage();
        RegistToggleGroup();
        if (UseDefaultSelect && _ToggleItems.Count > 0)
        {
            _ToggleItems[0].Select();
        }
    }

    protected override void OnRecycle()
    {

    }

    void RegistToggleGroup()
    {
        for (int index = 0; index < _ToggleItems.Count; ++index)
        {
            _ToggleItems[index].RegistToGroup(ItemToggleGroup);
        }
    }

    void UnRegistToggleGroup()
    {
        for (int index = 0; index < _ToggleItems.Count; ++index)
        {
            _ToggleItems[index].RegistToGroup(null);
        }
    }

    void DisplayPage()
    {
        if (null != _SelectPage)
        {
            _SelectPage(_PageIndex);
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ToggleTabPage dataComponent = gameObject.GetComponent<GUI_ToggleTabPage>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ToggleTabPage,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            PageName = dataComponent.PageName;
            ScrollGroupObject = dataComponent.ScrollGroupObject;
            ItemToggleGroup = dataComponent.ItemToggleGroup;
            UseDefaultSelect = dataComponent.UseDefaultSelect;
        }
    }
}
