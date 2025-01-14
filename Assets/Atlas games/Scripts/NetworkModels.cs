using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;

public class BaseModel
{
    public string ToJson
    {
        get { return JsonUtility.ToJson(this); }
    }

    public string ToParams
    {
        get
        {
            string param = "?";
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.ToJson);
            foreach (KeyValuePair<string, string> item in dict)
            {
                if (!string.IsNullOrEmpty(item.Value) || !APIManager.instance.IS_DEBUG)
                    param += $"{item.Key}={item.Value}&";
            }
            return param;
        }
    }

}
[Serializable]
public class AssetBundleUpdateResponse : BaseModel
{
    public string[] list = new string[0];

}

[Serializable]
public class AssetBundleGeneratorV2
{
    public AssetReferenceGameObject asset_bundle;
    public string scene_name;
    public float percent;
    public bool is_complete = false;
    public string root;
    public GameObject obj;
    public bool is_root_world = false, has_depends = false;
    public string dependence, dependence_type;
}
[Serializable]
public class AssetBundleGenerator
{
    public string name;
    public Transform root;
    public bool is_root_world = false, has_depends = false;
    public MonoBehaviour depends_type;
}
[Serializable]
public class ErrorResponse : BaseModel
{
    public string message;
    public ErrorResponse(string _message)
    {
        message = _message;
    }
}
[Serializable]
public class Data : BaseModel
{
    public int status;

}
[Serializable]
public class GemResponseModel : CommonErrorResponse
{
    public int gem, rank;
}

[Serializable]
public class data
{
    public int status;
}

[Serializable]
public class LeaderboardData: BaseModel
{
    // data for leader board that has to change based on postman.
    public int user_id;
    public string user_name;
    public string user_first_name;
    public string user_last_name;
    public string user_link;
    public string user_avatar;
    public int points;
    public int rank;
    //public string imageUrl;
    // public int rxp;
}
[Serializable]
public class LeaderBoardParams: BaseModel {
    public string game_id;
    public string type;
}
[Serializable]
public class LeaderBoardResponseModel : CommonErrorResponse
{
    public LeaderboardData[] results;
}

[Serializable]
public class TimeAndDateResponseModel : CommonErrorResponse
{
  public  string abbreviation;
  public  string client_ip ;
  public  string datetime;
  public  string day_of_week;
  public  string day_of_year;
  public  string dst;
  public  string dst_from;
  public  string dst_offset ;
  public  string dst_until;
  public  string raw_offset ;
  public  string timezone;
  public  string unixtime;
  public  string utc_datetime;
  public  string utc_offset ;
  public  string week_number;
}
[Serializable]
public class GemRequestModel : BaseModel
{
    public string amount = null;
    public string game_id = "442";
    public string date = null;
    public GemRequestModel() { }
    public GemRequestModel(string _game_id = null, string _amount = null, string _date = null)
    {
        amount = _amount;
        game_id = _game_id == null ? _game_id : "442";
        date = _date;
    }
}

[Serializable]
public class RxpRequestModel : BaseModel
{
    public string amount = null;
    public string game_id = "442";
    public string date = null;
    public RxpRequestModel(string _game_id = null, string _amount = null, string _date = null)
    {
        amount = _amount;
        game_id = _game_id == null ? "442" : _game_id;
        date = _date;
    }
}
[Serializable]
public class AuthenticationResponse : CommonErrorResponse
{
    public string token;
}
[Serializable]
public class CommonErrorResponse : BaseModel
{
    public string message;
    public string code;
    public bool result;
    public Data data;
}
[Serializable]
public class Authentication : BaseModel
{
    public string username = null;
    public string password = null;

}
[Serializable]
public class UserResponse : BaseModel
{
    public string first_name, last_name, display_name, email, username, registered_date, avatar;
    public int coin, gem, uxp, rxp, rxpTotal,xp, rank, points;
}

[Serializable]
public class UserUpdate : BaseModel
{
    public int gem = User.UserProfile.gem, uxp = User.UserProfile.uxp, coin = User.UserProfile.coin;
    public UserUpdate()
    {
    }
    public UserUpdate(int _gem = 0, int _uxp = 0, int _coin = 0)
    {
        gem = _gem;
        uxp = _uxp;
        coin = _coin;
    }

}
[Serializable]
public class Message : BaseModel
{
    public string server = "";
    public string auth = "";
}

public static class NetworkStatusError
{
    public static readonly string TOKEN_LOGIN_FAIL = "Most login first";
    public static readonly string LOGIN_FAIL = "Wrong login information";
    public static readonly string UNKNOWN_ERROR = "Something might be wrong, try again later";
    public static readonly string COULDNT_GET_UPDATES = "Didn't find any updates, try again later";
    public static readonly string SUCCESSFUL_LOGIN = "Login successful!";
    public static readonly string FAIL_LOGIN = "Something might be wrong with login, try again later";
    public static readonly string USER_UPDATE_FAIL = "We couldn't fetch some data, please be patient!";

}
public class NetworkModels : BaseModel
{

}