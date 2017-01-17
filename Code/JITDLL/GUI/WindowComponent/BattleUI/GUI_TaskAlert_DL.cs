using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_TaskAlert_DL : MonoBehaviour
{
    #region alert logic
    public Text TaskType;
    public Text TaskName;
    public Text TaskState;
    public Text TaskDescription;
    public GUI_TweenPosition TweenIn = null;
    public GUI_TweenPosition TweenOut = null;


    void OnEnable()
    {
        DataCenter.PlayerDataCenter.TaskData.OnTaskTracker += OnTaskAlert;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.TaskData.OnTaskTracker -= OnTaskAlert;
    }

    void OnTaskAlert(DataCenter.Task task)
    {
        if(null != task)
        {
            CSV_b_task_template taskTemplate = CSV_b_task_template.FindData(task.CsvId);
            if(null != taskTemplate)
            {
                TaskType.text = taskTemplate.TypeName;
                TaskName.text = taskTemplate.Name;
                TaskState.gameObject.SetActive(task.TaskState == (uint)PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award);
                if(task.TaskSchedule < task.TaskTargetValue)
                {
                    TaskDescription.text = string.Format("{0}/{1} {2}", task.TaskSchedule.ToString(), task.TaskTargetValue.ToString(), taskTemplate.Description);
                }
                else
                {
                    string finishText;
                    if(TextLocalization.GetText(TextId.Finish, out finishText))
                    {
                        TaskDescription.text = string.Format("({0}) {1}", finishText, taskTemplate.Description);
                    }
                    else//should not happen
                    {
                        TaskDescription.text = string.Format("(Finish) {0}", taskTemplate.Description);
                    }
                }

                Reset();
                Play();
            }
        }
    }

    void Reset()
    {
        TweenIn.ResetToBeginning();
        TweenOut.ResetToBeginning();
    }

    void Play()
    {
        TweenIn.PlayForward();
        TweenOut.PlayForward();
    }
    #endregion

    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_TaskAlert dataComponent = gameObject.GetComponent<GUI_TaskAlert>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_TaskAlert,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            TaskType = dataComponent.TaskType;
            TaskName = dataComponent.TaskName;
            TaskState = dataComponent.TaskState;
            TaskDescription = dataComponent.TaskDescription;
            TweenIn = dataComponent.TweenIn;
            TweenOut = dataComponent.TweenOut;
        }
    }
    #endregion
}
