using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GUI_ExtendToggleItem_DL : GUI_ToggleItem_DL
{
    public UnityEngine.Events.UnityEvent OnSelectItem;
    public UnityEngine.Events.UnityEvent OnDeSelectItem;
    protected override void OnSelected()
    {
        if (OnSelectItem != null)
        {
            OnSelectItem.Invoke();
        }
    }

    protected override void OnDeSelected()
    {
        if (OnDeSelectItem != null)
        {
            OnDeSelectItem.Invoke();
        }
    }

    protected override void OnRecycle()
    {

    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_ExtendToggleItem dataComponent = gameObject.GetComponent<GUI_ExtendToggleItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExtendToggleItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            OnSelectItem = dataComponent.OnSelectItem;
            OnDeSelectItem = dataComponent.OnDeSelectItem;
        }
    }
}
