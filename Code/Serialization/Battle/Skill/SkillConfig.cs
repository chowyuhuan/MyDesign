using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using BUFF;

namespace SKILL
{
    /// <summary>
    /// 意图
    /// </summary>
    public enum Intent
    {
        Damage          =   0, // 伤害
        Cure            =   1, // 治疗
    }


    /// <summary>
    /// 载体
    /// </summary>
    public enum Carrier
    {
        None    =   0, // 无（技能释放即命中类，如：治疗）
        Bullet  =   1, // 子弹（子弹技）
        Weapon  =   2, // 武器（挥砍类技能）
    }

    /// <summary>
    /// 终结信号（依赖什么来判断是否终结）
    /// </summary>
    public enum TerminateSignal
    {
        Hit         =   0, // 命中指定次数
        Time        =   1, // 持续时间（有可能与Distance合为Motion）
        Distance    =   2, // 距离（有可能与Time合为Motion）
    }

    /// <summary>
    /// 终结方式
    /// </summary>
    public enum TerminateMode
    {
        Destroy         =   0, // 销毁
        Immortal        =   1, // 什么都不做，常驻
        DisObject       =   2, // 禁用GameObject
        DisCollider     =   3, // 禁用Collider
        DeactiveWeapon  =   4, // 取消武器激活状态
    }

    public enum MixEffect
    {
        Parry       =   0, // 格挡
        Dodge       =   1, // 闪避
        Counter     =   2, // 反击
        Crit        =   3, // 暴击
        Hold        =   4, // 定身
        Stun        =   5, // 眩晕
        Damage      =   6, // 受到攻击
        Cure        =   7, // 受到治疗

        Max
    }

    // 职业
    public enum School
    {
        Sword       =   0, // 剑士
        Knight      =   1, // 骑士
        Archer      =   2, // 弓手
        Hunter      =   3, // 猎人
        Wizard      =   4, // 法师
        Flamen      =   5, // 祭司
    }

    /// <summary>
    /// 阵营
    /// </summary>
    public enum Camp
    {
        Comrade,    // 我方
        Enemy,      // 敌方
    }

    /// <summary>
    /// 具体的目标
    /// </summary>
    public enum Target
    {
        All             =   0, // 全体
        Foward          =   1, // 前排
        Middle          =   2, // 中排
        Back            =   3, // 后排
        LowestHP        =   4, // 最低血量
        Hit             =   5, // 命中者
        Caster          =   6, // 施法者
        Partner         =   7, // 队友
        Leader          =   8, // 队长
        HighestHPMax    =   9, // 最高血量上限
    }

    /// <summary>
    /// 运算
    /// </summary>
    public enum Op
    {
        Assign      =   0, // 赋值
        Multiply    =   1, // 乘
        Add         =   2, // 加
    }

    public abstract class MetaBase
    {
        // 用于AddBuff和AddTrigger的复制，由于这两个节点比较特殊，存的是索引，单独复制时无法定位实际数据。
        // 目前只在技能整体复制时才支持这两个节点的复制, 这里标示是否是技能整体复制操作。
        public static bool isSkillMetaUseForCopy;

        public abstract MetaBase DeepClone();
    }

    [System.Serializable]
    public class ValidMetaBase : MetaBase
    {
        [SerializeField]
        bool _valid = true;
        public void Invalid()
        {
            _valid = false;
        }

        public void Validate()
        {
            _valid = true;
        }

        public bool Valid()
        {
            return _valid;
        }

        public override MetaBase DeepClone()
        {
            return null;
        }
    }

    /// <summary>
    /// 数值
    /// 举例：
    /// TargetEx：Hit
    /// BaseField:DFMagic
    /// OpEx:Multiply
    /// Factor:30%
    /// 标示：提高魔法防御，数值为命中方魔法防御数值的30%
    /// </summary>
    [System.Serializable]
    public class NumericMeta : MetaBase
    {
        public enum Base
        {
            Caster  = 0,
            Hit     = 1,
        }
        [System.Serializable]
        public class ValueUnit : MetaBase
        {
            public Base BaseEx; // 只使用Self和Hit
            public ActorField BaseField;
            public Op OpEx;
            public float Factor;
            public ValueUnit(Base baseEx, ActorField field, Op op, float factor)
            {
                BaseEx = baseEx;
                BaseField = field;
                OpEx = op;
                Factor = factor;
            }
            public override MetaBase DeepClone()
            {
                return new ValueUnit(BaseEx, BaseField, OpEx, Factor);
            }
        }

        public List<ValueUnit> Nums = new List<ValueUnit>();

        public override MetaBase DeepClone()
        {
            NumericMeta temp = new NumericMeta();
            for (int i = 0; i < Nums.Count; ++i )
            {
                temp.Nums.Add(Nums[i].DeepClone() as ValueUnit);
            }
            return temp;
        }
    }

    /// <summary>
    /// 攻击性质
    /// </summary>
    public enum Nature
    {
        Magic   = 0, // 魔法
        Physics = 1, // 物理
        Real    = 2, // 真实
    }

    [System.Serializable]
    public class Volume : MetaBase
    {
        [System.Serializable]
        public class ATK : MetaBase
        {
            public Nature NatureEx; // 性质
            public NumericMeta Meta = new NumericMeta();
            public override MetaBase DeepClone()
            {
                ATK temp = new ATK();
                temp.NatureEx = NatureEx;
                temp.Meta = Meta.DeepClone() as NumericMeta;
                return temp;
            }
        }
        [System.Serializable]
        public class Field : MetaBase
        {
            public ActorField FieldType;
            public NumericMeta Meta = new NumericMeta();
            public override MetaBase DeepClone()
            {
                Field temp = new Field();
                temp.FieldType = FieldType;
                temp.Meta = Meta.DeepClone() as NumericMeta;
                return temp;
            }
        }
        // 实例：
        // 造成380%攻击力的物理伤害，追加造成自身体力10%的物理伤害，并无视与100%自身物理防御等值的对方物理防御
        public ATK[] Atks; // 技能伤害、治疗的具体数值（造成380%攻击力的物理伤害；追加造成自身体力10%的物理伤害）
        public Field[] Fields; // 属性数值，比如：暴击率提高25%，忽视50%物防（进攻属性加到施法者，防御属性加到被攻击者；因为使用场景非常少，所以用数组，没有使用其他方式做优化）（无视与100%自身物理防御等值的对方物理防御）

        public override MetaBase DeepClone()
        {
            Volume temp = new Volume();
            if (Atks != null && Atks.Length > 0)
            {
                temp.Atks = new ATK[Atks.Length];
                for (int i = 0; i < temp.Atks.Length; ++i )
                {
                    temp.Atks[i] = Atks[i].DeepClone() as ATK;
                }
            }
            if (Fields != null && Fields.Length > 0)
            {
                temp.Fields = new Field[Fields.Length];
                for (int i = 0; i < temp.Fields.Length; ++i)
                {
                    temp.Fields[i] = Fields[i].DeepClone() as Field;
                }
            }
            return temp;
        }
    }

    /// <summary>
    /// 生效的方式
    /// </summary>
    public enum ImpactWay
    {
        Once        =   0, // 一次性
        Interval    =   1, // 间隔性
    }

    /// <summary>
    /// 伤害和治疗描述数据
    /// 没有找到合适的统称词，使用damage和cure的缩写
    /// 一旦被初始化，不允许修改内部的值
    /// </summary>
    [System.Serializable]
    public class DCMeta : MetaBase
    {
        public Intent Intention; // 意图（伤害 or 治疗）
        public Camp CampEx; // 目标所在阵营
        public Target TargetEx; // 具体目标
        public Carrier CarrierEx = Carrier.Bullet; // 载体
        public Volume VolumeEx = new Volume(); // 数值
        public MotionMeta Motion = new MotionMeta(); // 运动
        public TerminateMode TerminateModeEx = TerminateMode.Destroy; // 终结方式
        public int CollideLimit = 1; // 碰撞限制
        public string EmitSound; // 发射音效
        public string HitSound = "HitNormal"; // 命中音效
        public string HitEffect; // 命中特效
        public HitMotion HitMotion = HitMotion.None; // 击中改变目标运动
        public float Force1 = 0; // 力
        public float Force2 = 0; // 力
        public List<TriggerBehaviorMeta> Behaviors; //执行的行为

        public DCMeta()
        {
            Behaviors = new List<TriggerBehaviorMeta>();
        }

        public override MetaBase DeepClone()
        {
            DCMeta temp = new DCMeta();
            temp.Intention = Intention;
            temp.CampEx = CampEx;
            temp.TargetEx = TargetEx;
            temp.CarrierEx = CarrierEx;
            temp.VolumeEx = VolumeEx.DeepClone() as Volume;
            temp.Motion = Motion.DeepClone() as MotionMeta;
            temp.TerminateModeEx = TerminateModeEx;
            temp.CollideLimit = CollideLimit;
            temp.EmitSound = EmitSound;
            temp.HitSound = HitSound;
            temp.HitEffect = HitEffect;
            temp.HitMotion = HitMotion;
            temp.Force1 = Force1;
            temp.Force2 = Force2;

            temp.Behaviors = new List<TriggerBehaviorMeta>();
            for (int i = 0; i < Behaviors.Count; ++i)
            {
                temp.Behaviors.Add(Behaviors[i].DeepClone() as TriggerBehaviorMeta);
            }

            return temp;
        }
    }

    /// <summary>
    /// 一次攻击数据
    /// 一旦被初始化，不允许修改内部的值
    /// </summary>
    [System.Serializable]
    public class AttackMeta : MetaBase
    {
        public DCMeta[] DCs; // 大部分AttackMeta DCs元素数为1，个别AttackMeta会有多个的时候，如吉他奶的主动技能，即治疗己方，又攻击敌方
        public List<TriggerBehaviorMeta> Behaviors; //要执行的行为
        public bool MoveCasting = false; // 施法过程中伴随移动
        public float Speed = 0; // 速度
        public float Distance = 0; // 距离

        public AttackMeta()
        {
            Behaviors = new List<TriggerBehaviorMeta>();
        }

        public override MetaBase DeepClone()
        {
            AttackMeta temp = new AttackMeta();
            if (DCs != null)
            {
                temp.DCs = new DCMeta[DCs.Length];
                for (int i = 0; i < temp.DCs.Length; ++i )
                {
                    temp.DCs[i] = DCs[i].DeepClone() as DCMeta;
                }
            }

            temp.Behaviors = new List<TriggerBehaviorMeta>();
            for (int i = 0; i < Behaviors.Count; ++i)
            {
                temp.Behaviors.Add(Behaviors[i].DeepClone() as TriggerBehaviorMeta);
            }
            
            temp.MoveCasting = MoveCasting;
            temp.Speed = Speed;
            temp.Distance = Distance;
            return temp;
        }
    }


    /// <summary>
    /// 技能单元
    /// SkillDC和Buff组成
    /// buff的位置由类型（立即、命中）决定：
    ///     a.立即：Buff直接放在Skill下
    ///     b.命中：Buff放在SkillDC下
    /// 一旦被初始化，不允许修改内部的值
    /// </summary>
    [System.Serializable]
    public class Skill : MetaBase
    {
        public int ID = -1;
        public School SchoolEx = School.Sword; // 职业
        public int GroupID = -1; // 技能组，并不一定有用
        public string Name = "未命名"; // 名字
        public string CastAnim = ""; // 施法动画
        public float Range = 10000; // 射程（与Motion的Range区分开，这里的Range主要是做技能可达的验证）
        public string IconAtlas; // 图标图集
        public string IconSprite; // 图标Sprite
        public AttackMeta[] Attacks = null; // 多段攻击，攻击序列（大多一个元素；持续多段施法时，多个元素）

        public List<NewTriggerMeta> Triggers; // 技能用到的所有Trigger
        public List<NewBuffMeta> Buffs; // 技能用到的所有Buff

        public Skill()
        {
            Triggers = new List<NewTriggerMeta>();
            Buffs = new List<NewBuffMeta>();
        }

        public override MetaBase DeepClone()
        {
            Skill temp = new Skill();
            temp.ID = ID; // TODO:ID不能直接复制过来，得走统一的ID分配
            temp.SchoolEx = SchoolEx;
            temp.GroupID = GroupID;
            temp.Name = Name;
            temp.CastAnim = CastAnim;
            temp.Range = Range;
            temp.IconAtlas = IconAtlas;
            temp.IconSprite = IconSprite;
            if(Attacks != null)
            {
                temp.Attacks = new AttackMeta[Attacks.Length];
                for (int i = 0; i < temp.Attacks.Length; ++i )
                {
                    temp.Attacks[i] = Attacks[i].DeepClone() as AttackMeta;
                }
            }

            foreach (NewTriggerMeta meta in Triggers)
            {
                temp.Triggers.Add(meta.DeepClone() as NewTriggerMeta);
            }
            foreach (NewBuffMeta meta in Buffs)
            {
                temp.Buffs.Add(meta.DeepClone() as NewBuffMeta);
            }

            return temp;
        }
    }

    /// <summary>
    /// 发射点定位方式
    /// </summary>
    public enum Anchor
    {
        Caster    =   0, // 施法者
        Target    =   1, // 目标
        Position  =   2, // 指定位置
    }


    /// <summary>
    /// 运动轨迹
    /// </summary>
    public enum Navigation
    {
        SingleLine          =   0, // 单直线
        SingleCurve         =   1, // 单曲线
        MultipleLine_Rect   =   2, // 矩形多直线
        MultipleLine_Sect   =   3, // 扇形多直线
        MultipleCurve       =   4, // 多曲线
        Prefab              =   5, // Prefab控制运动信息
    }

    /// <summary>
    /// 旋转方式
    /// </summary>
    public enum RotationStyle
    {
        None,       // 无旋转 
        Spin,       // 自转 
        Slope,      // 切线 
    }

    /// <summary>
    /// 命中之后对目标的运行影响
    /// </summary>
    public enum HitMotion
    {
        None        =   0,
        NatureBump  =   1, // 自然规律的撞击，击飞）
        ManualBump  =   2, // 人为干预的撞击，击飞
        Retreat     =   3, // 击退
        Transfer    =   4, // 传送到指定位置（相对于施法者）
    }

    /// <summary>
    /// 运动描述数据（后期可能会叫做BulletMeta）
    /// </summary>
    [System.Serializable]
    public class MotionMeta : MetaBase
    {
        public string PrefabName = "Arrow";
        public Anchor AnchorEx = Anchor.Caster; // 锚点
        public Vector2 AnchorOffset = Vector2.zero; // 锚点偏移
        public Navigation Track = Navigation.SingleLine; // 轨迹
        public ImpactWay ImpactWayEx = ImpactWay.Once; // 应该放到DCMeta中的，但MotionMeta已经渐渐向子弹进化，所以想放这里
        public float Range = 10; // 曲线：拱高（先叫Range，待后期整理后依然仅仅表示拱高，就改名）
        public float MoveSpeed = 10; // 速度
        public RotationStyle RotateStyle = RotationStyle.None; // 旋转
        public float RotateSpeed = 1080; // 旋转速度
        public int EntityNum = 1; // 子弹数量
        public float Interval = 0; // 间隔，用于多子弹时子弹间施放间隔
        public float ExtendValue = 0; // 扩展参数，比如：矩形区域多子弹时，表示宽度；扇形区域多子弹时，表示角度；多曲线时，偏移范围
        public bool ForceHorizontal = true; // 强制水平方向运动（仅直线运动时有效）
        public float LifeCycle = 2;

        public override MetaBase DeepClone()
        {
            MotionMeta temp = new MotionMeta();
            temp.PrefabName = PrefabName;
            temp.AnchorEx = AnchorEx;
            temp.AnchorOffset = AnchorOffset;
            temp.Track = Track;
            temp.ImpactWayEx = ImpactWayEx;
            temp.Range = Range;
            temp.MoveSpeed = MoveSpeed;
            temp.RotateStyle = RotateStyle;
            temp.RotateSpeed = RotateSpeed;
            temp.EntityNum = EntityNum;
            temp.Interval = Interval;
            temp.ExtendValue = ExtendValue;
            return temp;
        }
    }

    /// <summary>
    /// 循环方式
    /// </summary>
    public enum WrapMode
    {
        Once    =   0, // 后期如果有次数的需求，把Once改成计数，次数就是1
        Loop    =   1,
    }
}
