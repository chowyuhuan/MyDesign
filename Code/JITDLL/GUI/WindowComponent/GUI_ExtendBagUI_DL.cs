using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ExtendBagUI_DL : GUI_Window_DL
{
    public Text CostCount;
    PbCommon.EExendBagType _BagType;
    public void ExtendBag(PbCommon.EExendBagType bagType)
    {
        _BagType = bagType;
        switch(bagType)
        {
            case PbCommon.EExendBagType.E_Extend_Hero_Bag:
                {
                    CostCount.text = DefaultConfig.GetInt("HeroBagExtendCost").ToString();
                    break;
                }
            case PbCommon.EExendBagType.E_Extend_Bread_Bag:
                {
                    CostCount.text = DefaultConfig.GetInt("BreadBagExtendCost").ToString();
                    break;
                }
            case PbCommon.EExendBagType.E_Extend_Fruit_Bag:
                {
                    CostCount.text = DefaultConfig.GetInt("FruitBagExtendCost").ToString();
                    break;
                }
            case PbCommon.EExendBagType.E_Extend_Ring_Bag:
                {
                    CostCount.text = DefaultConfig.GetInt("RingBagExtendCost").ToString();
                    break;
                }
            case PbCommon.EExendBagType.E_Extend_Weapon_Bag:
                {
                    CostCount.text = DefaultConfig.GetInt("WeaponBagExtendCost").ToString();
                    break;
                }
        }
    }

    public void OnConfirmExtendButtonClicked()
    {
        gsproto.ExtendBagReq req = new gsproto.ExtendBagReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        req.bag_type = (uint)_BagType;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnExtandBag += OnExtendHeroBagRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnExtandBag -= OnExtendHeroBagRsp;
    }

    void OnExtendHeroBagRsp(PbCommon.EExendBagType bagType)
    {
        if (bagType == _BagType)
        {
            HideWindow();
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ExtendBagUI dataComponent = gameObject.GetComponent<GUI_ExtendBagUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExtendBagUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            CostCount = dataComponent.CostCount;
            dataComponent.ConfirmExtendButton.onClick.AddListener(OnConfirmExtendButtonClicked);
            dataComponent.CancelExtendButton.onClick.AddListener(HideWindow);
        }
    }
}