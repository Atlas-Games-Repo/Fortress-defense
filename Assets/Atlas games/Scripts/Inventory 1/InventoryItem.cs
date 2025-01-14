using System.Collections;
using System.Collections.Generic;
using DynamicScrollRect;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : ScrollItem<ScrollItemData>
{
    public Image itemImage;
    public Inventory inventory;
    public GameObject selectedHighlight;
    private ShopItemData.ShopItem _itemData;
    private bool _isChosen;
    private Vector2 originalSize;
    void Init(ShopItemData.ShopItem data)
    {
        selectedHighlight.SetActive(false);
        itemImage.sprite = data.itemImage;
        _itemData = data;
        switch (data.type)
        {
            case Shop.ItemTypes.Towers:
                for (int i = 0; i < inventory.chosenTower.Length; i++)
                {
                    if (data.id == inventory.chosenTower[i])
                    {
                        _isChosen = true;
                        break;
                    }
                    else
                    {
                        _isChosen = false;
                    }
                }
                break;
            case Shop.ItemTypes.Magic:
                for (int i = 0; i < inventory.chosenMagics.Length; i++)
                {
                    if (data.id == inventory.chosenMagics[i])
                    {
                        _isChosen = true;
                        break;
                    }
                    else
                    {
                        _isChosen = false;
                    }
                }
                break;
            case Shop.ItemTypes.Item:
                for (int i = 0; i < inventory.chosenItems.Length; i++)
                {
                    if (data.id == inventory.chosenItems[i])
                    {
                        _isChosen = true;
                        break;
                    }
                    else
                    {
                        _isChosen = false;
                    }
                }
                break;
            case Shop.ItemTypes.Pet:
                for (int i = 0; i < inventory.chosenPet.Length; i++)
                {
                    if (data.id == inventory.chosenPet[i])
                    {
                        _isChosen = true;
                        break;
                    }
                    else
                    {
                        _isChosen = false;
                    }
                }
                break;
        }
        selectedHighlight.SetActive(_isChosen);
         originalSize =
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
    }
    protected override void InitItemData(ScrollItemData data)
    {
        Init(data.Data);
        base.InitItemData(data);
    }
    public void ChooseItem()
    {
        if (!_isChosen)
        {
            inventory.ChangeChosenItem(_itemData);
            SoundManager.Click(); 
        }
        else
        {
            SoundManager.Click();
        }
    }
}
