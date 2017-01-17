using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_HeroManageSimpleInfo_DL : GUI_HeroSimpleInfo_DL
{
    public GameObject TeamLeaderIcon;
    public Image UpdateIcon;
    public GameObject ExtendMask;
    public GameObject SafeLockMask;
    public delegate void OnSelectHero(GUI_HeroManageSimpleInfo_DL hss);
    OnSelectHero _OnSelectHero;
    OnSelectHero _OnDeSelectHero;
    public void SetHeroManageSimpleInfo(DataCenter.Hero hero, OnSelectHero onSelectHero, OnSelectHero onDeSelectHero, bool lockMaskOn)
    {
        if (null != hero)
        {
            _OnSelectHero = onSelectHero;
            _OnDeSelectHero = onDeSelectHero;
            base.SetHeroSimpleInfo(hero);
            ShowExtendMask(false);
            TeamLeaderIcon.SetActive(hero.ServerId == DataCenter.PlayerDataCenter.RepresentHero);
        }
        else
        {
            ShowExtendMask(true);
        }
        ActiveSafeMask(lockMaskOn);
    }

    void ShowExtendMask(bool show)
    {
        if (!show && ExtendMask.activeInHierarchy)
        {
            ExtendMask.SetActive(false);
        }
        else if (show && !ExtendMask.activeInHierarchy)
        {
            ExtendMask.SetActive(true);
        }
    }

    protected override void OnSelected()
    {
        if (null != _OnSelectHero)
        {
            _OnSelectHero(this);
        }
    }

    protected override void OnDeSelected()
    {
        if (null != _OnDeSelectHero)
        {
            _OnDeSelectHero(this);
        }
    }

    void ActiveSafeMask(bool active)
    {
        if (null != Hero)
        {
            SafeLockMask.SetActive(Hero.IsLock && active);
        }
        else if (SafeLockMask.activeInHierarchy)
        {
            SafeLockMask.SetActive(false);
        }
    }

    public void DisplayHeroDetail()
    {
        GUI_HeroDetailUI_DL heroDetail = GUI_Manager.Instance.ShowWindowWithName<GUI_HeroDetailUI_DL>("GUI_HeroDetailUI", false);
        heroDetail.ShowHero(Hero, HeroTemplate);
    }

    protected override void OnRecycle()
    {
        HeroTemplate = null;
        Hero = null;
    }

    public void SetUpdate()
    {
        UpdateIcon.gameObject.SetActive(true);
    }

    public void OnExtendHeroBagButtonClicked()
    {
        GUI_ExtendBagUI_DL extendUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExtendBagUI_DL>("UI_Extend_Hero", false);
        extendUI.ExtendBag(PbCommon.EExendBagType.E_Extend_Hero_Bag);
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroManageSimpleInfo dataComponent = gameObject.GetComponent<GUI_HeroManageSimpleInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroManageSimpleInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            TeamLeaderIcon = dataComponent.TeamLeaderIcon;
            UpdateIcon = dataComponent.UpdateIcon;
            ExtendMask = dataComponent.ExtendMask;
            SafeLockMask = dataComponent.SafeLockMask;
            dataComponent.ExtendButton.onClick.AddListener(OnExtendHeroBagButtonClicked);
        }
    }
}
