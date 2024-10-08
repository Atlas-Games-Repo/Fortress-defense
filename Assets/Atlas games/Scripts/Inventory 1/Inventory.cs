using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ShopItemData data;
    public ItemPicker itemPicker;
    public InventorySlot[] magicSlotsUI;
    public InventorySlot[] itemSlotsUI;
    public InventorySlot[] petSlotsUI;
    public InventorySlot[] towerSlotsUI;
    [HideInInspector] public int[] chosenMagics;
    [HideInInspector] public int[] chosenItems;
    [HideInInspector] public int[] chosenPet;
    [HideInInspector] public int[] chosenTower;
    public int[] chosenInitialItemsID;
    public int[] chosenInitialMagicsID;
    public int[] chsenInitialPetsID;
    public int[] chosenInitialTowersID;
    private int _editingSlot;
    private Shop.ItemTypes _edittedType;
    void Start()
    {
        InitSlots();
    }

    public void InitSlots()
    {
        ShopItemData.ShopItem[] itemsData = data.ShopData;
        chosenMagics = new int[magicSlotsUI.Length];
        chosenItems = new int[itemSlotsUI.Length];
        chosenPet = new int[petSlotsUI.Length];
        chosenTower = new int[towerSlotsUI.Length];

        // give initial value to the free items;
        for (int i = 0; i < itemsData.Length; i++)
        {
            if (itemsData[i].isFree)
            {
                GlobalValue.IncrementChosenShopItem(itemsData[i].itemName);
            }
        }
        // decodes chosen items from player prefs global value and put them in slots
        string[] chosenMagicsDecode = GlobalValue.inventoryMagic.Split(',');
        for (int i = 0; i < chosenMagicsDecode.Length; i++)
        {
            chosenMagics[i] = int.Parse(chosenMagicsDecode[i]);
            for (int j = 0; j < itemsData.Length; j++)
            {
                if (chosenMagics[i] == itemsData[j].id)
                {
                    
                        magicSlotsUI[i].Init(itemsData[j].itemImage);
                    
                }
            }
        }
        
        string[] chosenItemsDecode = GlobalValue.inventoryItem.Split(',');
        for (int i = 0; i < chosenItemsDecode.Length; i++)
        {
            chosenItems[i] = int.Parse(chosenItemsDecode[i]);
            for (int j = 0; j < itemsData.Length; j++)
            {
                if (chosenItems[i] == itemsData[j].id)
                {
                    if (GlobalValue.GetChosenShopItem(itemsData[j].itemName) == 0)
                    {
                        if (FallBackItem().itemImage == null)
                        {
                            itemSlotsUI[i].chosenItemImage.gameObject.SetActive(false);
                        }
                        else
                        {
                            itemSlotsUI[i].chosenItemImage.gameObject.SetActive(true);
                            itemSlotsUI[i].Init(FallBackItem().itemImage);
                        }
                    }
                    else
                    {
                        itemSlotsUI[i].Init(itemsData[j].itemImage);
                    }
                }
            }
        }
        string[] chosenPetsDecode = GlobalValue.inventoryPets.Split(',');
        for (int i = 0; i < chosenPetsDecode.Length; i++)
        {
            chosenPet[i] = int.Parse(chosenPetsDecode[i]);
            for (int j = 0; j < itemsData.Length; j++)
            {
                if (chosenPet[i] == itemsData[j].id)
                {
                    
                        petSlotsUI[i].Init(itemsData[j].itemImage);
                    
                }
            }
        }
        string[] chosenTowerDecode = GlobalValue.inventoryTowers.Split(',');
        for (int i = 0; i < chosenTowerDecode.Length; i++)
        {
            chosenTower[i] = int.Parse(chosenTowerDecode[i]);
            for (int j = 0; j < itemsData.Length; j++)
            {
                if (chosenTower[i] == itemsData[j].id)
                {
                    
                    
                        towerSlotsUI[i].Init(itemsData[j].itemImage);
                    
                }
            }
        }
        
    }
    public void OpenPets()
    {
        SoundManager.Click();
        _editingSlot = 0;
        _edittedType = Shop.ItemTypes.Pet;
        itemPicker.gameObject.SetActive(true);
        itemPicker.Init(data,chosenPet,Shop.ItemTypes.Pet);
    }

    public void OpenItems(int slot)
    {
        SoundManager.Click();
        _editingSlot = slot;
        _edittedType = Shop.ItemTypes.Item;
        itemPicker.gameObject.SetActive(true);
        itemPicker.Init(data, chosenItems,Shop.ItemTypes.Item);
    }

    public void OpenMagics(int slot)
    {
        SoundManager.Click();
        _editingSlot = slot;
        _edittedType = Shop.ItemTypes.Magic;
        itemPicker.gameObject.SetActive(true);
        itemPicker.Init(data,chosenMagics,Shop.ItemTypes.Magic);
    }

    public void OpenTowers()
    {
        SoundManager.Click();
        _editingSlot = 0;
        _edittedType = Shop.ItemTypes.Towers;
        itemPicker.gameObject.SetActive(true);
        itemPicker.Init(data,chosenTower,Shop.ItemTypes.Towers);
    }

    public void CloseItemPicker()
    {
        itemPicker.ClearItems();
        itemPicker.gameObject.SetActive(false);
        SoundManager.Click();
    }
    public void ChangeChosenItem(ShopItemData.ShopItem item)
    {
        switch (_edittedType)
        {
            case Shop.ItemTypes.Item:
                itemSlotsUI[_editingSlot].ChangeSlotSprite(item.itemImage);
                chosenItems[_editingSlot] = item.id;
                GlobalValue.inventoryItem = string.Join(",", chosenItems);
                break;
            case Shop.ItemTypes.Magic:
                magicSlotsUI[_editingSlot].ChangeSlotSprite(item.itemImage);
                chosenMagics[_editingSlot] = item.id;
                GlobalValue.inventoryMagic = string.Join(",", chosenMagics);
                break;
            case Shop.ItemTypes.Pet:
                petSlotsUI[_editingSlot].ChangeSlotSprite(item.itemImage);
                chosenPet[_editingSlot] = item.id;
                GlobalValue.inventoryPets = string.Join(",", chosenPet);
                break;
            case Shop.ItemTypes.Towers:
                towerSlotsUI[_editingSlot].ChangeSlotSprite(item.itemImage);
                chosenTower[_editingSlot] = item.id;
                GlobalValue.inventoryTowers = string.Join(",", chosenTower);
                break;
        }
        CloseItemPicker();
    }


    
    // if item supply is consumed completely chooses a fall back item wich is free and unlimited and isn't chosen yet in the slots
    ShopItemData.ShopItem FallBackItem()
    {
        bool shouldBreak = false;
        int itemID = -1;
        for (int i = 0; i < chosenInitialItemsID.Length; i++)
        {
            for (int j = 0; j < chosenItems.Length; j++)
            {
                if (chosenItems[j] != chosenInitialItemsID[i] && GlobalValue.GetChosenShopItem(GetShopItem(chosenInitialItemsID[i]).itemName)>0)
                {
                    itemID = chosenInitialItemsID[i];
                    chosenItems[j] = chosenInitialItemsID[i];
                    shouldBreak = true;
                    GlobalValue.inventoryItem = string.Join(",", chosenItems);
                }
            }

            if (shouldBreak)
            {
                break;
            }
        }

        ShopItemData.ShopItem item = new ShopItemData.ShopItem(); 
        if (GetShopItem(itemID) == null)
        {
            item = GetShopItem(itemID);
        }
        else
        {
            item = new ShopItemData.ShopItem();
        }

        return item;
    }

    ShopItemData.ShopItem GetShopItem(int itemID)
    {
        ShopItemData.ShopItem item = null;
        for (int i = 0; i < data.ShopData.Length; i++)
        {
            if (data.ShopData[i].id == itemID)
            {
                item = data.ShopData[i];
            }
        }

        return item;
    }
}
