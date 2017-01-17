using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GUI_HeroBindWeaponItem_DL : GUI_ToggleItem_DL
{
    #region equipment
    public GameObject EquipObjectRoot;
    public GUI_EquipSimpleInfo EquipSimpleInfo;
    public Image EquipBgIcon;

    /// <summary>
    /// 武器
    /// </summary>
    public void SetWeapon()
    {
        if (null != BindHero)
        {
            CSV_b_equip_template equipTemplate = CSV_b_equip_template.FindData(BindHero.Weapon.CsvId);
            if (null != equipTemplate)
            {
                GUI_Tools.IconTool.SetIcon(equipTemplate.IconAtlas, equipTemplate.IconSprite, EquipSimpleInfo.EquipIcon);
                EquipSimpleInfo.StarText.text = equipTemplate.Star.ToString();

                CSV_c_school_config equipSchool = CSV_c_school_config.FindData(equipTemplate.School);
                if (null != equipSchool)
                {
                    if (equipTemplate.WeaponType == (int)PbCommon.EWeaponType.E_Weapon_Type_Exclusive)
                    {
                        int maxRefineStar = DefaultConfig.GetInt("MaxRefineStar");
                        GUI_Tools.IconTool.SetIcon(equipSchool.Atlas, equipTemplate.Star == maxRefineStar ? equipSchool.ExclusiveWeaponMaxIcon : equipSchool.Icon, EquipSimpleInfo.SchoolIcon);
                    }
                    else
                    {
                        GUI_Tools.IconTool.SetIcon(equipSchool.Atlas, equipSchool.Icon, EquipSimpleInfo.SchoolIcon);
                    }
                }
            }

            RefreshReformText();
        }
    }

    void RefreshReformText()
    {
        DataCenter.Equip equip = BindHero.Weapon;
        int bigSuccessReformNum = 0;
        for (int index = 0; index < equip.ReformList.Count; ++index)
        {
            if (equip.ReformList[index].IsBigSuccess)
            {
                ++bigSuccessReformNum;
            }
        }

        if (bigSuccessReformNum > 0 && bigSuccessReformNum == equip.ReformList.Count)
        {
            string reformText;
            TextLocalization.GetText(TextId.BigSuccess, out reformText);
            EquipSimpleInfo.ReformText.text = reformText;
        }
        else if (bigSuccessReformNum > 0)
        {
            EquipSimpleInfo.ReformText.text = bigSuccessReformNum.ToString();
        }
        else
        {
            EquipSimpleInfo.ReformText.text = "";
        }
    }

    /// <summary>
    /// 戒指
    /// </summary>
    public void SetRing()
    {

    }


    void OnReformWordPropertyRsp(uint equipServerId, uint reformIndex)
    {
        if(null != BindHero && null != BindHero.Weapon && BindHero.Weapon.ServerId == equipServerId)
        {
            RefreshReformText();
        }
    }
    #endregion

    #region hero
    public GUI_ActorSimpleInfo HeroSimpleInfo;
    public Image HeadIconBg;
    public DataCenter.Hero BindHero;

    Action<GUI_HeroBindWeaponItem_DL> OnHeroSelect;
    Action<GUI_HeroBindWeaponItem_DL> OnHeroDeselect;

    public delegate bool DisplayWeapon();
    DisplayWeapon BindWeapon;

    public void SetHero(DataCenter.Hero hero, Action<GUI_HeroBindWeaponItem_DL> onHeroSelect, Action<GUI_HeroBindWeaponItem_DL> onHeroDeselect, DisplayWeapon bindWeapon)
    {
        BindHero = hero;
        OnHeroSelect = onHeroSelect;
        OnHeroDeselect = onHeroDeselect;
        BindWeapon = bindWeapon;
        if (null != BindHero)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
            if (null != heroTemplate)
            {
                HeroSimpleInfo.StarText.text = heroTemplate.Star.ToString();
                HeroSimpleInfo.LevelText.text = BindHero.Level.ToString();
                GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, HeroSimpleInfo.HeadIcon);
                CSV_c_school_config heroSchool = CSV_c_school_config.FindData(heroTemplate.School);
                if (null != heroSchool)
                {
                    GUI_Tools.IconTool.SetIcon(heroSchool.Atlas, heroSchool.Icon, HeroSimpleInfo.SchoolIcon);
                }
            }
        }
        RefreshBindEquip();
        RegistEvent();
    }

    public void RefreshBindEquip()
    {
        if(null != BindWeapon && !BindWeapon())
        {
            if(null == BindHero.Ring)
            {
                EquipObjectRoot.SetActive(false);
            }
            else
            {
                EquipObjectRoot.SetActive(true);
                SetRing();
            }
        }
        else
        {
            if(null == BindHero.Weapon)
            {
                EquipObjectRoot.SetActive(false);
            }
            else
            {
                EquipObjectRoot.SetActive(true);
                SetWeapon();
            }
        }
    }
    #endregion

    #region toggle
    protected override void OnSelected()
    {
        //set head icon bg
        //set equip icon bg
        if(null != OnHeroSelect)
        {
            OnHeroSelect(this);
        }
    }

    protected override void OnDeSelected()
    {
        //set head icon bg
        //set equip icon bg
        if(null != OnHeroDeselect)
        {
            OnHeroDeselect(this);
        }
    }

    protected override void OnRecycle()
    {
        BindHero = null;
        UnRegistEvent();
    }

    void OnDisable()
    {
        UnRegistEvent();
    }

    void RegistEvent()
    {
        DataCenter.PlayerDataCenter.OnEquipUpToHero += OnEquipUpRsp;
        DataCenter.PlayerDataCenter.OnEquipDownFromHero += OnEquipDismissRsp;
        DataCenter.PlayerDataCenter.OnWeaponRefine += OnRefineRsp;
        DataCenter.PlayerDataCenter.OnWeaponReform += OnReformWordPropertyRsp;
    }

    void UnRegistEvent()
    {
        DataCenter.PlayerDataCenter.OnEquipUpToHero -= OnEquipUpRsp;
        DataCenter.PlayerDataCenter.OnEquipDownFromHero -= OnEquipDismissRsp;
        DataCenter.PlayerDataCenter.OnWeaponRefine -= OnRefineRsp;
        DataCenter.PlayerDataCenter.OnWeaponReform -= OnReformWordPropertyRsp;
    }

    void OnRefineRsp(uint oldWeaponServerId, uint newWeaponServerId)
    {
        RefreshBindEquip();
    }

    void OnEquipDismissRsp(uint heroServerId, uint equipServerId)
    {
        if (heroServerId == BindHero.ServerId)
        {
            RefreshBindEquip();
        }
    }

    void OnEquipUpRsp(uint heroServerId, uint equipServerId)
    {
        if (heroServerId == BindHero.ServerId)
        {
            RefreshBindEquip();
        }
    }
    #endregion

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroBindWeaponItem dataComponent = gameObject.GetComponent<GUI_HeroBindWeaponItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroBindWeaponItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            EquipObjectRoot = dataComponent.EquipObjectRoot;
            EquipSimpleInfo = dataComponent.EquipSimpleInfo;
            EquipBgIcon = dataComponent.EquipBgIcon;
            HeroSimpleInfo = dataComponent.HeroSimpleInfo;
            HeadIconBg = dataComponent.HeadIconBg;
        }
    }
}
