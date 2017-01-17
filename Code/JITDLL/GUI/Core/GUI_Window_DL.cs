using UnityEngine;
using System.Collections;

public class GUI_Window_DL : MonoBehaviour
{
    public bool CloseOnEscape = true;
    public GameObject WindowObject { get; protected set; }
    public string WindowName { get; protected set; }
    private bool _Visual { get; set; }

    public string Sound = "";
    public float Delay = 0;

    void Awake()
    {
        CopyDataFromDataScript();
        WindowObject = gameObject;
        _Visual = false;
        OnAwake();
    }

    // Use this for initialization
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (false == _Visual)
        {
            _Visual = true;
            PostShowWindow();
        }
        OnUpdate();
    }

    public void SetWindowName(string windowName)
    {
        WindowName = windowName;
    }

    public bool ValidWindow()
    {
        return null != WindowObject;
    }

    public void ShowWindow()
    {
        if (null == WindowObject)
        {
            WindowObject = gameObject;

        }
        PreShowWindow();
        DoShow();
    }

    void DoShow()
    {
        _Visual = true;
        WindowObject.SetActive(true);
        GUI_Manager.Instance.RegistWindow(WindowName, this);

        if (!string.IsNullOrEmpty(Sound))
        {
            AudioManager.Instance.PlaySound(Sound, Delay);
        }
    }

    public void HideWindow()
    {
        if (ValidWindow())
        {
            PreHideWindow();
            DoHide();
            PostHideWindow();
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("Invallid Window : " + WindowName);
        }
#endif
    }

    void DoHide()
    {
        _Visual = false;
        GUI_Manager.Instance.UnRegistWindow(WindowName);
        GUI_Manager.Instance.ReleaseWindowRes(this);
    }


    void OnDestroy()
    {
        GUI_Manager.Instance.ReleaseWindowRes(this);
        OnDestroyed();
    }

    public void OnEscape()
    {
        if (CloseOnEscape)
        {
            HideWindow();
        }
    }


    protected virtual void OnAwake() { }

    protected virtual void OnStart() { }

    protected virtual void OnUpdate() { }
    protected virtual void OnDestroyed() { }

    public virtual void PreShowWindow() { }

    public virtual void PostShowWindow() { }

    public virtual void PreHideWindow() { }

    public virtual void PostHideWindow() { }
    protected virtual void CopyDataFromDataScript()
    {
        GUI_Window dataComponent = gameObject.GetComponent<GUI_Window>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_Window,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            CloseOnEscape = dataComponent.CloseOnEscape;
            Sound = dataComponent.Sound;
            Delay = dataComponent.Delay;
            WindowObject = dataComponent.gameObject;
            if(null != dataComponent.CloseButton)
            {
                dataComponent.CloseButton.onClick.AddListener(HideWindow);
            }
        }
    }
}
