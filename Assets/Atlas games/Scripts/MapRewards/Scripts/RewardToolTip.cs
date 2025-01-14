using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class RewardToolTip : MonoBehaviour
{
    public Image rewardImage;
    public GameObject rewardTooltip;
   // public GameObject rewardIcon;
    public GameObject rewardTick;
    public GameObject mysteryGiftIcon;
    private int _levelsUnlocked;
    private int _levelToReward;
    private Vector2 _initialSize;
    public RewardList rewardList;
    private Reward _currentReward;
    private NativeAspectRatio _rewardImageScaler;
    public Text amountText;
    private Animator _rewardAnimator;
    private bool _hasReward;
    private bool _isOpen;
    public float rewardToolTipDelay = 1f;
    void Start()
    {
        _rewardAnimator = rewardTooltip.GetComponent<Animator>();
        _rewardImageScaler = rewardImage.GetComponent<NativeAspectRatio>();
        List<int> levelsDefined = new List<int>();
        foreach (Reward reward in rewardList.rewards)
        {
            levelsDefined.Add(reward.rewardLevel);
        }
        _levelToReward = GetComponent<Level>().level;
        _levelsUnlocked = GlobalValue.LevelPass;
        if (levelsDefined.Contains(_levelToReward))
        {
            for (int i = 0; i < rewardList.rewards.Length; i++)
            {
                if (_levelToReward == rewardList.rewards[i].rewardLevel)
                {
                    _hasReward = true;
                    _currentReward = rewardList.rewards[i];
                }
            }
        }
    }
    
    private void OnEnable()
    {
        MapControllerUI.OnMapChange += OnElementEnteredCanvas; 
    }

    private void OnDisable()
    {
        MapControllerUI.OnMapChange -= OnElementEnteredCanvas; 
    }


    void OpenTooltip()
    {
        if (GlobalValue.LevelPass >= _levelToReward)
        {
            _rewardImageScaler.ChangeImage(_currentReward.icon);
            mysteryGiftIcon.SetActive(false);
            amountText.gameObject.SetActive(true);
            amountText.text = "x" + _currentReward.amount;
            rewardTooltip.SetActive(true);
            rewardImage.gameObject.SetActive(true);
            rewardTick.SetActive(true);
        }
        else
        {
            amountText.gameObject.SetActive(false);
            rewardImage.gameObject.SetActive(false);
            rewardTick.SetActive(false);
            mysteryGiftIcon.SetActive(true);
        }
        _rewardAnimator.SetTrigger("Open");
    }
    

    private void OnElementEnteredCanvas(int enteredMap, bool isDelayed)
    {
        if (_hasReward)
        {
            if (enteredMap == _levelToReward/11 && !_isOpen)
            {
                _isOpen = true;
                Invoke("OpenTooltip",isDelayed?rewardToolTipDelay:0f);
            }
            else if(_isOpen)
            {
                _isOpen = false;
                _rewardAnimator.SetTrigger("Close");
            }
        }
    }
}
