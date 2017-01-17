using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public abstract class AM_LoadAssetOperation : AM_LoadOperation
    {
        public string _AssetName { get; protected set; }
        public System.Type _AssetType { get; protected set; }
        public AM_IAssetPostProcessor _PostProcessor { get; protected set; }

        public abstract T GetAsset<T>() where T : UnityEngine.Object;

        public AM_LoadAssetOperation(string assetName, System.Type type, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack loadCallBack)
            : base(loadCallBack)
        {
            _AssetName = assetName;
            _AssetType = type;
            _PostProcessor = postProcessor;
        }
    }
}
