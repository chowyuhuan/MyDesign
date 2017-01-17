using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_ServerFileVersionFetcher : AU_FileVersionInfoFetcher
    {
        public AU_ServerFileVersionFetcher()
            :base(false)
        {
            _VersionType = "服务器";
        }

        protected override void OnGetBranches(bool success)
        {
            AU_VersionControl.OnGetServerFileVer(_Branches);
        }

        protected override string GetFilePath()
        {
            string filepath = AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.Remote) + "/" + AU_Config.Version_File;
            return filepath;
        }
    }
}
