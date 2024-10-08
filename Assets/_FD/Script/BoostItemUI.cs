using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostItemUI : MonoBehaviour, IKeyboardCall
{
    public void KeyDown(KeyCode data)
    {
        if (data == KeyCode.M)
            ActiveDoubleArror();
        if (data == KeyCode.N)
            ActiveFrezzeArrow();
        if (data == KeyCode.B)
            ActivePoisonArrow();
    }
    public KeyCode[] KeyType { get { return new KeyCode[] { KeyCode.M, KeyCode.N, KeyCode.B }; } }
    public int KeyObjectID { get { return gameObject.GetInstanceID(); } }
    public static BoostItemUI Instance;
    [ReadOnly] public WEAPON_EFFECT currentEffect = WEAPON_EFFECT.NONE;
    [ReadOnly] public NumberArrow currentNumberOfArrows = NumberArrow.Single;
    
    
    [Header("Buttons & Texts")]
    public Button[] itemButtons;
    public Text[] itemRemainingTexts;
    public Text[] itemTimerTexts;
    public GameObject[] itemIcons;
    private int[] _chosenItems;
    public ShopItemData data;
    public GameObject[] disabledItemIcons;
     float[] timeCounter;
    public Sprite emptySprite;
    [Header("Double Arrow")]
  //  public Text DA_remainTxt;
  //  public Button DA_Button;
  //  public GameObject DA_Icon;
  //  public Text DA_timerTxt;
    public int DA_Time = 25;
    [ReadOnly] public float DA_TimeCounter = 0;
    //[Header("Triple Arrow")]
    //public Text TA_remainTxt;
    //public Button TA_Button;
    //public GameObject TA_Icon;
    //public Text TA_timerTxt;
    public int TA_Time = 20;
    //[ReadOnly] public float TA_TimeCounter = 0;
    [Header("Poison Arrow")]
  //  public Text PA_remainTxt;
  //  public Button PA_Button;
  //  public GameObject PA_Icon;
  //  public Text PA_timerTxt;
    public int PA_Time = 30;
    [ReadOnly] public float PA_TimeCounter = 0;

    [Header("Freeze Arrow")]
    //  public Text FA_remainTxt;
    //  public Button FA_Button;
    //  public GameObject FA_Icon;
    //  public Text FA_timerTxt;
    
    public int FA_Time = 30;
    [ReadOnly] public float FA_TimeCounter = 0;

    [Header("Boost Item")]
    public Animator boostItemAnim;
    public Animator boostButtonAnim;
    public float boostItemautoHide = 3;
    
    [Header("Slowdown Enemies")]
    public float SD_Rate = 0.3f;
    public float SD_Time = 3f;
    
    [Header("Log item")]
    public GameObject TL_Prefab;
    public Transform TL_SpawnPos;

    [Header("Fortress Shield")] public float FS_health = 300f;

    [Header("Attack Damage")] public float AD_Time = 3f;
    public float AD_Multiplier = 2f;
    [Space]
    public GameObject activeIcons;

    [Header("Lightning Global")] public AffectZone[] zones;
    
    private void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        timeCounter = new float[itemButtons.Length];
        _chosenItems = new int[itemButtons.Length];
        string[] chosenMagicsDecode = GlobalValue.inventoryItem.Split(',');
        for (int i = 0; i < chosenMagicsDecode.Length; i++)
        {
            _chosenItems[i] = int.Parse(chosenMagicsDecode[i]);
        }
        for (int i = 0; i < _chosenItems.Length; i++)
        {
            itemIcons[i].SetActive(false);
            if (_chosenItems[i] == -1 || GlobalValue.GetChosenShopItem(GetItemData(_chosenItems[i]).itemName)<1)
            {
                disabledItemIcons[i].SetActive(true);
                itemButtons[i].interactable = false;
                if (_chosenItems[i] == -1)
                {
                    itemButtons[i].image.sprite = emptySprite;
                }
            }
            else
            {
                disabledItemIcons[i].SetActive(false);
                itemButtons[i].interactable = true;
                itemButtons[i].image.sprite = GetItemData(_chosenItems[i]).buttonImage;
                itemRemainingTexts[i].text = "x" + GlobalValue.GetChosenShopItem(GetItemData(i).itemName);
            }
        }
        
    //   DA_remainTxt.text = "x" + GlobalValue.ItemDoubleArrow;
    //   //TA_remainTxt.text = "x" + GlobalValue.ItemTripleArrow;
    //   PA_remainTxt.text = "x" + GlobalValue.ItemPoison;
    //   FA_remainTxt.text = "x" + GlobalValue.ItemFreeze;
    //
    //   DA_Button.interactable = GlobalValue.ItemDoubleArrow > 0;
    //   //TA_Button.interactable = GlobalValue.ItemTripleArrow > 0;
    //   PA_Button.interactable = GlobalValue.ItemPoison > 0;
    //   FA_Button.interactable = GlobalValue.ItemFreeze > 0;
    //
    //   DA_Icon.SetActive(false);
    //   //TA_Icon.SetActive(false);
    //   PA_Icon.SetActive(false);
    //   FA_Icon.SetActive(false);
    }

    public void UseItem(int index)
    {
        RunTimerAutoHideBoostPanel();
        switch (GetItemData(_chosenItems[index]).itemName)
        {
            case "Log":
                ThrowLog();
                break;
            case "Bomb":
                //throw bomb
                break;
            case "Shield":
                ActivateFortressShield();
                break;
            case "DoubleArrow":
                ActiveDoubleArror();
                StartCoroutine(RunTimerCo(itemIcons[index],itemTimerTexts[index],DA_Time,index,itemButtons[index],GetItemData(_chosenItems[index]).id));
                break;
            case "PosionArrow":
                ActivePoisonArrow();
                StartCoroutine(RunTimerCo(itemIcons[index],itemTimerTexts[index],PA_Time,index,itemButtons[index],GetItemData(_chosenItems[index]).id));
                break;
            case "FrozenArrow":
                ActiveFrezzeArrow();
                StartCoroutine(RunTimerCo(itemIcons[index],itemTimerTexts[index],FA_Time,index,itemButtons[index],GetItemData(_chosenItems[index]).id));
                break;
            case "FireArrow":
                // activate fire arrow
                break;
            case "AttackDamage":
                AttackDamage();
                break;
            case "SlowDown":
                SlowDownEnemies();
                break;
        }
        GlobalValue.DecrementChosenShopItem(GetItemData(_chosenItems[index]).itemName);
        for (int i = 0; i < _chosenItems[index]; i++)
        {
            if (GlobalValue.GetChosenShopItem(GetItemData(_chosenItems[index]).itemName) < 1)
            {
                disabledItemIcons[i].SetActive(true);
                itemButtons[i].interactable = false;
            }
            else
            {
                disabledItemIcons[i].SetActive(false);
                itemButtons[i].interactable = true;
            }
        }
    }
    private void Update()
    {
        activeIcons.SetActive(itemIcons[0].activeSelf /*|| TA_Icon.activeSelf */|| itemIcons[1].activeSelf || itemIcons[2].activeSelf);
    }

    #region Double Arrow
    public void ActiveDoubleArror()
    {
        SoundManager.PlaySfx(SoundManager.Instance.BTsoundUseBoost);
      //  GlobalValue.ItemDoubleArrow--;
      //  DA_Button.interactable = false;     //only active per game level

        currentNumberOfArrows = NumberArrow.Double;
        //DA_Icon.SetActive(true);
        RunTimerAutoHideBoostPanel();
      //  DoubleArrowTimerCoDo = DoubleArrowTimerCo();
      //  StartCoroutine(DoubleArrowTimerCoDo);
    }

 //   IEnumerator DoubleArrowTimerCoDo;
 //  IEnumerator DoubleArrowTimerCo()
 //  {
 //      DA_Icon.SetActive(true);

 //      DA_TimeCounter = (float)DA_Time;
 //      while (DA_TimeCounter > 0)
 //      {
 //          DA_TimeCounter -= Time.deltaTime;
 //          DA_timerTxt.text = (int)DA_TimeCounter + "";
 //          yield return null;
 //      }

 //      DA_Icon.SetActive(false);
 //      DA_Button.interactable = true && GlobalValue.ItemDoubleArrow > 0;
 //      currentNumberOfArrows = NumberArrow.Single;
 //  }
    #endregion

    //#region Triple Arrow
    //public void ActiveTripleArrow()
    //{
    //    SoundManager.PlaySfx(SoundManager.Instance.BTsoundUseBoost);
    //    GlobalValue.ItemTripleArrow--;
    //    TA_remainTxt.text = "x" + GlobalValue.ItemTripleArrow;
    //    TA_Button.interactable = false;     //only active per game level
    //    DA_Button.interactable = false;
    //    ArcherForceEffect.Instance.SetNumberArrow(ARCHER_FIRE_ARROWS.TRIPLE);

    //    RunTimerAutoHideBoostPanel();

    //    if (DoubleArrowTimerCoDo != null)
    //        StopCoroutine(DoubleArrowTimerCoDo);

    //    StartCoroutine(TripleArrowTimerCo());
    //}

    //IEnumerator TripleArrowTimerCo()
    //{
    //    DA_Icon.SetActive(false);
    //    TA_Icon.SetActive(true);

    //    TA_TimeCounter = (float)TA_Time;
    //    while (TA_TimeCounter > 0)
    //    {
    //        TA_TimeCounter -= Time.deltaTime;
    //        TA_timerTxt.text = (int)TA_TimeCounter + "";
    //        yield return null;
    //    }

    //    TA_Icon.SetActive(false);
    //    TA_Button.interactable = true && GlobalValue.ItemTripleArrow > 0;
    //    DA_Button.interactable = true && GlobalValue.ItemDoubleArrow > 0;
    //    ArcherForceEffect.Instance.SetNumberArrow(NumberArrow.ONE);
    //}
    //#endregion

    #region Poison Arrow
    public void ActivePoisonArrow()
    {
        SoundManager.PlaySfx(SoundManager.Instance.BTsoundUseBoost);
        GlobalValue.ItemPoison--;
     //   PA_remainTxt.text = "x" + GlobalValue.ItemPoison;
     //   PA_Button.interactable = false;
     //   FA_Button.interactable = false;
        currentEffect = WEAPON_EFFECT.POISON;

        RunTimerAutoHideBoostPanel();
      //  StartCoroutine(PoisonArrowTimerCo());
    }

    IEnumerator RunTimerCo(GameObject icon,Text timerText,float Time,int index,Button itemButton,int id)
    {
        icon.SetActive(true);
        timeCounter[index] = (float)Time;
        while (timeCounter[index]> 0)
        {
            timeCounter[index] -= UnityEngine.Time.deltaTime;
            timerText.text = (int)timeCounter[index] + "";
            yield return null;
        }

        icon.SetActive(false);
        for (int i = 0; i < _chosenItems.Length; i++)
        {
            itemButtons[i].interactable = GlobalValue.GetChosenShopItem(GetItemData(_chosenItems[i]).itemName) > 0;
        }
        currentEffect = WEAPON_EFFECT.NONE;
    }



  //  IEnumerator PoisonArrowTimerCo()
  //  {
  //      PA_Icon.SetActive(true);
//
  //      PA_TimeCounter = (float)PA_Time;
  //      while (PA_TimeCounter > 0)
  //      {
  //          PA_TimeCounter -= Time.deltaTime;
  //          PA_timerTxt.text = (int)PA_TimeCounter + "";
  //          yield return null;
  //      }
//
  //      PA_Icon.SetActive(false);
  //      PA_Button.interactable = true && GlobalValue.ItemPoison > 0;
  //      FA_Button.interactable = true && GlobalValue.ItemFreeze > 0;
  //      currentEffect = WEAPON_EFFECT.NONE;
  //  }
    #endregion

    #region Freeze Arrow
    public void ActiveFrezzeArrow()
    {
        SoundManager.PlaySfx(SoundManager.Instance.BTsoundUseBoost);
        GlobalValue.ItemFreeze--;
      //  FA_remainTxt.text = "x" + GlobalValue.ItemFreeze;
      //  FA_Button.interactable = false;
      //  PA_Button.interactable = false;
        currentEffect = WEAPON_EFFECT.FREEZE;

        RunTimerAutoHideBoostPanel();

    //    StartCoroutine(FrezzeArrowTimerCo());
    }

  // IEnumerator FrezzeArrowTimerCo()
  // {
  //     FA_Icon.SetActive(true);
  //     FA_TimeCounter = (float)FA_Time;
  //     while (FA_TimeCounter > 0)
  //     {
  //         FA_TimeCounter -= Time.deltaTime;
  //         FA_timerTxt.text = (int)FA_TimeCounter + "";
  //         yield return null;
  //     }

  //     FA_Icon.SetActive(false);
  //     PA_Button.interactable = true && GlobalValue.ItemPoison > 0;
  //     FA_Button.interactable = true && GlobalValue.ItemFreeze > 0;
  //     currentEffect = WEAPON_EFFECT.NONE;
  // }
    #endregion

    #region BOOST PANEL
    IEnumerator BoostItemHideCoDo;
    public void BoostItem()
    {
        if (boostItemAnim.GetBool("show"))
        {
            HideBoostPanel();
        }
        else
        {
            SoundManager.PlaySfx(SoundManager.Instance.BTsoundOpen);
            boostItemAnim.SetBool("show", true);
            boostButtonAnim.SetBool("on", true);
            RunTimerAutoHideBoostPanel();
        }
    }

    void RunTimerAutoHideBoostPanel()
    {
        if (BoostItemHideCoDo != null)
            StopCoroutine(BoostItemHideCoDo);

        BoostItemHideCoDo = BoostItemHideCo(boostItemautoHide);
        StartCoroutine(BoostItemHideCoDo);
    }

    IEnumerator BoostItemHideCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideBoostPanel();
    }

    void HideBoostPanel()
    {
        SoundManager.PlaySfx(SoundManager.Instance.BTsoundHide);
        boostItemAnim.SetBool("show", false);
        boostButtonAnim.SetBool("on", false);
    }
    

    #endregion
    #region Throw Log
    public void ThrowLog()
    {
        Instantiate(TL_Prefab,TL_SpawnPos.position, Quaternion.identity);
    }
    #endregion
    #region SlowDown
    public void SlowDownEnemies()
    {
        GlobalValue.SlowDownRate = SD_Rate;
        StartCoroutine(DisableSlowDown());
    }
    IEnumerator DisableSlowDown()
    {
        yield return new WaitForSeconds(SD_Time);
        GlobalValue.SlowDownRate = 1f;
    }
    #endregion
    #region Fortress Shield

    public void ActivateFortressShield()
    {
        FindObjectOfType<TheFortrest>().ActivateShield(FS_health);
    }
    #endregion
    
    #region Attack Damage

    public void AttackDamage()
    {
        GlobalValue.AttackDamageRate = AD_Multiplier;
        StartCoroutine(ResetAttackDamageRate());
    }

    IEnumerator ResetAttackDamageRate()
    {
        yield return new WaitForSeconds(AD_Time);
        GlobalValue.AttackDamageRate = 1f;
    }
    
    #endregion
    
    
    
    ShopItemData.ShopItem GetItemData(int id)
    {
        ShopItemData.ShopItem chosenData = new ShopItemData.ShopItem();
        for (int i = 0; i < data.ShopData.Length; i++)
        {
            if (data.ShopData[i].id == id)
            {
                chosenData = data.ShopData[i];
            }
        }

        return chosenData;
    }
}
