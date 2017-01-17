using UnityEngine;
using System.Collections;

namespace AssetManage
{
    public class AM_UIPrefabPostProcessor : AM_IAssetPostProcessor
    {
        protected override Object ProcessAsset(Object asset)
        {
            if (!Application.isEditor && null != asset)
            {
                GameObject ui = asset as GameObject;
                GUI_Sprite[] sps = ui.GetComponentsInChildren<GUI_Sprite>(true);
                for(int index = 0; index < sps.Length; ++index)
                {
                    GUI_Atlas uiatlas = AM_Manager.LoadAssetSync<GUI_Atlas>(sps[index]._AtlasName, true, E_AssetType.GUIAtlas);
                    if(null != uiatlas)
                    {
                        sps[index]._Image.sprite = uiatlas.GetSprite(sps[index]._Name);
                    }
#if UNITY_EDITOR
                    else
                    {
                        Debug.LogError("Atlas :" + sps[index]._AtlasName + " not found !");
                    }
#endif
                }
            }
            return asset;
        }
    }
}
