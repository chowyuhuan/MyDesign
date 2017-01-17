using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadAssetFromAB : AM_LoadAssetOperation
    {
        public bool _AutoUnloadAssetBundle { get; protected set; }
        public string _AssetBundleName { get; protected set; }
        protected AssetBundleRequest _AssetBundleRequest;
        protected Object _Asset;
        public AM_LoadAssetFromAB(string assetPath, string abName, System.Type type, bool autoUnloadAB, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack loadCallBack)
            : base(assetPath, type, postProcessor, loadCallBack)
        {
            _AssetBundleName = abName;
            _AutoUnloadAssetBundle = autoUnloadAB;
        }

        public override void PostProcess()
        {
            if(null != _PostProcessor)
            {
                _Asset = _PostProcessor.PostProcessAsset(_AssetBundleRequest.asset);
            }
            AM_LoadOperationManager.RemoveLoadingAsset(_AssetName);
            if(_AutoUnloadAssetBundle)
            {
                AM_AssetRepository.UnLoadAssetBundle(_AssetBundleName);
            }
        }

        public override T GetAsset<T>()
        {
            return _Asset as T;
        }

        public override bool Update()
        {
            if (_AssetBundleRequest != null)
            {
                return false;
            }

            string error;
            AM_LoadedAB loadedBundle = AM_AssetRepository.GetLoadedAssetBundle(_AssetBundleName, out error);
            _LoadError = error;
            if (loadedBundle != null)
            {
                _AssetBundleRequest = loadedBundle._Asset.LoadAssetAsync(_AssetName, _AssetType);
                return false;
            }
            else if (_LoadError != null)//error happens
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            if (_AssetBundleRequest == null && _LoadError != null)
            {
                return true;
            }

            return _AssetBundleRequest != null && _AssetBundleRequest.isDone;
        }
    }
}
