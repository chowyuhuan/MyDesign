using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetManage
{
    public class AM_AssetLoadController
    {
        static Dictionary<string, string> _ABPathDic = new Dictionary<string, string>();
        static List<string> _ForceLoadFromAPP = new List<string>();
        public static void Init(bool remote)
        {
            //todo: read config file to prepare _PathMap
            if(remote)
            {
                InitFromCacheFile();
            }
            else
            {
                InitFromApp();
            }
        }

        static void InitFromCacheFile()
        {
            AssetBundle ab = AM_FileReader.ReadABFromFile(Application.persistentDataPath + "/JITData/res/Android/assets/resources/configs/csv/c_csv/c_force_load_from_app");
            TextAsset ta = null;
            if(null != ab)
            {
                ta = ab.LoadAsset<TextAsset>("c_force_load_from_app");
                InitFroceLoadList(ta);
            }

            ab = AM_FileReader.ReadABFromFile(Application.persistentDataPath + "/JITData/res/Android/assets/resources/configs/csv/c_csv/c_asset_to_ab_pathmap");
            ta = ab.LoadAsset<TextAsset>("c_asset_to_ab_pathmap");
            InitAssetABPathMap(ta);
        }

        static void InitFromApp()
        {
            TextAsset ta = Resources.Load<TextAsset>("Configs/csv/c_csv/c_force_load_from_app");
            InitFroceLoadList(ta);

            ta = Resources.Load<TextAsset>("Configs/csv/c_csv/c_asset_to_ab_pathmap");
            InitAssetABPathMap(ta);
        }

        static void InitFroceLoadList(TextAsset ta)
        {
            CSV_c_force_load_from_app.InitCSVTable(ta);
            for (int index = 0; index < CSV_c_force_load_from_app.DateCount; ++index)
            {
                CSV_c_force_load_from_app flfa = CSV_c_force_load_from_app.GetData(index);
                _ForceLoadFromAPP.Add(flfa.AssetPath);
            }
            CSV_c_force_load_from_app.Recycle();
        }

        static void InitAssetABPathMap(TextAsset ta)
        {
            CSV_c_asset_to_ab_pathmap.InitCSVTable(ta);
            for (int index = 0; index < CSV_c_asset_to_ab_pathmap.DateCount; ++index)
            {
                CSV_c_asset_to_ab_pathmap atab = CSV_c_asset_to_ab_pathmap.GetData(index);
                if(!_ABPathDic.ContainsKey(atab.AssetPath))
                {
                    _ABPathDic.Add(atab.AssetPath, atab.ABName);
                }
                else
                {
                    Debug.LogError("repeat:" + atab.AssetPath);
                }
            }
            CSV_c_asset_to_ab_pathmap.Recycle();
        }

        public static bool LoadAssetFromAB(string assetPath)
        {
            return _ABPathDic.ContainsKey(assetPath) && !_ForceLoadFromAPP.Contains(assetPath);
        }

        public static bool MapABName(string assetPath, out string abName)
        {
            return _ABPathDic.TryGetValue(assetPath, out abName);
        }
    }
}