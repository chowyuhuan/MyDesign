using UnityEngine;
using System.Collections;

namespace ACTOR
{
    /// <summary>
    /// 角色属性字段
    /// </summary>
    public enum ActorField
    {
        // 基础属性
        HP              =   0,  // 体力（防）
        ATK             =   1,  // 攻击力（攻）
        DFMagic         =   2,  // 魔防（防）
        DFPhysics       =   3,  // 物防（防）

        // 高级属性
        CritRate        =   4,  // 暴击率（攻）
        CritDamage      =   5,  // 暴击伤害（攻）
        Dodge           =   6,  // 闪避（防）
        Precision       =   7,  // 命中（攻）

        // 隐藏属性
        APMagic         =   8,  // 魔法穿透（攻）
        APPhysics       =   9,  // 物理穿透（攻）
        Speed           =   10, // 急速（攻）
        Suck            =   11, // 吸血（攻）
        DMReduced       =   12, // 伤害减免（防）

        Parry           =   13, // 格挡

        // 运行时属性
        HPMax           =   14, // 体力上限（防）


        Max
    }

    /// <summary>
    /// 角色种类（英雄、怪） 
    /// 英雄属性按照初始 + 等级 * 成长
    /// 怪属性直接读表
    /// </summary>
    public enum ActorType
    {
        Hero,
        Monster,
    }

    [System.Serializable]
    public class FloatField
    {
        public System.Action<int> OnValueChanged = null; // TODO:这里是int，后期需要兼顾兼容性
        public float Value
        {
            set
            {
                if (value != ValueEx)
                {
                    ValueEx = value;
                    if(OnValueChanged != null)
                    {
                        OnValueChanged((int)ValueEx);
                    }
                }
            }
            get { return ValueEx; }
        }
        public float ValueEx; // 调试阶段先用public，后期数据从表中读取时，改成previate
    }

}

