using UnityEngine;
using System.Collections;

public sealed class GUI_PassLevelUI_DL : GUI_LevelBonus_DL
{
    protected override void PlayBonusAction()
    {
    }

    void OnEnable()
    {
        GUI_Root_DL.Instance.ShowLayer("Default");
    }

    void OnDisable()
    {
        GUI_Root_DL.Instance.HideLayer("Default");
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_PassLevelUI dataComponent = gameObject.GetComponent<GUI_PassLevelUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_PassLevelUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            dataComponent.RetryButton.onClick.AddListener(OnReTryButtonClicked);
            dataComponent.NextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
            dataComponent.ReturnCityButton.onClick.AddListener(OnBackTownBtnClicked);
        }
    }
}