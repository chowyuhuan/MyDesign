using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_game_level : CSVBase
{
	#region data
	//<BeginAttribute>
	public int LevelId;
	public string LevelPrefix;
	public string LevelName;
	public string MusicName;
	public string StartDescription;
	public string StartInformation;
	public int Difficulty;
	public int PassType;
	public int PrePass;
	public int CostType;
	public int CostCount;
	public string MonsterWaveList;
	public bool HasBoss;
	public int HiddenMonsterWave;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_game_level> csv_data = new List<CSV_b_game_level>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_game_level", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_game_level", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_game_level item 		= new CSV_b_game_level();

			//<BeginAssign>
			item.LevelId = new_file.GetInt("LevelId");
			item.LevelPrefix = new_file.GetString("LevelPrefix");
			item.LevelName = new_file.GetString("LevelName");
			item.MusicName = new_file.GetString("MusicName");
			item.StartDescription = new_file.GetString("StartDescription");
			item.StartInformation = new_file.GetString("StartInformation");
			item.Difficulty = new_file.GetInt("Difficulty");
			item.PassType = new_file.GetInt("PassType");
			item.PrePass = new_file.GetInt("PrePass");
			item.CostType = new_file.GetInt("CostType");
			item.CostCount = new_file.GetInt("CostCount");
			item.MonsterWaveList = new_file.GetString("MonsterWaveList");
			item.HasBoss = new_file.GetBool("HasBoss");
			item.HiddenMonsterWave = new_file.GetInt("HiddenMonsterWave");

			
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
	public static CSV_b_game_level GetData( int index )
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
    public static CSV_b_game_level FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.LevelId == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_b_game_level> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.LevelId == index );
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
	public static List<CSV_b_game_level> AllData
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


