using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GUI_ExpeditionAreaButtonItem_DL : GUI_ToggleItem_DL {
    #region jit init
    protected override void OnAwake()
    {
        base.Init(null);
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ExpeditionAreaButtonItem dataComponent = gameObject.GetComponent<GUI_ExpeditionAreaButtonItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExpeditionAreaButtonItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ButtonIcon = dataComponent.ButtonIcon;
        }
    }
    #endregion

    #region item logic
    public Image ButtonIcon;
    public CSV_b_expedition_quest_template MissionTemplate { get; protected set; }

    Action<GUI_ExpeditionAreaButtonItem_DL> OnAreaSelect;
    Action<GUI_ExpeditionAreaButtonItem_DL> OnAreaDeselect;

    public void InitAreaButton(Action<GUI_ExpeditionAreaButtonItem_DL> onAreaSelect, Action<GUI_ExpeditionAreaButtonItem_DL> onAreaDeselect)
    {
        OnAreaSelect = onAreaSelect;
        OnAreaDeselect = onAreaDeselect;

        MissionTemplate = CSV_b_expedition_quest_template.FindData(ToggleIndex);
    }

    public void ShowButton(bool show)
    {
        if (show && null != MissionTemplate)
        {
            GUI_Tools.ObjectTool.ActiveObject(CachedGameObject, show);
            GUI_Tools.IconTool.SetIcon(MissionTemplate.AreaAtlas, MissionTemplate.AreaIcon, ButtonIcon);
        }
        else
        {
            GUI_Tools.ObjectTool.ActiveObject(CachedGameObject, false);
        }
    }

    protected override void OnSelected()
    {
        if(null != OnAreaSelect)
        {
            OnAreaSelect(this);
        }
    }

    protected override void OnDeSelected()
    {
        if(null != OnAreaDeselect)
        {
            OnAreaDeselect(this);
        }
    }

    protected override void OnRecycle()
    {
        OnAreaSelect = null;
        OnAreaDeselect = null;
    }
    #endregion
}
