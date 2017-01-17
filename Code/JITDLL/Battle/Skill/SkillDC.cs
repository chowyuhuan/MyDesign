using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;

/// <summary>
/// 技能进攻效果基本单元
/// 两部分组成：
///     1.效果类型（这个地方考虑，是否抽到buff中，不由SkillDC管理）
///     2.数值（需要bool类型，考虑使用Value > 0）
/// 无论技能多么复杂，都由1个或若干个SkillDC组成
/// buff由SkillDC产生 or 直接由SkillCaster释放？至少，命中类buff由SkillDC产生，瞬发类buff可以考虑由SkillCaster释放，但这样不统一
/// SkillDC可能会作为打算技能的发起者，所以，在碰撞后要做打断的发起调用（可以的话，做到能否打断其他技能的可控）
/// 仅仅挂SkillDC的对象使用碰撞的回调，其他对象（如被命中对象）不使用，减少不必要的调用开销
/// </summary>
public class SkillDC : MonoCompBase
{
    [HideInInspector]
    public int SkillId = 0;
    [HideInInspector]
    public DCMeta MetaEx = null;
    [HideInInspector]
    public int CollideCount = 0;
    Collider _collider;
    float _intervalTime = 0;
    int _frameIndex = -1;
    bool _permitInterval;

    public void OnMoveFinish(GameObject obj)
    {
        Debug.LogWarning(gameObject.name + "移动结束，看到这条log后查看OnMoveFinish是否有必要，没必要，删除");
        //SkillUtility.Terminate(MetaEx.TerminateModeEx, gameObject);
    }

    public void Countdown()
    {
        StartCoroutine(CountdownToTerminate());
        _intervalTime = -1000;
    }

    void OnCollide(Collider other)
    {
        Actor target = other.gameObject.GetComponent<Actor>();
        if (target.IsDeath) return;


        target.SkillController.Mixer.Attacked(this, _collider);

        if (++CollideCount >= MetaEx.CollideLimit)
        {
            _collider.enabled = false;
            SkillUtility.Terminate(MetaEx.TerminateModeEx, gameObject, Owner);
        }
    }

    void Start()
    {
        _collider = GetComponent<Collider>();
        Countdown();
    }

    IEnumerator CountdownToTerminate()
    {
        yield return Yielders.GetWaitForSeconds(MetaEx.Motion.LifeCycle);
        SkillUtility.Terminate(MetaEx.TerminateModeEx, gameObject, Owner);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == BattleBound.ColliderEx)
        {
            return;
        }

        if (MetaEx.Motion.ImpactWayEx == ImpactWay.Once)
        {
            OnCollide(other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other == BattleBound.ColliderEx)
        {
            return;
        }

        if (MetaEx.Motion.ImpactWayEx == ImpactWay.Interval)
        {
            if (_frameIndex != GameTimer.frameCount)
            {
                _frameIndex = GameTimer.frameCount;
                _permitInterval = GameTimer.time - _intervalTime >= MetaEx.Motion.Interval;
                if (_permitInterval)
                {
                    _intervalTime = GameTimer.time;
                }
            }

            if (_permitInterval)
            {
                OnCollide(other);
            }
        }
    }


    /// 可能需要返回数值接口
    /// 可能需要返回效果类型接口
    /// 可能返回运算操作符（与其他SkillDC计算产生最终作用结果值）
    /// 




    // ----------------------------   先不删，如果最终确定继承MonoBehaviour，就删   --------------------------------------------
    //bool Destroyed = true;
    ///// 先按照MonoBehaviour的机制，如果去掉继承MonoBehaviour，Destroyed = false放在构造函数中
    //void Awake()
    //{
    //    Destroyed = false;
    //}

    //public void Destroy()
    //{
    //    Destroyed = true;
    //}

    //public static bool operator == (SkillDC _x, SkillDC _y)
    //{
    //    if (_y as object == null)
    //    {
    //        return _x.Destroyed || _x as object == null;
    //    }

    //    if (_x as object == null)
    //    {
    //        return _y.Destroyed || _y as object == null;
    //    }

    //    return _x as object == _y as object;
    //}

    //public static bool operator != (SkillDC _x, SkillDC _y)
    //{
    //    return !(_x == _y);
    //}
}
