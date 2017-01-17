using UnityEngine;
using System.Collections;
using System;

namespace AssetUpdate
{
    public abstract class AU_VersionFetcher : AU_WWWFileFetcher
    {
        public string _VersionType { get; protected set; }
        protected AU_VersionInfo _VersionInfo = new AU_VersionInfo();

        public AU_VersionFetcher()
            :base()
        {
        }

        protected override void EndWorkFlow()
        {
            if(null == _WWWFileLoader.error)
            {
                try
                {
                    string t = _WWWFileLoader.text;
                    if (t[0] == 0xFEFF)
                    {
                        t = t.Substring(1);
                    }
                    string[] lines = t.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var l in lines)
                    {
                        if (l.IndexOf("Ver:") == 0)
                        {
                            string[] vs = l.Substring(4).Split('.');
                            _VersionInfo.Set(int.Parse(vs[0]), int.Parse(vs[1]), int.Parse(vs[2]));
#if UNITY_EDITOR
                            Debug.Log("[更新]" + _VersionType + "版本号：" + _VersionInfo.ToString());
#endif
                            break;
                        }
                    }
                }
                catch (Exception er)
                {
#if UNITY_EDITOR
                    Debug.Log("[更新]从" + _VersionType + "路径加载版本号异常：" + er.ToString());
#endif
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("[更新]从" + _VersionType + "路径加载版本号错误：" + _WWWFileLoader.error);
                Debug.Log("[更新]从" + _VersionType + "路径加载版本号错误：" + GetFilePath());
            }
#endif
            _WWWFileLoader.Dispose();
            OnGetVersion();
        }

        protected abstract void OnGetVersion();
    }
}
