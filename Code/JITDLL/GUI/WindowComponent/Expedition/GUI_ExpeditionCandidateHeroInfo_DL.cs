using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public sealed class GUI_ExpeditionCandidateHeroInfo_DL : GUI_ToggleItem_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ExpeditionCandidateHeroInfo dataComponent = gameObject.GetComponent<GUI_ExpeditionCandidateHeroInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExpeditionCandidateHeroInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HeroIcon = dataComponent.HeroIcon;
            TypeIcon = dataComponent.TypeIcon;
            SchoolIcon = dataComponent.SchoolIcon;
            StarBar = dataComponent.StarBar;
        }
    }
    #endregion

    #region candidate logic
    public Image HeroIcon;
    public Image TypeIcon;
    public Image SchoolIcon;
    public GameObject StarBar;
    public DataCenter.Hero CandidateHero { get; protected set; }
    Action<GUI_ExpeditionCandidateHeroInfo_DL> OnCandidateHeroSelect;
    Action<GUI_ExpeditionCandidateHeroInfo_DL> OnCandidateHeroDeslect;
    GUI_HeroStarBar_DL HeroStarBar;
    public delegate bool IsHeroSelect(DataCenter.Hero candidateHero);
    IsHeroSelect HeroSelect;
    public void DisplayHero(DataCenter.Hero hero, Action<GUI_ExpeditionCandidateHeroInfo_DL> onSelect, Action<GUI_ExpeditionCandidateHeroInfo_DL> onDeselect, IsHeroSelect heroSelect)
    {
        CandidateHero = hero;
        OnCandidateHeroSelect = onSelect;
        OnCandidateHeroDeslect = onDeselect;
        HeroSelect = heroSelect;
        SetHeroInfo();
    }

    void SetHeroInfo()
    {
        if(null != CandidateHero)
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(CandidateHero.CsvId);
            if(null != heroTemplate)
            {
                GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, HeroIcon);
                GUI_Tools.IconTool.SetHeroTypeIcon(heroTemplate.HeroType, TypeIcon);
                GUI_Tools.IconTool.SetShoolIcon(heroTemplate.School, SchoolIcon, false);
                if(HeroStarBar == null)
                {
                    HeroStarBar = StarBar.GetComponent<GUI_HeroStarBar_DL>();
                }
                if(null != HeroStarBar)
                {
                    HeroStarBar.SetStarNum(heroTemplate.Star);
                }
            }
        }
    }

    public override void RefreshObject()
    {
        if(null != HeroSelect && HeroSelect(CandidateHero))
        {
            Select();
        }
        else
        {
            DeSelect();
        }
    }

    protected override void OnSelected()
    {
        if(null != OnCandidateHeroSelect)
        {
            OnCandidateHeroSelect(this);
        }
    }

    protected override void OnDeSelected()
    {
        if (null != OnCandidateHeroDeslect)
        {
            OnCandidateHeroDeslect(this);
        }
    }

    protected override void OnRecycle()
    {
        CandidateHero = null;
        OnCandidateHeroSelect = null;
        OnCandidateHeroDeslect = null;
        HeroSelect = null;
    }
    #endregion
}
