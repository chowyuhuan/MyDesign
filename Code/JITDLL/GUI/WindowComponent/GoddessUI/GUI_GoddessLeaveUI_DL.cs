using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_GoddessLeaveUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_GoddessLeaveUI dataComponent = gameObject.GetComponent<GUI_GoddessLeaveUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GoddessLeaveUI,GameObject：" + gameObject.name, gameObject);
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

    DataCenter.Goddess LeaveGoddess;
    CSV_c_goddess_config LeaveGoddessConfig;
    bool AlertGoddessLeaving = false;

    public void GoddessLeave(DataCenter.Goddess leaveGoddess, CSV_c_goddess_config goddessConfig)
    {
        AlertGoddessLeaving = false;
        LeaveGoddess = leaveGoddess;
        LeaveGoddessConfig = goddessConfig;
        if(null == goddessConfig || null == LeaveGoddess)
        {
            HideWindow();
        }
    }

    public void AlertGoddessLeave(int csvId)
    {
        AlertGoddessLeaving = true;
        LeaveGoddess = DataCenter.PlayerDataCenter.GoddessData.GetGoddess(csvId);
        LeaveGoddessConfig = CSV_c_goddess_config.FindData(csvId);
    }

    protected override void OnStart()
    {
        RefreshGoddessInfo();
    }

    void RefreshGoddessInfo()
    {
        if (null != LeaveGoddessConfig && null != LeaveGoddess)
        {
            GUI_Tools.IconTool.SetIcon(LeaveGoddessConfig.HeadIconAtlas, LeaveGoddessConfig.HeadIcon, GoddessIcon);
            string content = string.Format(RemindTextFormater, LeaveGoddess.LockChapter.ToString(), LeaveGoddessConfig.GoddessName);
            GUI_Tools.TextTool.SetText(RemindText, content);
        }
    }

    void OnConfirmButtonClicked()
    {
        if(AlertGoddessLeaving)
        {
            DataCenter.GoddessInOutInfo inOutInfo = DataCenter.PlayerDataCenter.GoddessData.GetGoddessInOut();
            if (null != inOutInfo)
            {
                switch (inOutInfo.inOut)
                {
                    case 0://入队
                        {
                            GUI_GetNewGoddessUI_DL getNewGoddess = GUI_Manager.Instance.ShowWindowWithName<GUI_GetNewGoddessUI_DL>("UI_Goddess_Join", false);
                            if (null != getNewGoddess)
                            {
                                getNewGoddess.AlertNewGoddess(inOutInfo.CsvId);
                            }
                            HideWindow();
                            break;
                        }
                    case 1://离队
                        {
                            AlertGoddessLeave(inOutInfo.CsvId);
                            RefreshGoddessInfo();
                            break;
                        }
                }
            }
            else
            {
                HideWindow();
            }
        }
        else
        {
            HideWindow();
        }
    }
    #endregion
}
