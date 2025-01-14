using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum HEALTH_CHARACTER { PLAYER, ENEMY}

[System.Serializable]
public class FortrestLevel
{
    public float maxHealth = 1000;
    public Sprite[] stateFortrestSprites;
    public int fortressId;
}

public class TheFortrest : MonoBehaviour, ICanTakeDamage
{
    //public HEALTH_CHARACTER healthCharacter;
    public FortrestLevel[] fortrestLevels;

    [ReadOnly] public int fortrestLevel = 1;
    public int[] enemyFortrestHealth;

    [HideInInspector] public float maxHealth;
    Sprite[] stateFortrestSprites;

    [ReadOnly] public float extraHealth = 0;
    [ReadOnly] public float currentHealth;


    public SpriteRenderer fortrestSprite;

    [Header("SHAKNG")] public float speed = 30f; //how fast it shakes
    public float amount = 0.5f; //how much it shakes
    public float shakeTime = 0.3f;
    public int floatingTextFontSize = 70;
    public bool shakeX, shakeY;

    private float _startingHealth;
    Vector2 startingPos;
    IEnumerator ShakeCoDo;

    [Header("Shield")] [HideInInspector] public bool shield = false;

    [Tooltip("Propper order is from min health to max health bubbles")]
    public GameObject[] shieldHealthBubbles;
    // public GameObject shieldBubbleMaxHealth;
    // public GameObject shieldBubbleMedHealth;
    // public GameObject shieldBubbleMinHealth;
    public float[] shieldHealthLevels;
    public bool useDefaultShieldHealthLevels = true;
    private float _shieldCurrentHealth;
    private float _maxShieldHealth;
    public ShopItemData data;


    void Awake()
    {
        startingPos = transform.position;
      
        
        if (useDefaultShieldHealthLevels)
        {
            shieldHealthLevels = new float[shieldHealthBubbles.Length];
            for (int i = 0; i < shieldHealthLevels.Length; i++)
            {
                shieldHealthLevels[i] = (maxHealth/shieldHealthLevels.Length) * (i);
            }
        }

        //defaultLevel = healthCharacter == HEALTH_CHARACTER.PLAYER ? GlobalValue.UpgradeStrongWall : defaultFortrest - 1;
        //if (healthCharacter == HEALTH_CHARACTER.PLAYER)
        //{
        int chosenTower = int.Parse(GlobalValue.inventoryTowers);
        for (int i = 0; i < fortrestLevels.Length; i++)
        {
            if (fortrestLevels[i].fortressId == chosenTower)
            {
                maxHealth = fortrestLevels[i].maxHealth;
                stateFortrestSprites = fortrestLevels[i].stateFortrestSprites;
                fortrestSprite.sprite = stateFortrestSprites[0];
            }
        }
      //  stateFortrestSprites = fortrestLevels[GlobalValue.UpgradeStrongWall].stateFortrestSprites;
      //  fortrestSprite.sprite = stateFortrestSprites[0];
        //}
        //else
        //{
        //    maxHealth = fortrestLevels[GameLevelSetup.Instance.GetEnemyFortrestLevel() - 1].maxHealth;
        //    stateFortrestSprites = fortrestLevels[GameLevelSetup.Instance.GetEnemyFortrestLevel() - 1].stateFortrestSprites;
        //    fortrestSprite.sprite = stateFortrestSprites[0];
        //}
    }

    IEnumerator ShakeCo(float time)
    {
        float counter = 0;
        while (counter < time)
        {
            transform.position = startingPos + new Vector2(Mathf.Sin(Time.time * speed) * amount * (shakeX ? 1 : 0),
                Mathf.Sin(Time.time * speed) * amount * (shakeY ? 1 : 0));

            yield return null;
            counter += Time.deltaTime;
        }

        transform.position = startingPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        DeActivateShield();
        extraHealth = maxHealth * GlobalValue.StrongWallExtra;
        maxHealth += extraHealth;
        currentHealth = maxHealth;
        _startingHealth = currentHealth;
        MenuManager.Instance.UpdateHealthbar(currentHealth, maxHealth /*, healthCharacter*/);
    }

    public void ActivateShield(float shieldHP)
    {
        shieldHealthBubbles[shieldHealthBubbles.Length-1].SetActive(true);
        shield = true;
        _maxShieldHealth = shieldHP;
        _shieldCurrentHealth = shieldHP;
        MenuManager.Instance.ActivateShield(shieldHP, shieldHP /*, healthCharacter*/);
    }

    public void DeActivateShield()
    {
        shieldHealthBubbles[0].SetActive(false);
        shield = false;
        MenuManager.Instance.DeactivateShield();
    }

    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator,
        BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null,
        WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        if (shield)
        {
            _shieldCurrentHealth -= damage;
            FloatingTextManager.Instance.ShowText("" + (int)damage, Vector2.up * 2, Color.red, transform.position,
                floatingTextFontSize);
            MenuManager.Instance.UpdateShieldHealthbar(_shieldCurrentHealth, _maxShieldHealth /*, healthCharacter*/);
            if (ShakeCoDo != null)
                StopCoroutine(ShakeCoDo);

            ShakeCoDo = ShakeCo(shakeTime);
            StartCoroutine(ShakeCoDo);
            if (_shieldCurrentHealth > shieldHealthLevels[shieldHealthLevels.Length - 1])
            {
                shieldHealthBubbles[shieldHealthBubbles.Length-1].SetActive(true);
            }
            else
            {
                for (int i = 1; i < shieldHealthBubbles.Length; i++)
                {
                    if (_shieldCurrentHealth < shieldHealthLevels[i] && _shieldCurrentHealth >= shieldHealthLevels[i-1])
                    {
                        shieldHealthBubbles[i].SetActive(true);
                    }
                    else
                    {
                        shieldHealthBubbles[i].SetActive(false);
                    }
                }
            }
            if (_shieldCurrentHealth <= 0)
            {
                DeActivateShield();
            }
            else
            {
                if (ShakeCoDo != null)
                    StopCoroutine(ShakeCoDo);

                ShakeCoDo = ShakeCo(shakeTime);
                StartCoroutine(ShakeCoDo);

                //shake the camera
                CameraShake.instance.StartShake(0.1f, 0.1f);
            }
        }
        else
        {
            currentHealth -= damage;
            FloatingTextManager.Instance.ShowText("" + (int)damage, Vector2.up * 2, Color.yellow, transform.position,
                floatingTextFontSize);

            MenuManager.Instance.UpdateHealthbar(currentHealth, maxHealth /*, healthCharacter*/);

            if (currentHealth <= 0)
            {
                //if (healthCharacter == HEALTH_CHARACTER.PLAYER)
                GameManager.Instance.GameOver();
                //else
                //    GameManager.Instance.Victory();
            }
            else
            {
                if (ShakeCoDo != null)
                    StopCoroutine(ShakeCoDo);

                ShakeCoDo = ShakeCo(shakeTime);
                StartCoroutine(ShakeCoDo);

                //shake the camera
                CameraShake.instance.StartShake(0.1f, 0.1f);
            }

            //update fortrest state
            UpdateFortressState();
        }
    }

    public void HealFortress(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > _startingHealth)
        {
            currentHealth = _startingHealth;
        }

        UpdateFortressState();
    }
  //  void OnEnable()
  //  {
  //      MenuManager.Instance.OnSceneReloaded += ResetShield;
  //  }
//
  //  void OnDisable()
  //  {
  //       MenuManager.Instance.OnSceneReloaded -= ResetShield;
  //  }

    void ResetShield()
    {
        for (int i = 0; i < shieldHealthBubbles.Length; i++)
        {
            shieldHealthBubbles[i].SetActive(false);
        }
        shield = false;
        MenuManager.Instance.DeactivateShield();
    }
    void UpdateFortressState()
    {
        MenuManager.Instance.UpdateHealthbar(currentHealth, maxHealth /*, healthCharacter*/);
        if (currentHealth > 0)
        {
            for (int i = (stateFortrestSprites.Length - 1); i > 0; i--)
            {
                if (currentHealth < ((maxHealth / (stateFortrestSprites.Length - 1)) * i))
                {
                    fortrestSprite.sprite = stateFortrestSprites[(stateFortrestSprites.Length - 1) - i];
                }
            }
        }
        else
            fortrestSprite.sprite = stateFortrestSprites[stateFortrestSprites.Length - 1];
    }
}