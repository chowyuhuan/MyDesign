using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_ReformWeaponUI_DL : GUI_Window_DL
{
    GUI_EquipSimpleInfo EquipInfo;
    GUI_EquipBasePropertyInfo AttackValue;
    GUI_EquipBasePropertyInfo AttackSpeed;
    Color NonBigSuccessColor;
    Color BigSuccessColor;
    List<GUI_EquipWordReformInfo> EquipWordInfoList;
    Button ResetReformPropertyButton;
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ReformWeaponUI dataComponent = gameObject.GetComponent<GUI_ReformWeaponUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ReformWeaponUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            EquipInfo = dataComponent.EquipInfo;
            AttackValue = dataComponent.AttackValue;
            AttackSpeed = dataComponent.AttackSpeed;
            EquipName = dataComponent.EquipName;
            NonBigSuccessColor = dataComponent.NonBigSuccessColor;
            BigSuccessColor = dataComponent.BigSuccessColor;
            EquipWordInfoList = dataComponent.EquipWordInfoList;
            ResetReformPropertyButton = dataComponent.ResetReformPropertyButton;
            dataComponent.ReformInfoButton.onClick.AddListener(OnReformInfoButtonClicked);
            dataComponent.ResetReformPropertyButton.onClick.AddListener(OnResetReformPropertyButtonClicked);
        }
    }
    #endregion

    #region reform logic
    DataCenter.Equip Equip;
    CSV_b_equip_template EquipTemplate;
    Dictionary<int, uint> EquipReformDic = new Dictionary<int, uint>();
    Text EquipName;
    int ReformCostIncreaseMaxCount = 0;
    
    public void ReformWeapon(DataCenter.Equip equip)
    {
        Equip = equip;
        EquipTemplate = CSV_b_equip_template.FindData(equip.CsvId);
        ReformCostIncreaseMaxCount = DefaultConfig.GetInt("ReformCostFixedTims");
        ResetReformPropertyButton.gameObject.SetActive(EquipTemplate.CanRefine != 0);
    }

    protected override void OnStart()
    {
        base.OnStart();
        RegistWordReformHandler();
        SetItemInfo();
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnWeaponReform += OnReformWordPropertyRsp;
        DataCenter.PlayerDataCenter.OnReformCostReset += OnResetWordReformCountRsp;
        DataCenter.PlayerDataCenter.OnSpecialWeaponReformReset += OnResetReformPropertyRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnWeaponReform -= OnReformWordPropertyRsp;
        DataCenter.PlayerDataCenter.OnReformCostReset -= OnResetWordReformCountRsp;
        DataCenter.PlayerDataCenter.OnSpecialWeaponReformReset -= OnResetReformPropertyRsp;
    }

    void SetItemInfo()
    {
        if (null != Equip)
        {
            AttackValue.Description.text = Equip.Attack.ToString();
            AttackSpeed.Description.text = Equip.AttackSpeed.ToString();
            if (null != EquipTemplate)
            {
                EquipName.text = EquipTemplate.Name;
                GUI_Tools.IconTool.SetIcon(EquipTemplate.IconAtlas, EquipTemplate.IconSprite, EquipInfo.EquipIcon);
                EquipInfo.StarText.text = EquipTemplate.Star.ToString();
            }

            CSV_c_school_config equipSchool = CSV_c_school_config.FindData(EquipTemplate.School);
            if (null != equipSchool)
            {
                int maxRefineStar = DefaultConfig.GetInt("MaxRefineStar");
                GUI_Tools.IconTool.SetIcon(equipSchool.Atlas, EquipTemplate.Star == maxRefineStar ? equipSchool.ExclusiveWeaponMaxIcon : equipSchool.Icon, EquipInfo.SchoolIcon);
            }

            RefreshReformInfo();
        }
    }

    void RefreshReformInfo()
    {
        EquipReformDic.Clear();
        if(null != Equip)
        {
            int bigSuccessCount = 0;
            for (int index = 0; index < EquipWordInfoList.Count; ++index)
            {
                if (index < Equip.ReformList.Count)
                {
                    if (Equip.ReformList[index].IsBigSuccess)
                    {
                        ++bigSuccessCount;
                    }
                    SetEquipWordItem(Equip.ReformList[index], EquipWordInfoList[index]);
                    EquipReformDic.Add(index, Equip.ReformList[index].ReformIndex);
                }
                else
                {
                    SetEquipWordItem(null, EquipWordInfoList[index]);
                }
            }
            if (bigSuccessCount > 0 && bigSuccessCount == Equip.ReformList.Count)
            {
                TextLocalization.SetTextById(EquipInfo.ReformText, TextId.BigSuccess);
            }
            else
            {
                GUI_Tools.TextTool.SetText(EquipInfo.ReformText, bigSuccessCount > 0 ? "+" + bigSuccessCount : null);
            }
        }
    }

    void SetEquipWordItem(DataCenter.EquipReform equipReform, GUI_EquipWordReformInfo wordProperty)
    {
        if (null != equipReform)
        {
            if (null != wordProperty)
            {
                if (!wordProperty.DescriptionRootObject.activeInHierarchy)
                {
                    wordProperty.DescriptionRootObject.SetActive(true);
                }
                wordProperty.WordDescription.text = GUI_Tools.TextTool.GetWordPropertyText((PbCommon.EReformType)equipReform.ReformType);

                string reformFormater = GUI_Tools.TextTool.GetReformTextFormater((PbCommon.EPropertyType)equipReform.ReformProperty);
                wordProperty.ReformText.text = string.Format(reformFormater, equipReform.ReformValue);

                string bigSuccessText;
                if (equipReform.IsBigSuccess)
                {
                    TextLocalization.GetText("Reform_Big_Success_Text", out bigSuccessText);
                    wordProperty.WordLevelText.text = GUI_Tools.RichTextTool.Color(BigSuccessColor, bigSuccessText);
                }
                else
                {
                    TextLocalization.GetText("Reform_Non_Big_Sucess_Text", out bigSuccessText);
                    wordProperty.WordLevelText.text = GUI_Tools.RichTextTool.Color(NonBigSuccessColor, bigSuccessText);
                }   

                int reformCount = Mathf.Clamp((int)equipReform.ReformCount, 1, ReformCostIncreaseMaxCount);
                CSV_b_reform_cost reformCost = CSV_b_reform_cost.FindData(string.Format("{0}_{1}", EquipTemplate.Star.ToString(), reformCount));
                if(null != reformCost)
                {
                    wordProperty.ReformCost.text = reformCost.Cost.ToString();
                }
                wordProperty.ResetCountButton.gameObject.SetActive(reformCount == ReformCostIncreaseMaxCount);
            }
        }
        else if (null != wordProperty)
        {
            if (wordProperty.DescriptionRootObject.activeInHierarchy)
            {
                wordProperty.DescriptionRootObject.SetActive(false);
            }
        }
    }

    void RegistWordReformHandler()
    {
        if(Equip.ReformList.Count > 0)
        {
            EquipWordInfoList[0].ReformButton.onClick.AddListener(ReformFirstWordProperty);
            EquipWordInfoList[0].ResetCountButton.onClick.AddListener(ResetFirstWordReformCount);
        }
        if (Equip.ReformList.Count > 1)
        {
            EquipWordInfoList[1].ReformButton.onClick.AddListener(ReformSecondWordProperty);
            EquipWordInfoList[1].ResetCountButton.onClick.AddListener(ResetSecondWordReformCount);
        }
        if (Equip.ReformList.Count > 2)
        {
            EquipWordInfoList[2].ReformButton.onClick.AddListener(ReformThirdWordProperty);
            EquipWordInfoList[2].ResetCountButton.onClick.AddListener(ResetThirdWordReformCount);
        }
    }

    void ReformFirstWordProperty()
    {
        GUI_MessageManager.Instance.ShowMessage("确定改造", "确定要进行改造吗", "确定", "取消", ConfirmReformFirstWordProperty, null, MessageBoxType.ConfirmAndConcell);
    }

    void ConfirmReformFirstWordProperty()
    {
        ReformWordProperty(0);
    }

    void ResetFirstWordReformCount()
    {
        ResetWordReformCount(0);
    }

    void ConfirmResetFirstWordReformCount()
    {
        ResetWordReformCount(0);
    }

    void ReformSecondWordProperty()
    {
        GUI_MessageManager.Instance.ShowMessage("确定改造", "确定要进行改造吗", "确定", "取消", ConfirmReformSecondWordProperty, null, MessageBoxType.ConfirmAndConcell);
    }

    void ConfirmReformSecondWordProperty()
    {
        ReformWordProperty(1);
    }

    void ResetSecondWordReformCount()
    {
        ResetWordReformCount(1);
    }

    void ReformThirdWordProperty()
    {
        GUI_MessageManager.Instance.ShowMessage("确定改造", "确定要进行改造吗", "确定", "取消", ConfirmReformThirdWordProperty, null, MessageBoxType.ConfirmAndConcell);
    }

    void ConfirmReformThirdWordProperty()
    {
        ReformWordProperty(2);
    }

    void ResetThirdWordReformCount()
    {
        ResetWordReformCount(2);
    }

    void ReformWordProperty(int reformOrder)
    {
        uint reformIndex;
        if (EquipReformDic.TryGetValue(reformOrder, out reformIndex))
        {
            DataCenter.EquipReform equipReform = Equip.GetEquipReform(reformIndex);
            if (null != equipReform)
            {
                gsproto.WeaponReformReq req = new gsproto.WeaponReformReq();
                req.weapon_id = Equip.ServerId;
                req.reform_index = equipReform.ReformIndex;
                req.session_id = DataCenter.PlayerDataCenter.SessionId;
                Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
            }
        }
    }

    void OnReformWordPropertyRsp(uint equipServerId, uint reformIndex)
    {
        if(equipServerId == Equip.ServerId)
        {
            GUI_ReformSuccess_DL reformSuccess = GUI_Manager.Instance.ShowWindowWithName<GUI_ReformSuccess_DL>("UI_ReformSuccess", false);
            if(null != reformSuccess)
            {
                reformSuccess.ShowReformProperty(Equip, reformIndex);
            }
            RefreshReformInfo();
        }
    }

    void ResetWordReformCount(int reformOrder)
    {
        uint reformIndex;
        if (EquipReformDic.TryGetValue(reformOrder, out reformIndex))
        {
            GUI_ResetReformCostUI_DL resetReformCost = GUI_Manager.Instance.ShowWindowWithName<GUI_ResetReformCostUI_DL>("UI_Remould_RECost", false);
            if(null != resetReformCost)
            {
                resetReformCost.ResetEquipReformCost(Equip, reformIndex, EquipTemplate);
            }
        }
    }

    void OnResetWordReformCountRsp(uint equipServerId)
    {
        if (equipServerId == Equip.ServerId)
        {
            RefreshReformInfo();
        }
    }

    void OnResetReformPropertyButtonClicked()
    {
        GUI_ResetWeaponPropertyUI_DL resetWeaponProperty = GUI_Manager.Instance.ShowWindowWithName<GUI_ResetWeaponPropertyUI_DL>("UI_Attribute_RE", false);
        if(null != resetWeaponProperty)
        {
            resetWeaponProperty.ResetEquipProperty(Equip, EquipTemplate);
        }
    }

    void OnResetReformPropertyRsp(uint weaponServerId)
    {
        if(weaponServerId == Equip.ServerId)
        {
            SetItemInfo();
        }
    }

    void OnReformInfoButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_Remould_Detail", false);
    }
    #endregion
}
