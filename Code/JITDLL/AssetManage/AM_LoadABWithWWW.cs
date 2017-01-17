using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadABWithWWW : AM_LoadABOperation
    {
        public WWW _WWW { get; protected set; }

        public override bool IsDone()
        {
            return null != _WWW ? _WWW.isDone : true;
        }

        public override string GetSourceURL()
        {
            return null != _WWW ? _WWW.url : null;
        }

        public override bool Update()
        {
            if (IsDone())
            {
                FinishLoadOperation();
                return false;
            }
            return true;
        }

        protected override void FinishLoadOperation()
        {
            _LoadError = _WWW.error;
            if (!string.IsNullOrEmpty(_LoadError))
            {
                return;
            }

            AssetBundle bundle = _WWW.assetBundle;
            if (bundle == null)
            {
                _LoadError = string.Format("{0} is not a valid asset bundle.", _AssetBundleName);
            }
            else
            {
                _LoadedAssetBundle = new AM_LoadedAB(_AssetBundleName, _WWW.assetBundle);
                AM_AssetRepository.AddLoadedAB(_AssetBundleName, _LoadedAssetBundle);
            }

            _WWW.Dispose();
            _WWW = null;
        }

        public AM_LoadABWithWWW(WWW loader, string assetBundleName)
            : base(assetBundleName, null)
        {
            _WWW = loader;
        }
    }
}
