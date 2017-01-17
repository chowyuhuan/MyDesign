using UnityEngine;
using System.Collections;

namespace Platform
{
    public class PlatformInterface : MonoBehaviour
    {
        public static System.Action<string> OnPlatformLoginCallback;
        public static System.Action<string> OnPlatformPayCallback;
        public static System.Action<string> OnPlatformQuitCallback;

        static PlatformBase platform = null;

        //[RuntimeInitializeOnLoadMethod]
        public static void Setup()
        {
            GameObject go = new GameObject("PlatformSdk");
            DontDestroyOnLoad(go);

            go.AddComponent<PlatformInterface>();

#if PLATFORM_XIAOMI
            platform = new PlatformXiaomi;
#endif

#if PLATFORM_TOURIST
            platform = new PlatformTourist();
#endif

            if (platform != null)
            {
                platform.Init(go);
            }
        }

        static bool Invalid()
        {
            return platform == null;
        }

        public static void CallPlatformLogin()
        {
            if (Invalid())
                return;

            platform.CallPlatformLogin();
        }

        public static void CallPlatformQuit()
        {
            if (Invalid())
                return;

            platform.CallPlatformQuit();
        }

        public static void CallPlatformPay(string data)
        {
            if (Invalid())
                return;

            platform.CallPlatformPay(data);
        }

        void OnPlatformLogin(string data)
        {
            //Debug.Log("OnPlatformLogin");
            if (OnPlatformLoginCallback != null)
            {
                OnPlatformLoginCallback(data);
            }
        }

        void OnPlatformQuit(string data)
        {
            //Debug.Log("OnPlatformQuit");
            if (OnPlatformQuitCallback != null)
            {
                OnPlatformQuitCallback(data);
            }
        }

        void OnPlatformPay(string data)
        {
            //Debug.Log("OnPlatformPay");
            if (OnPlatformPayCallback != null)
            {
                OnPlatformPayCallback(data);
            }
        }

        /// <summary>
        /// 处理不同平台到我们的服务器Verify字段赋值 
        /// </summary>
        /// <param name="data">底层回调传入的json字串</param>
        /// <returns></returns>
        public static PbLogin.VerifyReq GetVerifyReqInfo(string data)
        {
            return platform.GetVerifyReqInfo(data);
        }

        public static void AppendCreateReqInfo(ref PbLogin.CreateReq req, string data)
        {
            platform.AppendCreateReqInfo(ref req, data);
        }
    }
}

