using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ReformInfoUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ReformInfoUI dataComponent = gameObject.GetComponent<GUI_ReformInfoUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ReformInfoUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            VerticalGroupHelperObject = dataComponent.VerticalGroupHelperObject;
            ReformInfoPageObjectList = dataComponent.ReformInfoPageObjectList;
            SwitchButtonText = dataComponent.SwitchButtonText;
            dataComponent.BigSuccessButton.onClick.AddListener(OnBigSuccessButtonClicked);
        }
    }
    #endregion

    #region logic area
    public GameObject VerticalGroupHelperObject;
    public List<GameObject> ReformInfoPageObjectList;
    List<GUI_ToggleTabPage_DL> ReformInfoPageList;
    GUI_LogicObjectPool ReformItemPool;
    GUI_VerticallayouGroupHelper_DL GroupLayoutHelper;
    Text SwitchButtonText;
    int _CurrentPage = -1;
    int Reform_Property_Width = 1000;
    bool DisplayBigSuccessValue = false;

    protected override void OnStart()
    {
        InitReformInfoPage();

        _CurrentPage = -1;
        if(ReformInfoPageList.Count > 0)
        {
            ReformInfoPageList[0].Select();
        }

        RefreshSwitchButton();
    }

    void InitReformInfoPage()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Remould_Detail_Item", true, AssetManage.E_AssetType.UIPrefab);
        ReformItemPool = new GUI_LogicObjectPool(go);

        ReformInfoPageList = new List<GUI_ToggleTabPage_DL>();
        for (int index = 0; index < ReformInfoPageObjectList.Count; ++index)
        {
            ReformInfoPageList.Add(ReformInfoPageObjectList[index].GetComponent<GUI_ToggleTabPage_DL>());
        }

        GroupLayoutHelper = VerticalGroupHelperObject.GetComponent<GUI_VerticallayouGroupHelper_DL>();
        GroupLayoutHelper.SetScrollAction(DisplayScrollItem);
        for (int index = 0; index < ReformInfoPageList.Count; ++index)
        {
            ReformInfoPageList[index].Init(index, GroupLayoutHelper, OnPageSelect, null, false);
        }
    }

    void OnPageSelect(int index)
    {
        if (index == _CurrentPage)
        {
            return;
        }
        _CurrentPage = index;
        int typeId = _CurrentPage + 1;
        for (int reformIndex = 0; reformIndex < CSV_c_equip_reform_config.DateCount; ++reformIndex)
        {
            int reformType = CSV_c_equip_reform_config.AllData[reformIndex].ReformId / Reform_Property_Width;
            if(reformType == typeId)
            {
                GroupLayoutHelper.FillItem(CSV_c_equip_reform_config.AllData[reformIndex].ReformId);
            }
        }
        GroupLayoutHelper.FillItemEnd();
        GroupLayoutHelper.RefreshLayout();
        GroupLayoutHelper.LocateAtIndex(0);
    }

    void DisplayScrollItem(GUI_ScrollItem scrollItem)
    {
        if(null != scrollItem)
        {
            CSV_c_equip_reform_config equipReform = CSV_c_equip_reform_config.FindData(scrollItem.LogicIndex);
            if(null != equipReform)
            {
                GUI_ReformInfoItem_DL reformInfo = ReformItemPool.GetOneLogicComponent() as GUI_ReformInfoItem_DL;
                if(null != reformInfo)
                {
                    reformInfo.ShowReformInfo(equipReform, Reform_Property_Width, ShowBigSuccessValue);
                    scrollItem.SetTarget(reformInfo);
                }
            }
        }
    }

    bool ShowBigSuccessValue()
    {
        return DisplayBigSuccessValue;
    }

    void OnBigSuccessButtonClicked()
    {
        DisplayBigSuccessValue = !DisplayBigSuccessValue;
        RefreshSwitchButton();
        GroupLayoutHelper.RefreshDisplay();
    }

    void RefreshSwitchButton()
    {
        string buttonText;
        if (TextLocalization.GetText(DisplayBigSuccessValue ? TextId.BigSuccess : TextId.Success, out buttonText))
        {
            SwitchButtonText.text = buttonText;
        }
    }
    #endregion
}
