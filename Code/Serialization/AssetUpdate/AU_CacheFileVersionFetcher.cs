using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_CacheFileVersionFetcher : AU_LocalAssetFileVerFetcher
    {

        public AU_CacheFileVersionFetcher()
            :base()
        {
            _VersionType = "缓存";
        }

        protected override string GetFilePath()
        {
            string filepath = GetBasePath() + "/" + AU_Config.Version_File;
            return filepath;
        }

        protected override string GetBasePath()
        {
#if !UNITY_EDITOR
            return AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalCache);
#else
            return AU_Config.Editor_WWW_File_Access_Prefix + AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalCache);
#endif

        }
    }
}
