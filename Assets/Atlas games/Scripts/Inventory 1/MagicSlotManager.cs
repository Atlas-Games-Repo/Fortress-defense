using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicSlotManager : MonoBehaviour
{
    private int[] _chosenMagics;
    public ShopItemData data;
    public AffectZoneButton[] slots;
    void Start()
    {
        _chosenMagics = new int[slots.Length];
        print(GlobalValue.inventoryMagic);
        string[] chosenMagicsDecode = GlobalValue.inventoryMagic.Split(',');
        for (int i = 0; i < chosenMagicsDecode.Length; i++)
        {
            _chosenMagics[i] = int.Parse(chosenMagicsDecode[i]);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].GetComponent<Image>().sprite = GetMagicData(_chosenMagics[i]).buttonImage;
            switch (GetMagicData(_chosenMagics[i]).itemName)
            {
                case "Lightning":
                    slots[i].affectType = AffectZoneType.Lighting;
                    break;
                case "Aero":
                    slots[i].affectType = AffectZoneType.Aero;
                    break;
                case "Poison":
                    slots[i].affectType = AffectZoneType.Poison;
                    break;
                case "Ice":
                    slots[i].affectType = AffectZoneType.Frozen;
                    break;
                case "Cure":
                    slots[i].affectType = AffectZoneType.Cure;
                    break;
                case "Magnet":
                    slots[i].affectType = AffectZoneType.Magnet;
                    break;
            }
        }
    }

    ShopItemData.ShopItem GetMagicData(int id)
    {
        ShopItemData.ShopItem chosenData = new ShopItemData.ShopItem();
        for (int i = 0; i < data.ShopData.Length; i++)
        {
            if (data.ShopData[i].id == id)
            {
                chosenData = data.ShopData[i];
            }
        }

        return chosenData;
    }
}
