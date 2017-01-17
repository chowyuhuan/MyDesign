using UnityEngine;
using System.Collections;

namespace Network
{
    /// <summary>
    /// 给NetworkManager提供主线程的Update 
    /// </summary>
    public class NetworkUpdate : MonoBehaviour
    {
        public static void CreateInstance()
        {
            GameObject go = new GameObject("NetworkUpdate");
            //go.hideFlags = HideFlags.HideAndDontSave;

            GameObject.DontDestroyOnLoad(go);

            go.AddComponent<NetworkUpdate>();
        }

        void OnApplicationQuit()
        {
            //Debug.Log("OnApplicationQuit");
            NetworkManager.ClearData();
            NetworkManager.ThreadAbort();
        }

        // Update is called once per frame
        void Update()
        {
            NetworkManager.OnUpdate();
        }
    }
}

