using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadedAB : AM_AssetCounter<AssetBundle>
    {
        public AM_LoadedAB(string assetBundleName, AssetBundle assetBundle)
            :base(assetBundleName, assetBundle)
        {
        }

        public override void Unload()
        {
            _Asset.Unload(false);
            _Asset = null;
        }
    }

}