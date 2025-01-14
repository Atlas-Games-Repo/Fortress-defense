using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image chosenItemImage;
    private Vector2 originalSize;
    private bool _checkedSize;
    public void ChangeSlotSprite(Sprite itemSprite)
    {
        CheckSize();
        chosenItemImage.sprite = itemSprite;
        ResizeSprite();
    }

    void CheckSize()
    {
        if (!_checkedSize)
        {
            _checkedSize = true;
            originalSize =
                new Vector2(chosenItemImage.rectTransform.sizeDelta.x, chosenItemImage.rectTransform.sizeDelta.y);
        }
    }
    public void Init(Sprite initalSprite)
    {
      CheckSize();
        chosenItemImage.sprite = initalSprite;
        ResizeSprite();
    }

    void ResizeSprite()
    {
        
        chosenItemImage.SetNativeSize();
        if (chosenItemImage.rectTransform.sizeDelta.x > chosenItemImage.rectTransform.sizeDelta.y)
        {
            float aspectRatio = (float)chosenItemImage.rectTransform.sizeDelta.x / chosenItemImage.rectTransform.sizeDelta.y;
            chosenItemImage.rectTransform.sizeDelta = new Vector2(originalSize.x, originalSize.y / aspectRatio);
        }
        else
        {
            float aspectRatio = (float)chosenItemImage.rectTransform.sizeDelta.y / chosenItemImage.rectTransform.sizeDelta.x;
            chosenItemImage.rectTransform.sizeDelta = new Vector2(originalSize.x / aspectRatio, originalSize.y);
        }
    }
}
