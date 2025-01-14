using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class AffectZoneButton : MonoBehaviour, IKeyboardCall
{
    public void KeyDown(KeyCode keyCode)
    {
        OnBtnClick();
    }
    public KeyCode[] KeyType { get { return new KeyCode[] { key }; } }
    public int KeyObjectID { get { return gameObject.GetInstanceID(); } }
    public KeyCode key;
    public AffectZoneType affectType;
    public Button ownBtn;

    [Header("COOL DOWN")]
    public float delayOnStart = 2;
    public float coolDown = 3f;
    float coolDownCounter = 0;
    public Image image;
    public Text timerTxt, XPTxt;
    bool allowWork = true;
    bool allowCounting = false;
    bool canUse = true;
    bool can_pay = true;
    int XPConsume;
    public string xp_text_prefix = "xp";
    float holdCounter = 0;

    private MagicSlotManager _magicSlotManager;



    public CanvasGroup canvasGroup;

    void Start()
    {
        _magicSlotManager = transform.parent.GetComponent<MagicSlotManager>();
        XPConsume = AffectZoneManager.Instance.XPconsume(affectType);
        XPTxt.text = AffectZoneManager.Instance.isZoneUsedFirstTime ?  $"{xp_text_prefix}{XPConsume}" : $"{xp_text_prefix}0";
        ownBtn = GetComponent<Button>();
        ownBtn.onClick.AddListener(OnBtnClick);
        if (affectType == AffectZoneType.Cure)
            ownBtn.interactable = false;

        if (image == null)
            image = GetComponent<Image>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        allowWork = false;
        allowCounting = true;
        coolDownCounter = delayOnStart;
    }

    void Update()
    {
        if (!allowWork)
        {
            if (allowCounting)
            {
                coolDownCounter -= Time.deltaTime;

                if (coolDownCounter <= 0)
                    allowWork = true;
            }
        }
        else
        {
            holdCounter -= Time.deltaTime;
        }

        image.fillAmount = Mathf.Clamp01((coolDown - coolDownCounter) / coolDown);
        timerTxt.text = (int)coolDownCounter + "";
        if ((int)coolDownCounter == 0)
            timerTxt.text = "";

        int fortressHealth = (int)FindObjectOfType<TheFortrest>().maxHealth - (int)FindObjectOfType<TheFortrest>().currentHealth;

        canUse = coolDownCounter <= 0 && canvasGroup.blocksRaycasts && !AffectZoneManager.Instance.isAffectZoneWorking && !AffectZoneManager.Instance.isChecking;
        if (AffectZoneManager.Instance.isZoneUsedFirstTime)
        {
            can_pay = GameManager.Instance.currentExp >= XPConsume;
        }
        else
        {
            can_pay = true;
        }
        if (affectType == AffectZoneType.Cure)
            ownBtn.interactable = canUse && fortressHealth > 0 && can_pay;
        canvasGroup.interactable = canUse && can_pay;
    }
    #region LightningGlobal

    public void ActivateLightnings()
    {
        AffectZoneManager.Instance.LightningAll();
    }
    #endregion

    public void ActivateArmagdon()
    {
        AffectZoneManager.Instance.Armagdon();
    }

    public void ActivateDefenseWall()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.DefenseWall, this);
        SoundManager.Click();
    }
    void ActiveLighting()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.Lighting, this);
        SoundManager.Click();
    }
    void ActiveMagnet()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.Magnet, this);
        SoundManager.Click();
    }
    void ActiveFire()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.Fire, this);
        SoundManager.Click();
    }
    void ActiveDark()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.Dark, this);
        SoundManager.Click();
    }
    void ActiveAero()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.Aero, this);
        SoundManager.Click();
    }
    void ActiveCure()
    {
        //    AffectZoneManager.Instance.ActiveZone(AffectZoneType.Cure, this);
        StartCoroutine(AffectZoneManager.Instance.Cure(this,coolDown));
        SoundManager.Click();
    }

    void ActiveFrozen()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.Frozen, this);
        SoundManager.Click();
    }

    void ActivePoison()
    {
        AffectZoneManager.Instance.ActiveZone(AffectZoneType.Poison, this);
        SoundManager.Click();
    }


    public void UpdateXPText()
    {
        // AffectZoneManager.Instance.isZoneUsedFirstTime = true;
        XPTxt.text = $"{xp_text_prefix}{XPConsume}";
        XPTxt.text = $"{xp_text_prefix}{XPConsume}";
    }

    public void StartCountingDown(float custom_cooldown = 0)
    {
        allowCounting = true;
        coolDownCounter = custom_cooldown > 0 ? custom_cooldown : coolDown;
    }

    
    private void OnBtnClick()
    {

        _magicSlotManager.OnFirstMagicUse();
        if (!canUse)
            return;

        if (!allowWork)
            return;

        switch (affectType)
        {
            case AffectZoneType.Lighting:
                GlobalValue.LightningUsage++;
                ActiveLighting();
                break;
            case AffectZoneType.Frozen:
                GlobalValue.FreezeUsage++;
                ActiveFrozen();
                break;
            case AffectZoneType.Poison:
                GlobalValue.PoisionUsage++;
                ActivePoison();
                break;
            case AffectZoneType.Magnet:
                ActiveMagnet();
                break;
            case AffectZoneType.Cure:
                ActiveCure();
                break;
            case AffectZoneType.Fire:
                ActiveFire();
                break;
            case AffectZoneType.Aero:
                ActiveAero();
                break;
            case AffectZoneType.Dark:
                ActiveDark();
                break;
            case AffectZoneType.LightningAll:
                ActivateLightnings();
                break;
            case AffectZoneType.Armagdon:
                ActivateArmagdon();
                break;
            case AffectZoneType.DefenseWall :
                ActivateDefenseWall();
                break;
        }

        allowWork = false;
        allowCounting = false;

    }
}
