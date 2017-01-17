using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProgressLoading;

public class PL_Manager_DL : MonoBehaviour
{
    GameObject LoadingUIObject;
    public PL_LoadingUI_DL LoadingUI = null;
    [HideInInspector]
    public string LoadingSceneName;

    public delegate void FloatDelegate(float _progress);
    public delegate void VoidDelegate();
    public FloatDelegate SendProgressChangedMsg; // 通知外部，主要用于表现
    public VoidDelegate SendStartMsg;            // 通知外部

    private List<PL_LoadingProcessor> _ProcessorList = new List<PL_LoadingProcessor>();
    private int _CurrentLoadingIdx = 0;
    private float _Progress = 0.0f;
    private bool _UI = false;
    private float _LoadingPeace;
    /// <summary>
    /// 单件访问入口
    /// </summary>
    public static PL_Manager_DL Instance
    {
        get;
        private set;
    }

    /// <summary>
    /// 模块准备工作
    /// 美术资源:loading界面相关表现资源
    /// 加载数据:SM_LoadingProcessor的初始化
    /// </summary>
    public void Prepare(string nextSceneName)
    {
        _CurrentLoadingIdx = 0;
        _ProcessorList.Clear();
        PL_AsyncSceneLoader sceneLoader = new PL_AsyncSceneLoader();
        sceneLoader.Prepare(nextSceneName, 1);
        _ProcessorList.Add(sceneLoader);
        _LoadingPeace = 1f / _ProcessorList.Count;
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="_sceneName">场景名称</param>
    public void LoadSceneAsync(string sceneName, bool _ui)
    {
        Prepare(sceneName); // 这行代码要放在预加载处，现在没有使用Assetbundle，所以，先放这里
        LoadingSceneName = sceneName;
        _Progress = 0.0f;
        _UI = _ui;
        if (_UI)
        {
            OpenLoadingUI();
        }
        if (SendStartMsg != null)
        {
            SendStartMsg();
        }
        StartCoroutine(Loading());
    }

    /// <summary>
    /// 附加式加载场景
    /// </summary>
    /// <param name="_scnenName">场景名称</param>
    public void LoadAdditiveScene(string _scnenName)
    {
        // TODO
    }

    /// <summary>
    /// 显示加载界面
    /// </summary>
    public void OpenLoadingUI()
    {
        if (null != LoadingUIObject)
        {
            LoadingUIObject.SetActive(true);
            if(null == LoadingUI)
            {
                LoadingUI = LoadingUIObject.GetComponent<PL_LoadingUI_DL>();
            }
        }
    }

    public void CloseLoadingUI()
    {
        if (null != LoadingUIObject)
        {
            LoadingUIObject.SetActive(false);
            if (null != LoadingUI)
            {
                LoadingUI.OnClose();
            }
        }
    }

    /// <summary>
    /// 进度变化回调
    /// </summary>
    /// <param name="_progress"></param>
    public void OnProgressChanged(float progress)
    {
        _Progress = progress;
        if (SendProgressChangedMsg != null)
        {
            SendProgressChangedMsg(_Progress);
        }
    }

    /// <summary>
    /// 加载结束回调
    /// </summary>
    public void OnLoadEnd()
    {
        GameApplication.ReleaseMemory();
        GUI_Manager.Instance.ShowWaitingQueue();
        if (_UI)
        {
            CloseLoadingUI();
        }
    }


    void Awake()
    {
        CopyDataFromDataScript();
        if (Instance == null)
        {
            Instance = this;
        }
        if (LoadingUIObject)
        {
            DontDestroyOnLoad(LoadingUIObject);
        }
    }

    IEnumerator Loading()
    {
        //yield return null; 如果点击切场景按钮时定帧，放开这个注释
        float totalProgress = 0;
        while (totalProgress < 1.0f)
        {
            float curLoadingProgress = _ProcessorList[_CurrentLoadingIdx].Load();
            totalProgress = _CurrentLoadingIdx * _LoadingPeace + curLoadingProgress;
            if (curLoadingProgress >= 1.0f)
            {
                ++_CurrentLoadingIdx;
            }
            if (_CurrentLoadingIdx == _ProcessorList.Count)
            {
                totalProgress = 1f;
            }
            OnProgressChanged(totalProgress);
            yield return null;
        }
        OnLoadEnd();
    }
    protected void CopyDataFromDataScript()
    {
        PL_Manager dataComponent = gameObject.GetComponent<PL_Manager>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：PL_Manager,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            LoadingUIObject = dataComponent.LoadingUIObject;
        }
    }
}
