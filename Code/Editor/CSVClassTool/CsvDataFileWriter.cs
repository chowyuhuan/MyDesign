using UnityEngine;
using System.Collections;
using System.IO;
/// <summary>
/// Csv data file writer.
/// The csv file should be exist when write new content.
/// </summary>
public class CsvDataFileWriter : CSVDataFile{
	string _fileName = null;

	public CsvDataFileWriter(string fileName)
		:base()
	{
		_fileName = fileName;
	}

    public override void ParseCSVFor(string fileName)
    {
        string path = FixPath(fileName);
        TextAsset txt = Resources.Load(path) as TextAsset;
        ParseCSVFor (txt);
    }

	public void ClearData()
	{
		data.Clear();
		SetRow(0);
	}

	public string StringToCSVLine(string[] strvalue)
	{
		string ret = "";
		for (int i = 0; i < strvalue.Length; i++)
		{
			if (i == 0)
			{
				ret += strvalue[i];
			}
			else
			{
				ret = ret + "," + strvalue[i];
			}
		}
		return ret;
	}
	void PrintCode(string s)
	{
		if(!string.IsNullOrEmpty(s))
		{
			for(int i = 0; i< s.Length; i++)
			{
				Debug.Log("  " + s[i] + "  : " + ((int)s[i]));
			}
		}
	}
	//刷新表格
	public void FlushCacheToFile()
	{
		string pathName = "Assets/Resources/Configs/csv/" + _fileName + ".csv";
		FileStream fs = new FileStream(pathName
		                               , FileMode.Create
		                               , FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs
		                                   , System.Text.Encoding.UTF8);
		//PrintCode(StringToCSVLine(header));
		//PrintCode(StringToCSVLine(comment));
		string h = StringToCSVLine(header);
		h = h.Substring(0,h.IndexOf((char)13));
		sw.WriteLine(h);
		string c = StringToCSVLine(comment);
		c = c.Substring(0,c.IndexOf((char)13));
		sw.WriteLine(c);
		for (int i = 0; i < data.Count; i++)
		{
			sw.WriteLine(StringToCSVLine(data[i]));
		}
		sw.Close();
		fs.Close();
	}

	public void AddSpace()
	{
		if (header.Length==0)
		{
			return;
		}
		string [] newLine = new string[header.Length];
		data.Add(newLine);
		SetRow(CurrentRow + 1);

	}

	public void SetValue(string colHeader, string value)
	{
		if(colIndex.ContainsKey(colHeader))
		{
			int col = colIndex[colHeader];
			data[CurrentRow][col] = value;
		}
	}
}
