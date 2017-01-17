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
    public class BuffStateNode : SkillNodeBase
    {
        private static Dictionary<BuffStateType, System.Type> classTypeMap;

        private BuffStateType stateTypeRecord;

        public BuffStateMeta Meta
        {
            get
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                return (BuffStateMeta)wrapper.Meta;
            }
            set
            {
                WrapperMeta wrapper = MetaData as WrapperMeta;
                wrapper.Meta = value;
            }
        }

        public BuffStateNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.BuffState, new Color(0, 1, 0.5f, 1)) 
        {
            InitClassTypeMap();
        }

        protected override void Draw()
        {
            EditorGUIUtility.labelWidth = 56;

            BeginResizeHeight();
            Meta.Type = (BuffStateType)EditorGUILayout.EnumPopup("效果", Meta.Type, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            AddLine();
            
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
                case BuffStateType.Attribute:
                    Meta.UpdateValue(StateAttributeMeta.FieldType, (ActorField)EditorGUILayout.EnumPopup("属性", (ActorField)Meta.GetIntValue(StateAttributeMeta.FieldType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    DrawValue();
                    AddLine();
                    break;
                case BuffStateType.BuffTime:
                    Meta.UpdateValue(StateBuffTimeMeta.BuffType, (BuffType)EditorGUILayout.EnumPopup("Buff类型", (BuffType)Meta.GetIntValue(StateBuffTimeMeta.BuffType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(StateBuffTimeMeta.Time, EditorGUILayout.FloatField("时间", Meta.GetFloatValue(StateBuffTimeMeta.Time), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(2);
                    break;
                case BuffStateType.CubeSkill:
                    Meta.UpdateValue(StateCubeSkillMeta.SkillId, EditorGUILayout.TextField("技能ID", Meta.GetStringValue(StateCubeSkillMeta.SkillId), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case BuffStateType.CubeStrategy:
                    Meta.UpdateValue(StateCubeStrategyMeta.StrategyType, (CubeStrategyType)EditorGUILayout.EnumPopup("方块策略", (CubeStrategyType)Meta.GetIntValue(StateCubeStrategyMeta.StrategyType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case BuffStateType.Shield:
                    DrawValue();
                    break;
                case BuffStateType.EffectSize:
                case BuffStateType.CureReduced:
                case BuffStateType.ThornsDamage:
                case BuffStateType.ShareDamage:
                case BuffStateType.AdditionalDodge:
                case BuffStateType.SpeedCut:
                    Meta.UpdateValue(StateRateMeta.Rate, EditorGUILayout.FloatField("数值", Meta.GetFloatValue(StateRateMeta.Rate), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
                case BuffStateType.AdditionalDamage:
                    Meta.UpdateValue(StateAdditionalDamageMeta.NatureEx, (Nature)EditorGUILayout.EnumPopup(new GUIContent("性质", "攻击属性：物理 or 魔法"), (Nature)Meta.GetIntValue(StateAdditionalDamageMeta.NatureEx), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    Meta.UpdateValue(StateAdditionalDamageMeta.Factor, EditorGUILayout.FloatField("数值", Meta.GetFloatValue(StateAdditionalDamageMeta.Factor), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine(2);
                    break;
                case BuffStateType.CantKnockback:
                case BuffStateType.CantMove:
                case BuffStateType.Invinible:
                case BuffStateType.Stun:
                case BuffStateType.ImmuneStun:
                case BuffStateType.ImmuneSpeedCut:
                    break;
                case BuffStateType.ImmuneBuff:
                    Meta.UpdateValue(StateImmuneBuffMeta.BuffType, (BuffType)EditorGUILayout.EnumPopup("Buff类型", (BuffType)Meta.GetIntValue(StateImmuneBuffMeta.BuffType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                    AddLine();
                    break;
            }
        }

        public void InitClassTypeMap()
        {
            if (classTypeMap == null)
            {
                classTypeMap = new Dictionary<BuffStateType, System.Type>();

                classTypeMap.Add(BuffStateType.Attribute, typeof(StateAttributeMeta));
                classTypeMap.Add(BuffStateType.EffectSize, typeof(StateEffectSizeMeta));
                classTypeMap.Add(BuffStateType.BuffTime, typeof(StateBuffTimeMeta));

                classTypeMap.Add(BuffStateType.CubeSkill, typeof(StateCubeSkillMeta));
                classTypeMap.Add(BuffStateType.CubeStrategy, typeof(StateCubeStrategyMeta));

                classTypeMap.Add(BuffStateType.Shield, typeof(StateShieldMeta));
                classTypeMap.Add(BuffStateType.CureReduced, typeof(StateCureReducedMeta));
                classTypeMap.Add(BuffStateType.ThornsDamage, typeof(StateThornsDamageMeta));
                classTypeMap.Add(BuffStateType.ShareDamage, typeof(StateShareDamageMeta));
                classTypeMap.Add(BuffStateType.AdditionalDamage, typeof(StateAdditionalDamageMeta));
                classTypeMap.Add(BuffStateType.AdditionalDodge, typeof(StateAdditionalDodgeMeta));

                classTypeMap.Add(BuffStateType.CantKnockback, typeof(StateCantKnockbackMeta));
                classTypeMap.Add(BuffStateType.CantMove, typeof(StateCantMoveMeta));
                classTypeMap.Add(BuffStateType.ImmuneBuff, typeof(StateImmuneBuffMeta));
                classTypeMap.Add(BuffStateType.Invinible, typeof(StateInvinibleMeta));
                classTypeMap.Add(BuffStateType.Stun, typeof(StateStunMeta));
                classTypeMap.Add(BuffStateType.ImmuneStun, typeof(StateImmuneStunMeta));
                classTypeMap.Add(BuffStateType.SpeedCut, typeof(StateSpeedCutMeta));
                classTypeMap.Add(BuffStateType.ImmuneSpeedCut, typeof(StateImmuneSpeedCutMeta));
            }
        }

        public void ConvertClassType()
        {
            System.Type type = classTypeMap[Meta.Type];
            if (stateTypeRecord != Meta.Type && !type.IsInstanceOfType(Meta))
            {
                Meta = (BuffStateMeta)type.Assembly.CreateInstance(type.FullName);
                RecordBuffType();
            }
        }

        public void DrawValue()
        {
            BUFF.ValueType preValueType = (BUFF.ValueType)Meta.GetIntValue(StateValueMeta.ValueType);
            Meta.UpdateValue(StateValueMeta.ValueType, (BUFF.ValueType)EditorGUILayout.EnumPopup("数值类型", (BUFF.ValueType)Meta.GetIntValue(StateValueMeta.ValueType), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
            Meta.UpdateValue(StateValueMeta.Value, EditorGUILayout.FloatField("数值", Meta.GetFloatValue(StateValueMeta.Value), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
            ConvertValueType(preValueType);
            AddLine(2);
            if ((BUFF.ValueType)Meta.GetIntValue(StateValueMeta.ValueType) == BUFF.ValueType.Factor)
            {
                Meta.UpdateValue(StateValueMeta.BaseCamp, (Camp)EditorGUILayout.EnumPopup("基准阵营", (Camp)Meta.GetIntValue(StateValueMeta.BaseCamp), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                Meta.UpdateValue(StateValueMeta.BaseTarget, (Target)EditorGUILayout.EnumPopup("基准目标", (Target)Meta.GetIntValue(StateValueMeta.BaseTarget), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                Meta.UpdateValue(StateValueMeta.BaseField, (ActorField)EditorGUILayout.EnumPopup(new GUIContent("基准属性", "以哪个角色属性数值为换算基准"), (ActorField)Meta.GetIntValue(StateValueMeta.BaseField), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                Meta.UpdateValue(StateValueMeta.BasePoint, (AttributePoint)EditorGUILayout.EnumPopup(new GUIContent("属性点", "面板属性或当前属性"), (AttributePoint)Meta.GetIntValue(StateValueMeta.BasePoint), GUILayout.MaxWidth(SkillEditor.Width_Enum)));
                AddLine(4);
            }
        }

        public void ConvertValueType(BUFF.ValueType preValueType)
        {
            BUFF.ValueType currentValueType = (BUFF.ValueType)Meta.GetIntValue(StateValueMeta.ValueType);
            if (currentValueType != preValueType)
            {
                if (currentValueType == BUFF.ValueType.Number)
                {
                    Meta.TryRemoveValue(StateValueMeta.BaseCamp);
                    Meta.TryRemoveValue(StateValueMeta.BaseTarget);
                    Meta.TryRemoveValue(StateValueMeta.BaseField);
                    Meta.TryRemoveValue(StateValueMeta.BasePoint);
                }
                else
                {
                    Meta.TryAddValue(StateValueMeta.BaseCamp, Camp.Comrade);
                    Meta.TryAddValue(StateValueMeta.BaseTarget, Target.Caster);
                    Meta.TryAddValue(StateValueMeta.BaseField, ActorField.HP);
                    Meta.TryAddValue(StateValueMeta.BasePoint, AttributePoint.Current);
                }
            }
        }
        
        public override void OnCreated()
        {
            RecordBuffType();
        }

        public void RecordBuffType()
        {
            stateTypeRecord = Meta.Type;
        }
    }
}
