using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GUI_ExpeditionMissionItem_DL : GUI_ToggleItem_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ExpeditionMissionItem dataComponent = gameObject.GetComponent<GUI_ExpeditionMissionItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ExpeditionMissionItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ExpeditionStateText = dataComponent.ExpeditionStateText;
            ExpeditionIcon = dataComponent.ExpeditionIcon;
            MissionName = dataComponent.MissionName;
            ExpeditionHeroList = dataComponent.ExpeditionHeroList;
            ExpeditionScheduleText = dataComponent.ExpeditionScheduleText;
            ExpeditionScheduleValue = dataComponent.ExpeditionScheduleValue;
            FinishedTag = dataComponent.FinishedTag;
        }
    }
    #endregion

    #region item logic
    public Text ExpeditionStateText;
    public Image ExpeditionIcon;
    public Text MissionName;
    public List<GameObject> ExpeditionHeroList;
    public Text ExpeditionScheduleText;
    public Slider ExpeditionScheduleValue;
    public GameObject FinishedTag;

    public DataCenter.Expedition Expedition { get; protected set; }
    public CSV_b_expedition_quest_template ExpeditionMissionTemplate { get; protected set; }

    bool CountingMission = false;
    Action<GUI_ExpeditionMissionItem_DL> OnItemSelected;
    Action<GUI_ExpeditionMissionItem_DL> OnItemDeSelected;

    public void ShowExpeditionMission(DataCenter.Expedition expeditionInfo, Action<GUI_ExpeditionMissionItem_DL> onItemSelect, Action<GUI_ExpeditionMissionItem_DL> onItemDeselect)
    {
        CountingMission = false;
        Expedition = expeditionInfo;
        OnItemSelected = onItemSelect;
        OnItemDeSelected = onItemDeselect;
        if (null != Expedition)
        {
            ExpeditionMissionTemplate = Expedition.expeditionCSV;
        }
        InitExpeditionBaseInfo();
        RefreshExpeditionState();
    }

    void InitExpeditionBaseInfo()
    {
        if(null != ExpeditionMissionTemplate)
        {
            MissionName.text = ExpeditionMissionTemplate.Name;
            GUI_Tools.IconTool.SetIcon(ExpeditionMissionTemplate.QuestAtlas, ExpeditionMissionTemplate.QuestIcon, ExpeditionIcon);
            SetHeroCount(ExpeditionMissionTemplate.HeroCount);
        }
    }

    void SetHeroCount(int count)
    {
        if(null != ExpeditionHeroList)
        {
            for (int index = 0; index < ExpeditionHeroList.Count; ++index)
            {
                ExpeditionHeroList[index].SetActive(index < count);
            }
        }
    }

    void CheckExpeditionState()
    {
        StopCount();
        if (Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)//not finish
        {
            StartCount();
        }
    }

    void StartCount()
    {
        if(!CountingMission)
        {
            CountingMission = true;
            InvokeRepeating("RefreshExpeditionState", 0f, 60f);//update per minute
        }
    }

    void StopCount()
    {
        if (CountingMission)
        {
            CountingMission = false;
            CancelInvoke("RefreshExpeditionState");
        }
    }

    void RefreshExpeditionState()
    {
        if (null != Expedition && null != ExpeditionMissionTemplate)
        {
            if(Expedition.FinishTime > 0)
            {
                if (Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)//not finish
                {
                    uint leftTime = Expedition.FinishTime - DataCenter.PlayerDataCenter.ServerTime;
                    uint leftMinutes = leftTime / (uint)ConstDefine.SECOND_PER_MINUTE;
                    uint totalMinutes = (uint)(ExpeditionMissionTemplate.ExpeditionTime / ConstDefine.SECOND_PER_MINUTE);
                    uint leftHours = leftTime / (uint)ConstDefine.SECOND_PER_HOUR;
                    TextLocalization.SetTextById(ExpeditionStateText, TextId.Expedition_OnGoing);
                    ExpeditionScheduleValue.value = 1f - Mathf.Clamp01((float)leftMinutes / (float)totalMinutes);
                    if(leftHours > 0)
                    {
                        if(leftMinutes > 0)
                        {
                            ++leftHours;
                        }
                        GUI_Tools.TextTool.SetHour(ExpeditionScheduleText, (int)leftHours);
                    }
                    else 
                    {
                        uint leftSeconds = leftTime % (uint)ConstDefine.SECOND_PER_MINUTE;
                        if(leftSeconds > 0)
                        {
                            ++leftMinutes;
                        }
                        GUI_Tools.TextTool.SetMinute(ExpeditionScheduleText, (int)leftMinutes);
                    }
                    GUI_Tools.ObjectTool.ActiveObject(FinishedTag, false);
                }
                else//finished
                {
                    ExpeditionStateText.text = "";
                    TextLocalization.SetTextById(ExpeditionScheduleText, TextId.Already_Finish);
                    ExpeditionScheduleValue.value = 1f;
                    GUI_Tools.ObjectTool.ActiveObject(FinishedTag, true);
                }
            }
            else//not recieve mission
            {
                ExpeditionStateText.text = "";
                int totalHour = ExpeditionMissionTemplate.ExpeditionTime / ConstDefine.SECOND_PER_HOUR;
                GUI_Tools.TextTool.SetHour(ExpeditionScheduleText, totalHour);
                ExpeditionScheduleValue.value = 0f;
                GUI_Tools.ObjectTool.ActiveObject(FinishedTag, false);
            }
        }
    }

    protected override void OnSelected()
    {
        if(null != OnItemSelected)
        {
            OnItemSelected(this);
        }
    }

    protected override void OnDeSelected()
    {
        if(null != OnItemDeSelected)
        {
            OnItemDeSelected(this);
        }
    }

    protected override void OnRecycle()
    {
        Expedition = null;
        ExpeditionMissionTemplate = null;
        OnItemSelected = null;
        OnItemDeSelected = null;
        StopCount();
        RegistToGroup(null);
        if(IsSelect)
        {
            DeSelect();
        }
    }
    #endregion
}
