using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_asset_to_ab_pathmap : CSVBase
{
    #region data
    //<BeginAttribute>
    public string AssetPath;
    public string ABName;


    #endregion

    private static bool IsInited
    {
        get
        {
            return _InitDone;
        }
    }
    private static bool _InitDone = false;
    private static List<CSV_c_asset_to_ab_pathmap> csv_data = new List<CSV_c_asset_to_ab_pathmap>();

    /// <summary>
    /// 初始化 
    /// </summary>
    private static void InitCSVTable()
    {
        TextAsset ta = AssetManage.AM_Manager.LoadAssetSync<TextAsset>("Configs/csv/c_csv/c_asset_to_ab_pathmap", true, AssetManage.E_AssetType.Normal);

        InitCSVTable(ta);
    }

    public static void InitCSVTable(TextAsset ta)
    {
        if (ta == null)
            return;
        _InitDone = true;
        CSVDataFile new_file = new CSVDataFile();
        new_file.ParseCSVFor(ta);

        int row_index = 2;

        while (new_file.SetRow(row_index))
        {
            CSV_c_asset_to_ab_pathmap item = new CSV_c_asset_to_ab_pathmap();

            //<BeginAssign>
            item.AssetPath = new_file.GetString("AssetPath");
            item.ABName = new_file.GetString("ABName");


            item.OnReadRow(new_file);
            csv_data.Add(item);

            row_index++;
        }
    }

    /// <summary>
    /// 通过索引取得数据 
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>CSV的一行数据</returns>
    public static CSV_c_asset_to_ab_pathmap GetData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        int i = index;
        if (i < 0) i = 0;
        if (i >= csv_data.Count) i = csv_data.Count - 1;

        return csv_data[i];
    }

    /// <summary>
    /// 通过键值取得数据 
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>CSV的一行数据</returns>
    public static CSV_c_asset_to_ab_pathmap FindData(string index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find(x => x.AssetPath == index);
    }

    /// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
    public static List<CSV_c_asset_to_ab_pathmap> FindAll(string index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll(x => x.AssetPath == index);
    }

    /// <summary>
    /// 数据总行数 
    /// </summary>
    public static int DateCount
    {
        get
        {
            if (IsInited == false)
            {
                InitCSVTable();
            }

            return csv_data.Count;
        }
    }

    /// <summary>
    /// 返回所有数据
    /// </summary>
    public static List<CSV_c_asset_to_ab_pathmap> AllData
    {
        get
        {
            if (IsInited == false)
            {
                InitCSVTable();
            }
            return csv_data;
        }
    }

    /// <summary>
    /// 清除数据，释放内存
    /// </summary>
    public static void Recycle()
    {
        csv_data.Clear();
    }
}


