using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHomeScene : MonoBehaviour
{
    public static MainMenuHomeScene Instance;
    public GameObject HomeUI;
    public GameObject MapUI;
    public GameObject ShopUI;
    public GameObject TrophyUI,TrophyUIV2;
    public GameObject EventUI, CoinShopUI, UpgradeUI,LeaderBoardUI;
    public GameObject StoreUI;
    public GameObject Loading;
    public GameObject Settings;
    public GameObject inventory;
    public string facebookLink;
    public string twitterLink = "https://twitter.com/";
    public string playingLevelName = "Playing atlas";

    public Text[] coinTxt;

    [Header("Sound and Music")]
    public Image soundImage;
    public Image musicImage;
    public Sprite soundImageOn, soundImageOff, musicImageOn, musicImageOff;

    void Awake()
    {
        Instance = this;
        if (HomeUI)
            HomeUI.SetActive(false);
        if (Loading != null)
            Loading.SetActive(false);
        if (MapUI != null)
            MapUI.SetActive(false);
        if (Settings)
            Settings.SetActive(false);
        if (ShopUI)
            ShopUI.SetActive(false);
        if (TrophyUI)
            TrophyUI.SetActive(false);
        if (TrophyUIV2)
            TrophyUIV2.SetActive(false);
        if (EventUI)
            EventUI.SetActive(false);
        if (CoinShopUI)
            CoinShopUI.SetActive(false);
        if (UpgradeUI)
            UpgradeUI.SetActive(false);
        if (LeaderBoardUI)
            LeaderBoardUI.SetActive(false);
    }

    public void LoadScene()
    {
        if (Loading != null)
            Loading.SetActive(true);

        StartCoroutine(LoadAsynchronously(playingLevelName));
    }

    public void LoadScene(string sceneNamage)
    {
        if (Loading != null)
            Loading.SetActive(true);

        StartCoroutine(LoadAsynchronously(sceneNamage));
    }

    IEnumerator Start()
    {
        CheckSoundMusic();
        if (GlobalValue.isFirstOpenMainMenu)
        {
            GlobalValue.isFirstOpenMainMenu = false;
            SoundManager.Instance.PauseMusic(true);
            SoundManager.PlaySfx(SoundManager.Instance.beginSoundInMainMenu);
            yield return new WaitForSeconds(SoundManager.Instance.beginSoundInMainMenu.length);
            SoundManager.Instance.PauseMusic(false);
            SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
        }
        yield return new WaitForSeconds(1);
        HomeUI.SetActive(true);
    }

    void Update()
    {
        CheckSoundMusic();

        foreach (var ct in coinTxt)
        {
            ct.text = User.Coin + "";
        }
    }

    public void OpenMap(bool open)
    {
        SoundManager.Click();
        StartCoroutine(OpenMapCo(open));
    }

    public void OpenInventory(bool open)
    {
        SoundManager.Click();
        StartCoroutine(OpenInventoryCo(open));
    }

    IEnumerator OpenInventoryCo(bool open)
    {
        yield return null;
        BlackScreenUI.instance.Show(0.2f);
        inventory.SetActive(open);
        inventory.GetComponent<Inventory>().InitSlots();
        BlackScreenUI.instance.Hide(0.2f);
    }
    IEnumerator OpenMapCo(bool open)
    {
        yield return null;
        BlackScreenUI.instance.Show(0.2f);
        MapUI.SetActive(open);
        BlackScreenUI.instance.Hide(0.2f);
    }

    public void Facebook()
    {
        SoundManager.Click();
        Application.OpenURL(facebookLink);
    }

    public void Twitter()
    {
        SoundManager.Click();
        Application.OpenURL(twitterLink);
    }

    public void ExitGame()
    {
        SoundManager.Click();
        Application.Quit();
    }

    public void Setting(bool open)
    {
        SoundManager.Click();
        Settings.SetActive(open);
    }

    public void Store(bool open)
    {
        SoundManager.Click();
        StoreUI.SetActive(open);
        StoreUI.GetComponent<Shop>().OpenMenu("features");
    }
    public void OpenUpgradeUI(bool open)
    {
        SoundManager.Click();
        UpgradeUI.SetActive(open);
    }

    #region Music and Sound
    public void TurnSound()
    {
        GlobalValue.isSound = !GlobalValue.isSound;
        soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;

        SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
    }

    public void TurnMusic()
    {
        GlobalValue.isMusic = !GlobalValue.isMusic;
        musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

        SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
    }
    #endregion

    private void CheckSoundMusic()
    {
        soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
        musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;
        SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
        SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
    }

    public void OpenShop(bool open)
    {
        SoundManager.Click();
        ShopUI.SetActive(open);
    }
    public void OpenTrophy(bool open)
    {
        SoundManager.Click();
        TrophyUI.SetActive(open);
    }

    public void OpenLeaderBoard(bool open)
    {
        LeaderBoard leaderBoard = LeaderBoardUI.GetComponent<LeaderBoard>();
        LeaderBoardUI.SetActive(open);
        if(!open) leaderBoard.ClearList();
    }
    public void OpenTrophyV2(bool open)
    {
        SoundManager.Click();
        TrophyUIV2.SetActive(open);
    }
    public void OpenEvent(bool open)
    {
        SoundManager.Click();
        EventUI.SetActive(open);
    }
    public void OpenCoinShop(bool open)
    {
        SoundManager.Click();
        CoinShopUI.SetActive(open);
    }

    public void Tutorial()
    {
        SoundManager.Click();
        SceneManager.LoadScene("Tutorial");
    }

    public Slider slider;
    public Text progressText;
    IEnumerator LoadAsynchronously(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = (int)progress * 100f + "%";
            yield return null;
        }
    }

    public void ResetData()
    {
        if (GameMode.Instance)
            GameMode.Instance.ResetDATA();
    }
}
