using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_SellWeaponAlertUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_SellWeaponAlertUI dataComponent = gameObject.GetComponent<GUI_SellWeaponAlertUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SellWeaponAlertUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            AlertContent = dataComponent.AlertContent;
            dataComponent.ConfirmSellButton.onClick.AddListener(OnConfirmSell);
        }
    }
    #endregion

    #region window logic
    public Text AlertContent;
    List<uint> SelectedEquipItemList;

    public void SellEquip(List<uint> selecteEquipItems)
    {
        SelectedEquipItemList = selecteEquipItems;
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnSellItem += OnSellRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnSellItem -= OnSellRsp;
    }

    protected override void OnStart()
    {
        string contentFormater = "";
        string target = GetAlertTarget();
        string operation;
        TextLocalization.GetText(TextId.Sell, out operation);
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

    void OnConfirmSell()
    {
        gsproto.SellItemReq req = new gsproto.SellItemReq();
        req.item_ids.AddRange(SelectedEquipItemList);
        req.item_type = (uint)PbCommon.ESaleItemType.E_Sale_Equip;
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnSellRsp(uint itemType, uint coin)
    {
        if(itemType == (int)PbCommon.ESaleItemType.E_Sale_Equip)
        {
            HideWindow();
            GUI_GetGoldCoinUI_DL getGold = GUI_Manager.Instance.ShowWindowWithName<GUI_GetGoldCoinUI_DL>("UI_GetGold", false);
            getGold.GetGoldCoin(coin);
        }
    }
    #endregion
}
