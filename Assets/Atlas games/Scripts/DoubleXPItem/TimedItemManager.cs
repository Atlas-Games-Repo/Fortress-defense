using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

public class TimedItemManager : MonoBehaviour
{
    static TimeAndDateResponseModel date;
    static string extractedDate;
    static string extractedTime;
    static string dateTimeString;
    public static string itemName;
    public enum ItemDuration
    {
        Day,
        Hour
    }

    TimeAndDateResponseModel _globalDate;
    static TimeSpan globalTimeDifference;
    public static TimedItemManager self;
    public static ItemDuration duration;
    async void Awake()
    {
     //  if (self == null){
     //      self = this;
     //      DontDestroyOnLoad(this);
     //  } else {
     //      Destroy(this);
     //  }
     
     await SetDate();
    }

    public void Init(string name,ItemDuration dur)
    {
        itemName = name;
        duration = dur;
    }

    private static float newDifferenceSeconds = 0;
    private DateTime _currentGlobalDateTime;

    static double itemDuration()
    {
        return duration == ItemDuration.Day ? 24 : 1;
    }
    async Task SetDate()
    {
        _globalDate = await APIManager.instance.GetCurrentDateAndTime();
        dateTimeString = _globalDate.datetime;
        string time = dateTimeString.Substring(11, 8);
        string dateString = dateTimeString.Substring(0, 10);
        string finalDateTime = dateString + time;
        _currentGlobalDateTime = ConvertedStringToDate(finalDateTime);
        if (_currentGlobalDateTime > ConvertedStringToDate(GlobalValue.ItemOpened(itemName))
                .AddHours(itemDuration()))
        {
            GlobalValue.SetItemState(false, itemName);
        }
        else
        {
            GlobalValue.SetItemState(true, itemName);
        }

        if (GlobalValue.MainTimeDifference == 0)
        {
            GlobalValue.MainTimeDifference = (float)(_currentGlobalDateTime - DateTime.Now).TotalSeconds;
        }

        TimeSpan newDifference = _currentGlobalDateTime -
                                 DateTime.Now.AddSeconds(GlobalValue.MainTimeDifference);
        if (Math.Abs(newDifference.TotalSeconds) > 100)
        {
            newDifferenceSeconds = (float)newDifference.TotalSeconds;
        }
    }

    static DateTime ConvertedStringToDate(string insertedTime)
    {
        DateTime parsedDate;
        DateTime.TryParseExact(insertedTime,"yyyy-MM-ddTHH:mm:ss",null,DateTimeStyles.None,out parsedDate);
        return parsedDate;
    }

    void Update()
    {
        if (GlobalValue.GetItemState(itemName))
        {
            if (DateTime.Now.AddSeconds(GlobalValue.MainTimeDifference) >
                ConvertedStringToDate(GlobalValue.ItemOpened(itemName))
                    .AddHours(itemDuration()))
            {
                GlobalValue.SetItemState(false,itemName);
            }
        }
    }


   

    static string CountDownText(TimeSpan timeDifference)
    {
        int totalHours = (int)timeDifference.TotalHours; // Get the total hours as an integer
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
            totalHours, timeDifference.Minutes, timeDifference.Seconds);
    }

    public static string CounterText(string theItemName)
    {
        if (GlobalValue.GetItemState(theItemName))
        {
            DateTime dueDateTime = ConvertedStringToDate(GlobalValue.ItemOpened(theItemName))
                .AddHours(itemDuration());
            TimeSpan timeDifference = dueDateTime -
                                      DateTime.Now.AddSeconds(GlobalValue.MainTimeDifference +
                                                              newDifferenceSeconds);
            string countdownText = CountDownText(timeDifference);
            return countdownText;
        }
        else
        {
            return null;
        }
    }

    public static async void GetTime(ItemDuration duration)
    {
        date = await APIManager.instance.GetCurrentDateAndTime();
        dateTimeString = date.datetime;
        extractedDate = dateTimeString.Substring(0, 10);
        extractedTime = dateTimeString.Substring(11, 8);
        GlobalValue.SetItemActivationTime(itemName,extractedDate + extractedTime);
        GlobalValue.SetItemState(true,itemName);
        if (duration == ItemDuration.Hour)
        {
            GlobalValue.SetItemDuration(itemName,1);
        }

        if (duration == ItemDuration.Day)
        {
            GlobalValue.SetItemDuration(itemName,24);
        }
    }
}

