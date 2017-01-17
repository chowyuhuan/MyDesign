using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 游戏通用配置
/// </summary>
public class DefaultConfig
{
    static Dictionary<string, string> _configPool = new Dictionary<string, string>();
    static CSVDataFile _table = new CSVDataFile();

    static bool inited = false;

    public static void InitConfig()
    {
        _table.ParseCSVFor("b_csv/b_default_config");
        _configPool.Clear();
        for (int i = 1; i < _table.RowCount; ++i)
        {
            _table.SetRow(i);
            _configPool.Add(_table.GetString("Name"), _table.GetString("Value"));
        }

        inited = true;
    }

    public static string GetString(string key)
    {
        if (!inited)
            InitConfig();

        string value = "";
        if (_configPool.ContainsKey(key))
            _configPool.TryGetValue(key, out value);

        return value;
    }
    public static int GetInt(string key)
    {
        if (!inited)
            InitConfig();

        string value = "";

        if (_configPool.ContainsKey(key))
            _configPool.TryGetValue(key, out value);

        if (!string.IsNullOrEmpty(value))
        {
            return Convert.ToInt32(value);
        }
        return 0;
    }
    public static float GetFloat(string key)
    {
        if (!inited)
            InitConfig();

        string value = "";
        if (_configPool.ContainsKey(key))
            _configPool.TryGetValue(key, out value);
        if (!string.IsNullOrEmpty(value))
        {
            return Convert.ToSingle(value);
        }
        return 0f;
    }
    public static bool GetBool(string key)
    {
        if (!inited)
            InitConfig();

        string value = "";
        if (_configPool.ContainsKey(key))
            _configPool.TryGetValue(key, out value);
        if (!string.IsNullOrEmpty(value))
        {
            return Convert.ToBoolean(value);
        }
        return false;
    }
    public static Color GetColor(string key)
    {
        if (!inited)
            InitConfig();

        string value = "";
        if (_configPool.ContainsKey(key))
            _configPool.TryGetValue(key, out value);
        if (!string.IsNullOrEmpty(value))
        {
            string[] words = value.Split('|');
            if(words.Length == 4)
            {
                return new Color(float.Parse(words[0])/255f, float.Parse(words[1])/255f, float.Parse(words[2])/255f, float.Parse(words[3])/255f);
            }
        }
        return Color.clear;
    }

}
