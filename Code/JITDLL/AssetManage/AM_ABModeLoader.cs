using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssetManage
{
    public class AM_ABModeLoader : AM_IAssetLoader
    {
        AssetBundleManifest _ABManifest = null;
        AM_IABLoader _ABLoader = null;
        public AM_ABModeLoader(AM_IABLoader abLoader)
        {
            _ABLoader = abLoader;
            AssetBundle ab = AM_FileReader.ReadABFromFile(Application.persistentDataPath + "/JITData/res/Android/Android");
            if(null != ab)
            {
                _ABManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
        }

        public override AM_LoadAssetOperation LoadAssetAsync<T>(string assetPath, bool autoUnloadAB, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack lcb, E_AssetType assetType)
        {
            string abName;
            if (AM_AssetLoadController.MapABName(assetPath, out abName))
            {
                LoadAssetBundle(abName, true, false);
                AM_LoadAssetFromAB lafab = new AM_LoadAssetFromAB(assetPath, abName, typeof(T), autoUnloadAB, postProcessor, lcb);
                AM_LoadOperationManager.AddLoadAssetOperation(assetPath, lafab, false);
                return lafab;
            }
            return null;
        }

        public override AM_LoadLevelOperation LoadSceneAsync(string sceneName, UnityEngine.SceneManagement.LoadSceneMode lsm, AM_LoadCallBack lcb)
        {
            string sceneABName;
            if (AM_AssetLoadController.MapABName(sceneName, out sceneABName))
            {
                LoadAssetBundle(sceneABName, true, false);
                AM_LoadLevelFromAB llo = new AM_LoadLevelFromAB(sceneName, sceneABName, lsm, lcb);
                AM_LoadOperationManager.AddLoadAssetOperation(sceneName, llo, false);
                return llo;
            }
            return null;
        }

        string GetAssetName(string assetPath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(assetPath);
        }

        public override T LoadAssetSync<T>(string assetPath, bool autoUnloadAB, AM_IAssetPostProcessor postPorcessor, E_AssetType assetType)
        {
            string abName;
            T asset = null;
            if (AM_AssetLoadController.MapABName(assetPath, out abName))
            {
                AssetBundle ab = LoadAssetBundle(abName, false, false);
                if(null != ab)
                {
                    Object ob = ab.LoadAsset<Object>(GetAssetName(assetPath));
                    if(null != postPorcessor)
                    {
                        asset = postPorcessor.PostProcessAsset(ob) as T;
                    }
                    AM_LoadedAsset loadedasset = new AM_LoadedAsset(assetPath, asset);
                    AM_AssetRepository.AddLoadedAsset(assetPath, loadedasset);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError("Asset :" + assetPath + " not found !");
                }
#endif
                if (autoUnloadAB)
                {
                    AM_AssetRepository.UnLoadAssetBundle(abName);
                }
            }
            return asset;
        }

        public override AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB)
        {
            string sceneABName;
            AsyncOperation asyncop = null;
            if (AM_AssetLoadController.MapABName(sceneName, out sceneABName))
            {
                AssetBundle ab = LoadAssetBundle(sceneABName, false, false);
                if (null != ab)
                {
                    asyncop = SceneManager.LoadSceneAsync(sceneName, lsm);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError("Scene :" + sceneName + " not found !");
                }
#endif
                if (autoUnloadAB)
                {
                    AM_AssetRepository.UnLoadAssetBundle(sceneABName);
                }
            }
            return asyncop;
        }

        public override void LoadSceneSync(string sceneName, UnityEngine.SceneManagement.LoadSceneMode lsm, bool autoUnloadAB)
        {
            string sceneABName;
            if (AM_AssetLoadController.MapABName(sceneName, out sceneABName))
            {
                AssetBundle ab = LoadAssetBundle(sceneABName, false, false);
                if(null != ab)
                {
                    SceneManager.LoadScene(sceneName, lsm);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError("Scene :" + sceneName + " not found !");
                }
#endif
                if(autoUnloadAB)
                {
                    AM_AssetRepository.UnLoadAssetBundle(sceneABName);
                }
            }
        }


        bool LoadABInternal(string abName, bool async, out AssetBundle ab)
        {
            ab = null;
            AM_LoadedAB loadedAssetBundle;
            string loaderror;
            loadedAssetBundle = AM_AssetRepository.GetLoadedAssetBundle(abName, out loaderror);
            if (loadedAssetBundle != null)
            {
                loadedAssetBundle.IncreaseRef();
                ab = loadedAssetBundle._Asset;
                return true;
            }

            if (AM_LoadOperationManager.LoadingAB(abName))
            {
                return true;
            }
            string abPath = GetABPath(abName);
            if(async)
            {
                _ABLoader.LoadABAsync(abName, abPath);
            }
            else
            {
                ab = _ABLoader.LoadABSync(abName, abPath);
                if (null != ab)
                {
                    AM_LoadedAB loadedAB = new AM_LoadedAB(abName, ab);
                    AM_AssetRepository.AddLoadedAB(abName, loadedAB);
                    return false;
                }
            }
            return false;
        }

        string GetABPath(string abName)
        {
            return abName;
        }

        AssetBundle LoadAssetBundleDependencies(string assetBundleName, bool async)
        {
            string error;
            string[] dependencies = _ABManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length > 0)
            {
                AssetBundle ab;
                for (int index = 0; index < dependencies.Length; ++index)
                {
                    LoadABInternal(dependencies[index], async, out ab);
                }
                AM_AssetRepository.AddDependence(assetBundleName, dependencies);
            }

            AM_LoadedAB loadedAB = AM_AssetRepository.GetLoadedAssetBundle(assetBundleName, out error);
            if(null != loadedAB)
            {
                return loadedAB._Asset;
            }
            return null;
        }

        AssetBundle LoadAssetBundle(string assetBundleName, bool async, bool manifestFile)
        {
            if (_ABManifest == null && !manifestFile)
            {
#if UNITY_EDITOR
                Debug.LogError("Empty manifest file !");
#endif
                return null;
            }
            AssetBundle ab = null;
            if (LoadABInternal(assetBundleName, async, out ab))
            {
                return ab;
            }
            else if (!manifestFile)
            {
                ab = LoadAssetBundleDependencies(assetBundleName, async);
            }
            return ab;
        }
    }
}
