using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using System;
using SKILL;
using SKILL_EDITOR;

public class SkillEditor : EditorWindow
{
    public const float Width_Float = 300;
    public const float Width_Int = 300;
    public const float Width_Enum = 300;
    public const float Width_Name = 300;
    public const float Width_Vector2 = 300;
    public const float Width_Bool = 300;
    public const float Width_Charactoer_CN = 15;
    public const float Width_Charactoer_EN = 8;
    public const float Width_Lable = 60;
    public const float Width_Padding = 5;
    public static readonly Rect Rect_Content = new Rect(100, 20, 500, 5000);

    private static GUIStyle _lineStyle = null;

    public static SkillDataBase[] DataFile = new SkillDataBase[6];
    public delegate bool ReBoolAction();

    static SkillSchool[] _skillSchools = new SkillSchool[6];
    int _selectIdx = 0;
    string[] _schoolNames = new string[] { "剑士", "骑士", "弓手", "猎人", "法师", "祭司" };
    Color _color = new Color(0, 1, 0.78f, 1);
    bool _init = false;

    void OnEnable()
    {
        for (int i = 0; i < _skillSchools.Length; ++i)
        {
            SkillSchool temp = new SkillSchool();
            temp.SchoolEx = (School)i;
            _skillSchools[i] = temp;
        }

        LoadDataFromFiles();

        for (int k = 0; k < DataFile.Length; ++k)
        {
            SkillDataBase dataBase = DataFile[k];
            if (dataBase == null)
            {
                continue;
            }
            for (int i = 0; i < dataBase.Data.Count; ++i)
            {
                Skill skill = dataBase.Data[i];
                SkillSchool sschool = _skillSchools[(int)skill.SchoolEx];
                bool exist = false;
                for (int j = 0; j < sschool.Series.Count; ++j)
                {
                    if (sschool.Series[j].ID == skill.GroupID)
                    {
                        sschool.Series[j].Skills.Add(skill);
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    SkillSerie tmSerie = new SkillSerie();
                    tmSerie.ID = skill.GroupID;
                    tmSerie.TmID = skill.GroupID;
                    tmSerie.SchoolEx = skill.SchoolEx;
                    tmSerie.Skills.Add(skill);
                    sschool.Series.Add(tmSerie);
                }
            }
        }

        // 系列中，按照groupid排下序
        for (int i = 0; i < _skillSchools.Length; ++i)
        {
            _skillSchools[i].Series.Sort((left, right) =>
            {
                if (left.ID > right.ID)
                    return 1;
                else if (left.ID == right.ID)
                    return 0;
                else
                    return -1;
            });
        }
        _init = false; // 为了解决不能再OnEnable中获取EditorStyles的各种GUIStyle。在界面打开时，修改代码，重新编译完成后会再次调用OnEnable，但这个时候EditorStyles里面的GUIStyle都还是空的
    }

    static void LoadDataFromFiles()
    {
        DataFile[(int)School.Sword] = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Resources/configs/SkillDataBase_Sword.asset");
        DataFile[(int)School.Knight] = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Resources/configs/SkillDataBase_Knight.asset");
        DataFile[(int)School.Archer] = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Resources/configs/SkillDataBase_Archer.asset");
        DataFile[(int)School.Hunter] = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Resources/configs/SkillDataBase_Hunter.asset");
        DataFile[(int)School.Wizard] = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Resources/configs/SkillDataBase_Wizard.asset");
        DataFile[(int)School.Flamen] = AssetDatabase.LoadAssetAtPath<SkillDataBase>("Assets/Resources/configs/SkillDataBase_Flamen.asset");
    }

    [MenuItem("工具/技能/编辑器")]
    static void OpenWindow()
    {
        SkillEditor win = GetWindow<SkillEditor>("技能");
        win.Show();
    }

    void OnGUI()
    {
        Init();
        GUI.backgroundColor = _color;
        _selectIdx = GUILayout.Toolbar(_selectIdx, _schoolNames, GUILayout.MaxHeight(20));
        if (_selectIdx != -1)
        {
            _skillSchools[_selectIdx].Draw();
        }
    }

    void Init()
    {
        if(!_init)
        {
            for (int i = 0; i < _skillSchools.Length; ++i )
            {
                _skillSchools[i].Init(this);
            }
            _init = true;
        }
    }

    [MenuItem("工具/技能/规范化所有技能")]
    static void NormalizeAllSkills()
    {
        LoadDataFromFiles();
        for (int j = 0; j < DataFile.Length; ++j )
        {
            SkillDataBase dataBase = DataFile[j];
            if (dataBase == null)
            {
                continue;
            }
            for (int i = 0; i < dataBase.Data.Count; ++i)
            {
                NormalizeSkill(dataBase.Data[i]);
            }
            Sort(dataBase);
            EditorUtility.SetDirty(dataBase);
        }

        AssetDatabase.SaveAssets();
    }

    public static void DrawContentAroundFadeGroup(AnimBool aBool, string lable, Action drawFunc, int indent = 1)
    {
        aBool.target = EditorGUILayout.ToggleLeft(lable, aBool.target);

        //Extra block that can be toggled on and off.
        if (EditorGUILayout.BeginFadeGroup(aBool.faded))
        {
            EditorGUI.indentLevel += indent;

            drawFunc();

            EditorGUI.indentLevel -= indent;
        }
        FixedEndFadeGroup(aBool.faded);
        //EditorGUILayout.EndFadeGroup(); // 不能用这个，这个在嵌套的时候，会有问题
    }

    public static void DrawContentAroundFadeGroupAndButton(AnimBool aBool, string lable, Action drawFunc, string btnText, ReBoolAction btnCallBack, int indent = 1)
    {
        EditorGUILayout.BeginHorizontal();
        aBool.target = EditorGUILayout.ToggleLeft(lable, aBool.target);
        if (GUILayout.Button(btnText, GUILayout.MaxWidth(SkillEditor.Width_Charactoer_CN * btnText.Length + SkillEditor.Width_Padding * 2)))
        {
            if (btnCallBack()) // 如果返回true，则打断执行
            {
                EditorGUILayout.EndHorizontal();
                return;
            }
        }
        EditorGUILayout.EndHorizontal();

        //Extra block that can be toggled on and off.
        if (EditorGUILayout.BeginFadeGroup(aBool.faded))
        {
            EditorGUI.indentLevel += indent;

            drawFunc();

            EditorGUI.indentLevel -= indent;
        }
        FixedEndFadeGroup(aBool.faded);
        //EditorGUILayout.EndFadeGroup(); // 不能用这个，这个在嵌套的时候，会有问题
    }

    public static void FixedEndFadeGroup(float value)
    {
        if (value == 0 || value == 1)
        {
            return;
        }
        EditorGUILayout.EndFadeGroup();
    }

    public static GUIStyle CreateLineStyle(int horiz, int vertical)
    {
        GUIStyle style = new GUIStyle("box");
        style.border.top = style.border.bottom = 1;
        style.margin.top = style.margin.bottom = vertical;
        style.margin.left = style.margin.right = horiz;
        return style;
    }

    public static void DrawLine(GUIStyle style)
    {
        GUILayout.Box(GUIContent.none, style, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
    }

    public static void DrawLine()
    {
        if (_lineStyle == null)
        {
            GUIStyle style = new GUIStyle("box");
            style.border.top = style.border.bottom = 1;
            style.margin.top = style.margin.bottom = 1;
            style.margin.left = style.margin.right = 0;
        }
        GUILayout.Box(GUIContent.none, _lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
    }

    public static AnimBool CreateOneAnimBool(bool defValue, EditorWindow win)
    {
        AnimBool tmp = new AnimBool(defValue);
        tmp.valueChanged.AddListener(win.Repaint);
        return tmp;
    }

    public static void RemoveOneAnimBool(AnimBool aBool)
    {
        aBool.target = false;
        aBool.valueChanged.RemoveAllListeners();
    }

    public static void AddOneSkill(Skill skill)
    {
        DataFile[(int)skill.SchoolEx].Data.Add(skill);
        RefreshSkillData(skill.SchoolEx);
    }

    public static void RemoveOneSkill(Skill skill)
    {
        DataFile[(int)skill.SchoolEx].Data.Remove(skill);
        EditorUtility.SetDirty(DataFile[(int)skill.SchoolEx]);
        AssetDatabase.Refresh();
    }

    public static void RefreshSkillData(School school)
    {
        Sort(DataFile[(int)school]);
        EditorUtility.SetDirty(DataFile[(int)school]);
        AssetDatabase.Refresh();
    }

    public static void NormalizeSkill(Skill skill)
    {
        AttackMeta[] atks = skill.Attacks;
        if (atks == null)
        {
            return;
        }

        bool dirty = false;
        for (int i = 0; i < atks.Length; ++i)
        {
            AttackMeta atk = atks[i];
            DCMeta[] dcs = atk.DCs;
            if (dcs == null)
            {
                return;
            }
            for (int j = 0; j < dcs.Length; ++j)
            {
                DCMeta dc = dcs[j];

                // do something
            }
        }

        if (dirty)
        {
            RefreshSkillData(skill.SchoolEx);
        }
    }

    public static void Sort(SkillDataBase dataBase)
    {
        if (dataBase != null)
        {
            dataBase.Data.Sort((left, right) =>
            {
                if (left.ID > right.ID)
                    return 1;
                else if (left.ID == right.ID)
                    return 0;
                else
                    return -1;
            });
        }
    }

    public static bool CheckSkillExist(int id, Skill skill)
    {
        for (int i = 0; i < DataFile.Length; ++i)
        {
            List<Skill> list = DataFile[i].Data;
            for (int j = 0; j < list.Count; ++j )
            {
                if (list[j].ID == id && list[j] != skill)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static Skill GenerateOneSkill(School school, int group)
    {
        int skillID = GenerateSkillIDInSchool(school, group);
        if (skillID == 0)
        {
            skillID = GenerateSkillIDInGlobal(group);
        }

        Skill skill = SkillClipboard.Paste<Skill>();
        if (skill == null)
        {
            skill = new Skill();
        }
        skill.ID = skillID;
        skill.SchoolEx = school;
        skill.GroupID = group;
        SkillEditor.AddOneSkill(skill);
        return skill;
    }

    public static int GenerateSkillIDInSchool(School school, int group)
    {
        int skillID = -1;
        List<Skill> list = DataFile[(int)school].Data;
        for (int i = 0; i < list.Count; ++i )
        {
            int id = list[i].ID;
            if (id > skillID)
            {
                skillID = id;
            }
        }
        return skillID == -1 ? 0 : ++skillID;
    }

    public static int GenerateSkillIDInGlobal(int group)
    {
        int skillID = -1;
        for (int i = 0; i < _skillSchools.Length; ++i )
        {
            int tempID = GenerateSkillIDInSchool((School)i, group);
            skillID = tempID > skillID ? tempID : skillID;
        }
        return skillID == -1 ? 0 : ++skillID;
    }

    public static SkillSerie GenerateOneGroup(School school)
    {
        int maxID = -1;
        SkillSchool sschool = _skillSchools[(int)school];
        for (int i = 0; i < sschool.Series.Count; ++i)
        {
            SkillSerie serie = sschool.Series[i];
            if (serie.ID > maxID)
            {
                maxID = serie.ID;
            }
        }

        maxID = maxID == -1 ? 0 : ++maxID;

        Skill skill = GenerateOneSkill(school, maxID);
        if (skill != null)
        {
            SkillSerie serie = new SkillSerie();
            serie.ID = maxID;
            serie.TmID = maxID;
            serie.SchoolEx = school;
            serie.Skills.Add(skill);
            return serie;
        }
        return null;
    }


    #region Unity Message
    //OnDestroy is called when the EditorWindow is closed
    void OnDestroy()
    {

    }

    //Called when the window gets keyboard focus.
    void OnFocus()
    {

    }

    //Called whenever the scene hierarchy has changed.
    void OnHierarchyChange()
    {

    }
    //OnInspectorUpdate is called at 10 frames per second to give the inspector a chance to update.
    void OnInspectorUpdate()
    { 

    }
    //Called when the window loses keyboard focus.
    void OnLostFocus()
    {

    }
    //Called whenever the project has changed.
    void OnProjectChange()
    {

    }
    //Called whenever the selection has changed.
    void OnSelectionChange()
    {

    }
    //Called multiple times per second on all visible windows.
    void Update()
    {

    }
    #endregion
}