using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GUI_MessageManager : Singleton<GUI_MessageManager>
{
    Dictionary<string, MessageArg> _MessagesInfo = null;
    Dictionary<int, LinkedList<MessageArg>> _Messages = null;
    public GUI_MessageManager()
        : base()
    {
        _MessagesInfo = new Dictionary<string, MessageArg>();
        _Messages = new Dictionary<int, LinkedList<MessageArg>>();
        LoadMessages();
    }
    void LoadMessages()
    {
        for (int index = 0; index < CSV_c_gui_message_config.DateCount; index++)
        {
            CSV_c_gui_message_config message = CSV_c_gui_message_config.GetData(index);
            if (!string.IsNullOrEmpty(message.MessageTag) && !_MessagesInfo.ContainsKey(message.MessageTag))
            {
                MessageArg info = new MessageArg(message.MessageTag,
                                                 message.MessageContent,
                                                 message.Show,
                                                 GetMessageType(message.MessageType),
                                                 GetMessageQueueType(message.MessageQueueType));
                _MessagesInfo.Add(message.MessageTag, info);
            }
        }
    }

    public EMessageQueueType GetMessageQueueType(int type)
    {
        return (EMessageQueueType)type;
    }
    #region common message interface

    public MessageArg GetNextMessage(MessageArg arg)
    {
        if (arg == null)
        {
            return null;
        }
        return GetNextMessage(arg.MessageType);
    }

    public MessageArg GetNextMessage(EMessageType type)
    {
        MessageArg arg = null;
        LinkedList<MessageArg> list = GetMessageList(type);
        if (list != null && list.Count > 0)
        {
            LinkedListNode<MessageArg> node = list.First;
            arg = node.Value;
            list.RemoveFirst();
        }
        return arg;
    }

    public int GetMessageTypeIndex(EMessageType type)
    {
        return (int)type;
    }

    public bool ExistType(int type)
    {
        return (type >= GetMessageTypeIndex(EMessageType.MESSAGE_TYPE_COMMON) &&
                type < GetMessageTypeIndex(EMessageType.NONE));
    }

    public EMessageType GetMessageType(int type)
    {
        return (EMessageType)type;
    }

    public LinkedList<MessageArg> GetMessageList(EMessageType type)
    {
        LinkedList<MessageArg> list = null;
        int index = GetMessageTypeIndex(type);
        if (ExistType(index))
        {
            if (_Messages.ContainsKey(index))
            {
                list = _Messages[index];
            }
            else
            {
                list = new LinkedList<MessageArg>();
                _Messages.Add(index, list);
            }
        }
        return list;
    }

    public LinkedList<MessageArg> GetMessageList(MessageArg arg)
    {
        if (arg == null)
        {
            return null;
        }
        return GetMessageList(arg.MessageType);
    }

    public void ShowMessage<MessageWindow, MsgArg>(MsgArg arg)
        where MessageWindow : GUI_BaseMessageUI_DL
        where MsgArg : MessageArg
    {
        if (arg == null)
        {
            return;
        }
        if (!ExistType(GetMessageTypeIndex(arg.MessageType)))
        {
            return;
        }
        string winName = GUI_Tools.CommonTool.GetClassNameWithoutNameSpace<MessageWindow>();
        MessageWindow box = GUI_Manager.Instance.FindWindowWithName(winName, false) as MessageWindow;
        if (box != null)
        {//other message is showing, add message to queue
            CheckMsgQueueType<MessageWindow, MsgArg>(box, arg);
            return;
        }
        else
        {
            //creat one and show directly
            DoShowMessage<MessageWindow, MsgArg>(winName, arg);
        }
    }

    private void QueueMessage<MsgArg>(MsgArg arg)
        where MsgArg : MessageArg
    {
        LinkedListNode<MessageArg> node = new LinkedListNode<MessageArg>(arg);
        node.Value = arg;
        LinkedList<MessageArg> list = GetMessageList(arg);
        list.AddLast(node);
    }

    private void CheckMsgQueueType<MessageWindow, MsgArg>(MessageWindow box, MsgArg arg)
        where MessageWindow : GUI_BaseMessageUI_DL
        where MsgArg : MessageArg
    {
        switch (arg.MessageQueueType)
        {
            case EMessageQueueType.MESSAGE_QUEUE_TYPE_DROP:
                {
                    break;//维持原有的显示
                }
            case EMessageQueueType.MESSAGE_QUEUE_TYPE_QUEUE:
                {
                    QueueMessage(arg);
                    break;
                }
            case EMessageQueueType.MESSAGE_QUEUE_TYPE_SEIZE:
                {
                    box.HideWindow();
                    string winName = box.WindowName;
                    DoShowMessage<MessageWindow, MsgArg>(winName, arg);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void DoShowMessage<MessageWindow, MsgArg>(string winName, MsgArg arg)
        where MessageWindow : GUI_BaseMessageUI_DL
        where MsgArg : MessageArg
    {
        MessageWindow box = GUI_Manager.Instance.ShowWindowWithName<MessageWindow>(winName, false);
        if (box != null)
        {
            box.ShowMessage(arg);
        }
        else
        {
            Debug.LogError("Error : no " + winName + " panel found !!!");
        }
    }

    public bool ShowMessage<MessageWindow, MsgArg>(string msg,
                                                   EMessageType type = EMessageType.MESSAGE_TYPE_COMMON,
                                                   bool msgAsTag = false,
                                                   EMessageQueueType queueType = EMessageQueueType.MESSAGE_QUEUE_TYPE_SEIZE)
        where MessageWindow : GUI_BaseMessageUI_DL
        where MsgArg : MessageArg, new()
    {
        bool done = false;
        if (msgAsTag)
        {
            if (_MessagesInfo.ContainsKey(msg) && _MessagesInfo[msg].Show)
            {
                MsgArg arg = new MsgArg();
                arg.InitFromBase(_MessagesInfo[msg]);
                ShowMessage<MessageWindow, MsgArg>(arg);
                done = true;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(msg))
            {
                MessageArg ba = new MessageArg("",
                                               msg,
                                               true,
                                               type,
                                               queueType);
                MsgArg arg = new MsgArg();
                arg.InitFromBase(ba);
                ShowMessage<MessageWindow, MsgArg>(arg);
                done = true;
            }
        }
        return done;
    }
    #endregion

    #region promotion info has been shown or not
    public void ShowPromotionInfo(string promotion, GameObject target)
    {
        //if (_ErrorMessages.ContainsKey (promotion)) {
        //    FreeArrowTip tip = GUIManager.Instance.FindWindowWithName("FreeArrowTip",true) as FreeArrowTip;	
        //    if(tip != null){
        //        tip.ShowTip(_ErrorMessages[promotion]._ErrorMessage, target);
        //        _ErrorMessages.Remove(promotion);
        //    }
        //}
    }
    #endregion

    #region simple error tip

    public bool ShowErrorTip(string msg, bool msgAsTag = false)
    {
        bool done = false;
        if (msgAsTag)
        {
            if (_MessagesInfo.ContainsKey(msg) && _MessagesInfo[msg].Show)
            {
                ShowError(_MessagesInfo[msg]);
                done = true;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(msg))
            {
                ShowError(msg);
                done = true;
            }
        }
        return done;
    }

    public bool ShowErrorTip(int errorcode, bool useDefaultMsg = true)
    {
        if (_MessagesInfo.ContainsKey("ErrorCode_" + errorcode.ToString()))
        {
            if (!_MessagesInfo["ErrorCode_" + errorcode.ToString()].Show)
            {
                return false;
            }
        }
        if (_MessagesInfo.ContainsKey("ErrorCode_" + errorcode.ToString()))
        {
            ShowError(_MessagesInfo["ErrorCode_" + errorcode.ToString()]);
            return true;
        }
        else if (useDefaultMsg && _MessagesInfo.ContainsKey("Error_Net_Default"))
        {
            ShowError(_MessagesInfo["Error_Net_Default"], errorcode, true);
            return true;
        }
        return false;
    }

    void ShowError(MessageArg arg, int errorcode = -1, bool unknownError = false)
    {
        ErrorTipArg et = new ErrorTipArg(arg);
        if (unknownError)
        {
            et.MessageContent += " :" + errorcode.ToString();
        }
        ShowMessage<GUI_ErrorTip_DL, MessageArg>(et);
    }

    void ShowError(string info)
    {
        if (string.IsNullOrEmpty(info))
        {
            return;
        }
        ErrorTipArg arg = new ErrorTipArg("",
                                          info,
                                          true);
        ShowMessage<GUI_ErrorTip_DL, MessageArg>(arg);
    }
    #endregion

    #region common message box with confirm or cancel buttons
    string GetConfirmButton(MessageBoxType type)
    {
        string conf = null;
        switch (type)
        {
            case MessageBoxType.RetryAndCancell:
                {
                    TextLocalization.GetText(TextId.Retry, out conf);
                    break;
                }
            case MessageBoxType.ConfirmAndConcell:
                {
                    TextLocalization.GetText(TextId.Confirm, out conf);
                    break;
                }
            default:
                {
                    TextLocalization.GetText(TextId.Cancel, out conf);
                    break;
                }
        }
        return conf;
    }

    string GetCancelButton(MessageBoxType type)
    {
        string conf = null;
        TextLocalization.GetText(TextId.Cancel, out conf);
        return conf;
    }

    public void ShowMessage(string message,
                            MessageBoxType type = MessageBoxType.Confirm,
                            Action confirm = null,
                            Action cancel = null)
    {
        ShowMessage("",
                    message,
                    GetConfirmButton(type),
                    GetCancelButton(type),
                    confirm,
                    cancel,
                    type,
                    true);
    }

    public void ShowMessage(string title,
                            string message,
                            string buttonConfirm,
                            string buttonConcel,
                            Action confirm,
                            Action cancel,
                            MessageBoxType type,
                            bool useMsgBtnAndTitle = false)
    {
        MessageBoxArg ms = new MessageBoxArg(title,
                                             message,
                                             buttonConfirm,
                                             buttonConcel,
                                             confirm,
                                             cancel,
                                             type,
                                             useMsgBtnAndTitle);
        ShowMessage(ms);
    }

    public void ShowMessage(MessageBoxArg message)
    {
        if (message == null)
        {
            return;
        }
        ShowMessage<GUI_MessageBox_DL, MessageBoxArg>(message);
    }
    #endregion
}