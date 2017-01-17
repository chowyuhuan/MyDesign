using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_RefineWeaponSuccessUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_RefineWeaponSuccessUI dataComponent = gameObject.GetComponent<GUI_RefineWeaponSuccessUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_RefineWeaponSuccessUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            RefinedEquip = dataComponent.RefinedEquip;
            RefinedEquipStarbar = dataComponent.RefinedEquipStarbar;
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmRefine);
        }
    }
    #endregion

    #region window logic
    public GUI_EquipSimpleInfo RefinedEquip;
    public GameObject RefinedEquipStarbar;
    public Button ConfirmButton;

    DataCenter.Equip Equip;
    CSV_b_equip_template EquipTemplate;

    public void RefineEquipSuccess(uint weaponServerId)
    {
        Equip = DataCenter.PlayerDataCenter.GetEquip(weaponServerId);
        if (null == Equip)
        {
            HideWindow();
        }
        else
        {
            EquipTemplate = CSV_b_equip_template.FindData(Equip.CsvId);
        }
    }

    protected override void OnStart()
    {
        if (null != EquipTemplate)
        {
            SetEquipInfo(EquipTemplate, RefinedEquip);
        }
        SetEquipStarBar(RefinedEquipStarbar, EquipTemplate);
    }

    void SetEquipStarBar(GameObject starBarObject, CSV_b_equip_template equipTemplate)
    {
        if(null != starBarObject && null != equipTemplate)
        {
            GUI_HeroStarBar_DL equipStarBar = starBarObject.GetComponent<GUI_HeroStarBar_DL>();
            if (null != equipStarBar)
            {
                equipStarBar.SetStarNum(equipTemplate.Star);
            }
        }
    }

    void SetEquipInfo(CSV_b_equip_template equipTemplate, GUI_EquipSimpleInfo equipInfo)
    {
        if(null != equipTemplate && null != equipInfo)
        {
            equipInfo.ReformText.text = equipTemplate.Name;
            GUI_Tools.IconTool.SetIcon(equipTemplate.IconAtlas, equipTemplate.IconSprite, equipInfo.EquipIcon);
        }
    }

    void OnConfirmRefine()
    {
        HideWindow();
    }
    #endregion
}
