using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;
using UnityEditor;
using System;

namespace SKILL_EDITOR
{

    public sealed class SkillNode : SkillNodeBase
    {
        private int _skillID = -1;
        private GUIStyle _desStyle;
        private Texture2D _icon;
        private bool _showPicker = false;
        private GUIStyle _idIntStyle;
        private string _idLable = "ID";
        public SkillNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.Skill, Color.white) { }
        protected override void Draw()
        {
            Skill Skill = MetaData as Skill;
            if (GUILayout.Button(new GUIContent(_icon), GUILayout.Height(50), GUILayout.Width(50)))
            {
                IconEditWin.Prepare(Skill, RefreshIcon);
                IconEditWin win = EditorWindow.GetWindow<IconEditWin>();
                Vector2 pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                win.position = new Rect(pos.x, pos.y, 500, 500);
                win.Show();
            }
            EditorGUIUtility.labelWidth = 82;
            int preID = _skillID;
            _skillID = EditorGUILayout.DelayedIntField(_idLable, _skillID, _idIntStyle);
            if (preID != _skillID)
            {
                if (SkillEditor.CheckSkillExist(_skillID, Skill))
                {
                    _idLable = "ID（已经存在）";
                    _idIntStyle.focused.textColor = Color.red;
                    _idIntStyle.normal.textColor = Color.red;
                }
                else
                {
                    _idLable = "ID";
                    Skill.ID = _skillID;
                    _idIntStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    _idIntStyle.focused.textColor = EditorStyles.numberField.focused.textColor;
                }
                NeedRepaint = true;
            }
            Skill.Name = EditorGUILayout.DelayedTextField("名字", Skill.Name);
            Skill.Range = EditorGUILayout.DelayedFloatField("判定距离", Skill.Range);
            Skill.CastAnim = EditorGUILayout.DelayedTextField("施法动画", Skill.CastAnim);
            if (GUILayout.Button("+ 阶段进攻"))
            {
                CreateChild(Node.ATK);
            }
        }
        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            Skill Skill = MetaData as Skill;
            return CreateChildWithData<AttackMetaNode, AttackMeta>("段进攻", new Vector2(130, 70), ref Skill.Attacks, data, archive);
        }
        protected override void RemoveChildImp(SkillNodeBase _node)
        {
            Skill Skill = MetaData as Skill;
            RemoveOneData<AttackMeta>(ref Skill.Attacks, ref _node.MetaData);
        }
        protected override void Init()
        {
            RefreshIcon();
        }
        public override void OnCreated()
        {
            Skill Skill = MetaData as Skill;
            _skillID = Skill.ID;
            _desStyle = new GUIStyle(GUI.skin.button);
            _desStyle.wordWrap = true;
            _desStyle.alignment = TextAnchor.MiddleLeft;
            _desStyle.normal.background = null;
            _idIntStyle = new GUIStyle(EditorStyles.numberField);

            AttackMeta[] ATKMetas = Skill.Attacks;
            if (ATKMetas == null)
            {
                return;
            }

            for (int i = 0; i < ATKMetas.Length; ++i)
            {
                CreateChild(0, ATKMetas[i], false);
            }
        }
        void RefreshIcon()
        {
            Skill Skill = MetaData as Skill;
            GUI_Atlas atlas = AssetDatabase.LoadAssetAtPath<GUI_Atlas>("Assets/Resources/GUI/UIAtlas/" + Skill.IconAtlas + ".prefab");
            if (atlas != null)
            {
                atlas.Init();
                Sprite sp = atlas.GetSprite(Skill.IconSprite);
                if(sp != null)
                {
                    _icon = sp.texture;
                }
            }
        }
    }

    public class TextEditWin : EditorWindow
    {
        private static Skill Skill;
        private GUIStyle _style;
        private static string _content;
        public static void Prepare(Skill skill)
        {
            Skill = skill;
            //_content = skill.Description;
        }
        void OnEnable()
        {
            _style = new GUIStyle(GUI.skin.textField);
            _style.wordWrap = true;
        }
        void OnGUI()
        {
            _content = EditorGUILayout.TextField(_content, _style, GUILayout.Height(120));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("应用"))
            {
                //Skill.Description = _content;
                Close();
            }
            if (GUILayout.Button("取消"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public class IconEditWin : EditorWindow
    {
        private static Skill Skill;
        private static Action OnChanged;
        private static GUI_Atlas _curAtlas;
        private static GameObject _atlasObject;
        private static Sprite _icon;
        private static int _curSelected = -1;
        private static GUIContent[] _contents;
        public static void Prepare(Skill skill, Action onChanged)
        {
            Skill = skill;
            OnChanged += onChanged;
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/GUI/UIAtlas/" + Skill.IconAtlas + ".prefab");
            OnSelectAtlas(obj);
        }
        void OnGUI()
        {
            GameObject curObject = _atlasObject;
            curObject = EditorGUILayout.ObjectField(curObject, typeof(GameObject)) as GameObject;
            if (_atlasObject != curObject && OnSelectAtlas(curObject))
            {
                _atlasObject = curObject;
                return;
            }
            if (_contents == null)
            {
                return;
            }
            int preSelected = _curSelected;
            _curSelected = GUILayout.SelectionGrid(_curSelected, _contents, _contents.Length);
            if (_curSelected != preSelected)
            {
                Skill.IconAtlas = _atlasObject.name;
                Skill.IconSprite = _curAtlas._SpriteList[_curSelected].name;
            }
        }

        static bool OnSelectAtlas(GameObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!ReloadAtlas(obj.GetComponent<GUI_Atlas>()))
            {
                return false;
            }
            _atlasObject = obj;
            return true;
        }

        static bool ReloadAtlas(GUI_Atlas atlas)
        {
            if(atlas == null)
            {
                return false;
            }

            _curAtlas = atlas;
            _icon = _curAtlas.GetSprite(Skill.IconSprite);
            _curSelected = _curAtlas._SpriteList.FindIndex((Sprite s) => s == _icon);
            int count = _curAtlas._SpriteList.Count;
            _contents = new GUIContent[count];
            for (int i = 0; i < count; ++i)
            {
                _contents[i] = new GUIContent(_curAtlas._SpriteList[i].texture);
            }
            return true;
        }

        void OnDestroy()
        {
            if(OnChanged != null)
            {
                OnChanged();
            }

            Skill = null;
            OnChanged = null;
            _curAtlas = null;
            _atlasObject = null;
            _icon = null;
            _curSelected = -1;
            _contents = null;
        }
    }
}