using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_CacheConfigFetcher : AU_ConfigFetcher
    {
        public AU_CacheConfigFetcher()
            : base(true)
        {
            _ConfigType = "缓存";
        }

        protected override string GetFilePath()
        {
            string path = AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalCache) + "/" + AU_Config.Config_File;
#if UNITY_EDITOR
            path = AU_Config.Editor_WWW_File_Access_Prefix + path;
#endif
            return path;
        }

        protected override void OnGetConfig(bool success)
        {
            AU_VersionControl.OnGetLocalConfig(success, AU_AppConfig.EFilePos.LocalCache);
        }
    }
}
