using UnityEngine;

[CreateAssetMenu(fileName = "NewRewardList", menuName = "ScriptableObjects/RewardList", order = 1)]
public class RewardList : ScriptableObject
{
    public Sprite coinIcon; // Add the coin icon
    public Sprite expIcon;  
    public Reward[] rewards;
}