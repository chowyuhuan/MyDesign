using UnityEngine;
using System.Collections;

namespace AssetUpdate
{
    public class AU_PathHelper
    {
        public static string GetPersistentDataPath()
        {
            return Application.persistentDataPath + "/" + AU_Config.Asset_Base_Folder;
        }

        public static string GetStreamingAssetPath()
        {
            string StreamingAssetPath = null;
#if UNITY_ANDROID && !UNITY_EDITOR
            StreamingAssetPath = Application.streamingAssetsPath;
#else
            StreamingAssetPath = "file://" + Application.streamingAssetsPath;
#endif
            return StreamingAssetPath;
        }
    }
}
