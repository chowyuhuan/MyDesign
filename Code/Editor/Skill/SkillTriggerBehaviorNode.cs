using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;
using BUFF;
using UnityEditor;
using EVENT;

namespace SKILL_EDITOR
{
    public class TriggerBehaviorNode : SkillNodeBase
    {
        public const string EVENT_REMOVE_BEHAVIOR_BUFF = "event_remove_behavior_buff";
        public const string EVENT_REMOVE_BEHAVIOR_TRIGGER = "event_remove_behavior_trigger";

        private static EventDispatcher eventDispatcher = new EventDispatcher();

        private static Dictionary<TriggerBehaviorType, System.Type> classTypeMap;

        private TriggerBehaviorType behaviorTypeRecord;

        public TriggerBehaviorMeta Meta
        {
            get
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                return (TriggerBehaviorMeta)wrapper.Meta;
            }
            set
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                wrapper.Meta = value;
            }
        }

        private SkillNodeBase behaviorBuffNode;
        private SkillNodeBase behaviorTriggerNode;

        public TriggerBehaviorNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.TriggerBehavior, new Color(1, 0.5f, 1, 1)) 
        {
            InitClassTypeMap();
        }

        protected override void Draw()
        {
            EditorGUIUtility.labelWidth = 56;

            BeginResizeHeight();
            Meta.CampEx = (Camp)EditorGUILayout.EnumPopup("阵营", Meta.CampEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.TargetEx = (Target)EditorGUILayout.EnumPopup("目标", Meta.TargetEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.Type = (TriggerBehaviorType)EditorGUILayout.EnumPopup("行为", Meta.Type, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            AddLine(3);
            
            ConvertClassType();
            DrawDetail();

            if (GUILayout.Button("-", SkillEditorUtility.MidButton))
            {
                TryToDestroy();
            }
            AddLine();

            EndResizeHeight();
        }

        public void DrawDetail()
        {
            switch (Meta.Type)
            {
                case TriggerBehaviorType.GenerateCube:
                    break;
                case TriggerBehaviorType.ChangeCubeSkill:
                    Meta.UpdateValue(BehaviorChangeCubeSkillMeta.SkillId, EditorGUILayout.TextField("技能ID", Meta.GetStringValue(BehaviorChangeCubeSkillMeta.SkillId), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case TriggerBehaviorType.CastSkill:
                    Meta.UpdateValue(BehaviorCastSkillMeta.SkillId, EditorGUILayout.TextField("技能ID", Meta.GetStringValue(BehaviorCastSkillMeta.SkillId), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorCastSkillMeta.Priority, (SkillPriority)EditorGUILayout.EnumPopup("类型", (SkillPriority)Meta.GetIntValue(BehaviorCastSkillMeta.Priority), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(2);
                    break;
                case TriggerBehaviorType.StopSkill:
                    break;
                case TriggerBehaviorType.AddTrigger:
                    break;
                case TriggerBehaviorType.AddBuff:
                    break;
                case TriggerBehaviorType.ClearBuff:
                    Meta.UpdateValue(BehaviorClearBuffMeta.BuffType, (BuffType)EditorGUILayout.EnumPopup("类型", (BuffType)Meta.GetIntValue(BehaviorClearBuffMeta.BuffType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorClearBuffMeta.BuffId, EditorGUILayout.TextField("Buff ID", Meta.GetStringValue(BehaviorClearBuffMeta.BuffId), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(2);
                    break;
                case TriggerBehaviorType.Damage:
                case TriggerBehaviorType.Cure:
                    Meta.UpdateValue(BehaviorVolumeAtkMeta.NatureEx, (Nature)EditorGUILayout.EnumPopup(new GUIContent("性质", "攻击属性：物理 or 魔法"), (Nature)Meta.GetIntValue(BehaviorVolumeAtkMeta.NatureEx), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorVolumeAtkMeta.BaseEx, (NumericMeta.Base)EditorGUILayout.EnumPopup(new GUIContent("基准", "以谁的属性为基准：施法者 or 被攻击者"), (NumericMeta.Base)Meta.GetIntValue(BehaviorVolumeAtkMeta.BaseEx), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorVolumeAtkMeta.BaseField, (ActorField)EditorGUILayout.EnumPopup(new GUIContent("属性", "以哪个角色属性数值为换算基准"), (ActorField)Meta.GetIntValue(BehaviorVolumeAtkMeta.BaseField), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorVolumeAtkMeta.OpEx, (Op)EditorGUILayout.EnumPopup(new GUIContent("运算", "属性与数值如何运算（Assign时，基准、属性无效，直接取数值）"), (Op)Meta.GetIntValue(BehaviorVolumeAtkMeta.OpEx), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorVolumeAtkMeta.Factor, EditorGUILayout.DelayedFloatField("数值", Meta.GetFloatValue(BehaviorVolumeAtkMeta.Factor), GUILayout.MaxWidth(SkillEditor.Width_Int)));
                    AddLine(5);
                    break;
                case TriggerBehaviorType.Energy:
                    Meta.UpdateValue(BehaviorEnergyMeta.Energy, EditorGUILayout.IntField("能量", Meta.GetIntValue(BehaviorEnergyMeta.Energy), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case TriggerBehaviorType.Dodge:
                case TriggerBehaviorType.Parry:
                    break;
                case TriggerBehaviorType.ChangeCondition:
                    Meta.UpdateValue(BehaviorChangeConditionMeta.ChangeCondType, (ChangeConditionType)EditorGUILayout.EnumPopup("修改类型", (ChangeConditionType)Meta.GetIntValue(BehaviorChangeConditionMeta.ChangeCondType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorChangeConditionMeta.CondType, (BuffConditionType)EditorGUILayout.EnumPopup("条件类型", (BuffConditionType)Meta.GetIntValue(BehaviorChangeConditionMeta.CondType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(2);
                    DrawBehaviorChangeCondition();
                    break;
            }
        }

        private ChangeConditionType changeCondTypeRecord;
        private BuffConditionType condTypeRecord;

        private void DrawBehaviorChangeCondition()
        {
            ChangeConditionType changeCondType = (ChangeConditionType)Meta.GetIntValue(BehaviorChangeConditionMeta.ChangeCondType);

            if (changeCondTypeRecord != changeCondType)
            {
                BehaviorChangeConditionMeta.ConvertChangeCondType(Meta, changeCondType);
                changeCondTypeRecord = changeCondType;
            }

            switch (changeCondType)
            {
                case ChangeConditionType.BuffLifeCycle:
                    Meta.UpdateValue(BehaviorChangeConditionMeta.BuffType, (BuffType)EditorGUILayout.EnumPopup("Buff类型", (BuffType)Meta.GetIntValue(BehaviorChangeConditionMeta.BuffType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(BehaviorChangeConditionMeta.BuffId, EditorGUILayout.TextField("Buff ID", Meta.GetStringValue(BehaviorChangeConditionMeta.BuffId), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(2);
                    break;
                case ChangeConditionType.TriggerLifeCycle:
                case ChangeConditionType.TriggerCondition:
                    Meta.UpdateValue(BehaviorChangeConditionMeta.TriggerTag, EditorGUILayout.TextField("Trigger", Meta.GetStringValue(BehaviorChangeConditionMeta.TriggerTag), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
            }

            BuffConditionType condType = (BuffConditionType)Meta.GetIntValue(BehaviorChangeConditionMeta.CondType);

            if (condTypeRecord != condType)
            {
                BehaviorChangeConditionMeta.ConvertCondType(Meta, condType);
                condTypeRecord = condType;
            }

            switch (condType)
            {
                case BuffConditionType.Time:
                    Meta.UpdateValue(BehaviorChangeConditionMeta.Time, EditorGUILayout.FloatField("时间", Meta.GetFloatValue(BehaviorChangeConditionMeta.Time), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
            }
        }

        public void InitClassTypeMap()
        {
            if (classTypeMap == null)
            {
                classTypeMap = new Dictionary<TriggerBehaviorType, System.Type>();

                classTypeMap.Add(TriggerBehaviorType.GenerateCube, typeof(BehaviorGenerateCubeMeta));
                classTypeMap.Add(TriggerBehaviorType.ChangeCubeSkill, typeof(BehaviorChangeCubeSkillMeta));

                classTypeMap.Add(TriggerBehaviorType.CastSkill, typeof(BehaviorCastSkillMeta));
                classTypeMap.Add(TriggerBehaviorType.StopSkill, typeof(BehaviorStopSkillMeta));

                classTypeMap.Add(TriggerBehaviorType.AddTrigger, typeof(BehaviorAddTriggerMeta));
                classTypeMap.Add(TriggerBehaviorType.AddBuff, typeof(BehaviorAddBuffMeta));
                classTypeMap.Add(TriggerBehaviorType.ClearBuff, typeof(BehaviorClearBuffMeta));

                classTypeMap.Add(TriggerBehaviorType.Damage, typeof(BehaviorDamageMeta));
                classTypeMap.Add(TriggerBehaviorType.Cure, typeof(BehaviorCureMeta));
                classTypeMap.Add(TriggerBehaviorType.Energy, typeof(BehaviorEnergyMeta));
                classTypeMap.Add(TriggerBehaviorType.Dodge, typeof(BehaviorDodgeMeta));
                classTypeMap.Add(TriggerBehaviorType.Parry, typeof(BehaviorParryMeta));

                classTypeMap.Add(TriggerBehaviorType.ChangeCondition, typeof(BehaviorChangeConditionMeta));
            }
        }

        public void ConvertClassType()
        {
            System.Type type = classTypeMap[Meta.Type];
            if (behaviorTypeRecord != Meta.Type && !type.IsInstanceOfType(Meta))
            {
                // Record target
                Camp camp = Meta.CampEx;
                Target target = Meta.TargetEx;

                Meta = (TriggerBehaviorMeta)type.Assembly.CreateInstance(type.FullName);
                RebuildBehaviorAddBuff();
                RebuildBehaviorAddTrigger();
                RecordBuffType();

                // Set target
                Meta.CampEx = camp;
                Meta.TargetEx = target;
            }
        }
        
        public override void OnCreated()
        {
            InitBehaviorAddBuff();
            InitBehaviorAddTrigger();
            InitBehaviorChangeCondition();
            RecordBuffType();
        }

        public void RecordBuffType()
        {
            behaviorTypeRecord = Meta.Type;
        }

        private void InitBehaviorChangeCondition()
        {
            if (Meta.Type == TriggerBehaviorType.ChangeCondition)
            {
                changeCondTypeRecord = (ChangeConditionType)Meta.GetIntValue(BehaviorChangeConditionMeta.ChangeCondType);
                condTypeRecord = (BuffConditionType)Meta.GetIntValue(BehaviorChangeConditionMeta.CondType);
            }
        }

        private void InitBehaviorAddBuff()
        {
            if (Meta.Type == TriggerBehaviorType.AddBuff)
            {
                Skill skill = GetRoot().MetaData as Skill;
                int index = Meta.GetIntValue(BehaviorAddBuffMeta.BuffIndex);
                behaviorBuffNode = CreateChild(Node.Buff, skill.Buffs[index], false);
                eventDispatcher.AddListener(EVENT_REMOVE_BEHAVIOR_BUFF, RedirectBuffIndex);
            }
        }

        private void RebuildBehaviorAddBuff()
        {
            if (Meta.Type == TriggerBehaviorType.AddBuff)
            {
                AddBuffNode();
            }
            else if (behaviorTypeRecord == TriggerBehaviorType.AddBuff)
            {
                behaviorBuffNode.Destroy();
            }
        }

        private void AddBuffNode()
        {
            Skill skill = GetRoot().MetaData as Skill;
            NewBuffMeta buffMeta = new NewBuffMeta();
            skill.Buffs.Add(buffMeta);
            behaviorBuffNode = CreateChild(Node.Buff, buffMeta, false);
            Meta.UpdateValue(BehaviorAddBuffMeta.BuffIndex, skill.Buffs.Count - 1);

            eventDispatcher.AddListener(EVENT_REMOVE_BEHAVIOR_BUFF, RedirectBuffIndex);
        }

        private void RemoveBuffNode()
        {
            Skill skill = GetRoot().MetaData as Skill;
            skill.Buffs.Remove(behaviorBuffNode.MetaData as NewBuffMeta);

            eventDispatcher.RemoveListener(EVENT_REMOVE_BEHAVIOR_BUFF, RedirectBuffIndex);
            eventDispatcher.Dispatch(EVENT_REMOVE_BEHAVIOR_BUFF, skill);
        }

        private void RedirectBuffIndex(params object[] args)
        {
            if (Meta.Type == TriggerBehaviorType.AddBuff)
            {
                Skill skill = GetRoot().MetaData as Skill;
                if (skill.Equals(args[0]))
                {
                    Meta.UpdateValue(BehaviorAddBuffMeta.BuffIndex, skill.Buffs.IndexOf(behaviorBuffNode.MetaData as NewBuffMeta));
                }
            }
        }

        private void InitBehaviorAddTrigger()
        {
            if (Meta.Type == TriggerBehaviorType.AddTrigger)
            {
                Skill skill = GetRoot().MetaData as Skill;
                int index = Meta.GetIntValue(BehaviorAddTriggerMeta.TriggerIndex);
                behaviorTriggerNode = CreateChild(Node.Trigger, skill.Triggers[index], false);
                eventDispatcher.AddListener(EVENT_REMOVE_BEHAVIOR_TRIGGER, RedirectTriggerIndex);
            }
        }

        private void RebuildBehaviorAddTrigger()
        {
            if (Meta.Type == TriggerBehaviorType.AddTrigger)
            {
                AddTriggerNode();
            }
            else if (behaviorTypeRecord == TriggerBehaviorType.AddTrigger)
            {
                behaviorTriggerNode.Destroy();
            }
        }

        private void AddTriggerNode()
        {
            Skill skill = GetRoot().MetaData as Skill;
            NewTriggerMeta triggerMeta = new NewTriggerMeta();
            skill.Triggers.Add(triggerMeta);
            behaviorTriggerNode = CreateChild(Node.Trigger, triggerMeta, false);
            Meta.UpdateValue(BehaviorAddTriggerMeta.TriggerIndex, skill.Triggers.Count - 1);

            eventDispatcher.AddListener(EVENT_REMOVE_BEHAVIOR_TRIGGER, RedirectTriggerIndex);
        }

        private void RemoveTriggerNode()
        {
            Skill skill = GetRoot().MetaData as Skill;
            skill.Triggers.Remove(behaviorTriggerNode.MetaData as NewTriggerMeta);

            eventDispatcher.RemoveListener(EVENT_REMOVE_BEHAVIOR_TRIGGER, RedirectTriggerIndex);
            eventDispatcher.Dispatch(EVENT_REMOVE_BEHAVIOR_TRIGGER, skill);
        }

        private void RedirectTriggerIndex(params object[] args)
        {
            if (Meta.Type == TriggerBehaviorType.AddTrigger)
            {
                Skill skill = GetRoot().MetaData as Skill;
                if (skill.Equals(args[0]))
                {
                    Meta.UpdateValue(BehaviorAddTriggerMeta.TriggerIndex, skill.Triggers.IndexOf(behaviorTriggerNode.MetaData as NewTriggerMeta));
                }
            }
        }


        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            if (idx == Node.Buff)
            {
                return CreateChildWithData<UndeletableNewBuffMetaNode, NewBuffMeta>("Buff", new Vector2(150, 134), data, archive);
            }
            if (idx == Node.Trigger)
            {
                return CreateChildWithData<UndeletableNewTriggerMetaNode, NewTriggerMeta>("触发器", new Vector2(130, 56), data, archive);
            }
            return null;
        }

        protected override void RemoveChildImp(SkillNodeBase node)
        {
            if (node.Tag == Node.Buff)
            {
                RemoveBuffNode();
            }
            else if (node.Tag == Node.Trigger)
            {
                RemoveTriggerNode();
            }
        }
    }

    public class TriggerBehaviorForkNode : SkillNodeBase
    {
        public TriggerBehaviorForkMeta Meta
        {
            get
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                return (TriggerBehaviorForkMeta)wrapper.Meta;
            }
            set
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                wrapper.Meta = value;
            }
        }

        public TriggerBehaviorForkNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.TriggerBehaviorFork, new Color(1, 0.5f, 1, 1)) { }

        protected override void Draw()
        {
            EditorGUIUtility.labelWidth = 56;

            BeginResizeHeight();
            Meta.Weight = EditorGUILayout.IntField("权重", Meta.Weight, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            AddLine();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", SkillEditorUtility.MidButton))
            {
                TryToDestroy();
            }
            if (GUILayout.Button("行为 +", SkillEditorUtility.RightButton))
            {
                BehaviorGenerateCubeMeta generateCubeMeta = new BehaviorGenerateCubeMeta();
                Meta.BehaviorFork.Add(generateCubeMeta);
                CreateChild(Node.TriggerBehavior, new ListWrapper<TriggerBehaviorMeta>(Meta.BehaviorFork, generateCubeMeta), false);
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void OnCreated()
        {
            if (Meta.BehaviorFork != null)
            {
                for (int i = 0; i < Meta.BehaviorFork.Count; ++i)
                {
                    CreateChild(Node.TriggerBehavior, new ListWrapper<TriggerBehaviorMeta>(Meta.BehaviorFork, Meta.BehaviorFork[i]), false);
                }
            }
        }

        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            if (idx == Node.TriggerBehavior)
            {
                return CreateChildWithData<TriggerBehaviorNode, WrapperMeta>("行为", new Vector2(150, 95), data, archive);
            }
            return null;
        }

        protected override void RemoveChildImp(SkillNodeBase node)
        {
            if (node.Tag == Node.TriggerBehavior)
            {
                WrapperMeta Wrapper = node.MetaData as WrapperMeta;
                Meta.BehaviorFork.Remove((TriggerBehaviorMeta)Wrapper.Meta);
            }
        }
    }
}
