using UnityEngine;
using System;
using System.Collections;

namespace SKILL
{
    /// <summary>
    /// 技能输入
    /// 一个勇士挂一个
    /// 接受1、2、3消
    /// 
    /// 块生成器和UI，只关心谁的块（勇士）、块什么样（技能图标，1、2、3消对应3个节能，但图标是同一个）、消了几个什么块（告诉SkillInput几消或者特殊技能）
    /// 具体1、2、3消对应什么技能，由SkillInput决定
    /// 在角色身上，会有技能载体：1、2、3消对应什么技能，特殊技能是什么
    /// </summary>
    public class InputCenter : CompBase
    {
        /// <summary>
        /// 输入
        /// </summary>
        /// <param name="_index">勇士最多有4中输入，1、2、3消方框（_index分别为0、1、2）和1消圆（_index为3），为了通用，这里用索引</param>
        public void Input(int index)
        {
            int skillId = 0;
            int finalIndex = index;

            if (Owner.SkillController.BuffCubeSkill() != null)
            {
                skillId = Convert.ToInt32(Owner.SkillController.BuffCubeSkill());
            }
            else
            {
                finalIndex = Owner.SkillController.BuffCubeStrategy(index + 1) - 1;
                skillId = Owner.SkillController.SkillPossessorEx.GetSkillID(finalIndex);
            }

            Owner.SkillController.Caster.EnqueueToCast(skillId, index+1); // 施法器中，几消从1开始，所以+1
            Owner.SkillController.Caster.TryToCast();

            Owner.SkillController.EraseCube(finalIndex + 1);

            if (index < 3)
                Owner.TeamEx.AddGoddessSpByErase(index + 1);
        }
    }

}

