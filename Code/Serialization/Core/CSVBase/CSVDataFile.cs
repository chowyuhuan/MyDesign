using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class CSVDataFile {
	
	protected List<string[]> data = new List<string[]>();
	
	protected string [] header;
	protected string [] comment;

    protected Dictionary<string, int> colIndex = new Dictionary<string, int>();

    private int _rowCount;	
	private int _currentRow;		//Row count exclude header
	private int _filePointer;
	private string _charArray;

	public static List<int> ExtractIntArrayFromString( string source_string )
	{
		List<int> int_array = new List<int>();
		char [] splinter = {'|'};
		string [] str_array = source_string.Split( splinter, StringSplitOptions.RemoveEmptyEntries );

		foreach( string str in str_array )
		{
			int int_val = System.Convert.ToInt32( str );

			int_array.Add( int_val );
		}

		return int_array;
	}

	public int RowCount
	{
		get
		{
            return _rowCount;
		}
	}
	
	public int ColCount
	{
		get
		{
			return colIndex.Count;
		}
	}
	
	public int ColumnCount;

	protected string FixPath(string fileName)
	{
		string path = "Configs/csv/" + fileName;
	//	if (!fileName.Contains (".txt")) {
	//		path += ".txt";
	//	}
		return path;
	}

	public virtual void ParseCSVFor(string fileName)
	{
		string path = FixPath(fileName);
#if USE_ASSETBUNDLE
		TextAsset txt = AssetBundles.AssetBundleManager.Instance.LoadSync<TextAsset>(fileName, AssetBundles.E_Resource_Type.Normal, true);
#else
        TextAsset txt = Resources.Load(path) as TextAsset;
#endif
        ParseCSVFor (txt);
	}
	
	public void ParseCSVFor( TextAsset sourceFile ){
		if( sourceFile != null )
		{
            string txt = Encoding.UTF8.GetString(sourceFile.bytes);
            _charArray = new string(txt.ToCharArray());
			
			if( _charArray != null )
			{
				char [] spliter = {'\n'};
				string [] _lines = _charArray.Split( spliter );
				
				if(_lines.Length > 1)
				{
					char [] dot_spliter = {','};
					header = _lines[0].Split( dot_spliter );
					
					int index = 0;
					foreach( string s in header )
					{
						colIndex.Add( s.Trim(), index++ );	// create col index
					}
				}

				if(_lines.Length > 2)
				{
					char [] dot_spliter = {','};
					comment = _lines[1].Split(dot_spliter);
				}
				
				if( _lines.Length >= 3 ){
					for( int i = 0; i < _lines.Length ; i++ )
					{
						char [] dot_spliter = {','};
						string [] new_line = _lines[i].Split( dot_spliter );
						if( new_line.Length >= 2 )
						{
							data.Add( new_line );			// add row data
                            _rowCount++;
						}
					}
				}
			}
		}
	}
	
	public bool SetRow( int index ){
        if (index >= 0 && index < data.Count)
		{
            _currentRow = index;
			return true;
		}
		else
		{
			_currentRow = 0;
			return false;
		}
	}
	
	public int GetInt(string col_title){
		int ColID = 0;
		
		if( colIndex.TryGetValue( col_title, out ColID )){
			try
            {
                if (string.IsNullOrEmpty(data[_currentRow][ColID]))
                {
                    return -1;
                }
				 return Convert.ToInt32( data[_currentRow][ColID] );
			}
			catch
			{
                return -1;
			}
			
		}
		return -1;
	}

    public Vector3 GetVector3(string col_title)
    {
        int ColID = 0;
        string str;
        if (colIndex.TryGetValue(col_title, out ColID))
        {
            str = data[_currentRow][ColID].ToString().TrimEnd().Trim('(', ')');;
            if (!string.IsNullOrEmpty(str))
            {
                string[] words = str.Split('|');
                if (words.Length == 3)
                {
                    return new Vector3(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
                }
            }
        }

        return Vector3.one;
    }

	public string GetString(string col_title){
		int ColID = 0;
		
		if( colIndex.TryGetValue( col_title, out ColID )){
            return data[_currentRow][ColID].ToString().TrimEnd().Replace("\\n", "\n");
		}
		return "";
	}
	
	public double GetDouble( string col_title ){
		int ColID = 0;
		double ret_dbl = 0;
		
		if( colIndex.TryGetValue( col_title, out ColID )){
			try{
				ret_dbl = Convert.ToDouble( data[_currentRow][ColID] );
			}
			catch
			{
			}
			
			return ret_dbl;
		}
		return 0.0;
	}
	
	public float GetFloat( string col_title ){
		int ColID = 0;
		float ret_dbl = 0;
		
		if( colIndex.TryGetValue( col_title, out ColID )){
			try{
				ret_dbl = Convert.ToSingle( data[_currentRow][ColID] );
			}
			catch( Exception e)
			{
				Debug.LogError( e.Message +" Data is" +data[_currentRow][ColID] + " Col " + col_title );
			}
			
			return ret_dbl;
		}
		return 0.0f;
	}
	
	public bool GetBool( string col_title ){

		int ColID = 0;
		int ret_int = 0;
		bool ret_bool = false;
		
		if( colIndex.TryGetValue( col_title, out ColID )){
            if (data[_currentRow][ColID].ToUpper() == "TRUE")
            {
                ret_bool = true;
            }
            else
            {
                try
                {
                    ret_int = Convert.ToInt32(data[_currentRow][ColID]);
                    ret_bool = Convert.ToBoolean(ret_int);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                finally
                {

                }
            }			
			return ret_bool;
		}
		return false;
	}

    public string[] GetRow()
    {
        return data[_currentRow];
    }

    public List<string[]> GetAllData()
    {
        return data;
    }
	
	public int CurrentRow
	{
		get{
			return _currentRow;
		}
	}
	
	public int CurrentRowID
	{	get
		{
			return Convert.ToInt32( data[_currentRow][0] );
		}
	}
	
	int FileLength(){ 
		if( _charArray != null )
		{
			return _charArray.Length; 
		}
		return 0;
	}

}
