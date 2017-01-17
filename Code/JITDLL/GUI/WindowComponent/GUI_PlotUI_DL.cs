using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_PlotUI_DL : GUI_Window_DL
{
    GameObject GridObject;
    public GUI_GridLayoutGroupHelper_DL Grid;
    GUI_LogicObjectPool _ChapterItemPool;
    protected override void OnStart()
    {
        Grid = GridObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        Grid.SetScrollAction(DisplayItem);
        GameObject chapterProto = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/ChapterTemplete", true, AssetManage.E_AssetType.UIPrefab);
        _ChapterItemPool = new GUI_LogicObjectPool(chapterProto);
        int plotType = (int)E_ChapterType.Plot;
        CSV_c_game_chapter chapter;
        for (int index = 0; index < CSV_c_game_chapter.DateCount; ++index)
        {
            chapter = CSV_c_game_chapter.GetData(index);
            if (chapter.ChapterType == plotType)
            {
                Grid.FillItem(chapter.ChapterId);
            }
        }

        Grid.FillItemEnd();
    }

    public void DisplayItem(GUI_ScrollItem scrollItem)
    {
        if (null != scrollItem)
        {
            CSV_c_game_chapter chapter = CSV_c_game_chapter.FindData(scrollItem.LogicIndex);
            GUI_ChapterItem_DL chapterItem = _ChapterItemPool.GetOneLogicComponent() as GUI_ChapterItem_DL;
            chapterItem.SetChapterItem(chapter);
            scrollItem.SetTarget(chapterItem);
        }
    }
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_PlotUI dataComponent = gameObject.GetComponent<GUI_PlotUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_PlotUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GridObject = dataComponent.Grid;
        }
    }
}
