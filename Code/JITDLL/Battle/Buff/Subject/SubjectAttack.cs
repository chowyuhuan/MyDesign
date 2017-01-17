
namespace BUFF
{
    /// <summary>
    /// 攻击状态
    /// </summary>
    public enum AttackStatus
    {
        None,
        Begin,
        End,
        Interrupt,
        Hit,
    }

    /// <summary>
    /// 攻击类型
    /// </summary>
    public enum AttackType
    {
        None,
        Damage,
        Cure,
    }

    /// <summary>
    /// 攻击主题
    /// </summary>
    public class SubjectAttack : Subject, IMessage
    {
        // 技能Id
        public string skillId;

        // 攻击状态
        public AttackStatus statusFlag;

        // 攻击类型
        public AttackType attackType;

        // 目标
        public ITargetWrapper target;

        // 伤害值
        public float value;

        // 命中
        public bool hit;

        // 闪避
        public bool dodge;

        // 格挡
        public bool parry;

        // 暴击
        public bool crit;

        public SubjectAttack(ITargetWrapper caster, string skillId,  AttackStatus statusFlag, AttackType attackType, ITargetWrapper target, float value, bool hit, bool dodge, bool parry, bool crit)
        {
            this.type = SubjectType.Attack;
            Init(caster, skillId, statusFlag, attackType, target, value, hit, dodge, parry, crit);
        }

        public void Init(ITargetWrapper caster, string skillId, AttackStatus statusFlag, AttackType attackType , ITargetWrapper target, float value, bool hit, bool dodge, bool parry, bool crit)
        {
            this.caster = caster;
            this.skillId = skillId;
            this.attackType = attackType;
            this.statusFlag = statusFlag;
            this.target = target;
            this.value = value;
            this.hit = hit;
            this.dodge = dodge;
            this.parry = parry;
            this.crit = crit;
        }

        public override string Message()
        {
            string result = base.Message();

            result += " 技能:" + skillId;
            result += " 状态:" + statusFlag.ToString();

            if (attackType != AttackType.None)
            {
                result += " 类型:" + attackType.ToString();
                result += " 目标:" + target.Tag();
                result += " 数值:" + value.ToString();
                result += " 命中:" + hit.ToString();
                result += " 闪避:" + dodge.ToString();
                result += " 格挡:" + parry.ToString();
                result += " 暴击:" + crit.ToString();
            }

            return result;
        }
    }
}
