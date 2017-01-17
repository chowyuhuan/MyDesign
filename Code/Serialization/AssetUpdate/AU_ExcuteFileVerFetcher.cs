using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_ExcuteFileVerFetcher : AU_LocalAssetFileVerFetcher
    {
        public AU_ExcuteFileVerFetcher()
            :base()
        {
            _VersionType = "安装包";
        }

        protected override string GetFilePath()
        {
            return GetBasePath() + "/" + AU_Config.Version_File;
        }

        protected override string GetBasePath()
        {
            return AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalExecute);
        }
    }
}
