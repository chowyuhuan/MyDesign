using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Network;

public sealed class GUI_EvolutionUI_DL : GUI_Window_DL
{
    GameObject CurrentHeroObject;
    GameObject EvolutionHeroObject;
    public GUI_EvolutionHeroInfo_DL CurrentHero;
    public GUI_EvolutionHeroInfo_DL EvolutionHero;
    CSV_b_hero_template _EvolutionTemplate;
    DataCenter.Hero _EvolutionHero;
    DataCenter.Hero _CurrentHero;

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnHeroEvolution += OnEvolutionSuccess;
        GUI_Root_DL.Instance.HideLayer("Default");
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnHeroEvolution -= OnEvolutionSuccess;
        GUI_Root_DL.Instance.ShowLayer("Default");
    }

    public void SetEvolutionInfo(DataCenter.Hero currentHero, CSV_b_hero_template curHeroCSV)
    {
        _CurrentHero = currentHero;
        _EvolutionTemplate = CSV_b_hero_template.FindData(curHeroCSV.EvolutionHeroId);
        CurrentHero = CurrentHeroObject.GetComponent<GUI_EvolutionHeroInfo_DL>();
        EvolutionHero = EvolutionHeroObject.GetComponent<GUI_EvolutionHeroInfo_DL>();
        CurrentHero.SetHeroInfo(curHeroCSV, (int)currentHero.Level, false);
        EvolutionHero.SetHeroInfo(_EvolutionTemplate, 1, true);
    }

    public void OnEvolutionButtonClicked()
    {
        /*
        if (false)
        {
            _CurrentHero.CsvId = _EvolutionTemplate.Id;
            _CurrentHero.EnhanceExp = 0;
            _CurrentHero.EnhanceLevel = 0;
            _CurrentHero.Exp = 0;
            _CurrentHero.CalculateAttribute();

            GUI_HeroDetailUI_DL heroDetail = GUI_Manager.Instance.FindWindowWithName<GUI_HeroDetailUI_DL>("GUI_HeroDetailUI", false);
            heroDetail.OnHeroDataChange(_CurrentHero.ServerId);
            GUI_TrainUI_DL trainUI = GUI_Manager.Instance.FindWindowWithName<GUI_TrainUI_DL>("GUI_TrainUI", false);
            if (null != trainUI)
            {
                trainUI.SetTrainHero(_CurrentHero, _EvolutionTemplate);
            }
            GUI_HeroManage_DL heroManageUI = GUI_Manager.Instance.FindWindowWithName<GUI_HeroManage_DL>("GUI_HeroManage", false);
            if (null != heroManageUI)
            {
                heroManageUI.OnHeroDataChange(_CurrentHero.ServerId);
            }
            OnEvolutionSuccess(_CurrentHero.ServerId);
        }
        else
         * */
        {
            gsproto.HeroEvolutionReq evolutionReq = new gsproto.HeroEvolutionReq();
            evolutionReq.hero_id = _CurrentHero.ServerId;
            evolutionReq.session_id = DataCenter.PlayerDataCenter.SessionId;
            Network.NetworkManager.SendRequest(ProtocolDataType.TcpShort, evolutionReq);
        }
    }

    void OnEvolutionSuccess(uint heroId)
    {
        HideWindow();
        //GUI_Manager.Instance.HideAllWindow();
        GUI_MessageManager.Instance.ShowErrorTip("勇士进化成功");
        EvolutionHelper.CurrentHeroTemplate = CurrentHero.HeroTemplate;
        EvolutionHelper.EvolutionHero = DataCenter.PlayerDataCenter.GetHero(heroId);

        //ProgressLoading.PL_Manager.Instance.LoadSceneAsync("Evolution", true);
        GUI_Manager.Instance.ShowWindowWithName("GUI_EvolutionAnimUI", false);
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_EvolutionUI dataComponent = gameObject.GetComponent<GUI_EvolutionUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_EvolutionUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            CurrentHeroObject = dataComponent.CurrentHero;
            EvolutionHeroObject = dataComponent.EvolutionHero;
            dataComponent.ConrimEvolutionButton.onClick.AddListener(OnEvolutionButtonClicked);
        }
    }
}