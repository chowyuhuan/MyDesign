using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_GetNewHeroUI_DL : GUI_Window_DL
{
    public Text Name;
    public Image DisplayIcon;
    GameObject HeroStarBarObject;
    public GUI_HeroStarBar_DL StarBar;
    public string HeroAction;
    public GUI_Transform HeroTransform;
    CSV_b_hero_template _HeroTemplate;

    void OnEnable()
    {
        GUI_Root_DL.Instance.ShowLayer("Default");
    }

    void OnDisable()
    {
        GUI_Root_DL.Instance.HideLayer("Default");
    }

    protected override void OnStart()
    {
        StarBar = HeroStarBarObject.GetComponent<GUI_HeroStarBar_DL>();
        DataCenter.Hero hero = DataCenter.PlayerDataCenter.OverLevelData.DropHero;
        if (null != hero)
        {
            _HeroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
            StarBar.SetStarNum(_HeroTemplate.Star);
            DisplayModel(HeroTransform, HeroAction);
        }
    }

    void DisplayModel(GUI_Transform heroTrans, string heroAction)
    {
        GameObject heroModel;
        if (GUI_Tools.ModelTool.SpawnModel(DisplayIcon.gameObject, _HeroTemplate.Prefab, heroTrans, out heroModel))
        {
            Animator anim;
            if (GUI_Tools.ModelTool.AnimateUIModel(heroModel, _HeroTemplate.UiAnimCtrl, out anim))
            {
                anim.Play(heroAction);
            }
        }
    }

    public void OnConfirmButtonClicked()
    {
        HideWindow();
        GUI_BattleManager.Instance.OnStageClear();
    }
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_GetNewHeroUI dataComponent = gameObject.GetComponent<GUI_GetNewHeroUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GetNewHeroUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Name = dataComponent.Name;
            DisplayIcon = dataComponent.DisplayIcon;
            HeroStarBarObject = dataComponent.StarBar;
            HeroAction = dataComponent.HeroAction;
            HeroTransform = dataComponent.HeroTransform;
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
    }
}
