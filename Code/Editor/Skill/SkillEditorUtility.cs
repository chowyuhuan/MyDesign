using UnityEngine;
using UnityEditor;
using System.Collections;
using SKILL;

public class SkillEditorUtility
{
    #region GUIStyle
    private static GUIStyle _leftButton;
    private static GUIStyle _rightButton;
    private static GUIStyle _midButton;
    public static GUIStyle LeftButton
    {
        get
        {
            if (_leftButton == null)
            {
                _leftButton = GUI.skin.GetStyle("ButtonLeft");
            }
            return _leftButton;
        }
    }
    public static GUIStyle MidButton
    {
        get
        {
            if (_midButton == null)
            {
                _midButton = GUI.skin.GetStyle("ButtonMid");
            }
            return _midButton;
        }
    }
    public static GUIStyle RightButton
    {
        get
        {
            if (_rightButton == null)
            {
                _rightButton = GUI.skin.GetStyle("ButtonRight");
            }
            return _rightButton;
        }
    }
    #endregion
}
