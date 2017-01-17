using UnityEngine;
using System.Collections;

namespace Platform
{
    /// <summary>
    /// 对应不同平台登录，充值等处理的基类，希望IOS也能合到这里 
    /// </summary>
    public class PlatformBase
    {
#if UNITY_ANDROID
        protected AndroidJavaObject _plugin;
#elif UNITY_IPHONE

#endif

        protected GameObject _platformObject;

        public void Init(GameObject go)
        {
            _platformObject = go;

            if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
            {
#if UNITY_ANDROID
                using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    _plugin = pluginClass.GetStatic<AndroidJavaObject>("currentActivity");
#elif UNITY_IPHONE

#endif
            }

        }

        public virtual void CallPlatformLogin()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
            {
#if UNITY_ANDROID
                _plugin.Call("callPlatformLogin");
#elif UNITY_IPHONE

#endif
            }
        }

        public virtual void CallPlatformQuit()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
            {
#if UNITY_ANDROID
                _plugin.Call("callPlatformQuit");
#elif UNITY_IPHONE

#endif
            }
        }

        public virtual void CallPlatformPay(string data)
        {
            if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
            {
#if UNITY_ANDROID
                _plugin.Call("callPlatformPay", data);
#elif UNITY_IPHONE

#endif
            }
        }

        public virtual PbLogin.VerifyReq GetVerifyReqInfo(string data)
        {
            PbLogin.VerifyReq verifyReq = new PbLogin.VerifyReq();

            return verifyReq;
        }

        public virtual void AppendCreateReqInfo(ref PbLogin.CreateReq req, string data)
        {

        }
    }
}

