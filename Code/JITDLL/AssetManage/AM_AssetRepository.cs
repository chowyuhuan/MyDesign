using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetManage
{
    public class AM_AssetRepository
    {
        static Dictionary<string, AM_LoadedAsset> _LoadedAssets = new Dictionary<string, AM_LoadedAsset>();
        static Dictionary<string, AM_LoadedAB> _LoadedAB = new Dictionary<string, AM_LoadedAB>();
        static Dictionary<string, string> _LoadABError = new Dictionary<string, string>();
        static Dictionary<string, string[]> _Dependencies = new Dictionary<string, string[]>();

        public static bool LoadAssetFromCache(string assetPath, out AM_LoadedAsset loadedasset)
        {
            if (_LoadedAssets.TryGetValue(assetPath, out loadedasset))
            {
                if (null != loadedasset)
                {
                    loadedasset.IncreaseRef();
                    return true;
                }
            }
            return false;
        }

        public static void AddLoadedAsset(string assetPath, AM_LoadedAsset loadedasset)
        {
            if (null != loadedasset)
            {
                _LoadedAssets.Add(assetPath, loadedasset);
            }
        }

        public static void AddDependence(string abName, string[] dependence)
        {
            if(!string.IsNullOrEmpty(abName) && !_Dependencies.ContainsKey(abName))
            {
                _Dependencies.Add(abName, dependence);
            }
        }

        public static void UnloadAsset(string assetPath)
        {
            AM_LoadedAsset loadedasset;
            if (_LoadedAssets.TryGetValue(assetPath, out loadedasset))
            {
                if (null != loadedasset)
                {
                    loadedasset.DecreaseRef();
                    if (loadedasset.Useless())
                    {
                        loadedasset.Unload();
                        _LoadedAssets.Remove(assetPath);
                    }
                }
                else
                {
                    _LoadedAssets.Remove(assetPath);
#if UNITY_EDITOR
                    Debug.LogWarning("Null asset, should not happen !");
#endif
                }
            }
        }

        public static AM_LoadedAB GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (_LoadABError.TryGetValue(assetBundleName, out error))
            {
                return null;
            }

            AM_LoadedAB loadedAssetBundle = null;
            _LoadedAB.TryGetValue(assetBundleName, out loadedAssetBundle);
            if (null == loadedAssetBundle)
            {
                return null;
            }

            string[] dependencies = null;
            if (!_Dependencies.TryGetValue(assetBundleName, out dependencies))
            {
                return loadedAssetBundle;
            }

            for (int index = 0; index < dependencies.Length; ++index)
            {
                if (_LoadABError.TryGetValue(dependencies[index], out error))
                {
                    return null;
                }
                AM_LoadedAB depen;
                _LoadedAB.TryGetValue(dependencies[index], out depen);
                if (null == depen)
                {
                    return null;
                }
            }

            return loadedAssetBundle;
        }

        static void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            AM_LoadedAB loadedAssetBunlde = GetLoadedAssetBundle(assetBundleName, out error);
            if (null == loadedAssetBunlde)
            {
                return;
            }

            if (loadedAssetBunlde.DecreaseRef() == 0)
            {
                loadedAssetBunlde.Unload();
                _LoadedAB.Remove(assetBundleName);
            }
        }

        static void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!_Dependencies.TryGetValue(assetBundleName, out dependencies))
            {
                return;
            }

            // Loop dependencies.
            for (int index = 0; index < dependencies.Length; ++index)
            {
                UnloadAssetBundleInternal(dependencies[index]);
            }

            _Dependencies.Remove(assetBundleName);
        }

        public static void UnLoadAssetBundle(string assetBundleName)
        {
            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);
        }

        public static void AddLoadedAB(string abName, AM_LoadedAB loadedAB)
        {
            if(!string.IsNullOrEmpty(abName)
                && !_LoadedAB.ContainsKey(abName))
            {
                _LoadedAB.Add(abName, loadedAB);
            }
        }

        public static void AddLoadABError(string abName, string error)
        {
            if(!string.IsNullOrEmpty(abName)
                && !_LoadABError.ContainsKey(abName))
            {
                _LoadABError.Add(abName, error);
            }
        }
    }
}
