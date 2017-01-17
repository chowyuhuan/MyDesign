using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUI_GridLayoutGroupHelper_DL : GUI_GroupLayoutHelper_DL
{
    public GridLayoutGroup Grid;

    public int ItemCount
    {
        get { return _ScrollItems.Count; }
    }

    protected override void OnAwake()
    {
        Grid = LayoutComponent as GridLayoutGroup;
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_GridLayoutGroupHelper dataComponent = gameObject.GetComponent<GUI_GridLayoutGroupHelper>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GridLayoutGroupHelper,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
        }
    }

    public override GUI_ScrollItem LocateAtIndex(int index)
    {
        if (index >= 0 && index < _ScrollItems.Count)
        {
            int row = 0;
            int col = 0;
            int totalRow = 0;
            int totalCol = 0;
            switch (Grid.constraint)
            {
                case GridLayoutGroup.Constraint.FixedColumnCount:
                    {
                        row = GetRange(index, Grid.constraintCount, out col);
                        totalRow = GetRange(_ScrollItems.Count, Grid.constraintCount, out totalCol);
                        float itemHight = GetTargetHight(row, totalRow);
                        float totalhight = ContentRect.sizeDelta.y;
                        float viewRectHight = ViewRect.sizeDelta.y;
                        if (totalhight > viewRectHight)
                        {
                            float totalScrollHight = totalhight - viewRectHight;
                            float scrollHight = Mathf.Clamp(itemHight - viewRectHight, 0f, totalScrollHight);
                            ScrollViewRect.verticalNormalizedPosition = Mathf.Clamp01(1f - scrollHight / totalScrollHight);
                        }
                        break;
                    }
                case GridLayoutGroup.Constraint.FixedRowCount:
                    {
                        col = GetRange(index, Grid.constraintCount, out row);
                        totalCol = GetRange(_ScrollItems.Count, Grid.constraintCount, out totalRow);
                        float itemWidth = GetTargetWidth(col, totalCol);
                        float totalWidth = ContentRect.sizeDelta.x;
                        float viewRectWidth = ViewRect.sizeDelta.x;
                        if (itemWidth == viewRectWidth)
                        {
                            float totalScrollWidth = totalWidth - viewRectWidth;
                            float scrollWidth = Mathf.Clamp(itemWidth - viewRectWidth, 0f, totalScrollWidth);
                            ScrollViewRect.horizontalNormalizedPosition = Mathf.Clamp01(1f - scrollWidth / totalScrollWidth);
                        }
                        break;
                    }
            }
            return _ScrollItems[index];
        }
        return null;
    }

    float GetTargetHight(int row, int totalRowCount)
    {
        float targetHight = 0f;
        for(int index = totalRowCount - 1; index >=0 && index >= row ; --index)
        {
            targetHight += (Grid.cellSize.y + Grid.spacing.y);
        }
        if (targetHight > 0)
        {
            targetHight -= Grid.spacing.y;
        }
        return targetHight;
    }

    float GetTargetWidth(int col, int totalColCount)
    {
        float targetWidth = 0f;
        for (int index = totalColCount - 1; index >= 0 && index >= col; --index )
        {
            targetWidth += (Grid.cellSize.x + Grid.spacing.x);
        }
        if (targetWidth > 0)
        {
            targetWidth -= Grid.spacing.x;
        }
        return targetWidth;
    }


    int GetRange(int index, int rangeLimit, out int rangeValue)
    {
        int range = index / rangeLimit;
        rangeValue = index % rangeLimit;
        if (rangeValue > 0)
        {
            range++;
        }
        return range;
    }
}
