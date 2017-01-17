using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_QuickExpeditionUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_QuickExpeditionUI dataComponent = gameObject.GetComponent<GUI_QuickExpeditionUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_QuickExpeditionUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            AreaInfo = dataComponent.AreaInfo;
            RemainTime = dataComponent.RemainTime;
            ExpeditionSchedule = dataComponent.ExpeditionSchedule;
            ConfirmButton = dataComponent.ConfirmButton;
            MapPieceCount = dataComponent.MapPieceCount;
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmQuickExpedition);
        }
    }
    #endregion

    #region window logic
    public Text AreaInfo;
    public Text RemainTime;
    public Slider ExpeditionSchedule;
    public Text MapPieceCount;
    public Button ConfirmButton;

    bool CountingMission = false;
    DataCenter.Expedition Expedition;
    CSV_b_expedition_quest_template MissionTemplate;


    public void TryQuickFinish(DataCenter.Expedition expeiditon, CSV_b_expedition_quest_template missionTemplate)
    {
        Expedition = expeiditon;
        MissionTemplate = missionTemplate;
        if(null == Expedition || null == MissionTemplate)
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        CSV_b_expedition_template areaTemplate = CSV_b_expedition_template.FindData(MissionTemplate.QuestGroup);
        if(null != areaTemplate)
        {
            AreaInfo.text = string.Format("[{0}]{1}", areaTemplate.Name, MissionTemplate.Name);
        }
        CheckExpeditionState();
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
        if (!CountingMission)
        {
            CountingMission = true;
            InvokeRepeating("RefreshExpeditionState", 0f, 1f);//update per minute
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
        if (null != Expedition && null != MissionTemplate)
        {
            if (Expedition.FinishTime > 0)
            {
                if (Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)//not finish
                {
                    ConfirmButton.interactable = true;
                    uint remainTime = Expedition.FinishTime - DataCenter.PlayerDataCenter.ServerTime;
                    RemainTime.text = TimeFormater.Format(remainTime);

                    ExpeditionSchedule.value = 1.0f - Mathf.Clamp01((float)remainTime / MissionTemplate.ExpeditionTime);

                    uint leftTime = Expedition.FinishTime - DataCenter.PlayerDataCenter.ServerTime;
                    uint leftMinutes = leftTime / (uint)ConstDefine.SECOND_PER_MINUTE;
                    uint totalMinutes = (uint)(MissionTemplate.ExpeditionTime / ConstDefine.SECOND_PER_MINUTE);
                    uint leftHours = leftTime / (uint)ConstDefine.SECOND_PER_HOUR;

                    if (leftHours > 0)
                    {
                        if (leftMinutes > 0)
                        {
                            ++leftHours;
                        }
                    }
                    else
                    {
                        uint leftSeconds = leftTime % (uint)ConstDefine.SECOND_PER_MINUTE;
                        if (leftSeconds > 0)
                        {
                            ++leftMinutes;
                        }
                        if (leftMinutes > 0)
                        {
                            ++leftHours;
                        }
                    }
                    int totalHour = (MissionTemplate.ExpeditionTime + 3599) / ConstDefine.SECOND_PER_HOUR;
                    float mapCount = Mathf.Ceil((float)leftHours * MissionTemplate.MapPieceCount / totalHour);
                    MapPieceCount.text = mapCount.ToString();
                }
                else//finished
                {
                    ConfirmButton.interactable = false;
                    MapPieceCount.text = "0";
                    ExpeditionSchedule.value = 1f;
                    RemainTime.text = "已完成";
                }
            }
            else//not recieve mission, should not happen
            {
                ConfirmButton.interactable = false;
                RemainTime.text = "未接受任务";
            }
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnEndExpedition += OnQuickExpeditionRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnEndExpedition -= OnQuickExpeditionRsp;
    }

    void OnConfirmQuickExpedition()
    {
        if(Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)
        {
            gsproto.EndExpeditionReq req = new gsproto.EndExpeditionReq();
            req.end_type = (uint)PbCommon.EEndExpeditionType.E_EndExpedition_Quick;
            req.expedition_quest_id = (uint)MissionTemplate.Id;
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    void OnQuickExpeditionRsp(DataCenter.GroupAddExpInfo groupAddExp, List<DataCenter.HeroAddExpInfo> heroAddExpList, List<DataCenter.AwardInfo> extraAwardList)
    {
        GUI_ExpeditionAwardUI_DL awardUI = GUI_Manager.Instance.ShowWindowWithName<GUI_ExpeditionAwardUI_DL>("UI_FinishExploration", false);
        if(null != awardUI)
        {
            awardUI.ShowAwardInfo(Expedition, MissionTemplate, groupAddExp, heroAddExpList, extraAwardList);
        }
        HideWindow();
    }
    #endregion
}
