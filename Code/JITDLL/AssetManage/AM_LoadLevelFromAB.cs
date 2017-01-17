using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadLevelFromAB : AM_LoadLevelOperation
    {
        public string _SceneABName { get; protected set; }

        protected void LoadLevel()
        {
            _LoadLevelRequest = SceneManager.LoadSceneAsync(_LevelName, _LoadSceneMode);
        }

        public AM_LoadLevelFromAB(string levelName, string sceneBundleName, LoadSceneMode loadMode, AM_LoadCallBack loadCallBack)
            : base(levelName, loadMode, loadCallBack)
        {
            _SceneABName = sceneBundleName;
        }

        public void UnLoadSceneAB()
        {
            if (null != _SceneABName)
            {
                AM_AssetRepository.UnLoadAssetBundle(_SceneABName);
            }
        }

        public override bool Update()
        {
            if (_LoadLevelRequest != null)
            {
                return false;
            }

            string error;
            AM_LoadedAB loadedAssetBundle = AM_AssetRepository.GetLoadedAssetBundle(_SceneABName, out error);
            _LoadError = error;
            if (loadedAssetBundle != null)
            {
                LoadLevel();
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            if (null == _LoadLevelRequest && null != _LoadError)
            {
                return true;
            }
            else
            {
                return null != _LoadLevelRequest;
            }
        }
    }
}
