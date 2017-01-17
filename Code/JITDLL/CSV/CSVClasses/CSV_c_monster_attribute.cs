using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_monster_attribute : CSVBase
{
	#region data
	//<BeginAttribute>
	public int id;
	public float hp;
	public float attack;
	public float magicDefense;
	public float physicDefense;
	public float crticalRate;
	public float crticalDamage;
	public float dodge;
	public float precision;
	public float magicPenetration;
	public float physicPenetration;
	public float attackSpeed;
	public float suck;
	public float damageReduce;
	public float parry;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_c_monster_attribute> csv_data = new List<CSV_c_monster_attribute>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/c_csv/c_monster_attribute", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/c_csv/c_monster_attribute", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_c_monster_attribute item 		= new CSV_c_monster_attribute();

			//<BeginAssign>
			item.id = new_file.GetInt("id");
			item.hp = new_file.GetFloat("hp");
			item.attack = new_file.GetFloat("attack");
			item.magicDefense = new_file.GetFloat("magicDefense");
			item.physicDefense = new_file.GetFloat("physicDefense");
			item.crticalRate = new_file.GetFloat("crticalRate");
			item.crticalDamage = new_file.GetFloat("crticalDamage");
			item.dodge = new_file.GetFloat("dodge");
			item.precision = new_file.GetFloat("precision");
			item.magicPenetration = new_file.GetFloat("magicPenetration");
			item.physicPenetration = new_file.GetFloat("physicPenetration");
			item.attackSpeed = new_file.GetFloat("attackSpeed");
			item.suck = new_file.GetFloat("suck");
			item.damageReduce = new_file.GetFloat("damageReduce");
			item.parry = new_file.GetFloat("parry");

			
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
	public static CSV_c_monster_attribute GetData( int index )
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
    public static CSV_c_monster_attribute FindData(int index)
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
	public static List<CSV_c_monster_attribute> FindAll(int index)
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
	public static List<CSV_c_monster_attribute> AllData
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


