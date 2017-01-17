using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public abstract class AM_IAssetPostProcessor
    {
        AM_IAssetPostProcessor _Successor;
        public void SetSuccessor(AM_IAssetPostProcessor successor)
        {
            _Successor = successor;
        }
        public Object PostProcessAsset(Object asset)
        {
            asset = ProcessAsset(asset);
            if(null != _Successor)
            {
                return _Successor.PostProcessAsset(asset);
            }
            else
            {
                return asset;
            }
        }

        protected abstract Object ProcessAsset(Object asset);
    }
}
