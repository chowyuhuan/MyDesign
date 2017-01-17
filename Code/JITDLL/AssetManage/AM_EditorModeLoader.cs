#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;

namespace AssetManage
{
    public class AM_EditorModeLoader : AM_IAssetLoader
    {
        public override AM_LoadAssetOperation LoadAssetAsync<T>(string assetPath, bool autoUnloadAB, AM_IAssetPostProcessor postProcessor, AM_LoadCallBack lcb, E_AssetType assetType)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(FixAssetPath(assetPath, assetType));
            AM_LoadAssetSimulate las = new AM_LoadAssetSimulate(assetPath, asset, typeof(T), postProcessor, lcb);
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
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(FixAssetPath(assetPath, assetType));
            if(null != postProcessor)
            {
                asset = postProcessor.PostProcessAsset(asset);
            }
            return asset as T;
        }

        public override void LoadSceneSync(string sceneName, LoadSceneMode lsm, bool autoUnloadAB)
        {
            SceneManager.LoadScene(sceneName, lsm);
        }

        string FixAssetPath(string assetPath, E_AssetType assetType)
        {
            switch(assetType)
            {
                case E_AssetType.UIPrefab:
                    {
                        return "Assets/Resources_Editor/" + assetPath + ".prefab";
                    }
                case E_AssetType.GUIAtlas:
                case E_AssetType.ActorPrefab:
                    {
                        return "Assets/Resources/" + assetPath + ".prefab";
                    }
                case E_AssetType.AnimatorOverrideController:
                    {
                        return "Assets/Resources/" + assetPath + ".overrideController";
                    }
                default:
                    {
                        return "Assets/Resources/" + assetPath;
                    }
            }
        }
    }
}

#endif