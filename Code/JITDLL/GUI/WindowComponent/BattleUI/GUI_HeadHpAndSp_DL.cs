using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_HeadHpAndSp_DL : GUI_LogicObject
{
    GameObject HpSliderObject;
    GameObject SpSliderObject;
    public GUI_SpriteSlider_DL _HpSlider;
    public GUI_SpriteSlider_DL _SpSlider;
    public float HeadUpDistance;
    public float UpStepDistance = 20f;
    public int _CurHp { get; protected set; }
    public int _MaxHp { get; protected set; }
    public int _Level { get; protected set; }
    public int _Star { get; protected set; }
    public bool _HasSp { get; protected set; }
    public bool _HasStar { get; protected set; }
    public bool _HeroHeadHp { get; protected set; }
    public string _TargetName { get; protected set; }

    public Sprite _HeadIconSprite { get; protected set; }
    public Actor _TargetActor { get; protected set; }
    public int SortIndex { get; set; }
    protected GUI_EnemyInfo_DL _AttachDisplay;

    public void Display(Actor target, int maxHp, int star, int level, string targetName, bool heroHeadHp, float headUpDistance, float overlayDistance, Sprite headIcon = null)
    {
        if(null == _HpSlider)
        {
            _HpSlider = HpSliderObject.GetComponent<GUI_SpriteSlider_DL>();
        }
        if(null == _SpSlider)
        {
            _SpSlider = SpSliderObject.GetComponent<GUI_SpriteSlider_DL>();
        }


        _TargetActor = target;
        if (!heroHeadHp)
        {
            HeadUpDistance = headUpDistance;
            UpStepDistance = overlayDistance;
        }
        else
        {
            DataCenter.Hero hero = DataCenter.PlayerDataCenter.GetHero(_TargetActor.ServerId);
            if(null != hero)
            {
                CSV_b_skill_template skill = CSV_b_skill_template.FindData((int)hero.SkillServerId);
                _HasSp = (null != skill);
            }
        }
        Init(maxHp, star, level, targetName, heroHeadHp, headIcon);
        RegistEvent();
        GUI_Tools.CommonTool.AddChild(target.gameObject, CachedGameObject);
    }

    void Init(int maxHp, int star, int level, string targetName, bool heroHeadHp, Sprite headIcon = null)
    {
        _CurHp = maxHp;
        _MaxHp = maxHp;
        _Star = star;
        _HasStar = _Star > 0;
        _Level = level;
        _TargetName = targetName;
        _HeroHeadHp = heroHeadHp;
        _HeadIconSprite = headIcon;
#if UNITY_EDITOR
        Debug.Assert(null != _HpSlider);
        Debug.Assert(null != _SpSlider);
#endif
        _SpSlider.gameObject.SetActive(_HasSp);
        OnUpdateHp(0);
        OnUpdateSp(0);
    }

    public bool Overlap(GUI_HeadHpAndSp_DL otherHeadHpAndSp)
    {
        if (null != otherHeadHpAndSp)
        {
            return this.HeadUpDistance == otherHeadHpAndSp.HeadUpDistance;
        }
        return false;
    }

    public void AttachDisplay(GUI_EnemyInfo_DL enemyInfo)
    {
        _AttachDisplay = enemyInfo;
    }

    public void OnDamage(Actor attack, Actor attacked, int delta)
    {
        if (attacked.BattleId == _TargetActor.BattleId)
        {
            OnUpdateHp(-delta);
        }
        GUI_WarnNumberManager_DL.Instance.WarnNumber(attacked, EWarnNumberType.Damage, delta, attacked.SelfCamp, SortIndex);
    }

    public void OnCure(Actor attack, Actor attacked, int delta)
    {
        if (attacked.BattleId == _TargetActor.BattleId)
        {

            OnUpdateHp(delta);
        }
        GUI_WarnNumberManager_DL.Instance.WarnNumber(_TargetActor, EWarnNumberType.Cure, delta, _TargetActor.SelfCamp, SortIndex);
    }

    public void OnCrit(Actor attack, Actor attacked, int delta)
    {
        if (attacked.BattleId == _TargetActor.BattleId)
        {
            OnUpdateHp(-delta);
        }
        GUI_WarnNumberManager_DL.Instance.WarnNumber(_TargetActor, EWarnNumberType.Crit, delta, _TargetActor.SelfCamp, SortIndex);
    }

    public void OnUpdateHp(int hp)
    {
        _CurHp += hp;
        _CurHp = Mathf.Clamp(_CurHp, 0, _MaxHp);
        _HpSlider.Value = (float)_CurHp / _MaxHp;
        if (_AttachDisplay != null)
        {
            _AttachDisplay.Attach(this);
        }
    }

    public void OnUpdateSp(float sp)
    {
        _SpSlider.Value = sp;
        if (_AttachDisplay != null)
        {
            _AttachDisplay.EnemyInfoChange(this);
        }
    }

    public static void SortHpList(List<GUI_HeadHpAndSp_DL> hpList)
    {
        if (null != hpList && hpList.Count > 0)
        {
            for (int bubblePos = hpList.Count - 1; bubblePos >= 0; --bubblePos)
            {
                for (int index = 0; index < bubblePos; ++index)
                {
                    if (CompareByTargetZDepth(hpList[index], hpList[index + 1]) < 0)
                    {
                        GUI_HeadHpAndSp_DL temp = hpList[index];
                        hpList[index] = hpList[index + 1];
                        hpList[index + 1] = temp;
                    }
                }
                hpList[bubblePos].SortIndex = bubblePos;
            }
            hpList[0].SortIndex = 0;
        }
    }

    public static int CompareByTargetZDepth(GUI_HeadHpAndSp_DL a, GUI_HeadHpAndSp_DL b)
    {
        if (a == b)
        {
            return 0;
        }
        if (null != a && null != b)
        {
            /* 当前所有的actor的z值都相同，使用默认的节点排序比较深度，如果改变摄像机视角后actor的深度值有差异时就使用深度值比较；
            bool useDepth = false;
            if(useDepth)
            {
                float distance = a._TargetActor.transform.position.z - b._TargetActor.transform.position.z;
                if (distance < 0f)
                {
                    return -1;
                }
                else if (distance > 0f)
                {
                    return 1;
                }
                return 0;
            }
            else
             * */
            {
                int distance = a._TargetActor.transform.GetSiblingIndex() - b._TargetActor.transform.GetSiblingIndex();
                if (distance < 0)
                {
                    return -1;
                }
                else if (distance > 0)
                {
                    return 1;
                }
                return 0;
            }
        }
        else if (null == a)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    public void OnUpdatePos(Actor actor)
    {
        int upDistance = GUI_HpController_DL.Instance.GetUpDistance(SortIndex, _HeroHeadHp);
        CachedTransform.position = new Vector3(actor.transform.position.x, actor.transform.position.y + HeadUpDistance + upDistance * UpStepDistance, actor.transform.position.z);
        CachedTransform.LookAt(Camera.main.transform.position, Vector3.down);
    }

    public void OnTargetDead(Actor actor)
    {
        UnRegistEvent();
        Recycle();
        GUI_HpController_DL.Instance.RecycleHpItem(this, _HeroHeadHp);
    }

    protected override void OnRecycle()
    {
        CachedTransform.position = new Vector3(0f, 0f, 10000f);
        _CurHp = 0;
        _HasSp = false;
        _TargetName = null;
        if (_AttachDisplay != null)
        {
            _AttachDisplay.Dettach();
        }
    }

    void OnDestroy()
    {
        UnRegistEvent();
    }

    void RegistEvent()
    {
        if (null != _TargetActor)
        {
            _TargetActor.SkillController.Mixer.RegisterEffectNotify(SKILL.MixEffect.Damage, OnDamage);
            _TargetActor.SkillController.Mixer.RegisterEffectNotify(SKILL.MixEffect.Cure, OnCure);
            _TargetActor.SkillController.Mixer.RegisterEffectNotify(SKILL.MixEffect.Crit, OnCrit);
            _TargetActor.ActorReference.ActorMovementEx.OnMovePosition += OnUpdatePos;
            _TargetActor.ActorReference.ActorSpEx.OnSpProgressChange += OnUpdateSp;

            _TargetActor.OnDeath += OnTargetDead;
            _TargetActor.SkillController.Caster.OnStartCast += OnSkillStartCast;
        }
    }

    void UnRegistEvent()
    {
        if (null != _TargetActor)
        {
            _TargetActor.SkillController.Mixer.RemoveOnEffectNotify(SKILL.MixEffect.Damage, OnDamage);
            _TargetActor.SkillController.Mixer.RemoveOnEffectNotify(SKILL.MixEffect.Cure, OnCure);
            _TargetActor.SkillController.Mixer.RemoveOnEffectNotify(SKILL.MixEffect.Crit, OnCrit);
            _TargetActor.ActorReference.ActorMovementEx.OnMovePosition -= OnUpdatePos;
            _TargetActor.ActorReference.ActorSpEx.OnSpProgressChange -= OnUpdateSp;
            _TargetActor.OnDeath -= OnTargetDead;
            _TargetActor.SkillController.Caster.OnStartCast -= OnSkillStartCast;
        }
    }

    void OnSkillStartCast(SKILL.Skill skill, int block)
    {
        GUI_BattleManager.Instance.SkillCastWarn(_TargetActor, skill);
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_HeadHpAndSp dataComponent = gameObject.GetComponent<GUI_HeadHpAndSp>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeadHpAndSp,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HpSliderObject = dataComponent._HpSlider;
            SpSliderObject = dataComponent._SpSlider;
            HeadUpDistance = dataComponent.HeadUpDistance;
            UpStepDistance = dataComponent.UpStepDistance;
        }
    }
}
