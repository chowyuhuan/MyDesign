using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace AssetUpdate
{
    public enum EUpdateState
    {
        LoadLocalVersion,
        CheckLocalVersion,
        LoadExcuteConfig,
        LoadCacheConfig,
        LoadServerConfig,
        CheckServerConfig,
        LoadServerVersionInfo,
        LoadLocalFileVer,
        LoadServerFileVer,
        CheckingFiles,
        NoFileUpDate,
        UpdatingFiles,
        Finished,
        DownLoadFileError,
        RestartApp,
        Failed,
        DiffAPK
    }

    public class AU_Manager : MonoBehaviour
    {
        public static AU_Manager Instance
        {
            get;
            protected set;
        }

        [SerializeField]
        Text UpdateStateTipText;
        [SerializeField]
        Slider UpdateSchedule;
        [SerializeField]
        GameObject MessageBoxProto;

        EUpdateState _UpdateState;

        string[] _UpdateStateTip = {
                                       "加载本地版本信息",/**< LoadLocalVersion */
                                       "检查本地版本信息",/**< CheckLocalVersion */
                                       "加载本地配置信息",/**< LoadExcuteConfig */
                                       "加载本地配置信息",/**< LoadCacheConfig */
                                       "加载服务器配置信息", /**< LoadServerConfig */
                                       "检查服务器配置信息", /**< CheckServerConfig */
                                       "加载服务器版本信息", /**< LoadServerVersionInfo */
                                       "加载本地文件版本信息", /**< LoadLocalFileVer */
                                       "加载服务器文件版本信息", /**< LoadServerFileVer */
                                       "检查资源",     /**< CheckingFiles */
                                       "没有文件需要更新",  /**< NoFileUpDate */
                                       "更新资源",     /**< UpdatingFiles */
                                       "更新完成",     /**< Finished */
                                       "更新过程中遇到异常", /**< DownLoadFileError */
                                       "需要重启",     /**< Restart */
                                       "更新失败",      /**< Failed */
                                       "重大版本更新",  /**< DiffAPK */
                        };

        void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        void Start()
        {
#if USE_ABAR
            AU_AppConfig.Init();
            AU_VersionControl.StartLocalVersionCheck();
#else
            EnterGame(new AU_VersionInfo(), false);
#endif
        }

        // Update is called once per frame
        void Update()
        {
            AU_WorkPipeLine.Update();
            AU_FileLoader.Update();
        }

        public void UpdateFail(EUpdateState updateState)
        {
            Debug.Log("[更新][失败]" + _UpdateStateTip[(int)updateState]);
            UpdateUI(updateState, _UpdateStateTip[(int)EUpdateState.Failed]);
            EnterGame(AU_VersionControl._ClientVersion, AU_VersionControl.InitFromCacheConfig);
        }

        public void SetUpdateState(EUpdateState updateState)
        {
            UpdateUI(updateState, _UpdateStateTip[(int)updateState]);
            switch(updateState)
            {
                case EUpdateState.Finished:
                case EUpdateState.NoFileUpDate:
                    {
                        EnterGame(AU_VersionControl._ClientVersion, AU_VersionControl.InitFromCacheConfig);
                        break;
                    }
                case EUpdateState.DiffAPK:
                    {
                        string msg = "当前版本有重大更新，请下载最新版本。（点击取消将关闭游戏）";
                        AU_MessageBox.ShowMessage("更新", msg, "下载", "关闭游戏", OnConfirm, OnCancel, AU_MessageBox.MessageType.ConfirmAndCancell, MessageBoxProto);
                        break;
                    }
            }
        }

        void OnConfirm()
        {
            string url;
            if (AU_AppConfig.GetConfigValue("LatestPackage", true, out url))
            {
                Application.Quit();
#if TEST
                Debug.Log("下载安装包：" + url);
#endif
                Application.OpenURL(url);
            }
            else
            {
                Application.Quit();
#if TEST
                Debug.LogError("没有配置更新包的下载路径！！！！");
#endif
            }
        }

        void OnCancel()
        {
            Application.Quit();
        }

        void EnterGame(AU_VersionInfo versionInfo, bool initFromRemote)
        {
            AU_UpdateMessage.Instance.SetUpdateMessage(versionInfo, initFromRemote);
            gameObject.AddComponent<JITLoader>();
        }

        void UpdateUI(EUpdateState updateState, string content)
        {
            if (_UpdateState != updateState)
            {
#if UNITY_EDITOR
                Debug.Log("[更新]" + content);
#endif
                if (null != UpdateStateTipText)
                {
                    UpdateStateTipText.text = content;
                }
                _UpdateState = updateState;
            }
            if (updateState == EUpdateState.UpdatingFiles)
            {
                int totalCount = AU_FileLoader.GetTaskCount(AU_FileLoader.ETask.Both);
                int finishedCount = AU_FileLoader.GetFinishedCount(AU_FileLoader.ETask.Both);
                UpdateSchedule.value = (float)finishedCount / totalCount;
            }
        }
    }
}
