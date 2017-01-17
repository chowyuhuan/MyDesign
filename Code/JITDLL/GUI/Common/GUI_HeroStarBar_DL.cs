using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(HorizontalLayoutGroup))]
public sealed class GUI_HeroStarBar_DL : MonoBehaviour
{
    public int StarWidth;
    public RectTransform StarArea;
    public List<GameObject> Stars;

    public void SetStarNum(int starNum)
    {
        int index = 0;
        for (; index < starNum && index < Stars.Count; ++index)
        {
            if (!Stars[index].activeSelf)
            {
                Stars[index].SetActive(true);
            }
        }
        for (; index < Stars.Count; ++index)
        {
            if (Stars[index].activeSelf)
            {
                Stars[index].SetActive(false);
            }
        }
        StarArea.sizeDelta = new Vector2(StarWidth * starNum, StarArea.sizeDelta.y);
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_HeroStarBar dataComponent = gameObject.GetComponent<GUI_HeroStarBar>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroStarBar,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            StarWidth = dataComponent.StarWidth;
            StarArea = dataComponent.StarArea;
            Stars = dataComponent.Stars;
        }
    }
}
