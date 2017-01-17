using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_MessageTip_DL : GUI_BaseMessageUI_DL
{
    #region tip ui
    GUIText Content = null;
    string Message = "";
    GUI_TweenAlpha TweenAlpha = null;
    GUI_TweenScale TweenScale = null;
    GUI_TweenPosition TweenPostion = null;
    protected override void OnAwake()
    {
        this.MessageType = EMessageType.MESSAGE_TYPE_COMMON;
    }
    protected override void OnStart()
    {
        Content.text = Message;
    }
    public override void ShowMessage(MessageArg info)
    {
        if (info == null)
        {
            ShowTip(null);
        }
        else
        {
            ShowTip(info.MessageContent);
        }
    }
    public void ShowTip(string info)
    {
        if (string.IsNullOrEmpty(info))
        {
            this.HideWindow();
            return;
        }
        ResetTipPanel();
        InitShowData(info);
        BeginShow();
    }

    void BeginShow()
    {
        PlayShowAction();
        //bShowCount = true;
    }

    void InitShowData(string info)
    {
        Message = info;
        if (Content != null)
        {
            Content.text = info;
        }
    }

    void ResetTipPanel()
    {
        Message = "";
        if (Content != null)
        {
            Content.text = "";
        }
        if (TweenAlpha != null)
        {
            TweenAlpha.ResetToBeginning();
        }
        if (TweenScale != null)
        {
            TweenScale.ResetToBeginning();
        }
        if (TweenPostion != null)
        {
            TweenPostion.ResetToBeginning();
        }
    }

    void PlayShowAction()
    {
        if (TweenScale != null)
        {
            TweenScale.PlayForward(PlayFadingAction);
        }
    }

    void PlayFadingAction()
    {
        if (TweenPostion != null)
        {
            TweenPostion.PlayForward(ShowNextMessage);
        }
        if (TweenAlpha != null)
        {
            TweenAlpha.PlayForward();
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_MessageTip dataComponent = gameObject.GetComponent<GUI_MessageTip>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_MessageTip,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
        }
    }
    #endregion
}