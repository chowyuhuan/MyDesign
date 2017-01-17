using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ResolveWeaponAlertUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ResolveWeaponAlertUI dataComponent = gameObject.GetComponent<GUI_ResolveWeaponAlertUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ResolveWeaponAlertUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            AlertContent = dataComponent.AlertContent;
            dataComponent.ConfirmResovleButton.onClick.AddListener(OnConfirmResovle);
        }
    }
    #endregion

    #region window logic
    public Text AlertContent;
    List<uint> SelectedEquipItemList;

    public void ResolveEquip(List<uint> selecteEquipItems)
    {
        SelectedEquipItemList = selecteEquipItems;
    }

    protected override void OnStart()
    {
        string contentFormater = "";
        string target = GetAlertTarget();
        string operation;
        TextLocalization.GetText(TextId.Resolve, out operation);
        if(TextLocalization.GetText("Weapon_Alert_Formatter", out contentFormater))
        {
            AlertContent.text = string.Format(contentFormater, operation, target, operation);
        }
    }

    string GetAlertTarget()
    {
        bool ancient = false;
        bool reform = false;
        string target = null;
        int maxStar = 0;
        for (int index = 0; index < SelectedEquipItemList.Count; ++index)
        {
            DataCenter.Equip equip = DataCenter.PlayerDataCenter.GetEquip(SelectedEquipItemList[index]);
            CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData(equip.CsvId);

            if (equipTemplate.WeaponType == (int)PbCommon.EWeaponType.E_Weapon_Type_Exclusive)
            {
                TextLocalization.GetText("Weapon_Exclusive_Text", out target);
                return target;
            }
            else if (equipTemplate.WeaponType == (int)PbCommon.EWeaponType.E_Weapon_Type_Original)
            {
                ancient = true;
            }
            else if (equip.ReformList.Count > 0)
            {
                for (int reformIndex = 0; reformIndex < equip.ReformList.Count; ++reformIndex)
                {
                    if ((int)equip.ReformList[reformIndex].ReformProperty > 0)
                    {
                        reform = true;
                        break;
                    }
                }
            }
            maxStar = Mathf.Max(maxStar, equipTemplate.Star);
        }

        if (ancient)
        {
            TextLocalization.GetText("Weapon_Ancient_Text", out target);
        }
        else if (reform)
        {
            TextLocalization.GetText("Weapon_Exclusive_Text", out target);
        }
        else
        {
            TextLocalization.GetText("Weapon_Reformed_Text", out target);
            target = string.Format(target, maxStar.ToString());
        }
        return target;
    }

    void OnConfirmResovle()
    {
        HideWindow();
        GUI_SelectResolveTypeUI_DL selectTypeUI = GUI_Manager.Instance.ShowWindowWithName<GUI_SelectResolveTypeUI_DL>("UI_Decompose", false);
        if(null != selectTypeUI)
        {
            selectTypeUI.SelectResolveType(SelectedEquipItemList);
        }
    }
    #endregion
}
