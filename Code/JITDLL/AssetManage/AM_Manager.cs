using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AssetManage
{
    public enum E_AssetType
    {
        Normal,//普通资源，加载完成后直接返回，不做任何处理
        GUIAtlas,//加载后获取图集脚本对象
        UIPrefab,//加载后还原sprite的依赖
        ActorPrefab,//模型prefab
        AnimatorOverrideController,//模型动画
    }


    public class AM_Manager
    {
        #region asset loader
        static bool _InitDone = false;
        static AM_IAssetLoader _ABModeLoader;
        static AM_IAssetLoader _ResourceModeLoader;
#if UNITY_EDITOR
        static AM_IAssetLoader _EditorModeLoader;
#endif
        #endregion
        
        #region asset post processor
        static Dictionary<E_AssetType, AM_IAssetPostProcessor> _AssetProcessorDic = new Dictionary<E_AssetType, AM_IAssetPostProcessor>(); 
        #endregion



        public static void Init(bool remote)
        {
#if UNITY_EDITOR
            Debug.Log("[资源]init AM_Manager from remote:" + remote);
#endif
            //if(!_InitDone)
            {
                AM_AssetLoadController.Init(remote);
                InitLoader();
                InitPostProcessor();
                _InitDone = true;
            }
        }

        static void InitLoader()
        {
            AM_ABLoader abloader = new AM_ABLoader();
#if USE_ABAR
            _ABModeLoader = new AM_ABModeLoader(abloader);
#endif
            _ResourceModeLoader = new AM_ResourceModeLoader();
#if UNITY_EDITOR
            _EditorModeLoader = new AM_EditorModeLoader();
#endif
        }

        static void InitPostProcessor()
        {
            AddProcessor(E_AssetType.Normal, null);

            AM_UIPrefabPostProcessor uiprefabPostProcessor = new AM_UIPrefabPostProcessor();
            AddProcessor(E_AssetType.UIPrefab, uiprefabPostProcessor);

            AM_GUIAtlasPostProcessor altasPostProcessor = new AM_GUIAtlasPostProcessor();
            AddProcessor(E_AssetType.GUIAtlas, altasPostProcessor);

            AddProcessor(E_AssetType.ActorPrefab, null);
            AddProcessor(E_AssetType.AnimatorOverrideController, null);
        }

        static void AddProcessor(E_AssetType type , AM_IAssetPostProcessor postProcessor)
        {
            if(!_AssetProcessorDic.ContainsKey(type))
            {
                _AssetProcessorDic.Add(type, postProcessor);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("Repeat add processor:" + type.ToString());
            }
#endif
        }

        static AM_IAssetPostProcessor GetProcessor(E_AssetType type)
        {
            return _AssetProcessorDic[type];
        }


        static AM_IAssetLoader GetAssetLoader(string assetPath)
        {
#if USE_ABAR
            if (AM_AssetLoadController.LoadAssetFromAB(assetPath))
            {
                return _ABModeLoader;
            }
            else
            {
                return _ResourceModeLoader;
            }
#elif UNITY_EDITOR
            return _EditorModeLoader;
#else
            return _ResourceModeLoader;
#endif
        }

        #region async interface
        public static AM_LoadAssetOperation LoadAssetAsync<T>(string assetPath, bool autoUnloadAB, E_AssetType assetType, AM_LoadCallBack lcb) where T : UnityEngine.Object
        {
            AM_LoadedAsset loadedasset;
            if(AM_AssetRepository.LoadAssetFromCache(assetPath, out loadedasset))
            {
                AM_LoadAssetSimulate las = new AM_LoadAssetSimulate(assetPath, loadedasset._Asset, typeof(T), GetProcessor(assetType), lcb);
                AM_LoadOperationManager.AddLoadAssetOperation(assetPath, las, true);
                return las;
            }
            return GetAssetLoader(assetPath).LoadAssetAsync<T>(assetPath, autoUnloadAB, GetProcessor(assetType), lcb, assetType);
        }
        public static AM_LoadLevelOperation LoadSceneAsync(string sceneName, LoadSceneMode lsm, AM_LoadCallBack lcb)
        {
            return GetAssetLoader(sceneName).LoadSceneAsync(sceneName, lsm, lcb);
        }
        #endregion

        #region sync interface
        public static T LoadAssetSync<T>(string assetPath, bool autoUnloadAB, E_AssetType assetType) where T : UnityEngine.Object
        {
            AM_LoadedAsset loadedasset;
            if (AM_AssetRepository.LoadAssetFromCache(assetPath, out loadedasset))
            {
                return loadedasset._Asset as T;
            }
            return GetAssetLoader(assetPath).LoadAssetSync<T>(assetPath, autoUnloadAB, GetProcessor(assetType), assetType);
        }
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB)
        {
            return GetAssetLoader(sceneName).LoadSceneAsync(sceneName, lsm, autoUnloadAB);
        }
        public static void LoadSceneSync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB)
        {
            GetAssetLoader(sceneName).LoadSceneSync(sceneName, lsm, autoUnloadAB);
        }
        #endregion

        public static void UnloadAsset(string assetPath)
        {
            AM_AssetRepository.UnloadAsset(assetPath);
        }
    }
}
