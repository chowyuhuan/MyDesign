using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_hero_fruit_attribute : CSVBase
{
	#region data
	//<BeginAttribute>
	public int TemplateId;
	public float Attack;
	public float Hp;
	public float CriteRate;
	public float Phdef;
	public float MDef;
	public float CritDamage;
	public float HitRate;
	public float EvasionRate;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_hero_fruit_attribute> csv_data = new List<CSV_b_hero_fruit_attribute>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_hero_fruit_attribute", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_hero_fruit_attribute", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_hero_fruit_attribute item 		= new CSV_b_hero_fruit_attribute();

			//<BeginAssign>
			item.TemplateId = new_file.GetInt("TemplateId");
			item.Attack = new_file.GetFloat("Attack");
			item.Hp = new_file.GetFloat("Hp");
			item.CriteRate = new_file.GetFloat("CriteRate");
			item.Phdef = new_file.GetFloat("Phdef");
			item.MDef = new_file.GetFloat("MDef");
			item.CritDamage = new_file.GetFloat("CritDamage");
			item.HitRate = new_file.GetFloat("HitRate");
			item.EvasionRate = new_file.GetFloat("EvasionRate");

			
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
	public static CSV_b_hero_fruit_attribute GetData( int index )
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
    public static CSV_b_hero_fruit_attribute FindData(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find( x => x.TemplateId == index );
    }

	/// <summary>
    /// 通过键值取得数据
    /// </summary>
    /// <param name="index">键值</param>
    /// <returns>满足条件的所有数据</returns>
	public static List<CSV_b_hero_fruit_attribute> FindAll(int index)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.FindAll( x => x.TemplateId == index );
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
	public static List<CSV_b_hero_fruit_attribute> AllData
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


