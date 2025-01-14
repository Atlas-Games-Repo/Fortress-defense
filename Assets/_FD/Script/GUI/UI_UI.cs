using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UI : MonoBehaviour
{
    public float lerpSpeed = 1;
    public float shieldLerpSpeed = 4;
    [Header("PLAYER HEALTHBAR")]
    public Slider healthSlider;
    public Text health;
    float healthValue;
    
    [Header("Shield HEALTHBAR")]
    public Slider shieldHealthSlider;
    public Text shieldHealth;
    float _shieldhealthValue;
    float _shieldHealthValue;
    //[Header("ENEMY HEALTHBAR")]
    //public Slider enemyHealthSlider;
    //public Text enemyHealth;
    //float enemyHealthValue;

    [Header("ENEMY WAVE")]
    public Slider enemyWavePercentSlider;
    float enemyWaveValue;

    [Space]
    public Text coinTxt;
    public Text expTxt;
    public Text levelName;

  //  void OnEnable()
  //  {
  //      MenuManager.Instance.OnSceneReloaded += DeactivateShield;
  //  }

  // void OnDisable()
  // {
  //     MenuManager.Instance.OnSceneReloaded -= DeactivateShield;
  // }
    private void Start()
    {
        Invoke("DeactivateShield",Time.deltaTime);
        healthValue = 1;
        enemyWaveValue = 0;
        healthSlider.value = 1;
        healthSlider.gameObject.SetActive(false);
        enemyWavePercentSlider.value = 0;
        levelName.text = "Level " + GlobalValue.levelPlaying;
    }

    private void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, healthValue, lerpSpeed * Time.deltaTime);
        shieldHealthSlider.value = Mathf.Lerp(shieldHealthSlider.value, _shieldhealthValue, shieldLerpSpeed * Time.deltaTime);

        //enemyHealthSlider.value = Mathf.Lerp(enemyHealthSlider.value, enemyHealthValue, lerpSpeed * Time.deltaTime);
        
        enemyWavePercentSlider.value = Mathf.Lerp(enemyWavePercentSlider.value, enemyWaveValue, lerpSpeed * Time.deltaTime);
        coinTxt.text = User.Coin + "";
        expTxt.text = GameManager.Instance.currentExp + "";
    }

    public void UpdateHealthbar(float currentHealth, float maxHealth/*, HEALTH_CHARACTER healthBarType*/)
    {
        //if (healthBarType == HEALTH_CHARACTER.PLAYER)
        //{
            healthValue = Mathf.Clamp01(currentHealth / maxHealth);
            health.text = (int)currentHealth + "/" + (int)maxHealth;
        //}
        //else if(healthBarType == HEALTH_CHARACTER.ENEMY)
        //{
        //    enemyHealthValue = Mathf.Clamp01(currentHealth / maxHealth);
        //    enemyHealth.text = (int) currentHealth + "/" + (int)maxHealth;
        //}
    }

    public void ActivateShield(float currentShieldHealth, float maxShieldHealth)
    {
        health.gameObject.SetActive(false);
        healthSlider.gameObject.SetActive(false);
        shieldHealth.gameObject.SetActive(true);
        shieldHealthSlider.gameObject.SetActive(true);
        _shieldhealthValue = Mathf.Clamp01(currentShieldHealth / maxShieldHealth);
        shieldHealth.text = (int)currentShieldHealth + "/" + (int)maxShieldHealth;
    }

    public void DeactivateShield()
    {
        health.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(true);
        shieldHealth.gameObject.SetActive(false);
        shieldHealthSlider.gameObject.SetActive(false);
    }
    public void UpdateShieldHealthBar(float currentShieldHealth, float maxShieldHealth/*, HEALTH_CHARACTER healthBarType*/)
    {
        
        _shieldhealthValue = Mathf.Clamp01(currentShieldHealth / maxShieldHealth);
        shieldHealth.text = (int)currentShieldHealth + "/" + (int)maxShieldHealth;
      
    }
    public void UpdateEnemyWavePercent(float currentSpawn, float maxValue)
    {
        enemyWaveValue = Mathf.Clamp01(currentSpawn / maxValue);
    }
    
   
}
