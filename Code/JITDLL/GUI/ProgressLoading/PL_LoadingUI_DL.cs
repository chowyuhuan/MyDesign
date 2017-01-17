using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PL_LoadingUI_DL : MonoBehaviour
{
    Slider _ProgressBar = null;

    void OnEnable()
    {
        PL_Manager_DL.Instance.SendProgressChangedMsg += OnProgressChanged;
        PL_Manager_DL.Instance.SendStartMsg += OnStartLoading;
    }

    void OnDisable()
    {
        PL_Manager_DL.Instance.SendProgressChangedMsg -= OnProgressChanged;
        PL_Manager_DL.Instance.SendStartMsg -= OnStartLoading;
    }

    void OnStartLoading()
    {
        if (_ProgressBar)
        {
            _ProgressBar.value = 0;
        }
    }

    /// <summary>
    /// 进度变化
    /// </summary>
    /// <param name="_progress">0~100</param>
    void OnProgressChanged(float progress)
    {
        if (_ProgressBar)
        {
            _ProgressBar.value = progress;
        }
    }

    public void OnClose()
    {
    }

    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        PL_LoadingUI dataComponent = gameObject.GetComponent<PL_LoadingUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：PL_LoadingUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _ProgressBar = dataComponent._ProgressBar;
        }
    }
    #endregion
}
