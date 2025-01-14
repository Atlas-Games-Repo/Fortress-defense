using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MapControllerUI : MonoBehaviour
{
    //	public Transform BlockLevel;
    public RectTransform BlockLevel;
    public int howManyBlocks = 3;

    public float step = 720f;

    private float newPosX = 0;
    public Text worldTxt;
    int currentPos = 0;
    public static event Action<int,bool> OnMapChange;
    public float shadow_oppasity = 0.15f, shadow_delay = 0.01f;
    public AudioClip music;
    public GameObject life_prefab;
    public Transform life_parent;
    public GameObject[] lifes;
    // Use this for initialization
    void Start()
    {
        //SetDots();
        SetWorldNumber();
    }

    void SetWorldNumber()
    {
        worldTxt.text = currentPos + 1 + "/" + howManyBlocks;
    }
    //void SetDots()
    //{
    //    foreach(var obj in Dots)
    //    {
    //        obj.color = new Color(1, 1, 1, 0.5f);
    //        obj.rectTransform.sizeDelta = new Vector2(28, 28);
    //    }

    //    Dots[currentPos].color = Color.yellow;
    //    Dots[currentPos].rectTransform.sizeDelta = new Vector2(38, 38);
    //}
    void createLifes()
    {
        foreach (Transform obj in life_parent)
        {
            Destroy(obj.gameObject);
        }
        lifes = new GameObject[LifeTTRSource.max_life];
        for (int i = 0; i < lifes.Length; i++)
        {
            GameObject obj = Instantiate(life_prefab, life_parent, false);
            lifes[i] = obj;
        }

    }
    void OnEnable()
    {
        SoundManager.PlayMusic(music);
        createLifes();
        UpdateLifes();
    }
    void UpdateLifes()
    {
        foreach (var item in lifes)
        {
            item.SetActive(false);
        }
        // show how much life a player have
        for (int i = 1; i <= LifeTTRSource.Life; i++)
        {
            lifes[i - 1].SetActive(true);
        }
        if (APIManager.instance.lifeTTR.TTL() > 0)
        {
            lifes[LifeTTRSource.Life].SetActive(true);
        }
    }
    void Update()
    {
        if (APIManager.instance.lifeTTR.TTL() > 0)
        {
            lifes[LifeTTRSource.Life].GetComponent<Image>().fillAmount = 1 - APIManager.instance.lifeTTR.TTLPercent;
            UpdateLifes();
        }
    }
    void OnDisable()
    {
        SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
    }

    public void SetCurrentWorld(int world)
    {
        currentPos += (world - 1);
        newPosX -= step * (world - 1);
        newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
        SetMapPosition();
        SetWorldNumber();
        Invoke(nameof(MapChangeWithDelay),Time.unscaledDeltaTime);
    }

   void MapChangeWithDelay()
    {
        OnMapChange?.Invoke(currentPos,false);
    }

    public void SetMapPosition()
    {
        BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
    }

    bool allowPressButton = true;
    public void Next()
    {
        if (allowPressButton)
        {
            StartCoroutine(NextCo());
        }
    }

    IEnumerator NextCo()
    {
        allowPressButton = false;

        SoundManager.Click();

        if (newPosX != (-step * (howManyBlocks - 1)))
        {
            currentPos++;
            OnMapChange?.Invoke(currentPos,true);
            newPosX -= step;
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

        }
        else
        {
            allowPressButton = true;
            yield break;

            //currentPos = 0;

            //newPosX = 0;
            //newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);


        }

        BlackScreenUI.instance.Show(shadow_oppasity);

        yield return new WaitForSeconds(shadow_delay);
        SetMapPosition();
        BlackScreenUI.instance.Hide(shadow_oppasity);

        SetWorldNumber();


        allowPressButton = true;

    }

    public void Pre()
    {
        if (allowPressButton)
        {
            StartCoroutine(PreCo());
        }
    }

    IEnumerator PreCo()
    {
        allowPressButton = false;
        SoundManager.Click();
        if (newPosX != 0)
        {
            currentPos--;
            OnMapChange?.Invoke(currentPos,true);
            newPosX += step;
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);


        }
        else
        {
            allowPressButton = true;
            yield break;
            //currentPos = howManyBlocks - 1;

            //newPosX = -999999;
            //newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

        }

        BlackScreenUI.instance.Show(shadow_oppasity);

        yield return new WaitForSeconds(shadow_delay);
        SetMapPosition();
        BlackScreenUI.instance.Hide(shadow_oppasity);

        SetWorldNumber();


        allowPressButton = true;

    }

    public void UnlockAllLevels()
    {
        GlobalValue.LevelPass = (GlobalValue.LevelPass + 1000);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        SoundManager.Click();
    }
}
