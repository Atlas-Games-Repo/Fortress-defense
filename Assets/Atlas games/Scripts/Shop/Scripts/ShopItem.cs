using System.Collections;
using System.Collections.Generic;
using DynamicScrollRect;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : ScrollItem<ScrollItemData>
{
    public DynamicScrollRect.DynamicScrollRect dynamicScroll = null;
    [HideInInspector] public string itemName;
    [HideInInspector] public int itemPrice;
    [HideInInspector] public bool hasMaxValue;
    [HideInInspector] public int maxAmount;
    public Text itemPriceText;
    public Text itemCountText;
    public Text itemNameText;
    public Button purchaseButton;
    public Image itemImage;
    public ShopPopup popup;
    public GameObject coinImage;
    public GameObject expImage;
    private Shop.ItemPurchaseType _purchaseType;
    public Text levelLockText;
    public GameObject itemCountTag;
    public GameObject levelLockBG;
    private bool _oneTimePurchase;
    public GameObject ownedText;
    protected override void InitItemData(ScrollItemData data)
    {
        Init(data.Data);
        base.InitItemData(data);
    }

    // Start is called before the first frame update
    public void Init(ShopItemData.ShopItem itemData)
    {
        itemCountTag.SetActive(false);
        _oneTimePurchase = itemData.oneTimePurchase;
        hasMaxValue = itemData.hasMaxValue;
        maxAmount = itemData.itemMaxValue;
        itemImage.sprite = itemData.itemImage;
        itemPrice = itemData.itemPrice;
        itemName = itemData.itemName;
        _purchaseType = itemData.purchaseType;
        if (itemData.levelLock)
        {
            if (GlobalValue.LevelPass < itemData.levelToUnlock - 1)
            {
                purchaseButton.interactable = false;
                itemNameText.gameObject.SetActive(false);
                itemCountTag.SetActive(false);
                levelLockBG.SetActive(true);
                levelLockText.text = "Reach Level " + itemData.levelToUnlock + " To Unlock!";
            }
            else
            {
                purchaseButton.interactable = true;
                itemNameText.gameObject.SetActive(true);
                if (!itemData.oneTimePurchase)
                {
                    itemCountTag.SetActive(true);
                }
                levelLockBG.SetActive(false );
                if (_purchaseType == Shop.ItemPurchaseType.Coin)
                {
                    coinImage.SetActive(true);
                    expImage.SetActive(false);
                }
                else
                {
                    expImage.SetActive(true);
                    coinImage.SetActive(false);
                }

                Vector2 originalSize =
                    new Vector2(itemImage.rectTransform.sizeDelta.x, itemImage.rectTransform.sizeDelta.y);
                itemImage.SetNativeSize();
                if (itemImage.rectTransform.sizeDelta.x > itemImage.rectTransform.sizeDelta.y)
                {
                    float aspectRatio = (float)itemImage.rectTransform.sizeDelta.x / itemImage.rectTransform.sizeDelta.y;
                    itemImage.rectTransform.sizeDelta = new Vector2(originalSize.x, originalSize.y / aspectRatio);
                }
                else
                {
                    float aspectRatio = (float)itemImage.rectTransform.sizeDelta.y / itemImage.rectTransform.sizeDelta.x;
                    itemImage.rectTransform.sizeDelta = new Vector2(originalSize.x / aspectRatio, originalSize.y);
                }

                itemPriceText.text = itemPrice.ToString();
                int itemCount = GlobalValue.GetChosenShopItem(itemName);
                itemNameText.text = itemName;
                itemCountText.text = itemCount.ToString();
            }
        }
        else
        {
            purchaseButton.interactable = true;
            itemNameText.gameObject.SetActive(true);
            if (!itemData.oneTimePurchase)
            {
                itemCountTag.SetActive(true);
            }
            levelLockBG.SetActive(false );
            if (_purchaseType == Shop.ItemPurchaseType.Coin)
            {
                coinImage.SetActive(true);
                expImage.SetActive(false);
            }
            else
            {
                expImage.SetActive(true);
                coinImage.SetActive(false);
            }

            Vector2 originalSize =
                new Vector2(itemImage.rectTransform.sizeDelta.x, itemImage.rectTransform.sizeDelta.y);
            itemImage.SetNativeSize();
            if (itemImage.rectTransform.sizeDelta.x > itemImage.rectTransform.sizeDelta.y)
            {
                float aspectRatio = (float)itemImage.rectTransform.sizeDelta.x / itemImage.rectTransform.sizeDelta.y;
                itemImage.rectTransform.sizeDelta = new Vector2(originalSize.x, originalSize.y / aspectRatio);
            }
            else
            {
                float aspectRatio = (float)itemImage.rectTransform.sizeDelta.y / itemImage.rectTransform.sizeDelta.x;
                itemImage.rectTransform.sizeDelta = new Vector2(originalSize.x / aspectRatio, originalSize.y);
            }

            itemPriceText.text = itemPrice.ToString();
            int itemCount = GlobalValue.GetChosenShopItem(itemName);
            itemNameText.text = itemName;
            itemCountText.text = itemCount.ToString();
        }

        if (_oneTimePurchase && GlobalValue.GetChosenShopItem(itemName)>0)
        {
            expImage.gameObject.SetActive(false);
            coinImage.gameObject.SetActive(false);
            itemPriceText.text = "";
            ownedText.SetActive(true);
        }
        else
        {
            ownedText.SetActive(false);
        }

        if (itemData.type == Shop.ItemTypes.Timed)
        {
            GetComponent<TimedItemManager>().Init(itemData.itemName,itemData.duration);
            GetComponent<ItemActivator>().itemName = itemData.itemName;
            ownedText.SetActive(false);
        }
    }

    public void BuyItem()
    {
        int itemCount = GlobalValue.GetChosenShopItem(itemName);
        SoundManager.Click();

        if (!_oneTimePurchase)
        {
               if (itemCount < maxAmount)
            {
                if (_purchaseType == Shop.ItemPurchaseType.Coin)
                {
                    if (User.Coin > itemPrice)
                    {
                        if (hasMaxValue)
                        {
                            if (itemCount < maxAmount)
                            {
                                GlobalValue.IncrementChosenShopItem(itemName);
                                itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                                User.Coin = -itemPrice;
                            }
                            else
                            {
                                popup.gameObject.SetActive(true);
                                popup.OpenDialog(false);
                            }
                        }
                        else
                        {
                            GlobalValue.IncrementChosenShopItem(itemName);
                            itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                            User.Coin = -itemPrice;
                        }
                    }
                    else
                    {
                        popup.gameObject.SetActive(true);
                        popup.OpenDialog(true);
                    }
                }
                else
                {
                    if (User.Uxp > itemPrice)
                    {
                        if (hasMaxValue)
                        {
                            if (itemCount < maxAmount)
                            {
                                GlobalValue.IncrementChosenShopItem(itemName);
                                itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                                User.Uxp = -itemPrice;
                            }
                            else
                            {
                                popup.gameObject.SetActive(true);
                                popup.OpenDialog(false);
                            }
                        }
                        else
                        {
                            GlobalValue.IncrementChosenShopItem(itemName);
                            itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                            User.Uxp = -itemPrice;
                        }
                    }
                    else
                    {
                        popup.gameObject.SetActive(true);
                        popup.OpenDialog(true);
                    }
                }
            }
            else
            {
                popup.gameObject.SetActive(true);
                popup.OpenDialog(false);
            }
        
        }
        else
        {
            if (GlobalValue.GetChosenShopItem(itemName) < 1)
            {
                  if (_purchaseType == Shop.ItemPurchaseType.Coin)
                {
                    if (User.Coin > itemPrice)
                    {
                        if (hasMaxValue)
                        {
                            if (itemCount < maxAmount)
                            {
                                expImage.gameObject.SetActive(false);
                                coinImage.gameObject.SetActive(false);
                                itemPriceText.text = "";
                                            ownedText.SetActive(true);
                                GlobalValue.IncrementChosenShopItem(itemName);
                                itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                                User.Coin = -itemPrice;
                            }
                            else
                            {
                                popup.gameObject.SetActive(true);
                                popup.OpenDialog(false);
                            }
                        }
                        else
                        {
                            expImage.gameObject.SetActive(false);
                            coinImage.gameObject.SetActive(false);
                            itemPriceText.text = "";
                                        ownedText.SetActive(true);
                            GlobalValue.IncrementChosenShopItem(itemName);
                            itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                            User.Coin = -itemPrice;
                        }
                    }
                    else
                    {
                        popup.gameObject.SetActive(true);
                        popup.OpenDialog(true);
                    }
                }
                else
                {
                    if (User.Uxp > itemPrice)
                    {
                        if (hasMaxValue)
                        {
                            if (itemCount < maxAmount)
                            {
                            expImage.gameObject.SetActive(false);
            coinImage.gameObject.SetActive(false);
            itemPriceText.text = "";
                        ownedText.SetActive(true);
                                GlobalValue.IncrementChosenShopItem(itemName);
                                itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                                User.Uxp = -itemPrice;
                            }
                            else
                            {
                                popup.gameObject.SetActive(true);
                                popup.OpenDialog(false);
                            }
                        }
                        else
                        {
                        expImage.gameObject.SetActive(false);
            coinImage.gameObject.SetActive(false);
            itemPriceText.text = "";
                        ownedText.SetActive(true);
                            GlobalValue.IncrementChosenShopItem(itemName);
                            itemCountText.text = GlobalValue.GetChosenShopItem(itemName).ToString();
                            User.Uxp = -itemPrice;
                        }
                    }
                    else
                    {
                        popup.gameObject.SetActive(true);
                        popup.OpenDialog(true);
                    }
                }
        
            }
        }
    }

    public void HandleDeactivation()
    {
        if (User.Coin < itemPrice)
        {
        }
        else if (hasMaxValue && maxAmount == GlobalValue.GetChosenShopItem(itemName))
        {
            popup.OpenDialog(false);
        }
    }
}