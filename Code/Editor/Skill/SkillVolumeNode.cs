using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;
using UnityEditor;

namespace SKILL_EDITOR
{
    public sealed class VolumeNode : SkillNodeBase
    {
        public VolumeNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.VolAtk, new Color(0.5f, 0, 0.5f)) { }
        protected override void Draw()
        {
            if (GUILayout.Button("攻击 +"))
            {
                CreateChild(Node.VolAtk);
            }
            if (GUILayout.Button("属性 +"))
            {
                CreateChild(Node.VolField);
            }
        }

        protected override SkillNodeBase CreateChildImp(Node idx, object data, bool archive)
        {
            Volume Meta = MetaData as Volume;
            if (idx == Node.VolAtk)
            {
                return CreateChildWithData<ATKVolumeNode, Volume.ATK>("数值:攻击", new Vector2(100, 130), ref Meta.Atks, data, archive);
            }
            else if (idx == Node.VolField)
            {
                return CreateChildWithData<FieldVolumeNode, Volume.Field>("数值:属性", new Vector2(100, 130), ref Meta.Fields, data, archive);
            }
            return null;
        }
        protected override void RemoveChildImp(SkillNodeBase node)
        {
            Volume Meta = MetaData as Volume;
            if (node.Tag == Node.VolAtk)
            {
                RemoveOneData<Volume.ATK>(ref Meta.Atks, ref node.MetaData);
            }
            else if (node.Tag == Node.VolField)
            {
                RemoveOneData<Volume.Field>(ref Meta.Fields, ref node.MetaData);
            }
        }
    }

    public class VolumeSubNodeBase : SkillNodeBase
    {
        public VolumeSubNodeBase(SkillNodeBase parent, int layer, string title, Vector2 size, Node tag, Color color) : base(parent, layer, title, size, tag, color) { }
        protected override void Draw() { }
        protected void DrawNumeric(NumericMeta num)
        {
            if (num == null)
            {
                return;
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
                meta.BaseEx = (NumericMeta.Base)EditorGUILayout.EnumPopup(new GUIContent("基准", "以谁的属性为基准：施法者 or 被攻击者"), meta.BaseEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < num.Nums.Count; ++i)
            {
                NumericMeta.ValueUnit meta = num.Nums[i];
                meta.BaseField = (ActorField)EditorGUILayout.EnumPopup(new GUIContent("属性", "以哪个角色属性数值为换算基准"), meta.BaseField, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < num.Nums.Count; ++i)
            {
                NumericMeta.ValueUnit meta = num.Nums[i];
                meta.OpEx = (Op)EditorGUILayout.EnumPopup(new GUIContent("运算", "属性与数值如何运算（Assign时，基准、属性无效，直接取数值）"), meta.OpEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
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
    }

    public sealed class ATKVolumeNode : VolumeSubNodeBase
    {
        public ATKVolumeNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.VolAtk, Color.black) { }
        protected override void Draw()
        {
            Volume.ATK Meta = MetaData as Volume.ATK;
            EditorGUIUtility.labelWidth = 28;
            Meta.NatureEx = (Nature)EditorGUILayout.EnumPopup(new GUIContent("性质", "攻击属性：物理 or 魔法"), Meta.NatureEx, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            DrawNumeric(Meta.Meta);

            if (GUILayout.Button("-"))
            {
                TryToDestroy();
            }
        }
        public override void OnCreated()
        {
            Volume.ATK Meta = MetaData as Volume.ATK;
            if (Meta.Meta.Nums.Count <= 0)
            {
                Meta.Meta.Nums.Add(new NumericMeta.ValueUnit(NumericMeta.Base.Caster, ActorField.ATK, Op.Multiply, 0.1f)); // 数值默认创建一个
            }
        }
    }

    public sealed class FieldVolumeNode : VolumeSubNodeBase
    {
        public FieldVolumeNode(SkillNodeBase parent, int layer, string title, Vector2 size) : base(parent, layer, title, size, Node.VolField, Color.black) { }
        protected override void Draw()
        {
            Volume.Field Meta = MetaData as Volume.Field;
            EditorGUIUtility.labelWidth = 28;
            Meta.FieldType = (ActorField)EditorGUILayout.EnumPopup("属性", Meta.FieldType, GUILayout.MaxWidth(SkillEditor.Width_Enum));
            DrawNumeric(Meta.Meta);

            if (GUILayout.Button("-"))
            {
                TryToDestroy();
            }
        }
        public override void OnCreated()
        {
            Volume.Field Meta = MetaData as Volume.Field;
            if (Meta.Meta.Nums.Count <= 0)
            {
                Meta.Meta.Nums.Add(new NumericMeta.ValueUnit(NumericMeta.Base.Caster, ActorField.ATK, Op.Multiply, 0.1f)); // 数值默认创建一个
            }
        }
    }


}