using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_task_template : CSVBase
{
	#region data
	//<BeginAttribute>
	public int Id;
	public int Type;
	public int Target;
	public int TargetLevelInterface;
	public int TargetParam;
	public int TargetValue;
	public int AwardType;
	public int AwardValue;
	public string TypeName;
	public string Name;
	public string Description;
	public string IconAtlas;
	public string IconSprite;


	#endregion
	
	private static bool IsInited
	{
		get
		{
			return csv_data.Count > 0;
		}
	}
	
	private static List<CSV_b_task_template> csv_data = new List<CSV_b_task_template>();
	
	/// <summary>
    /// 初始化 
    /// </summary>
	private static void InitCSVTable()
	{
		CSVDataFile new_file = new CSVDataFile();

		TextAsset ta;
		#if !USE_ASSETBUNDLE
		ta = Resources.Load("Configs/csv/b_csv/b_task_template", typeof(TextAsset)) as TextAsset;
		#else
		ta = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>("Configs/csv/b_csv/b_task_template", AssetBundles.E_Resource_Type.Normal, true);
		#endif

		if (ta == null)
			return;

		new_file.ParseCSVFor( ta );
		
		int row_index = 2;
		
		while( new_file.SetRow( row_index ) )
		{
			CSV_b_task_template item 		= new CSV_b_task_template();

			//<BeginAssign>
			item.Id = new_file.GetInt("Id");
			item.Type = new_file.GetInt("Type");
			item.Target = new_file.GetInt("Target");
			item.TargetLevelInterface = new_file.GetInt("TargetLevelInterface");
			item.TargetParam = new_file.GetInt("TargetParam");
			item.TargetValue = new_file.GetInt("TargetValue");
			item.AwardType = new_file.GetInt("AwardType");
			item.AwardValue = new_file.GetInt("AwardValue");
			item.TypeName = new_file.GetString("TypeName");
			item.Name = new_file.GetString("Name");
			item.Description = new_file.GetString("Description");
			item.IconAtlas = new_file.GetString("IconAtlas");
			item.IconSprite = new_file.GetString("IconSprite");

			
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
	public static CSV_b_task_template GetData( int index )
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
    public static CSV_b_task_template FindData(int index)
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
	public static List<CSV_b_task_template> FindAll(int index)
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
	public static List<CSV_b_task_template> AllData
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


