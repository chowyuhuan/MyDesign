using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using AssetManage;

namespace ProgressLoading
{
    public class PL_AsyncSceneLoader : PL_LoadingProcessor
    {
        private AsyncOperation _Async = null; // 异步进度
        private string SceneName;
        private bool _StartLoad = false;

        /// <param name="_strParam">场景名称</param>
        /// <param name="_intParam">缓冲帧数</param>
        public override void Prepare(string _strParam, int _intParam)
        {
            SceneName = _strParam;
        }

        public override float Load()
        {
            if (_Async == null)
            {
                if (!_StartLoad)
                {
                    AM_LoadLevelOperation llo = AM_Manager.LoadSceneAsync(SceneName, LoadSceneMode.Single, OnSceneLoaded);
                    _Async = llo._LoadLevelRequest;
                    _StartLoad = true;
                }
                return Progress(0.0f);
            }
            else if (!_Async.isDone)
            {
                return Progress(_Async.progress / 0.901f);
            }
            else
            {
                AM_Manager.UnloadAsset(SceneName);
                return Progress(1.0f);
            }
        }

        void OnSceneLoaded(AM_LoadOperation op)
        {
            if (op._LoadError == null)
            {
                AM_LoadLevelOperation lo = op as AM_LoadLevelOperation;
                if (lo != null)
                {
                    _Async.allowSceneActivation = true;
                }
            }
            else
            {
                Debug.LogError("【加载场景】" + op._LoadError);
            }
        }
    }
}
