using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class BasePlayerPrefs<T> : MonoBehaviour
{
    public static string PrefsName;
    public static Dictionary<Guid, T> _dict = new Dictionary<Guid, T>{};
    public static void init(){
        _dict = Dict;
    }

    public static void Add(Guid index, T value)
    {
        if (!_dict.ContainsKey(index))
        {
            _dict.Add(index, value);
            Dict = _dict;
        }
    }
    public static void Update(Guid index, T value)
    {
            bool is_exist = TryGetValue(index, out T outvalue);
            if (is_exist)
            {
                Remove(index);
                Add(index,value);
            }
    }
    public static void Remove(Guid index)
    {
            _dict.Remove(index);
            Dict = _dict;
    }
    public static string Json
    {
        get { return JsonConvert.SerializeObject(Dict, Formatting.Indented); }
    }
    public static Guid[] Keys
    {
        get
        {
            return _dict.Keys.ToArray();
        }
    }
    public static bool TryGetValue(Guid index, out T value)
    {
        _dict.TryGetValue(index, out T found_value);
        if (found_value != null) { value = found_value; return true; }
        else { value = default; return false; }
    }

    public static T[] DictArray
    {
        get
        {
            return _dict.Values.ToArray();
        }
    }
    public static Dictionary<Guid, T> Dict
    {
        get { return JsonConvert.DeserializeObject<Dictionary<Guid, T>>(PlayerPrefs.GetString(typeof(T).Name,"{}")); }
        set {
            Task.Run(async () => {
                string data = JsonConvert.SerializeObject(value);
                await UniTask.SwitchToMainThread();
                PlayerPrefs.SetString(typeof(T).Name, data);
            });
        }
    }
}
