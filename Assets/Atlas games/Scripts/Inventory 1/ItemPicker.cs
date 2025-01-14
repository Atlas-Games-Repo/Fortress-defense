using System.Collections;
using System.Collections.Generic;
using DynamicScrollRect;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    public DynamicScrollRect.DynamicScrollRect dynamicScrollRect;
    public ScrollContent content;
    public Transform scrollContentParent;
    [HideInInspector]public List<ScrollItemData> petData;
    public GameObject noItemsMessage;
    public GameObject regularButton;
    public void Init(ShopItemData data,int[] chosenItems, Shop.ItemTypes category)
    {
        ClearItems();
        List<ScrollItemData> contentDatas = new List<ScrollItemData>();
        for (int i = 0; i < data.ShopData.Length; i++)
        {

            if (!data.ShopData[i].isTimed)
            {
                if (category != Shop.ItemTypes.Pet)
                {
                    if (data.ShopData[i].type == category && GlobalValue.GetChosenShopItem(data.ShopData[i].itemName)>0)
                    {
                        contentDatas.Add(new ScrollItemData(data.ShopData[i]));
                    }
                }
                else
                {
                    if (data.ShopData[i].type == Shop.ItemTypes.Pet)
                    {
                        if (GlobalValue.GetChosenShopItem(data.ShopData[i].itemName) > 0||data.ShopData[i].isFree)
                        {
                            contentDatas.Add(new ScrollItemData(data.ShopData[i]));
                        }
                        }
                    }
                }
            }

        if (contentDatas.Count > 0)
        {
            content.InitScrollContent(contentDatas);
            petData = contentDatas;
            noItemsMessage.SetActive(false);
            regularButton.SetActive(true);
        }else if (category == Shop.ItemTypes.Item)
        {
            noItemsMessage.SetActive(true);
            regularButton.SetActive(false);
        }
    
        
    }

    public void ClearItems()
    {
        for (int i = 0; i < scrollContentParent.childCount; i++)
        {
            if (scrollContentParent.GetChild(i).name != "RefItem")
            {
                Destroy(scrollContentParent.GetChild(i).gameObject);
            }            
        }
    }
}
