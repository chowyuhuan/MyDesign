using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;
using BUFF;
using UnityEditor;

namespace SKILL_EDITOR
{
    public class BuffConditionNode : SkillNodeBase
    {
        private static Dictionary<BuffConditionType, System.Type> classTypeMap;

        private BuffConditionType buffTypeRecord;

        private SkillNodeBase[] subConditionNode;

        public BuffConditionMeta Meta
        {
            get
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                return (BuffConditionMeta)wrapper.Meta;
            }
            set
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                wrapper.Meta = value;
            }
        }

        public BuffConditionNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.BuffCondition, new Color(0.5f, 0.5f, 1, 1)) 
        {
            InitClassTypeMap();
            subConditionNode = new SkillNodeBase[0];
        }

        protected override void Draw()
        {
            EditorGUIUtility.labelWidth = 56;

            BeginResizeHeight();

            Meta.Type = (BuffConditionType)EditorGUILayout.EnumPopup("类别", Meta.Type, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            AddLine();

            ConvertClassType();
            DrawDetail();

            EndResizeHeight();
        }

        public void DrawDetail()
        {
            if (Meta.HaveValue(CondTargetMeta.Camp))
            {
                Meta.UpdateValue(CondTargetMeta.Camp, (Camp)EditorGUILayout.EnumPopup("阵营", (Camp)Meta.GetIntValue(CondTargetMeta.Camp), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                Meta.UpdateValue(CondTargetMeta.Target, (Target)EditorGUILayout.EnumPopup("目标", (Target)Meta.GetIntValue(CondTargetMeta.Target), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                AddLine(2);
            }

            switch (Meta.Type)
            {
                case BuffConditionType.Cube:
                    Meta.UpdateValue(CondCubeMeta.CubeEraseType, (CubeEraseType)EditorGUILayout.EnumPopup("消除类型", (CubeEraseType)Meta.GetIntValue(CondCubeMeta.CubeEraseType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    if ((CubeEraseType)Meta.GetIntValue(CondCubeMeta.CubeEraseType) == CubeEraseType.Total)
                    {
                        Meta.UpdateValue(CondCubeMeta.Count, EditorGUILayout.IntField("累计消数", Meta.GetIntValue(CondCubeMeta.Count), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                        AddLine();
                    }
                    break;
                case BuffConditionType.Random:
                    Meta.UpdateValue(CondRandomMeta.Probability, EditorGUILayout.FloatField("几率", Meta.GetFloatValue(CondRandomMeta.Probability), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case BuffConditionType.Time:
                    Meta.UpdateValue(CondTimeMeta.Time, EditorGUILayout.FloatField("时间", Meta.GetFloatValue(CondTimeMeta.Time), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case BuffConditionType.Times:
                    Meta.UpdateValue(CondTimesMeta.Times, EditorGUILayout.IntField("次数", Meta.GetIntValue(CondTimesMeta.Times), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case BuffConditionType.Attribute:
                    BUFF.ValueType preValueType = (BUFF.ValueType)Meta.GetIntValue(CondAttributeMeta.ValueType);
                    Meta.UpdateValue(CondAttributeMeta.FieldType, (ActorField)EditorGUILayout.EnumPopup("属性", (ActorField)Meta.GetIntValue(CondAttributeMeta.FieldType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(CondAttributeMeta.Compare, (CompareOperation)EditorGUILayout.EnumPopup("运算", (CompareOperation)Meta.GetIntValue(CondAttributeMeta.Compare), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(CondAttributeMeta.ValueType, (BUFF.ValueType)EditorGUILayout.EnumPopup("数值类型", (BUFF.ValueType)Meta.GetIntValue(CondAttributeMeta.ValueType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(CondAttributeMeta.Value, EditorGUILayout.FloatField("数值", Meta.GetFloatValue(CondAttributeMeta.Value), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    ConvertAttributeValueType(preValueType);
                    AddLine(4);
                    if ((BUFF.ValueType)Meta.GetIntValue(CondAttributeMeta.ValueType) == BUFF.ValueType.Factor)
                    {
                        Meta.UpdateValue(CondAttributeMeta.BaseCamp, (Camp)EditorGUILayout.EnumPopup("基准阵营", (Camp)Meta.GetIntValue(CondAttributeMeta.BaseCamp), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                        Meta.UpdateValue(CondAttributeMeta.BaseTarget, (Target)EditorGUILayout.EnumPopup("基准目标", (Target)Meta.GetIntValue(CondAttributeMeta.BaseTarget), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                        Meta.UpdateValue(CondAttributeMeta.BaseField, (ActorField)EditorGUILayout.EnumPopup(new GUIContent("基准属性", "以哪个角色属性数值为换算基准"), (ActorField)Meta.GetIntValue(CondAttributeMeta.BaseField), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                        AddLine(3);
                    }
                    break;
                case BuffConditionType.Energy:
                    Meta.UpdateValue(CondEnergyMeta.Compare, (CompareOperation)EditorGUILayout.EnumPopup("运算", (CompareOperation)Meta.GetIntValue(CondEnergyMeta.Compare), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(CondEnergyMeta.Value, EditorGUILayout.FloatField("数值", Meta.GetFloatValue(CondEnergyMeta.Value), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(2);
                    break;
                case BuffConditionType.AttackBegin:
                case BuffConditionType.AttackEnd:
                case BuffConditionType.Attack:
                case BuffConditionType.Crit:
                case BuffConditionType.Dodge:
                case BuffConditionType.Hit:
                case BuffConditionType.Parry:
                case BuffConditionType.Cure:
                case BuffConditionType.BeAttack:
                case BuffConditionType.BeCrit:
                case BuffConditionType.BeDodge:
                case BuffConditionType.BeHit:
                case BuffConditionType.BeParry:
                case BuffConditionType.BeCure:
                    Meta.UpdateValue(CondAttackMeta.SkillId, EditorGUILayout.TextField("技能ID", Meta.GetStringValue(CondAttackMeta.SkillId)));
                    Meta.UpdateValue(CondAttackMeta.HaveTargetCondition, EditorGUILayout.Toggle("条件目标", Meta.GetBoolValue(CondAttackMeta.HaveTargetCondition)));
                    AddLine(2);
                    break;
                case BuffConditionType.HaveBuff:
                case BuffConditionType.BuffEnd:
                    Meta.UpdateValue(CondHaveBuffMeta.BuffType, (BuffType)EditorGUILayout.EnumPopup("Buff类型", (BuffType)Meta.GetIntValue(CondHaveBuffMeta.BuffType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(CondHaveBuffMeta.BuffId, EditorGUILayout.TextField("Buff ID", Meta.GetStringValue(CondHaveBuffMeta.BuffId)));
                    AddLine(2);
                    break;
                case BuffConditionType.BuffOverlay:
                    Meta.UpdateValue(CondBuffOverlayMeta.BuffType, (BuffType)EditorGUILayout.EnumPopup("Buff类型", (BuffType)Meta.GetIntValue(CondBuffOverlayMeta.BuffType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(CondBuffOverlayMeta.BuffId, EditorGUILayout.TextField("Buff ID", Meta.GetStringValue(CondBuffOverlayMeta.BuffId)));
                    Meta.UpdateValue(CondBuffOverlayMeta.Layer, EditorGUILayout.IntField("层数", Meta.GetIntValue(CondBuffOverlayMeta.Layer), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(3);
                    break;
                case BuffConditionType.False:
                case BuffConditionType.True:
                    break;
                case BuffConditionType.OpAnd:
                case BuffConditionType.OpOr:
                case BuffConditionType.OpNot:
                case BuffConditionType.OpLoop:
                    break;
                case BuffConditionType.OpTimeInterval:
                    Meta.UpdateValue(CondOpTimeIntervalMeta.Time, EditorGUILayout.FloatField("间隔时间", Meta.GetFloatValue(CondOpTimeIntervalMeta.Time), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case BuffConditionType.OpTrueTimes:
                    Meta.UpdateValue(CondOpTrueTimesMeta.Times, EditorGUILayout.IntField("次数", Meta.GetIntValue(CondOpTrueTimesMeta.Times), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
            }

            CreateTargetConditionNode(Meta);
        }

        public void InitClassTypeMap()
        {
            if (classTypeMap == null)
            {
                classTypeMap = new Dictionary<BuffConditionType, System.Type>();

                classTypeMap.Add(BuffConditionType.Cube, typeof(CondCubeMeta));

                classTypeMap.Add(BuffConditionType.Random, typeof(CondRandomMeta));

                classTypeMap.Add(BuffConditionType.Time, typeof(CondTimeMeta));
                classTypeMap.Add(BuffConditionType.Times, typeof(CondTimesMeta));

                classTypeMap.Add(BuffConditionType.Attribute, typeof(CondAttributeMeta));
                classTypeMap.Add(BuffConditionType.Energy, typeof(CondEnergyMeta));

                classTypeMap.Add(BuffConditionType.AttackBegin, typeof(CondAttackBeginMeta));
                classTypeMap.Add(BuffConditionType.AttackEnd, typeof(CondAttackEndMeta));

                classTypeMap.Add(BuffConditionType.Attack, typeof(CondAttackMeta));
                classTypeMap.Add(BuffConditionType.Crit, typeof(CondCritMeta));
                classTypeMap.Add(BuffConditionType.Dodge, typeof(CondDodgeMeta));
                classTypeMap.Add(BuffConditionType.Hit, typeof(CondHitMeta));
                classTypeMap.Add(BuffConditionType.Parry, typeof(CondParryMeta));
                classTypeMap.Add(BuffConditionType.Cure, typeof(CondCureMeta));

                classTypeMap.Add(BuffConditionType.BeAttack, typeof(CondBeAttackMeta));
                classTypeMap.Add(BuffConditionType.BeCrit, typeof(CondBeCritMeta));
                classTypeMap.Add(BuffConditionType.BeDodge, typeof(CondBeDodgeMeta));
                classTypeMap.Add(BuffConditionType.BeHit, typeof(CondBeHitMeta));
                classTypeMap.Add(BuffConditionType.BeParry, typeof(CondBeParryMeta));
                classTypeMap.Add(BuffConditionType.BeCure, typeof(CondBeCureMeta));

                classTypeMap.Add(BuffConditionType.HaveBuff, typeof(CondHaveBuffMeta));
                classTypeMap.Add(BuffConditionType.BuffEnd, typeof(CondBuffEndMeta));
                classTypeMap.Add(BuffConditionType.BuffOverlay, typeof(CondBuffOverlayMeta));

                classTypeMap.Add(BuffConditionType.False, typeof(CondFalseMeta));
                classTypeMap.Add(BuffConditionType.True, typeof(CondTrueMeta));

                classTypeMap.Add(BuffConditionType.OpAnd, typeof(CondOpAndMeta));
                classTypeMap.Add(BuffConditionType.OpNot, typeof(CondOpNotMeta));
                classTypeMap.Add(BuffConditionType.OpOr, typeof(CondOpOrMeta));
                classTypeMap.Add(BuffConditionType.OpTimeInterval, typeof(CondOpTimeIntervalMeta));
                classTypeMap.Add(BuffConditionType.OpTrueTimes, typeof(CondOpTrueTimesMeta));
                classTypeMap.Add(BuffConditionType.OpLoop, typeof(CondOpLoopMeta));
            }
        }

        public void ConvertClassType()
        {
            System.Type type = classTypeMap[Meta.Type];
            if (buffTypeRecord != Meta.Type && !type.IsInstanceOfType(Meta))
            {
                // Record target
                bool haveTarget = Meta.HaveValue(CondTargetMeta.Camp);
                Camp camp = Camp.Comrade;
                Target target = Target.Caster;
                if (haveTarget)
                {
                    camp = (Camp)Meta.GetIntValue(CondTargetMeta.Camp);
                    target = (Target)Meta.GetIntValue(CondTargetMeta.Target);
                }

                Meta = (BuffConditionMeta)type.Assembly.CreateInstance(type.FullName);
                InitSubConditionNode();
                RecordBuffType();

                // Set target
                if (Meta.HaveValue(CondTargetMeta.Camp) && haveTarget)
                {
                    Meta.UpdateValue(CondTargetMeta.Camp, camp);
                    Meta.UpdateValue(CondTargetMeta.Target, target);
                }
            }
        }

        public void ConvertAttributeValueType(BUFF.ValueType preValueType)
        {
            BUFF.ValueType currentValueType = (BUFF.ValueType)Meta.GetIntValue(CondAttributeMeta.ValueType);
            if (currentValueType != preValueType)
            {
                if (currentValueType == BUFF.ValueType.Number)
                {
                    Meta.TryRemoveValue(CondAttributeMeta.BaseCamp);
                    Meta.TryRemoveValue(CondAttributeMeta.BaseTarget);
                    Meta.TryRemoveValue(CondAttributeMeta.BaseField);
                }
                else
                {
                    Meta.TryAddValue(CondAttributeMeta.BaseCamp, Camp.Comrade);
                    Meta.TryAddValue(CondAttributeMeta.BaseTarget, Target.Caster);
                    Meta.TryAddValue(CondAttributeMeta.BaseField, ActorField.HP);
                }
            }
        }

        public bool IsAttackMetaNode(BuffConditionMeta meta)
        {
            switch (meta.Type)
            {
                case BuffConditionType.AttackBegin:
                case BuffConditionType.AttackEnd:
                case BuffConditionType.Attack:
                case BuffConditionType.Crit:
                case BuffConditionType.Dodge:
                case BuffConditionType.Hit:
                case BuffConditionType.Parry:
                case BuffConditionType.Cure:
                case BuffConditionType.BeAttack:
                case BuffConditionType.BeCrit:
                case BuffConditionType.BeDodge:
                case BuffConditionType.BeHit:
                case BuffConditionType.BeParry:
                case BuffConditionType.BeCure:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnCreated()
        {
            RecordBuffType();
            InitSubConditionNode(); 
        }

        public void RecordBuffType()
        {
            buffTypeRecord = Meta.Type;
        }

        public void CreateTargetConditionNode(BuffConditionMeta Meta)
        {
            if (IsAttackMetaNode(Meta))
            {
                if (Meta.GetBoolValue(CondAttackMeta.HaveTargetCondition))
                {
                    if (Meta.SubCondition.Count == 0)
                    {
                        Meta.CreateSubCondition(1);
                        ClearSubConditionNode();
                        CreateSubConditionNode(Node.BuffConditionTarget, ref Meta.SubCondition);
                    }
                }
                else
                {
                    if (Meta.SubCondition.Count != 0)
                    {
                        Meta.CreateSubCondition(0);
                        ClearSubConditionNode();
                    }
                }
            }
        }

        public void InitSubConditionNode()
        {
            ClearSubConditionNode();

            if (IsAttackMetaNode(Meta))
            {
                CreateSubConditionNode(Node.BuffConditionTarget, ref Meta.SubCondition);
            }
            else
            {
                CreateSubConditionNode(Node.BuffCondition, ref Meta.SubCondition);
            }
        }

        public void CreateSubConditionNode(Node idx, ref List<BuffConditionMeta> meta)
        {
            subConditionNode = new SkillNodeBase[meta.Count];

            for (int i = 0; i < meta.Count; i++)
            {
                subConditionNode[i] = CreateChild(idx, new ListWrapper<BuffConditionMeta>(meta, meta[i]), false);
            }
        }

        public void ClearSubConditionNode()
        {
            for (int i = 0; i < subConditionNode.Length; i++)
            {
                RemoveChild(subConditionNode[i]);
            }
            subConditionNode = new SkillNodeBase[0];
        }

        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            if (idx == Node.BuffCondition)
            {
                return CreateChildWithData<BuffConditionNode, WrapperMeta>("条件", new Vector2(150, 60), data, archive);
            }
            else if (idx == Node.BuffConditionTarget)
            {
                return CreateChildWithData<BuffConditionTargetNode, WrapperMeta>("条件目标", new Vector2(150, 60), data, archive);
            }
            return null;
        }
    }

    public enum BuffConditionTargetType
    {
        Cube = BuffConditionType.Cube,

        Attribute = BuffConditionType.Attribute,

        AttackEnd = BuffConditionType.AttackEnd,

        Attack = BuffConditionType.Attack,
        Crit = BuffConditionType.Crit,
        Dodge = BuffConditionType.Dodge,
        Hit = BuffConditionType.Hit,
        Parry = BuffConditionType.Parry,
        Cure = BuffConditionType.Cure,

        BeAttack = BuffConditionType.BeAttack,

        Buff = BuffConditionType.HaveBuff,
        BuffEnd = BuffConditionType.BuffEnd,
        BuffOverlay = BuffConditionType.BuffOverlay,
    }

    public class BuffConditionTargetNode : BuffConditionNode
    {
        public BuffConditionTargetNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size) 
        {
            BackgroundColor = new Color(0.54f, 0.16f, 0.88f, 1);
        }

        private BuffConditionTargetType conditionTargetType;

        protected override void Draw()
        {
            EditorGUIUtility.labelWidth = 56;

            BeginResizeHeight();

            conditionTargetType = (BuffConditionTargetType)((int)Meta.Type);
            conditionTargetType = (BuffConditionTargetType)EditorGUILayout.EnumPopup("类别", conditionTargetType, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            Meta.Type = (BuffConditionType)((int)conditionTargetType);
            AddLine();

            ConvertClassType();
            DrawDetail();

            EndResizeHeight();
        }
    }

    public class BuffConditionRedNode : BuffConditionNode
    {
        public BuffConditionRedNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size) 
        {
            BackgroundColor = new Color(1f, 0.5f, 0.3f, 1);
        }
    }

    public class BuffConditionGreenNode : BuffConditionNode
    {
        public BuffConditionGreenNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size) 
        {
            BackgroundColor = new Color(0.5f, 1f, 0f, 1);
        }
    }
}
