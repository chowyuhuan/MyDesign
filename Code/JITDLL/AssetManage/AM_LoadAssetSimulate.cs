using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadAssetSimulate : AM_LoadAssetOperation
    {
        public Object _Asset { get; protected set; }

        public AM_LoadAssetSimulate(string assetPath, Object asset, System.Type type, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack lcb)
            :base(assetPath, type, postProcessor, lcb)
        {
            _Asset = asset;
        }

        public override T GetAsset<T>()
        {
            return _Asset as T;
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return true;
        }
    }
}
