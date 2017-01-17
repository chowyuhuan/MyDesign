using UnityEngine;
using System.Collections;

public sealed class GUI_BakeryUI_DL : GUI_Window_DL
{
    GUI_LogicObjectPool _BakeryItemPool;
    GameObject GridHelperObject;
    public GUI_GridLayoutGroupHelper_DL GridHelper;
    protected override void OnStart()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/BakeryItem", true, AssetManage.E_AssetType.UIPrefab);
        _BakeryItemPool = new GUI_LogicObjectPool(go);
        GridHelper = GridHelperObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        GridHelper.SetScrollAction(DisplayItem);
        GridHelper.FillPage(CSV_b_bakeries_template.DateCount);
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            CSV_b_bakeries_template bakery = CSV_b_bakeries_template.GetData(scrollItem.LogicIndex);
            GUI_BakeryItem_DL bakeryItem = _BakeryItemPool.GetOneLogicComponent() as GUI_BakeryItem_DL;
            bakeryItem.ShowBakeryItem(bakery);
            scrollItem.SetTarget(bakeryItem);
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_BakeryUI dataComponent = gameObject.GetComponent<GUI_BakeryUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BakeryUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GridHelperObject = dataComponent.GridHelperObject;
        }
    }
}
