using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_RegimentPassiveSkillLevelUpUI_DL : GUI_Window_DL
{
    public Image SkillIcon;
    public Image SkillBuffIcon;
    public Text SkillName;
    public Text OldLevelText;
    public Text NewLevelText;
    public Text ConditionText;
    public Text SkillEffectText;
    public Image OldDescriptionIcon;
    public Text OldEffectCount;
    public Image NewDescriptionIcon;
    public Text NewEffectCount;

    protected override void OnStart()
    {
        OnConfirmButtonClicked();
    }

    public void OnConfirmButtonClicked()
    {
        if (DataCenter.PlayerDataCenter.GroupPassiveLevelUpList.Count > 0)
        {
            DataCenter.GroupPassiveLevelUpInfo levelUpInfo = DataCenter.PlayerDataCenter.GroupPassiveLevelUpList[0];
            DataCenter.PlayerDataCenter.GroupPassiveLevelUpList.RemoveAt(0);
            DisplayPassiveLevel(levelUpInfo);
        }
        else
        {
            HideWindow();
        }
    }

    void DisplayPassiveLevel(DataCenter.GroupPassiveLevelUpInfo levelUpInfo)
    {
        #region old level info
        int combinedId = (int)(levelUpInfo.PassiveType * GUI_Const.Config_Type_Distance + levelUpInfo.StartPassiveLevel);
        CSV_c_hero_group_effect_template heroGroupEffect = CSV_c_hero_group_effect_template.FindData(combinedId);
        if (null != heroGroupEffect)
        {
            OldLevelText.text = heroGroupEffect.LevelText;
            OldEffectCount.text = levelUpInfo.StartEffectValue.ToString();
            GUI_Tools.IconTool.SetIcon(heroGroupEffect.DescriptionAtlas, heroGroupEffect.DescriptionSrpite, OldDescriptionIcon);
        }
        #endregion

        #region new level info
        combinedId = (int)(levelUpInfo.PassiveType * GUI_Const.Config_Type_Distance + levelUpInfo.EndPassiveLevel);
        heroGroupEffect = CSV_c_hero_group_effect_template.FindData(combinedId);
        if (null != heroGroupEffect)
        {
            GUI_Tools.IconTool.SetIcon(heroGroupEffect.DisplayIconAtlas, heroGroupEffect.DisplayIconSprite, SkillIcon);
            GUI_Tools.IconTool.SetIcon(heroGroupEffect.EffectBuffAtlas, heroGroupEffect.EffectBuffSprite, SkillBuffIcon);
            SkillName.text = heroGroupEffect.Name;
            NewLevelText.text = heroGroupEffect.LevelText;
            ConditionText.text = heroGroupEffect.Condition;
            SkillEffectText.text = heroGroupEffect.Description;
            NewEffectCount.text = levelUpInfo.EndEffectValue.ToString();
            GUI_Tools.IconTool.SetIcon(heroGroupEffect.DescriptionAtlas, heroGroupEffect.DescriptionSrpite, NewDescriptionIcon);
        }
        #endregion
    }

    public void OnRegimentInfoClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("UI_HeroGroup", false);
        HideWindow();
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_RegimentPassiveSkillLevelUpUI dataComponent = gameObject.GetComponent<GUI_RegimentPassiveSkillLevelUpUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_RegimentPassiveSkillLevelUpUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SkillIcon = dataComponent.SkillIcon;
            SkillBuffIcon = dataComponent.SkillBuffIcon;
            SkillName = dataComponent.SkillName;
            OldLevelText = dataComponent.OldLevelText;
            NewLevelText = dataComponent.NewLevelText;
            ConditionText = dataComponent.ConditionText;
            SkillEffectText = dataComponent.SkillEffectText;
            OldDescriptionIcon = dataComponent.OldDescriptionIcon;
            OldEffectCount = dataComponent.OldEffectCount;
            NewDescriptionIcon = dataComponent.NewDescriptionIcon;
            NewEffectCount = dataComponent.NewEffectCount;

            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            dataComponent.RegimentInfoButton.onClick.AddListener(OnRegimentInfoClicked);
        }
    }
}
