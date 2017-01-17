using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_DailyTaskUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_DailyTaskUI dataComponent = gameObject.GetComponent<GUI_DailyTaskUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_DailyTaskUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            TaskScrollPageObject = dataComponent.TaskScrollPageObject;
            TaskTabPageObjectList = dataComponent.TaskTabPageObjectList;
            TaskToggleGroup = dataComponent.TaskToggleGroup;
        }
    }
    #endregion

    #region task page logic
    public List<GameObject> TaskTabPageObjectList;
    public GameObject TaskScrollPageObject;
    public ToggleGroup TaskToggleGroup;

    List<GUI_ToggleTabPage_DL> TaskTabPageList;
    GUI_VerticallayouGroupHelper_DL ScrollPageHelper;
    int CurrentPageIndex = -1;
    GUI_LogicObjectPool TaskItemPool;

    protected override void OnStart()
    {
        InitTabPageList();
        if(TaskTabPageList.Count > 0)
        {
            TaskTabPageList[0].Select();
        }
    }

    void InitTabPageList()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/MissionList_Item", true, AssetManage.E_AssetType.UIPrefab);
        TaskItemPool = new GUI_LogicObjectPool(go);

        ScrollPageHelper = TaskScrollPageObject.GetComponent<GUI_VerticallayouGroupHelper_DL>();
        ScrollPageHelper.SetScrollAction(ShowTaskItem);

        TaskTabPageList = new List<GUI_ToggleTabPage_DL>();
        for(int index = 0; index < TaskTabPageObjectList.Count; ++index)
        {
            GUI_ToggleTabPage_DL tabPage = TaskTabPageObjectList[index].GetComponent<GUI_ToggleTabPage_DL>();
            if(null != tabPage)
            {
                TaskTabPageList.Add(tabPage);
            }
        }

        for(int index = 0; index < TaskTabPageList.Count; ++index)
        {
            TaskTabPageList[index].Init(index, ScrollPageHelper, OnTaskPageSelect, null);
        }
    }

    void OnTaskPageSelect(int pageIndex)
    {
        if(pageIndex == CurrentPageIndex)
        {
            return;
        }
        CurrentPageIndex = pageIndex;
        List<DataCenter.Task> taskList = null;
        if(pageIndex == 0)
        {
            taskList = DataCenter.PlayerDataCenter.TaskData.GetDailyTaskList();
        }
        else if(pageIndex == 1)
        {
            taskList = DataCenter.PlayerDataCenter.TaskData.GetWeeklyTaskList();
        }
        else
        {
            GUI_MessageManager.Instance.ShowErrorTip(10001);
        }
        ShowTaskList(taskList);
    }

    void ShowTaskList(List<DataCenter.Task> taskList)
    {
        if (null != taskList)
        {
            for (int index = 0; index < taskList.Count; ++index)
            {
                ScrollPageHelper.FillItem(taskList[index].CsvId);
            }
            ScrollPageHelper.FillItemEnd();
        }
    }

    void ShowTaskItem(GUI_ScrollItem scrollItem)
    {
        if(null != scrollItem)
        {
            GUI_DailyTaskItem_DL taskItem = TaskItemPool.GetOneLogicComponent() as GUI_DailyTaskItem_DL;
            DataCenter.Task task = DataCenter.PlayerDataCenter.TaskData.GetTask(scrollItem.LogicIndex);
            if(null != task && null != taskItem)
            {
                taskItem.ShowTaskItem(task);
                scrollItem.SetTarget(taskItem);
                taskItem.RegistToGroup(TaskToggleGroup);
            }
        }
    }
    #endregion
}
