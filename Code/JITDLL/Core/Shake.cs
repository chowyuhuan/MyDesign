using UnityEngine;
using System.Collections;


public class Shake
{
    Vector3 _directon = Vector3.zero;
    float _maxRange = 0;
    float _curRange = 0;
    int _sectionCount = 0;
    float _totalTime = 0;
    float _passedTime = 0;
    bool _shaking = false;
    ShakeSection _shakeSection = new ShakeSection();

    public void TryToShake(float degrees, float range, int count, float time)
    {
        if (range <= _curRange)
        {
            return;
        }

        degrees += 180f; // 摄像机运动的方向与实际方向相反
        _directon.x = Mathf.Cos(degrees * Mathf.Deg2Rad);
        _directon.y = Mathf.Sin(degrees * Mathf.Deg2Rad);
        _directon.z = 0;

        _maxRange = range;
        _curRange = 0;
        _sectionCount = count;
        _totalTime = time;
        _passedTime = 0;
        _shaking = true;
        _shakeSection.Begin(0, _maxRange, _totalTime / _sectionCount, _passedTime);
    }

    public Vector3 Update()
    {
        if (_shaking)
        {
            int index = GetSectionIndex();
            if (!_shakeSection.Shaking(index))
            {
                float range = _maxRange - _maxRange / _sectionCount * index;
                range *= Mathf.Pow(-1, index);
                float sectionTime = _totalTime / _sectionCount;
                _shakeSection.Begin(index, range, _totalTime / _sectionCount, _passedTime - sectionTime * index);
            }
            _curRange = _shakeSection.Update();
            _passedTime += GameTimer.deltaTime;
            if (_passedTime >= _totalTime)
            {
                _curRange = 0;
                _shaking = false;
            }
        }
        return _directon * _curRange;
    }

    public bool Shaking()
    {
        return _shaking;
    }

    int GetSectionIndex()
    {
        return (int)(_passedTime * _sectionCount / _totalTime);
    }
}

public class ShakeSection
{
    int _key = -1;
    float _range = 0;
    float _totalTime = 0;
    bool _end = false;
    float _passedTime = 0;

    public bool Shaking(int key)
    {
        return _key == key;
    }

    public void Begin(int key, float range, float totalTime, float startTime)
    {
        _key = key;
        _range = range;
        _totalTime = totalTime;
        _passedTime = startTime;
    }

    public float Update()
    {
        _passedTime += GameTimer.deltaTime;
        float curPos = 0;
        if (_passedTime > _totalTime / 2f) // 后半段
        {
            curPos = _range - (_range / _totalTime * _passedTime);
        }
        else
        {
            curPos = _range / _totalTime * _passedTime;
        }
        if (_passedTime >= _totalTime)
        {
            _passedTime = 0;
            _totalTime = 0;
            _range = 0;
            _end = true;
            curPos = 0;
        }
        return curPos;
    }

    public bool End()
    {
        return _end;
    }
}


// 胡克定律方式，这种方式因为计算问题，停止的判断有困难，不是很可控
public class Shake1
{
    float _springFactor = 2;
    float _damping = 4;
    float _time = 5;
    Vector3 _curPos;
    Vector3 _originPos;
    Vector3 _velocity = Vector3.zero;
    bool _shaking = false;
    float _passedTime = 0;

    public bool Shaking()
    {
        return _shaking;
    }

    public void TryToShake(Vector3 origin, float degrees, float velocity, float springFactor, float damping, float time)
    {
        if (!_shaking || Override(velocity))
        {
            degrees += 180f; // 摄像机运动的方向与实际方向相反
            _originPos = origin;
            _curPos = _originPos;
            _velocity.x = Mathf.Cos(degrees * Mathf.Deg2Rad) * velocity; // 放大velocity配置的值
            _velocity.y = Mathf.Sin(degrees * Mathf.Deg2Rad) * velocity; // 放大velocity配置的值
            _springFactor = springFactor;
            _damping = damping;
            _time = time;
            _passedTime = 0;
            _shaking = true;
        }
    }

    /// <summary>
    /// 更新位置并返回基于原始位置的差量（原始位置不动）
    /// </summary>
    /// <returns>基于原始位置的差量</returns>
    public Vector3 Update_DeltaOnBase()
    {
        if (_shaking)
        {
            if (UpdateAndCheckFinished())
            {
                return Vector3.zero;
            }
            return _curPos - _originPos;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// 更新位置并返回基于上次位置的差量（相当于速度）
    /// </summary>
    /// <returns>基于上次位置的差量</returns>
    public Vector3 Update_DeltaOnPre()
    {
        if (_shaking)
        {
            Vector3 delta = _velocity * GameTimer.deltaTime;
            if (UpdateAndCheckFinished())
            {
                return Vector3.zero;
            }
            return delta;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// 更新位置并返回绝对位置
    /// </summary>
    /// <returns>绝对位置</returns>
    public Vector3 Update_AbsolutePos()
    {
        if (_shaking)
        {
            UpdateAndCheckFinished();
            return _curPos;
        }
        else
        {
            return _originPos;
        }
    }

    bool UpdateAndCheckFinished()
    {
        _curPos += _velocity * GameTimer.deltaTime;
        Vector3 force = _springFactor * (_curPos - _originPos) + _velocity * _damping; // 胡克定律 + 与速度成正比的阻尼因子形成的阻力；*0.01f是为了放大_springFactor和_damping填写时的数值，否则会是很小的小数，不直观
        _velocity -= force * GameTimer.deltaTime; // 阻力作为加速度影响速度

        _passedTime += GameTimer.deltaTime;
        if (_passedTime >= _time)
        {
            _shaking = false;
            _curPos = _originPos;
            _passedTime = 0;
            return true;
        }
        return false;
    }

    bool Override(float velocity)
    {
        return velocity >= _velocity.magnitude;
    }
}

// 改变摄像机投影矩阵方式
// 计算量有些大
public class Shake2
{
    void SetVanishingPoint(Camera cam, Vector2 perspectiveOffset)
    {
        var m = cam.projectionMatrix;
        var w = 2 * cam.nearClipPlane / m.m00;
        var h = 2 * cam.nearClipPlane / m.m11;

        var left = -w / 2 - perspectiveOffset.x;
        var right = left + w;
        var bottom = -h / 2 - perspectiveOffset.y;
        var top = bottom + h;

        cam.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
    }

    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = (2f * near) / (right - left);
        float y = (2f * near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2f * far * near) / (far - near);
        float e = -1f;

        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0f;
        m[0, 2] = a;
        m[0, 3] = 0f;

        m[1, 0] = 0f;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0f;

        m[2, 0] = 0f;
        m[2, 1] = 0f;
        m[2, 2] = c;
        m[2, 3] = d;

        m[3, 0] = 0f;
        m[3, 1] = 0f;
        m[3, 2] = e;
        m[3, 3] = 0f;
        return m;
    }
}