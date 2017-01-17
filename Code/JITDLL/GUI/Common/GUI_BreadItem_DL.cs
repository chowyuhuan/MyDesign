using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public sealed class GUI_BreadItem_DL : GUI_TrainItem_DL
{
    public Text StarNum;
    public delegate void OnBreadItemClicked(GUI_BreadItem_DL bread);
    OnBreadItemClicked _OnBreadSelected;
    OnBreadItemClicked _OnBreadDeselected;
    DataCenter.Bread _Bread;
    public CSV_b_bread_template BreadTemplate
    {
        get;
        protected set;
    }

    public uint ServerId
    {
        get { return _Bread.ServerId; }
    }


    public void SetBreadItem(DataCenter.Bread bread, OnBreadItemClicked onSelected, OnBreadItemClicked onDeselected)
    {
        if (null != bread)
        {
            _Bread = bread;
            _OnBreadSelected = onSelected;
            _OnBreadDeselected = onDeselected;
            BreadTemplate = CSV_b_bread_template.FindData(bread.CsvId);
            StarNum.text = BreadTemplate.Star.ToString();
            SetItemData(BreadTemplate.IconAtlas, BreadTemplate.BreadIcon, BreadTemplate.Name, BreadTemplate.TrainVolume, BreadTemplate.SuccessRate, BreadTemplate.SaleCoin);
        }
    }

    protected override void OnSelected()
    {
        if (null != _OnBreadSelected)
        {
            _OnBreadSelected(this);
        }
    }

    protected override void OnDeSelected()
    {
        if (null != _OnBreadDeselected)
        {
            _OnBreadDeselected(this);
        }
    }

    protected override void OnRecycle()
    {
        BreadTemplate = null;
        _Bread = null;
        _OnBreadDeselected = null;
        _OnBreadSelected = null;
        if (IsSelect)
        {
            DeSelect();
        }
    }

    static bool ValidBreadItem(int index)
    {
        return index >= 0 && index < DataCenter.PlayerDataCenter.BreadList.Count;
    }

    public static int CompareByStar(int a, int b)
    {
        int result;
        DataCenter.Bread breadA = ValidBreadItem(a) ? DataCenter.PlayerDataCenter.BreadList[a] : null;
        DataCenter.Bread breadB = ValidBreadItem(b) ? DataCenter.PlayerDataCenter.BreadList[b] : null;
        if (null != breadA && null != breadB)
        {
            CSV_b_bread_template aBreadTemplate = CSV_b_bread_template.FindData(breadA.CsvId);
            CSV_b_bread_template bBreadTemplate = CSV_b_bread_template.FindData(breadB.CsvId);
            result = Mathf.Clamp(aBreadTemplate.Star - bBreadTemplate.Star, -1, 1);
        }
        else if (null == breadA)
        {
            result = -1;
        }
        else
        {
            result = 1;
        }
        return result;
    }

    public static int CompareByTrainValue(int a, int b)
    {
        int result;
        DataCenter.Bread breadA = ValidBreadItem(a) ? DataCenter.PlayerDataCenter.BreadList[a] : null;
        DataCenter.Bread breadB = ValidBreadItem(b) ? DataCenter.PlayerDataCenter.BreadList[b] : null;
        if (null != breadA && null != breadB)
        {
            CSV_b_bread_template aBreadTemplate = CSV_b_bread_template.FindData(breadA.CsvId);
            CSV_b_bread_template bBreadTemplate = CSV_b_bread_template.FindData(breadB.CsvId);
            result = Mathf.Clamp(aBreadTemplate.TrainVolume - bBreadTemplate.TrainVolume, -1, 1);
        }
        else if (null == breadA)
        {
            result = -1;
        }
        else
        {
            result = 1;
        }
        return result;
    }

    public static int CompareBySuccessRate(int a, int b)
    {
        int result;
        DataCenter.Bread breadA = ValidBreadItem(a) ? DataCenter.PlayerDataCenter.BreadList[a] : null;
        DataCenter.Bread breadB = ValidBreadItem(b) ? DataCenter.PlayerDataCenter.BreadList[b] : null;
        if (null != breadA && null != breadB)
        {
            CSV_b_bread_template aBreadTemplate = CSV_b_bread_template.FindData(breadA.CsvId);
            CSV_b_bread_template bBreadTemplate = CSV_b_bread_template.FindData(breadB.CsvId);
            result = Mathf.Clamp(aBreadTemplate.SuccessRate - bBreadTemplate.SuccessRate, -1, 1);
        }
        else if (null == breadA)
        {
            result = -1;
        }
        else
        {
            result = 1;
        }
        return result;
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_BreadItem dataComponent = gameObject.GetComponent<GUI_BreadItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BreadItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            StarNum = dataComponent.StarNum;
        }
    }
}

