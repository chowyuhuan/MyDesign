using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_BakeDoneUI_DL : GUI_Window_DL
{
    GameObject GridHelperObject;
    public GUI_GridLayoutGroupHelper_DL GridHelper;
    GUI_LogicObjectPool _BakeProductItemPool;
    Dictionary<int, int> _BreadDictionary = new Dictionary<int, int>();

    public void DisplayBreadList(List<immortaldb.Bread> breadList)
    {
        GridHelper = GridHelperObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        if (null != breadList)
        {
            GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Bakery_Product_Item", true, AssetManage.E_AssetType.UIPrefab);
            _BakeProductItemPool = new GUI_LogicObjectPool(go);
            GridHelper.SetScrollAction(DisplayItem);

            for (int index = 0; index < breadList.Count; ++index)
            {
                int breadCsvId = (int)breadList[index].template_id;
                if (!_BreadDictionary.ContainsKey(breadCsvId))
                {
                    _BreadDictionary.Add(breadCsvId, 1);
                    GridHelper.FillItem(breadCsvId);
                }
                else
                {
                    ++_BreadDictionary[breadCsvId];
                }
            }
            GridHelper.FillItemEnd();
        }
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            GUI_BakeProductItem_DL breadItem = _BakeProductItemPool.GetOneLogicComponent() as GUI_BakeProductItem_DL;
            breadItem.SetItem(_BreadDictionary[scrollItem.LogicIndex], scrollItem.LogicIndex);
            scrollItem.SetTarget(breadItem);
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_BakeDoneUI dataComponent = gameObject.GetComponent<GUI_BakeDoneUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BakeDoneUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GridHelperObject = dataComponent.GridHelperObject;
            dataComponent.ConfirmButton.onClick.AddListener(HideWindow);
        }
    }
}