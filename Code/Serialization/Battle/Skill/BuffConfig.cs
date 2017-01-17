using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using SKILL;

namespace BUFF
{
    /// <summary>
    /// Buff类型 增益 减益 特殊 隐藏（比如追3类）
    /// </summary>
    public enum BuffType
    {
        Buff,
        DeBuff,
        Special,
        Hidden,
    }

    /// <summary>
    /// 消除方块类型 任意消 1消 2消 3消 特殊技能 消块计数
    /// </summary>
    public enum CubeEraseType
    {
        Any = 0,
        Single = 1,
        Double = 2,
        Triple = 3,
        Special = 4,
        Total = 9,
    }

    /// <summary>
    /// 方块策略 1消视为2消 1消视为3消 2消视为3消 任意消视为3消
    /// </summary>
    public enum CubeStrategyType
    {
        SingleToDouble,
        SingleToTriple,
        DoubleToTriple,
        AnyToTriple,
    }

    /// <summary>
    /// 数值类型 值 系数
    /// </summary>
    public enum ValueType
    {
        Number,
        Factor,
    }

    /// <summary>
    /// 比较运算
    /// </summary>
    public enum CompareOperation
    {
        LessThan,
        LessThanOrEqualTo,
        EqualTo,
        NotEqualTo,
        GreaterThanOrEqualTo,
        GreaterThan
    }

    /// <summary>
    /// 修改条件类型 buff生命周期 trigger生命周期 trigger触发条件
    /// </summary>
    public enum ChangeConditionType
    {
        BuffLifeCycle,
        TriggerLifeCycle,
        TriggerCondition,
    }

    /// <summary>
    /// 技能释放优先级
    /// </summary>
    public enum SkillPriority
    {
        Low,
        High,
    }

    /// <summary>
    /// 属性点 瞬时属性 面板属性
    /// </summary>
    public enum AttributePoint
    {
        Current,
        Panel,
    }

    /// <summary>
    /// 序列化字典类
    /// </summary>
    [System.Serializable]
    public class SerializableDictionaryMeta : MetaBase, ISerializationCallbackReceiver
    {
        private Dictionary<string, string> dict;

        public List<string> Keys;
        public List<string> Values;

        public SerializableDictionaryMeta()
        {
            dict = new Dictionary<string, string>();
        }

        public SerializableDictionaryMeta(SerializableDictionaryMeta meta)
        {
            dict = new Dictionary<string, string>(meta.dict);
        }

        public bool HaveValue(string key)
        {
            return dict.ContainsKey(key);
        }

        public void TryAddValue(string key, object value)
        {
            if (!dict.ContainsKey(key))
            {
                ConvertEnumToInt(ref value);
                dict.Add(key, value.ToString());
            }
        }

        public void TryRemoveValue(string key)
        {
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
            }
        }

        public void UpdateValue(string key, object value)
        {
            if (dict.ContainsKey(key))
            {
                ConvertEnumToInt(ref value);
                dict[key] = value.ToString();
            }
        }

        private void ConvertEnumToInt(ref object value)
        {
            if (value.GetType().IsEnum)
            {
                value = Convert.ToInt32(value);
            }
        }

        public string GetValue(string key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return null; 
        }

        public string GetStringValue(string key)
        {
            string result = GetValue(key);
            return result == null ? "" : result;
        }

        public int GetIntValue(string key)
        {
            string result = GetValue(key);
            return result == null ? 0 : Convert.ToInt32(result);
        }

        public float GetFloatValue(string key)
        {
            string result = GetValue(key);
            return result == null ? 0 : Convert.ToSingle(result);
        }

        public bool GetBoolValue(string key)
        {
            string result = GetValue(key);
            return result == null ? false : result == "True";
        }

        public void OnBeforeSerialize()
        {
            Keys = new List<string>(dict.Count);
            Values = new List<string>(dict.Count);
            foreach (var pair in dict)
            {
                Keys.Add(pair.Key);
                Values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dict.Clear();
            int count = Mathf.Min(Keys.Count, Values.Count);
            for (int i = 0; i < count; ++i)
            {
                dict.Add(Keys[i], Values[i]);
            }
            Keys = null;
            Values = null;
        }

        public override MetaBase DeepClone()
        {
            return new SerializableDictionaryMeta(this);
        }
    }

    /// <summary>
    /// 对象引用包装基类
    /// </summary>
    public class WrapperMeta : MetaBase
    {
        public virtual MetaBase Meta { get; set; }

        public WrapperMeta() { }

        public override MetaBase DeepClone()
        {
            return new WrapperMeta();
        } 
    }

    /// <summary>
    /// Object包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectWrapper<T> : WrapperMeta where T : MetaBase
    {
        private MetaBase meta;
        private string propertyName;

        public override MetaBase Meta
        {
            get { return (MetaBase)typeof(T).GetProperty(propertyName).GetValue(meta, null); }
            set { typeof(T).GetProperty(propertyName).SetValue(meta, value, null); }
        }

        public ObjectWrapper() { }

        public ObjectWrapper(MetaBase meta, string propertyName)
        {
            this.meta = meta;
            this.propertyName = propertyName;
        }

        public ObjectWrapper(ObjectWrapper<T> wrapper)
        {
            meta = wrapper.meta;
        }

        public override MetaBase DeepClone()
        {
            return new ObjectWrapper<T>(this);
        }
    }

    /// <summary>
    /// List包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListWrapper<T> : WrapperMeta where T : MetaBase
    {
        private List<T> list;
        private T item;

        public override MetaBase Meta
        {
            get { return item; }
            set { list[list.IndexOf(item)] = (T)value; item = (T)value; }
        }

        public ListWrapper(List<T> list, T t)
        {
            this.list = list;
            this.item = t;
        }

        public ListWrapper(ListWrapper<T> wrapper)
        {
            list = wrapper.list;
            item = wrapper.item;
        }

        public override MetaBase DeepClone()
        {
            return new ListWrapper<T>(this);
        }
    }

    /// <summary>
    /// Buff数据
    /// </summary>
    [System.Serializable]
    public class NewBuffMeta : MetaBase
    {
        [Header("Common")]
        public string BuffId;
        public BuffType BuffTypeEx;

        [Header("Effect")]
        public string Effect;

        [Header("LifeCycle")]
        public bool Removable = true;
        public BuffConditionWrapperMeta LifeCycle;

        [Header("Overlay")]
        public int Limit = 1;

        [Header("State")]
        public List<BuffStateMeta> States;

        public NewBuffMeta()
        {
            LifeCycle = new BuffConditionWrapperMeta();
            States = new List<BuffStateMeta>();
        }

        public NewBuffMeta(NewBuffMeta meta)
        {
            BuffId = meta.BuffId;
            BuffTypeEx = meta.BuffTypeEx;
            Effect = meta.Effect;
            LifeCycle = meta.LifeCycle.DeepClone() as BuffConditionWrapperMeta;
            Removable = meta.Removable;
            Limit = meta.Limit;

            if (meta.States != null)
            {
                States = new List<BuffStateMeta>();
                for (int i = 0; i < meta.States.Count; i++)
                {
                    States.Add(meta.States[i].DeepClone() as BuffStateMeta);
                }
            }
        }

        public override MetaBase DeepClone()
        {
            return new NewBuffMeta(this);
        }
    }

    /// <summary>
    /// Trigger数据
    /// </summary>
    [System.Serializable]
    public class NewTriggerMeta : MetaBase
    {
        public string Tag;

        [Header("LifeCycle")]
        public BuffConditionWrapperMeta LifeCycle;

        [Header("Condition")]
        public BuffConditionWrapperMeta Condition;

        [Header("Behavior")]
        public List<TriggerBehaviorForkMeta> BehaviorForkList;

        public NewTriggerMeta()
        {
            Tag = "";
            LifeCycle = new BuffConditionWrapperMeta();
            Condition = new BuffConditionWrapperMeta();
            BehaviorForkList = new List<TriggerBehaviorForkMeta>();
        }

        public NewTriggerMeta(NewTriggerMeta meta)
        {
            Tag = meta.Tag;
            LifeCycle = meta.LifeCycle.DeepClone() as BuffConditionWrapperMeta;
            Condition = meta.Condition.DeepClone() as BuffConditionWrapperMeta;

            BehaviorForkList = new List<TriggerBehaviorForkMeta>();
            for (int i = 0; i < meta.BehaviorForkList.Count; i++)
            {
                BehaviorForkList.Add((TriggerBehaviorForkMeta)meta.BehaviorForkList[i].DeepClone());
            }
        }

        public override MetaBase DeepClone()
        {
            return new NewTriggerMeta(this);
        }
    }

    /// <summary>
    /// 行为分支
    /// </summary>
    [System.Serializable]
    public class TriggerBehaviorForkMeta : MetaBase
    {
        public int Weight;
            
        public List<TriggerBehaviorMeta> BehaviorFork;

        public TriggerBehaviorForkMeta()
        {
            BehaviorFork = new List<TriggerBehaviorMeta>();
        }

        public TriggerBehaviorForkMeta(TriggerBehaviorForkMeta meta)
        {
            Weight = meta.Weight;

            BehaviorFork = new List<TriggerBehaviorMeta>();
            for (int i = 0; i < meta.BehaviorFork.Count; i++)
            {
                BehaviorFork.Add((TriggerBehaviorMeta)meta.BehaviorFork[i].DeepClone());
            }
        }

        public override MetaBase DeepClone()
        {
            return new TriggerBehaviorForkMeta(this);
        }
    }

    /// <summary>
    /// 行为
    /// </summary>
    public enum TriggerBehaviorType
    {
        GenerateCube = 0,
        ChangeCubeSkill = 1,

        CastSkill = 10,
        StopSkill = 11,

        AddTrigger = 20,
        AddBuff = 21,
        ClearBuff = 22,

        Damage = 50,
        Cure = 51,
        Energy = 52,
        Dodge = 53,
        Parry = 54,

        ChangeCondition = 99,
    }

    /// <summary>
    /// 行为数据
    /// </summary>
    [System.Serializable]
    public class TriggerBehaviorMeta : SerializableDictionaryMeta
    {
        public Camp CampEx;
        public Target TargetEx;

        public TriggerBehaviorType Type;

        public TriggerBehaviorMeta() { }

        public TriggerBehaviorMeta(TriggerBehaviorMeta meta) : base(meta)
        {
            CampEx = meta.CampEx;
            TargetEx = meta.TargetEx;

            Type = meta.Type;
        }

        public override MetaBase DeepClone()
        {
            if (UnCloneable() && !MetaBase.isSkillMetaUseForCopy)
            {
                return new BehaviorGenerateCubeMeta();
            }
            else
            {
                return new TriggerBehaviorMeta(this);
            }
        } 

        /// <summary>
        /// Addbuff 和 AddTrigger 目前不支持克隆，因为存的是索引
        /// </summary>
        /// <returns></returns>
        public bool UnCloneable()
        {
            return Type == TriggerBehaviorType.AddBuff || Type == TriggerBehaviorType.AddTrigger;
        }
    }

    public class BehaviorGenerateCubeMeta : TriggerBehaviorMeta
    {
        public BehaviorGenerateCubeMeta()
        {
            Type = TriggerBehaviorType.GenerateCube;
        }
    }

    public class BehaviorChangeCubeSkillMeta : TriggerBehaviorMeta
    {
        public const string SkillId = "SkillId";

        public BehaviorChangeCubeSkillMeta()
        {
            Type = TriggerBehaviorType.ChangeCubeSkill;
            TryAddValue(SkillId, "");
        }
    }

    public class BehaviorCastSkillMeta : TriggerBehaviorMeta
    {
        public const string SkillId = "SkillId";
        public const string Priority = "Priority";

        public BehaviorCastSkillMeta()
        {
            Type = TriggerBehaviorType.CastSkill;
            TryAddValue(SkillId, "");
            TryAddValue(Priority, SkillPriority.Low);
        }
    }

    public class BehaviorStopSkillMeta : TriggerBehaviorMeta
    {
        public BehaviorStopSkillMeta()
        {
            Type = TriggerBehaviorType.StopSkill;
        }
    }

    public class BehaviorAddTriggerMeta : TriggerBehaviorMeta
    {
        public const string TriggerIndex = "TriggerIndex";

        public BehaviorAddTriggerMeta()
        {
            Type = TriggerBehaviorType.AddTrigger;
            TryAddValue(TriggerIndex, "");
        }
    }

    public class BehaviorAddBuffMeta : TriggerBehaviorMeta
    {
        public const string BuffIndex = "BuffIndex";

        public BehaviorAddBuffMeta()
        {
            Type = TriggerBehaviorType.AddBuff;
            TryAddValue(BuffIndex, "");
        }
    }

    public class BehaviorClearBuffMeta : TriggerBehaviorMeta
    {
        public const string BuffType = "Type";
        public const string BuffId = "Id";

        public BehaviorClearBuffMeta()
        {
            Type = TriggerBehaviorType.ClearBuff;
            TryAddValue(BuffType, BUFF.BuffType.Buff);
            TryAddValue(BuffId, "");
        }
    }

    public class BehaviorVolumeAtkMeta : TriggerBehaviorMeta
    {
        public const string NatureEx = "NatureEx";
        public const string BaseEx = "BaseEx";
        public const string BaseField = "BaseField";
        public const string OpEx = "OpEx";
        public const string Factor = "Factor";

        public BehaviorVolumeAtkMeta()
        {
            TryAddValue(NatureEx, Nature.Magic);
            TryAddValue(BaseEx, NumericMeta.Base.Caster);
            TryAddValue(BaseField, ActorField.HP);
            TryAddValue(OpEx, Op.Multiply);
            TryAddValue(Factor, 0.1f);
        }
    }

    public class BehaviorDamageMeta : BehaviorVolumeAtkMeta
    {
        public BehaviorDamageMeta()
        {
            Type = TriggerBehaviorType.Damage;
        }
    }

    public class BehaviorCureMeta : BehaviorVolumeAtkMeta
    {
        public BehaviorCureMeta()
        {
            Type = TriggerBehaviorType.Cure;
        }
    }

    public class BehaviorEnergyMeta : TriggerBehaviorMeta
    {
        public const string Energy = "Energy";

        public BehaviorEnergyMeta()
        {
            Type = TriggerBehaviorType.Energy;
            TryAddValue(Energy, "0");
        }
    }

    public class BehaviorDodgeMeta : TriggerBehaviorMeta
    {
        public BehaviorDodgeMeta()
        {
            Type = TriggerBehaviorType.Dodge;
        }
    }

    public class BehaviorParryMeta : TriggerBehaviorMeta
    {
        public BehaviorParryMeta()
        {
            Type = TriggerBehaviorType.Parry;
        }
    }

    public class BehaviorChangeConditionMeta : TriggerBehaviorMeta
    {
        public const string ChangeCondType = "Type";

        // Buff or trigger
        public const string BuffType = "BuffType";
        public const string BuffId = "BuffId";
        public const string TriggerTag = "TriggerTag";

        public const string CondType = "CondType";
        // CondTime
        public const string Time = "Time";

        public BehaviorChangeConditionMeta()
        {
            Type = TriggerBehaviorType.ChangeCondition;
            TryAddValue(ChangeCondType, ChangeConditionType.BuffLifeCycle);
            TryAddValue(CondType, BuffConditionType.Time);
            ConvertChangeCondType(this, ChangeConditionType.BuffLifeCycle);
            ConvertCondType(this, BuffConditionType.Time);
        }

        public static void ConvertChangeCondType(TriggerBehaviorMeta meta, ChangeConditionType changeCondType)
        {
            meta.TryRemoveValue(BuffType);
            meta.TryRemoveValue(BuffId);
            meta.TryRemoveValue(TriggerTag);

            switch (changeCondType)
            {
                case ChangeConditionType.BuffLifeCycle:
                    meta.TryAddValue(BuffType, BUFF.BuffType.Buff);
                    meta.TryAddValue(BuffId, "");
                    break;
                case ChangeConditionType.TriggerLifeCycle:
                case ChangeConditionType.TriggerCondition:
                    meta.TryAddValue(TriggerTag, "");
                    break;
            }
        }

        public static void ConvertCondType(TriggerBehaviorMeta meta, BuffConditionType condType)
        {
            meta.TryRemoveValue(Time);

            switch (condType)
            {
                case BuffConditionType.Time:
                    meta.TryAddValue(Time, 0);
                    break;
            }
        }
    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum BuffStateType
    {
        Attribute = 0,
        EffectSize = 1,
        BuffTime = 2,

        CubeSkill = 10,
        CubeStrategy = 11,

        Shield = 21,
        CureReduced = 22,
        ThornsDamage = 23,
        ShareDamage = 24,
        AdditionalDamage = 25,
        AdditionalDodge = 26,

        CantKnockback = 50,
        CantMove = 51,
        ImmuneBuff = 52,
        Invinible = 53,
        Stun = 54,
        ImmuneStun = 55,
        SpeedCut = 56,
        ImmuneSpeedCut = 57,
    }
 
    /// <summary>
    /// 状态数据
    /// </summary>
    [System.Serializable]
    public class BuffStateMeta : SerializableDictionaryMeta 
    {
        public BuffStateType Type;

        public BuffStateMeta() { }

        public BuffStateMeta(BuffStateMeta meta) : base(meta)
        {
            Type = meta.Type;
        }

        public override MetaBase DeepClone()
        {
            return new BuffStateMeta(this);
        } 
    }

    public class StateValueMeta : BuffStateMeta
    {
        public const string ValueType = "ValueType";
        public const string Value = "Value";

        public const string BaseCamp = "BaseCamp";
        public const string BaseTarget = "BaseTarget";
        public const string BaseField = "BaseField";
        public const string BasePoint = "BasePoint";

        public StateValueMeta()
        {
            TryAddValue(ValueType, BUFF.ValueType.Number);
            TryAddValue(Value, "0");
        }
    }

    public class StateAttributeMeta : StateValueMeta
    {
        public const string FieldType = "FieldType";

        public StateAttributeMeta()
        {
            Type = BuffStateType.Attribute;
            TryAddValue(FieldType, ActorField.HP);
        }
    }

    public class StateShieldMeta : StateValueMeta
    {
        public StateShieldMeta()
        {
            Type = BuffStateType.Shield;
        }
    }

    public class StateEffectSizeMeta : StateRateMeta
    {
        public StateEffectSizeMeta()
        {
            Type = BuffStateType.EffectSize;
        }
    }

    public class StateBuffTimeMeta : BuffStateMeta
    {
        public const string BuffType = "Type";
        public const string Time = "Time";

        public StateBuffTimeMeta()
        {
            Type = BuffStateType.BuffTime;
            TryAddValue(BuffType, BUFF.BuffType.Buff);
            TryAddValue(Time, "0");
        }
    }

    public class StateCubeSkillMeta : BuffStateMeta
    {
        public const string SkillId = "SkillId";

        public StateCubeSkillMeta()
        {
            Type = BuffStateType.CubeSkill;
            TryAddValue(SkillId, "");
        }
    }

    public class StateCubeStrategyMeta : BuffStateMeta
    {
        public const string StrategyType = "StrategyType";

        public StateCubeStrategyMeta()
        {
            Type = BuffStateType.CubeStrategy;
            TryAddValue(StrategyType, CubeStrategyType.SingleToTriple);
        }
    }

    public class StateRateMeta : BuffStateMeta
    {
        public const string Rate = "Rate";

        public StateRateMeta()
        {
            TryAddValue(Rate, "0");
        }
    }

    public class StateCureReducedMeta : StateRateMeta
    {
        public StateCureReducedMeta()
        {
            Type = BuffStateType.CureReduced;
        }
    }

    public class StateThornsDamageMeta : StateRateMeta
    {
        public StateThornsDamageMeta()
        {
            Type = BuffStateType.ThornsDamage;
        }
    }

    public class StateShareDamageMeta : StateRateMeta
    {
        public StateShareDamageMeta()
        {
            Type = BuffStateType.ShareDamage;
        }
    }

    public class StateAdditionalDamageMeta : BuffStateMeta
    {
        public const string NatureEx = "NatureEx";
        public const string Factor = "Factor";

        public StateAdditionalDamageMeta()
        {
            Type = BuffStateType.AdditionalDamage;
            TryAddValue(NatureEx, Nature.Magic);
            TryAddValue(Factor, 0.1f);
        }
    }

    public class StateAdditionalDodgeMeta : StateRateMeta
    {
        public StateAdditionalDodgeMeta()
        {
            Type = BuffStateType.AdditionalDodge;
        }
    }

    public class StateCantKnockbackMeta : BuffStateMeta
    {
        public StateCantKnockbackMeta()
        {
            Type = BuffStateType.CantKnockback;
        }
    }

    public class StateCantMoveMeta : BuffStateMeta
    {
        public StateCantMoveMeta()
        {
            Type = BuffStateType.CantMove;
        }
    }

    public class StateImmuneBuffMeta : BuffStateMeta
    {
        public const string BuffType = "Type";

        public StateImmuneBuffMeta()
        {
            Type = BuffStateType.ImmuneBuff;
            TryAddValue(BuffType, BUFF.BuffType.Buff);
        }
    }

    public class StateInvinibleMeta : BuffStateMeta
    {
        public StateInvinibleMeta()
        {
            Type = BuffStateType.Invinible;
        }
    }

    public class StateStunMeta : BuffStateMeta
    {
        public StateStunMeta()
        {
            Type = BuffStateType.Stun;
        }

    }

    public class StateImmuneStunMeta : BuffStateMeta
    {
        public StateImmuneStunMeta()
        {
            Type = BuffStateType.ImmuneStun;
        }
    }

    public class StateSpeedCutMeta : StateRateMeta
    {
        public StateSpeedCutMeta()
        {
            Type = BuffStateType.SpeedCut;
        }
    }

    public class StateImmuneSpeedCutMeta : BuffStateMeta
    {
        public StateImmuneSpeedCutMeta()
        {
            Type = BuffStateType.ImmuneSpeedCut;
        }
    }

    /// <summary>
    /// 条件
    /// </summary>
    public enum BuffConditionType
    {
        Cube = 0,

        Random = 1,

        Time = 2,
        Times = 3,

        Attribute = 10,
        Energy = 11,

        AttackBegin = 50,
        AttackEnd = 51,

        Attack = 100,
        Crit = 101,
        Dodge = 102,
        Hit = 103,
        Parry = 104,
        Cure = 105,

        BeAttack = 200,
        BeCrit = 201,
        BeDodge = 202,
        BeHit = 203,
        BeParry = 204,
        BeCure = 205,

        HaveBuff = 300,
        BuffEnd = 301,
        BuffOverlay = 302,

        False = 888,
        True = 899,

        OpAnd = 900,
        OpNot = 901,
        OpOr = 902,
        OpTimeInterval = 903,
        OpTrueTimes = 904,
        OpLoop = 905,
    }

    /// <summary>
    /// 条件包装类用于条件数据序列化与反序列化类
    /// </summary>
    [System.Serializable]
    public class BuffConditionWrapperMeta : MetaBase, ISerializationCallbackReceiver
    {
        //the field we give unity to serialize. 
        public List<SerializableBuffConditionMeta> CondList;

        //the field of what we use at runtime. not serialized.
        private BuffConditionMeta conditionMeta;

        public BuffConditionMeta ConditionMeta
        {
            get { return conditionMeta; }
            set { conditionMeta = value; }
        }

        public BuffConditionWrapperMeta()
        {
            conditionMeta = new CondCubeMeta();
        }

        public BuffConditionWrapperMeta(BuffConditionWrapperMeta meta)
        {
            conditionMeta = (BuffConditionMeta)meta.conditionMeta.DeepClone();
        }

        public override MetaBase DeepClone()
        {
            return new BuffConditionWrapperMeta(this);
        }

        public void OnBeforeSerialize()
        {
            //unity is about to read the serializedNodes field's contents. lets make sure
            //we write out the correct data into that field "just in time".
            CondList = new List<SerializableBuffConditionMeta>();
            AddNodeToSerializedNodes(conditionMeta);
        }

        void AddNodeToSerializedNodes(BuffConditionMeta meta)
        {
            var serializedNode = new SerializableBuffConditionMeta(meta)
            {
                Type = meta.Type,
                ChildCount = meta.SubCondition.Count,
                IndexOfFirstChild = CondList.Count + 1
            };
            CondList.Add(serializedNode);
            foreach (var child in meta.SubCondition)
                AddNodeToSerializedNodes(child);
        }

        public void OnAfterDeserialize()
        {
            //Unity has just written new data into the serializedNodes field.
            //let's populate our actual runtime data with those new values.
            if (CondList.Count > 0)
                conditionMeta = ReadNodeFromSerializedNodes(0);
            else
                conditionMeta = new BuffConditionMeta();
            CondList = null;
        }

        BuffConditionMeta ReadNodeFromSerializedNodes(int index)
        {
            var serializedNode = CondList[index];
            var children = new List<BuffConditionMeta>();
            for (int i = 0; i != serializedNode.ChildCount; i++)
                children.Add(ReadNodeFromSerializedNodes(serializedNode.IndexOfFirstChild + i));

            return new BuffConditionMeta(serializedNode)
            {
                Type = serializedNode.Type,
                SubCondition = children
            };
        }
    }

    /// <summary>
    /// 用于保存条件在序列化时的信息
    /// </summary>
    [System.Serializable]
    public class SerializableBuffConditionMeta : SerializableDictionaryMeta
    {
        public BuffConditionType Type;

        public int ChildCount;
        public int IndexOfFirstChild;

        public SerializableBuffConditionMeta() { }

        public SerializableBuffConditionMeta(SerializableDictionaryMeta meta) : base(meta) { } 
    }

    /// <summary>
    /// 条件数据
    /// </summary>
    public class BuffConditionMeta : SerializableDictionaryMeta
    {
        public BuffConditionType Type;

        public List<BuffConditionMeta> SubCondition;
        
        public BuffConditionMeta() 
        {
            SubCondition = new List<BuffConditionMeta>();
        }

        public BuffConditionMeta(BuffConditionMeta meta) : base(meta) 
        {
            Type = meta.Type;

            SubCondition = new List<BuffConditionMeta>();
            for (int i = 0; i < meta.SubCondition.Count; i++)
            {
                SubCondition.Add(meta.SubCondition[i].DeepClone() as BuffConditionMeta);
            } 
        }

        public BuffConditionMeta(SerializableDictionaryMeta meta) : base(meta) { } 

        public override MetaBase DeepClone()
        {
            return new BuffConditionMeta(this);
        }

        public void CreateSubCondition(int num)
        {
            SubCondition.Clear();
            for (int i = 0; i < num; i++)
            {
                SubCondition.Add(new CondCubeMeta());
            }
        }
    }

    public class CondTargetMeta : BuffConditionMeta
    {
        public const string Camp = "Camp";
        public const string Target = "Target";

        public CondTargetMeta()
        {
            TryAddValue(Camp, SKILL.Camp.Comrade);
            TryAddValue(Target, SKILL.Target.Caster);
        }
    }

    public class CondFalseMeta: BuffConditionMeta
    {
        public CondFalseMeta()
        {
            Type = BuffConditionType.False;
        }
    }

    public class CondTrueMeta: BuffConditionMeta
    {
        public CondTrueMeta()
        {
            Type = BuffConditionType.True;
        }
    }

    public class CondCubeMeta : CondTargetMeta
    {
        public const string CubeEraseType = "Type";
        public const string Count = "Count";

        public CondCubeMeta()
        {
            Type = BuffConditionType.Cube;
            TryAddValue(CubeEraseType, BUFF.CubeEraseType.Any);
            TryAddValue(Count, "0");
        }
    }

    public class CondRandomMeta : BuffConditionMeta
    {
        public const string Probability = "Probability";
        
        public CondRandomMeta()
        {
            Type = BuffConditionType.Random;
            TryAddValue(Probability, "0");
        }
    }

    public class CondTimeMeta : BuffConditionMeta
    {
        public const string Time = "Time";

        public CondTimeMeta()
        {
            Type = BuffConditionType.Time;
            TryAddValue(Time, "0");
        }
    }

    public class CondTimesMeta : BuffConditionMeta
    {
        public const string Times = "Times";

        public CondTimesMeta()
        {
            Type = BuffConditionType.Times;
            TryAddValue(Times, "0");
        }
    }

    public class CondAttributeMeta : CondTargetMeta 
    {
        public const string FieldType = "FieldType";
        public const string Compare = "Compare";
        public const string ValueType = "ValueType";
        public const string Value = "Value";

        public const string BaseCamp = "BaseCamp";
        public const string BaseTarget = "BaseTarget";
        public const string BaseField = "BaseField";

        public CondAttributeMeta()
        {
            Type = BuffConditionType.Attribute;
            TryAddValue(FieldType, ActorField.HP);
            TryAddValue(Compare, CompareOperation.LessThan);
            TryAddValue(ValueType, BUFF.ValueType.Number);
            TryAddValue(Value, "0");
        }
    }

    public class CondEnergyMeta : CondTargetMeta 
    {
        public const string Compare = "Compare";
        public const string Value = "Value";

        public CondEnergyMeta()
        {
            Type = BuffConditionType.Energy;
            TryAddValue(Compare, CompareOperation.LessThan);
            TryAddValue(Value, "0");
        }
    }

    public class CondAttackBeginMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.AttackBegin;
        }
    }

    public class CondAttackEndMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.AttackEnd;
        }
    }

    public class CondAttackMeta : CondTargetMeta
    {
        public const string SkillId = "Id";
        public const string HaveTargetCondition = "Flag";

        public CondAttackMeta()
        {
            InitType();
            TryAddValue(SkillId, "");
            TryAddValue(HaveTargetCondition, 0);
        }

        protected virtual void InitType()
        {
            Type = BuffConditionType.Attack;
        }
    }

    public class CondCritMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.Crit;
        }
    }

    public class CondDodgeMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.Dodge;
        }
    }

    public class CondHitMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.Hit;
        }
    }

    public class CondParryMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.Parry;
        }
    }
    
    public class CondCureMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.Cure;
        }
    }

    public class CondBeAttackMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.BeAttack;
        }
    }

    public class CondBeCritMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.BeCrit;
        }
    }

    public class CondBeDodgeMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.BeDodge;
        }
    }

    public class CondBeHitMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.BeHit;
        }
    }

    public class CondBeParryMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.BeParry;
        }
    }
    
    public class CondBeCureMeta : CondAttackMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.BeCure;
        }
    }

    public class CondHaveBuffMeta : CondTargetMeta
    {
        public const string BuffType = "Type";
        public const string BuffId = "Id"; 
        
        public CondHaveBuffMeta()
        {
            InitType();
            TryAddValue(BuffType, BUFF.BuffType.Buff);
            TryAddValue(BuffId, "");
        }

        protected virtual void InitType()
        {
            Type = BuffConditionType.HaveBuff;
        }
    }

    public class CondBuffEndMeta : CondHaveBuffMeta
    {
        protected override void InitType()
        {
            Type = BuffConditionType.BuffEnd;
        }
    }

    public class CondBuffOverlayMeta : CondHaveBuffMeta
    {
        public const string Layer = "Layer";

        public CondBuffOverlayMeta()
        {
            TryAddValue(Layer, "1");
        }

        protected override void InitType()
        {
            Type = BuffConditionType.BuffOverlay;
        }
    }

    public class CondOpAndMeta : BuffConditionMeta
    {
        public CondOpAndMeta() 
        {
            Type = BuffConditionType.OpAnd;
            CreateSubCondition(2);
        }
    }

    public class CondOpNotMeta : BuffConditionMeta
    {
        public CondOpNotMeta() 
        {
            Type = BuffConditionType.OpNot;
            CreateSubCondition(1);
        }
    }

    public class CondOpOrMeta : BuffConditionMeta
    {
        public CondOpOrMeta() 
        {
            Type = BuffConditionType.OpOr;
            CreateSubCondition(2);
        }
    }

    public class CondOpTimeIntervalMeta : BuffConditionMeta
    {
        public const string Time = "Time";

        public CondOpTimeIntervalMeta() 
        {
            Type = BuffConditionType.OpTimeInterval;
            TryAddValue(Time, "0");
            CreateSubCondition(1);
        }
    }

    public class CondOpTrueTimesMeta : BuffConditionMeta
    {
        public const string Times = "Times";

        public CondOpTrueTimesMeta()
        {
            Type = BuffConditionType.OpTrueTimes;
            TryAddValue(Times, "0");
            CreateSubCondition(1);
        }
    }

    public class CondOpLoopMeta : BuffConditionMeta
    {
        public CondOpLoopMeta()
        {
            Type = BuffConditionType.OpLoop;
            CreateSubCondition(1);
        }
    }

    //[System.Serializable]
    //public class TestNode1
    //{
    //    public string value = "1";
    //    public TestNode2 node = new TestNode2();
    //}
    //[System.Serializable]
    //public class TestNode2
    //{
    //    public string value = "2";
    //    public TestNode3 node = new TestNode3();
    //}
    //[System.Serializable]
    //public class TestNode3
    //{
    //    public string value = "3";
    //    public TestNode4 node = new TestNode4();
    //}
    //[System.Serializable]
    //public class TestNode4
    //{
    //    public string value = "4";
    //    public TestNode5 node = new TestNode5();
    //}
    //[System.Serializable]
    //public class TestNode5
    //{
    //    public string value = "5";
    //    public TestNode6 node = new TestNode6();
    //}
    //[System.Serializable]
    //public class TestNode6
    //{
    //    public string value = "6";
    //    public TestNode7 node = new TestNode7();
    //}
    //[System.Serializable]
    //public class TestNode7
    //{
    //    public string value = "7";
    //    public TestNode8 node = new TestNode8();
    //}
    //[System.Serializable]
    //public class TestNode8
    //{
    //    public string value = "8";
    //    public TestNode9 node = new TestNode9();
    //}
    //[System.Serializable]
    //public class TestNode9
    //{
    //    public string value = "9";
    //    public TestNode10 node = new TestNode10();
    //}
    //public class TestNode10
    //{
    //    public string value = "10";
    //}
}
