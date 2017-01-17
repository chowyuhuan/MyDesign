using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_game_chapter : CSVBase
{
	#region data
	//<BeginAttribute>
	public int ChapterId;
	public int ChapterType;
	public string ChapterName;
	public string Description;
	public string ChapterInformation;
	public string DisplayAtlas;
	public string DisplayIcon;
	public int Restriction;
	public int RestrictionTime;
	public string SectionList;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_c_game_chapter> csv_data = new List<CSV_c_game_chapter>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/c_csv/c_game_chapter", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/c_csv/c_game_chapter", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_c_game_chapter item 		= new CSV_c_game_chapter();

			//<BeginAssign>
			item.ChapterId = new_file.GetInt("ChapterId");
			item.ChapterType = new_file.GetInt("ChapterType");
			item.ChapterName = new_file.GetString("ChapterName");
			item.Description = new_file.GetString("Description");
			item.ChapterInformation = new_file.GetString("ChapterInformation");
			item.DisplayAtlas = new_file.GetString("DisplayAtlas");
			item.DisplayIcon = new_file.GetString("DisplayIcon");
			item.Restriction = new_file.GetInt("Restriction");
			item.RestrictionTime = new_file.GetInt("RestrictionTime");
			item.SectionList = new_file.GetString("SectionList");

			
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
	public static CSV_c_game_chapter GetData( int index )
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
    public static CSV_c_game_chapter FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.ChapterId == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_c_game_chapter> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.ChapterId == index );
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
	public static List<CSV_c_game_chapter> AllData
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


