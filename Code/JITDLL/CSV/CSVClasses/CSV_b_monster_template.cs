using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_monster_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public string Name;
	public string HeadIconAtlas;
	public string HeadIcon;
	public float HpSpDistance;
	public float HpSpOverlayDistance;
	public int Level;
	public int Exp;
	public int MonsterType;
	public int Star;
	public int MaxSp;
	public float FlyFactor;
	public int attributeId;
	public string prefab;
	public string WeaponPrefab;
	public Vector3 WeaponPosition;
	public Vector3 WeaponRotation;
	public string AnimCtrl;
	public string AttackDefendDesList;
	public int NormalRangeId;
	public int NormalSkillCount;
	public int NormalSkillID1;
	public int NormalSkillID2;
	public int stateCount;
	public int skillGroup1;
	public float condition1;
	public int skillGroup2;
	public float condition2;
	public int skillGroup3;
	public float condition3;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_monster_template> csv_data = new List<CSV_b_monster_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_monster_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_monster_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_monster_template item 		= new CSV_b_monster_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.Name = new_file.GetString("Name");
			item.HeadIconAtlas = new_file.GetString("HeadIconAtlas");
			item.HeadIcon = new_file.GetString("HeadIcon");
			item.HpSpDistance = new_file.GetFloat("HpSpDistance");
			item.HpSpOverlayDistance = new_file.GetFloat("HpSpOverlayDistance");
			item.Level = new_file.GetInt("Level");
			item.Exp = new_file.GetInt("Exp");
			item.MonsterType = new_file.GetInt("MonsterType");
			item.Star = new_file.GetInt("Star");
			item.MaxSp = new_file.GetInt("MaxSp");
			item.FlyFactor = new_file.GetFloat("FlyFactor");
			item.attributeId = new_file.GetInt("attributeId");
			item.prefab = new_file.GetString("prefab");
			item.WeaponPrefab = new_file.GetString("WeaponPrefab");
			item.WeaponPosition = new_file.GetVector3("WeaponPosition");
			item.WeaponRotation = new_file.GetVector3("WeaponRotation");
			item.AnimCtrl = new_file.GetString("AnimCtrl");
			item.AttackDefendDesList = new_file.GetString("AttackDefendDesList");
			item.NormalRangeId = new_file.GetInt("NormalRangeId");
			item.NormalSkillCount = new_file.GetInt("NormalSkillCount");
			item.NormalSkillID1 = new_file.GetInt("NormalSkillID1");
			item.NormalSkillID2 = new_file.GetInt("NormalSkillID2");
			item.stateCount = new_file.GetInt("stateCount");
			item.skillGroup1 = new_file.GetInt("skillGroup1");
			item.condition1 = new_file.GetFloat("condition1");
			item.skillGroup2 = new_file.GetInt("skillGroup2");
			item.condition2 = new_file.GetFloat("condition2");
			item.skillGroup3 = new_file.GetInt("skillGroup3");
			item.condition3 = new_file.GetFloat("condition3");

			
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
	public static CSV_b_monster_template GetData( int index )
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
    public static CSV_b_monster_template FindData(int index)
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
	public static List<CSV_b_monster_template> FindAll(int index)
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
	public static List<CSV_b_monster_template> AllData
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


