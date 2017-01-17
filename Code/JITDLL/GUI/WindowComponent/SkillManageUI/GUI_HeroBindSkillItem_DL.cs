using UnityEngine;

public sealed class GUI_HeroBindSkillItem_DL : GUI_ToggleItem_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroBindSkillItem dataComponent = gameObject.GetComponent<GUI_HeroBindSkillItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroBindSkillItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SkillSimpleInfo = dataComponent.SkillSimpleInfo;
            HeroSimpleInfo = dataComponent.HeroSimpleInfo;
        }
    }
    #endregion

    #region item logic
    public GUI_SkillSimpleInfo SkillSimpleInfo;
    public GUI_ActorSimpleInfo HeroSimpleInfo;

    public delegate void OnBindItemSelected(GUI_HeroBindSkillItem_DL bindItem);
    OnBindItemSelected OnItemSelected;
    OnBindItemSelected OnItemDeSelected;

    public delegate DataCenter.Hero GetHero();
    GetHero GetCurrentHero;

    public DataCenter.Hero Hero { get; protected set; }
    public void ShowHero(DataCenter.Hero hero, OnBindItemSelected onItemSelected, OnBindItemSelected onItemDeselected, GetHero getCurrentHero)
    {
        Hero = hero;
        OnItemSelected = onItemSelected;
        OnItemDeSelected = onItemDeselected;
        GetCurrentHero = getCurrentHero;
        InitHeroInfo();
        RefreshSkillInfo();
    }

    void InitHeroInfo()
    {
        if (null != Hero)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(Hero.CsvId);
            if (null != heroTemplate)
            {
                HeroSimpleInfo.StarText.text = heroTemplate.Star.ToString();
                HeroSimpleInfo.LevelText.text = Hero.Level.ToString();
                GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, HeroSimpleInfo.HeadIcon);
                CSV_c_school_config heroSchool = CSV_c_school_config.FindData(heroTemplate.School);
                if (null != heroSchool)
                {
                    GUI_Tools.IconTool.SetIcon(heroSchool.Atlas, heroSchool.Icon, HeroSimpleInfo.SchoolIcon);
                }
            }
        }
    }

    void RefreshSkillInfo()
    {
        if(null != Hero)
        {
            CSV_b_skill_template skillTemplate = CSV_b_skill_template.FindData((int)Hero.SkillServerId);
            if(null != skillTemplate)
            {
                string levelText;
                DataCenter.SpecialSkillUnit skillUnit = DataCenter.PlayerDataCenter.SpecialSkillData.GetSpecialSkill((uint)skillTemplate.GroupId);
                if(TextLocalization.GetText(skillUnit.Level == skillUnit.MaxLevel ? TextId.Top_Level : TextId.Level, out levelText))
                {
                    SkillSimpleInfo.Level.text = levelText + skillTemplate.Level.ToString();
                }
                else
                {
                    SkillSimpleInfo.Level.text = skillTemplate.Level.ToString();
                }

                GUI_Tools.IconTool.SetSkillIcon(skillTemplate.Id, SkillSimpleInfo.Icon);
            }
        }
    }
    #endregion

    #region toggle logic
    public override void RefreshObject()
    {
        if(null != GetCurrentHero)
        {
            DataCenter.Hero curSelectHero = GetCurrentHero();
            if(null != curSelectHero && curSelectHero.ServerId == Hero.ServerId)
            {
                RefreshSkillInfo();
            }
        }
        else
        {
            RefreshSkillInfo();
        }
    }

    protected override void OnSelected()
    {
        if(null != OnItemSelected)
        {
            OnItemSelected(this);
        }
    }

    protected override void OnDeSelected()
    {
        if (null != OnItemDeSelected)
        {
            OnItemDeSelected(this);
        }
    }

    protected override void OnRecycle()
    {
        Hero = null;
        OnItemSelected = null;
        OnItemDeSelected = null;
        RegistToGroup(null);
    }
    #endregion
}