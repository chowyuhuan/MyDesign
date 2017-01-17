using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextLocalization{
    static Dictionary<string, string> _TextDic = new Dictionary<string, string>();

    public static void Init()
    {
        _TextDic.Clear();
        //Todo:根据系统语言实现本地化,当前默认简体中文
        for(int index = 0; index < CSV_c_text_config.DateCount; ++index)
        {
            _TextDic.Add(CSV_c_text_config.AllData[index].TextId, CSV_c_text_config.AllData[index].SimplifiedChinese);
        }
        CSV_c_text_config.Recycle();
    }

    public static bool GetText(string textId, out string text)
    {
        return _TextDic.TryGetValue(textId, out text);
    }

    public static void SetTextById(Text text, string textId)
    {
        if(null != text)
        {
            string tempStr;
            if(GetText(textId, out tempStr))
            {
                text.text = tempStr;
            }
        }
    }
}
