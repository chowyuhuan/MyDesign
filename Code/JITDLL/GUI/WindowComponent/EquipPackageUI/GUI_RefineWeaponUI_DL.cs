using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_RefineWeaponUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_RefineWeaponUI dataComponent = gameObject.GetComponent<GUI_RefineWeaponUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_RefineWeaponUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            CurrentRefineEquip = dataComponent.CurrentRefineEquip;
            RefinedEquip = dataComponent.RefinedEquip;
            CurrentEquipStarbar = dataComponent.CurrentEquipStarbar;
            RefinedEquipStarbar = dataComponent.RefinedEquipStarbar;
            RefineMat_Crystal = dataComponent.RefineMat_Crystal;
            RefineMat_Iron = dataComponent.RefineMat_Iron;
            RefineCost_GoldCoin = dataComponent.RefineCost_GoldCoin;
            ConfirmButton = dataComponent.ConfirmButton;
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmRefine);
        }
    }
    #endregion

    #region window logic
    public GUI_EquipSimpleInfo CurrentRefineEquip;
    public GUI_EquipSimpleInfo RefinedEquip;
    public GameObject CurrentEquipStarbar;
    public GameObject RefinedEquipStarbar;
    public Text RefineMat_Crystal;
    public Text RefineMat_Iron;
    public Text RefineCost_GoldCoin;
    public Button ConfirmButton;

    DataCenter.Equip Equip;
    CSV_b_equip_template EquipTemplate;
    CSV_b_equip_template RefinedEquipTemplate;

    public void RefineEquip(DataCenter.Equip equip, CSV_b_equip_template equipTemplate)
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
        SetEquipInfo(EquipTemplate, CurrentRefineEquip);
        GUI_HeroStarBar_DL equipStarbar = CurrentEquipStarbar.GetComponent<GUI_HeroStarBar_DL>();
        if(null != equipStarbar)
        {
            equipStarbar.SetStarNum(EquipTemplate.Star);
        }
        SetEquipStarBar(CurrentEquipStarbar, EquipTemplate);

        RefinedEquipTemplate = CSV_b_equip_template.FindData(EquipTemplate.NextRefineId);
        if (null != RefinedEquipTemplate)
        {
            SetEquipInfo(RefinedEquipTemplate, RefinedEquip);
        }
        SetEquipStarBar(RefinedEquipStarbar, RefinedEquipTemplate);

        RefineMat_Crystal.text = GetRefineMat_Crystal(EquipTemplate).ToString();
        RefineMat_Iron.text = GetRefineMat_Iron(EquipTemplate).ToString();
        RefineCost_GoldCoin.text = GetRefineMat_Gold(EquipTemplate).ToString();

        ConfirmButton.interactable = EnoughMat();
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnWeaponRefine += OnRefineRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnWeaponRefine -= OnRefineRsp;
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

    public static int GetRefineMat_Crystal(CSV_b_equip_template equipTemplate)
    {
        int crystal = 0;
        if(null != equipTemplate)
        {
            switch((PbCommon.EWeaponType)equipTemplate.WeaponType)
            {
                case PbCommon.EWeaponType.E_Weapon_Type_Original:
                    {
                        crystal = DefaultConfig.GetInt("OriginalRefineCostCrystalPowder");
                        break;
                    }
                case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                    {
                        if(equipTemplate.Star == 4)
                        {
                            crystal = DefaultConfig.GetInt("Star4RefineCostCrystalPiece");
                        }
                        else if (equipTemplate.Star == 5)
                        {
                            crystal = DefaultConfig.GetInt("Satr5RefineCostCrystalRime");
                        }
                        break;
                    }
            }
        }
        return crystal;
    }

    public static void SetRefineMatIcon_Crystal(CSV_b_equip_template equipTemplate, Image icon)
    {
        if (null != equipTemplate)
        {
            switch ((PbCommon.EWeaponType)equipTemplate.WeaponType)
            {
                case PbCommon.EWeaponType.E_Weapon_Type_Original:
                    {
                        GUI_Tools.IconTool.SetItemIcon(PbCommon.EAwardType.E_Award_Crystal_Powder, icon);
                        break;
                    }
                case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                    {
                        if (equipTemplate.Star == 4)
                        {
                            GUI_Tools.IconTool.SetItemIcon(PbCommon.EAwardType.E_Award_Crystal_Piece, icon);
                        }
                        else if (equipTemplate.Star == 5)
                        {
                            GUI_Tools.IconTool.SetItemIcon(PbCommon.EAwardType.E_Award_Crystal_Rime, icon);
                        }
                        break;
                    }
            }
        }
    }

    public static int GetRefineMat_Iron(CSV_b_equip_template equipTemplate)
    {
        int iron = 0;
        if (null != equipTemplate)
        {
            switch ((PbCommon.EWeaponType)equipTemplate.WeaponType)
            {
                case PbCommon.EWeaponType.E_Weapon_Type_Original:
                    {
                        iron = DefaultConfig.GetInt("OriginalRefineCostIron");
                        break;
                    }
                case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                    {
                        if (equipTemplate.Star == 4)
                        {
                            iron = DefaultConfig.GetInt("Star4RefineCostIron");
                        }
                        else if (equipTemplate.Star == 5)
                        {
                            iron = DefaultConfig.GetInt("Star5RefineCostIron");
                        }
                        break;
                    }
            }
        }
        return iron;
    }

    public static int GetRefineMat_Gold(CSV_b_equip_template equipTemplate)
    {
        int gold = 0;
        if (null != equipTemplate)
        {
            switch ((PbCommon.EWeaponType)equipTemplate.WeaponType)
            {
                case PbCommon.EWeaponType.E_Weapon_Type_Original:
                    {
                        gold = DefaultConfig.GetInt("OriginalRefineCostCoin");
                        break;
                    }
                case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                    {
                        if (equipTemplate.Star == 4)
                        {
                            gold = DefaultConfig.GetInt("Star4RefineCostCoin");
                        }
                        else if (equipTemplate.Star == 5)
                        {
                            gold = DefaultConfig.GetInt("Star5RefineCostCoin");
                        }
                        break;
                    }
            }
        }
        return gold;
    }

    void SetEquipInfo(CSV_b_equip_template equipTemplate, GUI_EquipSimpleInfo equipInfo)
    {
        if(null != equipTemplate && null != equipInfo)
        {
            equipInfo.ReformText.text = equipTemplate.Name;
            //equipInfo.StarText.text = "";
            GUI_Tools.IconTool.SetIcon(equipTemplate.IconAtlas, equipTemplate.IconSprite, equipInfo.EquipIcon);
            CSV_c_school_config schoolConfig = CSV_c_school_config.FindData(equipTemplate.School);
            if(null != schoolConfig)
            {
                GUI_Tools.IconTool.SetIcon(schoolConfig.Atlas, schoolConfig.Icon, equipInfo.SchoolIcon);
            }
        }
    }

    void OnConfirmRefine()
    {
        if (EnoughMat())
        {
            gsproto.WeaponRefineReq req = new gsproto.WeaponRefineReq();
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            req.weapon_id = Equip.ServerId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("Not enough refine mat");
        }
    }

    void OnRefineRsp(uint oldWeaponServerId, uint newWeaponServerId)
    {
        HideWindow();
        GUI_RefineWeaponSuccessUI_DL refineSuccessUi = GUI_Manager.Instance.ShowWindowWithName<GUI_RefineWeaponSuccessUI_DL>("UI_Weapon_jingcuiSuccess", false);
        if(null != refineSuccessUi)
        {
            refineSuccessUi.RefineEquipSuccess(newWeaponServerId);
        }
    }

    bool EnoughMat()
    {
        bool enough = false;
        int neededCrystal = 0;
        int neededIron = 0;
        int neededGold = 0;
        if (null != RefinedEquipTemplate)
        {
            switch ((PbCommon.EWeaponType)EquipTemplate.WeaponType)
            {
                case PbCommon.EWeaponType.E_Weapon_Type_Original:
                    {
                        neededCrystal = DefaultConfig.GetInt("OriginalRefineCostCrystalPowder");
                        neededIron = DefaultConfig.GetInt("OriginalRefineCostIron");
                        neededGold = DefaultConfig.GetInt("OriginalRefineCostCoin");
                        enough = (DataCenter.PlayerDataCenter.CrystalPowder >= neededCrystal
                            && DataCenter.PlayerDataCenter.Iron >= neededIron
                            && DataCenter.PlayerDataCenter.Coin >= neededGold);
                        break;
                    }
                case PbCommon.EWeaponType.E_Weapon_Type_Exclusive:
                    {
                        if (RefinedEquipTemplate.Star == 4)
                        {
                            neededCrystal = DefaultConfig.GetInt("Star4RefineCostCrystalPiece");
                            neededIron = DefaultConfig.GetInt("Star4RefineCostIron");
                            neededGold = DefaultConfig.GetInt("Star4RefineCostCoin");
                        }
                        else if (RefinedEquipTemplate.Star == 5)
                        {
                            neededCrystal = DefaultConfig.GetInt("Satr5RefineCostCrystalRime");
                            neededIron = DefaultConfig.GetInt("Star5RefineCostIron");
                            neededGold = DefaultConfig.GetInt("Star5RefineCostCoin");
                        }
                        enough = (DataCenter.PlayerDataCenter.CrystalPowder >= neededCrystal
                                && DataCenter.PlayerDataCenter.Iron >= neededIron
                                && DataCenter.PlayerDataCenter.Coin >= neededGold);
                        break;
                    }
            }
        }
        return enough;
    }
    #endregion
}
