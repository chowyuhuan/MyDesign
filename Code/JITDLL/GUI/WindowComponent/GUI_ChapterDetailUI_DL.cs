using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ChapterDetailUI_DL : GUI_Window_DL
{
    #region section display tab page
    GameObject GridLayoutHelperObject;
    public GUI_GridLayoutGroupHelper_DL GridLayoutHelper;
    List<GameObject> TabPageObjectList;
    public List<GUI_ToggleTabPage_DL> TabPageList;
    int _CurPageIndex = -1;
    List<CSV_c_game_section> _DisplaySectionList = new List<CSV_c_game_section>();
    GUI_LogicObjectPool _LevelItemPool;

    public void ShowTargetLevel(int levelId)
    {
        CSV_b_game_level targetLevel = CSV_b_game_level.FindData(levelId);
        if(null != targetLevel)
        {
            for(int index = 0; index < CSV_c_game_chapter.DateCount; ++index)
            {
                List<int> sectionList = CSVDataFile.ExtractIntArrayFromString(CSV_c_game_chapter.AllData[index].SectionList);
                for (int sectionIndex = 0; sectionIndex < sectionList.Count; ++sectionIndex)
                {
                    CSV_c_game_section gameSection = CSV_c_game_section.FindData(sectionList[sectionIndex]);
                    if(null != gameSection)
                    {
                        List<int> levelList = CSVDataFile.ExtractIntArrayFromString(gameSection.LevelList);
                        if(levelList.Contains(levelId))
                        {
                            GUI_BattleManager.Instance.SelectChapter(CSV_c_game_chapter.AllData[index], sectionList);
                            GUI_BattleManager.Instance.SelectSection(gameSection, levelList);

                            List<int> monsters = CSVDataFile.ExtractIntArrayFromString(targetLevel.MonsterWaveList);
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

                            GUI_BattleManager.Instance.SelectLevel(targetLevel, bossList);
                        }
                    }
                }
            }
        }
    }

    protected override void OnStart()
    {
        GridLayoutHelper = GridLayoutHelperObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        GridLayoutHelper.SetScrollAction(DisplayItem);
        TabPageList = new List<GUI_ToggleTabPage_DL>();
        for (int index = 0; index < TabPageObjectList.Count; ++index)
        {
            TabPageList.Add(TabPageObjectList[index].GetComponent<GUI_ToggleTabPage_DL>());
        }
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/LevelItem", true, AssetManage.E_AssetType.UIPrefab);
        _LevelItemPool = new GUI_LogicObjectPool(go);
        ShowTabPage();
    }

    void ShowTabPage()
    {
        List<int> sectionList = GUI_BattleManager.Instance.SelectedChapterSectionList;

        _DisplaySectionList.Clear();
        int index = 0;
        int selectedSectionIndex = 0;
        for (; index < sectionList.Count && index < TabPageList.Count; ++index)
        {
            CSV_c_game_section gs = CSV_c_game_section.FindData(sectionList[index]);
            TabPageList[index].Init(index, GridLayoutHelper, OnPageSelect, gs.SectionName, false);
            _DisplaySectionList.Add(gs);
        }
        if (sectionList.Count > 0)
        {
            TabPageList[selectedSectionIndex].Select();
        }
        for (; index < TabPageList.Count; ++index)
        {
            TabPageList[index].DisableToggle();
        }
    }

    void OnPageSelect(int index)
    {
        if (index == _CurPageIndex)
        {
            return;
        }
        else
        {
            _CurPageIndex = index;
            ShowPageData(_DisplaySectionList[index], TabPageList[index]);
        }
    }

    void ShowPageData(CSV_c_game_section gameSection, GUI_ToggleTabPage_DL tabPage)
    {
        List<int> levelList = CSVDataFile.ExtractIntArrayFromString(gameSection.LevelList);
        if (GUI_BattleManager.Instance.SelectedSection == null
            || GUI_BattleManager.Instance.SelectedSection.SectionID != gameSection.SectionID)
        {
            if (levelList.Count > 0)
            {
                CSV_b_game_level clt = CSV_b_game_level.FindData(levelList[0]);
                GUI_BattleManager.Instance.SelectLevel(clt, null);
            }
        }
        GUI_BattleManager.Instance.SelectSection(gameSection, levelList);
        GridLayoutHelper.FillPage(levelList.Count);
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            CSV_b_game_level level = CSV_b_game_level.FindData(GUI_BattleManager.Instance.SelectedSectionLevelList[scrollItem.LogicIndex]);
            GUI_LevelItem_DL li = _LevelItemPool.GetOneLogicComponent() as GUI_LevelItem_DL;
            li.DeSelect();
            li.SetLevelItem(level);
            scrollItem.SetTarget(li);
            TabPageList[_CurPageIndex].ShowItem(li);
            if (null != GUI_BattleManager.Instance.SelectedLevel
                && GUI_BattleManager.Instance.SelectedLevel.LevelId == level.LevelId)
            {
                li.Select();
            }
        }
    }
    #endregion

    #region boss info
    List<GameObject> BossInfoObjectList;
    public List<GUI_BossInfo_DL> BossInfoList = new List<GUI_BossInfo_DL>();

    void InitBossInfoList()
    {
        if(BossInfoList.Count == 0)
        {
            for(int index = 0; index < BossInfoObjectList.Count; ++index)
            {
                BossInfoList.Add(BossInfoObjectList[index].GetComponent<GUI_BossInfo_DL>());
            }
        }
    }

    public void ShowBossInfo(List<int> bossList)
    {
        InitBossInfoList();
        int index = 0;
        for (; index < bossList.Count && index < BossInfoList.Count; ++index)
        {
            BossInfoList[index].SetBossInfo(bossList[index]);
            BossInfoList[index].gameObject.SetActive(true);
        }
        for (; index < BossInfoList.Count; ++index)
        {
            BossInfoList[index].gameObject.SetActive(false);
        }
    }
    #endregion

    public void OnEnterButtonClicked()
    {
        GUI_Manager.Instance.ShowWindowWithName("PrepareBattleUI", false);
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ChapterDetailUI dataComponent = gameObject.GetComponent<GUI_ChapterDetailUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ChapterDetailUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GridLayoutHelperObject = dataComponent.GridLayoutHelperObject;
            TabPageObjectList = dataComponent.TabPageObjectList;
            BossInfoObjectList = dataComponent.BossInfoList;
            dataComponent.BeginButton.onClick.AddListener(OnEnterButtonClicked);
        }
    }
}
