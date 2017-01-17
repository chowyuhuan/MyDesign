using System.Collections.Generic;
using ACTOR;
using SKILL;

namespace BUFF
{
    /// <summary>
    /// 目标管理
    /// </summary>
    public class TargetManager
    {
        private static TargetManager instance;

        public static TargetManager Instance
        {
            get 
            { 
                if (instance == null)
                {
                    instance = new TargetManager();
                }
                return instance; 
            }
        }

        private TargetManager() { }

        /// <summary>
        /// 选择目标
        /// </summary>
        /// <param name="camp"></param>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="hitters"></param>
        /// <returns></returns>
        public ITargetWrapper[] Choose(Camp camp, Target target, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            ITargetWrapper[] targetNull = new ITargetWrapper[0];

            switch (target)
            {
                case Target.Caster:
                    return caster == null ? targetNull : new ITargetWrapper[] { caster };
                case Target.Hit:
                    return hitters == null ? targetNull : hitters;
                default:
                    Camp realCamp = caster == null ? camp : SkillUtility.LocateCamp(((Actor)caster).SelfCamp, camp);
                    Actor[] actors = ActorManager.Instance.Choose((Actor)caster, realCamp, target);
                    return actors == null ? targetNull : actors;
            }
        }

        /// <summary>
        /// 选择单个目标, 返回第一个
        /// </summary>
        /// <param name="camp"></param>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="hitters"></param>
        /// <returns></returns>
        public ITargetWrapper ChooseOne(Camp camp, Target target, ITargetWrapper caster = null, params ITargetWrapper[] hitters)
        {
            ITargetWrapper[] targets = Choose(camp, target, caster, hitters);

            return targets.Length == 0 ? null : targets[0];
        }
    }
}
