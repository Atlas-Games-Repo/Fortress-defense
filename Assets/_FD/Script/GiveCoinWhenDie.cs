using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCoinWhenDie : MonoBehaviour
{
    public int coinGiveMin = 5;
    public int coinGiveMax = 10;
    [Space(3)] [Header("NightMode")] public int customNightMultiplier = 2;
    public bool useCustomNightMultiplierOnly = false;
    void Start()
    {
        int initialMinCoin = coinGiveMin;
        int initialMaxCoin = coinGiveMax;
        if (GameLevelSetup.Instance && GameLevelSetup.Instance.NightMode())
        {
            if (useCustomNightMultiplierOnly)
            {
                coinGiveMax = Mathf.RoundToInt(initialMaxCoin * customNightMultiplier);
                coinGiveMin = Mathf.RoundToInt(initialMinCoin * customNightMultiplier);
            }
            else
            {
                coinGiveMax =  Mathf.RoundToInt(GameLevelSetup.Instance.NightModeXpMultiplier() * initialMaxCoin);
                coinGiveMin = Mathf.RoundToInt(GameLevelSetup.Instance.NightModeXpMultiplier() * initialMinCoin);
            }
        }
    }
    public void GiveCoin()
    {
        SoundManager.PlaySfx(SoundManager.Instance.coinCollect);
        int random = Random.Range(coinGiveMin, coinGiveMax);
        User.Coin = random;

        FloatingTextManager.Instance.ShowText((int)random + "", Vector2.up * 1, Color.yellow, transform.position);
    }
}
