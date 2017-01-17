using UnityEngine;
using System.Collections;
using System;

namespace AssetUpdate
{
    public abstract class AU_FileVersionInfoFetcher : AU_WWWFileFetcher
    {
        public string _VersionType { get; protected set; }
        protected bool _Local;
        public AU_BranchesVer _Branches { get; protected set; }
        protected bool _FetchSuccess = false;

        public AU_FileVersionInfoFetcher(bool local)
            :base()
        {
            _Local = local;
            _Branches = new AU_BranchesVer();
        }

        protected override void EndWorkFlow()
        {
            try
            {
                if (string.IsNullOrEmpty(_WWWFileLoader.error) == false) /// 失败
                {
#if UNITY_EDITOR
                    Debug.LogError("[更新]从" + _VersionType + "获取版本文件失败! Err: " + _WWWFileLoader.error);
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
                            Debug.Log("[更新]从服务器获取版本文件失败!网络不可达！");
#endif
                            _FetchSuccess = false;
                            return;
                        }
                    }
#if UNITY_EDITOR
                    Debug.Log("[更新]" + _VersionType + _WWWFileLoader.text);
#endif
                    _FetchSuccess = _Branches.LoadBranchesVerFromString(_WWWFileLoader.text);
                }
            }
            catch (System.Exception ex)
            {
                _FetchSuccess = false;
#if UNITY_EDITOR
                Debug.Log("[更新]获取版本异常：" + ex.ToString());
#endif
            }
            OnGetBranches(_FetchSuccess);
        }

        protected abstract void OnGetBranches(bool success);
        protected virtual void OnGetAllFileVer()
        {
            AU_VersionControl.OnGetAllLocalFileVer(_Branches);
        }
    }
}
