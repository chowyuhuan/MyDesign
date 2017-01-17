using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Network;
using Platform;

public sealed class GUI_LoginUI_DL : GUI_Window_DL
{
    public Text VersionText;

    public Button StartButton;

    static string _platformLoginData = "";

    bool _LoginCallBackRegisted = false;

    void OnEnable()
    {
        PlatformInterface.OnPlatformLoginCallback += OnPlatformLogin;
    }

    void OnDisable()
    {
        PlatformInterface.OnPlatformLoginCallback -= OnPlatformLogin;
    }

    protected override void OnStart()
    {
        if (null != VersionText)
        {
            VersionText.text = "Ver:" + GameLogicController_DL.Instance.ClientVersion.ToString();
        }

        StartButton.interactable = false;
        PlatformInterface.CallPlatformLogin();
    }

    void EnterMainCity()
    {
        PL_Manager_DL.Instance.LoadSceneAsync("MainCity", true);
        GUI_Manager.Instance.PushWndToWaitingQueue("MainUI");
        HideWindow();
    }

    public void OnLoginButtonClicked()
    {
        if (!_LoginCallBackRegisted)
        {
            LoginHelper.OnFinished += OnFinished;
            _LoginCallBackRegisted = true;
        }

        StartButton.interactable = false;
        LoginHelper.StartLogin(_platformLoginData);

        //EnterMainCity();
    }

    void OnFinished(bool result)
    {
        StartButton.interactable = true;

        if (result)
        {
            LoginHelper.OnFinished -= OnFinished;
            _LoginCallBackRegisted = false;
            EnterMainCity();
        }
    }

    void OnPlatformLogin(string data)
    {
        //Debug.Log("OnPlatformLogin");
        _platformLoginData = data;

        StartButton.interactable = true;
    }
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_LoginUI dataComponent = gameObject.GetComponent<GUI_LoginUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_LoginUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            VersionText = dataComponent.VersionText;
            StartButton = dataComponent.StartButton;

            if (null != StartButton)
            {
                StartButton.onClick.AddListener(OnLoginButtonClicked);
            }
        }
    }
}
