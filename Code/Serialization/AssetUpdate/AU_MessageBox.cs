using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AssetUpdate
{
    public class AU_MessageBox : MonoBehaviour
    {

        public delegate void VoidDelegate();
        #region Message info
        public enum MessageType
        {
            Confirm = 0,
            ConfirmAndCancell = 1,
            None
        }
        public class Message
        {
            public string Title { get; protected set; }
            public string MessageContent { get; protected set; }
            public string ButtonConfirm { get; protected set; }
            public string ButtonCancel { get; protected set; }
            public VoidDelegate OnConfirm { get; protected set; }
            public VoidDelegate OnCancel { get; protected set; }
            public MessageType MessageType { get; protected set; }

            public Message(string title,
                           string message,
                           string confirm,
                           string cancel,
                           VoidDelegate onConfirm,
                           VoidDelegate onCancel,
                           MessageType type)
            {
                Title = title;
                MessageContent = message;
                ButtonConfirm = confirm;
                ButtonCancel = cancel;
                OnConfirm = onConfirm;
                OnCancel = onCancel;
                MessageType = type;
            }
        }
        #endregion

        #region Message Panel
        [SerializeField]
        Text _Title;
        [SerializeField]
        Text _MessageContent;
        [SerializeField]
        Text _ButtonConfirm;
        [SerializeField]
        Text _ButtonCancel;


        private Message _Message = null;

        public void Confirm()
        {
            if (_Message == null)
            {
                return;
            }
            if (_Message.OnConfirm != null)
            {
                _Message.OnConfirm();
            }
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            if (_Message == null)
            {
                return;
            }
            if (_Message.OnCancel != null)
            {
                _Message.OnCancel();
            }
            gameObject.SetActive(false);
        }
        public void DoShowMessage(Message message)
        {
            _Message = message;
            if (_Title != null)
            {
                _Title.text = message.Title;
            }
            if (_MessageContent != null)
            {
                _MessageContent.text = message.MessageContent;
            }
            if (_ButtonConfirm != null)
            {
                _ButtonConfirm.text = message.ButtonConfirm;
            }
            if (_ButtonCancel != null)
            {
                _ButtonCancel.text = message.ButtonCancel;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public static AU_MessageBox ShowMessage(string title,
                                string message,
                                string buttonConfirm,
                                string buttonConcel,
                                VoidDelegate confirm,
                                VoidDelegate cancel,
                                AU_MessageBox.MessageType type,
                                GameObject defaultPrefab)
        {
            AU_MessageBox.Message ms = new AU_MessageBox.Message(title,
                                                           message,
                                                           buttonConfirm,
                                                           buttonConcel,
                                                           confirm,
                                                           cancel,
                                                           type);
            return ShowMessage(ms, defaultPrefab);
        }

        public static AU_MessageBox ShowMessage(AU_MessageBox.Message message, GameObject defaultPrefab)
        {
            if (message == null)
            {
                return null;
            }
            defaultPrefab.SetActive(true);
            AU_MessageBox ms = defaultPrefab.GetComponent<AU_MessageBox>();
            if (null == ms)
            {
                ms = defaultPrefab.AddComponent<AU_MessageBox>();
            }
            ms.DoShowMessage(message);
            return ms;
        }
        #endregion
    }
}
