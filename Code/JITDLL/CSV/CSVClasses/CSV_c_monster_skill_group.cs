using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_monster_skill_group : CSVBase
{
	#region data
	//<BeginAttribute>
	public int id;
	public int skillCount;
	public int skillId1;
	public float duration1;
	public int skillId2;
	public float duration2;
	public int skillId3;
	public float duration3;
	public int skillId4;
	public float duration4;
	public int skillId5;
	public float duration5;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_c_monster_skill_group> csv_data = new List<CSV_c_monster_skill_group>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/c_csv/c_monster_skill_group", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/c_csv/c_monster_skill_group", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_c_monster_skill_group item 		= new CSV_c_monster_skill_group();

			//<BeginAssign>
			item.id = new_file.GetInt("id");
			item.skillCount = new_file.GetInt("skillCount");
			item.skillId1 = new_file.GetInt("skillId1");
			item.duration1 = new_file.GetFloat("duration1");
			item.skillId2 = new_file.GetInt("skillId2");
			item.duration2 = new_file.GetFloat("duration2");
			item.skillId3 = new_file.GetInt("skillId3");
			item.duration3 = new_file.GetFloat("duration3");
			item.skillId4 = new_file.GetInt("skillId4");
			item.duration4 = new_file.GetFloat("duration4");
			item.skillId5 = new_file.GetInt("skillId5");
			item.duration5 = new_file.GetFloat("duration5");

			
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
	public static CSV_c_monster_skill_group GetData( int index )
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
    public static CSV_c_monster_skill_group FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.id == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_c_monster_skill_group> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.id == index );
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
	public static List<CSV_c_monster_skill_group> AllData
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


