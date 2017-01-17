using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssetUpdate
{
    public class AU_AppConfig
    {
        public enum EFilePos
        {
            Remote,         /**< 网络 */
            LocalExecute,   /**< streamingAssetsPath */
            LocalCache      /**< persistentDataPath */
        }

        public static string _RemoteUrl
        {
            get;
            private set;
        }
        public static string _LocalExecutePath
        {
            get;
            private set;
        }
        public static string _LocalCachePath
        {
            get;
            private set;
        }
        public static string _JITUPFile
        {
            get;
            private set;
        }
        protected static Dictionary<string, string> _ConfigData = new Dictionary<string, string>();

        public static void Init()
        {
            if (!Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _LocalExecutePath = "file://" + Application.streamingAssetsPath;
                }
                else
                {
                    _LocalExecutePath = Application.streamingAssetsPath;
                }
            }
            else
            {
                _LocalExecutePath = "file://" + Application.streamingAssetsPath;
            }

            _LocalCachePath = Application.persistentDataPath + "/" +  "JITData";
            AU_FileHelper.CreateDirectory(_LocalCachePath);
#if UNITY_EDITOR
            Debug.Log("[更新]本地执行路径=" + _LocalExecutePath);
            Debug.Log("[更新]本地缓存路径=" + _LocalCachePath);
#endif
        }

        public static string GetPathOrUrl(EFilePos type)
        {
            switch (type)
            {
                case EFilePos.Remote:
                    return _RemoteUrl;
                case EFilePos.LocalExecute:
                    return _LocalExecutePath;
                case EFilePos.LocalCache:
                    return _LocalCachePath;
            }
            return "";
        }


        public static bool ParseConfigFromLines(string[] lines, bool local)
        {
            try
            {
                if (lines.Length == 0)
                {
                    Debug.Log("[更新]加载配置文件失败，配置文件为空!");
                    return false;
                }

                _ConfigData.Clear();
                foreach (var l in lines)
                {
                    var sp = l.Split('=');
                    _ConfigData.Add(sp[0], sp[1]);
                }
                if (_ConfigData.Count == 0)
                {
                    Debug.Log("[更新]加载配置文件失败，配置文件为空!");
                    return false;
                }

                /* 仅仅为了优化，理论上应该通过GetConfigValue查找RemoteUrl的 */
                string tmValue;
                if (!GetConfigValue("RemoteURL", true, out tmValue))
                {
                    Debug.Log("[更新]加载配置文件失败，缺少键值： RemoteURL!");
                    return false;
                }
                _RemoteUrl = tmValue;
                if (GetConfigValue("JITUPApp", true, out tmValue))
                {
                    _JITUPFile = tmValue;
                }
#if UNITY_EDITOR
                PrintConfigData();
#endif
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("[更新]异常： 加载配置文件失败！" + e.Message);
                return false;
            }
        }

#if UNITY_EDITOR
        public static void PrintConfigData()
        {
            Debug.Log("[更新]配置文件--------------------- ");
            foreach (var value in _ConfigData)
            {
                Debug.Log("[更新][" + value.Key + "]=[" + value.Value + "]");
            }
        }
#endif

        public static bool GetConfigValue(string key, bool log, out string value)
        {
            bool result = _ConfigData.TryGetValue(key, out value);
#if UNITY_EDITOR
            if (log && !result)
            {
                Debug.Log("[更新]获取配置信息失败，失败键： " + key);
            }
#endif
            return result;
        }
    }
}
