using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_expedition_random_award : CSVBase
{
	#region data
	//<BeginAttribute>
	public int AwardId;
	public int GroupId;
	public int Weight;
	public int RandomAwardGroup1;
	public int ConditionCount1;
	public string BoxAtlas1;
	public string BoxIcon1;
	public int RandomAwardGroup2;
	public int ConditionCount2;
	public string BoxAtlas2;
	public string BoxIcon2;
	public int RandomAwardGroup3;
	public int ConditionCount3;
	public string BoxAtlas3;
	public string BoxIcon3;
	public int ConditionType1;
	public int ConditionValue1;
	public int ConditionType2;
	public int ConditionValue2;
	public int ConditionType3;
	public int ConditionValue3;
	public int ConditionType4;
	public int ConditionValue4;
	public int ConditionType5;
	public int ConditionValue5;
	public int ConditionType6;
	public int ConditionValue6;
	public int ConditionType7;
	public int ConditionValue7;
	public int ConditionType8;
	public int ConditionValue8;
	public int ConditionType9;
	public int ConditionValue9;
	public int ConditionType10;
	public int ConditionValue10;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_expedition_random_award> csv_data = new List<CSV_b_expedition_random_award>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_expedition_random_award", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_expedition_random_award", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_expedition_random_award item 		= new CSV_b_expedition_random_award();

			//<BeginAssign>
			item.AwardId = new_file.GetInt("AwardId");
			item.GroupId = new_file.GetInt("GroupId");
			item.Weight = new_file.GetInt("Weight");
			item.RandomAwardGroup1 = new_file.GetInt("RandomAwardGroup1");
			item.ConditionCount1 = new_file.GetInt("ConditionCount1");
			item.BoxAtlas1 = new_file.GetString("BoxAtlas1");
			item.BoxIcon1 = new_file.GetString("BoxIcon1");
			item.RandomAwardGroup2 = new_file.GetInt("RandomAwardGroup2");
			item.ConditionCount2 = new_file.GetInt("ConditionCount2");
			item.BoxAtlas2 = new_file.GetString("BoxAtlas2");
			item.BoxIcon2 = new_file.GetString("BoxIcon2");
			item.RandomAwardGroup3 = new_file.GetInt("RandomAwardGroup3");
			item.ConditionCount3 = new_file.GetInt("ConditionCount3");
			item.BoxAtlas3 = new_file.GetString("BoxAtlas3");
			item.BoxIcon3 = new_file.GetString("BoxIcon3");
			item.ConditionType1 = new_file.GetInt("ConditionType1");
			item.ConditionValue1 = new_file.GetInt("ConditionValue1");
			item.ConditionType2 = new_file.GetInt("ConditionType2");
			item.ConditionValue2 = new_file.GetInt("ConditionValue2");
			item.ConditionType3 = new_file.GetInt("ConditionType3");
			item.ConditionValue3 = new_file.GetInt("ConditionValue3");
			item.ConditionType4 = new_file.GetInt("ConditionType4");
			item.ConditionValue4 = new_file.GetInt("ConditionValue4");
			item.ConditionType5 = new_file.GetInt("ConditionType5");
			item.ConditionValue5 = new_file.GetInt("ConditionValue5");
			item.ConditionType6 = new_file.GetInt("ConditionType6");
			item.ConditionValue6 = new_file.GetInt("ConditionValue6");
			item.ConditionType7 = new_file.GetInt("ConditionType7");
			item.ConditionValue7 = new_file.GetInt("ConditionValue7");
			item.ConditionType8 = new_file.GetInt("ConditionType8");
			item.ConditionValue8 = new_file.GetInt("ConditionValue8");
			item.ConditionType9 = new_file.GetInt("ConditionType9");
			item.ConditionValue9 = new_file.GetInt("ConditionValue9");
			item.ConditionType10 = new_file.GetInt("ConditionType10");
			item.ConditionValue10 = new_file.GetInt("ConditionValue10");

			
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
	public static CSV_b_expedition_random_award GetData( int index )
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
    public static CSV_b_expedition_random_award FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.AwardId == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_b_expedition_random_award> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.AwardId == index );
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
	public static List<CSV_b_expedition_random_award> AllData
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


