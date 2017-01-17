using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_GetNewGoddessUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_GetNewGoddessUI dataComponent = gameObject.GetComponent<GUI_GetNewGoddessUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GetNewGoddessUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            RemindTextFormater = dataComponent.RemindTextFormater;
            RemindText = dataComponent.RemindText;
            GoddessIcon = dataComponent.GoddessIcon;
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
    }
    #endregion
    #region window logic
    public string RemindTextFormater;
    public Text RemindText;
    public Image GoddessIcon;

    CSV_c_goddess_config NewGoddessConfig;

    public void AlertNewGoddess(int newGoddessCsvId)
    {
        NewGoddessConfig = CSV_c_goddess_config.FindData(newGoddessCsvId);
    }

    protected override void OnStart()
    {
        RefreshGoddessInfo();
    }

    void RefreshGoddessInfo()
    {
        if (null != NewGoddessConfig)
        {
            GUI_Tools.IconTool.SetIcon(NewGoddessConfig.HeadIconAtlas, NewGoddessConfig.HeadIcon, GoddessIcon);
            string content = string.Format(RemindTextFormater, NewGoddessConfig.GoddessName);
            GUI_Tools.TextTool.SetText(RemindText, content);
        }
    }

    void OnConfirmButtonClicked()
    {
        DataCenter.GoddessInOutInfo inOutInfo = DataCenter.PlayerDataCenter.GoddessData.GetGoddessInOut();
        if (null != inOutInfo)
        {
            switch (inOutInfo.inOut)
            {
                case 0://入队
                    {
                        AlertNewGoddess(inOutInfo.CsvId);
                        RefreshGoddessInfo();
                        break;
                    }
                case 1://离队
                    {
                        GUI_GoddessLeaveUI_DL goddessLeave = GUI_Manager.Instance.ShowWindowWithName<GUI_GoddessLeaveUI_DL>("UI_Goddess_Leave", false);
                        if (null != goddessLeave)
                        {
                            goddessLeave.AlertGoddessLeave(inOutInfo.CsvId);
                        }
                        HideWindow();
                        break;
                    }
            }
        }
        else
        {
            HideWindow();
        }
    }
    #endregion
}
