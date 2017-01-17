using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class GUI_LevelBonus_DL : GUI_Window_DL
{
    List<GameObject> HeroInfoObjectList;
    public List<GUI_HeroBonusSimpleInfo_DL> HeroInfoList = new List<GUI_HeroBonusSimpleInfo_DL>();
    public Text LevelName;
    public GUI_Transform HeroTransTemplate;
    public string HeroAction;

    public Text GoldCoin;
    int _GoldCoinCount;
    public float HeroExpSimulateRate = 0.02f;
    public GameObject RegimentLevelUpRoot;
    public Text RegimentGroupLevel;
    public Slider RegimentGroupExp;
    List<GameObject> BonusItemObjectList;
    public List<GUI_LevelBonusItem_DL> BonusItemList = new List<GUI_LevelBonusItem_DL>();
    bool _OpenBonusChestStart = false;
    public Text TaskSchedule;
    public Text TaskFinish;

    protected override void OnStart()
    {
        TaskFinish.gameObject.SetActive(false);
        TaskSchedule.gameObject.SetActive(false);

        for (int index = 0; index < BonusItemObjectList.Count; ++index )
        {
            BonusItemList.Add(BonusItemObjectList[index].GetComponent<GUI_LevelBonusItem_DL>());
        }
        for (int index = 0; index < HeroInfoObjectList.Count; ++index)
        {
            HeroInfoList.Add(HeroInfoObjectList[index].GetComponent<GUI_HeroBonusSimpleInfo_DL>());
        }
        GoldCoin.text = DataCenter.PlayerDataCenter.OverLevelData.Coin.ToString();
        LevelName.text = GUI_BattleManager.Instance.SelectedLevel.LevelName;
        SetHeroInfo();
        StartCoroutine("RegimentExpGrowUp");
        DisPlayBonusChest();
        PlayBonusAction();
        CheckGoddessInOut();
    }

    void CheckGoddessInOut()
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
                        break;
                    }
                case 1://离队
                    {
                        GUI_GoddessLeaveUI_DL goddessLeave = GUI_Manager.Instance.ShowWindowWithName<GUI_GoddessLeaveUI_DL>("UI_Goddess_Leave", false);
                        if (null != goddessLeave)
                        {
                            goddessLeave.AlertGoddessLeave(inOutInfo.CsvId);
                        }
                        break;
                    }
            }
        }
    }

    IEnumerator RegimentExpGrowUp()
    {
        RegimentLevelUp(false);
        DataCenter.GroupAddExpInfo regimentExpInfo = DataCenter.PlayerDataCenter.OverLevelData.GroupAddExpData;
        int curExp = (int)regimentExpInfo.StartExp;
        int endExp = (int)regimentExpInfo.EndExp;
        int totalGrowExp = endExp - curExp;
        uint curLevel = regimentExpInfo.StartLevel;
        uint endLevel = regimentExpInfo.EndLevel;
        int curLevelGrowExp;
        int curLevelExp;
        CSV_b_hero_group_template curHeroGroupTemplate = CSV_b_hero_group_template.FindData((int)curLevel);
        while (curExp < endExp)
        {
            int growStep = (int)(totalGrowExp * HeroExpSimulateRate);
            curExp += growStep;
            if (curExp > endExp)
            {
                curExp = endExp;
            }
            if (curExp > curHeroGroupTemplate.Exp)
            {
                ++curLevel;
                RegimentLevelUp(true);
                if (curLevel > endLevel)
                {
                    curLevel = endLevel;
                }
            }
            curLevelGrowExp = CSV_b_hero_group_template.GetLevelGrowUpExp(curLevel);
            curLevelExp = CSV_b_hero_group_template.GetCurLevelExp(curExp, curLevel);
            RegimentGroupLevel.text = curLevel.ToString();
            RegimentGroupExp.value = Mathf.Clamp01((float)curLevelExp / (float)curLevelGrowExp);
            yield return null;
        }

        curLevelGrowExp = CSV_b_hero_group_template.GetLevelGrowUpExp(endLevel);
        curLevelExp = CSV_b_hero_group_template.GetCurLevelExp(endExp, endLevel);
        RegimentGroupLevel.text = endLevel.ToString();
        RegimentGroupExp.value = Mathf.Clamp01((float)curLevelExp / (float)curLevelGrowExp);

        CheckTaskState();
    }

    void CheckTaskState()
    {
        PbCommon.ETaskStateType taskState = DataCenter.PlayerDataCenter.TaskData.GetAllTaskAwardState();
        if(taskState == PbCommon.ETaskStateType.E_Task_State_Not_Draw_Award)
        {
            ActiveAwardState(true);
            ActiveTaskSchedule(false);
        }
        else
        {
            ActiveAwardState(false);

            List<DataCenter.Task> normalTask = DataCenter.PlayerDataCenter.TaskData.GetNormalTaskList();
            if(null != normalTask)
            {
                bool activeSchedule = false;
                for (int index = 0; index < normalTask.Count; ++index)
                {
                    CSV_b_task_template taskTemplate = CSV_b_task_template.FindData(normalTask[index].CsvId);
                    if(taskTemplate.TargetLevelInterface == GUI_BattleManager.Instance.SelectedLevel.LevelId)
                    {
                        activeSchedule = true;
                        TaskSchedule.text = string.Format("任务{0}/{1}", normalTask[index].TaskSchedule.ToString(), normalTask[index].TaskTargetValue.ToString());
                        break;
                    }
                }
                ActiveTaskSchedule(activeSchedule);
            }
        }
    }

    void ActiveAwardState(bool active)
    {
        if(active && !TaskFinish.gameObject.activeInHierarchy)
        {
            TaskFinish.gameObject.SetActive(true);
        }
        else if (!active && TaskFinish.gameObject.activeInHierarchy)
        {
            TaskFinish.gameObject.SetActive(false);
        }
    }

    void ActiveTaskSchedule(bool active)
    {
        if (active && !TaskSchedule.gameObject.activeInHierarchy)
        {
            TaskSchedule.gameObject.SetActive(true);
        }
        else if (!active && TaskSchedule.gameObject.activeInHierarchy)
        {
            TaskSchedule.gameObject.SetActive(false);
        }
    }

    void RegimentLevelUp(bool levelUp)
    {
        RegimentLevelUpRoot.SetActive(levelUp);
    }

    void SetHeroInfo()
    {
        for (int index = 0; index < DataCenter.PlayerDataCenter.OverLevelData.HeroAddExpList.Count && index < HeroInfoList.Count; ++index)
        {
            HeroInfoList[index].SetHeroInfo(DataCenter.PlayerDataCenter.OverLevelData.HeroAddExpList[index], HeroTransTemplate, HeroAction, HeroExpSimulateRate);
            HeroInfoList[index].SimulateExpUp(OpenBonusChest);
        }
    }

    void DisPlayBonusChest()
    {
        DataCenter.PassOverInfo overInfo = DataCenter.PlayerDataCenter.OverLevelData;
        int index = 0;
        for (; index < overInfo.DropChestList.Count; ++index)
        {
            BonusItemList[index].SetItem(overInfo.DropChestList[index]);
        }
        for (; index < BonusItemList.Count; ++index)
        {
            BonusItemList[index].SetItem(null);
        }
    }

    void OpenBonusChest()
    {
        if (!_OpenBonusChestStart)
        {
            _OpenBonusChestStart = true;
            for (int index = 0; index < BonusItemList.Count; ++index)
            {
                BonusItemList[index].CheckChestOpenOperation();
            }
        }
    }

    protected abstract void PlayBonusAction();

    public virtual void OnReTryButtonClicked()
    {
        GUI_Manager.Instance.HideAllWindow();
        PL_Manager_DL.Instance.LoadSceneAsync("MainCity", true);
        GUI_Manager.Instance.PushWndToWaitingQueue("MainUI");
        GUI_Manager.Instance.PushWndToWaitingQueue(GUI_BattleManager.Instance.SelectedChapterUIName);
        GUI_Manager.Instance.PushWndToWaitingQueue("ChapterDetailUI");
    }

    public virtual void OnNextLevelButtonClicked()
    {
        GUI_Manager.Instance.HideAllWindow();
        PL_Manager_DL.Instance.LoadSceneAsync("MainCity", true);
        GUI_BattleManager.Instance.NextLevel();
        GUI_Manager.Instance.PushWndToWaitingQueue("MainUI");
        GUI_Manager.Instance.PushWndToWaitingQueue(GUI_BattleManager.Instance.SelectedChapterUIName);
        GUI_Manager.Instance.PushWndToWaitingQueue("ChapterDetailUI");
    }

    public virtual void OnBackTownBtnClicked()
    {
        GUI_Manager.Instance.HideAllWindow();
        PL_Manager_DL.Instance.LoadSceneAsync("MainCity", true);
        GUI_Manager.Instance.PushWndToWaitingQueue("MainUI");
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_LevelBonus dataComponent = gameObject.GetComponent<GUI_LevelBonus>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_LevelBonus,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HeroInfoObjectList = dataComponent.HeroInfoList;
            LevelName = dataComponent.LevelName;
            HeroTransTemplate = dataComponent.HeroTransTemplate;
            HeroAction = dataComponent.HeroAction;

            GoldCoin = dataComponent.GoldCoin;
            HeroExpSimulateRate = dataComponent.HeroExpSimulateRate;
            RegimentLevelUpRoot = dataComponent.RegimentLevelUpRoot;
            RegimentGroupLevel = dataComponent.RegimentGroupLevel;
            RegimentGroupExp = dataComponent.RegimentGroupExp;
            BonusItemObjectList = dataComponent.BonusItemList;

            TaskSchedule = dataComponent.TaskSchedule;
            TaskFinish = dataComponent.TaskFinish;

            dataComponent.CloseButton.onClick.RemoveAllListeners();
            dataComponent.CloseButton.onClick.AddListener(OnBackTownBtnClicked);
        }
    }
}
