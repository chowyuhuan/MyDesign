using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssetUpdate
{
    public abstract class AU_ConfigFetcher : AU_WWWFileFetcher
    {
        public string _ConfigType{get;protected set;}
        protected bool _Local;
        protected bool _FetchSuccess = false;

        public AU_ConfigFetcher(bool local)
            : base()
        {
            _Local = local;
        }

        protected override void EndWorkFlow()
        {
            try
            {
                if (string.IsNullOrEmpty(_WWWFileLoader.error) == false) /// 失败
                {
#if UNITY_EDITOR
                    Debug.Log("[更新]从" + _ConfigType + "获取配置文件失败! Err: " + _WWWFileLoader.error);
#endif
                    _FetchSuccess = false;
                }
                else /// 成功
                {
                    if (!_Local)
                    {
                        if (_WWWFileLoader.text.Contains("<html>")) // 这是Unity的bug，先这样处理一下。待Unity解决之后，再去掉
                        {
#if UNITY_EDITOR
                            Debug.Log("[更新]从服务器获取配置文件失败!网络不可达！");
#endif
                            _FetchSuccess = false;
                            return;
                        }
                    }
#if UNITY_EDITOR
                    Debug.Log("[更新]从" + _ConfigType + "获取配置文件成功  ");
#endif
                    _FetchSuccess = AU_AppConfig.ParseConfigFromLines(_WWWFileLoader.text.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries), _Local);
                }
            }
            catch (System.Exception ex)
            {
                _FetchSuccess = false;
                Debug.Log("[更新]异常：" +_ConfigType + ": " + ex.ToString());
            }
            OnGetConfig(_FetchSuccess);
        }
        protected abstract void OnGetConfig(bool success);
    }
}
