using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_Root_DL : MonoBehaviour
{
    public static GUI_Root_DL Instance { get; protected set; }
    public GameObject _RootObject;
    public GameObject GUIRootObject
    {
        get
        {
            if (_RootObject == null)
            {
                _RootObject = GameObject.Find("GameUI");
            }
            return _RootObject;
        }
    }

    public Camera _UICamera;
    public Camera UICamera { get { return _UICamera; } }
    public CanvasScaler _ScreenScaler;
    public CanvasScaler ScreenScaler { get { return _ScreenScaler; } }
    void Awake()
    {
        CopyDataFromDataScript();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int layerMask = 1 << layer;
        _UICamera.cullingMask |= layerMask;
    }

    public void HideLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int layerMask = 1 << layer;
        layerMask = ~layerMask;
        _UICamera.cullingMask &= layerMask;
    }

    protected void CopyDataFromDataScript()
    {
        GUI_Root dataComponent = gameObject.GetComponent<GUI_Root>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_Root,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _RootObject = dataComponent._RootObject;
            _UICamera = dataComponent._UICamera;
            _ScreenScaler = dataComponent._ScreenScaler;
        }
    }
}
