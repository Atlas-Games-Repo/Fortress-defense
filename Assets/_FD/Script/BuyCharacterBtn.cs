using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyCharacterBtn : MonoBehaviour, IKeyboardCall
{
    public void KeyDown(KeyCode code)
    {
        OnBtnClick();
    }
    public LayerMask targetLayerForPet;
    public ShopItemData data;
    private int[] _chosenPet;
    int _petToSpawn = 0;
    public int _petCount = 1;
    public GameObject[] pets;
    public KeyCode[] KeyType { get { return new KeyCode[] { KeyCode.E }; } }
    public int KeyObjectID { get { return gameObject.GetInstanceID(); } }
    public UpgradedCharacterParameter characterID;
    public int price = 888;
    public int max = 5;
    [ReadOnly] public int current = 0;
    public GameObject character;

    public GameObject lockIcon;
    public Text priceTxt;
    //public Text unlockLevelTxt;
    public Text numberTxt;
    public AudioClip soundPurchase;
    Button ownBtn;
    bool isUnlocked = false;

    [ReadOnly] public List<GameObject> listCharacters;


    [Header("COOL DOWN")]
    public float delayOnStart = 2;
    public float coolDown = 3f;
    float coolDownCounter = 0;
    public Image image;
    bool allowWork = true;
    bool canUse = true;
    float holdCounter = 0;
    public CanvasGroup canvasGroup;

    public GameObject poisonFX, freezeFX;

    void Start()
    {
        string[] chosenPetsDecode = GlobalValue.inventoryPets.Split(',');
        _petCount = chosenPetsDecode.Length;
        _chosenPet = new int[_petCount];
        pets = new GameObject[_petCount];
        for (int i = 0; i < chosenPetsDecode.Length; i++)
        {
            _chosenPet[i] = int.Parse(chosenPetsDecode[i]);
            pets[i] = GetPetData(_chosenPet[i]).pet;
        }
        _petToSpawn = _chosenPet[0];
        character = GetPetData(_petToSpawn).pet;
        image.sprite = GetPetData(_petToSpawn).buttonImage;
      // character.GetComponent<SmartEnemyGrounded>().startBehavior = STARTBEHAVIOR.WALK_LEFT;
      // character.layer = LayerMask.NameToLayer("Player");
      // character.tag = "Warrior";
      // character.GetComponent<EnemyMeleeAttack>().targetLayer = 31;
      // character.GetComponent<EnemyMeleeAttack>().targetLayer = LayerMask.NameToLayer("Enemy");
      // character.GetComponent<CheckTargetHelper>().targetLayer = 31;
      // character.GetComponent<CheckTargetHelper>().targetLayer = LayerMask.NameToLayer("Enemy");
        
        ownBtn = GetComponent<Button>();
        ownBtn.onClick.AddListener(OnBtnClick);

        poisonFX.SetActive(characterID.weaponEffect.effectType == WEAPON_EFFECT.POISON);
        freezeFX.SetActive(characterID.weaponEffect.effectType == WEAPON_EFFECT.FREEZE);

        listCharacters = new List<GameObject>();

        //if (GlobalValue.levelPlaying >= characterID.unlockAtLevel)
        //{
        isUnlocked = true;
        //}

        if (isUnlocked)
        {
            priceTxt.text = price + "";
            //unlockLevelTxt.enabled = false;
            InvokeRepeating("CheckAvailable", 0, 0.1f);
        }
        else
        {
            priceTxt.text = "LOCKED";
            //unlockLevelTxt.text = "" + characterID.unlockAtLevel;
            ownBtn.interactable = false;
        }

        lockIcon.SetActive(!isUnlocked);

        if (image == null)
            image = GetComponent<Image>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        allowWork = false;
        coolDownCounter = delayOnStart;
    }

    int numberCharacterAlive()
    {
        int alives = 0;
        foreach (var cha in listCharacters)
        {
            if (cha.activeInHierarchy)
                alives++;
        }
        return alives;
    }

    void Update()
    {
        //update number 
        numberTxt.text = numberCharacterAlive() + "/" + max;
        if (!allowWork)
        {
            coolDownCounter -= Time.deltaTime;

            if (coolDownCounter <= 0)
                allowWork = true;
        }
        else
        {
            holdCounter -= Time.deltaTime;
        }

        image.fillAmount = Mathf.Clamp01((coolDown - coolDownCounter) / coolDown);

        canvasGroup.interactable = coolDownCounter <= 0;

        canUse = canvasGroup.interactable && canvasGroup.blocksRaycasts;
    }

    void CheckAvailable()
    {
        ownBtn.interactable = User.Coin >= price && numberCharacterAlive() < max;
    }

    private void OnBtnClick()
    {
        if (!canUse)
            return;

        if (!allowWork)
            return;

        if (GameManager.Instance.currentExp >= price)
        {
            //SoundManager.PlaySfx(SoundManager.Instance.buyCharacter);
            CharacterManager.Instance.SpawnCharacter(this, targetLayerForPet);
            GlobalValue.BoughtGroundUnit++;
        }
    }

    public void AddCharacter(GameObject character)
    {
        listCharacters.Add(character);
        allowWork = false;
        coolDownCounter = coolDown;
        GameManager.Instance.currentExp -= price;
    }
    ShopItemData.ShopItem GetPetData(int id)
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
