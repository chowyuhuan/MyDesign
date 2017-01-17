using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_HeroSelectSimpleInfo_DL : GUI_HeroSimpleInfo_DL
{
    public Image SkillIcon;
    public Image SpecialSkillIcon;
    public GameObject CaptainTag;

    public delegate void OnSelectHero(GUI_HeroSelectSimpleInfo_DL hss);
    OnSelectHero _OnSelectHero;
    OnSelectHero _OnDeSelectHero;
    OnSelectHero _OnRefreshSelectionInfo;

    public void SetHeroInfo(DataCenter.Hero hero, OnSelectHero onSelectHero, OnSelectHero onDeSelectHero, OnSelectHero onRefreshSelectionInfo)
    {
        base.SetHeroSimpleInfo(hero);
        _OnSelectHero = onSelectHero;
        _OnDeSelectHero = onDeSelectHero;
        _OnRefreshSelectionInfo = onRefreshSelectionInfo;
        TagAsCaptain(false);
        SetSkillIcon(HeroTemplate.SkillID1, SkillIcon);
        if(null != hero)
        {
            SetSkillIcon((int)hero.SkillServerId, SpecialSkillIcon);
        }
        else
        {
            SetSkillIcon(0, SpecialSkillIcon);
        }
    }

    public void TagAsCaptain(bool captain)
    {
        CaptainTag.SetActive(captain);
    }

    void SetSkillIcon(int skillId, Image iconImage)
    {
        SKILL.Skill data;
        if (SkillDataCenter.Instance.TryToGetSkill(skillId, out data))
        {
            GUI_Tools.IconTool.SetIcon(data.IconAtlas, data.IconSprite, iconImage);
        }
    }

    protected override void OnRecycle()
    {
        Hero = null;
        HeroTemplate = null;
        _OnSelectHero = null;
        _OnDeSelectHero = null;
        _OnRefreshSelectionInfo = null;
        if (IsSelect)
        {
            DeSelect();
        }
    }

    public override void RefreshObject()
    {
        if (null != _OnRefreshSelectionInfo)
        {
            _OnRefreshSelectionInfo(this);
        }
    }

    protected override void OnSelected()
    {
        if (null != _OnSelectHero)
        {
            _OnSelectHero(this);
        }
        base.SelectHero();
    }

    protected override void OnDeSelected()
    {
        if (null != _OnDeSelectHero)
        {
            _OnDeSelectHero(this);
        }
        TagAsCaptain(false);
        base.DeSelectHero();
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroSelectSimpleInfo dataComponent = gameObject.GetComponent<GUI_HeroSelectSimpleInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroSelectSimpleInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SkillIcon = dataComponent.SkillIcon;
            SpecialSkillIcon = dataComponent.SpecialSkillIcon;
            CaptainTag = dataComponent.CaptainTag;
        }
    }
}
