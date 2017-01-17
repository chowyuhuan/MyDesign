using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_ServerConfigFetcher : AU_ConfigFetcher
    {
        public AU_ServerConfigFetcher()
            : base(false)
        {
            _ConfigType = "服务器";
        }

        protected override string GetFilePath()
        {
            string path = AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.Remote) + "/" + AU_Config.Config_File;
            return path;
        }

        protected override void OnGetConfig(bool success)
        {
            if(success)
            {
                AU_FileHelper.WriteFile(AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalCache), "Config.cfg", _WWWFileLoader.bytes);
            }
            AU_VersionControl.OnGetServerConfig(success);
        }
    }
}
