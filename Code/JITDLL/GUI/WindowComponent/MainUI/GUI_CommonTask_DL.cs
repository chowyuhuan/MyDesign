using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_CommonTask_DL : MonoBehaviour
{
    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_CommonTask dataComponent = gameObject.GetComponent<GUI_CommonTask>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_CommonTask,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            TaskUIInfo = dataComponent.TaskUIInfo;
            TaskUIInfo.TaskButton.onClick.AddListener(OnTaskButtonClicked);
        }
    }
    #endregion

    #region task logic
    public GUI_CommonTaskInfo TaskUIInfo;
    DataCenter.Task Task;
    bool Counting = false;
    int MaxNormalTaskCount = 3;
    uint TaskColdTime;
    void Start()
    {
        int coldMin = DefaultConfig.GetInt("TaskPosCoolTime");
        TaskColdTime = (uint)coldMin * ConstDefine.SECOND_PER_MINUTE;
        RefreshTaskInfo();
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange += OnTaskDataChange;
        DataCenter.PlayerDataCenter.OnGroupExpChange += OnGroupExpChange;
        DataCenter.PlayerDataCenter.OnDrawTaskAward += OnGetAwardRsp;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange += OnRefuseTaskRsp;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange += OnAcceptTaskRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange -= OnTaskDataChange;
        DataCenter.PlayerDataCenter.OnGroupExpChange -= OnGroupExpChange;
        DataCenter.PlayerDataCenter.OnDrawTaskAward -= OnGetAwardRsp;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange -= OnRefuseTaskRsp;
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange -= OnAcceptTaskRsp;
    }

    void OnTaskDataChange(uint position)
    {
        if(TaskUIInfo.TaskIndex == (int)position)
        {
            RefreshTaskInfo();
        }
    }

    void OnGroupExpChange(uint oldExp, uint newExp, uint oldLevel, uint newLevel)
    {
        RefreshTaskInfo();
    }

    public void RefreshTaskInfo()
    {
        if(TaskSystemOpen())
        {
            ActiveTask(true);
            if(TaskUIInfo.TaskIndex <= MaxNormalTaskCount)
            {
                GUI_Tools.ObjectTool.ActiveObject(TaskUIInfo.ScheduleText.gameObject, true);
                RefreshNormalTask();
            }
            else
            {
                GUI_Tools.ObjectTool.ActiveObject(TaskUIInfo.ScheduleText.gameObject, false);
                RefreshDailyTask();
            }
        }
        else
        {
            ActiveTask(false);
        }
    }

    void RefreshDailyTask()
    {
        PbCommon.ETaskStateType dailyTaskState = DataCenter.PlayerDataCenter.TaskData.GetDailyTaskAwardState();
        PbCommon.ETaskStateType weeklyTaskState = DataCenter.PlayerDataCenter.TaskData.GetWeeklyTaskAwardState();
        if(PbCommon.ETaskStateType.E_Task_State_Not_Receive == dailyTaskState
            || PbCommon.ETaskStateType.E_Task_State_Not_Receive == weeklyTaskState)
        {
            TaskUIInfo.StateText.text = "<color=yellow>?</color>";
        }
        else if(PbCommon.ETaskStateType.E_Task_State_Not_Finish == dailyTaskState
            || PbCommon.ETaskStateType.E_Task_State_Not_Finish == weeklyTaskState)
        {
            TaskUIInfo.StateText.text = "<color=yellow>!</color>";
        }
        else
        {
            TaskUIInfo.StateText.text = "";
        }
    }

    void RefreshNormalTask()
    {
        if (Counting)
        {
            StopNormalTaskCount();
        }
        else
        {
            TaskUIInfo.ColdMask.gameObject.SetActive(false);
        }
        Task = DataCenter.PlayerDataCenter.TaskData.GetNormalTaskByPosition(TaskUIInfo.TaskIndex);
        if (null != Task)
        {
            if (Task.CountdownFinishTime > DataCenter.PlayerDataCenter.ServerTime)
            {
                InvokeRepeating("NormalTaskCountDown", 0f, 1f);
            }
            else
            {
                SetNormalTaskInfo();
            }
            RefreshNormalTaskState();
        }
        else
        {
            ActiveTask(false);
        }
    }

    void ActiveTask(bool active)
    {
        if(active && !gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        else if (!active && gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    bool TaskSystemOpen()
    {
        int openLevel = DefaultConfig.GetInt("TaskOpenLevel");
        return DataCenter.PlayerDataCenter.Level >= openLevel;
    }

    void SetNormalTaskInfo()
    {
        if (null != Task)
        {
            CSV_b_task_template taskTamplate = CSV_b_task_template.FindData(Task.CsvId);
            TaskUIInfo.ScheduleText.text = string.Format("{0}/{1}", Task.TaskSchedule.ToString(), taskTamplate.TargetValue.ToString());
            GUI_Tools.IconTool.SetIcon(taskTamplate.IconAtlas, taskTamplate.IconSprite, TaskUIInfo.TaskIcon);
        }
    }

    void NormalTaskCountDown()
    {
        if(null != Task)
        {
            Counting = true;
            if(Task.CountdownFinishTime > DataCenter.PlayerDataCenter.ServerTime)
            {
                uint leftColdTime = Task.CountdownFinishTime - DataCenter.PlayerDataCenter.ServerTime;
                TaskUIInfo.ColdTime.text = TimeFormater.FormatShrinkIfNeeded(leftColdTime);
                if (!TaskUIInfo.ColdMask.gameObject.activeInHierarchy)
                {
                    TaskUIInfo.ColdMask.gameObject.SetActive(true);
                }
                TaskUIInfo.ColdMask.fillAmount = Mathf.Clamp01((float)leftColdTime / TaskColdTime);
            }
            else
            {
                StopNormalTaskCount();
            }
        }
        else
        {
            StopNormalTaskCount();
        }
    }

    void StopNormalTaskCount()
    {
        CancelInvoke("NormalTaskCountDown");
        Counting = false;
        TaskUIInfo.ColdTime.text = "";
        if (TaskUIInfo.ColdMask.gameObject.activeInHierarchy)
        {
            TaskUIInfo.ColdMask.gameObject.SetActive(false);
        }
    }

    void RefreshNormalTaskState()
    {
        if(null != Task)
        {
            switch ((PbCommon.ETaskStateType)Task.TaskState)
            {
                case PbCommon.ETaskStateType.E_Task_State_Not_Receive:
                    {
                        TaskUIInfo.StateText.text = "<color=yellow>!</color>";
                        TaskUIInfo.ColdTime.text = "";
                        break;
                    }
                case PbCommon.ETaskStateType.E_Task_State_Not_Finish:
                    {
                        TaskUIInfo.StateText.text = "<color=grey>?</color>";
                        TaskUIInfo.ColdTime.text = "";
                        break;
                    }
                case PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award:
                    {
                        TaskUIInfo.StateText.text = "<color=yellow>?</color>";
                        TaskUIInfo.ColdTime.text = "完成";
                        break;
                    }
                default:
                    {
                        TaskUIInfo.StateText.text = "";
                        TaskUIInfo.ColdTime.text = "";
                        break;
                    }
            }
        }
    }

    void OnTaskButtonClicked()
    {
        if (TaskUIInfo.TaskIndex <= MaxNormalTaskCount)
        {
            if (Counting)
            {
                GUI_MessageManager.Instance.ShowErrorTip("无法接受任务");
                return;
            }
            if (null != Task)
            {
                switch ((PbCommon.ETaskStateType)Task.TaskState)
                {
                    case PbCommon.ETaskStateType.E_Task_State_Not_Receive:
                    case PbCommon.ETaskStateType.E_Task_State_Not_Finish:
                        {
                            GUI_MessageManager.Instance.ShowErrorTip("接受或拒绝");
                            GUI_CommonTaskDisplayUI_DL comDis = GUI_Manager.Instance.ShowWindowWithName<GUI_CommonTaskDisplayUI_DL>("UI_Mission", false);
                            if(null != comDis)
                            {
                                comDis.DisplayTask(Task);
                            }
                            break;
                        }
                    case PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award:
                        {
                            TryGetAward();
                            break;
                        }
                }
            }
            else
            {
                GUI_MessageManager.Instance.ShowErrorTip("没有任务");
            }
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip("日常任务");
            GUI_Manager.Instance.ShowWindowWithName("UI_MissionList", false);
        }
    }

    void TryGetAward()
    {
        if(null != Task)
        {
            GUI_MessageManager.Instance.ShowErrorTip("展示奖励");
            gsproto.DrawTaskAwardReq req = new gsproto.DrawTaskAwardReq();
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            req.task_id = (uint)Task.CsvId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    void OnGetAwardRsp(uint awardType, uint awardValue)
    {
        RefreshTaskInfo();
    }

    void OnRefuseTaskRsp(uint position)
    {
        if(position == (uint)TaskUIInfo.TaskIndex)
        {
            RefreshTaskInfo();
        }
    }

    void OnAcceptTaskRsp(uint position)
    {
        if (position == (uint)TaskUIInfo.TaskIndex)
        {
            RefreshTaskInfo();
        }
    }
    #endregion
}
