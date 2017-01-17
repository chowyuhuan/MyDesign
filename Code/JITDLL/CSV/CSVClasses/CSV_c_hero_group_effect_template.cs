using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_hero_group_effect_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int CombinedId;
	public string Name;
	public string Description;
	public string DescriptionAtlas;
	public string DescriptionSrpite;
	public string Condition;
	public string DisplayIconAtlas;
	public string DisplayIconSprite;
	public string EffectBuffAtlas;
	public string EffectBuffSprite;
	public string LevelText;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_c_hero_group_effect_template> csv_data = new List<CSV_c_hero_group_effect_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/c_csv/c_hero_group_effect_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/c_csv/c_hero_group_effect_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_c_hero_group_effect_template item 		= new CSV_c_hero_group_effect_template();

			//<BeginAssign>
			item.CombinedId = new_file.GetInt("CombinedId");
			item.Name = new_file.GetString("Name");
			item.Description = new_file.GetString("Description");
			item.DescriptionAtlas = new_file.GetString("DescriptionAtlas");
			item.DescriptionSrpite = new_file.GetString("DescriptionSrpite");
			item.Condition = new_file.GetString("Condition");
			item.DisplayIconAtlas = new_file.GetString("DisplayIconAtlas");
			item.DisplayIconSprite = new_file.GetString("DisplayIconSprite");
			item.EffectBuffAtlas = new_file.GetString("EffectBuffAtlas");
			item.EffectBuffSprite = new_file.GetString("EffectBuffSprite");
			item.LevelText = new_file.GetString("LevelText");

			
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
	public static CSV_c_hero_group_effect_template GetData( int index )
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
    public static CSV_c_hero_group_effect_template FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.CombinedId == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_c_hero_group_effect_template> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.CombinedId == index );
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
	public static List<CSV_c_hero_group_effect_template> AllData
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


