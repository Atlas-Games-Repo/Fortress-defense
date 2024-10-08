using System;
using System.Collections;
using System.Collections.Generic;
using DynamicScrollRect;
using UnityEngine;

public class Shop : MonoBehaviour
{

    public DynamicScrollRect.DynamicScrollRect dynamicScrollRect;
    public ScrollContent content;
    public Transform scrollContentParent;
    public enum ItemTypes
    {
        Pet,Item,Magic,Monster,Website,Towers,Timed
    }

    public enum ItemPurchaseType
    {
        Exp,Coin
    }

    public ShopItemData data;
    ItemTypes _chosenType;
    public void OpenMenu(string menuName)
    {
        SoundManager.Instance.PauseMusic(true);
        switch (menuName)
        {
            case "pets":
                _chosenType = ItemTypes.Pet;
                break;
            case "items":
                _chosenType = ItemTypes.Item;
                break;
            case "magics":
                _chosenType = ItemTypes.Magic;
                break;
            case "monsters":
                _chosenType = ItemTypes.Monster;
                break;
            case "website":
                _chosenType = ItemTypes.Website;
                break;
            case "tower":
                _chosenType = ItemTypes.Towers;
                break;
        }
        List<ScrollItemData> contentDatas = new List<ScrollItemData>();
        for (int i = 0; i < data.ShopData.Length; i++)
        {
            if (data.ShopData[i].type == _chosenType && !data.ShopData[i].isFree)
            {
                contentDatas.Add(new ScrollItemData(data.ShopData[i]));
            }
        }

        for (int i = 0; i < scrollContentParent.childCount; i++)
        {
            if (scrollContentParent.GetChild(i).name != "RefItem")
            {
                Destroy(scrollContentParent.GetChild(i).gameObject);
            }            
        }

        if (contentDatas.Count > 0)
        {
            dynamicScrollRect.vertical = true;
            content.InitScrollContent(contentDatas);
        }
        else
        {
            dynamicScrollRect.vertical = false;
        }
    }
    
    //delete
    void Start()
    {
        User.Coin = 1000;
    }
}
