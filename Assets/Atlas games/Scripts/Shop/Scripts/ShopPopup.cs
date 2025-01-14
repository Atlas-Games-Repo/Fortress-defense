using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPopup : MonoBehaviour
{
    public GameObject notEnoughCoinText;
    public GameObject maxAmountText;
    public Animator popupAnimator;


    public void OpenDialog(bool notEnoughCoins)
    {
        popupAnimator = GetComponent<Animator>();
        popupAnimator.SetTrigger("Open");
        if (notEnoughCoins)
        {
            notEnoughCoinText.SetActive(true);
            maxAmountText.SetActive(false);
        }
        else
        {
            maxAmountText.SetActive(true);
            notEnoughCoinText.SetActive(false);
        }
    }

    public void CloseDialog()
    {
        popupAnimator.SetTrigger("Close");
    }
    
    public void DisablePopup()
    {
        Invoke("Deactivate", 0.1f);
    }
    void Deactivate()
    {
        // Deactivate the GameObject this script is attached to
        gameObject.SetActive(false);
    }
}
