using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ErrorTip_DL : GUI_BaseMessageUI_DL
{
    #region tip ui
    public Text Content;
    string _ErrorInfo = "";
    public GUI_TweenAlpha TweenAlpha = null;
    public GUI_TweenScale TweenScale = null;
    public GUI_TweenPosition TweenPostion = null;
    protected override void OnAwake()
    {
        this.MessageType = EMessageType.MESSAGE_TYPE_ERROR;
    }
    protected override void OnStart()
    {
        Content.text = _ErrorInfo;
    }
    public override void ShowMessage(MessageArg info)
    {
        ErrorTipArg tip = info as ErrorTipArg;
        if (tip == null)
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
        _ErrorInfo = info;
        if (Content != null)
        {
            Content.text = info;
        }
    }

    void ResetTipPanel()
    {
        _ErrorInfo = "";
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
        GUI_ErrorTip dataComponent = gameObject.GetComponent<GUI_ErrorTip>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ErrorTip,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Content = dataComponent.Content;
            TweenAlpha = dataComponent.TweenAlpha;
            TweenScale = dataComponent.TweenScale;
            TweenPostion = dataComponent.TweenPostion;
        }
    }
    #endregion
}