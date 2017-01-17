using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_RefuseTaskUI_DL : GUI_Window_DL
{
    #region window logic
    uint RefuseTaskId;
    void OnEnable()
    {
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange += OnRefuseTaskRsp;
    }

    void OnDisable()
    {
        DataCenter.PlayerDataCenter.OnNormalTaskDataChange -= OnRefuseTaskRsp;
    }

    public void TryRefuseTask(uint taskId)
    {
        RefuseTaskId = taskId;
    }

    void OnConfirmRefuseButtonClicked()
    {
        gsproto.RefuseTaskReq req = new gsproto.RefuseTaskReq();
        req.session_id = DataCenter.PlayerDataCenter.SessionId;
        req.task_id = RefuseTaskId;
        Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
    }

    void OnRefuseTaskRsp(uint position)
    {
        GUI_MessageManager.Instance.ShowErrorTip("拒绝任务成功");
        HideWindow();
    }
    #endregion

    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_RefuseTaskUI dataComponent = gameObject.GetComponent<GUI_RefuseTaskUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_RefuseTaskUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            dataComponent.ConfirmRefuseButton.onClick.AddListener(OnConfirmRefuseButtonClicked);
        }
    }
    #endregion
}
