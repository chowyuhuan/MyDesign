using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_HeroBattleSimpleInfo_DL : GUI_ActorBattleSimpleInfo_DL
{
    public GameObject CaptainAlertIcon;
    public GameObject CaptainTagIcon;
    public delegate void OnBattleHeroClicked(DataCenter.Hero hero);
    OnBattleHeroClicked _OnBattleHeroClicked;
    DataCenter.Hero _SelectedHero;

    public void SetHeroSimgpleInfo(DataCenter.Hero hero, OnBattleHeroClicked battleHeroClicked)
    {
        _SelectedHero = hero;
        _OnBattleHeroClicked = battleHeroClicked;
        if (null == hero)
        {
            SetHeroInfo(null);
        }
        else
        {
            CSV_b_hero_template heroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
            SetHeroInfo(heroTemplate);
        }

    }

    public void TagAsCaptain(bool captain)
    {
        CaptainTagIcon.SetActive(captain);
    }

    public void OnClick()
    {
        if (null != _OnBattleHeroClicked)
        {
            _OnBattleHeroClicked(_SelectedHero);
        }
    }

    public void AlertCaptain(bool alert)
    {
        CaptainAlertIcon.SetActive(alert);
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroBattleSimpleInfo dataComponent = gameObject.GetComponent<GUI_HeroBattleSimpleInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroBattleSimpleInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            CaptainAlertIcon = dataComponent.CaptainAlertIcon;
            CaptainTagIcon = dataComponent.CaptainTagIcon;
            dataComponent.ItemButton.onClick.AddListener(OnClick);
        }
    }
}
