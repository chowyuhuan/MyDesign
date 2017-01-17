using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;
using BUFF;
using UnityEditor;

namespace SKILL_EDITOR
{
    public class NewTriggerMetaNode : SkillNodeBase
    {
        public NewTriggerMetaNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.Trigger, Color.yellow) { }

        protected override void Draw()
        {
            NewTriggerMeta Meta = MetaData as NewTriggerMeta;
            EditorGUIUtility.labelWidth = 36;

            Meta.Tag = EditorGUILayout.TextField("标签", Meta.Tag, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            AddLine();

            EditorGUILayout.BeginHorizontal();
            if (CanDeleteSelf())
            {
                if (GUILayout.Button("-", SkillEditorUtility.MidButton))
                {
                    TryToDestroy();
                }
            }
            if (GUILayout.Button("行为分支 +", SkillEditorUtility.RightButton))
            {
                TriggerBehaviorForkMeta behaviorForkMeta = new TriggerBehaviorForkMeta();
                Meta.BehaviorForkList.Add(behaviorForkMeta);
                CreateChild(Node.TriggerBehaviorFork, new ListWrapper<TriggerBehaviorForkMeta>(Meta.BehaviorForkList, behaviorForkMeta), false);
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void OnCreated()
        {
            NewTriggerMeta Meta = MetaData as NewTriggerMeta;

            CreateChild(Node.BuffConditionRed, new ObjectWrapper<BuffConditionWrapperMeta>(Meta.LifeCycle, "ConditionMeta"), false);
            CreateChild(Node.BuffConditionGreen, new ObjectWrapper<BuffConditionWrapperMeta>(Meta.Condition, "ConditionMeta"), false);

            if (Meta.BehaviorForkList != null)
            {
                for (int i = 0; i < Meta.BehaviorForkList.Count; ++i)
                {
                    CreateChild(Node.TriggerBehaviorFork, new ListWrapper<TriggerBehaviorForkMeta>(Meta.BehaviorForkList, Meta.BehaviorForkList[i]), false);
                }
            }
        }

        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            if (idx == Node.BuffConditionRed)
            {
                return CreateChildWithData<BuffConditionRedNode, WrapperMeta>("生命周期", new Vector2(150, 60), data, archive);
            }
            else if (idx == Node.BuffConditionGreen)
            {
                return CreateChildWithData<BuffConditionGreenNode, WrapperMeta>("触发条件", new Vector2(150, 60), data, archive);
            }
            else if (idx == Node.TriggerBehaviorFork)
            {
                return CreateChildWithData<TriggerBehaviorForkNode, WrapperMeta>("行为分支", new Vector2(150, 60), data, archive);
            }
            
            return null;
        }

        protected override void RemoveChildImp(SkillNodeBase node)
        {
            NewTriggerMeta Meta = MetaData as NewTriggerMeta;

            if (node.Tag == Node.TriggerBehaviorFork)
            {
                WrapperMeta Wrapper = node.MetaData as WrapperMeta;
                Meta.BehaviorForkList.Remove((TriggerBehaviorForkMeta)Wrapper.Meta);
            }
        }

        protected virtual bool CanDeleteSelf()
        {
            return true;
        }
    }

    public class UndeletableNewTriggerMetaNode : NewTriggerMetaNode
    {
        public UndeletableNewTriggerMetaNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size) { }

        protected override bool CanDeleteSelf()
        {
            return false;
        }
    }
}
