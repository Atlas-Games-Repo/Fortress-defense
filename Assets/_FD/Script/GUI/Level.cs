/// <summary>
/// The UI Level, check the current level
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public enum IsBoss { NONE, MINIBOSS, BOSS }; //is the level has boos or not
    public enum LeveType { MISSION, EVENT };
    public int world = 1;
    public int level = 1;
    public bool isUnlock = false;
    public Text numberTxt;
    public GameObject imgLock, imgOpen, imgPass;

    public GameObject starGroup;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public GameObject bossGroup;
    public GameObject miniBoss;
    public GameObject boss;
    public IsBoss is_boss = IsBoss.NONE;

    public bool loadSceneManual = false;
    public string loadSceneName = "story1";

    public LeveType levelType = LeveType.MISSION;
    public Events _event;

    public bool nightMode;
    void Start()
    {
        switch (levelType)
        {
            case LeveType.MISSION:
                StartMission();
                break;
            case LeveType.EVENT:
                StartEvent();
                break;
            default:
                break;
        }
    }
    void StartEvent()
    {
        imgLock.SetActive(false);
        imgOpen.SetActive(false);
        imgPass.SetActive(false);
        //check if this level is completed
        Events evnt = _Event.GetEvent(_event.level);
        var openLevel = evnt == null ? true : !evnt.is_passed;
        if (openLevel)
        {
            imgOpen.SetActive(false);
        }
        else
        {
            imgPass.SetActive(true);

        }
        GetComponent<Button>().interactable = openLevel;

    }
    // Use this for initialization
    void StartMission()
    {

        //check if this level > allowing level then disable it
        if (GameLevelSetup.Instance && level > GameLevelSetup.Instance.getTotalLevels())
        {
            gameObject.SetActive(false);
            return;
        }

        numberTxt.text = level + "";
        var openLevel = isUnlock ? true : GlobalValue.LevelPass + 1 >= level;
        //		var levelUnlocked = isUnlock ? true : GlobalValue.isLevelUnlocked (levelSceneName);	
        var stars = GlobalValue.LevelStar(level);       //get the stars of the current level

        star1.SetActive(openLevel && stars >= 1);
        star2.SetActive(openLevel && stars >= 2);
        star3.SetActive(openLevel && stars >= 3);

        imgLock.SetActive(false);
        imgOpen.SetActive(false);
        imgPass.SetActive(false);
        starGroup.SetActive(false);
        bossGroup.SetActive(false);

        if (openLevel)
        {
            if (GlobalValue.LevelPass + 1 == level)
            {
                imgOpen.SetActive(true);
                FindObjectOfType<MapControllerUI>().SetCurrentWorld(world);
            }
            else
            {
                imgPass.SetActive(true);
                starGroup.SetActive(true);
                //numberTxt.gameObject.SetActive(false);
            }

        }
        else
            imgLock.SetActive(true);

        if (is_boss != IsBoss.NONE)
        {
            bossGroup.SetActive(true);
            switch (is_boss)
            {
                case IsBoss.MINIBOSS:
                    miniBoss.SetActive(true);
                    break;
                case IsBoss.BOSS:
                    boss.SetActive(true);
                    break;
                default:
                    break;
            }
        }


        GetComponent<Button>().interactable = openLevel;
    }

    public void Play()
    {
        GlobalValue.levelPlaying = level;
        GlobalValue.levelType = levelType;
       // GlobalValue.NightMode = nightMode;
        if (levelType == LeveType.EVENT)
        {
            _Event.Add(_event.level, _event);
            _Event.CurrentEventPlaying = _event.level;
        }
        SoundManager.Click();

        if (LifeTTRSource.Life > 0)
            MainMenuHomeScene.Instance.LoadScene();

    }

    public void Play(string _levelSceneName = null)
    {

        SoundManager.Click();
        //if (loadSceneManual && GlobalValue.showComicBossLevel)
        //{
        //    MainMenuHomeScene.Instance.LoadScene(loadSceneName);
        //}
        //else
        //{
        GlobalValue.levelPlaying = level;
        GlobalValue.levelType = levelType;
        if (levelType == LeveType.EVENT)
        {
            _Event.Add(_event.level, _event);
            _Event.CurrentEventPlaying = _event.level;
        }
        if (LifeTTRSource.Life > 0)
            MainMenuHomeScene.Instance.LoadScene(_levelSceneName);
        //}
    }
}

