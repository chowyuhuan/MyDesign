using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_SellAllWeaponUI_DL : GUI_Window_DL
{
    public Text SelectWeaponCount;
    public Text SellPrice;
    Toggle ReformEquipFilter;
    Toggle RefineAndAncientEquipFilter;
    Button ConfirmButton;
    List<Toggle> StarFilterList;
    List<uint> SelectedEquipItems = new List<uint>();
    Dictionary<int, int> StarWeaponDic = new Dictionary<int, int>();
    int WeaponMaxStar;

    public void OnRemakeMarkClicked(bool select)
    {
        RefreshStarFilter();
        RefreshSellInfo();
    }

    public void OnQuenchingAndAncentMarkClicked(bool select)
    {
        RefreshStarFilter();
        RefreshSellInfo();
    }

    public void OnConfirmButtonClicked()
    {
        if (SelectedEquipItems.Count > 0)
        {
            GUI_SellWeaponAlertUI_DL alertUI = GUI_Manager.Instance.ShowWindowWithName<GUI_SellWeaponAlertUI_DL>("UI_SellAffirm", false);
            if (null != alertUI)
            {
                alertUI.SellEquip(SelectedEquipItems);
            }
            HideWindow();
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("Error_No_Select_Sell_Weapon");
        }
    }

    void OnStarFilterToggleClicked(bool selected)
    {
        RefreshSellInfo();
    }

    void RefreshSellInfo()
    {
        SelectedEquipItems.Clear();
        int sellCount = 0;
        int sellPrice = 0;
        List<DataCenter.Equip> weaponList = DataCenter.PlayerDataCenter.WeaponList;
        for (int index = 0; index < weaponList.Count; ++index)
        {
            DataCenter.Equip equip = weaponList[index];
            CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData((int)equip.CsvId);
            if(CanSell(equip, equipTemplate))
            {
                int starToggleIndex = equipTemplate.Star - 1;
                if (starToggleIndex >= 0 && starToggleIndex < StarFilterList.Count)
                {
                    if (StarFilterList[starToggleIndex].isOn)
                    {
                        ++sellCount;
                        sellPrice += equipTemplate.SaleCoin;
                        SelectedEquipItems.Add(equip.ServerId);
                    }
                }
            }
        }
        SelectWeaponCount.text = sellCount.ToString();
        SellPrice.text = sellPrice.ToString();
        ConfirmButton.interactable = sellCount > 0;
    }

    bool CanSell(DataCenter.Equip equip, CSV_b_equip_template equipTemplate)
    {
        if (null != equip && null != equipTemplate)
        {
            if (equip.IsLock)//locked
            {
                return false;
            }
            DataCenter.Hero bindhero = DataCenter.PlayerDataCenter.GetHero(equip.HeroServerId);
            if (null != bindhero)//equiped
            {
                return false;
            }
            if (ReformEquipFilter.isOn && IsReformWeapon(equip))//reformed
            {
                return false;
            }
            if (RefineAndAncientEquipFilter.isOn
                && (equip.EquipType == (int)PbCommon.EWeaponType.E_Weapon_Type_Exclusive || equip.EquipType == (int)PbCommon.EWeaponType.E_Weapon_Type_Original))
            {
                return false;
            }

            return true;
        }
        return false;
    }

    void RefreshStarFilter()
    {
        for (int index = 1; index <= WeaponMaxStar; ++index)
        {
            StarWeaponDic[index] = 0;
        }
        List<DataCenter.Equip> weaponList = DataCenter.PlayerDataCenter.WeaponList;
        for (int index = 0; index < weaponList.Count; ++index)
        {
            DataCenter.Equip equip = weaponList[index];
            CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData((int)equip.CsvId);
            if(CanSell(equip, equipTemplate))
            {
                ++StarWeaponDic[equipTemplate.Star];
            }
        }

        for(int index = 1; index <= WeaponMaxStar; ++index)
        {
            StarFilterList[index - 1].interactable = StarWeaponDic[index] > 0;
        }
    }

    bool IsReformWeapon(DataCenter.Equip equip)
    {
        if(null != equip && null != equip.ReformList)
        {
            for(int index = 0; index < equip.ReformList.Count; ++index)
            {
                if((uint)equip.ReformList[index].ReformProperty > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected override void OnStart()
    {
        WeaponMaxStar = DefaultConfig.GetInt("Weapon_Max_Star");
        StarWeaponDic = new Dictionary<int, int>();
        for (int index = 1; index <= WeaponMaxStar; ++index)
        {
            StarWeaponDic.Add(index, 0);
        }
        RefreshStarFilter();
        RefreshSellInfo();
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_SellAllWeaponUI dataComponent = gameObject.GetComponent<GUI_SellAllWeaponUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SellAllWeaponUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SelectWeaponCount = dataComponent.SelectWeaponCount;
            SellPrice = dataComponent.SellPrice;
            ReformEquipFilter = dataComponent.RemakeWeaponFilter;
            RefineAndAncientEquipFilter = dataComponent.QuenchingAndAncentWeaponFilter;
            ConfirmButton = dataComponent.ConfirmButton;
            dataComponent.RemakeWeaponFilter.onValueChanged.AddListener(OnRemakeMarkClicked);
            dataComponent.QuenchingAndAncentWeaponFilter.onValueChanged.AddListener(OnQuenchingAndAncentMarkClicked);
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);

            StarFilterList = dataComponent.StarFilterList;
            for(int index = 0; index < StarFilterList.Count; ++index)
            {
                StarFilterList[index].onValueChanged.AddListener(OnStarFilterToggleClicked);
            }
        }
    }
}
