using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_GUIAtlasPostProcessor : AM_IAssetPostProcessor
    {
        protected override Object ProcessAsset(Object asset)
        {
            GameObject go = asset as GameObject;
            if(null != go)
            {
                GUI_Atlas uiatlas = go.GetComponent<GUI_Atlas>() as GUI_Atlas;
                if(null != uiatlas)
                {
                    uiatlas.Init();
                }
                return uiatlas;
            }
            return asset;
        }
    }
}
