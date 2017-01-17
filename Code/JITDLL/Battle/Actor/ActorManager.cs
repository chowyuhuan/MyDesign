using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using ACTOR;

namespace ACTOR
{
    public class ActorManager : MonoBehaviour
    {
        public delegate void OnActorBorn(Actor actor);
        public OnActorBorn HeroBorn;
        public OnActorBorn MonsterBorn;

        private static ActorManager _instance;
        public static ActorManager Instance
        {
            get { return _instance; }
        }

        int _generatorId; // actor id 计数 

        void Awake()
        {
            _instance = this;
        }

        Team _comrade;   // 我方 
        Team _enemy;     // 敌人 

        /// <summary>
        /// 初始化 
        /// </summary>
        public void Initialize()
        {
            _generatorId = 0;
        }


        public void SetComradeTeam(Team comrade)
        {
            _comrade = comrade;
        }
        public void SetEnemyTeam(Team enemy)
        {
            _enemy = enemy;
        }

        public void EnemyTeamClear()
        {
            _enemy.Clear();
        }

        public bool TeamDeath(Camp camp)
        {
            Team t = GetTeam(camp);
            return t.Death();
        }

        Actor BuildActor(ActorPrepareInfo prepareInfo, float x)
        {
            GameObject go = EntityPool.Spwan(prepareInfo.PrefabPath) as GameObject;
            if (!string.IsNullOrEmpty(prepareInfo.EffectPath))
            {
                GameObject effectPrefab = Resources.Load<GameObject>(prepareInfo.EffectPath);
                if (effectPrefab != null)
                {
                    GameObject effectRoot = GameObject.Instantiate<GameObject>(effectPrefab);
                    effectRoot.transform.SetParent(go.transform);
                }
            }

            if (!string.IsNullOrEmpty(prepareInfo.AnimPath))
            {
                Animator anim = go.GetComponent<Animator>();
                if(anim == null)
                {
                    anim = go.AddComponent<Animator>();
                }
                anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(prepareInfo.AnimPath);
                if (anim.runtimeAnimatorController != null)
                {
                    anim.updateMode = AnimatorUpdateMode.Normal;
                    anim.applyRootMotion = false;
                    anim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                }
            }

            Actor actor = go.AddComponent<Actor>();
            actor.Initialize(prepareInfo, ++_generatorId);

            actor.ActorReference.ActorControlEx.Initialize(prepareInfo.NormalRangeId);

            actor.ActorReference.ActorMovementEx.MovePosition(new Vector2(x + prepareInfo.OffsetX, 0), false);

            if (prepareInfo.CampEx == Camp.Comrade)
            {
                _comrade.Add(actor, prepareInfo.IsLeader);
            }
            else
            {
                _enemy.Add(actor, prepareInfo.IsLeader);
            }

            if (prepareInfo.WeaponInfo != null)
            {
                GameObject weapon = EntityPool.Spwan(prepareInfo.WeaponInfo.prefabPath) as GameObject;
                if(weapon != null)
                {
                    actor.ActorReference.ActorWeaponEx.SetWeapon(weapon.transform, prepareInfo.WeaponInfo.LocalPosition, prepareInfo.WeaponInfo.LocalRotation);
                }
            }

            return actor;
        }

        public Actor SpawnHero(ActorPrepareInfo prepareInfo, float x)
        {
            Actor actor = BuildActor(prepareInfo, x);
            actor.gameObject.layer = LayerMask.NameToLayer("Comrade"); // TODO:优化，待layer稳定后，直接用int

            if (null != HeroBorn)
            {
                HeroBorn(actor);
            }
            return actor;
        }

        public Actor SpawnMonster(ActorPrepareInfo prepareInfo, float x)
        {
            Actor actor = BuildActor(prepareInfo, x);
            actor.gameObject.layer = LayerMask.NameToLayer("Enemy"); // TODO:优化，待layer稳定后，直接用int
            actor.transform.rotation = Quaternion.Euler(0, -90, 0);

            if (null != MonsterBorn)
            {
                MonsterBorn(actor);
            }
            return actor;
        }

        public float GetFrontline(Camp camp)
        {
            if (camp == Camp.Comrade)
            {
                return _comrade.Frontline;
            }
            else
            {
                return _enemy.Frontline;
            }
        }

        public Team GetTeam(Camp camp)
        {
            Team team;
            if (camp == Camp.Enemy)
                team = _enemy;
            else
                team = _comrade;

            return team;
        }

        /// <summary>
        /// 技能选择用 
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="x">x轴范围</param>
        /// <returns></returns>
        public Actor[] Choose(Actor self, Camp camp, Target type, float x = 0)
        {
            return GetTeam(camp).Choose(self, type, x);
        }

        /// <summary>
        /// 普通攻击选择用 
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="reachX">x轴接触范围</param>
        /// <param name="maxX">x轴最大范围</param>
        /// <returns></returns>
        public Actor[] Choose(Camp camp, Target type, float reachX, float maxX)
        {
            return GetTeam(camp).Choose(type, reachX, maxX);
        }

        public float GetTeamFrontline(Camp camp)
        {
            return GetTeam(camp).Frontline;
        }

        public Actor FindActorById(int battleId)
        {
            return _comrade.FindActor(battleId);
        }
    }
}