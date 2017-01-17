using UnityEngine;
using System.Collections;

public sealed class GUI_PauseUI_DL : GUI_Window_DL
{
    public void OnContinueButtonClick()
    {
        SkillGenerator.Instance.Continue();
        HideWindow();
        Time.timeScale = 1f;
    }

    public void OnExitButtonClicked()
    {
        BattleManager_DL.Instance.SendPassOverReq(PbCommon.EPassResult.E_Result_Exit);
        SkillGenerator.Instance.Clear();
        Time.timeScale = 1f;
        GUI_Manager.Instance.HideAllWindow();
        PL_Manager_DL.Instance.LoadSceneAsync("MainCity", true);
        GUI_Manager.Instance.PushWndToWaitingQueue("MainUI");
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_PauseUI dataComponent = gameObject.GetComponent<GUI_PauseUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_PauseUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            dataComponent.ReturnToMainCityButton.onClick.AddListener(OnExitButtonClicked);
            dataComponent.ContinueButton.onClick.AddListener(OnContinueButtonClick);
        }
    }
}