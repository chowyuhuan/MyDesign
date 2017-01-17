using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssetManage
{
    public abstract class AM_IAssetLoader
    {
        #region async interface
        public abstract AM_LoadAssetOperation LoadAssetAsync<T>(string assetPath, bool autoUnloadAB, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack lcb, E_AssetType assetType) where T : UnityEngine.Object;
        public abstract AM_LoadLevelOperation LoadSceneAsync(string sceneName, LoadSceneMode lsm, AM_LoadCallBack lcb);
        #endregion

        #region sync interface
        public abstract T LoadAssetSync<T>(string assetPath, bool autoUnloadAB, AM_IAssetPostProcessor postProcessor, E_AssetType assetType) where T : UnityEngine.Object;
        public abstract AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB);
        public abstract void LoadSceneSync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB);
        #endregion

        public virtual void UnloadAsset(string assetPath)
        {
            AM_AssetRepository.UnloadAsset(assetPath);
        }
    }
}
