using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadedAsset : AM_AssetCounter<Object>
    {
        public AM_LoadedAsset(string assetPath, Object asset)
            :base(assetPath, asset)
        {

        }

        public override void Unload()
        {
            if(_Asset != null)
            {
                Resources.UnloadAsset(_Asset);
                _Asset = null;
            }
        }
    }
}
