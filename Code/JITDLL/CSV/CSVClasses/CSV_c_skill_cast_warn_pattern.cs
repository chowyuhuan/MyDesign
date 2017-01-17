using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_skill_cast_warn_pattern : CSVBase
{
	#region data
	//<BeginAttribute>
	public int ID;
	public int NameAsTitle;
	public int IntentAsDescription;
	public string CastColor;
	public string Title;
	public string TitleColor;
	public string Description;
	public string DescriptionColor;
	public int ShowHeadUpTip;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_c_skill_cast_warn_pattern> csv_data = new List<CSV_c_skill_cast_warn_pattern>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/c_csv/c_skill_cast_warn_pattern", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/c_csv/c_skill_cast_warn_pattern", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_c_skill_cast_warn_pattern item 		= new CSV_c_skill_cast_warn_pattern();

			//<BeginAssign>
			item.ID = new_file.GetInt("ID");
			item.NameAsTitle = new_file.GetInt("NameAsTitle");
			item.IntentAsDescription = new_file.GetInt("IntentAsDescription");
			item.CastColor = new_file.GetString("CastColor");
			item.Title = new_file.GetString("Title");
			item.TitleColor = new_file.GetString("TitleColor");
			item.Description = new_file.GetString("Description");
			item.DescriptionColor = new_file.GetString("DescriptionColor");
			item.ShowHeadUpTip = new_file.GetInt("ShowHeadUpTip");

			
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
	public static CSV_c_skill_cast_warn_pattern GetData( int index )
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
    public static CSV_c_skill_cast_warn_pattern FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.ID == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_c_skill_cast_warn_pattern> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.ID == index );
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
	public static List<CSV_c_skill_cast_warn_pattern> AllData
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


