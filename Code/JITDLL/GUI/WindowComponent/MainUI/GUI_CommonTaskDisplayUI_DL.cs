using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_CommonTaskDisplayUI_DL : GUI_Window_DL
{
    #region task logic
    public Text TypeName;
    public Text TaskName;
    public Text TaskDescription;
    public Text ScheduleText;
    public Slider ScheduleSlider;
    public Image AwardIcon;
    public Text AwardCount;
    public Text ChangeText;

    DataCenter.Task Task;

    public void DisplayTask(DataCenter.Task task)
    {
        Task = task;
        if(null == task)
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        CSV_b_task_template taskTemplate = CSV_b_task_template.FindData(Task.CsvId);
        if(null != taskTemplate)
        {
            TypeName.text = taskTemplate.TypeName;
            TaskName.text = taskTemplate.Name;
            TaskDescription.text = taskTemplate.Description;
            ScheduleText.text = string.Format("{0}/{1}", Task.TaskSchedule.ToString(), taskTemplate.TargetValue);
            ScheduleSlider.value = Mathf.Clamp01((float)Task.TaskSchedule / taskTemplate.TargetValue);
            GUI_Tools.ItemTool.SetAwardItemInfo((PbCommon.EAwardType)taskTemplate.AwardType, null, AwardCount, AwardIcon, taskTemplate.AwardValue);
            RefreshButtonState();
        }
        else
        {
            HideWindow();
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange += OnRefuseTaskRsp;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange += OnAcceptTaskRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange -= OnRefuseTaskRsp;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange -= OnAcceptTaskRsp;
    }

    void RefreshButtonState()
    {
        switch ((PbCommon.ETaskStateType)Task.TaskState)
        {
            case PbCommon.ETaskStateType.E_Task_State_Not_Finish:
                {
                    ChangeText.text = "立即跳转";
                    break;
                }
            case PbCommon.ETaskStateType.E_Task_State_Not_Receive:
                {
                    ChangeText.text = "接受";
                    break;
                }
            default:
                {
                    ChangeText.text = "Unknown";
                    break;
                }
        }
    }

    void OnRefuseButtonClicked()
    {
        if(Task.TaskState == (int)PbCommon.ETaskStateType.E_Task_State_Not_Finish)
        {
            GUI_RefuseTaskUI_DL refuseTaskUI = GUI_Manager.Instance.ShowWindowWithName<GUI_RefuseTaskUI_DL>("UI_MissionAbandon", false);
            if(null != refuseTaskUI)
            {
                refuseTaskUI.TryRefuseTask((uint)Task.CsvId);
            }
        }
    }

    void OnRefuseTaskRsp(uint position)
    {
        GUI_MessageManager.Instance.ShowErrorTip("放弃任务成功");
        HideWindow();
    }

    void OnChangeButtonClicked()
    {
        switch ((PbCommon.ETaskStateType)Task.TaskState)
        {
            case PbCommon.ETaskStateType.E_Task_State_Not_Finish:
                {
                    JumpToTask();
                    break;
                }
            case PbCommon.ETaskStateType.E_Task_State_Not_Receive:
                {
                    AcceptTask();
                    break;
                }
            default:
                {
                    ScheduleText.text = "Unknown";
                    break;
                }
        }
    }


    void JumpToTask()
    {
        GUI_MessageManager.Instance.ShowErrorTip("任务跳转");
        switch ((PbCommon.ETaskTargetType)Task.TaskTarget)
        {
            case PbCommon.ETaskTargetType.E_Target_Arena_Times:
                {
                    GUI_MessageManager.Instance.ShowErrorTip(10001);
                    break;
                }
            case PbCommon.ETaskTargetType.E_Target_Dungeon_Times:
                {
                    GUI_MessageManager.Instance.ShowErrorTip(10001);
                    break;
                }
            case PbCommon.ETaskTargetType.E_Target_Evolve_Hero_Times:
            case PbCommon.ETaskTargetType.E_Target_Feed_Fruit_Times:
                {
                    Building_DL.FocusBuilding(BuildingType.Hero);
                    GUI_Manager.Instance.ShowWindowWithName("GUI_HeroManage", false);
                    break;
                }
            case PbCommon.ETaskTargetType.E_Target_Kill_Monster_Count:
            case PbCommon.ETaskTargetType.E_Target_Pass_Level_Times:
                {
                    Building_DL.FocusBuilding(BuildingType.Battle);
                    CSV_b_task_template taskTemplate = CSV_b_task_template.FindData(Task.CsvId);
                    GUI_ChapterDetailUI_DL chapterDetail = GUI_Manager.Instance.ShowWindowWithName<GUI_ChapterDetailUI_DL>("ChapterDetailUI", false);
                    if(null != chapterDetail)
                    {
                        chapterDetail.ShowTargetLevel(taskTemplate.TargetLevelInterface);
                    }
                    break;
                }
            case PbCommon.ETaskTargetType.E_Target_Reform_Weapon_Times:
                {
                    Building_DL.FocusBuilding(BuildingType.Equipment);
                    GUI_Manager.Instance.ShowWindowWithName("GUI_EquipPackageUI", false);
                    break;
                }
            case PbCommon.ETaskTargetType.E_Target_Roast_Times:
                {
                    Building_DL.FocusBuilding(BuildingType.Bread);
                    GUI_Manager.Instance.ShowWindowWithName("GUI_BakeryUI", false);
                    break;
                }
            case PbCommon.ETaskTargetType.E_Target_Soul_Fortress_Success_Times:
            case PbCommon.ETaskTargetType.E_Target_Soul_Fortress_Times:
                {
                    GUI_MessageManager.Instance.ShowErrorTip(10001);
                    break;
                }
            case PbCommon.ETaskTargetType.E_Target_World_Lord_Times:
                {
                    GUI_MessageManager.Instance.ShowErrorTip(10001);
                    break;
                }
        }
    }

    void AcceptTask()
    {
        gsproto.ReceiveTaskReq req = new gsproto.ReceiveTaskReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        req.task_id = (uint)Task.CsvId;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnAcceptTaskRsp(uint position)
    {
        GUI_MessageManager.Instance.ShowErrorTip("接受任务成功");
        RefreshButtonState();
    }
    #endregion

    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_CommonTaskDisplayUI dataComponent = gameObject.GetComponent<GUI_CommonTaskDisplayUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_CommonTaskDisplayUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            TypeName = dataComponent.TypeName;
            TaskName = dataComponent.TaskName;
            TaskDescription = dataComponent.TaskDescription;
            ScheduleText = dataComponent.ScheduleText;
            ScheduleSlider = dataComponent.ScheduleSlider;
            AwardIcon = dataComponent.AwardIcon;
            AwardCount = dataComponent.AwardCount;
            ChangeText = dataComponent.ChangeText;

            dataComponent.RefuseButton.onClick.AddListener(OnRefuseButtonClicked);
            dataComponent.ChangeButton.onClick.AddListener(OnChangeButtonClicked);
        }
    }
    #endregion
}
