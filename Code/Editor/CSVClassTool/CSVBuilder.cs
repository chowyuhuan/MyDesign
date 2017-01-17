using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class CSVBuilder
{
	public const string c_insert_attribute = "//<BeginAttribute>";
	public const string c_insert_assignment = "//<BeginAssign>";
	public const string c_class_name = "{classname}";
    public const string c_file_path = "{filepath}";
	public const string c_file_name = "{filename}";
	public const string c_index_name = "{id_string}";
    public const string c_id_type = "{id_type}";
	
	public string ClassName;
    public string FilePath;
    public string FileName;
	public string HeaderRow;
	public string TypeRow;
	public string IndexString;
    public string idType = "int";
	
	public List<string> headerList = new List<string>();
	public List<string> typeList = new List<string>();
	
	public Dictionary<string, string> all_attributes = new Dictionary<string, string> ();
	
	public string templateString;
	public string result;
	
	public void BuildDefaultTemplate()
	{
		StreamReader sr = new StreamReader (CSVBuildingTools.DefaultTemplate);
		templateString = sr.ReadToEnd();
		sr.Close();
		
		int insert_attribute = templateString.IndexOf (c_insert_attribute) + c_insert_attribute.Length;
		
		string attribute_string = "\n";
		string assign_string = "\n";
		
		foreach (KeyValuePair<string,string> kvp in all_attributes) 
		{
			attribute_string += string.Format( "\tpublic {0} {1};\n",kvp.Value,kvp.Key );
			switch( kvp.Value )
			{
                case "bool":
                    assign_string += string.Format("\t\t\titem.{0} = new_file.GetBool(\"{1}\");\n", kvp.Key, kvp.Key);
                    break;
                case "int":
                    assign_string += string.Format("\t\t\titem.{0} = new_file.GetInt(\"{1}\");\n", kvp.Key, kvp.Key);
                    break;
                case "string":
                    assign_string += string.Format("\t\t\titem.{0} = new_file.GetString(\"{1}\");\n", kvp.Key, kvp.Key);
                    break;
                case "float":
                    assign_string += string.Format("\t\t\titem.{0} = new_file.GetFloat(\"{1}\");\n", kvp.Key, kvp.Key);
                    break;
                case "Vector3":
                    assign_string += string.Format("\t\t\titem.{0} = new_file.GetVector3(\"{1}\");\n", kvp.Key, kvp.Key);
                    break;
			}
		}

		string fp = templateString.Substring (0, insert_attribute);
		string lp = templateString.Substring (insert_attribute);
		
		result = fp + attribute_string + lp;

		//assignment
		int insert_assignment = result.IndexOf ( c_insert_assignment ) + c_insert_assignment.Length;
		fp = result.Substring (0, insert_assignment);
		lp = result.Substring (insert_assignment);

		result = fp + assign_string + lp;

		//replace class name and file name
		while (result.IndexOf (c_class_name) >= 0) 
		{
			result = result.Replace (c_class_name, ClassName); 
		}

        while (result.IndexOf(c_file_path) >= 0)
        {
            result = result.Replace(c_file_path, FilePath);
        }

		while (result.IndexOf( c_file_name ) >= 0) 
		{
            result = result.Replace(c_file_name, FileName); 
		}

		while ( result.IndexOf( c_index_name ) >= 0 ) 
		{
			result = result.Replace( c_index_name, IndexString );
		}

        result += "\n";
	}
	
	public CSVBuilder( string file_path, string file_name, string header, string type )
	{
        FilePath = file_path;
        FileName = file_name;
        ClassName = "CSV_" + file_name;
		HeaderRow = header;
		TypeRow = type;
		
		if (ProcessDataFirstPass())
		    BuildDefaultTemplate();

        //post process
        while (result.IndexOf(c_id_type) >= 0)
        {
            Debug.Log("ID TYPE " + c_id_type);
            result = result.Replace(c_id_type, idType);
        }
	}

	public bool ProcessDataFirstPass()
	{
		Debug.Log ("----------Building for " + ClassName);
		
		if (string.IsNullOrEmpty(ClassName)) 
		{ 
			Debug.LogError( "CSVFile is invalidate!!! " + ClassName );
			return false;
		}
		
		char [] splinter = {','};
		headerList.AddRange (HeaderRow.Split (splinter, System.StringSplitOptions.RemoveEmptyEntries));
		typeList.AddRange (TypeRow.Split (splinter, System.StringSplitOptions.RemoveEmptyEntries));
		
		if (headerList.Count < 2 || typeList.Count < 2 || headerList.Count != typeList.Count) 
		{
			Debug.LogError( "Column is at least 2, and head must equal to type`s length" );
			return false;
		} 
		
        IndexString = headerList[0];
        string typeString;
		for (int i = 0; i < headerList.Count; i++) 
		{


            typeString = string.Empty;
            if (typeList[i].ToLower().Contains("int"))
            {
                typeString = "int";
            }
            else if (typeList[i].ToLower().Contains("bool"))
            {
                typeString = "bool";
            }
            else if (typeList[i].ToLower().Contains("string"))
            {
                typeString = "string";
            }
            else if (typeList[i].ToLower().Contains("float"))
            {
                typeString = "float";
            }
            else if (typeList[i].ToLower().Contains("vector3"))
            {
                typeString = "Vector3";
            }

            if (i == 0)
            {
                idType = typeString;
            }

            if (string.IsNullOrEmpty(typeString))
            {
                Debug.LogError("Faile to parse type!");
                return false;
            }

			try
			{
                all_attributes.Add(headerList[i], typeString);
			}
			catch( System.Exception ex )
			{
				Debug.Log(ex.Message);
				Debug.Log( "Key Confilcted no columns can have same Header" );
				return false;
			}
		}

        return true;
	} 
	
	public string Result;
}
