using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

public class AchievementTasksV2 : BasePlayerPrefs<AchievementModel>
{
    public static AchievementTasksV2 self;
    public float InitialDelaySeconds = 5;
    public float ListenerTickSeconds = 2;
    public AchievementEventsV2[] achievements;
    private Coroutine _dispatcher;

    void Awake()
    {
        if (self == null)
        {
            self = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (_dispatcher == null)
            _dispatcher = StartCoroutine(Listener());

    }
    IEnumerator Listener()
    {
        achievements = new AchievementEventsV2[DictArray.Length];
        for (int i = 0; i < achievements.Length; i++)
        {
            achievements[i] = new AchievementEventsV2(DictArray[i]);
        }
        // AddNewAchievements(models);
        yield return new WaitForSeconds(InitialDelaySeconds);
        while (true)
        {
            foreach (AchievementEventsV2 item in achievements)
            {
                _ = item.IsPassed;
            }
            yield return new WaitForSeconds(ListenerTickSeconds);
        }
    }
    public void AddNewAchievements(AchievementModel[] models)
    {
        for (int i = 0; i < models.Length; i++)
        {
            Add(models[i]._id, models[i]);
        }
        achievements = new AchievementEventsV2[DictArray.Length];
        for (int i = 0; i < achievements.Length; i++)
        {
            achievements[i] = new AchievementEventsV2(DictArray[i]);
        }
    }
    public void TryGetEvent(Guid index, out AchievementEventsV2 Event)
    {
        foreach (AchievementEventsV2 item in achievements)
        {
            if (item.model._id == index)
            {
                Event = item;
                return;
            }
        }
        Event = null;
    }
    void OnApplicationQuit()
    {
        if (_dispatcher != null)
            StopCoroutine(_dispatcher);
    }

}
