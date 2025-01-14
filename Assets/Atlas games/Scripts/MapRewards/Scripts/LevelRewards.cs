using System;
using UnityEngine;

[Serializable]
public enum RewardType
{
    Coin,
    Exp,
    ShopItem
}

[Serializable]
public class Reward
{
    public int rewardLevel;
    public RewardType type;
    public string shopItemName; 
    public int amount;
    public Sprite icon;
}