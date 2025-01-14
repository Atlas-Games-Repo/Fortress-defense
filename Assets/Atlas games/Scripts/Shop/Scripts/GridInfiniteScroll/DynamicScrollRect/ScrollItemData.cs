using System;

namespace DynamicScrollRect
{
    [Serializable]
    public class ScrollItemData
    {
        public ShopItemData.ShopItem Data { get; }

        public ScrollItemData(ShopItemData.ShopItem data)
        {
            Data =data;
        }
    }
}
