using UnityEngine;
using System.Collections;
using System;

namespace AssetUpdate
{
    public abstract class AU_LocalAssetFileVerFetcher : AU_FileVersionInfoFetcher
    {
        public AU_LocalAssetFileVerFetcher()
            :base(true)
        {
        }

        protected abstract string GetBasePath();

        protected override void OnGetBranches(bool success)
        {
            int branchcount = 0;
            Action<WWW, string> onLoadBranchVerFile = (www, tag) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    string t = www.text;
                    if (t.Length > 0 && t[0] == 0xFEFF)
                    {
                        t = t.Substring(1);
                    }
                    _Branches.Branches[tag].LoadFilesVerFromString(t);
                    branchcount--;
                    if (branchcount == 0)
                    {
#if UNITY_EDITOR
                        Debug.Log("[更新]从" + _VersionType + "加载版本信息文件完成");
#endif
                        OnGetAllFileVer();
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.Log("[更新]从" + _VersionType + "路径加载文件版本信息错误：" + _WWWFileLoader.error);
                    Debug.Log("[更新]从" + _VersionType + "路径加载文件版本信息错误：" + GetFilePath());
                }
#endif
            };
            foreach (var g in _Branches.Branches)
            {
                branchcount++;
                AU_FileLoader.LoadFromWWW(GetBasePath() + "/" + g.Key + AU_Config.Config_Suffix, g.Key, onLoadBranchVerFile);
            }
        }
    }
}
