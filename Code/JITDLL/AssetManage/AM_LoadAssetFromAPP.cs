using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadAssetFromAPP : AM_LoadAssetOperation
    {
        Object _Asset;
        ResourceRequest _ResRequest;
        public AM_LoadAssetFromAPP(string assetPath, System.Type type, ResourceRequest resrequest, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack lcb)
            :base(assetPath, type, postProcessor, lcb)
        {
            _ResRequest = resrequest;
        }

        public override T GetAsset<T>()
        {
            return _Asset as T;
        }

        public override void PostProcess()
        {
            if(null != _PostProcessor)
            {
                _Asset = _PostProcessor.PostProcessAsset(_Asset);
            }
        }

        public override bool Update()
        {
            if(null == _ResRequest)
            {
                _LoadError = "Asset : " + _AssetName + " not found!";
#if UNITY_EDITOR
                Debug.LogError(_LoadError);
#endif
            }
            return false;
        }

        public override bool IsDone()
        {
            return _ResRequest != null && _ResRequest.isDone;
        }
    }
}
