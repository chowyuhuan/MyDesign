using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_BakeProductItem_DL : GUI_LogicObject
{
    public Text StarNum;
    public Image Icon;
    public Text Name;
    public Color CountColor;
    public int CountSize;
    public Text AttributeValue;
    public Text BigSuccessRate;

    public void SetItem(int count, int breadCsvId)
    {
        CSV_b_bread_template bread = CSV_b_bread_template.FindData(breadCsvId);
        if (null != bread)
        {
            StarNum.text = bread.Star.ToString();
            GUI_Tools.IconTool.SetIcon(bread.IconAtlas, bread.BreadIcon, Icon);
            string itemIount = GUI_Tools.RichTextTool.Color(CountColor, "x" + count.ToString());
            itemIount = GUI_Tools.RichTextTool.Size(CountSize, itemIount);
            Name.text = bread.Name + itemIount;
            AttributeValue.text = bread.TrainVolume.ToString();
            BigSuccessRate.text = bread.SuccessRate.ToString() + "%";
        }
        else
        {
            StarNum.text = "";
            Name.text = "";
            AttributeValue.text = "";
            BigSuccessRate.text = "";
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
        GUI_BakeProductItem dataComponent = gameObject.GetComponent<GUI_BakeProductItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BakeProductItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            StarNum = dataComponent.StarNum;
            Icon = dataComponent.Icon;
            Name = dataComponent.Name;
            CountSize = dataComponent.CountSize;
            AttributeValue = dataComponent.AttributeValue;
            BigSuccessRate = dataComponent.BigSuccessRate;
        }
    }
}
