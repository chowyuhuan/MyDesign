using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public sealed class GUI_GoddessHouseItem_DL : GUI_ToggleItem_DL {
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_GoddessHouseItem dataComponent = gameObject.GetComponent<GUI_GoddessHouseItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GoddessHouseItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GoddessName = dataComponent.GoddessName;
            GoddessIcon = dataComponent.GoddessIcon;
            GoddessLeaveTag = dataComponent.GoddessLeaveTag;
        }
    }
    #endregion

    #region item logic
    public Text GoddessName;
    public Image GoddessIcon;
    public GameObject GoddessLeaveTag;
    public DataCenter.Goddess TargetGoddess { get; protected set; }
    public CSV_c_goddess_config GoddessConfig { get; protected set; }
    Action<GUI_GoddessHouseItem_DL> OnGoddessItemSelect;
    Action<GUI_GoddessHouseItem_DL> OnGoddessItemDeselect;

    public void DisplayGoddess(DataCenter.Goddess goddess, Action<GUI_GoddessHouseItem_DL> onItemSelect, Action<GUI_GoddessHouseItem_DL> onItemDeselect, bool visitingGoddess)
    {
        TargetGoddess = goddess;
        OnGoddessItemSelect = onItemSelect;
        OnGoddessItemDeselect = onItemDeselect;

        if(null != TargetGoddess)
        {
            GoddessConfig = CSV_c_goddess_config.FindData(TargetGoddess.csvId);
        }

        RefreshGoddessInfo(visitingGoddess);
    }

    void RefreshGoddessInfo(bool visitingGoddess)
    {
        if(null != GoddessConfig)
        {
            GUI_Tools.TextTool.SetText(GoddessName, GoddessConfig.GoddessName);
            GUI_Tools.IconTool.SetIcon(GoddessConfig.HeadIconAtlas, GoddessConfig.HeadIcon, GoddessIcon);
        }
        if(null != TargetGoddess)
        {
            bool inTeam = (TargetGoddess.LockChapter == 0);//没有离队
            if(visitingGoddess)
            {
                GUI_Tools.ObjectTool.ActiveObject(GoddessLeaveTag, !inTeam);
            }
            else
            {
                if(!inTeam)
                {
                    inTeam = (TargetGoddess.LockChapter > GUI_BattleManager.Instance.SelectedChapter.ChapterId);
                }
                GUI_Tools.ObjectTool.ActiveObject(GoddessLeaveTag, !inTeam);
            }
        }
    }

    protected override void OnSelected()
    {
        if(null != OnGoddessItemSelect)
        {
            OnGoddessItemSelect(this);
        }
    }

    protected override void OnDeSelected()
    {
        if(null != OnGoddessItemDeselect)
        {
            OnGoddessItemDeselect(this);
        }
    }

    protected override void OnRecycle()
    {
        TargetGoddess = null;
        OnGoddessItemSelect = null;
        OnGoddessItemDeselect = null;
        GoddessConfig = null;
        RegistToGroup(null);
    }
    #endregion
}
