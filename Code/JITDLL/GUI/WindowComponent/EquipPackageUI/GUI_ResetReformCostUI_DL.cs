using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ResetReformCostUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ResetReformCostUI dataComponent = gameObject.GetComponent<GUI_ResetReformCostUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ResetReformCostUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            EquipSimpleInfo = dataComponent.EquipSimpleInfo;
            EquipName = dataComponent.EquipName;
            ReformCost_Current = dataComponent.ReformCost_Current;
            ReformCost_Reset = dataComponent.ReformCost_Reset;
            ItemCost = dataComponent.ItemCost;
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
    }
    #endregion
    #region window logic
    public GUI_EquipSimpleInfo EquipSimpleInfo;
    Text EquipName;
    public Text ReformCost_Current;
    public Text ReformCost_Reset;
    public Text ItemCost;

    DataCenter.Equip Equip = null;
    uint ReformIndex = 0;
    CSV_b_equip_template EquipTemplate;
    

    public void ResetEquipReformCost(DataCenter.Equip equip, uint reformIndex, CSV_b_equip_template equipTemplate)
    {
        Equip = equip;
        ReformIndex = reformIndex;
        EquipTemplate = equipTemplate;
        if(null == Equip || null == EquipTemplate)
        {
            HideWindow();
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnReformCostReset += OnResetWordReformCountRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnReformCostReset -= OnResetWordReformCountRsp;
    }

    protected override void OnStart()
    {
        base.OnStart();
        CSV_c_school_config schoolConfig = CSV_c_school_config.FindData(EquipTemplate.School);
        GUI_Tools.IconTool.SetIcon(EquipTemplate.IconAtlas, EquipTemplate.IconSprite, EquipSimpleInfo.EquipIcon);
        GUI_Tools.IconTool.SetIcon(schoolConfig.Atlas, schoolConfig.Icon, EquipSimpleInfo.SchoolIcon);
        EquipSimpleInfo.StarText.text = EquipTemplate.Star.ToString();
        EquipName.text = EquipTemplate.Name;
        

        DataCenter.EquipReform equipReform = Equip.GetEquipReform(ReformIndex);
        int ReformCostIncreaseMaxCount = DefaultConfig.GetInt("ReformCostFixedTims");

        int reformCount = Mathf.Clamp((int)equipReform.ReformCount, 1, ReformCostIncreaseMaxCount);
        CSV_b_reform_cost currentReformCost = CSV_b_reform_cost.FindData(string.Format("{0}_{1}", EquipTemplate.Star.ToString(), reformCount));
        if (null != currentReformCost)
        {
            ReformCost_Current.text = currentReformCost.Cost.ToString();
        }

        CSV_b_reform_cost resetReformCost = CSV_b_reform_cost.FindData(string.Format("{0}_{1}", EquipTemplate.Star.ToString(), 1));
        if (null != resetReformCost)
        {
            ReformCost_Reset.text = resetReformCost.Cost.ToString();
        }

        ItemCost.text = DefaultConfig.GetInt("ReformResetCost").ToString();
    }

    void OnConfirmButtonClicked()
    {
        DataCenter.EquipReform equipReform = Equip.GetEquipReform(ReformIndex);
        if (null != equipReform)
        {
            gsproto.ReformCostResetReq req = new gsproto.ReformCostResetReq();
            req.weapon_id = Equip.ServerId;
            req.reform_index = equipReform.ReformIndex;
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    void OnResetWordReformCountRsp(uint equipServerId)
    {
        GUI_MessageManager.Instance.ShowErrorTip("重置改造费成功");
        HideWindow();
    }
    #endregion
}
