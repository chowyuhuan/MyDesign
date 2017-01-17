using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_skill_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public int GroupId;
	public string Name;
	public string IntentEffect;
	public int Level;
	public int School;
	public string Description;
	public int WarnPatternID;
	public string TypeDescription;
	public string ConditionDescription;
	public int Type;
	public int AcquiredCondition;
	public int AcquiredCondArg1;
	public int AcquiredCondArg2;
	public int EquipedRequireStar;
	public int EquipCostGoldNum;
	public int EquipCostHonorNum;
	public int BigSuccBaseRate;
	public int BigSuccSkillLevel;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_skill_template> csv_data = new List<CSV_b_skill_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_skill_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_skill_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_skill_template item 		= new CSV_b_skill_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.GroupId = new_file.GetInt("GroupId");
			item.Name = new_file.GetString("Name");
			item.IntentEffect = new_file.GetString("IntentEffect");
			item.Level = new_file.GetInt("Level");
			item.School = new_file.GetInt("School");
			item.Description = new_file.GetString("Description");
			item.WarnPatternID = new_file.GetInt("WarnPatternID");
			item.TypeDescription = new_file.GetString("TypeDescription");
			item.ConditionDescription = new_file.GetString("ConditionDescription");
			item.Type = new_file.GetInt("Type");
			item.AcquiredCondition = new_file.GetInt("AcquiredCondition");
			item.AcquiredCondArg1 = new_file.GetInt("AcquiredCondArg1");
			item.AcquiredCondArg2 = new_file.GetInt("AcquiredCondArg2");
			item.EquipedRequireStar = new_file.GetInt("EquipedRequireStar");
			item.EquipCostGoldNum = new_file.GetInt("EquipCostGoldNum");
			item.EquipCostHonorNum = new_file.GetInt("EquipCostHonorNum");
			item.BigSuccBaseRate = new_file.GetInt("BigSuccBaseRate");
			item.BigSuccSkillLevel = new_file.GetInt("BigSuccSkillLevel");

			
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
	public static CSV_b_skill_template GetData( int index )
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
    public static CSV_b_skill_template FindData(int index)
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
	public static List<CSV_b_skill_template> FindAll(int index)
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
	public static List<CSV_b_skill_template> AllData
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


