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
public sealed class GUI_MessageBox : GUI_BaseMessageUI
{

    #region Message Panel
    public Text Title;
    public Text Message;
    public Text ConfirmLabel;
    public Text CancelLabel;
    public Image ConfirmIcon;
    public Image CancelIcon;
    public Button ConfirmButton;
    public Button CancelButton;
    #endregion
}