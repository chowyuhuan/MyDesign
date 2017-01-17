using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssetManage
{
    public class AM_LoadLevelFromAPP : AM_LoadLevelOperation
    {
        public AM_LoadLevelFromAPP(string sceneName, AsyncOperation loadRequest, LoadSceneMode lsm, AM_LoadCallBack lcb)
            :base(sceneName, lsm , lcb)
        {
            _LoadLevelRequest = loadRequest;
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return null != _LoadLevelRequest && _LoadLevelRequest.isDone;
        }
    }
}
