using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssetManage
{
    public abstract class AM_LoadLevelOperation : AM_LoadOperation
    {
        public string _LevelName { get; protected set; }
        public LoadSceneMode _LoadSceneMode { get; protected set; }
        public AsyncOperation _LoadLevelRequest { get; protected set; }

        public AM_LoadLevelOperation(string scenenName, LoadSceneMode lsm, AM_LoadCallBack lcb)
            :base(lcb)
        {
            _LevelName = scenenName;
            _LoadSceneMode = lsm;
        }

        public override void PostProcess()
        {
            AM_LoadOperationManager.RemoveLoadingAsset(_LevelName);
        }
    }
}
