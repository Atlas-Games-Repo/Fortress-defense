using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

[Serializable]
public class _Trophy
{
    public string name = "null";
    public string _id = "null";
    public string imageURL = null;
    public string details = "null";
    public int price = 0;
    [ReadOnly] public TrophyStatus status = TrophyStatus.UNKNOWN;
    // [ReadOnly] public bool is_served = false;

}
public enum TrophyStatus
{
    UNKNOWN,
    PENDING,
    ACHIEVED,
    RECIVED,
    PAYED,
}

public class Trophy : MonoBehaviour
{
    public static Trophy self;
    public float Start_delay = 1f;
    public Sprite DefaultSprite;
    // public Dictionary<string, _Trophy> Trophies = new Dictionary<string, _Trophy>();
    public _Trophy[] _Trophies = new _Trophy[0];
    void Awake()
    {
        self = this;
    }
    // Start is called before the first frame update
    public void Start()
    {
        // StartCoroutine(StartEnum());
    }

    // Update is called once per frame
    IEnumerator StartEnum()
    {
        yield return new WaitForSeconds(Start_delay);
        foreach (_Trophy item in _Trophies)
        {
            bool is_exists = TryGetTrophy(item._id, out _Trophy trophy);
            if (!is_exists)
                Add = item;
        }
    }

    public static string STATUS_UP
    {
        set
        {
            bool is_exist = TryGetTrophy(value, out _Trophy trophy);
            if (is_exist)
            {
                if ((int)trophy.status < (int)TrophyStatus.RECIVED)
                    trophy.status++;
                Update = trophy;
            }
        }
    }
    public static void SetStatus(string id, TrophyStatus status)
    {
        bool is_exist = TryGetTrophy(id, out _Trophy trophy);
        if (is_exist)
        {
            trophy.status = status;
            Update = trophy;
        }
    }
    public static _Trophy Add
    {
        set
        {
            Dictionary<string, _Trophy> dict = Trophies;
            if (!dict.ContainsKey(value._id))
            {
                dict.Add(value._id, value);
                Trophies = dict;
            }
        }
    }
    public static _Trophy Update
    {
        set
        {
            bool is_exist = TryGetTrophy(value._id, out _Trophy trophy);
            if (is_exist)
            {
                Remove = value._id;
                Add = value;
            }
        }
    }
    public static string Remove
    {
        set
        {
            var dict = Trophies;
            dict.Remove(value);
            Trophies = dict;
        }
    }
    public static string Json
    {
        get { return JsonConvert.SerializeObject(Trophies, Formatting.Indented); }
    }
    public static Dictionary<string, _Trophy> Trophies
    {
        get { return JsonConvert.DeserializeObject<Dictionary<string, _Trophy>>(PlayerPrefs.GetString("Trophies", "{}")); }
        set { PlayerPrefs.SetString("Trophies", JsonConvert.SerializeObject(value)); }
    }
    public static string[] Keys
    {
        get
        {
            Dictionary<string, _Trophy> dict = Trophies;
            return dict.Keys.ToArray();
        }
    }
    public static bool TryGetTrophy(string key, out _Trophy value)
    {
        Dictionary<string, _Trophy> dict = Trophies;
        bool is_exist = dict.TryGetValue(key, out _Trophy trophy);
        if (trophy != null) { value = trophy; return true; }
        else { value = null; return false; }
    }
    public static _Trophy[] TrophiesArray
    {
        get
        {
            Dictionary<string, _Trophy> dict = Trophies;
            return dict.Values.ToArray();
        }
    }


}


