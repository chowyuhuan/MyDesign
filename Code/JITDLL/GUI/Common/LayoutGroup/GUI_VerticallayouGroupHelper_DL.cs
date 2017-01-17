using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_VerticallayouGroupHelper_DL : GUI_GroupLayoutHelper_DL
{
    #region layout logic
    VerticalLayoutGroup VerticalLayout;
    public override GUI_ScrollItem LocateAtIndex(int index)
    {
        if (index >= 0 && index < _ScrollItems.Count)
        {
            GUI_ScrollItem si = _ScrollItems[index];
            if (null != si)
            {
                float totalHeight = ContentRect.sizeDelta.y;
                float itemPos = GetLayoutHight(index);
                float viewRectHeight = ViewRect.sizeDelta.y;
                if(totalHeight > viewRectHeight)
                {
                    float totalScrollHeight = totalHeight - viewRectHeight;
                    float scrollHeight = Mathf.Clamp(itemPos - viewRectHeight, 0f, totalScrollHeight);
                    ScrollViewRect.verticalNormalizedPosition = Mathf.Clamp01(scrollHeight / totalScrollHeight);
                }
            }
            return si;
        }
        return null;
    }

    protected override void OnFillItemEnd()
    {
        RefreshLayout();
    }


    float GetLayoutHight(int targetIndex)
    {
        float height = 0f;
        for (int index = ItemCount - 1; index >= 0 && index >= targetIndex; --index)
        {
            height += (_ScrollItems[index].ItemLayout.preferredHeight + VerticalLayout.spacing);
        }
        if (height > 0)
        {
            height -= VerticalLayout.spacing;
        }
        return height;
    }

    public override void RefreshLayout()
    {
        if (null != VerticalLayout)
        {
            float height = GetLayoutHight(0);
            if (height < ViewRect.sizeDelta.y)
            {
                height = ViewRect.sizeDelta.y;
            }
            float oldHeight = ContentRect.sizeDelta.y;
            ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x, height);
            ContentRect.anchoredPosition = new Vector2(ContentRect.anchoredPosition.x, ContentRect.anchoredPosition.y - (ContentRect.sizeDelta.y - oldHeight) / 2);
        }
    }
    #endregion

    #region jit init
    protected override  void OnAwake()
    {
        VerticalLayout = LayoutComponent as VerticalLayoutGroup;
    }


    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_VerticallayouGroupHelper dataComponent = gameObject.GetComponent<GUI_VerticallayouGroupHelper>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_VerticallayouGroupHelper,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
        }
    }
    #endregion
}
