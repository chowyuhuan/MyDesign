using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using SKILL_EDITOR;
using UnityEditor.AnimatedValues;

public class SkillSchool
{
    public EditorWindow OwnerEditorWin;
    public List<SkillSerie> Series = new List<SkillSerie>();
    public School SchoolEx = School.Sword;
    private SkillSerie _preOpenSerie = null;
    private SkillSerie _currentOpenSerie = null;
    private Color _color = new Color(1, 0.55f, 0);
    private AnimBool _curAnimBool;
    private AnimBool _preAnimBool;
    private int _groupID = -1;
    private GUIStyle _numStyle1;
    private GUIStyle _numStyle2;
    public void Init(EditorWindow win)
    {
        OwnerEditorWin = win;
        _curAnimBool = SkillEditor.CreateOneAnimBool(false, win);
        _preAnimBool = SkillEditor.CreateOneAnimBool(false, win);
        _numStyle1 = new GUIStyle(EditorStyles.numberField);
        _numStyle1.alignment = TextAnchor.MiddleCenter;
        _numStyle2 = new GUIStyle(_numStyle1);
    }
    public void Draw()
    {
        foreach (var s in Series)
        {
            GUI.backgroundColor = _color;
            EditorGUILayout.BeginHorizontal();
            int preID = s.TmID;
            s.TmID = EditorGUILayout.DelayedIntField(s.TmID, _numStyle2, GUILayout.MaxWidth(80), GUILayout.MaxHeight(40));
            if (preID != s.TmID)
            {
                if (CheckExist(s.TmID, s))
                {
                    s.TmID = s.ID;
                    OwnerEditorWin.ShowNotification(new GUIContent(s.TmID + "已经存在，请重新输入"));
                }
                else
                {
                    s.ID = s.TmID;
                    for (int j = 0; j < s.Skills.Count; ++j)
                    {
                        s.Skills[j].GroupID = s.TmID;
                    }
                }
                OwnerEditorWin.Repaint();
            }

            string name = s.Skills.Count > 0 ? s.Skills[0].Name.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '级') : "空";
            //name = "[" + s.ID + "]" + name;
            if (GUILayout.Button(name, SkillEditorUtility.LeftButton, GUILayout.MinHeight(40)))
            {
                if (_currentOpenSerie == s)
                {
                    _curAnimBool.target = !_curAnimBool.target;
                }
                else
                {
                    _preOpenSerie = _currentOpenSerie;
                    SkillEditor.RemoveOneAnimBool(_preAnimBool);
                    _preAnimBool = _curAnimBool;
                    _preAnimBool.target = false;
                    _currentOpenSerie = s;
                    _curAnimBool = SkillEditor.CreateOneAnimBool(false, OwnerEditorWin);
                    _curAnimBool.target = true;
                }
            }


            if (GUILayout.Button("-", SkillEditorUtility.RightButton, GUILayout.MaxWidth(45), GUILayout.MaxHeight(40)))
            {
                if (EditorUtility.DisplayDialog("警告", "确定要删除技能组：\n\n" + name + " ？", "确定", "取消"))
                {
                    for (int i = 0; i < s.Skills.Count; ++i)
                    {
                        SkillEditor.RemoveOneSkill(s.Skills[i]);
                    }
                    Series.Remove(s);
                    EditorGUILayout.EndHorizontal();
                }
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (_preOpenSerie == s)
            {
                if (EditorGUILayout.BeginFadeGroup(_preAnimBool.faded))
                {
                    _preOpenSerie.Draw();
                }
                SkillEditor.FixedEndFadeGroup(_preAnimBool.faded);
            }

            if (_currentOpenSerie == s)
            {
                if (EditorGUILayout.BeginFadeGroup(_curAnimBool.faded))
                {
                    _currentOpenSerie.Draw();
                }
                SkillEditor.FixedEndFadeGroup(_curAnimBool.faded);
            }
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("创建新技能组？请输入ID：");
        EditorGUILayout.BeginHorizontal();
        _groupID = EditorGUILayout.IntField(_groupID, _numStyle1, GUILayout.MinHeight(24));
        if (GUILayout.Button("创建", GUILayout.MinHeight(20), GUILayout.MaxWidth(40)))
        {
            bool passed = true;
            if (CheckExist(_groupID, null))
            {
                OwnerEditorWin.ShowNotification(new GUIContent(_groupID + "已经存在，请重新输入"));
                _groupID = -1;
                passed = false;
            }
            else if (_groupID <= 0)
            {
                OwnerEditorWin.ShowNotification(new GUIContent(_groupID + "不合法，请重新输入"));
                _groupID = -1;
                passed = false;
            }
            if (passed)
            {
                SkillSerie serie = SkillEditor.GenerateOneGroup(SchoolEx);
                Series.Add(serie);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    bool CheckExist(int groupID, SkillSerie except)
    {
        for (int i = 0; i < Series.Count; ++i)
        {
            if (Series[i] != except && Series[i].ID == groupID)
            {
                return true;
            }
        }
        return false;
    }
}