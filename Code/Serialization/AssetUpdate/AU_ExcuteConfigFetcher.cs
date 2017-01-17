using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_ExcuteConfigFetcher : AU_ConfigFetcher
    {
        public AU_ExcuteConfigFetcher()
            :base(true)
        {
            _ConfigType = "安装包";
        }

        protected override string GetFilePath()
        {
            return AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalExecute) + "/" + "Config.cfg";
        }

        protected override void OnGetConfig(bool success)
        {
            AU_VersionControl.OnGetLocalConfig(success, AU_AppConfig.EFilePos.LocalExecute);
        }
    }
}
