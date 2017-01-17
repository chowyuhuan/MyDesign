using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

public class SkillUtility
{
    /// <summary>
    /// 校正阵营
    /// 从敌人视角进行攻击时，需要转换阵营。敌人的攻击行为配置时，攻击的阵营为Enemy，但在全局管理上，之区分我方和敌方。所以，敌人在攻击时，尽管攻击的阵营配置的是敌方，但其实攻击的是全局的友方。
    /// </summary>
    /// <param name="_attack">攻击方阵营</param>
    /// <param name="_attacked">被攻击方阵营，相对于攻击方</param>
    /// <returns>转换到全局的阵营</returns>
    public static Camp LocateCamp(Camp _attack, Camp _attacked)
    {
        if(_attack == Camp.Comrade)
        {
            return _attacked;
        }
        else if (_attack == Camp.Enemy)
        {
            if (_attacked == Camp.Comrade)
            {
                return Camp.Enemy;
            }
            else if (_attacked == Camp.Enemy)
            {
                return Camp.Comrade;
            }
        }
        return _attacked;
    }


    /// <summary>
    /// 终结
    /// </summary>
    /// <param name="obj">被终结对象</param>
    /// <param name="actor">被终结角色</param>
    public static void Terminate(TerminateMode mode, GameObject obj, Actor actor = null)
    {
        switch(mode)
        {
            case TerminateMode.Destroy:
                EntityPool.Destroy(obj);
                break;
            case TerminateMode.DisObject:
                obj.SetActive(false);
                break;
            case TerminateMode.DisCollider:
                Collider[] cols = obj.GetComponentsInChildren<Collider>();
                for (int i = 0; i < cols.Length; ++i )
                {
                    cols[i].enabled = false;
                }
                break;
            case TerminateMode.DeactiveWeapon:
                actor.ActorReference.ActorWeaponEx.Deactive();
                break;
            case TerminateMode.Immortal:
                break;
        }
    }


    public static float GetSkillValueUnit(NumericMeta.ValueUnit unit, Actor actor)
    {
        switch (unit.OpEx)
        {
            case Op.Assign:
                return unit.Factor;
            case Op.Multiply:
                return actor.GetValue(unit.BaseField, true) * unit.Factor;
            case Op.Add:
                return actor.GetValue(unit.BaseField, true) + unit.Factor;
        }
        return 0;
    }

    public static float GetSkillValueNumeric(NumericMeta num, Actor self, Actor target)
    {
        float result = 0;
        List<NumericMeta.ValueUnit> nums = num.Nums;
        for (int i = 0; i < nums.Count; ++i)
        {
            result += GetSkillValueUnit(nums[i], nums[i].BaseEx == NumericMeta.Base.Caster ? self : target);
        }
        return result;
    }

    public static float GetSkillValueField(Volume vol, ACTOR.ActorField field, Actor self, Actor target)
    {
        SKILL.Volume.Field[] fields = vol.Fields;
        if (fields == null || fields.Length <= 0)
        {
            return 0;
        }
        for (int i = 0; i < fields.Length; ++i)
        {
            SKILL.Volume.Field temp = fields[i];
            if (temp.FieldType == field)
            {
                return GetSkillValueNumeric(temp.Meta, self, target);
            }
        }
        return 0;
    }
}
