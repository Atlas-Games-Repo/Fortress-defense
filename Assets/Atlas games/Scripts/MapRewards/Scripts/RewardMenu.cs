using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
public class RewardMenu : MonoBehaviour
{
    private Animator _anim;
    private MenuManager _menuManager;
    private int _rewardAmount;
    public Image rewardImage;
    public float rewardAmountLerpingTime = 0.3f;
    public Text rewardAmountText;
    private string _itemName;
    public Text rewardItemName;
    private Reward _currentReward;
    public Animator claimButtonAnimator;
    public float prizeClaimDelay = 1.5f;
    public float delayForTextLerp = 0.53f;    
    public ParticleSystem[] confettiFx;
    public AudioClip prizeSource;
    
    IEnumerator DelayBeforeClaimButton()
    {
        yield return new WaitForSeconds(prizeClaimDelay);
        claimButtonAnimator.SetTrigger("Grow");
    }
    public void Init(Reward reward,MenuManager menuManager)
    {
     StartCoroutine(DelayBeforeClaimButton());
     _currentReward = reward;
     _itemName = reward.shopItemName;
     rewardItemName.text = _itemName;
     _menuManager = menuManager;
     _anim = GetComponent<Animator>();
     _anim.SetTrigger("Open");
     foreach (ParticleSystem confetti in confettiFx)
     {
         confetti.Play();
     }
     GetComponent<AudioSource>().clip = prizeSource;
     GetComponent<AudioSource>().Play();
     rewardImage.GetComponent<NativeAspectRatio>().ChangeImage(reward.icon);
     _rewardAmount = reward.amount;
     StartCoroutine(CountRewardAmount(_rewardAmount));
    }

    public void ClaimReward()
    {

        switch (_currentReward.type)
        {
            case RewardType.Coin:
                User.Coin = _rewardAmount;
                break;
            case RewardType.Exp:
                User.Rxp = _rewardAmount;
                break;
            case RewardType.ShopItem:
                // this is present in the shop branch, remove the comments after merging with Update !
                // GlobalValue.IncrementChosenShopItem(_itemName);
                break;
        }
        
        SoundManager.Instance.ClickBut();
        _menuManager.OpenVictoryMenu();
    }

    IEnumerator CountRewardAmount(int targetAmount)
    {
        yield return new WaitForSeconds(delayForTextLerp);
        float addedAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime<= rewardAmountLerpingTime)
        {
            elapsedTime+= Time.deltaTime;
            float progress = elapsedTime/rewardAmountLerpingTime;
            addedAmount = Mathf.Lerp(0f, targetAmount, progress);
            rewardAmountText.text = "x" + addedAmount;
            yield return null;
        }
        rewardAmountText.text = "x" + targetAmount;
    }
}
