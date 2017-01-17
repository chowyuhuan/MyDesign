using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public abstract class AM_LoadABOperation : AM_LoadOperation
    {
        public string _AssetBundleName { get; private set; }

        public AM_LoadedAB _LoadedAssetBundle { get; protected set; }

        protected abstract void FinishLoadOperation();
        public abstract string GetSourceURL();

        public AM_LoadABOperation(string abName, AM_LoadCallBack loadCallBack)
            : base(loadCallBack)
        {
            _AssetBundleName = abName;
        }
    }
}
