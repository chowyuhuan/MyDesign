using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public abstract class AU_WWWFileFetcher : AU_WorkFlow
    {
        protected WWW _WWWFileLoader = null;
        protected abstract string GetFilePath();

        protected override void StartWorkFlow()
        {
            string versionFilePath = GetFilePath();
            _WWWFileLoader = new WWW(versionFilePath);
        }

        protected override bool UpdateWorkFlow()
        {
            return null != _WWWFileLoader && !_WWWFileLoader.isDone;
        }
    }
}
