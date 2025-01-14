using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementType
{
    NORMAL,
    RARE,
    EPIC,
    LEGENDARY
}
public enum ScheduleType
{
    DAYLY,
    WEEKLY,
    ONETIME,
    PERMENENT,
}
public enum ScheduleStatus
{
    PENDING,
    DONE,
    EXPIRED
}
[Serializable]
public class AchievementModel
{
    public Guid _id = Guid.Empty;
    public Guid Schedul_id = Guid.Empty;
    public string name;
    public string description;
    public int reward;
    public int startPoint;
    public int checkpoint;
    public string fieldName;
    public bool isActive = true;
    public bool isOneTime = false;
    public AchievementType type = AchievementType.NORMAL;
    public TrophyStatus status = TrophyStatus.UNKNOWN;

}
[Serializable]
public class AchievementScheduleModel
{
    public Guid _id = Guid.NewGuid();
    public string name;
    public DateTime ExpireDate = DateTime.Now;
    public int DurationMinutes = 10;
    public int NumberOfMissions = 4;
    public ScheduleType type = ScheduleType.DAYLY;
    public ScheduleStatus status = ScheduleStatus.PENDING;

    public AchievementScheduleModel()
    {
        ExpireDate = DateTime.Now.AddMinutes(DurationMinutes);
    }
    public AchievementScheduleModel(ScheduleType _type)
    {
        DurationMinutes = _type switch
        {
            ScheduleType.DAYLY => 1_440,
            ScheduleType.ONETIME => DurationMinutes * UnityEngine.Random.Range(1, 144),
            ScheduleType.WEEKLY => 10_080,
            _ => 1_440,
        };
        type = _type;
        ExpireDate = DateTime.Now.AddMinutes(DurationMinutes);
    }
    public AchievementScheduleModel(ScheduleType _type, int numberOfMissions)
    {
        DurationMinutes = _type switch
        {
            ScheduleType.DAYLY => 1_440,
            ScheduleType.ONETIME => DurationMinutes * UnityEngine.Random.Range(1, 144),
            ScheduleType.WEEKLY => 10_080,
            _ => 1_440,
        };
        type = _type;
        NumberOfMissions = numberOfMissions;
        ExpireDate = DateTime.Now.AddMinutes(DurationMinutes);
    }
    public AchievementScheduleModel(ScheduleType _type, int numberOfMissions, string _name)
    {
        name = _name;
        DurationMinutes = _type switch
        {
            ScheduleType.DAYLY => 1_440,
            ScheduleType.ONETIME => DurationMinutes * UnityEngine.Random.Range(1, 144),
            ScheduleType.WEEKLY => 10_080,
            ScheduleType.PERMENENT => 525_600,
            _ => 1_440,
        };
        type = _type;
        NumberOfMissions = numberOfMissions;
        ExpireDate = DateTime.Now.AddMinutes(DurationMinutes);
    }
    public AchievementScheduleModel(ScheduleType _type, int numberOfMissions, string _name, int duration)
    {
        name = _name;
        DurationMinutes = duration;
        type = _type;
        NumberOfMissions = numberOfMissions;
        ExpireDate = DateTime.Now.AddMinutes(DurationMinutes);
    }
}
[Serializable]
public class AchievementUpdateModel : BaseModel
{
    public string id;
    public int status;
    public AchievementUpdateModel()
    {
        id = "0";
        status = 0;
    }
    public AchievementUpdateModel(string _id, int _status)
    {
        id = _id;
        status = _status;
    }

}
