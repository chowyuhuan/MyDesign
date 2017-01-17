using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_equip_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public int School;
	public int EquipType;
	public int WeaponType;
	public string Prefab;
	public int Attack;
	public float AttackSpeed;
	public int Star;
	public string Name;
	public string ExclusiveDescription;
	public int SaleCoin;
	public int BreakIron;
	public string IconAtlas;
	public string IconSprite;
	public int CanReform;
	public int CanSale;
	public int CanRefine;
	public int CanBreak;
	public int OriginalRefineGet;
	public int RefineGetRate;
	public int NextRefineId;
	public int SpecialSkillId;
	public int BreakCoin;
	public int Hero1;
	public int Hero2;
	public int Hero3;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_equip_template> csv_data = new List<CSV_b_equip_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_equip_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_equip_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_equip_template item 		= new CSV_b_equip_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.School = new_file.GetInt("School");
			item.EquipType = new_file.GetInt("EquipType");
			item.WeaponType = new_file.GetInt("WeaponType");
			item.Prefab = new_file.GetString("Prefab");
			item.Attack = new_file.GetInt("Attack");
			item.AttackSpeed = new_file.GetFloat("AttackSpeed");
			item.Star = new_file.GetInt("Star");
			item.Name = new_file.GetString("Name");
			item.ExclusiveDescription = new_file.GetString("ExclusiveDescription");
			item.SaleCoin = new_file.GetInt("SaleCoin");
			item.BreakIron = new_file.GetInt("BreakIron");
			item.IconAtlas = new_file.GetString("IconAtlas");
			item.IconSprite = new_file.GetString("IconSprite");
			item.CanReform = new_file.GetInt("CanReform");
			item.CanSale = new_file.GetInt("CanSale");
			item.CanRefine = new_file.GetInt("CanRefine");
			item.CanBreak = new_file.GetInt("CanBreak");
			item.OriginalRefineGet = new_file.GetInt("OriginalRefineGet");
			item.RefineGetRate = new_file.GetInt("RefineGetRate");
			item.NextRefineId = new_file.GetInt("NextRefineId");
			item.SpecialSkillId = new_file.GetInt("SpecialSkillId");
			item.BreakCoin = new_file.GetInt("BreakCoin");
			item.Hero1 = new_file.GetInt("Hero1");
			item.Hero2 = new_file.GetInt("Hero2");
			item.Hero3 = new_file.GetInt("Hero3");

			
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
	public static CSV_b_equip_template GetData( int index )
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
    public static CSV_b_equip_template FindData(int index)
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
	public static List<CSV_b_equip_template> FindAll(int index)
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
	public static List<CSV_b_equip_template> AllData
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


