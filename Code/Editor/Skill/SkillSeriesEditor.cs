using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using SKILL_EDITOR;
using UnityEditor.AnimatedValues;

public class SkillSerie
{
    public int ID = -1;
    public int TmID = -1;
    public List<Skill> Skills = new List<Skill>();
    public School SchoolEx = School.Sword;
    Color _color = new Color(0, 1, 1);
    GUIContent _copyTip = new GUIContent("c", "复制");
    public void Draw()
    {
        GUI.backgroundColor = _color;
        for (int i = 0; i < Skills.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            string name = "(" + Skills[i].ID + ")" + Skills[i].Name;
            if (GUILayout.Button(name, SkillEditorUtility.LeftButton, GUILayout.MaxHeight(30)))
            {
                SkillDetailEditor.SkillEx = Skills[i];
                //SkillDetailEditor win = EditorWindow.CreateInstance<SkillDetailEditor>(); // 使用CreateInstance是为了多开
                SkillDetailEditor win = EditorWindow.GetWindow<SkillDetailEditor>(); // 使用CreateInstance是为了多开
                win.Init();
                win.titleContent = new GUIContent(Skills[i].Name);
                win.Show();
                EditorGUILayout.EndHorizontal();
                break;
            }
            if (GUILayout.Button(_copyTip, SkillEditorUtility.MidButton, GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
            {
                SkillClipboard.Copy(Skills[i]);
                EditorGUILayout.EndHorizontal();
                break;
            }
            if (GUILayout.Button("-", SkillEditorUtility.RightButton, GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
            {
                if (EditorUtility.DisplayDialog("警告", "确定要删除技能：\n\n" + name + " ？", "确定", "取消"))
                {
                    SkillEditor.RemoveOneSkill(Skills[i]);
                    Skills.Remove(Skills[i]);
                    EditorGUILayout.EndHorizontal();
                }
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.MaxHeight(30)))
        {
            Skill skill = SkillEditor.GenerateOneSkill(SchoolEx, ID);
            Skills.Add(skill);
        }
        EditorGUILayout.EndHorizontal();
    }
}