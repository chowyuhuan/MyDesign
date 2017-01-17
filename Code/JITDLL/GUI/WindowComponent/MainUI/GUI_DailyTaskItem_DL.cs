using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_DailyTaskItem_DL : GUI_ToggleItem_DL
{
    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_DailyTaskItem dataComponent = gameObject.GetComponent<GUI_DailyTaskItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_DailyTaskItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            TaskIcon = dataComponent.TaskIcon;
            TaskTypeBg = dataComponent.TaskTypeBg;
            TaskTypeName = dataComponent.TaskTypeName;
            TaskName = dataComponent.TaskName;
            Description = dataComponent.Description;
            TaskSchedule = dataComponent.TaskSchedule;
            TaskScheduleText = dataComponent.TaskScheduleText;
            AwardItem = dataComponent.AwardItem;
            TaskJumpButton = dataComponent.TaskJumpButton;
            AwardItemRoot = dataComponent.AwardItemRoot;
            TaskFinishLock = dataComponent.TaskFinishLock;
            AwardGotTag = dataComponent.AwardGotTag;
            GetAwardAlert = dataComponent.GetAwardAlert;

            TaskJumpButton.onClick.AddListener(OnJumpButtonClicked);
        }
    }
    #endregion

    #region task item logic
    public Image TaskIcon;
    public Image TaskTypeBg;
    public Text TaskTypeName;
    public Text TaskName;
    public Text Description;
    public Slider TaskSchedule;
    public Text TaskScheduleText;
    public GUI_ItemSimpleInfo AwardItem;
    public Button TaskJumpButton;
    public GameObject AwardItemRoot;
    public GameObject TaskFinishLock;
    public GameObject AwardGotTag;
    public GameObject GetAwardAlert;

    DataCenter.Task Task;
    CSV_b_task_template TaskTemplate;

    public void ShowTaskItem(DataCenter.Task task)
    {
        Task = task;
        RefreshTaskInfo();
        RegistEvent();
    }

    void RefreshTaskInfo()
    {
        TaskJumpButton.gameObject.SetActive(false);
        if (null != Task)
        {
            TaskTemplate = CSV_b_task_template.FindData(Task.CsvId);
            if (null != TaskTemplate)
            {
                GUI_Tools.IconTool.SetIcon(TaskTemplate.IconAtlas, TaskTemplate.IconSprite, TaskIcon);
                TaskName.text = TaskTemplate.Name;
                Description.text = TaskTemplate.Description;
                TaskSchedule.value = Mathf.Clamp01((float)Task.TaskSchedule / Task.TaskTargetValue);
                TaskScheduleText.text = string.Format("{0}/{1}", Task.TaskSchedule.ToString(), Task.TaskTargetValue);
                GUI_Tools.ItemTool.SetAwardItemInfo(TaskTemplate.AwardType, null, AwardItem.Name_Star_Count, AwardItem.Icon, TaskTemplate.AwardValue);
            }
        }
        RefreshTaskType();
        RefreshTaskState();
    }

    void RefreshTaskState()
    {
        if(null != Task)
        {
            switch((PbCommon.ETaskStateType)Task.TaskState)
            {
                case PbCommon.ETaskStateType.E_Task_State_Not_Finish:
                    {
                        AwardItemRoot.SetActive(true);
                        TaskFinishLock.SetActive(false);
                        GetAwardAlert.SetActive(false);
                        AwardGotTag.SetActive(false);
                        break;
                    }
                case PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award:
                    {
                        AwardItemRoot.SetActive(true);
                        TaskFinishLock.SetActive(true);
                        GetAwardAlert.SetActive(true);
                        AwardGotTag.SetActive(false);
                        break;
                    }
                case PbCommon.ETaskStateType.E_Task_State_Draw_Award:
                    {
                        AwardItemRoot.SetActive(false);
                        TaskFinishLock.SetActive(true);
                        GetAwardAlert.SetActive(false);
                        AwardGotTag.SetActive(true);
                        break;
                    }
            }
        }
    }

    void RefreshTaskType()
    {
        if(null != Task)
        {
            string typeName = null;
            switch ((PbCommon.ETaskType)Task.TaskType)
            {
                case PbCommon.ETaskType.E_Task_Daily:
                    {
                        TextLocalization.GetText(TextId.Daily, out typeName);
                        break;
                    }
                case PbCommon.ETaskType.E_Task_Weekly:
                    {
                        TextLocalization.GetText(TextId.Weekly, out typeName);
                        break;
                    }
            }
            TaskTypeName.text = typeName;
        }
    }

    void OnJumpButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip("任务跳转");
        switch((PbCommon.ETaskTargetType)Task.TaskTarget)
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
                    if(taskTemplate.TargetLevelInterface != 0)
                    {
                        GUI_ChapterDetailUI_DL chapterDetail = GUI_Manager.Instance.ShowWindowWithName<GUI_ChapterDetailUI_DL>("ChapterDetailUI", false);
                        if (null != chapterDetail)
                        {
                            chapterDetail.ShowTargetLevel(taskTemplate.TargetLevelInterface);
                        }
                    }
                    else
                    {
                        GUI_Manager.Instance.ShowWindowWithName("PlotUI", false);
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
            case PbCommon.ETaskTargetType.E_Target_Pass_Any_Level_Times:
                {
                    GUI_Manager.Instance.ShowWindowWithName("PlotUI", false);
                    break;
                }
        }
        DeSelect();
    }

    protected override void OnRecycle()
    {
        Task = null;
        TaskTemplate = null;
        RegistToGroup(null);
        UnRegisteEvent();
    }

    protected override void OnSelected()
    {
        TaskJumpButton.gameObject.SetActive(Task.TaskState == (uint)PbCommon.ETaskStateType.E_Task_State_Not_Finish);
        if(Task.TaskState == (uint)PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award)
        {
            gsproto.DrawTaskAwardReq req = new gsproto.DrawTaskAwardReq();
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            req.task_id = (uint)Task.CsvId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    protected override void OnDeSelected()
    {
        TaskJumpButton.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        UnRegisteEvent();
    }

    void RegistEvent()
    {
        DataCenter.PlayerDataCenter.OnDrawTaskAward += OnGetTaskAwardRsp;
        DataCenter.PlayerDataCenter.OnTaskDataChange += OnTaskDataChange;
    }

    void UnRegisteEvent()
    {
        DataCenter.PlayerDataCenter.OnDrawTaskAward -= OnGetTaskAwardRsp;
        DataCenter.PlayerDataCenter.OnTaskDataChange -= OnTaskDataChange;
    }

    void OnGetTaskAwardRsp(uint awardType, uint awardValue)
    {
        RefreshTaskInfo();
    }

    void OnTaskDataChange(int csvId)
    {
        RefreshTaskInfo();
    }
    #endregion
}
