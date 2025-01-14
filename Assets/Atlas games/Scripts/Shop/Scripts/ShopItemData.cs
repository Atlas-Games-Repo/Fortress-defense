using System;
using  UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "ScriptableObjects/ShopData", order = 1)]
public class ShopItemData : ScriptableObject
{
    
    [Serializable]
    public class ShopItem
    {
        public string itemName;
        public Sprite itemImage;
        public Sprite buttonImage;
        public int itemPrice;
        public int itemMaxValue;
        public bool hasMaxValue;
        public Shop.ItemTypes type;
        public bool isTimed;
        public Shop.ItemPurchaseType purchaseType;
        public bool levelLock;
        public int levelToUnlock;
        public int id;
        public bool isFree;
        public bool oneTimePurchase;
        public GameObject pet;
        public TimedItemManager.ItemDuration duration;
        public string[] otherTimedItems;
    }
    public ShopItem[] ShopData;
}
