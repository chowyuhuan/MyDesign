using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_hero_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public int School;
	public int Sex;
	public int Nationality;
	public string Prefab;
	public Vector3 WeaponPosition;
	public Vector3 WeaponRotation;
	public string AnimCtrl;
	public string UiAnimCtrl;
	public string EvoAnimCtrl;
	public string AnimEffect;
	public int NormalRangeId;
	public int NormalSkillCount;
	public int NormalSkillID1;
	public int NormalSkillID2;
	public int AttributeTemplateId;
	public int AttributeGrowthId;
	public int Star;
	public bool IsHandbookBeginer;
	public int IsRandomEvolution;
	public int EvolutionRate;
	public int EvolutionHeroId;
	public int SkillPassive;
	public int SkillID1;
	public int SkillID2;
	public int SkillID3;
	public string Name;
	public string HeadIconAtlas;
	public string HeadIcon;
	public string DisplayIconAtlas;
	public string DisplayIcon;
	public int HeroType;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_hero_template> csv_data = new List<CSV_b_hero_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_hero_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_hero_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_hero_template item 		= new CSV_b_hero_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.School = new_file.GetInt("School");
			item.Sex = new_file.GetInt("Sex");
			item.Nationality = new_file.GetInt("Nationality");
			item.Prefab = new_file.GetString("Prefab");
			item.WeaponPosition = new_file.GetVector3("WeaponPosition");
			item.WeaponRotation = new_file.GetVector3("WeaponRotation");
			item.AnimCtrl = new_file.GetString("AnimCtrl");
			item.UiAnimCtrl = new_file.GetString("UiAnimCtrl");
			item.EvoAnimCtrl = new_file.GetString("EvoAnimCtrl");
			item.AnimEffect = new_file.GetString("AnimEffect");
			item.NormalRangeId = new_file.GetInt("NormalRangeId");
			item.NormalSkillCount = new_file.GetInt("NormalSkillCount");
			item.NormalSkillID1 = new_file.GetInt("NormalSkillID1");
			item.NormalSkillID2 = new_file.GetInt("NormalSkillID2");
			item.AttributeTemplateId = new_file.GetInt("AttributeTemplateId");
			item.AttributeGrowthId = new_file.GetInt("AttributeGrowthId");
			item.Star = new_file.GetInt("Star");
			item.IsHandbookBeginer = new_file.GetBool("IsHandbookBeginer");
			item.IsRandomEvolution = new_file.GetInt("IsRandomEvolution");
			item.EvolutionRate = new_file.GetInt("EvolutionRate");
			item.EvolutionHeroId = new_file.GetInt("EvolutionHeroId");
			item.SkillPassive = new_file.GetInt("SkillPassive");
			item.SkillID1 = new_file.GetInt("SkillID1");
			item.SkillID2 = new_file.GetInt("SkillID2");
			item.SkillID3 = new_file.GetInt("SkillID3");
			item.Name = new_file.GetString("Name");
			item.HeadIconAtlas = new_file.GetString("HeadIconAtlas");
			item.HeadIcon = new_file.GetString("HeadIcon");
			item.DisplayIconAtlas = new_file.GetString("DisplayIconAtlas");
			item.DisplayIcon = new_file.GetString("DisplayIcon");
			item.HeroType = new_file.GetInt("HeroType");

			
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
	public static CSV_b_hero_template GetData( int index )
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
    public static CSV_b_hero_template FindData(int index)
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
	public static List<CSV_b_hero_template> FindAll(int index)
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
	public static List<CSV_b_hero_template> AllData
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


