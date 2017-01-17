using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class GUI_TrainItem_DL : GUI_ToggleItem_DL
{
    public Image Icon;
    public Text Name;
    public Text AttributeValue;
    public Text BigSuccessRate;
    public Text SellPrice;
    public GameObject SellPriceRoot;

    protected void SetItemData(string iconAtlas, string spriteName, string itemName, int attributeValue, int bigSuccessValue, int sellPrice)
    {
        GUI_Tools.IconTool.SetIcon(iconAtlas, spriteName, Icon);
        Name.text = itemName;
        AttributeValue.text = attributeValue.ToString();
        BigSuccessRate.text = bigSuccessValue.ToString();
        SellPrice.text = sellPrice.ToString();
        SellItem(false);
    }

    protected void SetItemData(string iconAtlas, string spriteName, string itemName, float attributeValue, int bigSuccessValue, int sellPrice)
    {
        GUI_Tools.IconTool.SetIcon(iconAtlas, spriteName, Icon);
        Name.text = itemName;
        AttributeValue.text = attributeValue.ToString();
        BigSuccessRate.text = bigSuccessValue.ToString();
        SellPrice.text = sellPrice.ToString();
        SellItem(false);
    }

    protected void SellItem(bool sell)
    {
        SellPriceRoot.SetActive(sell);
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_TrainItem dataComponent = gameObject.GetComponent<GUI_TrainItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_TrainItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Icon = dataComponent.Icon;
            Name = dataComponent.Name;
            AttributeValue = dataComponent.AttributeValue;
            BigSuccessRate = dataComponent.BigSuccessRate;
            SellPrice = dataComponent.SellPrice;
            SellPriceRoot = dataComponent.SellPriceRoot;
        }
    }
}
