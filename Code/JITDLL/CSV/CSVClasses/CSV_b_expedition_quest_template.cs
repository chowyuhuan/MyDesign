using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_expedition_quest_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public string Name;
	public string QuestAtlas;
	public string QuestIcon;
	public string AreaAtlas;
	public string AreaIcon;
	public string Dialog;
	public int QuestGroup;
	public int HeroGroupLevel;
	public int Weight;
	public int ExpeditionTime;
	public int HeroGroupExp;
	public int HeroExp;
	public int HeroCount;
	public int MapPieceCount;
	public int FixAwardType1;
	public int FixAwardValue1;
	public int FixAwardType2;
	public int FixAwardValue2;
	public int FixAwardType3;
	public int FixAwardValue3;
	public int RandomAwardGroupId;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_expedition_quest_template> csv_data = new List<CSV_b_expedition_quest_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_expedition_quest_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_expedition_quest_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_expedition_quest_template item 		= new CSV_b_expedition_quest_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.Name = new_file.GetString("Name");
			item.QuestAtlas = new_file.GetString("QuestAtlas");
			item.QuestIcon = new_file.GetString("QuestIcon");
			item.AreaAtlas = new_file.GetString("AreaAtlas");
			item.AreaIcon = new_file.GetString("AreaIcon");
			item.Dialog = new_file.GetString("Dialog");
			item.QuestGroup = new_file.GetInt("QuestGroup");
			item.HeroGroupLevel = new_file.GetInt("HeroGroupLevel");
			item.Weight = new_file.GetInt("Weight");
			item.ExpeditionTime = new_file.GetInt("ExpeditionTime");
			item.HeroGroupExp = new_file.GetInt("HeroGroupExp");
			item.HeroExp = new_file.GetInt("HeroExp");
			item.HeroCount = new_file.GetInt("HeroCount");
			item.MapPieceCount = new_file.GetInt("MapPieceCount");
			item.FixAwardType1 = new_file.GetInt("FixAwardType1");
			item.FixAwardValue1 = new_file.GetInt("FixAwardValue1");
			item.FixAwardType2 = new_file.GetInt("FixAwardType2");
			item.FixAwardValue2 = new_file.GetInt("FixAwardValue2");
			item.FixAwardType3 = new_file.GetInt("FixAwardType3");
			item.FixAwardValue3 = new_file.GetInt("FixAwardValue3");
			item.RandomAwardGroupId = new_file.GetInt("RandomAwardGroupId");

			
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
	public static CSV_b_expedition_quest_template GetData( int index )
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
    public static CSV_b_expedition_quest_template FindData(int index)
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
	public static List<CSV_b_expedition_quest_template> FindAll(int index)
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
	public static List<CSV_b_expedition_quest_template> AllData
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


