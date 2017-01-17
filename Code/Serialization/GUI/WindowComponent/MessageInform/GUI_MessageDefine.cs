using UnityEngine;
using System.Collections;
using System;

#region message arg base
public enum EMessageType
{
    MESSAGE_TYPE_COMMON = 1,
    MESSAGE_TYPE_SUCCESS = 2,
    MESSAGE_TYPE_ERROR = 3,
    MESSAGE_TYPE_CONFIRM_AND_CANCEL = 4,
    NONE
}

//相同类型的消息共用显示窗口，多个消息需要显示时，消息的排队策略
public enum EMessageQueueType
{
    MESSAGE_QUEUE_TYPE_QUEUE = 1,//排队
    MESSAGE_QUEUE_TYPE_DROP = 2,//放弃自己的显示，维持原有的显示不变
    MESSAGE_QUEUE_TYPE_SEIZE = 3,//直接显示自己，原有的显示被抢占（隐藏）
    NONE
}

public class MessageArg
{
    public string MessageTag = "";
    public string MessageContent = "";
    public bool Show = false;
    public EMessageType MessageType = EMessageType.MESSAGE_TYPE_COMMON;
    public EMessageQueueType MessageQueueType = EMessageQueueType.MESSAGE_QUEUE_TYPE_QUEUE;
    public MessageArg()
    { }
    public MessageArg(string name, string msg, bool show, EMessageType type, EMessageQueueType queueType)
    {
        MessageTag = name;
        MessageContent = msg;
        Show = show;
        MessageType = type;
        MessageQueueType = queueType;
    }
    public MessageArg(MessageArg ma)
    {
        if (ma != null)
        {
            MessageTag = ma.MessageTag;
            MessageContent = ma.MessageContent;
            Show = ma.Show;
            MessageType = ma.MessageType;
            MessageQueueType = ma.MessageQueueType;
        }
    }
    public void InitFromBase(MessageArg ma)
    {
        if (ma != null)
        {
            MessageTag = ma.MessageTag;
            MessageContent = ma.MessageContent;
            Show = ma.Show;
            MessageType = ma.MessageType;
            MessageQueueType = ma.MessageQueueType;
        }
    }
}
#endregion

#region common tip info
public class CommonMessageArg : MessageArg
{
    public CommonMessageArg()
        : base()
    { }
    public CommonMessageArg(string name, string msg, bool show)
        : base(name, msg, show, EMessageType.MESSAGE_TYPE_ERROR, EMessageQueueType.MESSAGE_QUEUE_TYPE_SEIZE)
    {
    }
    public CommonMessageArg(CommonMessageArg ti)
        : base(ti)
    {
    }
    public CommonMessageArg(MessageArg ti)
        : base(ti)
    {
    }
}
#endregion

#region error tip info
public class ErrorTipArg : MessageArg
{
    public ErrorTipArg()
        : base()
    { }
    public ErrorTipArg(string name, string msg, bool show)
        : base(name, msg, show, EMessageType.MESSAGE_TYPE_ERROR, EMessageQueueType.MESSAGE_QUEUE_TYPE_SEIZE)
    {
    }
    public ErrorTipArg(ErrorTipArg ti)
        : base(ti)
    {
    }
    public ErrorTipArg(MessageArg ti)
        : base(ti)
    {
    }
}
#endregion

#region Message box info
public enum MessageBoxType
{
    Confirm = 0,
    ConfirmAndConcell = 1,
    RetryAndCancell = 2,
    One = 3,
    None
}
public class MessageBoxArg : MessageArg
{
    public string Title = null;
    public string ButtonConfirm = null;
    public string ButtonCancel = null;
    public bool UseMsgBtnAndTitle = false;
    public Action OnConfirm = null;
    public Action OnCancel = null;
    public MessageBoxType MessageType = MessageBoxType.One;
    public MessageBoxArg()
        : base()
    { }
    public MessageBoxArg(string title,
                         string message,
                         string confirm,
                         string cancel,
                         Action onConfirm,
                         Action onCancel,
                         MessageBoxType type,
                         bool useMsgBtnAndTitle = false)
        : base("", message, true, EMessageType.MESSAGE_TYPE_CONFIRM_AND_CANCEL, EMessageQueueType.MESSAGE_QUEUE_TYPE_QUEUE)
    {
        Title = title;
        ButtonConfirm = confirm;
        ButtonCancel = cancel;
        OnConfirm = onConfirm;
        OnCancel = onCancel;
        MessageType = type;
        UseMsgBtnAndTitle = useMsgBtnAndTitle;
    }
}
#endregion
