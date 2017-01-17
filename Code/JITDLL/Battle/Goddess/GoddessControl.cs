using UnityEngine;
using System.Collections;
using SKILL;
using ACTOR;

public class GoddessControl : MonoBehaviour {

    GoddessInBattle _owner;

    Vector3 _toPos;

    Team _team;

    int _skillId;

    float _followDistance;
    float _floatHeight;

    float _speed;
    float _realSpeed;
    float _runFastDistance;

    float _distance;

    Vector3 _direction;

    Animator _animator;

    //ParticleSystem[] _readyParticles;
    GameObject _readyParticleGo;

    float _speedRate = 1;
    public float SpeedRate
    {
        set
        {
            _speedRate = value;
            if (_speedRate < 0) _speedRate = 1;

            _animator.speed = _speedRate;
        }
    }

	// Use this for initialization
	void Start () {
	
	}

    public void Initialize(GoddessPrepareInfo goddessPrepareInfo, Team team, GoddessInBattle owner)
    {
        _owner = owner;
        _team = team;

        _skillId = goddessPrepareInfo.SkillId;

        _followDistance = DefaultConfig.GetFloat("GoddessFollowDistance");
        _floatHeight = DefaultConfig.GetFloat("GoddessfloatHeight");
        _speed = DefaultConfig.GetFloat("ActorMoveSpeed");
        _runFastDistance = DefaultConfig.GetFloat("RunFastDistance");

        _animator = GetComponent<Animator>();

        CollectReadyParticles();

        SetOrigPostion();

        StartCoroutine(UpdateToPos());
        StartCoroutine(UpdatePosition());

        PlayAnimation("run");
    }

    void CollectReadyParticles()
    {
        //_readyParticles = GetComponentsInChildren<ParticleSystem>(true);
        _readyParticleGo = transform.FindChild("GodEffect_SkillReady").gameObject;
        _readyParticleGo.SetActive(false);
    }

    public void SetReadyParticles(bool state)
    {
        if (_readyParticleGo.activeSelf != state)
        {
            _readyParticleGo.SetActive(state);
        }
    }

    void SetOrigPostion()
    {
        transform.position = GetToPos();
    }

    Vector3 GetToPos()
    {
        float frontLine = _team.Frontline;

        frontLine += _team.CampEx == Camp.Comrade ? -_followDistance : _followDistance;

        return new Vector3(frontLine, _floatHeight, 0);
    }

    IEnumerator UpdateToPos()
    {
        while(true)
        {
            _toPos = GetToPos();

            yield return Yielders.GetWaitForSeconds(1);
        }
    }

    IEnumerator UpdatePosition()
    {
        while(true)
        {
            _distance = Vector3.Distance(transform.position, _toPos);
            if (_distance > 0.1f)
            {
                _realSpeed = _speed;
                if (_distance > _runFastDistance)
                {
                    _realSpeed *= 3;
                }

                _direction = _toPos - transform.position;

                transform.position += _direction.normalized * _realSpeed * GameTimer.deltaTime * _speedRate;
            }

            yield return null;
        }
    }

    public void SetSpeedRate(float speedRate, float duration)
    {
        this.SpeedRate = speedRate;

        StopCoroutine("SpeedRateRecover");

        StartCoroutine(SpeedRateRecover(duration));
    }

    IEnumerator SpeedRateRecover(float duation)
    {
        yield return Yielders.GetWaitForSeconds(duation);

        SpeedRate = 1f;
    }

    public void PlayAnimation(string animationName)
    {
        _animator.CrossFadeInFixedTime(Animator.StringToHash(animationName), 0.2f, -1, 0);
    }

    public void cast()
    {
        //Debug.Log("cast");

        if (!_team.Death())
        {
            SkillControl.ExcuteSkillBehaviors(_skillId, _team.Leader);
        }
    }

    public void end()
    {
        //Debug.Log("end");

        PlayAnimation("run");

        _owner.CheckReadyParticles();
    }

    public void sound(string soundName)
    {
        AudioManager.Instance.PlaySound(soundName);
    }

    public void shake(string shakeParam)
    {
        string[] temp = shakeParam.Split(',');
        if (temp.Length == 4)
        {
            float[] param = new float[4];
            if (float.TryParse(temp[0], out param[0]) && float.TryParse(temp[1], out param[1]) && float.TryParse(temp[2], out param[2]) && float.TryParse(temp[3], out param[3]))
            {
                CameraControl.Instance.Shake(param[0], param[1], (int)param[2], param[3]);
            }
        }
    }
}
