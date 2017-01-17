using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum E_ChapterRestriction
{
    Easy,
    Normal,
    Difficult,
    Time,
}

public class GUI_ChapterItem_DL : GUI_LogicObject
{
    public Text ChapterName;
    public Text ChaperDesciption;
    public Image ChapterContentIcon;
    public Text Restriction;
    int _ChapterId;
    CSV_c_game_chapter _ChapterInfo;
    List<int> _SectionList;

    public void SetChapterItem(CSV_c_game_chapter chapter)
    {
        ChapterName.text = chapter.ChapterName;
        ChaperDesciption.text = chapter.Description;
        _ChapterId = chapter.ChapterId;
        _ChapterInfo = chapter;
        _SectionList = CSVDataFile.ExtractIntArrayFromString(chapter.SectionList);
        SetRestriction((E_ChapterRestriction)chapter.Restriction);
        GUI_Tools.IconTool.SetIcon(chapter.DisplayAtlas, chapter.DisplayIcon, ChapterContentIcon);
    }

    /// <summary>
    /// Todo：放入本地化中，测试先这么用吧
    /// </summary>
    /// <param name="res"></param>
    void SetRestriction(E_ChapterRestriction res)
    {
        switch (res)
        {
            case E_ChapterRestriction.Easy:
                {
                    Restriction.text = "简单";
                    break;
                }
            case E_ChapterRestriction.Normal:
                {
                    Restriction.text = "普通";
                    break;
                }
            case E_ChapterRestriction.Difficult:
                {
                    Restriction.text = "困难";
                    break;
                }
            case E_ChapterRestriction.Time:
                {
                    Restriction.text = _ChapterInfo.RestrictionTime.ToString();
                    break;
                }
        }
    }

    public void OnEnterButtonClicked()
    {
        GUI_BattleManager.Instance.SelectChapter(_ChapterInfo, _SectionList);
        GUI_ChapterDetailUI_DL chapterDetail = GUI_Manager.Instance.ShowWindowWithName<GUI_ChapterDetailUI_DL>("ChapterDetailUI", false);
    }

    protected override void OnRecycle()
    {

    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_ChapterItem dataComponent = gameObject.GetComponent<GUI_ChapterItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ChapterItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ChapterName = dataComponent.ChapterName;
            ChaperDesciption = dataComponent.ChaperDesciption;
            ChapterContentIcon = dataComponent.ChapterContentIcon;
            Restriction = dataComponent.Restriction;
            dataComponent.EnterButton.onClick.AddListener(OnEnterButtonClicked);
        }
    }
}
