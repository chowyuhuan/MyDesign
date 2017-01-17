using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_LevelItem_DL : GUI_ToggleItem_DL
{
    public Text SectionInfo;
    public Color SectionSelectedColor;
    public Text LevelName;
    public Color LevelSelectedColor;
    public Text Difficulty;
    public Color DifficultySelectedColor;
    public Text CostCount;
    public Color CostSelectedColor;
    public Image CostIcon;
    public Image SelectionIcon;
    public GameObject TaskTag;
    public GameObject TaskFinishTag;

    CSV_b_game_level _LevelInfo;

    public void SetLevelItem(CSV_b_game_level gl)
    {
        _LevelInfo = gl;
        FlipSelectedColor(false);
        CheckTaskScedule();
    }

    void CheckTaskScedule()
    {
        if(null != _LevelInfo)
        {
            List<DataCenter.Task> normalTask = DataCenter.PlayerDataCenter.TaskData.GetNormalTaskList();
            if(null != normalTask)
            {
                PbCommon.ETaskStateType taskState = PbCommon.ETaskStateType.E_Task_State_Not_Receive;
                for(int index = 0; index < normalTask.Count; ++index)
                {
                    CSV_b_task_template taskTemplate = CSV_b_task_template.FindData(normalTask[index].CsvId);                    
                    if(null != taskTemplate)
                    {
                        if(taskTemplate.TargetLevelInterface == _LevelInfo.LevelId)
                        {
                            taskState = (PbCommon.ETaskStateType)normalTask[index].TaskState;
                            if(normalTask[index].TaskState == (int)PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award)//完成未领奖的标记优先未完成标记
                            {
                                break;
                            }
                        }                        
                    }
                }
                SetTaskTag(taskState);
            }
        }
    }

    void SetTaskTag(PbCommon.ETaskStateType taskState)
    {
        switch(taskState)
        {
            case PbCommon.ETaskStateType.E_Task_State_Not_Finish:
                {
                    GUI_Tools.ObjectTool.ActiveObject(TaskTag, true);
                    GUI_Tools.ObjectTool.ActiveObject(TaskFinishTag, false);
                    break;
                }
            case PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award:
                {
                    GUI_Tools.ObjectTool.ActiveObject(TaskTag, false);
                    GUI_Tools.ObjectTool.ActiveObject(TaskFinishTag, true);
                    break;
                }
            default:
                {
                    GUI_Tools.ObjectTool.ActiveObject(TaskTag, false);
                    GUI_Tools.ObjectTool.ActiveObject(TaskFinishTag, false);
                    break;
                }
        }
    }

    void FlipSelectedColor(bool selected)
    {
        if (selected)
        {
            SectionInfo.text = GUI_Tools.RichTextTool.Color(SectionSelectedColor, _LevelInfo.LevelPrefix.ToString());
            LevelName.text = GUI_Tools.RichTextTool.Color(LevelSelectedColor, _LevelInfo.LevelName);
            CostCount.text = GUI_Tools.RichTextTool.Color(CostSelectedColor, _LevelInfo.CostCount.ToString());
            Difficulty.text = GUI_Tools.RichTextTool.Color(DifficultySelectedColor, GUI_Tools.TextTool.GetDifficultText((E_GameLevel_Difficulty)_LevelInfo.Difficulty));
        }
        else
        {
            SectionInfo.text = _LevelInfo.LevelPrefix.ToString();
            LevelName.text = _LevelInfo.LevelName;
            CostCount.text = _LevelInfo.CostCount.ToString();
            Difficulty.text = GUI_Tools.TextTool.GetDifficultText((E_GameLevel_Difficulty)_LevelInfo.Difficulty);
        }
    }

    protected override void OnSelected()
    {
        FlipSelectedColor(true);
        List<int> monsters = CSVDataFile.ExtractIntArrayFromString(_LevelInfo.MonsterWaveList);
        CSV_b_monster_wave mw = CSV_b_monster_wave.FindData(monsters[monsters.Count - 1]);
        List<int> bossList = new List<int>();
        if (mw.monsterCount > 0)
        {
            bossList.Add(mw.monsterId1);
        }
        if (mw.monsterCount > 1)
        {
            bossList.Add(mw.monsterId2);
        }
        if (mw.monsterCount > 2)
        {
            bossList.Add(mw.monsterId3);
        }
        GUI_ChapterDetailUI_DL cdui = GUI_Manager.Instance.FindWindowWithName<GUI_ChapterDetailUI_DL>("ChapterDetailUI", false);
        cdui.ShowBossInfo(bossList);
        GUI_BattleManager.Instance.SelectLevel(_LevelInfo, bossList);
    }

    protected override void OnDeSelected()
    {
        FlipSelectedColor(false);
    }

    protected override void OnRecycle()
    {
        RegistToGroup(null);
        DeSelect();
        _LevelInfo = null;
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_LevelItem dataComponent = gameObject.GetComponent<GUI_LevelItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_LevelItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SectionInfo = dataComponent.SectionInfo;
            LevelName = dataComponent.LevelName;
            Difficulty = dataComponent.Difficulty;
            CostCount = dataComponent.CostCount;
            CostIcon = dataComponent.CostIcon;
            SelectionIcon = dataComponent.SelectionIcon;
            SectionSelectedColor = dataComponent.SectionSelectedColor;
            LevelSelectedColor = dataComponent.LevelSelectedColor;
            DifficultySelectedColor = dataComponent.DifficultySelectedColor;
            CostSelectedColor = dataComponent.CostSelectedColor;
            TaskTag = dataComponent.TaskTag;
            TaskFinishTag = dataComponent.TaskFinishTag;
        }
    }
}
