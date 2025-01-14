using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimeChecker : MonoBehaviour
{
    static TimeAndDateResponseModel date;
    static string extractedDate;
    static string extractedTime;
    TimeAndDateResponseModel _globalDate;
    private DateTime _currentGlobalDateTime;
    private DateTime _syncedGlobalDateTime;
    [HideInInspector] public static float newDifferenceSeconds = 0;
    private bool _fetchedTime;
    public ShopItemData data;
    private List<ShopItemData.ShopItem>  _timedItems = new List<ShopItemData.ShopItem>();
    [HideInInspector]public TimedItemManager[] _items;
    public static TimeChecker Instance { get; private set; }
    async void Awake()
    {
        _items = new TimedItemManager[0];
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        while (!_fetchedTime)
        {
            APIManager apiManager = FindObjectOfType<APIManager>();
            _globalDate = await apiManager.GetCurrentDateAndTime();
            if (_globalDate.datetime != null)
            {
                _fetchedTime = true;
            }
            _currentGlobalDateTime = DateTime.Parse(_globalDate.datetime, null, DateTimeStyles.RoundtripKind);
            _syncedGlobalDateTime = _currentGlobalDateTime;
        }
        extractedDate = _syncedGlobalDateTime.ToString("yyyy-MM-dd");
        extractedTime = _syncedGlobalDateTime.ToString("HH:mm:ss");
        StartCoroutine(SyncTimeEverySecond());
        
        for (int i = 0; i < data.ShopData.Length; i++)
        {
            if (data.ShopData[i].isTimed)
            {
                _timedItems.Add(data.ShopData[i]);
                CheckItem(data.ShopData[i]);
            }
        }
    }

    IEnumerator SyncTimeEverySecond()
    {
            yield return new WaitForSeconds(1f);
            _syncedGlobalDateTime = _syncedGlobalDateTime.AddSeconds(1);
            extractedDate = _syncedGlobalDateTime.ToString("yyyy-MM-dd");
            extractedTime = _syncedGlobalDateTime.ToString("HH:mm:ss");
            SyncTimers();
            StartCoroutine(SyncTimeEverySecond());
    }

    public void SyncTimers()
    {
        _items = FindObjectsOfType<TimedItemManager>();
       if (_items.Length > 0)
       {
           for (int i = 0; i < _items.Length; i++)
           {
               if (_items[i].isInit)
               {
                   _items[i].UpdateWithTimeTick();
               }
           }
       }
    }

    public string GetCurrentDateTimeString()
    {
        return _syncedGlobalDateTime.ToString("MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
    }


    public float TimeDifference()
    {
        return newDifferenceSeconds;
    }
    
    void CheckItem(ShopItemData.ShopItem currentData)
    {
        if (GlobalValue.GetItemState(currentData.itemName))
        {
            if (ConvertedStringToDate(GetCurrentDateTimeString()) >
                ConvertedStringToDate(GlobalValue.ItemOpened(currentData.itemName))
                    .AddHours(currentData.duration == TimedItemManager.ItemDuration.Day?24:1))
            {
                GlobalValue.SetItemState(false,currentData.itemName);
            }
        }
    }
    static DateTime ConvertedStringToDate(string insertedTime)
    {
        DateTime parsedDate;
        parsedDate=   DateTime.ParseExact(insertedTime, "M/dd/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture); 
        return parsedDate;
    }

    public void InitTimedItems()
    {
        _items = FindObjectsOfType<TimedItemManager>();
        for (int i = 0; i < _items.Length; i++)
        {
            _items[i].Init(_items[i].itemName,_items[i].duration,_items[i].allTimedItems,_items[i].purchaseWithCoin);
        }
        SyncTimers();
    }
}
