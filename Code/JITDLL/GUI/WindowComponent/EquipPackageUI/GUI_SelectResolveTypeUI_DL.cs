using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_SelectResolveTypeUI_DL : GUI_Window_DL {

    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_SelectResolveTypeUI dataComponent = gameObject.GetComponent<GUI_SelectResolveTypeUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SelectResolveTypeUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            MidLevelResolveCost = dataComponent.MidLevelResolveCost;
            NormalResolve = dataComponent.NormalResolve;
            dataComponent.ConfirmResolveButton.onClick.AddListener(OnCnofirmResolve);
        }
    }
    #endregion

    #region window logic
    public Text MidLevelResolveCost;
    public Toggle NormalResolve;
    public Button ConfirmResolveButton;

    List<uint> SelectItemList;

    public void SelectResolveType(List<uint> selectItems)
    {
        SelectItemList = selectItems;
        if(null == selectItems)
        {
            HideWindow();
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnWeaponBreak += OnResolveRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnWeaponBreak -= OnResolveRsp;
    }

    protected override void OnStart()
    {
        if(!NormalResolve.isOn)
        {
            NormalResolve.Select();
        }
        if(null != SelectItemList)
        {
            int middleResolveCost = 0;
            for(int index = 0; index < SelectItemList.Count; ++index)
            {
                DataCenter.Equip equip = DataCenter.PlayerDataCenter.GetEquip(SelectItemList[index]);
                CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData(equip.CsvId);
                if (null != equipTemplate)
                {
                    middleResolveCost += equipTemplate.BreakCoin;
                }
            }
            MidLevelResolveCost.text = middleResolveCost.ToString();
        }
        else
        {
            MidLevelResolveCost.text = "0";
        }
    }

    void OnCnofirmResolve()
    {
        gsproto.WeaponBreakReq req = new gsproto.WeaponBreakReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        req.weapon_ids.AddRange(SelectItemList);
        req.break_type = NormalResolve.isOn ? (uint)PbCommon.EWeaponBreakType.E_Break_Normal : (uint)PbCommon.EWeaponBreakType.E_Break_Mid;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnResolveRsp(List<DataCenter.AwardInfo> awardList, List<DataCenter.AwardInfo> extraAwardList, bool isBigSuccess)
    {
        GUI_ResolveSuccessUI_DL resolveAwardUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ResolveSuccessUI_DL>("UI_DecomposeSuccess", false);
        if(null != resolveAwardUI)
        {
            resolveAwardUI.ResovleSuccess(awardList, extraAwardList, isBigSuccess);
        }
        HideWindow();
    }
    #endregion
}
