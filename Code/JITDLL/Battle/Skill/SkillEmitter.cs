using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using BUFF;

namespace SKILL
{
    /// <summary>
    /// 发射器
    /// </summary>
    public class Emitter : CompBase
    {
        AttackMeta _atk = null;
        EntityEmitter _entityEmitter = null;

        public override void Init(Actor a)
        {
            base.Init(a);
            _entityEmitter = new EntityEmitter();
            _entityEmitter.Init(a);
        }


        public void Update()
        {
            _entityEmitter.Update();
        }

        /// <summary>
        /// 发射
        /// </summary>
        /// <param name="_skill">技能数据</param>
        public void Emit(int skillId, AttackMeta atk, float range, Actor[] goals)
        {
            _atk = atk;
            for (int i = 0; i < _atk.DCs.Length; ++i)
            {
                DCMeta dc = _atk.DCs[i];
                switch (dc.CarrierEx)
                {
                    case Carrier.None:
                        EmitNonentity(skillId, dc, range, goals);
                        break;
                    case Carrier.Bullet:
                        EmitEntity(skillId, dc, range, goals);
                        break;
                    case Carrier.Weapon:
                        EmitToWeapon(skillId, dc);
                        break;
                }
                if(!string.IsNullOrEmpty(dc.EmitSound))
                {
                    AudioManager.Instance.PlaySound(dc.EmitSound);
                }
            }
        }

        /// <summary>
        /// 发射没有实体的技能（施放即命中类，如治疗、buff）
        /// </summary>
        void EmitNonentity(int skillId, DCMeta dc, float range, Actor[] goals)
        {
            if (ReChooseTargetIfNoReady(dc.CampEx, dc.TargetEx, range, ref goals))
            {
                for (int i = 0; i < goals.Length; ++i)
                {
                    goals[i].SkillController.Mixer.Attacked(skillId, dc, Owner);
                }
            }

            ExcuteBehaviors(skillId, dc.Behaviors, goals);
        }

        /// <summary>
        /// 激活武器
        /// </summary>
        /// <param name="dc">DCMeta</param>
        void EmitToWeapon(int skillId, DCMeta dc)
        {
            Owner.ActorReference.ActorWeaponEx.Active(skillId, dc);
        }

        /// <summary>
        /// 发射拥有实体类技能（子弹技）
        /// 需要碰撞来完成攻击（伤害/治疗）的技能
        /// </summary>
        /// <param name="dc">DCMeta</param>
        /// <param name="goals">目标</param>
        void EmitEntity(int skillId, DCMeta dc, float range, Actor[] goals)
        {
            if (ReChooseTargetIfNoReady(dc.CampEx, dc.TargetEx, range, ref goals))
            {
                _entityEmitter.Emit(skillId, dc, goals[0]);
            }
            else
            {
                _entityEmitter.Emit(skillId, dc, null);
            }
        }

        /// <summary>
        /// 如果没有准备好，就尝试再一次选择目标
        /// </summary>
        /// <param name="toCamp">被攻击的阵营</param>
        /// <param name="target">具体位置</param>
        /// <param name="range">可达距离</param>
        /// <param name="actors">目标引用</param>
        /// <returns>是否成功选择到目标</returns>
        bool ReChooseTargetIfNoReady(Camp toCamp, Target target, float range, ref Actor[] actors)
        {
            if (actors == null)
            {
                actors = ActorManager.Instance.Choose(Owner, SkillUtility.LocateCamp(Owner.SelfCamp, toCamp), target, range);
                return actors != null;
            }
            else
            {
                return true;
            }
        }

        public void ExcuteBehaviors(int skillId, List<TriggerBehaviorMeta> behaviors, params Actor[] hitters)
        {
            Owner.SkillController.ExcuteBehaviors(behaviors, skillId, hitters);
        }
    }

    /// <summary>
    /// 实体发射器
    /// 因为实体的运动形式有限且固定，故采用Switch-Case的方式编码，代替多态（多态需要根据运动形式，new出不同的处理类对象，堆分配过于频繁，且不便存储管理）
    /// </summary>
    public class EntityEmitter : CompBase
    {
        class DCBuffer
        {
            public int SkillId = 0; 
            public DCMeta DC = null;
            public int Count = 0;
            public float FireTime = 0;
            public Vector2 Origin;
            public Vector2 End;
            public bool Idle = true;
            public void Clear()
            {
                DC = null;
                Count = 0;
                FireTime = 0;
                Origin = Vector2.zero;
                End = Vector2.zero;
                Idle = true;
            }
        }
        const float Virtual_Goal_Offset = 2; // 没有目标空放时，相对源点偏移
        List<DCBuffer> _dcBuffer = new List<DCBuffer>(); // 仅仅处理延迟释放，同时释放的要在Emit时全部释放

        public void Emit(int skillId, DCMeta dc, Actor goal)
        {
            if (goal == null) // 没有目标时，在施法者前方Virtual_Goal_Offset距离处施放
            {
                Vector2 temp = Owner.transform.position;
                Emit(skillId, dc, temp, new Vector2(temp.x + Virtual_Goal_Offset, temp.y));
            }
            else
            {
                Emit(skillId, dc, Owner.transform.position, goal.transform.position);
            }
        }

        public void Emit(int skillId, DCMeta dc, Vector2 origin, Vector2 end)
        {
            EmitOne(skillId, dc, origin, end); // 无论是一颗子弹还是多颗，第一颗是一定要立刻释放出去的

            if (dc.Motion.EntityNum > 1) // 表示，不会立刻释放出所有子弹
            {
                if (dc.Motion.Interval <= 0)
                {
                    for (int i = 0; i < dc.Motion.EntityNum - 1; ++i)
                    {
                        EmitOne(skillId, dc, origin, end);
                    }
                }
                else
                {
                    DCBuffer buffer = GetIdleDCBuffer();
                    buffer.SkillId = skillId;
                    buffer.DC = dc;
                    buffer.Count = dc.Motion.EntityNum - 1;
                    buffer.FireTime = GameTimer.time;
                    buffer.Origin = origin;
                    buffer.End = end;
                    buffer.Idle = false;
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _dcBuffer.Count; ++i)
            {
                _dcBuffer[i].Clear();
            }
        }

        public void Update()
        {
            for (int i = 0; i < _dcBuffer.Count; ++i)
            {
                if (_dcBuffer[i].Idle)
                {
                    continue;
                }
                if (GameTimer.time >= (_dcBuffer[i].FireTime + _dcBuffer[i].DC.Motion.Interval))
                {
                    EmitOne(_dcBuffer[i].SkillId, _dcBuffer[i].DC, _dcBuffer[i].Origin, _dcBuffer[i].End);
                    if (--(_dcBuffer[i].Count) <= 0)
                    {
                        _dcBuffer[i].Clear();
                    }
                    else
                    {
                        _dcBuffer[i].FireTime = GameTimer.time;
                    }
                }
            }
        }

        void EmitOne(int skillId, DCMeta dc, Vector2 origin, Vector2 end)
        {
            // TODO:优化，转到内存池 -------------------------------------------------------------------------------
            GameObject go = EntityPool.Spwan(AssetManage.AM_PathHelper.GetActorEffectFullPathByName(dc.Motion.PrefabName)) as GameObject;
            SkillDC tmDc = go.AddComponent<SkillDC>();
            tmDc.SkillId = skillId;
            tmDc.MetaEx = dc;
            tmDc.Init(Owner);
            //Rigidbody rig = go.AddComponent<Rigidbody>();
            //rig.isKinematic = true;
            //BoxCollider collider = go.AddComponent<BoxCollider>();
            //collider.isTrigger = true;
            go.layer = LayerMask.NameToLayer(Owner.SelfCamp == Camp.Enemy ? "EnemyBullet" : "ComradeBullet"); // TODO:优化，待layer稳定后，直接用int
            // ----------------------------------------------------------------------------------------------------

#if DETECT
            go.AddComponent<CheckBulletDestroy>();
#endif

            origin = OriginModifier(dc, origin, end);
            switch (dc.Motion.Track)
            {
                case Navigation.SingleLine:
                case Navigation.MultipleLine_Rect:
                case Navigation.MultipleLine_Sect:
                    LineMotion.Begin(go, origin, EndModifier(dc, origin, end), dc.Motion.LifeCycle, dc.Motion.MoveSpeed, dc.Motion.RotateStyle, dc.Motion.RotateSpeed, tmDc.OnMoveFinish);
                    break;
                case Navigation.SingleCurve:
                case Navigation.MultipleCurve:
                    ParabolaMotion.Begin(go, dc.Motion.MoveSpeed, origin, EndModifier(dc, origin, end), dc.Motion.Range, dc.Motion.RotateStyle, dc.Motion.RotateSpeed, tmDc.OnMoveFinish);
                    break;
                case Navigation.Prefab: // do nothing，即位置信息也在prefab中
                    go.transform.position = origin;
                    break;
            }
        }

        Vector2 OriginModifier(DCMeta meta, Vector2 origin, Vector2 end)
        {
            switch (meta.Motion.AnchorEx)
            {
                case Anchor.Caster:
                    origin += meta.Motion.AnchorOffset;
                    break;
                case Anchor.Target:
                    origin = end + meta.Motion.AnchorOffset;
                    break;
                case Anchor.Position:
                    origin = meta.Motion.AnchorOffset;
                    break;
            }
            Vector2 dir = (end - origin).normalized;
            switch (meta.Motion.Track)
            {
                case Navigation.MultipleLine_Rect:
                    Vector2 d90 = new Vector2(-dir.y, dir.x).normalized;
                    return d90 * Random.Range(-meta.Motion.ExtendValue / 2, meta.Motion.ExtendValue / 2) + origin;
            }
            return origin;
        }

        Vector2 EndModifier(DCMeta meta, Vector2 origin, Vector2 end)
        {
            switch (meta.Motion.Track)
            {
                case Navigation.SingleLine:
                case Navigation.MultipleLine_Rect:
                    if(meta.Motion.ForceHorizontal)
                    {
                        end.y = origin.y;
                    }
                    return (end - origin).normalized;
                case Navigation.MultipleLine_Sect:
                    if(meta.Motion.ForceHorizontal)
                    {
                        end.y = origin.y;
                    }
                    Vector2 dir = (end - origin).normalized;
                    float rad = Random.Range(-meta.Motion.ExtendValue / 2, meta.Motion.ExtendValue / 2) * Mathf.Deg2Rad;
                    float cosValue = Mathf.Cos(rad);
                    float sinValue = Mathf.Sin(rad);
                    return new Vector2(dir.x * cosValue - dir.y * sinValue, dir.x * sinValue + dir.y * cosValue);
                case Navigation.MultipleCurve:
                    return end + Vector2.right * Random.Range(-meta.Motion.ExtendValue / 2, meta.Motion.ExtendValue / 2);
            }
            return end;
        }

        DCBuffer GetIdleDCBuffer()
        {
            for (int i = 0; i < _dcBuffer.Count; ++i)
            {
                if (_dcBuffer[i].Idle)
                {
                    return _dcBuffer[i];
                }
            }
            DCBuffer tmp = new DCBuffer();
            _dcBuffer.Add(tmp);
            return tmp;
        }
    }
}