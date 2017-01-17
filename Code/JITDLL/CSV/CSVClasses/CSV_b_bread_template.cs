using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_bread_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public string Name;
	public int Star;
	public string IconAtlas;
	public string BreadIcon;
	public int SuccessRate;
	public int TrainVolume;
	public int SaleCoin;
	public int TrainCoin2Star;
	public int TrainCoin3Star;
	public int TrainCoin4Star;
	public int TrainCoin5Star;
	public int TrainCoin6Star;
	public int CanRoast;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_bread_template> csv_data = new List<CSV_b_bread_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_bread_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_bread_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_bread_template item 		= new CSV_b_bread_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.Name = new_file.GetString("Name");
			item.Star = new_file.GetInt("Star");
			item.IconAtlas = new_file.GetString("IconAtlas");
			item.BreadIcon = new_file.GetString("BreadIcon");
			item.SuccessRate = new_file.GetInt("SuccessRate");
			item.TrainVolume = new_file.GetInt("TrainVolume");
			item.SaleCoin = new_file.GetInt("SaleCoin");
			item.TrainCoin2Star = new_file.GetInt("TrainCoin2Star");
			item.TrainCoin3Star = new_file.GetInt("TrainCoin3Star");
			item.TrainCoin4Star = new_file.GetInt("TrainCoin4Star");
			item.TrainCoin5Star = new_file.GetInt("TrainCoin5Star");
			item.TrainCoin6Star = new_file.GetInt("TrainCoin6Star");
			item.CanRoast = new_file.GetInt("CanRoast");

			
            item.OnReadRow(new_file);
			csv_data.Add( item );
			
			row_index++;
		}
	}

	/// <summary>
    /// 通过索引取得数据 
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>CSV的一行数据</returns>
	public static CSV_b_bread_template GetData( int index )
	{
		if( IsInited == false )
		{
			InitCSVTable();
		}
		
		int i = index;
		if( i < 0 ) i = 0;
		if( i >= csv_data.Count ) i = csv_data.Count - 1;
		
		return csv_data[i];
	}
	
	/// <summary>
    /// 通过键值取得数据 
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>CSV的一行数据</returns>
    public static CSV_b_bread_template FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.Id == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_b_bread_template> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.Id == index );
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
	public static List<CSV_b_bread_template> AllData
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


