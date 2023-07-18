using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum AchievementFilter
{
    ALL,
    NORMAL,
    RARE,
    EPIC,
    LEGENDARY,
    COMPELETE,
    EXPIRED,
    DAILY,
    WEEKLY,
    ONETIME
}

public class AchievementManagerV2 : MonoBehaviour
{
    public RectTransform parent;
    public GameObject rootObject;
    public string TextPlaceHolder = "zNoNz";
    public string ColorTag = "<color=\"green\">TAGED</color>";
    public string ColorTagPlaceHolder = "TAGED", RewardPrefix = "Reward: ";
    public Gradient BackgroundColor, OppositeColors;
    public string NameIndex, DescriptionIndex, SliderIndex, ClaimIndex, DoneIndex, StatusIndex, TypeIndex, BackgroundIndex, TimerIndex, RewardIndex;

    public void ReloadPage(int index)
    {
        StartCoroutine(StartEnum(index));
    }
    IEnumerator StartEnum(int index)
    {
        // Get all child objects
        GameObject[] children = new GameObject[parent.transform.childCount];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
            yield return null;
        }

        // Delete child objects
        foreach (GameObject child in children)
        {
            Destroy(child);
            yield return null;
        }
        AchievementModel[] models = new AchievementModel[0];
        AchievementScheduleModel scheduleModelDaily = BasePlayerPrefs<AchievementScheduleModel>.DictArray.Where(s => s.type == ScheduleType.DAYLY).FirstOrDefault();
        AchievementScheduleModel scheduleModelWeekly = BasePlayerPrefs<AchievementScheduleModel>.DictArray.Where(s => s.type == ScheduleType.WEEKLY).FirstOrDefault();
        AchievementScheduleModel scheduleModelOneTime = BasePlayerPrefs<AchievementScheduleModel>.DictArray.Where(s => s.type == ScheduleType.ONETIME).FirstOrDefault();
        switch (index)
        {
            case 0: models = BasePlayerPrefs<AchievementModel>.DictArray; break;
            case 1: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.type == AchievementType.NORMAL).ToArray(); break;
            case 2: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.type == AchievementType.RARE).ToArray(); break;
            case 3: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.type == AchievementType.EPIC).ToArray(); break;
            case 4: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.type == AchievementType.LEGENDARY).ToArray(); break;
            case 5: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.status > TrophyStatus.ACHIEVED).ToArray(); break;
            case 6: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.status < TrophyStatus.ACHIEVED && !k.isActive).ToArray(); break;
            case 7: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.Schedul_id == scheduleModelDaily._id).ToArray(); break;
            case 8: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.Schedul_id == scheduleModelWeekly._id).ToArray(); break;
            case 9: models = BasePlayerPrefs<AchievementModel>.DictArray.Where(k => k.Schedul_id == scheduleModelOneTime._id).ToArray(); break;
            default: break;
        }
        foreach (AchievementModel model in models)
        {
            Add(model);
            yield return null;
        }
    }
    public async void ClaimAchievement(AchievementModel foundedModel, GameObject obj){
        if (foundedModel == null || !foundedModel.isActive || !((int)foundedModel.status >= 2)) return;
        bool result = await UserV2.SyncSetCoin(foundedModel.reward);
        if (result)
        {
            foundedModel.status = TrophyStatus.PAYED;
            BasePlayerPrefs<AchievementModel>.Update(foundedModel._id,foundedModel);
            SetUpButtons(obj,DoneIndex);
        }
    }
    void Add(AchievementModel trophy)
    {
        GameObject obj = Instantiate(rootObject, parent, false);
        ChildInParent.GetChild(obj.transform, NameIndex).GetComponent<TextMeshProUGUI>().text = trophy.name;
        ChildInParent.GetChild(obj.transform, DescriptionIndex).GetComponent<TextMeshProUGUI>().text = trophy.description.Replace(TextPlaceHolder, ColorTag.Replace(ColorTagPlaceHolder,trophy.checkpoint.ToString()));
        ChildInParent.GetChild(obj.transform, BackgroundIndex).GetComponent<Image>().color = BackgroundColor.Evaluate((float)trophy.type/(float)AchievementType.LEGENDARY);
        ChildInParent.GetChild(obj.transform, RewardIndex).GetComponent<TextMeshProUGUI>().text = RewardPrefix + trophy.reward.ToString();
        ChildInParent.GetChild(obj.transform, RewardIndex).GetComponent<TextMeshProUGUI>().color = OppositeColors.Evaluate((float)trophy.type/(float)AchievementType.LEGENDARY);

        BasePlayerPrefs<AchievementScheduleModel>.TryGetValue(trophy.Schedul_id, out AchievementScheduleModel scheduleModel);
        if (scheduleModel != null)
        {
            ChildInParent.GetChild(obj.transform, TypeIndex).GetComponent<TextMeshProUGUI>().text = scheduleModel.name;
            // TimeSpan timer = scheduleModel.ExpireDate - DateTime.Now;
            ChildInParent.GetChild(obj.transform, TimerIndex).GetComponent<AvhievementTimerClock>().StartTheClock(scheduleModel.ExpireDate);
            ChildInParent.GetChild(obj.transform, TimerIndex).GetComponent<TextMeshProUGUI>().color = OppositeColors.Evaluate((float)trophy.type/(float)AchievementType.LEGENDARY);
        }

        AchievementTasksV2.self.TryGetEvent(trophy._id, out AchievementEventsV2 Events);
        if (Events != null)
        {
            ChildInParent.GetChild(obj.transform,StatusIndex).GetComponent<TextMeshProUGUI>().text = Events.OutOf;
            ChildInParent.GetChild(obj.transform,SliderIndex).GetComponent<Slider>().value = Events.Proccess;
            ChildInParent.GetChild(obj.transform,ClaimIndex).GetComponent<Button>().onClick.AddListener(() => ClaimAchievement(trophy, obj));
        }

        if ((int)trophy.status >= (int)TrophyStatus.ACHIEVED)
        {
            SetUpButtons(obj,ClaimIndex);
        }
        else
        {
            SetUpButtons(obj,SliderIndex);
        }
        if ((int)trophy.status == (int)TrophyStatus.PAYED)
        {
            SetUpButtons(obj,DoneIndex);
        }

    }
    void SetUpButtons(GameObject obj, string state){
        ChildInParent.GetChild(obj.transform,ClaimIndex).gameObject.SetActive(state == ClaimIndex);
        ChildInParent.GetChild(obj.transform,SliderIndex).gameObject.SetActive(state == SliderIndex);
        ChildInParent.GetChild(obj.transform,DoneIndex).gameObject.SetActive(state == DoneIndex);
    }

}
