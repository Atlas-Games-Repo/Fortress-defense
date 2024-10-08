using UnityEngine;
using System;
public class GlobalValue : MonoBehaviour
{
    public static bool isFirstOpenMainMenu = true;
    public static int worldPlaying = 1;
    
    public static int levelPlaying = 1;
    public static Level.LeveType levelType = Level.LeveType.MISSION;
    //public static int finishGameAtLevel = 50;

    public static string WorldReached = "WorldReached";
    public static bool isSound = true;
    public static bool isMusic = true;

    public static void Debug(object message)
    {
        var textobj = GameObject.FindGameObjectWithTag("DebugText");
        if (textobj == null)
        {
            UnityEngine.Debug.LogError($"[{DateTime.Now}]: {message}\n");
            return;
        }
        textobj.TryGetComponent(out UnityEngine.UI.Text text);
        if (text != null)
            text.text += $"[{DateTime.Now}]: {message}\n";
        else
            UnityEngine.Debug.LogError($"[{DateTime.Now}]: {message}\n");
    }
    public static bool isNewGame
    {
        get { return PlayerPrefs.GetInt("isNewGame", 0) == 0; }
        set { PlayerPrefs.SetInt("isNewGame", value ? 0 : 1); }
    }

    public static int lastDayShowNativeAd1
    {
        get { return PlayerPrefs.GetInt("lastDayShowNativeAd1", 0); }
        set { PlayerPrefs.SetInt("lastDayShowNativeAd1", value); }
    }

    
    
    public static int lastDayShowNativeAd2
    {
        get { return PlayerPrefs.GetInt("lastDayShowNativeAd2", 0); }
        set { PlayerPrefs.SetInt("lastDayShowNativeAd2", value); }
    }
    public static string inventoryMagic
    {
        get { return PlayerPrefs.GetString("InventoryMagic", "0,1,2"); }
        set { PlayerPrefs.SetString("InventoryMagic", value); }
    }

    public static string inventoryItem
    {
        get { return PlayerPrefs.GetString("InventoryItem", "3,4,5"); }
        set { PlayerPrefs.SetString("InventoryItem", value); }
    }
    
    public static string inventoryPets
    {
        get { return PlayerPrefs.GetString("InventoryPet", "23"); }
        set { PlayerPrefs.SetString("InventoryPet", value); }
    }
    
    public static string inventoryTowers
    {
        get { return PlayerPrefs.GetString("inventoryTowers", "7"); }
        set { PlayerPrefs.SetString("inventoryTowers", value); }
    }
    
    public static int GetTutorialState(string tutorialName)
    {
        return PlayerPrefs.GetInt("tutorial :"+tutorialName, 0);
    }
    public static void SetTutorialState(string tutorialName, int value)
    {
        PlayerPrefs.SetInt("tutorial :"+tutorialName, value);
    }
    public static int lastDayShowNativeAd3
    {
        get { return PlayerPrefs.GetInt("lastDayShowNativeAd3", 0); }
        set { PlayerPrefs.SetInt("lastDayShowNativeAd3", value); }
    }

    public static int GetChosenShopItem(string itemName)
    {
        return PlayerPrefs.GetInt(itemName);
    }

    public static void IncrementChosenShopItem(string itemName)
    {
        PlayerPrefs.SetInt(itemName,GetChosenShopItem(itemName) + 1);
    }
    public static void DecrementChosenShopItem(string itemName)
    {
        PlayerPrefs.SetInt(itemName,GetChosenShopItem(itemName) - 1);
    }




    public static void LevelStar(int level, int stars)
    {
        PlayerPrefs.SetInt("LevelStars" + level, stars);
    }

    public static int LevelStar(int level)
    {
        return PlayerPrefs.GetInt("LevelStars" + level, 0);
    }

    public static bool RemoveAds
    {
        get { return PlayerPrefs.GetInt("RemoveAds", 0) == 1 ? true : false; }
        set { PlayerPrefs.SetInt("RemoveAds", value ? 1 : 0); }
    }

    public static int ItemDoubleArrow
    {
        get { return PlayerPrefs.GetInt("ItemDoubleArrow", 3); }
        set { PlayerPrefs.SetInt("ItemDoubleArrow", value); }
    }

    public static int ItemTripleArrow
    {
        get { return PlayerPrefs.GetInt("ItemTripleArrow", 1); }
        set { PlayerPrefs.SetInt("ItemTripleArrow", value); }
    }

    public static int ItemPoison
    {
        get { return PlayerPrefs.GetInt("ItemPoison", 3); }
        set { PlayerPrefs.SetInt("ItemPoison", value); }
    }

    public static void SetItemState(bool state,string itemName)
    {
        PlayerPrefs.SetInt(itemName + "activation state", state ? 1 : 0);
    }

    public static bool GetItemState(string itemName)
    {
        return PlayerPrefs.GetInt(itemName + "activation state") == 1 ? true : false;
    }
   // public static bool DoubleXp
   // {
   //     get
   //     {
   //         return PlayerPrefs.GetInt("Night") == 1;
   //     }
   //     set
   //     {
   //         PlayerPrefs.SetInt("Night", value ? 1 : 0 );
   //     }
   // }
    public static int ItemFreeze
    {
        get { return PlayerPrefs.GetInt("ItemFreeze", 3); }
        set { PlayerPrefs.SetInt("ItemFreeze", value); }
    }

   

    public static int UpgradeStrongWall
    {
        get { return PlayerPrefs.GetInt("UpgradeStrongWall", 0); }
        set { PlayerPrefs.SetInt("UpgradeStrongWall", value); }
    }
    
  //  public static string DoubleXpActivationTime
  //  {
  //      get { return PlayerPrefs.GetString("DoubleXp1HourDurationActivationTime", "1970-01-01T00:00:01"); }
  //      set { PlayerPrefs.SetString("DoubleXp1HourDurationActivationTime", value); }
  //  }

    public static string ItemOpened(string itemName)
    {
        return PlayerPrefs.GetString(itemName, "1970-01-01T00:00:01");
    }

    public static void SetItemActivationTime(string itemName, string value)
    {
        PlayerPrefs.SetString(itemName, value);
    }

    public static int ItemDuration(string itemName)
    {
        return PlayerPrefs.GetInt(itemName + "duration", 0);
    }

    public static void SetItemDuration(string itemName, int duration)
    {
        PlayerPrefs.SetInt(itemName + "duration",duration);
    }
  //  public static int DoubleXPDuration
  //  {
  //      get { return PlayerPrefs.GetInt("DoubleXPDuration", 0); }
  //      set { PlayerPrefs.SetInt("DoubleXPDuration", value); }
  //  }
  //  public static int DoubleXpActive
  //  {
  //      get { return PlayerPrefs.GetInt("DoubleXpActive", 0); }
  //      set { PlayerPrefs.SetInt("DoubleXpActive", value); }
  //  }
    public static float StrongWallExtra
    {
        get { return PlayerPrefs.GetFloat("StrongWallExtra", 0); }
        set { PlayerPrefs.SetFloat("StrongWallExtra", value); }
    }

    public static float SlowDownRate
    {
        get { return PlayerPrefs.GetFloat("SlowDownRate", 1f); }
        set { PlayerPrefs.SetFloat("SlowDownRate", value); }
    }

    public static float AttackDamageRate
    {
        get { return PlayerPrefs.GetFloat("AttackDamageRate", 1f); }
        set { PlayerPrefs.SetFloat("AttackDamageRate", value); }
    }
    public static float MainTimeDifference
    {
        get { return PlayerPrefs.GetFloat("MainTimeDifference", 0); }
        set { PlayerPrefs.SetFloat("MainTimeDifference", value); }
    }
 
    // Achievements
    public static int LevelPass
    {
        get { return PlayerPrefs.GetInt("LevelReached", 0); }
        set { PlayerPrefs.SetInt("LevelReached", value); }
    }
    public static int WorldPass
    {
        get { return PlayerPrefs.GetInt("WorldReached", 1); }
        set { PlayerPrefs.SetInt("WorldReached", value); }
    }
    public static int KillCount
    {
        get { return PlayerPrefs.GetInt("killcount", 0); }
        set { PlayerPrefs.SetInt("killcount", value); }
    }
    public static int UserGold
    {
        get { return User.Coin; }
    }
    public static int BoughtGroundUnit
    {
        get { return PlayerPrefs.GetInt("BoughtGroundUnit", 0); }
        set { PlayerPrefs.SetInt("BoughtGroundUnit", value); }
    }
    public static int PoisionUsage
    {
        get { return PlayerPrefs.GetInt("PoisionUsage", 0); }
        set { PlayerPrefs.SetInt("PoisionUsage", value); }
    }
    public static int FreezeUsage
    {
        get { return PlayerPrefs.GetInt("FreezeUsage", 0); }
        set { PlayerPrefs.SetInt("FreezeUsage", value); }
    }
    public static int LightningUsage
    {
        get { return PlayerPrefs.GetInt("LightningUsage", 0); }
        set { PlayerPrefs.SetInt("LightningUsage", value); }
    }

    public static int GameStartTimerMinutes
    {
        get
        {
            string strDateTime = PlayerPrefs.GetString("GameStartTimerMinutes", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return (DateTime.Now - DateTime.ParseExact(strDateTime, "yyyy-MM-dd HH:mm:ss", null)).Minutes;
        }
        set { PlayerPrefs.SetString("GameStartTimerMinutes", DateTime.Now.AddMinutes(value).ToString("yyyy-MM-dd HH:mm:ss")); }
    }
}

