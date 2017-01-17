using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_ServerVersionFetcher : AU_VersionFetcher
    {
        public AU_ServerVersionFetcher()
            :base()
        {
            _WorkName = "AU_ServerVersionFetcher";
            _VersionType = "服务器";
        }

        protected override string GetFilePath()
        {
            string filepath = AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.Remote) + "/" + AU_Config.Version_File;
            return filepath;
        }

        protected override void OnGetVersion()
        {
            AU_VersionControl._ServerVersion = _VersionInfo;
        }
    }
}
