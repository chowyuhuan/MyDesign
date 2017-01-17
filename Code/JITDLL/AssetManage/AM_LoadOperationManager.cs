using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetManage
{
    public class AM_LoadOperationManager
    {
        static List<string> _LoadingAssets = new List<string>();
        static List<AM_LoadOperation> _LoadingAssetOperations = new List<AM_LoadOperation>();
        static List<AM_LoadOperation> _ProcessingAssetOperations = new List<AM_LoadOperation>();

        static List<AM_LoadOperation> _LoadingABOperations = new List<AM_LoadOperation>();
        static List<string> _LoadingAssetBundles = new List<string>();

        public static void Update()
        {
            UpdateLoadingABOperations();
            UpdateAssetLoadOperation();
        }

        public static bool LoadingAB(string abName)
        {
            return _LoadingAssetBundles.Contains(abName);
        }

        public static bool LoadingAsset(string assetPath)
        {
            return _LoadingAssets.Contains(assetPath);
        }

        public static void RemoveLoadingAsset(string assetPath)
        {
            if(_LoadingAssets.Contains(assetPath))
            {
                _LoadingAssets.Remove(assetPath);
            }
        }

        public static void AddLoadAssetOperation(string assetPath, AM_LoadOperation loadop, bool simulate)
        {
            if (null == loadop)
            {
#if UNITY_EDITOR
                Debug.LogError("Null load operation !");
#endif
                return;
            }

            if (_LoadingAssets.Contains(assetPath))
            {
#if UNITY_EDITOR
                Debug.LogError("Repeat asset load operation !");
#endif
            }
            else
            {
                if(!simulate)
                {
                    _LoadingAssets.Add(assetPath);
                }
                _LoadingAssetOperations.Add(loadop);
            }
        }

        public static void AddLoadABOperation(string abName, AM_LoadOperation loadop)
        {
            if (null == loadop)
            {
#if UNITY_EDITOR
                Debug.LogError("Null load operation !");
#endif
                return;
            }

            if (_LoadingAssetBundles.Contains(abName))
            {
#if UNITY_EDITOR
                Debug.LogError("Repeat AssetBundle load operation !");
#endif
            }
            else
            {
                _LoadingAssetBundles.Add(abName);
                _LoadingABOperations.Add(loadop);
            }
        }

        static void UpdateAssetLoadOperation()
        {
            for (int index = 0; index < _LoadingAssetOperations.Count; )
            {
                var operation = _LoadingAssetOperations[index];
                if (operation.Update())
                {
                    ++index;
                }
                else
                {
                    _LoadingAssetOperations.RemoveAt(index);
                    _ProcessingAssetOperations.Add(operation);
                }
            }

            for (int index = 0; index < _ProcessingAssetOperations.Count; )
            {
                AM_LoadOperation loadoperation = _ProcessingAssetOperations[index];
                if (loadoperation.IsDone())
                {
                    loadoperation.LoadDone();
                    _ProcessingAssetOperations.RemoveAt(index);
                }
                else
                {
                    ++index;
                }
            }
        }

        static void UpdateLoadingABOperations()
        {
            for (int index = 0; index < _LoadingABOperations.Count; )
            {
                var operation = _LoadingABOperations[index];
                if (operation.Update())
                {
                    ++index;
                }
                else
                {
                    _LoadingABOperations.RemoveAt(index);
                    ProcessFinishedABOperation(operation);
                }
            }
        }

        static void ProcessFinishedABOperation(AM_LoadOperation operation)
        {
            AM_LoadABWithWWW loadOperation = operation as AM_LoadABWithWWW;
            if (null == loadOperation)
            {
                return;
            }

            if (loadOperation._LoadError == null)
            {
                AM_AssetRepository.AddLoadedAB(loadOperation._AssetBundleName, loadOperation._LoadedAssetBundle);
            }
            else
            {
                AM_AssetRepository.AddLoadABError(loadOperation._AssetBundleName, loadOperation._LoadError);
            }
            loadOperation.LoadDone();
            _LoadingAssetBundles.Remove(loadOperation._AssetBundleName);
        }
    }
}
