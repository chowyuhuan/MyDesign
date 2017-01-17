using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

public class ActorAnim : CompBase
{
    Animator _animator;
    AnimEffect _animEffect;
    AnimEventsCtl _animEventsCtl;
    int _frame = 0;

    public float Speed
    {
        set
        {
            _animator.speed = value;
            _animEventsCtl.Speed = value;
            _animEffect.Speed = value;
            Owner.SkillController.ActorAudioEx.Speed = value;
        }
    }

    public override void Init(Actor a)
    {
        base.Init(a);
        _animator = a.GetComponent<Animator>();
        _animEventsCtl = new AnimEventsCtl();
        _animEventsCtl.Init(a);
        _animEffect = new AnimEffect();
        _animEffect.Init(a);
    }

    void RegistEventTrigger()
    {
        AnimEventTrigger[] triggers = _animator.GetBehaviours<AnimEventTrigger>();
        for (int i = 0; i < triggers.Length; ++i)
        {
            triggers[i].EnterTriggerFunc = OnStateEnter;
        }
    }

    public void Play(string name)
    {
        //if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            _animator.CrossFadeInFixedTime(Animator.StringToHash(name), 0.2f, -1, 0);
        }
    }

    public void Stop(string name)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            _animator.Stop();
            _animEventsCtl.StopTick();
        }
    }

    public void Stop()
    {
        _animator.Stop();
        _animEventsCtl.StopTick();
    }

    public Animator GetAnimator()
    {
        return _animator;
    }

    public void CUpdate()
    {
        _animEventsCtl.CUpdate();
        if (_frame != -1 && _frame++ > 0)
        {
            RegistEventTrigger();
            _frame = -1;
        }
    }

    void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] info = animator.GetNextAnimatorClipInfo(0);
        if (info.Length > 0)
        {
            string clip = info[0].clip.name;
            _animEffect.TryToPlay(clip);
            _animEventsCtl.StartTick(clip);
        }
    }

    #region 动画事件
    class AnimEventsCtl : CompBase
    {
        static Dictionary<int, AnimEvents> _gAnimEventsCache = new Dictionary<int, AnimEvents>();

        public float Speed
        {
            set { _speed = value; }
            get { return _speed; }
        }

        Dictionary<string, AnimEvents> _animEvents = new Dictionary<string, AnimEvents>();
        AnimEvents _tickingAnimEvents;
        string _stateName;
        float _speed = 1;
        public override void Init(Actor a)
        {
            base.Init(a);
            RecastEvents();
        }

        public void StartTick(string name)
        {
            AnimEvents es;
            if (_animEvents.TryGetValue(name, out es))
            {
                es.RestartTick();
                _tickingAnimEvents = es;
            }
        }

        public void StopTick()
        {
            _tickingAnimEvents = null;
        }

        public void CUpdate()
        {
            if (_tickingAnimEvents != null)
            {
                _tickingAnimEvents.CUpdate(Owner, _speed);
            }
        }

        /// <summary>
        /// 遇到的问题：
        /// clip一旦被第一次加载处理完毕，clip.events = null之后，会改变asset内的clip.events属性。之后的角色，复用这个动画的角色在加载重构AnimEvents时，读出来的clip.events为null，导致建立animevents失败，比如：两者同样的小怪，第二只实例化的小怪的animevents就是空的。
        /// 尝试修改上面的问题：添加_gAnimEventsCache，缓存所有之前已经加载并构建的animevents。但遇到新的问题，旧的animevents中注册的回调，是旧的角色上的接口。如果要纠正为新的角色的回调，就要破坏AnimEvent的通用性。毕竟AnimEvent现在的设计，是忽略具体的类型的，直接传
        /// 委托即可。
        /// </summary>
        void RecastEvents()
        {
            Animator anim = Owner.ActorReference.ActorAnimEx.GetAnimator();
            List<AnimationClip> clips = UniqueAnimationEvents(anim.runtimeAnimatorController.animationClips);
            foreach (AnimationClip clip in clips)
            {
                if ((clip.name.StartsWith("skill") || clip.name.StartsWith("attack")))
                {
                    AnimEvents e = null;
                    if (!_gAnimEventsCache.TryGetValue(clip.GetInstanceID(), out e))
                    {
                        AnimationEvent[] events = clip.events;
                        int length = events.Length;
                        e = new AnimEvents(length);
                        for (int i = 0; i < length; ++i)
                        {
                            AnimationEvent priEvent = events[i];
                            switch (priEvent.functionName)
                            {
                                case "cast":
                                    e.PushEvent(AnimEvents.Action.Cast, priEvent.time);
                                    break;
                                case "end":
                                    e.PushEvent(AnimEvents.Action.End, priEvent.time);
                                    break;
                                case "sound":
                                    e.PushEvent(AnimEvents.Action.Audio, priEvent.time, priEvent.stringParameter);
                                    break;
                                case "shake":
                                    string[] temp = events[i].stringParameter.Split(',');
                                    if (temp.Length == 4)
                                    {
                                        float[] param = new float[4];
                                        if (float.TryParse(temp[0], out param[0]) && float.TryParse(temp[1], out param[1]) && float.TryParse(temp[2], out param[2]) && float.TryParse(temp[3], out param[3]))
                                        {
                                            e.PushEvent(AnimEvents.Action.Shake, priEvent.time, param);
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                        _gAnimEventsCache.Add(clip.GetInstanceID(), e);
                        _animEvents.Add(clip.name, e);
                    }
                    else
                    {
                        _animEvents.Add(clip.name, e.Clone());
                    }

                    clip.events = null;
                }
            }
        }

        List<AnimationClip> UniqueAnimationEvents(AnimationClip[] clips)
        {
            List<AnimationClip> list = new List<AnimationClip>();
            for (int i = 0; i < clips.Length; ++i)
            {
                bool exist = false;
                for (int j = 0; j < list.Count; ++j)
                {
                    if (list[j].name == clips[i].name)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    list.Add(clips[i]);
                }
            }
            return list;
        }
    }
    class AnimEvents
    {
        public enum Action
        {
            Invalid,
            Cast,
            End,
            Audio,
            Shake,
        }
        public struct Event
        {
            public Action ActionEx;
            public bool Invoked;
            public float Time;
            public string SParam;
            public float[] FParam;
        }

        Actor _owner;
        Event[] _events = null;
        int _idx = 0;
        float _beginTime = 0;

        public AnimEvents(int capacity)
        {
            _events = new Event[capacity];
            Clear(true);
            _idx = 0;
        }

        public AnimEvents Clone()
        {
            AnimEvents temp = new AnimEvents(_events.Length);
            for (int i = 0; i < _events.Length; ++i )
            {
                temp.PushEvent(_events[i].ActionEx, _events[i].Time, _events[i].SParam, _events[i].FParam);
            }
            return temp;
        }

        public void PushEvent(Action action, float time)
        {
            PushEvent(action, time, null, null);
        }

        public void PushEvent(Action action, float time, string str)
        {
            PushEvent(action, time, str, null);
        }

        public void PushEvent(Action action, float time, float[] f)
        {
            PushEvent(action, time, null, f);
        }

        public void StartTick()
        {
            _beginTime = GameTimer.time;
            _idx = 0;
        }

        public void RestartTick()
        {
            Clear(false);
            StartTick();
        }

        public void CUpdate(Actor owner, float speed)
        {
            float curTime = (GameTimer.time - _beginTime) * speed;
            for (int i = _idx; i < _events.Length; ++i)
            {
                Event e = _events[i];
                if (curTime < e.Time)
                {
                    break;
                }
                if (!e.Invoked)
                {
                    // Invoked和_idx的赋值，提前到switch前面：为了防止switch中代码出现异常，导致事件触发永不停止
                    _events[i].Invoked = true;
                    ++_idx;

                    switch (e.ActionEx)
                    {
                        case Action.Cast:
                            owner.SkillController.Caster.Cast();
                            break;
                        case Action.End:
                            owner.SkillController.Caster.CastFinish();
                            break;
                        case Action.Audio:
                            owner.SkillController.ActorAudioEx.Play(e.SParam);
                            break;
                        case Action.Shake:
                            CameraControl.Instance.Shake(e.FParam[0], e.FParam[1], (int)e.FParam[2], e.FParam[3]);
                            break;
                        case Action.Invalid:
                            break;
                    }
                }
            }
        }

        void PushEvent(Action action, float time, string str, float[] f)
        {
            if (_idx < _events.Length)
            {
                _events[_idx].ActionEx = action;
                _events[_idx].Time = time;
                _events[_idx].SParam = str;
                _events[_idx].FParam = f;
                _events[_idx].Invoked = false;
                ++_idx;
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        /// <param name="clearValid">是否清理可用标志位</param>
        void Clear(bool clearValid)
        {
            for (int i = 0; i < _events.Length; ++i) // 出于安全考虑，没有从_idx倒序清理
            {
                _events[i].ActionEx = clearValid ? Action.Invalid : _events[i].ActionEx;
                _events[i].Invoked = false;
            }
        }
    }
    #endregion
    #region 动画特效
    class AnimEffect : CompBase
    {
        // 这里的特效，都是动作特效，所以没有单独抽出一个特效组件
        ParticleSystem[] _allParticles;
        Dictionary<string, ParticleSystem> _animParticle = new Dictionary<string, ParticleSystem>();
        public float Speed
        {
            set
            {
                SpeedParticleSystems(value);
            }
        }

        public override void Init(Actor a)
        {
            base.Init(a);
            CollectAnimEffect();
        }

        public void SpeedParticleSystems(float speed)
        {
            if (_allParticles != null)
            {
                for (int i = 0; i < _allParticles.Length; ++i)
                {
                    _allParticles[i].playbackSpeed = speed;
                }
            }
        }
        public void CollectAnimEffect()
        {
            _allParticles = Owner.GetComponentsInChildren<ParticleSystem>(true);

            Transform ownerTransform = Owner.transform;
            int count = ownerTransform.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform child = ownerTransform.GetChild(i);
                if (!child.name.StartsWith("EffectA_")) // TODO:特效父节点名字要规范
                {
                    continue;
                }

                int c = child.childCount;
                for (int j = 0; j < c; ++j)
                {
                    Transform sub = child.GetChild(j);
                    _animParticle.Add(sub.name.ToLower(), sub.GetComponent<ParticleSystem>());
                }
                break;
            }
        }

        public void TryToPlay(string name)
        {
            ParticleSystem effect;
            if (_animParticle.TryGetValue(name, out effect))
            {
                effect.Clear(true);
                effect.Play();
            }
        }
    }
    #endregion
}
