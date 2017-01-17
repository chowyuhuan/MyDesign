using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssetManage
{
    public class AM_ResourceModeLoader : AM_IAssetLoader
    {
        public override AM_LoadAssetOperation LoadAssetAsync<T>(string assetPath, bool autoUnloadAB, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack lcb, E_AssetType assetType)
        {
            ResourceRequest resrequest = Resources.LoadAsync<T>(FixAssetPath(assetPath, assetType));
            AM_LoadAssetFromAPP las = new AM_LoadAssetFromAPP(assetPath, typeof(T), resrequest, postProcessor, lcb);
            AM_LoadOperationManager.AddLoadAssetOperation(assetPath, las, false);
            return las;
        }

        public override AM_LoadLevelOperation LoadSceneAsync(string sceneName, UnityEngine.SceneManagement.LoadSceneMode lsm, AM_LoadCallBack lcb)
        {
            AsyncOperation asyncop = SceneManager.LoadSceneAsync(sceneName, lsm);
            AM_LoadLevelFromAPP llfa = new AM_LoadLevelFromAPP(sceneName, asyncop, lsm, lcb);
            AM_LoadOperationManager.AddLoadAssetOperation(sceneName, llfa, false);
            return llfa;
        }

        public override AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB)
        {
            return SceneManager.LoadSceneAsync(sceneName, lsm);
        }

        public override T LoadAssetSync<T>(string assetPath, bool autoUnloadAB, AM_IAssetPostProcessor postProcessor, E_AssetType assetType)
        {
            Object asset = Resources.Load<Object>(FixAssetPath(assetPath, assetType));
            if(null != postProcessor)
            {
                asset = postProcessor.PostProcessAsset(asset) as T;
            }
            return asset as T;
        }

        public override void LoadSceneSync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB)
        {
            SceneManager.LoadScene(sceneName, lsm);
        }

        /// <summary>
        /// 先用接口，后期没有特殊情况就优化去掉
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        string FixAssetPath(string assetPath, E_AssetType assetType)
        {
            return assetPath;
        }
    }
}
