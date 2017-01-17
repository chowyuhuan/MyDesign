using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ReformInfoItem_DL : GUI_LogicObject
{
    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_ReformInfoItem dataComponent = gameObject.GetComponent<GUI_ReformInfoItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ReformInfoItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            RequierdHeroGroupLevel = dataComponent.RequierdHeroGroupLevel;
            WeaponStarRange = dataComponent.WeaponStarRange;
            ReformInfoText = dataComponent.ReformInfoText;
            LockMask = dataComponent.LockMask;
            dataComponent.UnLockButton.onClick.AddListener(OnUnLockButtonClicked);
        }
    }
    #endregion

    #region logic area
    public Text RequierdHeroGroupLevel;
    public Text WeaponStarRange;
    public Text ReformInfoText;
    public GameObject LockMask;
    public Button UnLockButton;
    CSV_c_equip_reform_config EquipReformInfo;
    int Reform_Property_Width;

    public delegate bool ShowBigSuccessInfo();
    ShowBigSuccessInfo DisplayBigSuccess;

    public void ShowReformInfo(CSV_c_equip_reform_config equipReformInfo, int reformPropertyWidth, ShowBigSuccessInfo displayBigSuccess)
    {
        DisplayBigSuccess = displayBigSuccess;
        Reform_Property_Width = reformPropertyWidth;
        EquipReformInfo = equipReformInfo;
        if(null != EquipReformInfo)
        {
            RequierdHeroGroupLevel.text = EquipReformInfo.HeroGroupLevel.ToString();
            WeaponStarRange.text = EquipReformInfo.StarRange;
        }
        RefreshReformInfoText();
        RefreshLockMask();
    }

    public void RefreshReformInfoText()
    {
        if(null != EquipReformInfo)
        {
            string propertyFormater = GUI_Tools.TextTool.GetReformTextFormater((PbCommon.EPropertyType)EquipReformInfo.PropertyType);
            if (null != DisplayBigSuccess)
            {
                ReformInfoText.text = string.Format(propertyFormater, DisplayBigSuccess() ? EquipReformInfo.BigSuccessValue.ToString() : EquipReformInfo.ReformValueRange);
            }
            else
            {
                ReformInfoText.text = string.Format(propertyFormater, EquipReformInfo.ReformValueRange);
            }
        }
    }

    public override void RefreshObject()
    {
        RefreshReformInfoText();
        RefreshLockMask();
    }

    void RefreshLockMask()
    {
        LockMask.SetActive(EquipReformInfo.HeroGroupLevel > (int)DataCenter.PlayerDataCenter.Level);
    }

    void OnUnLockButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    protected override void OnRecycle()
    {
        EquipReformInfo = null;
        Reform_Property_Width = 0;
        DisplayBigSuccess = null;
    }
    #endregion
}
