using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CSVBuildingTools {
    public static string ConfigAssetPathBase = "Assets/Resources/";
    public static string ConfigAssetSuffix = ".csv";
    public static string CSVConfigFilePath = Application.dataPath + "/Resources/Configs/csv/";
    public static string CSVRelativeConfigFilePath = "Assets/Resources/Configs/csv/";
    public static string CSVClassesFilePath = Application.dataPath + "/Scripts/JITDLL/CSV/CSVClasses/";
    public static string CSVTemplateFilePath = Application.dataPath + "/Scripts/Editor/CsvClassTool/Template/";
	public static string DefaultTemplate = CSVTemplateFilePath + "templateFile.txt";

	//static List<string> ignoreList = new List<string>();

	[MenuItem("工具/CSV/Generate CSVClasses")]
	static void GenerateCSVClasses()
	{
        Object[] allCSV = Selection.objects;

        if (allCSV.Length == 0) 
        {
            Debug.LogError("Please Select CSV Files to convert");
            return;
        }

        foreach (Object obj in allCSV) 
		{
			Debug.Log( "Process CSV "+obj.name );
            try
            {
                string ap = AssetDatabase.GetAssetPath(obj);
                FileStream fs = new FileStream(ap, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //StreamReader sr = new StreamReader(CSVConfigFilePath + obj.name + ".csv");
                StreamReader sr = new StreamReader(fs);
                string header = sr.ReadLine();
                string type = sr.ReadLine();
                sr.Close();

                string relativePath = ap.Replace(ConfigAssetPathBase, "");
                relativePath = relativePath.Replace(ConfigAssetSuffix, "");
                relativePath = relativePath.Replace(obj.name, "");
                ProcessCSVFile(relativePath, obj.name, header, type);
            }
            catch (System.Exception ex) 
            {
                Debug.LogError(ex.Message);
                return;
            }
		}

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
	}

	static void ProcessCSVFile( string filepath, string filename, string header, string type )
	{
		//Debug.Log (string.Format ("Name {0}, Header {1}, Type {2} ", filename, header, type));

		CSVBuilder csvb = new CSVBuilder (filepath, filename, header, type);

		StreamWriter sw = new StreamWriter ( CSVClassesFilePath + csvb.ClassName + ".cs" );

		sw.WriteLine (csvb.result);
		sw.Flush ();
		sw.Close ();

		AssetDatabase.Refresh ();
	}
}
