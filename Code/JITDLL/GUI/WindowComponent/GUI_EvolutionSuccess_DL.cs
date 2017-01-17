using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public sealed class GUI_EvolutionSuccess_DL : GUI_Window_DL
{
    GameObject EvolutionHeroObject;
    public GUI_EvolutionHeroInfo_DL HeroInfo;
    public Text EvolutionInfo;

    protected override void OnStart()
    {
        HeroInfo = EvolutionHeroObject.GetComponent<GUI_EvolutionHeroInfo_DL>();
        CSV_b_hero_template evolutionTemplate = CSV_b_hero_template.FindData(EvolutionHelper.EvolutionHero.CsvId);
        SetEvolutionHeroInfo(evolutionTemplate);
        EvolutionHelper.EvolutionHero = null;
    }

    void SetEvolutionHeroInfo(CSV_b_hero_template heroTemplate)
    {
        HeroInfo.SetHeroInfo(heroTemplate, 1, false);
        string evolutionStr;
        TextLocalization.GetText("Evolution_Des", out evolutionStr);
        EvolutionInfo.text = evolutionStr + heroTemplate.Name + "!";
    }

    public override void PreHideWindow()
    {
        //ProgressLoading.PL_Manager.Instance.LoadSceneAsync("MainCity", true);
        //GUI_Manager.Instance.PushWndToWaitingQueue("MainUI");
        GUI_Manager.Instance.HideWindowWithName("GUI_EvolutionAnimUI");
    }
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_EvolutionSuccess dataComponent = gameObject.GetComponent<GUI_EvolutionSuccess>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_EvolutionSuccess,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            EvolutionHeroObject = dataComponent.HeroInfo;
            EvolutionInfo = dataComponent.EvolutionInfo;
        }
    }
}
