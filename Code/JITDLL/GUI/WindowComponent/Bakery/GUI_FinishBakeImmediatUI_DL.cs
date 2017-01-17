using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_FinishBakeImmediatUI_DL : GUI_Window_DL
{
    public Text Name;
    public Image IconImage;
    public Text Product;
    public Text BakeScheduleCount;
    public Slider BakeSchedule;
    public Text BakeScheduleText;
    public Text CostCount;
    CSV_b_bakeries_template _BakeryTemplate;
    float _BakeTime;


    public void TryFinishImmediate(CSV_b_bakeries_template bakeryTemplate)
    {
        _BakeTime = (float)bakeryTemplate.NeedTime * ConstDefine.SECOND_PER_MINUTE;
        _BakeryTemplate = bakeryTemplate;
        GUI_Tools.IconTool.SetIcon(bakeryTemplate.IconAltas, bakeryTemplate.IconSprite, IconImage);
        Name.text = bakeryTemplate.Name;
        Product.text = bakeryTemplate.Product;
        InvokeRepeating("UpdateBakeCount", 0f, 1f);
    }

    void UpdateBakeCount()
    {
        uint remainCount = 0;
        if (DataCenter.PlayerDataCenter.BakeriesFinishTime > DataCenter.PlayerDataCenter.ServerTime)
        {
            remainCount = DataCenter.PlayerDataCenter.BakeriesFinishTime - DataCenter.PlayerDataCenter.ServerTime;
            BakeSchedule.value = Mathf.Clamp01(1f - remainCount / _BakeTime);
            BakeScheduleCount.text = TimeFormater.Format(remainCount);
            int remainHours = (int)remainCount / ConstDefine.SECOND_PER_HOUR;
            int remainSeconds = (int)remainCount - remainHours * ConstDefine.SECOND_PER_HOUR;
            int costCount = (remainHours / _BakeryTemplate.FinishUnit * _BakeryTemplate.FinishCostPerUnit);
            if (remainHours % _BakeryTemplate.FinishUnit > 0
                || remainSeconds > 0)
            {
                ++costCount;
            }
            CostCount.text = costCount.ToString();
            int schedule = (int)(100 * BakeSchedule.value);
            BakeScheduleText.text = schedule + "/100";
        }
        else
        {
            BakeSchedule.value = 1f;
            BakeScheduleCount.text = TimeFormater.Format(0);
            CostCount.text = "0";
            BakeScheduleText.text = "100/100";
            CancelInvoke("UpdateBakeCount");
            HideWindow();
        }
    }

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnFinishRoast += OnFinishBake;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnFinishRoast -= OnFinishBake;
    }

    void OnFinishBake(uint bakeryType, List<immortaldb.Bread> breadList)
    {
        HideWindow();
    }

    public void OnFinishImmediatButtonClicked()
    {
        gsproto.FinishRoastReq req = new gsproto.FinishRoastReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_FinishBakeImmediatUI dataComponent = gameObject.GetComponent<GUI_FinishBakeImmediatUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_FinishBakeImmediatUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Name = dataComponent.Name;
            IconImage = dataComponent.IconImage;
            Product = dataComponent.Product;
            BakeScheduleCount = dataComponent.BakeScheduleCount;
            BakeSchedule = dataComponent.BakeSchedule;
            BakeScheduleText = dataComponent.BakeScheduleText;
            CostCount = dataComponent.CostCount;

            dataComponent.FinishImmediatButton.onClick.AddListener(OnFinishImmediatButtonClicked);
        }
    }
}