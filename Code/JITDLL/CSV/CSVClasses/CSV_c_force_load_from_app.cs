using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_force_load_from_app : CSVBase
{
    #region data
    //<BeginAttribute>
    public int Index;
    public string AssetPath;


    #endregion

    private static bool _InitDone = false;

    private static List<CSV_c_force_load_from_app> csv_data = new List<CSV_c_force_load_from_app>();

    /// <summary>
    /// 初始化 
    /// </summary>
    private static void InitCSVTable()
    {
        TextAsset ta = AssetManage.AM_Manager.LoadAssetSync<TextAsset>("Configs/csv/c_csv/CSV_c_force_load_from_app", true, AssetManage.E_AssetType.Normal);
        InitCSVTable(ta);
    }

    public static void InitCSVTable(TextAsset ta)
    {
        if (ta == null)
            return;
        CSVDataFile new_file = new CSVDataFile();
        new_file.ParseCSVFor(ta);

        int row_index = 2;

        while (new_file.SetRow(row_index))
        {
            CSV_c_force_load_from_app item = new CSV_c_force_load_from_app();

            //<BeginAssign>
            item.Index = new_file.GetInt("Index");
            item.AssetPath = new_file.GetString("AssetPath");


            item.OnReadRow(new_file);
            csv_data.Add(item);

            row_index++;
        }

        _InitDone = true;
    }

    /// <summary>
    /// 通过索引取得数据 
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>CSV的一行数据</returns>
    public static CSV_c_force_load_from_app GetData(int index)
    {
        if (_InitDone == false)
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
    public static CSV_c_force_load_from_app FindData(int index)
    {
        if (_InitDone == false)
        {
            InitCSVTable();
        }

        return csv_data.Find(x => x.Index == index);
    }

    /// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
    public static List<CSV_c_force_load_from_app> FindAll(int index)
    {
        if (_InitDone == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll(x => x.Index == index);
    }

    /// <summary>
    /// 数据总行数 
    /// </summary>
    public static int DateCount
    {
        get
        {
            if (_InitDone == false)
            {
                InitCSVTable();
            }

            return csv_data.Count;
        }
    }

    /// <summary>
    /// 返回所有数据
    /// </summary>
    public static List<CSV_c_force_load_from_app> AllData
    {
        get
        {
            if (_InitDone == false)
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
        _InitDone = false;
        csv_data.Clear();
    }
}


