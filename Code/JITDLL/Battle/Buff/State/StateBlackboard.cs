using System.Collections.Generic;

namespace BUFF
{
    /// <summary>
    /// Buff状态黑板
    /// </summary>
    public class StateBlackboard
    {
        // 方块技能
        private string cubeSkill;

        public string CubeSkill
        {
            get { return cubeSkill; }
            set { cubeSkill = value; }
        }

        // 方块策略（1消视为3消，任意消视为3消等）
        private ICubeStrategy cubeStrategy;

        public ICubeStrategy CubeStrategy
        {
            get { return cubeStrategy; }
            set { cubeStrategy = value; }
        }

        // 不可移动
        private bool cantMove;

        public bool CantMove
        {
            get { return cantMove; }
            set { cantMove = value; }
        }

        // 不可击退
        private bool cantKnockback;

        public bool CantKnockback
        {
            get { return cantKnockback; }
            set { cantKnockback = value; }
        }

        // 减速
        private float speedCut;

        public float SpeedCut
        {
            get { return speedCut; }
            set { speedCut = value; }
        }

        // 免疫减速
        private bool immuneSpeedCut;

        public bool ImmuneSpeedCut
        {
            get { return immuneSpeedCut; }
            set { immuneSpeedCut = value; }
        }

        // 眩晕
        private bool stun;

        public bool Stun
        {
            get { return stun; }
            set { stun = value; }
        }

        // 免疫眩晕
        private bool immuneStun;

        public bool ImmuneStun
        {
            get { return immuneStun; }
            set { immuneStun = value; }
        }

        // 免疫buff类型
        private int immuneBuffType;

        public int ImmuneBuffType
        {
            get { return immuneBuffType; }
            set { immuneBuffType = value; }
        }

        // 无敌 （免伤）
        private bool invincible;

        public bool Invincible
        {
            get { return invincible; }
            set { invincible = value; }
        }

        // 护盾
        private float shield;

        public float Shield
        {
            get { return shield; }
            set { shield = value; }
        }

        // 恢复减免 
        private float cureReduced;

        public float CureReduced
        {
            get { return cureReduced; }
            set { cureReduced = value; }
        }

        // 荆棘伤害
        private float thornsDamage;

        public float ThornsDamage
        {
            get { return thornsDamage; }
            set { thornsDamage = value; }
        }

        // 承担队友伤害
        private float shareDamage;

        public float ShareDamage
        {
            get { return shareDamage; }
            set { shareDamage = value; }
        }

        public struct DamageStruct
        {
            public int nature;
            public float factor;
        }

        // 追加造成伤害量百分比的伤害
        private List<DamageStruct> additionalDamage;

        public List<DamageStruct> AdditionalDamage
        {
            get { return additionalDamage; }
            set { additionalDamage = value; }
        }

        // 追加闪避（独立判断）
        private float additionalDodge;

        public float AdditionalDodge
        {
            get { return additionalDodge; }
            set { additionalDodge = value; }
        }

        // buff类型持续时间
        private float[] buffTime;

        public float[] BuffTime
        {
            get { return buffTime; }
            set { buffTime = value; }
        }

        // 效果量
        private float effectSize;

        public float EffectSize
        {
            get { return effectSize; }
            set { effectSize = value; }
        }

        // 数值属性
        private float[] attributes;

        public float[] Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public StateBlackboard(int attributeSize) 
        {
            additionalDamage = new List<DamageStruct>();
            buffTime = new float[BuffManager.BuffTypeNumber];
            attributes = new float[attributeSize];
        }

        public void Reset()
        {
            cubeSkill = null;
            cubeStrategy = null;

            cantMove = false;
            cantKnockback = false;
            speedCut = 0;
            immuneSpeedCut = false;
            stun = false;
            immuneStun = false;
            immuneBuffType = -1;

            invincible = false;
            shield = 0;
            cureReduced = 0;
            thornsDamage = 0;
            shareDamage = 0;
            additionalDamage.Clear();
            additionalDodge = 0;

            effectSize = 1;

            for (int i = 0; i < buffTime.Length; i++)
            {
                buffTime[i] = 0;
            }

            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i] = 0;
            }
        }
    }
}
