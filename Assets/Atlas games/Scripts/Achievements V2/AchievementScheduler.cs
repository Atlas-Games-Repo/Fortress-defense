using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

public class AchievementScheduler : BasePlayerPrefs<AchievementScheduleModel>
{
    public AchievementsData data;

    public float InitialDelaySeconds;
    public float ListenerTickSeconds;
    public int HoldExpiredScheduleMinutes;
    public AnimationCurve Probability;
    public AchievementScheduleModel[] Schedules;
    private Coroutine Dispacher;

    void OnApplicationQuit()
    {
        if (Dispacher != null)
            StopCoroutine(Dispacher);
    }
    void Start()
    {
        if (Dispacher == null)
            Dispacher = StartCoroutine(Listener());
    }
    IEnumerator Listener()
    {

        yield return new WaitForSeconds(InitialDelaySeconds);
        while (true)
        {
            foreach (AchievementScheduleModel schedules in Schedules)
            {
                // check if the schedule is expired or active
                AchievementScheduleModel foundSchedule = DictArray.Where(a => a.type == schedules.type && a.status == ScheduleStatus.PENDING).FirstOrDefault();
                if (schedules.type == ScheduleType.PERMENENT)
                {
                    if (foundSchedule != null) continue;

                    // Check if the schedule is permenent
                    // add new schedule of this type
                    AchievementScheduleModel new_permenent_schedule = new AchievementScheduleModel(schedules.type, schedules.NumberOfMissions, schedules.name);
                    Add(new_permenent_schedule._id, new_permenent_schedule);
                    // generate the permenent models
                    AchievementModel[] models = data.models.Where(ach => ach.isOneTime == true).ToArray();
                    for (int i = 0; i < models.Length; i++)
                    {
                        AchievementModel new_achievement = models[i];
                        new_achievement._id = Guid.NewGuid();
                        new_achievement.Schedul_id = new_permenent_schedule._id;
                        ChangeModelCheckPoint(ref new_achievement, new_permenent_schedule);
                        AchievementTasksV2.self.AddNewAchievements(new AchievementModel[1] { new_achievement });
                    }
                }
                // Executes Only of the schedule is expired or its one time and done or expired
                if (foundSchedule != null)
                {

                    int expired = DateTime.Compare(DateTime.Now, foundSchedule.ExpireDate);
                    if (expired < 0 && foundSchedule.type != ScheduleType.ONETIME) continue;  // if the schedule isn't expired

                    int successfullTasks = 0;
                    foreach (AchievementModel item in BasePlayerPrefs<AchievementModel>.DictArray.Where(a => a.Schedul_id == foundSchedule._id).ToArray()) // deactivate the expired Tasks
                    {
                        item.isActive = expired < 0 && item.isActive && item.status != TrophyStatus.PAYED;

                        if (item.status == TrophyStatus.PAYED) successfullTasks++;
                        BasePlayerPrefs<AchievementModel>.Update(item._id, item);
                    }
                    foundSchedule.status = successfullTasks < foundSchedule.NumberOfMissions ? expired >= 0 ? ScheduleStatus.EXPIRED : ScheduleStatus.PENDING : ScheduleStatus.DONE;
                    Update(foundSchedule._id, foundSchedule);
                    if (foundSchedule.status == ScheduleStatus.PENDING) continue;
                }
                // add new schedule of this type
                AchievementScheduleModel new_schedule = new AchievementScheduleModel(schedules.type, schedules.NumberOfMissions, schedules.name);

                Add(new_schedule._id, new_schedule);

                for (int i = 0; i < schedules.NumberOfMissions; i++)
                {
                    AchievementModel new_achievement = GetRandomAchievemntByType(GetAchievementType);
                    new_achievement._id = Guid.NewGuid();
                    new_achievement.Schedul_id = new_schedule._id;
                    ChangeModelCheckPoint(ref new_achievement, new_schedule);
                    AchievementTasksV2.self.AddNewAchievements(new AchievementModel[1] { new_achievement });
                }
            }
            // check if the schedule is expired and delete its Task and the Schedule
            AchievementScheduleModel foundEndedSchedule = DictArray.Where(a => a.status != ScheduleStatus.PENDING).FirstOrDefault();
            if (foundEndedSchedule != null)
            {
                int passed_delettion = DateTime.Compare(DateTime.Now, foundEndedSchedule.ExpireDate.AddMinutes(HoldExpiredScheduleMinutes));
                if (passed_delettion >= 0)
                {
                    foreach (AchievementModel item in BasePlayerPrefs<AchievementModel>.DictArray.Where(a => a.Schedul_id == foundEndedSchedule._id).ToArray())
                    {
                        BasePlayerPrefs<AchievementModel>.Remove(item._id);
                    }
                    Remove(foundEndedSchedule._id);
                }
            }
            yield return new WaitForSeconds(ListenerTickSeconds);
        }
    }
    public void ChangeModelCheckPoint(ref AchievementModel model, AchievementScheduleModel schedule)
    {

        if (schedule != null && schedule.type == ScheduleType.ONETIME)
        {
            model.startPoint = 0;
            model.checkpoint = (int)Math.Round((Value(model) + model.checkpoint + 100) / 100d, 0, MidpointRounding.AwayFromZero) * 100;
        }
        else if (schedule.type == ScheduleType.PERMENENT)
            model.startPoint = 0;
        else
            model.startPoint = model.startPoint > 0 ? model.startPoint : Value(model);
    }
    public int Value(AchievementModel model)
    {
        Type type = typeof(GlobalValue);
        PropertyInfo field = type.GetProperty(model.fieldName, BindingFlags.Static | BindingFlags.Public);
        return (int)field.GetValue(null);
    }
    public AchievementModel GetRandomAchievemntByType(AchievementType _type)
    {
        return data.models.Where(achiv => achiv.type == _type && achiv.isOneTime == false).OrderBy(x => UnityEngine.Random.value).First();
    }
    public AchievementType GetAchievementType
    {
        get
        {
            float P = UnityEngine.Random.Range(0f, 1f);
            float C = Probability.Evaluate(P);
            int i = Mathf.RoundToInt(C);
            return (AchievementType)i;
        }
    }

}

