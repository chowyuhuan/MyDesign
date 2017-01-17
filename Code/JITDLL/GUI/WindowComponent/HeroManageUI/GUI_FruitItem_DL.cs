using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_FruitItem_DL : GUI_TrainItem_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_FruitItem dataComponent = gameObject.GetComponent<GUI_FruitItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_FruitItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {

        }
    }
    #endregion

    #region item logic
    public DataCenter.Fruit Fruit{get; protected set;}
    public CSV_b_fruit_template FruitTemplate{get; protected set;}
    public delegate void OnSelectOperation(GUI_FruitItem_DL fruitItem);
    OnSelectOperation OnFruitSelect;
    OnSelectOperation OnFruitDeSelect;
    public delegate bool SellingItem();
    SellingItem SellingFruitItem;
    public delegate bool FruitSelected(DataCenter.Fruit fruit);
    FruitSelected FruitItemSelected;

    public void DisplayFruit(DataCenter.Fruit fruit, OnSelectOperation onFruitSelect, OnSelectOperation onFruitDeselect, SellingItem sellingItem, FruitSelected fruitItemSelect)
    {
        Fruit = fruit;
        OnFruitSelect = onFruitSelect;
        OnFruitDeSelect = onFruitDeselect;
        SellingFruitItem = sellingItem;
        FruitItemSelected = fruitItemSelect;
        FruitTemplate = CSV_b_fruit_template.FindData(fruit.CsvId);
        SetItemData(FruitTemplate.IconAtlas, FruitTemplate.IconAtlas, FruitTemplate.Name, FruitTemplate.AttributeValue, FruitTemplate.SuccessRate, FruitTemplate.SaleCoin);
        RefreshObject();
    }

    protected override void OnSelected()
    {
        if(null != OnFruitSelect)
        {
            OnFruitSelect(this);
        }
    }

    protected override void OnDeSelected()
    {
        if (null != OnFruitSelect)
        {
            OnFruitDeSelect(this);
        }
    }

    public override void RefreshObject()
    {
        if(null != SellingFruitItem)
        {
            SellItem(SellingFruitItem());
        }
        if(FruitItemSelected(Fruit))
        {
            Select();
        }
        else
        {
            DeSelect();
        }
    }

    protected override void OnRecycle()
    {
        Fruit = null;
        FruitTemplate = null;
        OnFruitSelect = null;
        OnFruitDeSelect = null;
        SellingFruitItem = null;
        FruitItemSelected = null;
    }
    #endregion
}
