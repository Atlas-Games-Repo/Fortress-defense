using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance;
    //[Header("UNITY AD SETUP")]
    //public string UNITY_ANDROID_ID = "1486550";
    //public string UNITY_IOS_ID = "1486551";
    //public bool isTestMode = true;
    //public AudioClip soundReward;
    //[Tooltip("Show video ad after how many time Victort or Gameover")]
    //public int showVideoAdAfter = 3;
    //int counter = 0;

    [Header("SHOP SETUP")]
    public int upgradeFortressPrice = 1000;
    public int doubleArrowPrice = 1000;
    public int poisonArrowPrice = 1000;
    public int freezeArrowPrice = 1000;

    [Header("FPS DISPLAY")]

    public bool showInfor = true;
    [HideInInspector] public Vector2 resolution = new Vector2(1280, 720);
    public int setFPS = 60;
    float deltaTime = 0.0f;

    // public Purchaser purchase;

    // public void BuyItem(int id)
    // {
    //     switch (id)
    //     {
    //         case 1:
    //             purchase.BuyItem1();
    //             break;
    //         case 2:
    //             purchase.BuyItem2();
    //             break;
    //         case 3:
    //             purchase.BuyItem3();
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // public void BuyRemoveAds()
    // {
    //     purchase.BuyRemoveAds();
    // }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //Screen.SetResolution((int)resolution.x, (int)resolution.y);
        // Application.targetFrameRate = setFPS;

        //        string gameId = "";
        //#if UNITY_IOS
        //		gameId = UNITY_IOS_ID;
        //#elif UNITY_ANDROID
        //        gameId = UNITY_ANDROID_ID;
        //#endif

        //        if (Advertisement.isSupported)
        //        {
        //            Advertisement.Initialize(gameId, isTestMode);
        //        }
    }

    #region FPS DISPLAY
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.U))
        {
            GlobalValue.LevelPass = 16;
            GlobalValue.WorldPass = 2;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            ResetDATA();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            User.Coin = 999999;
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
        {
            GlobalValue.LevelPass = 999999;
        }
    }

    public void ResetDATA()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    void OnGUI()
    {
        if (showInfor)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);


            GUI.Label(rect, text, style);

            Rect rect2 = new Rect(250, 0, w, h * 2 / 100);
            GUI.Label(rect2, Screen.currentResolution.width + "x" + Screen.currentResolution.height, style);
        }
    }
    #endregion

    //#region UNITY AD
    //public bool isRewardedAdReady()
    //{
    //    return Advertisement.IsReady("rewardedVideo");
    //}

    //public void ShowRewardVideo()
    //{
    //    SoundManager.Click();
    //    if (Advertisement.IsReady("rewardedVideo"))
    //    {
    //        var options = new ShowOptions { resultCallback = HandleShowResult };
    //        Advertisement.Show("rewardedVideo", options);
    //    }
    //}

    //public void ShowNormalAd()
    //{
    //    counter++;
    //    Debug.Log("AD COUNTER = " + counter);
    //    if (counter < showVideoAdAfter)
    //        return;

    //    if (Advertisement.IsReady())
    //    {
    //        Debug.Log("SHOW VIDEO AD");
    //        counter = 0;
    //        Advertisement.Show();
    //    }
    //}

    //private void HandleShowResult(ShowResult result)
    //{
    //    switch (result)
    //    {
    //        case ShowResult.Finished:
    //            Debug.Log("The ad was successfully shown.");

    //            //	#if UNITY_EDITOR
    //            User.Coin = watchVideoAdRewarded;
    //            SoundManager.PlaySfx(soundReward);
    //            //	#endif
    //            break;
    //        case ShowResult.Skipped:
    //            Debug.Log("The ad was skipped before reaching the end.");

    //            break;
    //        case ShowResult.Failed:
    //            Debug.LogError("The ad failed to be shown.");
    //            break;
    //    }
    //}
    //#endregion
}
