using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Message manager.
/// Don't use MessageBox UI show message directly,
/// use MessageManager.Instance.ShowMessage() instade.
/// Todo: use message type to re-layout MessageBox ui button.
/// </summary>
public sealed class GUI_MessageBox_DL : GUI_BaseMessageUI_DL
{

    #region Message Panel
    public Text Title;
    public Text Message;
    public Text ConfirmLabel;
    public Text CancelLabel;
    public Image ConfirmIcon;
    public Image CancelIcon;
    private MessageBoxArg _MessageArg = null;

    Vector3 origConfirmPos;
    Vector3 origCancelPos;

    public void Confirm()
    {
        if (_MessageArg == null)
        {
            return;
        }
        if (_MessageArg.OnConfirm != null)
        {
            _MessageArg.OnConfirm();
        }
        ShowNextMessage();
    }

    public void Cancel()
    {
        if (_MessageArg == null)
        {
            return;
        }
        if (_MessageArg.OnCancel != null)
        {
            _MessageArg.OnCancel();
        }
        ShowNextMessage();
    }
    public override void ShowMessage(MessageArg message)
    {
        if (message == null)
        {
            HideWindow();
        }

        _MessageArg = message as MessageBoxArg;

        if (Title != null && _MessageArg.UseMsgBtnAndTitle)
        {
            //Todo: 策划暂定所有标题一样，若有不一样需求时，此行注回
            Title.text = _MessageArg.Title;			
        }
        if (Message != null)
        {
            Message.text = message.MessageContent;
        }
        if (ConfirmLabel != null && _MessageArg.UseMsgBtnAndTitle)
        {
            ConfirmLabel.text = _MessageArg.ButtonConfirm;
        }
        if (CancelLabel != null && _MessageArg.UseMsgBtnAndTitle)
        {
            CancelLabel.text = _MessageArg.ButtonCancel;
        }
        AdjustButton(_MessageArg.MessageType);
    }

    void AdjustButton(MessageBoxType type)
    {
        switch (type)
        {
            case MessageBoxType.ConfirmAndConcell:
                {
                    //ConfirmIcon.sprite = "lvseanniu";//策划对消息框按钮有需求后再做处理
                    ConfirmIcon.transform.localPosition = origConfirmPos;
                    CancelIcon.transform.localPosition = origCancelPos;
                    CancelIcon.gameObject.SetActive(true);
                    break;
                }
            case MessageBoxType.RetryAndCancell:
                {
                    //ConfirmIcon.spriteName = "juseanniu";//策划对消息框按钮有需求后再做处理
                    ConfirmIcon.transform.localPosition = origConfirmPos;
                    CancelIcon.transform.localPosition = origCancelPos;
                    CancelIcon.gameObject.SetActive(true);
                    break;
                }
            case MessageBoxType.Confirm:
            case MessageBoxType.One:
                {
                    ConfirmIcon.transform.localPosition = new Vector3(0, origConfirmPos.y, origConfirmPos.z);
                    CancelIcon.gameObject.SetActive(false);
                    break;
                }
            default:
                {
                    Message.transform.localPosition = new Vector3(0, 0, 0);
                    ConfirmIcon.gameObject.SetActive(false);
                    CancelIcon.gameObject.SetActive(false);
                    break;
                }
        }
    }

    // Use this for initialization
    void InitMessage()
    {
        this.MessageType = EMessageType.MESSAGE_TYPE_CONFIRM_AND_CANCEL;
        origConfirmPos = ConfirmIcon.transform.localPosition;
        origCancelPos = CancelIcon.transform.localPosition;
    }
    void OnButtonCloseClicked(GameObject go)
    {
        HideWindow();
    }

    protected override void OnAwake()
    {
        InitMessage();
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_MessageBox dataComponent = gameObject.GetComponent<GUI_MessageBox>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_MessageBox,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Title = dataComponent.Title;
            Message = dataComponent.Message;
            ConfirmLabel = dataComponent.ConfirmLabel;
            CancelLabel = dataComponent.CancelLabel;
            ConfirmIcon = dataComponent.ConfirmIcon;
            CancelIcon = dataComponent.CancelIcon;

            dataComponent.ConfirmButton.onClick.AddListener(Confirm);
            dataComponent.CancelButton.onClick.AddListener(Cancel);
        }
    }
    #endregion
}