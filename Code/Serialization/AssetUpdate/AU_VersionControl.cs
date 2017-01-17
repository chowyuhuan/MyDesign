using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace AssetUpdate
{
    public class AU_VersionControl
    {
        public delegate void StringLongException(string arg1, string arg2, string arg3, long arg4, Exception arg5);

        public static AU_VersionInfo _ExcuteVersion = null;
        public static AU_VersionInfo _CacheVersion = null;
        public static AU_VersionInfo _ServerVersion = null;
        public static AU_VersionInfo _ClientVersion = null;

        protected static AU_BranchesVer _LocalFileVer = null;
        protected static AU_BranchesVer _ServerFileVer = null;

        static int _LocalVersionCount = 2;
        static int _VersionCheckCount = 2;

        static AU_VersionUpgrade _VersionUpgrade;

        static bool _InitFromRemote = false;
        public static bool InitFromCacheConfig
        {
            get { return _InitFromRemote; }
        }

        public static void StartLocalVersionCheck()
        {
            AU_Manager.Instance.SetUpdateState(EUpdateState.LoadLocalVersion);
            StartWorkFlow<AU_ExcuteVersionFetcher>();
            StartWorkFlow<AU_CacheVersionFetcher>();
        }

        static void ClearCache()
        {
            //Todo：可以考虑在app包较新时删除缓存的更新文件
        }

        public static void TryFinishLocalVersionCheck()
        {
            --_LocalVersionCount;
            if(_LocalVersionCount == 0)
            {
                AU_Manager.Instance.SetUpdateState(EUpdateState.CheckLocalVersion);
                if (_ExcuteVersion >= _CacheVersion)
                {
                    _ClientVersion = _ExcuteVersion;
                    ClearCache();
                    StartWorkFlow<AU_ExcuteConfigFetcher>();
                    StartWorkFlow<AU_ExcuteFileVerFetcher>();
                }
                else
                {
                    _ClientVersion = _CacheVersion;
                    _InitFromRemote = true;
                    StartWorkFlow<AU_CacheConfigFetcher>();
                    StartWorkFlow<AU_CacheFileVersionFetcher>();
                }
            }
        }

        public static void OnGetLocalConfig(bool success, AU_AppConfig.EFilePos configPos)
        {
            if (success)
            {
                AU_Manager.Instance.SetUpdateState(EUpdateState.LoadServerConfig);
                StartWorkFlow<AU_ServerVersionFetcher>();
                StartWorkFlow<AU_ServerConfigFetcher>();
            }
            else
            {
                if(configPos == AU_AppConfig.EFilePos.LocalExecute)
                {
                    AU_Manager.Instance.UpdateFail(EUpdateState.LoadExcuteConfig);
                }
                else
                {
                    AU_Manager.Instance.UpdateFail(EUpdateState.LoadCacheConfig);
                }
            }
        }

        public static void OnGetServerConfig(bool success)
        {
            if(success)
            {
                AU_Manager.Instance.SetUpdateState(EUpdateState.LoadServerVersionInfo);
                StartWorkFlow<AU_ServerFileVersionFetcher>();
            }
            else
            {
                AU_Manager.Instance.UpdateFail(EUpdateState.LoadServerConfig);
            }
        }

        public static void OnGetAllLocalFileVer(AU_BranchesVer branchesVer)
        {
            _LocalFileVer = branchesVer;
            if(null == _LocalFileVer)
            {
                AU_Manager.Instance.UpdateFail(EUpdateState.LoadLocalFileVer);
            }
            else
            {
                TryStartFileVerCheck();
            }
        }

        public static void OnGetServerFileVer(AU_BranchesVer branchesVer)
        {
            _ServerFileVer = branchesVer;
            if(null == _ServerFileVer)
            {
                AU_Manager.Instance.UpdateFail(EUpdateState.LoadServerFileVer);
            }
            else
            {
                TryStartFileVerCheck();
            }
        }

        public static void SaveToCacheFiles(string _filename)
        {
            string path = AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalCache);
            string outtxt = "Ver:" + _LocalFileVer.Ver.ToString() + "\n";
            //just Write,分支内文件版本信息
            foreach (var g in _LocalFileVer.Branches)
            {
                g.Value.SaveToFile(System.IO.Path.Combine(path, g.Key + AU_Config.Config_Suffix));
                outtxt += g.Value.Branch + "|" + g.Value.HashValue + "\n";
            }
            // 分支版本信息，直接创建/覆盖
            AU_FileHelper.WriteFile(path, _filename, outtxt);
        }

        public static void SaveBranchVerToFile(string _branch)
        {
            _LocalFileVer.Branches[_branch].SaveToFile(System.IO.Path.Combine(AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.LocalCache), _branch + AU_Config.Config_Suffix));
        }

        static void TryStartFileVerCheck()
        {
            --_VersionCheckCount;
            if(_VersionCheckCount == 0)
            {
                AU_Manager.Instance.SetUpdateState(EUpdateState.CheckingFiles);
                if(null != _LocalFileVer && null != _ServerFileVer)
                {
                    if(_ServerVersion <= _ClientVersion)
                    {
                        AU_Manager.Instance.SetUpdateState(EUpdateState.NoFileUpDate);
#if UNITY_EDITOR
                        Debug.Log("[更新]不需要更新（本地版本：" + _ClientVersion.ToString() + ",服务器版本：" + _ServerVersion.ToString() + "）");
#endif
                        return;
                    }
                    else if(_ServerVersion.DiffAPK(_ClientVersion))
                    {
                        AU_Manager.Instance.SetUpdateState(EUpdateState.DiffAPK);
#if UNITY_EDITOR
                        Debug.Log("[更新]重大版本更新，需要重新下载APK并安装（本地版本：" + _ClientVersion.ToString() + ",服务器版本：" + _ServerVersion.ToString() + "）");
#endif
                        return;
                    }

                    int branchcount = 0;
                    /* 当分支内文件版本信息文件下载完毕后，计算并比对之前从版本信息总表中取出的分支hash。
                     * 一致后，读取分支内文件版本信息至内存 */
                    Action<WWW, string> onBranchVerFileDownloaded = (www, branch) =>
                    {
                        if (string.IsNullOrEmpty(www.error) == false)
                        {
                            Debug.LogWarning("[更新]下载" + www.url + "错误");
                        }
                        else
                        {
                            string t = www.text;
                            if (t[0] == 0xFEFF)
                            {
                                t = t.Substring(1);
                            }
                            var rhash = AU_FileHelper.sha1.ComputeHash(www.bytes);
                            var shash = Convert.ToBase64String(rhash);
                            if (shash != _ServerFileVer.Branches[branch].HashValue)
                            {
#if UNITY_EDITOR
                                Debug.Log("[更新]网络上的分支" + branch + "的版本信息文件的计算hash与配置文件中的hash不匹配！");
                                Debug.Log("[更新]网络上的分支 :" + shash);
                                Debug.Log("[更新]配置文件中的hash :" + _ServerFileVer.Branches[branch].HashValue);
#endif
                            }
                            else
                            {
                                _ServerFileVer.Branches[branch].LoadFilesVerFromString(t);
                            }
                        }
                        branchcount--;
                        if (branchcount == 0)
                        {
                            ExcuteFileUpdate();
                        }
                    };

                    foreach (var g in _ServerFileVer.Branches)
                    {
                        if (_LocalFileVer.Branches.ContainsKey(g.Key) == false || g.Value.HashValue != _LocalFileVer.Branches[g.Key].HashValue)
                        {
#if UNITY_EDITOR
                            Debug.Log("[更新]分支： " + g.Key + " 有更新，下载对应版本信息文件");
#endif
                            branchcount++;
                            string filepath = AU_AppConfig.GetPathOrUrl(AU_AppConfig.EFilePos.Remote) + "/" + g.Key + AU_Config.Config_Suffix;
#if UNITY_EDITOR
                            //filepath = AU_Config.Editor_WWW_File_Access_Prefix + filepath;
#endif
                            Debug.Log("[更新]分支： " + g.Key + " 有更新，下载对应版本信息文件" + filepath);
                            AU_FileLoader.LoadFromWWW(filepath, g.Key, onBranchVerFileDownloaded);
                        }
                    }
                    if (branchcount == 0)
                    {
                        AU_Manager.Instance.SetUpdateState(EUpdateState.NoFileUpDate);
                    }
                }
            }
        }

        static void ExcuteFileUpdate()
        {
            AU_Manager.Instance.SetUpdateState(EUpdateState.UpdatingFiles);
            _InitFromRemote = true;
            _VersionUpgrade = new AU_VersionUpgrade(_LocalFileVer, _ServerFileVer);
            _VersionUpgrade.BeginLoadRemoteAndUpgradeLocal();
            IEnumerable<AU_FileVer> downlist = _VersionUpgrade.GetNeedDownloadRes();
            bool upJIT = false;
            foreach (var d in downlist)
            {
                if (d.Name == AU_AppConfig._JITUPFile)
                {
                    upJIT = true;
                    d.Download(OnOneFileDownloadFinish);
                }
            }
            if (!upJIT)
            {
                foreach (var d in downlist)
                {
                    d.Download(OnOneFileDownloadFinish);
                }
            }
            AU_FileLoader.SetDownloadFinishCallBack(AllDownloadFinish);
        }

        static bool needRestart = false;

        static void OnOneFileDownloadFinish(string branch, string name, string hash, long length, Exception err)
        {
            if (err == null)
            {
                if (AU_AppConfig._JITUPFile == name)
                {
                    needRestart = true;
                }
                _VersionUpgrade.UpdateVerInfo(branch, name, hash, length, needRestart);
            }
            AU_Manager.Instance.SetUpdateState(EUpdateState.UpdatingFiles);
        }

        static void AllDownloadFinish(bool error_happens)
        {
            if (!error_happens)
            {
                Debug.Log("[更新]所有下载任务完成!");
                if (needRestart)
                {
                    _VersionUpgrade.SaveAllVerInfo(false);
                    Debug.Log("[更新]热更新更新了自己，需要重启！");
                    // 重启游戏
                    // 目前来看，弹出一个窗口，告知用户要需要重启游戏，由玩家自己手动点击打开游戏
                    //Application.Quit();
                    AU_Manager.Instance.SetUpdateState(EUpdateState.RestartApp);
                }
                else
                {
                    _ClientVersion = _ServerVersion;
                    _VersionUpgrade.SaveAllVerInfo(true);
                    AU_Manager.Instance.SetUpdateState(EUpdateState.Finished);
                    //StartGameLogic();
                }
            }
            else
            {
                AU_Manager.Instance.UpdateFail(EUpdateState.DownLoadFileError);
            }
        }

        public static AU_VersionInfo GetLocalVersion()
        {
            return _ExcuteVersion >= _CacheVersion ? _ExcuteVersion : _CacheVersion;
        }

        public static void StartWorkFlow<T>() where T : AU_WorkFlow, new()
        {
            T work = new T();
            AU_WorkPipeLine.AddWorkFlow(work);
        }
    }
}
