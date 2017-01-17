using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ResetWeaponPropertyUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ResetWeaponPropertyUI dataComponent = gameObject.GetComponent<GUI_ResetWeaponPropertyUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ResetWeaponPropertyUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            FirstProperty = dataComponent.FirstProperty;
            SecondProerty = dataComponent.SecondProerty;
            ItemCost = dataComponent.ItemCost;
            dataComponent.ConfirmButton.onClick.AddListener(ConfirmResetProperty);
        }
    }
    #endregion

    #region window logic
    public GUI_EquipBasePropertyInfo FirstProperty;
    public GUI_EquipBasePropertyInfo SecondProerty;
    public Text ItemCost;

    DataCenter.Equip Equip;
    CSV_b_equip_template EquipTemplate;

    public void ResetEquipProperty(DataCenter.Equip equip, CSV_b_equip_template equipTemplate)
    {
        Equip = equip;
        EquipTemplate = equipTemplate;
        if(null == equip || null == equipTemplate)
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        SetReformProperty(FirstProperty, Equip.ReformList.Count > 0 ? Equip.ReformList[0] : null);
        SetReformProperty(SecondProerty, Equip.ReformList.Count > 1 ? Equip.ReformList[1] : null);
        ItemCost.text = DefaultConfig.GetInt("SpecialWeaponResetCost").ToString();
    }

    void SetReformProperty(GUI_EquipBasePropertyInfo reformProp, DataCenter.EquipReform equipReform)
    {
        if(null != equipReform)
        {
            reformProp.Name.text = GUI_Tools.TextTool.GetWordPropertyText((PbCommon.EReformType)equipReform.ReformType);
            string reformFormater = GUI_Tools.TextTool.GetReformTextFormater((PbCommon.EPropertyType)equipReform.ReformProperty);
            reformProp.Description.text = string.Format(reformFormater, equipReform.ReformValue);
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnSpecialWeaponReformReset += OnResetPropertyRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnSpecialWeaponReformReset -= OnResetPropertyRsp;
    }

    void ConfirmResetProperty()
    {
        if(EnoughResetMat())
        {
            gsproto.SpecialWeaponReformResetReq req = new gsproto.SpecialWeaponReformResetReq();
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            req.weapon_id = Equip.ServerId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    bool EnoughResetMat()
    {
        return true;
    }

    void OnResetPropertyRsp(uint weaponServerId)
    {
        GUI_MessageManager.Instance.ShowErrorTip("重置武器属性成功");
        Equip = DataCenter.PlayerDataCenter.GetEquip(weaponServerId);
        OnStart();
    }
    #endregion
}
