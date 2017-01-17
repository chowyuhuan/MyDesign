using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetUpdate
{
    public class AU_VersionUpgrade
    {
        public AU_BranchesVer verLocal;
        public AU_BranchesVer verRemote;
        public AU_BranchesVer verUpgrade;
        public int saveVerRate = 5; /**< 下载多个文件，写一次版本信息文件 */
        public bool needReinstallAPK = false;

        private int saveVerRateCur = 0;
        private HashSet<string> UpgradeBranches = new HashSet<string>();
        /**
         *	\brief 加载本地版本信息
         */
        /// <summary>
        /// 更新版本信息
        /// </summary>
        /// <param name="onLoadEnd">更新结束回调</param>
        public void BeginLoadRemoteAndUpgradeLocal()
        {
               //检查需更新的列表
                int addcount = 0;
                int updatecount = 0;
                verUpgrade = new AU_BranchesVer();
                foreach (var g in verRemote.Branches)
                {
                    foreach (var f in g.Value.filelist)
                    {
                        if (verLocal.Branches[g.Key].filelist.ContainsKey(f.Key))
                        {
                            var fl = verLocal.Branches[g.Key].filelist[f.Key];
                            if (fl.Hash != f.Value.Hash)
                            {
                                if (verUpgrade.Branches.ContainsKey(g.Key) == false)
                                {
                                    verUpgrade.Branches[g.Key] = new AU_BranchVer(g.Key, g.Value.HashValue);
                                }
                                
                                verUpgrade.Branches[g.Key].filelist[f.Key] = new AU_FileVer(g.Key, f.Key, f.Value.Hash, f.Value.Length, false);
                                updatecount++;
#if UNITY_EDITOR
                                Debug.Log("[更新]发现更新： " + f.Key);
#endif

                                if (fl.Name == AU_AppConfig._JITUPFile)
                                {
                                    Debug.LogError("[更新]发现更新代码更新");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("[更新]发现新的文件： " + f.Key);
                            if (verUpgrade.Branches.ContainsKey(g.Key) == false)
                            {
                                verUpgrade.Branches[g.Key] = new AU_BranchVer(g.Key, g.Value.HashValue);
                            }
                            verUpgrade.Branches[g.Key].filelist[f.Key] = new AU_FileVer(g.Key, f.Key, f.Value.Hash, f.Value.Length, false);
                            addcount++;
                        }
                    }
                }
#if UNITY_EDITOR
                Debug.Log("[更新]发现更新汇总： 新的文件:" + addcount + ", 更新:" + updatecount);
#endif
        }
        public void UpdateVerInfo(string _branch, string _name, string _hash, long _length, bool _foceSave)
        {
            if (verLocal.Branches.ContainsKey(_branch) == false)
            {
                verLocal.Branches[_branch] = new AU_BranchVer(_branch, "");
            }

            if (verLocal.Branches[_branch].filelist.ContainsKey(_name))
            {
                verLocal.Branches[_branch].filelist[_name].Hash = _hash;
                verLocal.Branches[_branch].filelist[_name].Length = _length;
            }
            else
            {
                verLocal.Branches[_branch].filelist[_name] = new AU_FileVer(_branch, _name, _hash, _length, false);
            }

            UpgradeBranches.Add(_branch);
            if (++saveVerRateCur >= saveVerRate || _foceSave)
            {
                foreach(var b in UpgradeBranches)
                {
                    AU_VersionControl.SaveBranchVerToFile(b);
                }
                saveVerRateCur = 0;
                UpgradeBranches.Clear();
            }

        }
        public void SaveAllVerInfo(bool overwrite)
        {
            if(overwrite)
            {
                verLocal.Ver.Set(verRemote.Ver);
                foreach (var l in verRemote.Branches)
                {
                    verLocal.Branches[l.Key].HashValue = verRemote.Branches[l.Key].HashValue;
                }
            }
            AU_VersionControl.SaveToCacheFiles(AU_Config.Version_File);
        }

        public IEnumerable<AU_FileVer> GetNeedDownloadRes()
        {
            if (verUpgrade == null)
            {
                return null;
            }
            List<AU_FileVer> infos = new List<AU_FileVer>();
            foreach (var g in verUpgrade.Branches)
            {
                foreach (var f in g.Value.filelist.Values)
                {
                    infos.Add(f);
                }
            }
            return infos;
        }

        public void SetSaveVerRate(int rate)
        {
            saveVerRate = rate;
        }
        public AU_VersionUpgrade(AU_BranchesVer localVer, AU_BranchesVer remoteVer)
        {
            verLocal = localVer;
            verRemote = remoteVer;
        }
    }
}
