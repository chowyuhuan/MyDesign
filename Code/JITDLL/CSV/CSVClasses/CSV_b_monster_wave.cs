using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_monster_wave : CSVBase
{
	#region data
	//<BeginAttribute>
	public int id;
	public int monsterCount;
	public int monsterId1;
	public float monsterOffset1;
	public int monsterId2;
	public float monsterOffset2;
	public int monsterId3;
	public float monsterOffset3;
	public int monsterId4;
	public float monsterOffset4;
	public int monsterId5;
	public float monsterOffset5;
	public int monsterId6;
	public float monsterOffset6;
	public int monsterId7;
	public float monsterOffset7;
	public int monsterId8;
	public float monsterOffset8;
	public int monsterId9;
	public float monsterOffset9;
	public int monsterId10;
	public float monsterOffset10;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_monster_wave> csv_data = new List<CSV_b_monster_wave>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_monster_wave", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_monster_wave", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_monster_wave item 		= new CSV_b_monster_wave();

			//<BeginAssign>
			item.id = new_file.GetInt("id");
			item.monsterCount = new_file.GetInt("monsterCount");
			item.monsterId1 = new_file.GetInt("monsterId1");
			item.monsterOffset1 = new_file.GetFloat("monsterOffset1");
			item.monsterId2 = new_file.GetInt("monsterId2");
			item.monsterOffset2 = new_file.GetFloat("monsterOffset2");
			item.monsterId3 = new_file.GetInt("monsterId3");
			item.monsterOffset3 = new_file.GetFloat("monsterOffset3");
			item.monsterId4 = new_file.GetInt("monsterId4");
			item.monsterOffset4 = new_file.GetFloat("monsterOffset4");
			item.monsterId5 = new_file.GetInt("monsterId5");
			item.monsterOffset5 = new_file.GetFloat("monsterOffset5");
			item.monsterId6 = new_file.GetInt("monsterId6");
			item.monsterOffset6 = new_file.GetFloat("monsterOffset6");
			item.monsterId7 = new_file.GetInt("monsterId7");
			item.monsterOffset7 = new_file.GetFloat("monsterOffset7");
			item.monsterId8 = new_file.GetInt("monsterId8");
			item.monsterOffset8 = new_file.GetFloat("monsterOffset8");
			item.monsterId9 = new_file.GetInt("monsterId9");
			item.monsterOffset9 = new_file.GetFloat("monsterOffset9");
			item.monsterId10 = new_file.GetInt("monsterId10");
			item.monsterOffset10 = new_file.GetFloat("monsterOffset10");

			
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
	public static CSV_b_monster_wave GetData( int index )
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
    public static CSV_b_monster_wave FindData(int index)
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
	public static List<CSV_b_monster_wave> FindAll(int index)
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
	public static List<CSV_b_monster_wave> AllData
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


