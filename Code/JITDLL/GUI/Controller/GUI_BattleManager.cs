using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_BattleManager : Singleton<GUI_BattleManager>
{
    List<int> _SelectLevelBossList;
    List<DataCenter.Hero> _SelectedHeroList;
    int _CaptainIndex;
    public bool UseDynamicLoadMode = false;//有完整的流程执行后删除
    public GUI_BattleUI_DL BattleUI;//有完整流程后动态生成
    public GUI_HpController_DL HpController;//有完整流程后动态生成
    public E_ChapterType ChapterType
    {
        get;
        protected set;
    }

    public CSV_c_game_chapter SelectedChapter
    {
        get;
        protected set;
    }

    public List<int> SelectedChapterSectionList
    {
        get;
        protected set;
    }

    public CSV_c_game_section SelectedSection
    {
        get;
        protected set;
    }

    public List<int> SelectedSectionLevelList
    {
        get;
        protected set;
    }

    public CSV_b_game_level SelectedLevel
    {
        get;
        protected set;
    }

    public string SelectedChapterUIName
    {
        get
        {
            switch ((E_ChapterType)SelectedChapter.ChapterType)
            {
                case E_ChapterType.Plot:
                    {
                        return "PlotUI";
                    }
            }
            return "PlotUI";
        }
    }

    public void SelectChapter(CSV_c_game_chapter chapter, List<int> sectionList)
    {
        SelectedChapter = chapter;
        SelectedChapterSectionList = sectionList;
    }

    public void SelectSection(CSV_c_game_section section, List<int> levelList)
    {
        SelectedSection = section;
        SelectedSectionLevelList = levelList;
    }

    public void SelectLevel(CSV_b_game_level curSelectLevel, List<int> bossList)
    {
        SelectedLevel = curSelectLevel;
        _SelectLevelBossList = bossList;
    }

    public void NextLevel()
    {
        int curLevelIndex = SelectedSectionLevelList.IndexOf(SelectedLevel.LevelId);
        if (curLevelIndex >= 0 && curLevelIndex < SelectedSectionLevelList.Count - 1)
        {

            SelectedLevel = CSV_b_game_level.FindData(SelectedSectionLevelList[curLevelIndex + 1]);
        }
        else
        {
            int curSectionIndex = SelectedChapterSectionList.IndexOf(SelectedSection.SectionID);
            if (curSectionIndex >= 0 && curSectionIndex < SelectedChapterSectionList.Count - 1)
            {
                SelectedSection = CSV_c_game_section.FindData(SelectedChapterSectionList[curSectionIndex + 1]);
                SelectedSectionLevelList = CSVDataFile.ExtractIntArrayFromString(SelectedSection.LevelList);
                SelectedLevel = CSV_b_game_level.FindData(SelectedSectionLevelList[0]);
            }
            else
            {
                CSV_c_game_chapter nextChapter = CSV_c_game_chapter.FindData(SelectedChapter.ChapterId + 1);
                if (null != nextChapter && nextChapter.ChapterType == SelectedChapter.ChapterType)
                {
                    SelectedChapter = nextChapter;
                    SelectedChapterSectionList = CSVDataFile.ExtractIntArrayFromString(nextChapter.SectionList);
                    SelectedSection = CSV_c_game_section.FindData(SelectedChapterSectionList[0]);
                    SelectedSectionLevelList = CSVDataFile.ExtractIntArrayFromString(SelectedSection.LevelList);
                    SelectedLevel = CSV_b_game_level.FindData(SelectedSectionLevelList[0]);
                }
            }
        }
    }

    public List<int> GetCurLevelBossList()
    {
        return _SelectLevelBossList;
    }

    public void SelectHero(List<DataCenter.Hero> heroList, int captainIndex)
    {
        _SelectedHeroList = heroList;
        _CaptainIndex = captainIndex;
    }

    public List<DataCenter.Hero> GetSelectHero()
    {
        return _SelectedHeroList;
    }

    public int GetCaptainIndex()
    {
        return _CaptainIndex;
    }

    public bool IsCaptain(int heroDisplayIndex)
    {
        return heroDisplayIndex == _CaptainIndex;
    }

    public void OnStageClear()
    {
        if (null != BattleUI)
        {
            BattleUI._SpecialWarning.Warning(ESpecialWarningType.PassLevel);
        }
    }

    public void OnGameOver()
    {
        if (null != BattleUI)
        {
            BattleUI._SpecialWarning.Warning(ESpecialWarningType.FailLevel);
        }
    }

    #region skill cast warn
    GUI_SkillCastWarn_DL _ComradeSkillCastWarn;
    GUI_SkillCastWarn_DL _EnemySkillCastWarn;

    public void RegistSkillCastWarn(SKILL.Camp camp, GUI_SkillCastWarn_DL warn)
    {
        if (camp == SKILL.Camp.Comrade)
        {
            _ComradeSkillCastWarn = warn;
        }
        else
        {
            _EnemySkillCastWarn = warn;
        }
    }

    public void UnRegistSkillCastWarn(SKILL.Camp camp)
    {
        if (camp == SKILL.Camp.Comrade)
        {
            _ComradeSkillCastWarn = null;
        }
        else
        {
            _EnemySkillCastWarn = null;
        }
    }

    public void SkillCastWarn(Actor actor, SKILL.Skill skill)
    {
        if (actor.SelfCamp == SKILL.Camp.Comrade)
        {
            if (null != _ComradeSkillCastWarn)
            {
                _ComradeSkillCastWarn.OnSkillCastStart(actor, skill);
            }
        }
        else
        {
            if (null != _EnemySkillCastWarn)
            {
                _EnemySkillCastWarn.OnSkillCastStart(actor, skill);
            }
        }
    }
    #endregion
}