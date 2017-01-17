using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

interface AlarmTimer
{
    int GetNextCallTime(DateTime now);

    bool IsSameTime(AlarmTimer timer);
}

class AlarmInfo : AlarmTimer
{
    AlarmTimer timer;

    List<CallbackInfo> calls = new List<CallbackInfo>();

    public AlarmInfo(AlarmTimer timer)
    {
        this.timer = timer;
    }

    public List<CallbackInfo> GetCallbackInfoList()
    {
        return calls;
    }

    public void AddCallbackInfo(CallbackInfo call)
    {
        calls.Add(call);
    }

    public void RemoveCallbackInfo(CallbackInfo call)
    {
        calls.Remove(call);
    }

    public int GetNextCallTime(DateTime now)
    {
        return timer.GetNextCallTime(now);
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        return this.timer.IsSameTime(timer);
    }

    public bool HasCallbackInfo()
    {
        return calls.Count > 0;
    }
}

class DateAlarmTimer : AlarmTimer
{
    int year;
    int month;
    int day;
    int hour;
    int minute;
    int second;

    public DateAlarmTimer(int year, int month, int day, int hour, int minute, int second)
    {
        this.year = year;
        this.month = month;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    public int GetNextCallTime(DateTime now)
    {
        System.DateTime to = new System.DateTime(year, month, day, hour, minute, second);

        if (to > now)
        {
            return Mathf.CeilToInt((float)(to - now).TotalSeconds);
        }
        else
        {
            return 0;
        }
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        DateAlarmTimer other = timer as DateAlarmTimer;
        if (other != null)
        {
            return year == other.year && month == other.month && day == other.day && hour == other.hour && minute == other.minute && second == other.second;
        }

        return false;
    }
}

class YearlyAlarmTimer : AlarmTimer
{
    int month;
    int day;
    int hour;
    int minute;
    int second;

    public YearlyAlarmTimer(int month, int day, int hour, int minute, int second)
    {
        this.month = month;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    public int GetNextCallTime(DateTime now)
    {
        System.DateTime to = new System.DateTime(now.Year, month, day, hour, minute, second);

        if (to > now)
        {
            return Mathf.CeilToInt((float)(to - now).TotalSeconds);
        }
        else
        {
            return Mathf.CeilToInt((float)(to.AddYears(1) - now).TotalSeconds);
        }
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        YearlyAlarmTimer other = timer as YearlyAlarmTimer;
        if (other != null)
        {
            return month == other.month && day == other.day && hour == other.hour && minute == other.minute && second == other.second;
        }

        return false;
    }
}

class MonthlyAlarmTimer : AlarmTimer
{
    int day;
    int hour;
    int minute;
    int second;

    public MonthlyAlarmTimer(int day, int hour, int minute, int second)
    {
        this.day = day;
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    public int GetNextCallTime(DateTime now)
    {
        System.DateTime to = new System.DateTime(now.Year, now.Month, day, hour, minute, second);

        if (to > now)
        {
            return Mathf.CeilToInt((float)(to - now).TotalSeconds);
        }
        else
        {
            return Mathf.CeilToInt((float)(to.AddMonths(1) - now).TotalSeconds);
        }
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        MonthlyAlarmTimer other = timer as MonthlyAlarmTimer;
        if (other != null)
        {
            return day == other.day && hour == other.hour && minute == other.minute && second == other.second;
        }

        return false;
    }
}

class WeeklyAlarmTimer : AlarmTimer
{
    int dayOfWeek;
    int hour;
    int minute;
    int second;

    public WeeklyAlarmTimer(int dayOfWeek, int hour, int minute, int second)
    {
        this.dayOfWeek = dayOfWeek;
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    public int GetNextCallTime(DateTime now)
    {
        System.DateTime to = new System.DateTime(now.Year, now.Month, now.Day, hour, minute, second);
        to.AddDays(dayOfWeek % 7 - (int)now.DayOfWeek);

        if (to > now)
        {
            return Mathf.CeilToInt((float)(to - now).TotalSeconds);
        }
        else
        {
            return Mathf.CeilToInt((float)(to.AddDays(7) - now).TotalSeconds);
        }
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        WeeklyAlarmTimer other = timer as WeeklyAlarmTimer;
        if (other != null)
        {
            return dayOfWeek == other.dayOfWeek && hour == other.hour && minute == other.minute && second == other.second;
        }

        return false;
    }
}

class DailyAlarmTimer : AlarmTimer
{
    int hour;
    int minute;
    int second;

    public DailyAlarmTimer(int hour, int minute, int second)
    {
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    public int GetNextCallTime(DateTime now)
    {
        System.DateTime to = new System.DateTime(now.Year, now.Month, now.Day, hour, minute, second);

        if (to > now)
        {
            return Mathf.CeilToInt((float)(to - now).TotalSeconds);
        }
        else
        {
            return Mathf.CeilToInt((float)(to.AddDays(1) - now).TotalSeconds);
        }
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        DailyAlarmTimer other = timer as DailyAlarmTimer;
        if (other != null)
        {
            return hour == other.hour && minute == other.minute && second == other.second;
        }

        return false;
    }
}

class HourlyAlarmTimer : AlarmTimer
{
    int minute;
    int second;

    public HourlyAlarmTimer(int minute, int second)
    {
        this.minute = minute;
        this.second = second;
    }

    public int GetNextCallTime(DateTime now)
    {
        System.DateTime to = new System.DateTime(now.Year, now.Month, now.Day, now.Hour, minute, second);

        if (to > now)
        {
            return Mathf.CeilToInt((float)(to - now).TotalSeconds);
        }
        else
        {
            return Mathf.CeilToInt((float)(to.AddHours(1) - now).TotalSeconds);
        }
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        HourlyAlarmTimer other = timer as HourlyAlarmTimer;
        if (other != null)
        {
            return minute == other.minute && second == other.second;
        }

        return false;
    }
}

class MinutelyAlarmTimer : AlarmTimer
{
    int second;

    public MinutelyAlarmTimer(int second)
    {
        this.second = second;
    }

    public int GetNextCallTime(DateTime now)
    {
        System.DateTime to = new System.DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, second);

        if (to > now)
        {
            return Mathf.CeilToInt((float)(to - now).TotalSeconds);
        }
        else
        {
            return Mathf.CeilToInt((float)(to.AddMinutes(1) - now).TotalSeconds);
        }
    }

    public bool IsSameTime(AlarmTimer timer)
    {
        MinutelyAlarmTimer other = timer as MinutelyAlarmTimer;
        if (other != null)
        {
            return second == other.second;
        }

        return false;
    }
}

class CallbackInfo
{
    public bool oneShot;
    public Action callback;

    public CallbackInfo(Action callback, bool oneShot)
    {
        this.callback = callback;
        this.oneShot = oneShot;
    }

    public static void Execute(List<CallbackInfo> calls)
    {
        for (int i = calls.Count - 1; i >= 0; --i)
        {
            if (calls[i].callback != null)
            {
                calls[i].callback();
            }

            if (calls[i].oneShot)
            {
                calls.RemoveAt(i);
            }
        }
    }

    public static void RemoveCallback(List<CallbackInfo> calls, Action callback, bool oneShot)
    {
        for (int i = 0; i < calls.Count; ++i)
        {
            if (calls[i].callback == callback && calls[i].oneShot == oneShot)
            {
                calls.RemoveAt(i);
                return;
            }
        }
    }
}

public class AlarmClock : MonoBehaviour
{
    static AlarmClock instance;
    public static AlarmClock Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AlarmClock");
                GameObject.DontDestroyOnLoad(go);

                instance = go.AddComponent<AlarmClock>();
            }

            return instance;
        }
    }

    List<AlarmInfo> alarmList = new List<AlarmInfo>();

    public delegate DateTime NowTimeFunction();

    NowTimeFunction OnNowTimeFunction;

	// Use this for initialization
	void Start () {
	
	}

    public void ClearAlarmInfo()
    {
        alarmList.Clear();
    }

    AlarmInfo GetAlarmInfoByTimer(AlarmTimer timer)
    {
        for (int i = 0; i < alarmList.Count; ++i)
        {
            if (alarmList[i].IsSameTime(timer))
            {
                return alarmList[i];
            }
        }

        return null;
    }

    void AddAlarmInfo(AlarmInfo alarmInfo)
    {
        alarmList.Add(alarmInfo);
    }

    void RemoveAlarmInfo(AlarmInfo alarmInfo)
    {
        alarmList.Remove(alarmInfo);
    }

    public void RegisterNowTimeFunction(NowTimeFunction func)
    {
        OnNowTimeFunction = func;
    }

    public void UnregisterNowTimeFunction()
    {
        OnNowTimeFunction = null;
    }

    DateTime GetNowTime()
    {
        if (OnNowTimeFunction != null)
        {
            return OnNowTimeFunction();
        }

        return DateTime.Now;
    }

    IEnumerator AlarmCoroutine(AlarmTimer timer, float second)
    {
        yield return Yielders.GetWaitForRtSeconds(second);

        AlarmInfo alarmInfo = GetAlarmInfoByTimer(timer);
        List<CallbackInfo> calls = alarmInfo.GetCallbackInfoList();
        CallbackInfo.Execute(calls);

        int needTime = alarmInfo.GetNextCallTime(GetNowTime());
        if (alarmInfo.HasCallbackInfo() && needTime > 0)
        {
            StartCoroutine(AlarmCoroutine(timer, needTime));
        }
        else
        {
            RemoveAlarmInfo(alarmInfo);
        }

        yield return null;
    }

    void AddAlarm(AlarmTimer timer, Action callback, bool oneShot)
    {
        CallbackInfo callbackInfo = new CallbackInfo(callback, oneShot);

        int needTime = timer.GetNextCallTime(GetNowTime());
        if (needTime > 0)
        {
            AlarmInfo alarmInfo = GetAlarmInfoByTimer(timer);
            if (alarmInfo == null)
            {
                alarmInfo = new AlarmInfo(timer);
                AddAlarmInfo(alarmInfo);

                StartCoroutine(AlarmCoroutine(timer, needTime));
            }

            alarmInfo.AddCallbackInfo(callbackInfo);
        }
    }

    void RemoveAlarm(AlarmTimer timer, Action callback, bool oneShot)
    {
        AlarmInfo alarmInfo = GetAlarmInfoByTimer(timer);
        if (alarmInfo != null)
        {
            List<CallbackInfo> calls = alarmInfo.GetCallbackInfoList();
            CallbackInfo.RemoveCallback(calls, callback, oneShot);
            if (alarmInfo.HasCallbackInfo()) RemoveAlarmInfo(alarmInfo);
        }
    }

    public static DateTime GetDateTime(uint timestamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        TimeSpan toNow = new TimeSpan((long)timestamp * 10000000);
        return dtStart.Add(toNow);
    }

    public void AddDateAlarm(int year, int month, int day, int hour, int minute, int second, Action callback)
    {
        AlarmTimer timer = new DateAlarmTimer(year, month, day, hour, minute, second);
        AddAlarm(timer, callback, true);
    }

    public void AddDateAlarm(uint timestamp, Action callback)
    {
        DateTime dateTime = GetDateTime(timestamp);
        AlarmTimer timer = new DateAlarmTimer(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        AddAlarm(timer, callback, true);
    }

    public void RemoveDateAlarm(int year, int month, int day, int hour, int minute, int second, Action callback)
    {
        AlarmTimer timer = new DateAlarmTimer(year, month, day, hour, minute, second);
        RemoveAlarm(timer, callback, true);
    }

    public void RemoveDateAlarm(uint timestamp, Action callback)
    {
        DateTime dateTime = GetDateTime(timestamp);
        AlarmTimer timer = new DateAlarmTimer(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        RemoveAlarm(timer, callback, true);
    }

    public void AddYearlyAlarm(int month, int day, int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new YearlyAlarmTimer(month, day, hour, minute, second);
        AddAlarm(timer, callback, oneShot);
    }

    public void RemoveYearlyAlarm(int month, int day, int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new YearlyAlarmTimer(month, day, hour, minute, second);
        RemoveAlarm(timer, callback, oneShot);
    }

    public void AddMonthlyAlarm(int day, int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new MonthlyAlarmTimer(day, hour, minute, second);
        AddAlarm(timer, callback, oneShot);
    }

    public void RemoveMonthlyAlarm(int day, int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new MonthlyAlarmTimer(day, hour, minute, second);
        RemoveAlarm(timer, callback, oneShot);
    }

    public void AddWeeklyAlarm(int dayOfWeek, int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new WeeklyAlarmTimer(dayOfWeek, hour, minute, second);
        AddAlarm(timer, callback, oneShot);
    }

    public void RemoveWeeklyAlarm(int dayOfWeek, int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new WeeklyAlarmTimer(dayOfWeek, hour, minute, second);
        RemoveAlarm(timer, callback, oneShot);
    }

    public void AddDailyAlarm(int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new DailyAlarmTimer(hour, minute, second);
        AddAlarm(timer, callback, oneShot);
    }

    public void RemoveDailyAlarm(int hour, int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new DailyAlarmTimer(hour, minute, second);
        RemoveAlarm(timer, callback, oneShot);
    }

    public void AddHourlyAlarm(int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new HourlyAlarmTimer(minute, second);
        AddAlarm(timer, callback, oneShot);
    }

    public void RemoveHourlyAlarm(int minute, int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new HourlyAlarmTimer(minute, second);
        RemoveAlarm(timer, callback, oneShot);
    }

    public void AddMinutelyAlarm(int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new MinutelyAlarmTimer(second);
        AddAlarm(timer, callback, oneShot);
    }

    public void RemoveMinutelyAlarm(int second, Action callback, bool oneShot = false)
    {
        AlarmTimer timer = new MinutelyAlarmTimer(second);
        RemoveAlarm(timer, callback, oneShot);
    }
}
