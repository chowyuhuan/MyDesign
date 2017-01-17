using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class TimeFormater
{
    /// <summary>
    /// 传入时间，返回字符串 XX：XX：XX 
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns></returns>
    public static string Format(uint time)
    {
        uint hour = (time / ConstDefine.SECOND_PER_HOUR);
        uint min = ((time - hour * ConstDefine.SECOND_PER_HOUR) / ConstDefine.SECOND_PER_MINUTE);
        uint sec = (time - hour * ConstDefine.SECOND_PER_HOUR - min * ConstDefine.SECOND_PER_MINUTE);

        return string.Format("{0}:{1}:{2}", hour.ToString("d2"), min.ToString("d2"), sec.ToString("d2"));
    }

    public static string FormatShrinkIfNeeded(uint time)
    {
        uint hour = (time / ConstDefine.SECOND_PER_HOUR);
        uint min = ((time - hour * ConstDefine.SECOND_PER_HOUR) / ConstDefine.SECOND_PER_MINUTE);
        uint sec = (time - hour * ConstDefine.SECOND_PER_HOUR - min * ConstDefine.SECOND_PER_MINUTE);

        if(hour > 0)
        {
            return string.Format("{0}:{1}:{2}", hour.ToString("d2"), min.ToString("d2"), sec.ToString("d2"));
        }
        else if(min > 0)
        {
            return string.Format("{0}:{1}", min.ToString("d2"), sec.ToString("d2"));
        }
        else
        {
            return sec.ToString("d2"); 
        }
    }

    public static DateTime GetDateTime(uint timestamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        TimeSpan toNow = new TimeSpan((long)timestamp * 10000000);
        return dtStart.Add(toNow);
    }

    public static bool IsInSameDay(uint ta, uint tb)
    {
        DateTime dateA = GetDateTime(ta);
        DateTime dateB = GetDateTime(tb);

        if (dateA.Year == dateB.Year && dateA.DayOfYear == dateB.DayOfYear)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
