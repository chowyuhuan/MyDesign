using UnityEngine;
using System.Collections;
using System;

namespace AssetUpdate
{
    public class AU_CacheVersionFetcher : AU_VersionFetcher
    {
        public AU_CacheVersionFetcher()
            : base()
        {
            _WorkName = "AU_CacheVersionFetcher";
            _VersionType = "缓存";
        }


        protected override string GetFilePath()
        {
            string filePath = AU_PathHelper.GetPersistentDataPath() + "/" + AU_Config.Version_File;
#if UNITY_EDITOR
            filePath = AU_Config.Editor_WWW_File_Access_Prefix + filePath;
#endif
            return filePath;
        }


        protected override void OnGetVersion()
        {
            AU_VersionControl._CacheVersion = _VersionInfo;
            AU_VersionControl.TryFinishLocalVersionCheck();
        }
    }
}
