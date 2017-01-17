using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using BUFF;
using ACTOR;
using UnityEditor;

namespace SKILL_EDITOR
{

    public sealed class AttackMetaNode : SkillNodeBase
    {
        public AttackMetaNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.ATK, new Color(1, 0.7f, 0, 1)) { }
        protected override void Draw()
        {
            AttackMeta Meta = MetaData as AttackMeta;
            BeginResizeHeight();
            EditorGUIUtility.labelWidth = 28;
            Meta.MoveCasting = EditorGUILayout.Toggle(new GUIContent("移动", "施法过程中伴随移动"), Meta.MoveCasting);
            AddLine();
            if(Meta.MoveCasting)
            {
                Meta.Speed = EditorGUILayout.DelayedFloatField(new GUIContent("速度", "移动速度"), Meta.Speed, GUILayout.MaxWidth(SkillEditor.Width_Float));
                Meta.Distance = EditorGUILayout.DelayedFloatField(new GUIContent("距离", "移动多远距离"), Meta.Distance, GUILayout.MaxWidth(SkillEditor.Width_Float));
                AddLine(2);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 攻击", SkillEditorUtility.LeftButton))
            {
                CreateChild(Node.DC);
            }
            if (GUILayout.Button("-", SkillEditorUtility.MidButton))
            {
                TryToDestroy();
            }
            //if (GUILayout.Button("Buff +", SkillEditorUtility.RightButton))
            //{
            //    CreateChild(Node.Buff);
            //}
            if (GUILayout.Button("行为 +", SkillEditorUtility.RightButton))
            {
                BehaviorGenerateCubeMeta generateCubeMeta = new BehaviorGenerateCubeMeta();
                Meta.Behaviors.Add(generateCubeMeta);
                CreateChild(Node.TriggerBehavior, new ListWrapper<TriggerBehaviorMeta>(Meta.Behaviors, generateCubeMeta), false);
            }
            EditorGUILayout.EndHorizontal();
            AddLine();
            EndResizeHeight();
        }
        public override void OnCreated()
        {
            AttackMeta Meta = MetaData as AttackMeta;
            DCMeta[] dcs = Meta.DCs;
            if (dcs != null && dcs.Length > 0)
            {
                for (int j = 0; j < dcs.Length; ++j)
                {
                    CreateChild(Node.DC, dcs[j], false);
                }
            }

            for (int i = 0; i < Meta.Behaviors.Count; i++)
            {
                CreateChild(Node.TriggerBehavior, new ListWrapper<TriggerBehaviorMeta>(Meta.Behaviors, Meta.Behaviors[i]), false);
            }
        }
        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            AttackMeta Meta = MetaData as AttackMeta;
            if (idx == Node.DC)
            {
                return CreateChildWithData<DCMetaNode, DCMeta>("攻击/治疗", new Vector2(150, 240), ref Meta.DCs, data, archive);
            }
            else if (idx == Node.TriggerBehavior)
            {
                return CreateChildWithData<TriggerBehaviorNode, WrapperMeta>("行为", new Vector2(150, 95), data, archive);
            }
            return null;
        }
        protected override void RemoveChildImp(SkillNodeBase node)
        {
            AttackMeta Meta = MetaData as AttackMeta;
            if (node.Tag == Node.DC)
            {
                RemoveOneData<DCMeta>(ref Meta.DCs, ref node.MetaData);
            }
            else if (node.Tag == Node.TriggerBehavior)
            {
                WrapperMeta Wrapper = node.MetaData as WrapperMeta;
                Meta.Behaviors.Remove((TriggerBehaviorMeta)Wrapper.Meta);
            }
        }
    }


}