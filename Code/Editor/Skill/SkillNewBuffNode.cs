using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;
using BUFF;
using UnityEditor;

namespace SKILL_EDITOR
{
    public class NewBuffMetaNode : SkillNodeBase
    {
        private ObjectToStringField effect = new ObjectToStringField();

        public NewBuffMetaNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.Buff, Color.cyan) { }

        protected override void Draw()
        {
            NewBuffMeta Meta = MetaData as NewBuffMeta;
            EditorGUIUtility.labelWidth = 56;

            //Meta.CampEx = (Camp)EditorGUILayout.EnumPopup("阵营", Meta.CampEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            //Meta.TargetEx = (Target)EditorGUILayout.EnumPopup("目标", Meta.TargetEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.BuffId = EditorGUILayout.TextField("ID", Meta.BuffId);
            Meta.BuffTypeEx = (BuffType)EditorGUILayout.EnumPopup("类型", Meta.BuffTypeEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.Effect = effect.ObjectField();
            Meta.Removable = EditorGUILayout.Toggle("可以移除", Meta.Removable);
            Meta.Limit = EditorGUILayout.IntField("可叠层数", Meta.Limit);

            EditorGUILayout.BeginHorizontal();
            if (CanDeleteSelf())
            {
                if (GUILayout.Button("-", SkillEditorUtility.MidButton))
                {
                    TryToDestroy();
                }
            }
            if (GUILayout.Button("状态 +", SkillEditorUtility.RightButton))
            {
                StateAttributeMeta attributeMeta = new StateAttributeMeta();
                Meta.States.Add(attributeMeta);
                CreateChild(Node.BuffState, new ListWrapper<BuffStateMeta>(Meta.States, attributeMeta), false);
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void OnCreated()
        {
            NewBuffMeta Meta = MetaData as NewBuffMeta;

            CreateChild(Node.BuffConditionRed, new ObjectWrapper<BuffConditionWrapperMeta>(Meta.LifeCycle, "ConditionMeta"), false);

            if (Meta.States != null)
            {
                for (int i = 0; i < Meta.States.Count; ++i)
                {
                    CreateChild(Node.BuffState, new ListWrapper<BuffStateMeta>(Meta.States, Meta.States[i]), false);
                }
            }
        }

        protected override void Init()
        {
            NewBuffMeta Meta = MetaData as NewBuffMeta;

            string path = "Assets/Resources/" + AssetManage.AM_PathHelper.GetActorEffectFullPathByName(Meta.Effect) + ".prefab";
            effect.Init(new GUIContent("特效"), path, typeof(GameObject), false);
        }

        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            if (idx == Node.BuffState)
            {
                return CreateChildWithData<BuffStateNode, WrapperMeta>("状态", new Vector2(150, 60), data, archive);
            }
            if (idx == Node.BuffConditionRed)
            {
                return CreateChildWithData<BuffConditionRedNode, WrapperMeta>("生命周期", new Vector2(150, 60), data, archive);
            }
            return null;
        }

        protected override void RemoveChildImp(SkillNodeBase node)
        {
            NewBuffMeta Meta = MetaData as NewBuffMeta;

            if (node.Tag == Node.BuffState)
            {
                WrapperMeta Wrapper = node.MetaData as WrapperMeta;
                Meta.States.Remove((BuffStateMeta)Wrapper.Meta);
            }
        }

        protected virtual bool CanDeleteSelf()
        {
            return true;
        }
    }

    public class UndeletableNewBuffMetaNode : NewBuffMetaNode
    {
        public UndeletableNewBuffMetaNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size) { }

        protected override bool CanDeleteSelf()
        {
            return false;
        }
    }
}
