using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_BakeryItem_DL : GUI_LogicObject
{
    public Text Name;
    public Image IconImage;
    public Text Product;
    public Text TimeDesciption;
    public Text BakeScheduleCount;
    public Slider BakeSchedule;
    public Text BakeScheduleText;

    public GameObject LockMask;
    public GameObject BakeDone;
    public GameObject Baking;

    CSV_b_bakeries_template _BakeryTemplate;
    float _BakeTime;
    bool _UpdateBakingInfo;

    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnReceiveBread += OnGetProductRsp;
        DataCenter.PlayerDataCenter.OnFinishRoast += OnGetProductRsp;
        DataCenter.PlayerDataCenter.OnCancelRoast += OnAbortBakeRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnStartRoast -= OnStartBakeRsp;
        DataCenter.PlayerDataCenter.OnReceiveBread -= OnGetProductRsp;
        DataCenter.PlayerDataCenter.OnFinishRoast -= OnGetProductRsp;
        DataCenter.PlayerDataCenter.OnCancelRoast -= OnAbortBakeRsp;
    }


    public void ShowBakeryItem(CSV_b_bakeries_template bakeryItem)
    {
        if (null != bakeryItem)
        {
            _BakeTime = (float)bakeryItem.NeedTime * ConstDefine.SECOND_PER_MINUTE;
            _BakeryTemplate = bakeryItem;
            LockBakery(DataCenter.PlayerDataCenter.Level < bakeryItem.OpenLevel);
            GUI_Tools.IconTool.SetIcon(bakeryItem.IconAltas, bakeryItem.IconSprite, IconImage);

            if (bakeryItem.Id == (int)DataCenter.PlayerDataCenter.BakeriesType)
            {
                if (DataCenter.PlayerDataCenter.BakeriesFinishTime > DataCenter.PlayerDataCenter.ServerTime)
                {
                    ShowBakingInfo();
                }
                else
                {
                    BakeItemsDone();
                }
            }
            else
            {
                ClearBakeState();
            }
            Name.text = bakeryItem.Name;
            Product.text = bakeryItem.Product;
            TimeDesciption.text = bakeryItem.TimeDesciption;
        }
    }

    void ShowBakingInfo()
    {
        BakingItems();
        if (!_UpdateBakingInfo)
        {
            _UpdateBakingInfo = true;
            InvokeRepeating("UpdateBakeCount", 0f, 1f);
        }
    }

    void UpdateBakeCount()
    {
        uint remainCount = 0;
        if (DataCenter.PlayerDataCenter.BakeriesFinishTime > DataCenter.PlayerDataCenter.ServerTime)
        {
            remainCount = DataCenter.PlayerDataCenter.BakeriesFinishTime - DataCenter.PlayerDataCenter.ServerTime;
            BakeScheduleCount.text = TimeFormater.Format(remainCount);
            BakeSchedule.value = Mathf.Clamp01(1f - (float)remainCount / _BakeTime);
            int schedule = (int)(100 * BakeSchedule.value);
            BakeScheduleText.text = schedule + "/100";
        }
        else
        {
            BakeScheduleCount.text = TimeFormater.Format(0);
            BakeSchedule.value = 1f;
            BakeScheduleText.text = "100/100";
            BakeItemsDone();
            StopUpdateBakingInfo();
        }
    }

    void LockBakery(bool lockBakery)
    {
        LockMask.SetActive(lockBakery);
    }

    public void OnBakeryClick()
    {
        if (DataCenter.PlayerDataCenter.BakeriesType == 0)
        {
            StartBake();
        }
    }

    public void OnAbortButtonClicked()
    {
        GUI_AbortBakeUI_DL abortBakeUI = GUI_Manager.Instance.ShowWindowWithName<GUI_AbortBakeUI_DL>("GUI_AbortBakeUI", false);
        if (null != abortBakeUI)
        {
            abortBakeUI.TryAbort(_BakeryTemplate);
        }
    }

    public void OnFinishBakeImmediatButtonClicked()
    {
        GUI_FinishBakeImmediatUI_DL finishImmediate = GUI_Manager.Instance.ShowWindowWithName<GUI_FinishBakeImmediatUI_DL>("GUI_FinishBakeImmediatUI", false);
        if (null != finishImmediate)
        {
            finishImmediate.TryFinishImmediate(_BakeryTemplate);
        }
    }

    public void OnGetProductButtonClicked()
    {
        gsproto.ReceiveBreadReq req = new gsproto.ReceiveBreadReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnGetProductRsp(uint bakeryType, List<immortaldb.Bread> breadList)
    {
        if (bakeryType == _BakeryTemplate.Id)
        {
            ClearBakeState();
            GUI_BakeDoneUI_DL bakeDoneUI = GUI_Manager.Instance.ShowWindowWithName<GUI_BakeDoneUI_DL>("GUI_BakeDoneUI", false);
            bakeDoneUI.DisplayBreadList(breadList);
        }
    }

    void StartBake()
    {
        DataCenter.PlayerDataCenter.OnStartRoast -= OnStartBakeRsp;
        DataCenter.PlayerDataCenter.OnStartRoast += OnStartBakeRsp;
        //通知服务器
        gsproto.StartRoastReq startReq = new gsproto.StartRoastReq();
        startReq.bakeries_type = (uint)_BakeryTemplate.Id;
        startReq.session_id = DataCenter.PlayerDataCenter.SessionId;

        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, startReq);
    }

    void OnStartBakeRsp()
    {
        DataCenter.PlayerDataCenter.OnStartRoast -= OnStartBakeRsp;
        ShowBakingInfo();
    }

    void BakingItems()
    {
        if (!Baking.activeInHierarchy)
        {
            Baking.SetActive(true);
        }
        if (BakeDone.activeInHierarchy)
        {
            BakeDone.SetActive(false);
        }
    }

    void BakeItemsDone()
    {
        if (Baking.activeInHierarchy)
        {
            Baking.SetActive(false);
        }
        if (!BakeDone.activeInHierarchy)
        {
            BakeDone.SetActive(true);
        }
    }

    void StopUpdateBakingInfo()
    {
        if (_UpdateBakingInfo)
        {
            _UpdateBakingInfo = false;
            CancelInvoke("UpdateBakeCount");
        }
    }

    void ClearBakeState()
    {
        if (Baking.activeInHierarchy)
        {
            Baking.SetActive(false);
        }
        if (BakeDone.activeInHierarchy)
        {
            BakeDone.SetActive(false);
        }
        StopUpdateBakingInfo();
    }

    void OnAbortBakeRsp()
    {
        ClearBakeState();
    }

    protected override void OnRecycle()
    {

    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_BakeryItem dataComponent = gameObject.GetComponent<GUI_BakeryItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BakeryItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Name = dataComponent.Name;
            IconImage = dataComponent.IconImage;
            Product = dataComponent.Product;
            TimeDesciption = dataComponent.TimeDesciption;
            BakeScheduleCount = dataComponent.BakeScheduleCount;
            BakeSchedule = dataComponent.BakeSchedule;
            BakeScheduleText = dataComponent.BakeScheduleText;
            LockMask = dataComponent.LockMask;
            BakeDone = dataComponent.BakeDone;
            Baking = dataComponent.Baking;

            dataComponent.ItemButton.onClick.AddListener(OnBakeryClick);
            dataComponent.AbortButton.onClick.AddListener(OnAbortButtonClicked);
            dataComponent.FinishImmediateButton.onClick.AddListener(OnFinishBakeImmediatButtonClicked);
            dataComponent.GetBakeryProductButton.onClick.AddListener(OnGetProductButtonClicked);
        }
    }
}
