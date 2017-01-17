using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_AbortExpeditionUI_DL : GUI_Window_DL {
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_AbortExpeditionUI dataComponent = gameObject.GetComponent<GUI_AbortExpeditionUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_AbortExpeditionUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            AreaInfo = dataComponent.AreaInfo;
            RemainTime = dataComponent.RemainTime;
            ExpeditionSchedule = dataComponent.ExpeditionSchedule;
            ConfirmButton = dataComponent.ConfirmButton;
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmAbortExpedition);
        }
    }
    #endregion

    #region window logic
    public Text AreaInfo;
    public Text RemainTime;
    public Slider ExpeditionSchedule;
    public Button ConfirmButton;

    bool CountingMission = false;
    DataCenter.Expedition Expedition;
    CSV_b_expedition_quest_template MissionTemplate;


    public void TryAbortExpedition(DataCenter.Expedition expeiditon, CSV_b_expedition_quest_template missionTemplate)
    {
        Expedition = expeiditon;
        MissionTemplate = missionTemplate;
        if (null == Expedition || null == MissionTemplate)
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        CSV_b_expedition_template areaTemplate = CSV_b_expedition_template.FindData(MissionTemplate.QuestGroup);
        if (null != areaTemplate)
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
                    int remainHour = ((int)remainTime + 3599) / ConstDefine.SECOND_PER_HOUR;
                    int totalHour = (MissionTemplate.ExpeditionTime + 3599) / ConstDefine.SECOND_PER_HOUR;
                    float mapCount = Mathf.Ceil((float)remainHour * MissionTemplate.MapPieceCount / totalHour);
                    ExpeditionSchedule.value = 1.0f - Mathf.Clamp01((float)remainTime / MissionTemplate.ExpeditionTime);
                }
                else//finished
                {
                    ConfirmButton.interactable = false;
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
        DataCenter.PlayerDataCenter.OnCancelExpedition += OnAbortExpeditionRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnCancelExpedition -= OnAbortExpeditionRsp;
    }

    void OnConfirmAbortExpedition()
    {
        if (Expedition.FinishTime > DataCenter.PlayerDataCenter.ServerTime)
        {
            gsproto.CancelExpeditionReq req = new gsproto.CancelExpeditionReq();
            req.expedition_quest_id = (uint)MissionTemplate.Id;
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    void OnAbortExpeditionRsp(int csvId)
    {
        HideWindow();
    }
    #endregion
}
