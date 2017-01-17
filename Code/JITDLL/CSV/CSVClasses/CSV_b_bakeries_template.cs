using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_bakeries_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public string Name;
	public string IconAltas;
	public string IconSprite;
	public string Product;
	public string TimeDesciption;
	public int NeedTime;
	public string BreadStar1;
	public string BreadNum1;
	public string BreadStar2;
	public string BreadNum2;
	public int OpenLevel;
	public int FinishCostPerUnit;
	public int FinishUnit;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_bakeries_template> csv_data = new List<CSV_b_bakeries_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_bakeries_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_bakeries_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_bakeries_template item 		= new CSV_b_bakeries_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.Name = new_file.GetString("Name");
			item.IconAltas = new_file.GetString("IconAltas");
			item.IconSprite = new_file.GetString("IconSprite");
			item.Product = new_file.GetString("Product");
			item.TimeDesciption = new_file.GetString("TimeDesciption");
			item.NeedTime = new_file.GetInt("NeedTime");
			item.BreadStar1 = new_file.GetString("BreadStar1");
			item.BreadNum1 = new_file.GetString("BreadNum1");
			item.BreadStar2 = new_file.GetString("BreadStar2");
			item.BreadNum2 = new_file.GetString("BreadNum2");
			item.OpenLevel = new_file.GetInt("OpenLevel");
			item.FinishCostPerUnit = new_file.GetInt("FinishCostPerUnit");
			item.FinishUnit = new_file.GetInt("FinishUnit");

			
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
	public static CSV_b_bakeries_template GetData( int index )
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
    public static CSV_b_bakeries_template FindData(int index)
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
	public static List<CSV_b_bakeries_template> FindAll(int index)
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
	public static List<CSV_b_bakeries_template> AllData
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


