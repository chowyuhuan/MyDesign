using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_GoddessHouseUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_GoddessHouseUI dataComponent = gameObject.GetComponent<GUI_GoddessHouseUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GoddessHouseUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            CurGoddessSkillName = dataComponent.CurGoddessSkillName;
            CurGoddessSkillDescription = dataComponent.CurGoddessSkillDescription;
            GoddessButtonText = dataComponent.GoddessButtonText;
            GoddessPageObject = dataComponent.GoddessPageObject;
            GoddessButton = dataComponent.GoddessButton;
            GoddessGroup = dataComponent.GoddessGroup;
            dataComponent.GoddessButton.onClick.AddListener(OnGoddessButtonClicked);
        }
    }
    #endregion

    #region window logic
    protected override void OnStart()
    {
        InitGoddessPage();
        RefreshGoddessPage();
        DisplayGoddess();

        List<DataCenter.Goddess> goddessList = DataCenter.PlayerDataCenter.GoddessData.GetGoddessList();
        if(null != goddessList && goddessList.Count > 0)
        {
            CurrentGoddess = goddessList[0];
        }
    }

    public void VisiteGoddess()
    {
        VisitingGoddess = true;
    }

    public void InviteGoddess(DataCenter.TeamInfo teamInfo)
    {
        VisitingGoddess = false;
        CurrentTeam = teamInfo;
    }
    #endregion

    #region goddess page
    public Text CurGoddessSkillName;
    public Text CurGoddessSkillDescription;
    public Text GoddessButtonText;
    public GameObject GoddessPageObject;
    public Button GoddessButton;
    public ToggleGroup GoddessGroup;

    GUI_GridLayoutGroupHelper_DL GoddessPageHelper;
    GUI_LogicObjectPool GoddessItemPool;
    DataCenter.Goddess CurrentGoddess;
    CSV_c_goddess_config CurrentGoddessConfig;
    bool VisitingGoddess = false;
    DataCenter.TeamInfo CurrentTeam;

    void InitGoddessPage()
    {
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/Goddess_GoddessList_Item", true, AssetManage.E_AssetType.UIPrefab);
#if UNITY_EDITOR
        Debug.Assert(null != go);
#endif
        GoddessItemPool = new GUI_LogicObjectPool(go);
        GoddessPageHelper = GoddessPageObject.GetComponent<GUI_GridLayoutGroupHelper_DL>();
        GoddessPageHelper.SetScrollAction(DisplayGoddessItem);
    }

    void DisplayGoddess()
    {
        //显示女神
    }

    void RefreshGoddessPage()
    {
        if(null != GoddessPageHelper)
        {
            List<DataCenter.Goddess> goddessList = DataCenter.PlayerDataCenter.GoddessData.GetGoddessList();
            if (null != goddessList)
            {
                for (int index = 0; index < goddessList.Count; ++index)
                {
                    GoddessPageHelper.FillItem(goddessList[index].csvId);
                }
            }

            GoddessPageHelper.FillItemEnd();
        }
    }

    void DisplayGoddessItem(GUI_ScrollItem scrollItem)
    {
        if(null != scrollItem)
        {
            DataCenter.Goddess goddess = DataCenter.PlayerDataCenter.GoddessData.GetGoddess(scrollItem.LogicIndex);
            if(null != goddess)
            {
                GUI_GoddessHouseItem_DL goddessItem = GoddessItemPool.GetOneLogicComponent() as GUI_GoddessHouseItem_DL;
                if(null != goddessItem)
                {
                    goddessItem.DisplayGoddess(goddess, OnGoddessItemSelect, OnGoddessItemDeselect, VisitingGoddess);
                    scrollItem.SetTarget(goddessItem);
                    goddessItem.RegistToGroup(GoddessGroup);
                    if (null != CurrentGoddess && CurrentGoddess.csvId == goddess.csvId)
                    {
                        goddessItem.Select();
                    }
                }
            }
        }
    }

    void OnGoddessItemSelect(GUI_GoddessHouseItem_DL goddessItem)
    {
        if(null != goddessItem)
        {
            CurrentGoddess = goddessItem.TargetGoddess;
            CurrentGoddessConfig = goddessItem.GoddessConfig;
            RefreshGoddessButton(goddessItem.TargetGoddess);
            RefreshGoddessSkillInfo(goddessItem.GoddessConfig);
            DisplayGoddessSkill(goddessItem.GoddessConfig);
        }
    }

    void OnGoddessItemDeselect(GUI_GoddessHouseItem_DL goddessItem)
    {

    }

    void RefreshGoddessButton(DataCenter.Goddess goddess)
    {
        if(null != goddess)
        {
            bool inTeam = (goddess.LockChapter == 0);//没有离队
            if(VisitingGoddess)
            {
                GoddessButton.interactable = true;
                TextLocalization.SetTextById(GoddessButtonText, inTeam ? "Dialog" : "Seprating");
            }
            else//选择战斗应该根据当前章节是否是女神离队的章节进行处理
            {
                bool inCompany = (goddess.csvId == (int)CurrentTeam.Goddess);
                GoddessButton.interactable = !inCompany;
                TextLocalization.SetTextById(GoddessButtonText, inCompany ? "InCompany" : "InviteToInCompany");
            }
        }
    }

    void RefreshGoddessSkillInfo(CSV_c_goddess_config goddessConfig)
    {
        if(null != goddessConfig)
        {
            CSV_c_skill_description skillDes = CSV_c_skill_description.FindData(goddessConfig.GoddessSkillId);
            if(null != skillDes)
            {
                GUI_Tools.TextTool.SetText(CurGoddessSkillName, skillDes.Name);
                GUI_Tools.TextTool.SetText(CurGoddessSkillDescription, skillDes.Description);
            }
        }
    }

    void OnGoddessButtonClicked()
    {
        if(null != CurrentGoddess)
        {
            bool inTeam = (CurrentGoddess.LockChapter == 0);
            if (inTeam)
            {
                if (VisitingGoddess)
                {
                    GUI_MessageManager.Instance.ShowErrorTip(CurrentGoddessConfig.GoddessName);//对话
                }
                else
                {
                    bool inCompany = (CurrentGoddess.csvId == (int)CurrentTeam.Goddess);
                    if (!inCompany)
                    {
                        CurrentTeam.Goddess = (uint)CurrentGoddess.csvId;
                        GUI_MessageManager.Instance.ShowErrorTip(CurrentGoddessConfig.GoddessName + "同行中。。。");//对话
                    }
                }
            }
            else
            {
                GUI_GoddessLeaveUI_DL goddessLeave = GUI_Manager.Instance.ShowWindowWithName<GUI_GoddessLeaveUI_DL>("UI_Goddess_Leave", false);
                if (null != goddessLeave)
                {
                    goddessLeave.GoddessLeave(CurrentGoddess, CurrentGoddessConfig);
                }
            }
        }
    }

    void DisplayGoddessSkill(CSV_c_goddess_config goddessConfig)
    {
        //展示女神技能
    }
    #endregion
}
