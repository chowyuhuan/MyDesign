using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ErrorTip : GUI_BaseMessageUI
{
    #region tip ui
    public Text Content;
    public GUI_TweenAlpha TweenAlpha = null;
    public GUI_TweenScale TweenScale = null;
    public GUI_TweenPosition TweenPostion = null;
    #endregion
}