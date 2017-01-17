using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_BGMoveController_DL : MonoBehaviour
{
    float _SingleBGLength = 91.4f;
    public Transform _LeftCullPoint;
    public Transform _RightCullPoint;
    Transform _Transform;
    Transform _CacheTransform
    {
        get
        {
            if (null == _Transform)
            {
                _Transform = transform;
            }
            return _Transform;
        }
    }
    public static GUI_BGMoveController_DL Instance
    {
        get;
        protected set;
    }

    void Awake()
    {
        CopyDataFromDataScript();
        Instance = this;
        _SingleBGLength = _RightCullPoint.position.x - _LeftCullPoint.position.x;
    }

    public void CameraMove(float x)
    {
        if (x < _LeftCullPoint.position.x)
        {
            _CacheTransform.position = new Vector3(_CacheTransform.position.x - _SingleBGLength, _CacheTransform.position.y, _CacheTransform.position.z);
        }
        else if (x > _RightCullPoint.position.x)
        {
            _CacheTransform.position = new Vector3(_CacheTransform.position.x + _SingleBGLength, _CacheTransform.position.y, _CacheTransform.position.z);
        }
    }
    protected void CopyDataFromDataScript()
    {
        GUI_BGMoveController dataComponent = gameObject.GetComponent<GUI_BGMoveController>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BGMoveController,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _LeftCullPoint = dataComponent._LeftCullPoint;
            _RightCullPoint = dataComponent._RightCullPoint;
        }
    }
}
