using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssetUpdate
{
    public class AU_ExcuteVersionFetcher : AU_VersionFetcher
    {
        public AU_ExcuteVersionFetcher()
            : base()
        {
            _WorkName = "AU_AppVersionFetcher";
            _VersionType = "安装包";
        }

        protected override string GetFilePath()
        {
            return AU_PathHelper.GetStreamingAssetPath() + "/" +  AU_Config.Version_File;
        }


        protected override void OnGetVersion()
        {
            AU_VersionControl._ExcuteVersion = _VersionInfo;
            AU_VersionControl.TryFinishLocalVersionCheck();
        }
    }
}
