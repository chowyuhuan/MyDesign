using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetUpdate
{
    public class AU_FileLoader : MonoBehaviour
    {
        public static AU_WWWFileLoader WWWLoader;
        public static AU_StreamFileLoader StreamLoader;
        static Action<bool> TaskFinish = null;

        public static void LoadFromWWW(string filename, string tag, Action<WWW, string> onLoadEnd)
        {
            try
            {
                WWWLoader.Load(filename, tag, onLoadEnd);
            }
            catch (Exception err)
            {
                Debug.Log("异常： LoadFromWWW :" + err.ToString());
            }
        }
        public static void LoadAssetBundleFromStream(string filename, string tag, Action<AssetBundle, string> onLoadEnd)
        {
            try
            {
                StreamLoader.LoadAssetBundle(filename, tag, onLoadEnd);
            }
            catch (Exception err)
            {
                Debug.Log("异常： LoadAssetBundleFromStream :" + err.ToString());
            }
        }
        public static AssetBundle LoadAssetBundleFromSteamImmediately(string filename)
        {
            try
            {
                return StreamLoader.LoadAssetBundleImmediately(filename);
            }
            catch (Exception err)
            {
                Debug.Log("异常： LoadAssetBundleImmediately :" + err.ToString());
                return null;
            }
        }
        public static byte[] LoadBytesFromStreamImmediately(string filename)
        {
            try
            {
                return StreamLoader.LoadBytesImmediately(filename);
            }
            catch (Exception err)
            {
                Debug.Log("异常： LoadBytesFromStreamImmediately :" + err.ToString());
                return null;
            }
        }
        public static Texture2D LoadTexture2DFromStreamImmediately(string filename)
        {
            try
            {
                return StreamLoader.LoadTexture2DImmediately(filename);
            }
            catch (Exception err)
            {
                Debug.Log("异常： LoadTexture2DFromStreamImmediately :" + err.ToString());
                return null;
            }
        }
        public static string LoadStringFromStreamImmediately(string filename)
        {
            try
            {
                return StreamLoader.LoadStringImmediately(filename);
            }
            catch (Exception err)
            {
                Debug.Log("异常： LoadStringFromStreamImmediately :" + err.ToString());
                return null;
            }
        }

        public enum ETask
        {
            WWW,
            Local,
            Both
        }
        public static int GetTaskCount(ETask type)
        {
            int tmNum = 0;
            if (type == ETask.WWW)
            {
                tmNum += WWWLoader.taskState.taskcount;
            }
            else if (type == ETask.Local)
            {
                tmNum += StreamLoader.taskState.taskcount;
            }
            else
            {
                tmNum += StreamLoader.taskState.taskcount + WWWLoader.taskState.taskcount;
            }
            return tmNum;
        }

        public static int GetFinishedCount(ETask type)
        {
            int tmNum = 0;
            if (type == ETask.WWW)
            {
                tmNum += WWWLoader.taskState.downloadcount;
            }
            else if (type == ETask.Local)
            {
                tmNum += StreamLoader.taskState.downloadcount;
            }
            else
            {
                tmNum += StreamLoader.taskState.downloadcount + WWWLoader.taskState.downloadcount;
            }
            return tmNum;
        }

        public static void SetDownloadFinishCallBack(Action<bool> finish)
        {
            if (TaskFinish != null)
            {
                TaskFinish += finish;
            }
            else
            {
                TaskFinish = finish;
            }
        }

        public static void Update()
        {
            StreamLoader.Update();
            WWWLoader.Update();

            //监测完成事件
            if (WWWLoader.DownLoadError
                || (StreamLoader.IsFinished() && WWWLoader.IsFinished() && TaskFinish != null))
            {
                Action<bool> tt = TaskFinish;
                tt(WWWLoader.DownLoadError);
                TaskFinish = null;
            }
        }
        static AU_FileLoader()
        {
            WWWLoader = new AU_WWWFileLoader();
            StreamLoader = new AU_StreamFileLoader();
        }
    }
}
