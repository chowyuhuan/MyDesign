using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using UnityEditor.AnimatedValues;
using System;
using ACTOR;
using SKILL_EDITOR;

public class SkillDetailEditor : EditorWindow
{
    public static Skill SkillEx = null; // static：为了能够在OnEnable之前，就获取到SkillEx（OnEnable在GetWindow时就已经被调用）
    private SkillNodeBase _rootNode = null;
    private static Vector2 _viewOffset = Vector2.zero;
    Vector2 _mousePos = new Vector2(float.MinValue, float.MinValue);
    GUIStyle _tipStyle = null;

    public void Init()
    {
        if (SkillEx == null)
        {
            return;
        }

        _tipStyle = new GUIStyle(EditorStyles.label);
        _tipStyle.alignment = TextAnchor.UpperRight;

        SkillNodeBase.ClearGNodes();

        SkillNode skill = new SkillNode(null, 0, SkillEx.Name, new Vector2(180, 167));
        skill.MetaData = SkillEx;
        skill.Rect = new Rect(0, 0, skill.Size.x, skill.Size.y);
        skill.OnCreated();
        _rootNode = skill;
    }

    public static void BuildVolumeNode(ref Volume vol, SkillNodeBase parentNode)
    {
        if (vol == null)
        {
            vol = new Volume();
        }
        if (vol.Atks != null)
        {
            for (int at = 0; at < vol.Atks.Length; ++at)
            {
                parentNode.CreateChild(Node.VolAtk, vol.Atks[at], false);
            }
        }

        if (vol.Fields != null)
        {
            for (int f = 0; f < vol.Fields.Length; ++f)
            {
                parentNode.CreateChild(Node.VolField, vol.Fields[f], false);
            }
        }
    }

    void OnGUI()
    {
        if (_rootNode == null)
        {
            return;
        }

        EditorGUILayout.LabelField("按住鼠标左键并移动可拖拽指定节点", _tipStyle);
        EditorGUILayout.LabelField("Ctrl+按住鼠标左键并移动可拖拽指定节点及其子节点", _tipStyle);
        EditorGUILayout.LabelField("按住鼠标中键并移动可拖拽整棵技能树", _tipStyle);
        EditorGUILayout.LabelField("点击鼠标右键可复制指定节点", _tipStyle);

        //SkillNodeBase.NeedRepaint = false;
        EditorGUI.BeginChangeCheck();
        BeginWindows();
        _rootNode.DrawNode();
        EndWindows();
        if (EditorGUI.EndChangeCheck())
        {
            SkillEditor.RefreshSkillData(SkillEx.SchoolEx);
        }
        //if (SkillNodeBase.NeedRepaint)
        //{
        //    Repaint();
        //}

        Event e = Event.current;
        if (e.button == 2 && e.type == UnityEngine.EventType.mouseDrag)
        {
            Vector2 currPos = Event.current.mousePosition;
            if (Vector2.Distance(currPos, _mousePos) < 100)
            {
                float x = currPos.x - _mousePos.x;
                float y = currPos.y - _mousePos.y;
                _rootNode.Move(new Vector2(x,y),true);
                _viewOffset.x += x;
                _viewOffset.y += y;
                Event.current.Use();
            }
            _mousePos = currPos;
        }
    }

    public static void DrawNodeCurve(Rect start, Rect end, Color color)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height, 0);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y, 0);
        Handles.color = color;
        Handles.DrawLine(startPos, endPos);
    }

    public static bool DrawNumeric(NumericMeta num, bool foldout)
    {
        foldout = EditorGUILayout.Foldout(foldout, "数值");
        if (foldout)
        {
            if (num.Nums.Count <= 0)
            {
                num.Nums.Add(new NumericMeta.ValueUnit(NumericMeta.Base.Caster, ActorField.ATK, Op.Multiply, 0.1f));
            }

            //EditorGUILayout.Separator();
            //EditorGUILayout.BeginHorizontal();
            //for (int i = 0; i < _num.Nums.Count; ++i)
            //{
            //    EditorGUILayout.LabelField(i.ToString() + "数值");
            //}
            //EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < num.Nums.Count; ++i)
            {
                NumericMeta.ValueUnit meta = num.Nums[i];
                meta.BaseEx = (NumericMeta.Base)EditorGUILayout.EnumPopup("基准", meta.BaseEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < num.Nums.Count; ++i)
            {
                NumericMeta.ValueUnit meta = num.Nums[i];
                meta.BaseField = (ActorField)EditorGUILayout.EnumPopup("属性", meta.BaseField, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < num.Nums.Count; ++i)
            {
                NumericMeta.ValueUnit meta = num.Nums[i];
                meta.OpEx = (Op)EditorGUILayout.EnumPopup("远算", meta.OpEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < num.Nums.Count; ++i)
            {
                NumericMeta.ValueUnit meta = num.Nums[i];
                meta.Factor = EditorGUILayout.DelayedFloatField("数值", meta.Factor, GUILayout.MaxWidth(SkillEditor.Width_Int));
            }
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.Separator();
        }
        return foldout;
    }

    void OnDestroy()
    {
        SkillEditor.NormalizeSkill(SkillEx);
        AssetDatabase.SaveAssets();
    }
}