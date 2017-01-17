using UnityEngine;
using System.Collections;

public sealed class GUI_FailLevelUI_DL : GUI_LevelBonus_DL
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
        GUI_FailLevelUI dataComponent = gameObject.GetComponent<GUI_FailLevelUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_FailLevelUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            dataComponent.RetryButton.onClick.AddListener(OnReTryButtonClicked);
            dataComponent.ReturnCityButton.onClick.AddListener(OnBackTownBtnClicked);
        }
    }
}
