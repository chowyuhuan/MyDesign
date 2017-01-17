using UnityEngine;
using System.Collections;

namespace SKILL
{
    /// <summary>
    /// 技能模块组件基类
    /// </summary>
    public class CompBase
    {
        public Actor Owner = null;
        public virtual void Init(Actor a)
        {
            Owner = a;
        }
    }

    /// <summary>
    /// 技能模块组件基类
    /// </summary>
    public class MonoCompBase : MonoBehaviour
    {
        [HideInInspector]
        public Actor Owner = null;
        public virtual void Init(Actor a)
        {
            Owner = a;
        }
    }
}
