using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AM_PathHelper_Editor
{
    public static string GetAssetBundlePlatFormFolder()
    {
#if UNITY_EDITOR
        return GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformFolderForAssetBundles(Application.platform);
#endif
    }

#if UNITY_EDITOR
    public static string GetPlatformBuildAchieveSuffix()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.iOS:
                return ".IPA";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
            default:
                return "noneidentified";
        }
    }

    static string GetPlatformFolderForAssetBundles(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return AM_Config.Platform_Folder_Android;
            case BuildTarget.iOS:
                return AM_Config.Platform_Folder_IOS;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return AM_Config.Platform_Folder_Windows;
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return AM_Config.Platform_Folder_OSX;
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }
#endif

    static string GetPlatformFolderForAssetBundles(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return AM_Config.Platform_Folder_Android;
            case RuntimePlatform.IPhonePlayer:
                return AM_Config.Platform_Folder_IOS;
            case RuntimePlatform.WindowsPlayer:
                return AM_Config.Platform_Folder_Windows;
            case RuntimePlatform.OSXPlayer:
                return AM_Config.Platform_Folder_OSX;
            // Add more build platform for your own.
            // If you add more platforms, don't forget to add the same targets to GetPlatformFolderForAssetBundles(BuildTarget) function.
            default:
                return null;
        }
    }

    public static string GetRelativePath()
    {
        if (Application.isEditor)
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
        else if (Application.isWebPlayer)
            return System.IO.Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
            return Application.streamingAssetsPath;
        else // For standalone player.
            return "file://" + Application.streamingAssetsPath;
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
